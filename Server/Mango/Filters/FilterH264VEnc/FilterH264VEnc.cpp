/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH264VEnc.cpp
 *
 * GENERAL DESCRIPTION: H.264 video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 11/12/2005 		Kfir Yehezkel
 *
 ******************************************************************************/
#include "FilterH264VEnc.h"

#ifdef DSP
static int filterNum = 0;

extern short x_sizes[MX_NUM_IMGSIZES];
extern short y_sizes[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES];
#endif

void CFilterH264VEnc::SetDefaults()
{
	m_nNumInPins = 2; 
	m_nNumOutPins = 1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_PLANAR_420);
	m_pInPins[1].SetType(MT_H264V_CMD);
	m_pInPins[1].SetRequired(false);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_H264);

	strcpy(m_strName, "CFilterH264VEnc");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
	m_hCmdTask = NULL;
#endif
}

// Constructor
CFilterH264VEnc::CFilterH264VEnc()
{
	SetDefaults();
}

// load the parameters
CFilterH264VEnc::CFilterH264VEnc(stH264V_h2dparams& params)
{		
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterH264VEnc::~CFilterH264VEnc()
{
#ifdef DSP
	if (m_isInited)
	{
		// free encoder instance.
		if (m_h264encHandle)
			ALG_delete((IALG_Handle) m_h264encHandle);

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free output buffer(s)
		for (int i = 0 ; i < m_sParams.nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_sParams.nMaxOutputSize));
		
		delete []p_outputBuffs;
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer
int CFilterH264VEnc::Serialize(char *pBuff, bool bStore)
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

// default params of encoder - do not change it.
extern H264ENC_Params IH264ENC_PARAMS;
extern far IH264ENC_Fxns H264ENC_WW_IH264ENC;

bool CFilterH264VEnc::Init()
{
	H264ENC_Params encParams = IH264ENC_PARAMS;

	if (m_sParams.eImageSize != MX_CUSTOMSIZE)
	{
		m_sParams.nCustomWidth = x_sizes[m_sParams.eImageSize];
		m_sParams.nCustomHeight = y_sizes[m_sParams.eVideoStandard][m_sParams.eImageSize];
	}

	encParams.img_width = m_sParams.nCustomWidth;
	encParams.img_height = m_sParams.nCustomHeight;
	encParams.iFrameRate = m_sParams.nFrameRate;
	encParams.iBitrate = m_sParams.nBitrate/1024;
	encParams.qp0 = m_sParams.nQp0;
	encParams.qpN = m_sParams.nQpN;
//	encParams.search_range = m_sParams.nSearchRange;
	encParams.intra_period = m_sParams.nNumGOPFrames;
//	encParams.num_reference_frames = m_sParams.nNumRefrenceFrames;

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);

	m_h264encHandle =
			(H264ENC_Handle) ALG_create ((IALG_Fxns *) &H264ENC_WW_IH264ENC, NULL, (IALG_Params *) &encParams);	
	SEM_post(&shared_scratch_SEM);
	if (!m_h264encHandle)
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

	// Allocate output buffer(s)
	stMediaBuffer sOutBuf;
	p_outputBuffs = new unsigned char*[m_sParams.nNumBufs];
	for (int i = 0; i < m_sParams.nNumBufs ; i++)
	{
		p_outputBuffs[i] = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_sParams.nMaxOutputSize), CACHE_L2_LINESIZE);
		if (!p_outputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}
		
		// submit to free mailbox
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

bool CFilterH264VEnc::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	ta.name = m_strName;
	ta.stackseg = seg_isram;
	ta.stacksize = 0x1000;
	
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	if (m_pInPins[1].IsConnected())
	{
		ta = TSK_ATTRS;
		sprintf(m_cmdStrName, "CFilterH264VEnc_Cmd_%04x", filterNum);
		ta.name = m_cmdStrName;
		m_hCmdTask = TSK_create((Fxn)RunFilterCmdTask, &ta, this);
		if (!m_hCmdTask)
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
			return false;
		}
		shuffle_task_list();
	}
	m_nFilterNum = filterNum++;

	return true;
}

void CFilterH264VEnc::Stop()
{
	if (m_hTask)
	{
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		TSK_delete(m_hTask);
		SEM_post(&shared_scratch_SEM);
		m_hTask = NULL;
	}
	if (m_hCmdTask)
	{
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		TSK_delete(m_hCmdTask);
		SEM_post(&shared_scratch_SEM);
		m_hCmdTask = NULL;
	}
}

// DSP Filter C-callable task function
void RunFilterTask(CFilterH264VEnc * pFilter)
{
	pFilter->FilterTask();
}

void RunFilterCmdTask(CFilterH264VEnc * pFilter)
{
	pFilter->FilterCmdTask();
}

// DSP Filter task
void CFilterH264VEnc::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	unsigned char* in_ptr[3];
	int frame_num = 0;
	
	while(true)
	{
		// receive new image to encode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// receive empty buffer to encode into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
		{
			sOutBuf.nSize = 0;
			goto skip;
		}

		in_ptr[0] = (unsigned char*) sInBuf.pBuffer;
		in_ptr[1] = in_ptr[0] + m_sParams.nCustomWidth * m_sParams.nCustomHeight;
		in_ptr[2] = in_ptr[1] + m_sParams.nCustomWidth * m_sParams.nCustomHeight /4;

		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		if (!m_h264encHandle) // if Reopen failed (this must be checked within critical sec)
		{
			sOutBuf.nSize = 0;
			SEM_post(&shared_scratch_SEM);
			goto skip;
		}
	
		ALG_activate((IALG_Handle) m_h264encHandle);
		
		if(m_h264encHandle && m_h264encHandle->fxns->encodeFrame)
			m_h264encHandle->fxns->encodeFrame(m_h264encHandle , in_ptr, (unsigned char *)sOutBuf.pBuffer, frame_num++);

		ALG_deactivate((IALG_Handle)m_h264encHandle);
		
		// getting the status
		if (m_h264encHandle && m_h264encHandle->fxns->control)
			m_h264encHandle->fxns->control(m_h264encHandle, IH264ENC_GETSTATUS, &m_h264encstatus);

		SEM_post(&shared_scratch_SEM);
		
		sOutBuf.nSize = m_h264encstatus.nBits / 8;

		if (sOutBuf.nSize < 0)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_buf_overflow);
			return;
		}

		large_CACHE_wbL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
		large_CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

skip:
		// send encoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free image buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

// DSP command task
void CFilterH264VEnc::FilterCmdTask()
{
	stMediaBuffer sInBuf;
	stH264VEncCmd* h264_cmd;

	while(true)
	{
		MBX_pend(m_pInPins[1].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		h264_cmd = (stH264VEncCmd*)sInBuf.pBuffer;
		if (h264_cmd->cmd == H264CMD_REOPEN)
			Reopen(h264_cmd->params);
		else
			Reopen(m_sParams); // Force I-Frame by reopening with current parameters

		MBX_post(m_pInPins[1].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

// Reopen function
void CFilterH264VEnc::Reopen(stH264V_h2dparams& params)
{
	if (!m_isInited)
		return;

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);

	// free encoder instance.
	ALG_delete((IALG_Handle) m_h264encHandle);

	m_sParams = params;

	// create new encoder instance
	H264ENC_Params encParams = IH264ENC_PARAMS;

	if (m_sParams.eImageSize != MX_CUSTOMSIZE)
	{
		m_sParams.nCustomWidth = x_sizes[m_sParams.eImageSize];
		m_sParams.nCustomHeight = y_sizes[m_sParams.eVideoStandard][m_sParams.eImageSize];
	}

	encParams.img_width = m_sParams.nCustomWidth;
	encParams.img_height = m_sParams.nCustomHeight;
	encParams.iFrameRate = m_sParams.nFrameRate;
	encParams.iBitrate = m_sParams.nBitrate/1024;
	encParams.qp0 = m_sParams.nQp0;
	encParams.qpN = m_sParams.nQpN;
	encParams.intra_period = m_sParams.nNumGOPFrames;

	m_h264encHandle =
			(H264ENC_Handle) ALG_create ((IALG_Fxns *) &H264ENC_WW_IH264ENC, NULL, (IALG_Params *) &encParams);
	if (!m_h264encHandle)
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);

	SEM_post(&shared_scratch_SEM);
}

#endif // #ifdef DSP
