/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg4VDec.cpp
 *
 * GENERAL DESCRIPTION: Mpeg-4 video decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "MangoBios.h"
#include "FilterMpeg4VDec.h"

#ifdef DSP
#include "MangoAV_HWExp.h"
static int filterNum = 0;
#endif

// Constructor
void CFilterMpeg4VDec::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_MPEG4);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_PLANAR_420);

	strcpy(m_strName, "CFilterMpeg4VDec");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}
// Constructor
CFilterMpeg4VDec::CFilterMpeg4VDec()
{
	SetDefaults();
}

CFilterMpeg4VDec::CFilterMpeg4VDec(int nNumBufs, bool bFastMode)
{
	m_nNumBufs = nNumBufs;
	m_bFastMode = bFastMode;

	SetDefaults();
}

// Destructor
CFilterMpeg4VDec::~CFilterMpeg4VDec()
{
#ifdef DSP
	if (m_isInited)
	{
		ALG_delete((IALG_Handle)m_sDecHandle);

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free decode output buffers (created only not in fast mode)
		if (!m_bFastMode)
		{
			if (m_pCurrent)
				MEM_free(seg_sdram, m_pCurrent, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize));
			if (m_pPrevious)
				MEM_free(seg_sdram, m_pPrevious, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize));
		}

		// Free output buffer(s)
		for (int i=0; i<m_nNumBufs; i++)
			if (p_outputBuffs[i])
				MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize));
		delete []p_outputBuffs;
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer
int CFilterMpeg4VDec::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_bFastMode);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_bFastMode);
	}

	return p-pBuff;
}

#ifdef DSP

// DSP side init
bool CFilterMpeg4VDec::Init()
{
	// Create Mpeg-4 decoder instance
	m_sInfoFirst = MP4SPDEC_INFOFIRST;
	m_sInfoFrame = MP4SPDEC_INFOFRAME;
	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
    m_sDecHandle = (MP4SPDEC_Handle)ALG_create((IALG_Fxns *)&MP4SPDEC_PRS_IMP4SPDEC, NULL, (IALG_Params *)&MP4SPDEC_PARAMS);
	SEM_post(&shared_scratch_SEM);

	if (!m_sDecHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}
	m_nMp4First = 1;

	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	p_outputBuffs = new unsigned char*[m_nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}
	memset(p_outputBuffs, 0, m_nNumBufs * sizeof(unsigned char*));

	m_pCurrent = NULL;
	m_pPrevious = NULL;
	m_isInited=1;
	return true;
}

bool CFilterMpeg4VDec::Start()
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

void CFilterMpeg4VDec::Stop()
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
void RunFilterTask(CFilterMpeg4VDec * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterMpeg4VDec::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	unsigned char* pDecBuf, *temp;

	while(true)
	{
		// receive new bitstream to decode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
		{
			sOutBuf.nSize = 0;
			sOutBuf.pBuffer = NULL;
			MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
			MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
	
			goto skip;
		}

		pDecBuf = (Uint8*)sInBuf.pBuffer;
		if (m_nMp4First)
		{
			SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
			// Decode headers.
			pDecBuf = (unsigned char*)MP4SPDEC_PRS_sincroIntra(m_sDecHandle, pDecBuf, sInBuf.nSize);
			SEM_post(&shared_scratch_SEM);
			if (!pDecBuf)
			{
				report_error(ERRTYPE_ERROR, m_strName, "Decode error (sincroIntra)");
				sOutBuf.nSize = 0;
				sOutBuf.pBuffer = NULL;
				MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
				MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
				goto skip;
			}
			SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
			MP4SPDEC_PRS_initializeSequence(m_sDecHandle, pDecBuf, &m_sInfoFirst, &pDecBuf);
			SEM_post(&shared_scratch_SEM);
			m_nMp4First = 0;
			
			m_nMaxOutputSize = m_sInfoFirst.width * m_sInfoFirst.height * 3/2;
			
			if (!m_bFastMode)
			{
				// Allocate two frame buffers (current and reference frame)
				m_pCurrent = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize), CACHE_L2_LINESIZE); // alignment is Prodys spec (0x80)
				m_pPrevious = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize), CACHE_L2_LINESIZE);
				if (!m_pCurrent || !m_pPrevious)
				{
					report_error(ERRTYPE_ERROR, m_strName, "Can't allocate decoder buffers");
					return;
				}
			}

			// Allocate output buffer(s) and submit to free mailbox
			for (int i=0; i<m_nNumBufs; i++)
			{
				p_outputBuffs[i] = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize), CACHE_L2_LINESIZE);
				if (!p_outputBuffs[i])
				{
					report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
					return;
				}
				sOutBuf.pBuffer = p_outputBuffs[i];
				sOutBuf.nMaxSize = m_nMaxOutputSize;
				sOutBuf.nSize = 0;
				if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
				{
					report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
					return;
				}
			}
		}

		// receive empty buffer to decode into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
//		CLEAN_CACHE
		if (m_bFastMode)
			m_pCurrent = (Uint8*)sOutBuf.pBuffer;

		do
		{
			SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
			if (pDecBuf)
				MP4SPDEC_PRS_decodeFrame(m_sDecHandle, &m_sInfoFrame, pDecBuf, &pDecBuf, m_pPrevious, m_pCurrent);
			SEM_post(&shared_scratch_SEM);

			if (!pDecBuf) // || s->infoFrame.errorFlag)
			{
				report_error(ERRTYPE_ERROR, m_strName, "Decode error (decodeFrame)");
				sOutBuf.nSize = 0;
				goto send;
			}
			
		} while (m_sInfoFrame.skips); // decode again if necessary

		sOutBuf.nSize = m_nMaxOutputSize;

		if (m_bFastMode)
			large_CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
		else
		{
			qdma_memcpy(sOutBuf.pBuffer, m_pCurrent, m_nMaxOutputSize);

			// to save time, perform this cache_invL2 while transfer is occurring
			large_CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

			qdma_memcpy_wait();
		}

		temp = m_pPrevious;
		m_pPrevious = m_pCurrent;
		m_pCurrent = temp;
send:
		// send decoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
skip:
		// free bitstream buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
