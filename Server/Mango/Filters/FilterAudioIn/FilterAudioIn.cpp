/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterAudioIn.cpp
 *
 * GENERAL DESCRIPTION: Analog source video input filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 31/03/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterAudioIn.h"

#ifdef DSP
static int filterNum = 0;
#endif

// This function is called by all constructors.
void CFilterAudioIn::SetDefaults()
{
	// Set number of input and output pins for this filter here.
	m_nNumInPins=0; 
	m_nNumOutPins=2;

	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	
	// Set the allowed input type and the output type for your pins here.
	m_pOutPins[0].SetType(MT_A_PCM);
	m_pOutPins[1].SetType(MT_TIME_TAG);
	m_pOutPins[1].SetRequired(false);

	// This should be the same as the class's name
	strcpy(m_strName, "CFilterAudioIn");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterAudioIn::CFilterAudioIn()
{
	SetDefaults();
}

CFilterAudioIn::CFilterAudioIn(int nNumBufs, int nMicNum, int nIsStereo, int nSampleRateDiv,
								enAudioInputType eAudInputType)

{
	m_nNumBufs = nNumBufs;
	m_nMicNum = nMicNum;
	m_nIsStereo = nIsStereo;
	m_nSampleRateDiv = nSampleRateDiv;
	m_eAudInputType = eAudInputType;
	SetDefaults();
}

// Destructor
CFilterAudioIn::~CFilterAudioIn()
{
#ifdef DSP
	if (m_isInited)
	{
		MangoAV_HW_close_mic_input(m_nMicNum);
		
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
int CFilterAudioIn::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_nMicNum);
		SERIALIZE(p, m_nIsStereo);
		SERIALIZE(p, m_nSampleRateDiv);
		SERIALIZE(p, m_eAudInputType);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_nMicNum);
		DESERIALIZE(p, m_nIsStereo);
		DESERIALIZE(p, m_nSampleRateDiv);
		DESERIALIZE(p, m_eAudInputType);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init; called after constructor but before graph is connected
bool CFilterAudioIn::Init()
{
	// Open audio input
	if (MangoAV_HW_open_mic_input(m_nMicNum, m_nIsStereo, m_nSampleRateDiv, (Audio_Connector_T)m_eAudInputType) != MANGOERROR_SUCCESS)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Can't open audio input");
		return false;
	}
	
	m_nOutputSize = AUDIO_SAMPLES_PER_FRAME * sizeof(int);
	
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
	
	return true;
}

// This function is called to indicate to filter to start operating. At this point the
// graph is fully connected. This function normally creates the task that performs the
// filter's function.
bool CFilterAudioIn::Start()
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
void RunFilterTask(CFilterAudioIn * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterAudioIn::FilterTask()
{
	stMediaBuffer sOutBuf, sTimeTagOutBuf;
	Time_tag_T time_tag;

	while(true)
	{
		// receive empty output buffer
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (m_pOutPins[1].IsConnected())
			MBX_pend(m_pOutPins[1].GetFreeBuffsMbx(), &sTimeTagOutBuf, SYS_FOREVER);

		// receive audio data
		MangoAV_HW_get_audio_frame_from_mic(
			(short*)sOutBuf.pBuffer,
			m_nMicNum,
			&sOutBuf.nSize,
			&time_tag
		);

		CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
		
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
