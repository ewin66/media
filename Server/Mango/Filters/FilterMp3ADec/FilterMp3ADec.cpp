/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMp3ADec.cpp
 *
 * GENERAL DESCRIPTION: MPEG-1 Layer 3 Audio decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/07/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterMp3ADec.h"

#ifdef DSP
static int filterNum = 0;
#endif

// Constructor
void CFilterMp3ADec::SetDefaults()
{
	m_nNumInPins=1;
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_A_MPEG12L3);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_A_PCM);

	strcpy(m_strName, "CFilterMp3ADec");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
	m_pDecBuffer = NULL;
#endif
}

// Constructor
CFilterMp3ADec::CFilterMp3ADec()
{
	SetDefaults();
}

CFilterMp3ADec::CFilterMp3ADec(int nNumBufs, int nMaxOutputSize)
{
	m_nNumBufs = nNumBufs;
	m_nMaxOutputSize = nMaxOutputSize;
	SetDefaults();
}

// Destructor
CFilterMp3ADec::~CFilterMp3ADec()
{
#ifdef DSP
	if (m_isInited)
	{
		ALG_delete((IALG_Handle)m_sDecHandle);

		if (m_pDecBuffer)
			MEM_free(seg_sdram, m_pDecBuffer, ROUND_UP_CACHE_L2_LINESIZE(m_nDecBufferSize));

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free output buffer(s)
		for (int i=0; i<m_nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize));
		delete []p_outputBuffs;
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer
int CFilterMp3ADec::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_nMaxOutputSize);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_nMaxOutputSize);
	}

	return p-pBuff;
}

#ifdef DSP

// DSP side init
bool CFilterMp3ADec::Init()
{
    m_sDecParams = IMP3DECODER_PARAMS;
    m_fxns = &MP3DECODER_ITTIAM_IMP3DECODER;
	
	// Create MP3 decoder instance
	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	m_sDecHandle = (IMP3DECODER_Handle)ALG_create((IALG_Fxns*)m_fxns, NULL, (IALG_Params*)&m_sDecParams);
	SEM_post(&shared_scratch_SEM);
	if (!m_sDecHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}
	
	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	// Allocate output buffer(s) and submit to free mailbox
	stMediaBuffer sOutBuf;
	p_outputBuffs = new unsigned char*[m_nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	for (int i=0; i<m_nNumBufs; i++)
	{
		p_outputBuffs[i] = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize), CACHE_L2_LINESIZE);
		if (!p_outputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}
		sOutBuf.pBuffer = p_outputBuffs[i];
		sOutBuf.nMaxSize = m_nMaxOutputSize;
		sOutBuf.nSize = 0;
		if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
			return false;
		}
	}
	m_isInited=1;
	return true;
}

bool CFilterMp3ADec::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stacksize = 0x800;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

void CFilterMp3ADec::Stop()
{
	if (m_hTask)
	{
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		TSK_delete(m_hTask);
		SEM_post(&shared_scratch_SEM);
		m_hTask = NULL;
	}
}

// DSP Filter C-callable task function
void RunFilterTask(CFilterMp3ADec * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterMp3ADec::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	int buf_rd, buf_wr;
    IMP3DECODER_Status status;
	char errorDescriptionBuffer[256];
		
	while(true)
	{
		// receive new bitstream to decode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// receive empty buffer to copy decoded data into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
		sOutBuf.nSize = 0;

		if (!sInBuf.nSize) // empty buffer?
			goto skip;

		// prepare bitstream buffer.
		if (!m_pDecBuffer)
		{
			m_nDecBufferSize = (sOutBuf.nMaxSize * 2 > MIN_DEC_BUFF_SIZE ? sOutBuf.nMaxSize * 2 : MIN_DEC_BUFF_SIZE);
			m_pDecBuffer = (unsigned char*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nDecBufferSize), CACHE_L2_LINESIZE);
			if (!m_pDecBuffer)
			{
				report_error(ERRTYPE_WARNING, m_strName, "Can't allocate decoder bitstream buffer");
				return;
			}
			buf_rd = 0;
			buf_wr = 0;
		}

		memcpy(m_pDecBuffer + buf_wr, sInBuf.pBuffer, sInBuf.nSize);
		large_CACHE_invL2(sInBuf.pBuffer, sInBuf.nSize, CACHE_WAIT);
		large_CACHE_wbL2(m_pDecBuffer, m_nDecBufferSize, CACHE_WAIT);
		
		buf_wr += sInBuf.nSize;
		if (buf_wr > m_nDecBufferSize)
		{
			report_error(ERRTYPE_FATAL, m_strName, "Decoder bitstream buffer overflow");
			return;
		}

		large_CACHE_invL2(sOutBuf.pBuffer, m_nMaxOutputSize, CACHE_WAIT);

		while (buf_wr > buf_rd)
		{
			SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
			m_sDecHandle->fxns->mp3Decode(m_sDecHandle, m_pDecBuffer + buf_rd, (char*)sOutBuf.pBuffer + sOutBuf.nSize, buf_wr - buf_rd);
        	m_sDecHandle->fxns->ialg.algControl((IALG_Handle)m_sDecHandle, 
                        	IMP3DECODER_GETSTATUS, (IALG_Status *)&status);
			SEM_post(&shared_scratch_SEM);
			if (status.frameInfo.error == 1) // need more bits?
			{
				buf_rd += status.frameInfo.bytesConsumed;
				status.frameInfo.error = 1;
				break;
			}

			if(status.frameInfo.samples == -1 || status.frameInfo.error)
			{
				m_sDecHandle->fxns->mp3GetErrorDescription(m_sDecHandle, status.frameInfo.error, errorDescriptionBuffer);
				report_error((status.frameInfo.error < 0? ERRTYPE_FATAL : ERRTYPE_WARNING),
								m_strName, errorDescriptionBuffer);
			}

			buf_rd += status.frameInfo.bytesConsumed;
			sOutBuf.nSize += status.frameInfo.samples * sizeof(short);
		}

		if (buf_rd)
		{
			if (buf_wr > buf_rd)
				memmove(m_pDecBuffer, m_pDecBuffer+buf_rd, buf_wr-buf_rd);
			buf_wr -= buf_rd;
			buf_rd = 0;
		}

		if (sOutBuf.nSize >= sOutBuf.nMaxSize)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_buf_overflow);
			return;
		}

		large_CACHE_wbL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

skip:
		// send decoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free bitstream buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
