/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMp2AEnc.cpp
 *
 * GENERAL DESCRIPTION: MPEG-1 Layer 2 Audio Encoding Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 22/01/06 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterMp2AEnc.h"

#ifdef DSP
static int filterNum = 0;
#endif

void CFilterMp2AEnc::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_A_PCM);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_A_MPEG12L3);

	strcpy(m_strName, "CFilterMp2AEnc");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Constructor
CFilterMp2AEnc::CFilterMp2AEnc()
{
	SetDefaults();
}

CFilterMp2AEnc::CFilterMp2AEnc(stMP2A_h2dparams& params)
{
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterMp2AEnc::~CFilterMp2AEnc()
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
int CFilterMp2AEnc::Serialize(char *pBuff, bool bStore)
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
bool CFilterMp2AEnc::Init()
{
    m_sEncParams = IMP3ENC5064L2_PARAMS;
    m_fxns   = &MP3ENC5064L2_CUTE_IMP3ENC5064L2;

    m_sEncParams.samplingFrequency = m_sParams.nSampleFreq;
    m_sEncParams.bitrate = m_sParams.nBitrate;
    m_sEncParams.mode = m_sParams.enMode;
    
	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	m_sEncHandle = (MP3ENC5064L2_Handle)ALG_create((IALG_Fxns*)m_fxns, NULL, (IALG_Params*)&m_sEncParams);
	SEM_post(&shared_scratch_SEM);
	if (!m_sEncHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}

	if(MP3ENC5064L2_encParamsValidityCheck(m_sEncHandle))
	{
		report_error(ERRTYPE_ERROR, m_strName, "Parameters fail validity check!");
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

bool CFilterMp2AEnc::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stackseg = seg_sdram;
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

void CFilterMp2AEnc::Stop()
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
void RunFilterTask(CFilterMp2AEnc * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterMp2AEnc::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	
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
		MP3ENC5064L2_encodeFrame(m_sEncHandle, sInBuf.pBuffer, sOutBuf.pBuffer, &sOutBuf.nSize);
		SEM_post(&shared_scratch_SEM);

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
