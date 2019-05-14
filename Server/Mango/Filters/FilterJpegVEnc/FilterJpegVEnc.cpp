/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterJpegVEnc.cpp
 *
 * GENERAL DESCRIPTION: (M)Jpeg video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/12/2005 		Kfir Yehezkel
 *
 ******************************************************************************/
#include "MangoBios.h"
#include "FilterJpegVEnc.h"
static int filterNum = 0;

void CFilterJpegVEnc::SetDefaults()
{
	m_nNumInPins = 2;
	m_nNumOutPins = 1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_PLANAR_420);
	m_pInPins[1].SetType(MT_JPEG_CMD);
	m_pInPins[1].SetRequired(false);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_JPEG);

	strcpy(m_strName, "CFilterJpegVEnc");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
	m_hCmdTask = NULL;
#endif
}

// Constructor
CFilterJpegVEnc::CFilterJpegVEnc()
{
	SetDefaults();
}

CFilterJpegVEnc::CFilterJpegVEnc(stJpegV_h2dparams& params)
{	
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterJpegVEnc::~CFilterJpegVEnc()
{
#ifdef DSP
	if (m_isInited)
	{
		// free encoder instance.
		MEM_free(seg_sdram, m_jpegencHandle, JPEGENC_INSTANCE_SIZE);

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
int CFilterJpegVEnc::Serialize(char *pBuff, bool bStore)
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

extern void* shared_isram;

// DSP side init

extern "C" void jpege_create(unsigned char*, void* isram);
extern "C" int jpege_encode(unsigned char*, unsigned char**, unsigned char*);
extern "C" void jpege_setparams(unsigned char*,IJPEGENC_MECOSO_Params*);

bool CFilterJpegVEnc::Init()
{
	// Make sure shared memory exists
	if (!shared_isram)
		if (!_ALG_allocMemory(NULL, 0))
		{
			report_error(ERRTYPE_ERROR, m_strName, "Can't allocate shared memory");
			return false;
		}

	// allocate room in SDRAM for instance.
	m_jpegencHandle = (unsigned char*) MEM_alloc(seg_sdram, JPEGENC_INSTANCE_SIZE, 8);
	if (!m_jpegencHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}
	
	m_encParams.size      = sizeof(IJPEGENC_MECOSO_Status);
	m_encParams.width     = m_sParams.width;
	m_encParams.height    = m_sParams.height;
	m_encParams.pitch     = m_sParams.pitch;	
	m_encParams.jfif      = m_sParams.jfif; // enable jfif header
	m_encParams.restart   = 0;			  // no restart
	m_encParams.quality   = m_sParams.quality;
	m_encParams.quant_tab = NULL;	
	m_encParams.format    = (int)m_sParams.format;
	if (m_encParams.format >= MX_JPG_FORMAT_NUM)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Invalid JPEG Format");
		return false;
	}
	else
	{
		m_encParams.format = (int) m_sParams.format;
	}

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	
	jpege_create(m_jpegencHandle, shared_isram);
	jpege_setparams(m_jpegencHandle, &m_encParams);

	SEM_post(&shared_scratch_SEM);
		
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

bool CFilterJpegVEnc::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	ta.name = m_strName;
	ta.stacksize = 2048; // as to requirement spec of ijpeg enc
	
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
		sprintf(m_cmdStrName, "CFilterJpegVEnc_Cmd_%04x", filterNum);
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

void CFilterJpegVEnc::Stop()
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
void RunFilterTask(CFilterJpegVEnc * pFilter)
{
	pFilter->FilterTask();
}

void RunFilterCmdTask(CFilterJpegVEnc * pFilter)
{
	pFilter->FilterCmdTask();
}

// DSP Filter task
void CFilterJpegVEnc::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	unsigned char* in_ptr[3];
	
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
		if (m_sParams.format != MX_JPG_YUV400)
		{
			in_ptr[1] = in_ptr[0] + m_sParams.width * m_sParams.height;
			if (m_sParams.format == MX_JPG_YUV420)
				in_ptr[2] = in_ptr[1] + m_sParams.width * m_sParams.height / 4;
			else  // MX_JPG_YUV422
				in_ptr[2] = in_ptr[1] + m_sParams.width * m_sParams.height / 2;
		}

		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	
		// call the encoder
		sOutBuf.nSize =
			jpege_encode(m_jpegencHandle, in_ptr, (unsigned char*)sOutBuf.pBuffer);

		SEM_post(&shared_scratch_SEM);

		if (sOutBuf.nSize < 0)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_buf_overflow);
			return;
		}

skip:
		// send encoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free image buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

// DSP command task
void CFilterJpegVEnc::FilterCmdTask()
{
	stMediaBuffer sInBuf;
	stJpegVEncCmd* jpg_cmd;

	while(true)
	{
		MBX_pend(m_pInPins[1].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		jpg_cmd = (stJpegVEncCmd*)sInBuf.pBuffer;
		Reopen(jpg_cmd->params);

		MBX_post(m_pInPins[1].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

// Reopen function
void CFilterJpegVEnc::Reopen(stJpegV_h2dparams& params)
{
	if (!m_isInited)
		return;

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	m_sParams = params;
	m_encParams.width     = m_sParams.width;
	m_encParams.height    = m_sParams.height;
	m_encParams.pitch     = m_sParams.pitch;
	m_encParams.jfif      = m_sParams.jfif;
	m_encParams.quality   = m_sParams.quality;
	m_encParams.format	  = m_sParams.format;

	jpege_create(m_jpegencHandle, shared_isram);
	jpege_setparams(m_jpegencHandle, &m_encParams);
	SEM_post(&shared_scratch_SEM);
}

#endif // #ifdef DSP
