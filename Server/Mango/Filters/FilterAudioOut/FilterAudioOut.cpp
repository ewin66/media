/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterAudioOut.cpp
 *
 * GENERAL DESCRIPTION: Analog video output filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterAudioOut.h"

#ifdef DSP
static int filterNum = 0;
#endif

// This function is called by all constructors.
void CFilterAudioOut::SetDefaults()
{
	// Set number of input and output pins for this filter here.
	m_nNumInPins=1; 
	m_nNumOutPins=0;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	
	// Set the allowed input type and the output type for your pins here.
	m_pInPins[0].SetType(MT_A_PCM);

	// This should be the same as the class's name
	strcpy(m_strName, "CFilterAudioOut");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterAudioOut::CFilterAudioOut()
{
	SetDefaults();
}

CFilterAudioOut::CFilterAudioOut(int nSpkNum, int nIsStereo, int nSampleRateDiv)
{
	m_nSpkNum = nSpkNum;
	m_nIsStereo = nIsStereo;
	m_nSampleRateDiv = nSampleRateDiv;
	SetDefaults();
}

// Destructor
CFilterAudioOut::~CFilterAudioOut()
{
#ifdef DSP
	if (m_isInited)
		MangoAV_HW_close_spk_output(m_nSpkNum);
#endif
	delete []m_pInPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterAudioOut::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nSpkNum);
		SERIALIZE(p, m_nIsStereo);
		SERIALIZE(p, m_nSampleRateDiv);
	}
	else
	{
		DESERIALIZE(p, m_nSpkNum);
		DESERIALIZE(p, m_nIsStereo);
		DESERIALIZE(p, m_nSampleRateDiv);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init; called after constructor but before graph is connected
bool CFilterAudioOut::Init()
{
	// Open video output
	if (MangoAV_HW_open_spk_output(m_nSpkNum, m_nIsStereo, m_nSampleRateDiv) != MANGOERROR_SUCCESS)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Can't open audio output");
		return false;
	}
	
	m_isInited=1;

	return true;
}

// This function is called to indicate to filter to start operating. At this point the
// graph is fully connected. This function normally creates the task that performs the
// filter's function.
bool CFilterAudioOut::Start()
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

// DSP Filter C-callable task function. This is required because DSP/BIOS can't directly call
// a class method.
void RunFilterTask(CFilterAudioOut * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterAudioOut::FilterTask()
{
	stMediaBuffer sInBuf;
	int outputSize = (m_nIsStereo+1) * AUDIO_SAMPLES_PER_FRAME * sizeof(short);
	int i;

	while(true)
	{
		// receive image
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
			goto skip;

		// send to audio out
		if (sInBuf.nSize % outputSize)
		{
			report_error(ERRTYPE_WARNING, m_strName, "skipping oddly sized buffer");
			goto skip;
		}

		for (i=0; i<sInBuf.nSize / outputSize; i++)
			MangoAV_HW_send_audio_frame_to_spk(
				(short*)sInBuf.pBuffer + i * outputSize / sizeof(short),
				m_nSpkNum,
				outputSize
			);
		
skip:		
		// free buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
