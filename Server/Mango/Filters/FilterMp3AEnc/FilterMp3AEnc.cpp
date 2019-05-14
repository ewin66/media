/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMp3AEnc.cpp
 *
 * GENERAL DESCRIPTION: MPEG-1 Layer 3 Audio Encoding Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/07/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterMp3AEnc.h"

#ifdef DSP
static int filterNum = 0;
#endif

void CFilterMp3AEnc::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_A_PCM);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_A_MPEG12L3);

	strcpy(m_strName, "CFilterMp3AEnc");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Constructor
CFilterMp3AEnc::CFilterMp3AEnc()
{
	SetDefaults();
}

CFilterMp3AEnc::CFilterMp3AEnc(stMP3A_h2dparams& params)
{
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterMp3AEnc::~CFilterMp3AEnc()
{
#ifdef DSP
	if (m_isInited)
	{
		ALG_delete((IALG_Handle)m_sEncHandle);

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free output buffer(s)
		for (int i=0; i<m_sParams.nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_sParams.nMaxOutputSize));
		delete []p_outputBuffs;
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer
int CFilterMp3AEnc::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_sParams);
	}
	else
	{
		DESERIALIZE(p, m_sParams);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init
bool CFilterMp3AEnc::Init()
{
    m_sEncParams = IMP3ENC_PARAMS;
    m_fxns   = &MP3ENC_ITTIAM_IMP3ENC;

    m_sEncParams.bitRate     = m_sParams.nBitrate;
    m_sEncParams.numChannels = m_sParams.nIsStereo? 2 : 1;
    m_sEncParams.samplingFreq = m_sParams.nSampleFreq;
    
	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	m_sEncHandle = (IMP3ENC_Handle)ALG_create((IALG_Fxns*)m_fxns, NULL, (IALG_Params*)&m_sEncParams);
	SEM_post(&shared_scratch_SEM);
	if (!m_sEncHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}

	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_sParams.nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_sParams.nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	// Allocate output buffer(s) and submit to free mailbox
	stMediaBuffer sOutBuf;
	p_outputBuffs = new unsigned char*[m_sParams.nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}
	
	for (int i=0; i<m_sParams.nNumBufs; i++)
	{
		p_outputBuffs[i] = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_sParams.nMaxOutputSize), CACHE_L2_LINESIZE);
		if (!p_outputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}
		sOutBuf.pBuffer = p_outputBuffs[i];
		sOutBuf.nMaxSize = m_sParams.nMaxOutputSize;
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

bool CFilterMp3AEnc::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stackseg = seg_sdram;
	ta.stacksize = 0x1200;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

void CFilterMp3AEnc::Stop()
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
void RunFilterTask(CFilterMp3AEnc * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterMp3AEnc::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	int numCodedSamples;
	
	while(true)
	{
		// receive new audio to encode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// receive empty buffer to encode into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
		{
			sOutBuf.nSize = 0;
			goto skip;
		}

		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		numCodedSamples = m_sEncHandle->fxns->mp3Encoder(m_sEncHandle, sInBuf.pBuffer, sOutBuf.pBuffer, 
														sInBuf.nSize/m_sEncParams.numChannels/sizeof(short));
		SEM_post(&shared_scratch_SEM);

		if (numCodedSamples == -1)
		{
			report_error(ERRTYPE_FATAL, m_strName, "Error in input parameters");
			return;
		}
		
		sOutBuf.nSize = numCodedSamples;

		if (sOutBuf.nSize >= sOutBuf.nMaxSize)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_buf_overflow);
			return;
		}
		CACHE_wbInvL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

skip:		
		// send encoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free image buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
