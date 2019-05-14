/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterVideoOut.cpp
 *
 * GENERAL DESCRIPTION: Analog video output filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterVideoOut.h"

#ifdef DSP
#include "MangoAV_SharedExp.h"
#include "MangoAV_HWExp.h"
static int filterNum = 0;

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
		MANGO_AV_QCIF_PAL,	// MX_QCIF
		MANGO_AV_CIF_PAL,	// MX_CIF
		MANGO_AV_2CIF_PAL,	// MX_2CIF
		MANGO_AV_4CIF_INTERLEAVED_PAL,	// MX_4CIF
		MANGO_AV_SIF_NTSC,	// MX_QVGA
		MANGO_AV_VGA_NTSC	// MX_VGA
	}
};

#endif

// This function is called by all constructors.
void CFilterVideoOut::SetDefaults()
{
	// Set number of input and output pins for this filter here.
	m_nNumInPins=1; 
	m_nNumOutPins=0;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	
	// Set the allowed input type and the output type for your pins here.
	m_pInPins[0].SetType(MT_V_PLANAR_420);

	// This should be the same as the class's name
	strcpy(m_strName, "CFilterVideoOut");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterVideoOut::CFilterVideoOut()
{
	SetDefaults();
}

CFilterVideoOut::CFilterVideoOut(enImageSize eImageSize, enVideoStandard eVideoStandard,
									int nVideoBufs, bool bDropFrames)
{
	m_eImageSize = eImageSize;
	m_eVideoStandard = eVideoStandard;
	m_nVideoBufs = nVideoBufs;
	m_bDropFrames = bDropFrames;
	SetDefaults();
}

// Destructor
CFilterVideoOut::~CFilterVideoOut()
{
#ifdef DSP
	if (m_isInited)
		MangoAV_HW_close_tv_output();
#endif
	delete []m_pInPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterVideoOut::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_eImageSize);
		SERIALIZE(p, m_eVideoStandard);
		SERIALIZE(p, m_nVideoBufs);
		SERIALIZE(p, m_bDropFrames);
	}
	else
	{
		DESERIALIZE(p, m_eImageSize);
		DESERIALIZE(p, m_eVideoStandard);
		DESERIALIZE(p, m_nVideoBufs);
		DESERIALIZE(p, m_bDropFrames);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init; called after constructor but before graph is connected
bool CFilterVideoOut::Init()
{
	int output_timeout;

	output_timeout = (m_bDropFrames? MANGOBIOS_POLL : MANGOBIOS_FOREVER);

	// Open video output
	m_eMAVVideoStandard = (m_eVideoStandard == MX_NTSC? MANGO_AV_NTSC : MANGO_AV_PAL);
	m_eMAVImageSize = image_sizes[m_eVideoStandard][m_eImageSize];

	if (m_eMAVImageSize == (Image_Sizes_T)-1)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Invalid image size");
		return false;
	}

	if (MangoAV_HW_open_tv_output(m_eMAVVideoStandard, m_nVideoBufs, output_timeout) != MANGOERROR_SUCCESS)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Can't open video output");
		return false;
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
bool CFilterVideoOut::Start()
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

void CFilterVideoOut::Stop()
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
void RunFilterTask(CFilterVideoOut * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterVideoOut::FilterTask()
{
	stMediaBuffer sInBuf;
	
	while(true)
	{
		// receive image
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
			goto skip;

		// send to video out
		MangoAV_HW_send_img_to_monitor(
			(unsigned char*)sInBuf.pBuffer,
			m_eMAVImageSize,
			shared_isram,
			&shared_scratch_SEM
		);

skip:		
		// free buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
