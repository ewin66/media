/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg4VEnc.cpp
 *
 * GENERAL DESCRIPTION: Mpeg-4 video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "MangoBios.h"
#include "FilterMpeg4VEnc.h"

#ifdef DSP
static int filterNum = 0;
static char max_fps_tvsystem[] = {30, 25};

extern short x_sizes[MX_NUM_IMGSIZES];
extern short y_sizes[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES];

#endif // #ifdef DSP

void CFilterMpeg4VEnc::SetDefaults()
{
	m_nNumInPins=2; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_PLANAR_420);
	m_pInPins[1].SetType(MT_MP4V_CMD);
	m_pInPins[1].SetRequired(false);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_MPEG4);

	strcpy(m_strName, "CFilterMpeg4VEnc");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
	m_hCmdTask = NULL;
#endif
}

// Constructor
CFilterMpeg4VEnc::CFilterMpeg4VEnc()
{
	SetDefaults();
}

CFilterMpeg4VEnc::CFilterMpeg4VEnc(stMp4V_h2dparams& params)
{
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterMpeg4VEnc::~CFilterMpeg4VEnc()
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
int CFilterMpeg4VEnc::Serialize(char *pBuff, bool bStore)
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
bool CFilterMpeg4VEnc::Init()
{
	// Create Mpeg-4 encoder instance
	MP4SPENC_Params	encParams = MP4SPENC_PARAMS;
	encParams.qmax = m_sParams.nQmax;
	encParams.qinitial = m_sParams.nQinitial;
	encParams.numInter = m_sParams.nNumGOPFrames;
	encParams.RC_mode = m_sParams.eRC_mode;
	encParams.Constant_br = m_sParams.nConstantBitrate;
	encParams.Averaged_br = m_sParams.nAvgBitrate;
	encParams.Maximum_br = m_sParams.nMaxBitrate;
	encParams.bufSize = m_sParams.nCodecBufsize;
	encParams.srcFormat = -1; // define width and height directly
	encParams.Base_FrameRate = max_fps_tvsystem[m_sParams.eVideoStandard];
	encParams.Divisor_FrameRate = m_sParams.nFramerateDivisor;
	if (m_sParams.eImageSize != MX_CUSTOMSIZE)
	{
		encParams.Width = x_sizes[m_sParams.eImageSize];
		encParams.Height = y_sizes[m_sParams.eVideoStandard][m_sParams.eImageSize];
	}
	else
	{
		encParams.Width = m_sParams.nCustomWidth;
		encParams.Height = m_sParams.nCustomHeight;
	}
	m_nMsPerFrame = (1000 * encParams.Divisor_FrameRate) / encParams.Base_FrameRate;

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
    m_sEncHandle = (MP4SPENC_Handle)ALG_create((IALG_Fxns *)&MP4SPENC_PRS_IMP4SPENC, NULL, (IALG_Params *)&encParams);
	SEM_post(&shared_scratch_SEM);
	if (!m_sEncHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}
	m_nMp4First = 1;

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
	m_isInited = 1;
	return true;
}

bool CFilterMpeg4VEnc::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	ta.name = m_strName;
	ta.stacksize = 0x700;
	
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
		sprintf(m_cmdStrName, "CFilterMpegVEnc_Cmd_%04x", filterNum);
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

void CFilterMpeg4VEnc::Stop()
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
void RunFilterTask(CFilterMpeg4VEnc * pFilter)
{
	pFilter->FilterTask();
}

void RunFilterCmdTask(CFilterMpeg4VEnc * pFilter)
{
	pFilter->FilterCmdTask();
}

// DSP Filter task
void CFilterMpeg4VEnc::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	Uint8* pEncBuf;
	m_nSkipCnt = 0;
	
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
			
		pEncBuf = (Uint8*)sOutBuf.pBuffer;

		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);

		if (!m_sEncHandle) // can happen in case of error during Reopen()
		{
			sOutBuf.nSize = 0;
			goto skip;
		}

		if (m_nMp4First)
		{
			m_nMp4First = 0;
			m_nSkipRC = MP4SPENC_PRS_encodeFirst(m_sEncHandle, (Uint8*)sInBuf.pBuffer, &pEncBuf, &m_sClient);

			//Initialize client params for encodeFrame.
			if ((m_sParams.eRC_mode==RC_CBR) || (m_sParams.eRC_mode==RC_CBR_NON_DROP))
			{
				m_sClient.aux4 = (void *)m_sParams.nConstantBitrate;
				m_sClient.aux7 = (void*)m_sParams.nQmin;
			}
			else if ((m_sParams.eRC_mode==RC_VBR)||(m_sParams.eRC_mode==RC_VBR_NON_DROP))
			{
				m_sClient.aux4 = (void *)m_sParams.nAvgBitrate;
				m_sClient.aux7 = (void*)m_sParams.nQmin;
			}
			m_sClient.aux8 = (void *)m_sParams.nQmax;
			m_sClient.aux9 = (void *)m_sParams.nFramerateDivisor;
			m_bForceIFrame = 0;
		}
		else
		{
			m_sClient.aux5 = (void*)m_bForceIFrame; // force I-Frame if requested
#ifdef MP4ENC_IGNORE_FRAMESKIP
			m_nSkipRC = MP4SPENC_PRS_encodeFrame(m_sEncHandle, (Uint8*)sInBuf.pBuffer, &pEncBuf, 
					(1 + m_nSkipRC) * m_nMsPerFrame, &m_sClient);
#else
			if (m_nSkipCnt)
				m_nSkipCnt--;
			else
			{
				m_nSkipRC = MP4SPENC_PRS_encodeFrame(m_sEncHandle, (Uint8*)sInBuf.pBuffer, &pEncBuf, 
						(1 + m_nSkipRC) * m_nMsPerFrame, &m_sClient);
				m_nSkipCnt = m_nSkipRC;
			}
#endif
			m_bForceIFrame = 0;
		}
		SEM_post(&shared_scratch_SEM);

		sOutBuf.nSize = pEncBuf-(Uint8*)sOutBuf.pBuffer;

		// to know what kind of frame encoded: (m_sClient.aux1? PFRAME : IFRAME);

		if (sOutBuf.nSize >= sOutBuf.nMaxSize)
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
void CFilterMpeg4VEnc::FilterCmdTask()
{
	stMediaBuffer sInBuf;
	stMpeg4VEncCmd* mp4_cmd;

	while(true)
	{
		MBX_pend(m_pInPins[1].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		mp4_cmd = (stMpeg4VEncCmd*)sInBuf.pBuffer;
		if (mp4_cmd->cmd == MP4CMD_REOPEN)
			Reopen(mp4_cmd->params);
		else
		{
			SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
			m_bForceIFrame = 1;
			SEM_post(&shared_scratch_SEM);
		}

		MBX_post(m_pInPins[1].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

// Reopen function
void CFilterMpeg4VEnc::Reopen(stMp4V_h2dparams& params)
{
	if (!m_isInited)
		return;

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	// Delete old instance
	ALG_delete((IALG_Handle)m_sEncHandle);

	// Get new paramters
	m_sParams = params;
	MP4SPENC_Params	encParams = MP4SPENC_PARAMS;
	encParams.qmax = m_sParams.nQmax;
	encParams.qinitial = m_sParams.nQinitial;
	encParams.numInter = m_sParams.nNumGOPFrames;
	encParams.RC_mode = m_sParams.eRC_mode;
	encParams.Constant_br = m_sParams.nConstantBitrate;
	encParams.Averaged_br = m_sParams.nAvgBitrate;
	encParams.Maximum_br = m_sParams.nMaxBitrate;
	encParams.bufSize = m_sParams.nCodecBufsize;
	encParams.srcFormat = -1; // define width and height directly
	encParams.Base_FrameRate = max_fps_tvsystem[m_sParams.eVideoStandard];
	encParams.Divisor_FrameRate = m_sParams.nFramerateDivisor;
	if (m_sParams.eImageSize != MX_CUSTOMSIZE)
	{
		encParams.Width = x_sizes[m_sParams.eImageSize];
		encParams.Height = y_sizes[m_sParams.eVideoStandard][m_sParams.eImageSize];
	}
	else
	{
		encParams.Width = m_sParams.nCustomWidth;
		encParams.Height = m_sParams.nCustomHeight;
	}
	m_nMsPerFrame = (1000 * encParams.Divisor_FrameRate) / encParams.Base_FrameRate;

    m_sEncHandle = (MP4SPENC_Handle)ALG_create((IALG_Fxns *)&MP4SPENC_PRS_IMP4SPENC, NULL, (IALG_Params *)&encParams);
	if (!m_sEncHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		SEM_post(&shared_scratch_SEM);
		return;
	}
	m_nMp4First = 1;
	SEM_post(&shared_scratch_SEM);
}

#endif // #ifdef DSP
