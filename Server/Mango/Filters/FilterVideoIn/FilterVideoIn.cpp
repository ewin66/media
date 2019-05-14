/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterVideoIn.cpp
 *
 * GENERAL DESCRIPTION: Analog source video input filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 31/03/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterVideoIn.h"

#ifdef DSP
#include "MangoAV_SharedExp.h"
#include "MangoAV_HWExp.h"
static int filterNum = 0;

extern short x_sizes[MX_NUM_IMGSIZES];
extern short y_sizes[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES];

static Image_Sizes_T image_sizes[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES] =
{
	{
		(Image_Sizes_T)-1,	// MX_SQCIF
		MANGO_AV_QCIF_NTSC,	// MX_QCIF
		MANGO_AV_CIF_NTSC,	// MX_CIF
		MANGO_AV_2CIF_NTSC,	// MX_2CIF
		MANGO_AV_4CIF_INTERLEAVED_NTSC,	// MX_4CIF
		MANGO_AV_SIF_NTSC,	// MX_QVGA 
		MANGO_AV_VGA_NTSC	// MX_VGA
	},
	{
		(Image_Sizes_T)-1,	// MX_SQCIF
		MANGO_AV_QCIF_PAL,	// MX_QCIF,
		MANGO_AV_CIF_PAL,	// MX_CIF,
		MANGO_AV_2CIF_PAL,	// MX_2CIF,
		MANGO_AV_4CIF_INTERLEAVED_PAL,	// MX_4CIF,
		MANGO_AV_SIF_NTSC,	// MX_QVGA
		MANGO_AV_VGA_NTSC	// MX_VGA
	}
};

#endif

// This function is called by all constructors.
void CFilterVideoIn::SetDefaults()
{
	// Set number of input and output pins for this filter here.
	m_nNumInPins=0; 
	m_nNumOutPins=2;

	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	
	// Set the allowed input type and the output type pins.
	m_pOutPins[0].SetType(MT_V_PLANAR_420);
	m_pOutPins[1].SetType(MT_TIME_TAG);
	m_pOutPins[1].SetRequired(false);

	// This should be the same as the class's name
	strcpy(m_strName, "CFilterVideoIn");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterVideoIn::CFilterVideoIn()
{
	SetDefaults();
}

CFilterVideoIn::CFilterVideoIn(int nNumBufs, int nCamNum,
							   enImageSize eImageSize,
							   enVideoStandard eVideoStandard,
							   enVideoAnalogConnection eVideoConn,
							   int nCamBufs)
{
	m_nNumBufs = nNumBufs;
	m_nCamNum = nCamNum;
	m_eImageSize = eImageSize;
	m_eVideoStandard = eVideoStandard;
	m_eVideoConn = eVideoConn;
	m_nCamBufs = nCamBufs;
	SetDefaults();
}

// Destructor
CFilterVideoIn::~CFilterVideoIn()
{
#ifdef DSP
	if (m_isInited)
	{
		MangoAV_HW_close_cam_input(m_nCamNum);
		
		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free output buffer(s)
		for (int i=0; i<m_nNumBufs; i++)
			MEM_free(seg_sdram, m_pOutputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_nOutputSize));
		delete []m_pOutputBuffs;

		if (m_pOutPins[1].IsConnected())
		{
			// Delete output mailboxes
			MBX_delete(m_pOutPins[1].GetFullBuffsMbx());
			MBX_delete(m_pOutPins[1].GetFreeBuffsMbx());
			delete []m_pTimeTag;
		}
	}
#endif
	delete []m_pOutPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterVideoIn::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_nCamNum);
		SERIALIZE(p, m_eImageSize);
		SERIALIZE(p, m_eVideoStandard);
		SERIALIZE(p, m_eVideoConn);
		SERIALIZE(p, m_nCamBufs);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_nCamNum);
		DESERIALIZE(p, m_eImageSize);
		DESERIALIZE(p, m_eVideoStandard);
		DESERIALIZE(p, m_eVideoConn);
		DESERIALIZE(p, m_nCamBufs);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init; called after constructor but before graph is connected
bool CFilterVideoIn::Init()
{
	// Open video input
	if (m_eImageSize == MX_QVGA || m_eImageSize == MX_VGA)
	{
		if (m_eVideoStandard != MX_NTSC)
		{
			report_error(ERRTYPE_ERROR, m_strName, "Can't use VGA size in PAL mode");
			return false;
		}
		m_eMAVVideoStandard = MANGO_AV_NTSC_VGA;
	}
	else
		m_eMAVVideoStandard = (m_eVideoStandard == MX_NTSC? MANGO_AV_NTSC : MANGO_AV_PAL);

	m_eMAVImageSize = image_sizes[m_eVideoStandard][m_eImageSize];
	m_eMAVConn = (m_eVideoConn == MX_S_VIDEO? S_VIDEO : COMPOSITE_VIDEO);

	if (m_eMAVImageSize == (Image_Sizes_T)-1)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Invalid image size");
		return false;
	}

	if (MangoAV_HW_open_cam_input(m_eMAVVideoStandard, m_eMAVConn,
									m_nCamNum, m_nCamBufs, MANGOBIOS_FOREVER) != MANGOERROR_SUCCESS)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Can't open video input");
		return false;
	}
	
	m_nOutputSize = x_sizes[m_eImageSize] * y_sizes[m_eVideoStandard][m_eImageSize] * 3/2;
	
	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	// Allocate output buffer(s) and submit to free mailbox.
	stMediaBuffer sOutBuf;
	m_pOutputBuffs = new unsigned char*[m_nNumBufs];
	if (!m_pOutputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	for (int i=0; i<m_nNumBufs; i++)
	{
		m_pOutputBuffs[i] = (unsigned char*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nOutputSize), CACHE_L2_LINESIZE);
		if (!m_pOutputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}
		sOutBuf.pBuffer = m_pOutputBuffs[i];
		sOutBuf.nMaxSize = m_nOutputSize;
		sOutBuf.nSize = 0;
		if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
			return false;
		}
	}

	// Setup optional output pin 1 (time tag output)
	if (m_pOutPins[1].GetDestPinId() != -1)
	{
		// Initialize output mailboxes
		m_pOutPins[1].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
		m_pOutPins[1].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
		if (!m_pOutPins[1].GetFullBuffsMbx() || !m_pOutPins[1].GetFreeBuffsMbx())
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
			return false;
		}

		// Allocate output buffer (just time tags) and submit to free mailbox.
		m_pTimeTag = new Time_tag_T[m_nNumBufs];
		if (!m_pTimeTag)
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
			return false;
		}

		for (int i=0; i<m_nNumBufs; i++)
		{
			sOutBuf.pBuffer = &m_pTimeTag[i];
			sOutBuf.nMaxSize = sizeof(Time_tag_T);
			sOutBuf.nSize = sizeof(Time_tag_T);
			if (!MBX_post(m_pOutPins[1].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
			{
				report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
				return false;
			}
		}
	}

	m_isInited=1;
	
	// Make sure shared memory exists
	if (!shared_isram)
		if (!_ALG_allocMemory(NULL, 0))
		{
			report_error(ERRTYPE_ERROR, m_strName, "Can't allocate shared memory");
			return false;
		}
			
	return true;
}

// This function is called to indicate to filter to start operating. At this point the
// graph is fully connected. This function normally creates the task that performs the
// filter's function.
bool CFilterVideoIn::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

void CFilterVideoIn::Stop()
{
	if (m_hTask)
	{
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		TSK_delete(m_hTask);
		SEM_post(&shared_scratch_SEM);
		m_hTask = NULL;
	}
}

// DSP Filter C-callable task function. This is required because DSP/BIOS can't directly call
// a class method.
void RunFilterTask(CFilterVideoIn * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterVideoIn::FilterTask()
{
	stMediaBuffer sOutBuf, sTimeTagOutBuf;
	Time_tag_T time_tag;
	
	while(true)
	{
		// receive empty output buffer
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (m_pOutPins[1].IsConnected())
			MBX_pend(m_pOutPins[1].GetFreeBuffsMbx(), &sTimeTagOutBuf, SYS_FOREVER);

		// receive input data
		MangoAV_HW_get_img_from_cam(
			(unsigned char*)sOutBuf.pBuffer,
			m_eMAVImageSize,
			m_nCamNum,
			MANGOAV_HW_420PLANAR,
			&time_tag,
			shared_isram,
			&shared_scratch_SEM
		);

		sOutBuf.nSize = m_nOutputSize;
		large_CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

		// send data buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// send time tag
		if (m_pOutPins[1].IsConnected())
		{
			*(Time_tag_T*)sTimeTagOutBuf.pBuffer = time_tag;
			MBX_post(m_pOutPins[1].GetFullBuffsMbx(), &sTimeTagOutBuf, SYS_FOREVER);
		}
	}
}

#endif // #ifdef DSP
