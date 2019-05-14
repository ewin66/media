/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterFramerate.cpp
 *
 * GENERAL DESCRIPTION: Frame rate filter - allows one in every 'm_nFramerate'
 *						frames to pass through to the next filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 12/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterFramerate.h"

#ifdef DSP
static int filterNum = 0;
#endif

// This function is called by all constructors.
void CFilterFramerate::SetDefaults()
{
	// Set number of input and output pins for this filter here.
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];

	// Set the allowed input type and the output type for your pins here.
	m_pInPins[0].SetType(MT_ANY);
	m_pOutPins[0].SetType(MT_ANY);

	// This should be the same as the class's name
	strcpy(m_strName, "CFilterFramerate");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterFramerate::CFilterFramerate()
{
	SetDefaults();
}

// Constructor with parameters
CFilterFramerate::CFilterFramerate(int nNumBufs, int nFramerate)
{
	m_nNumBufs = nNumBufs;
	m_nFramerate = nFramerate;
	SetDefaults();
}

// Destructor
CFilterFramerate::~CFilterFramerate()
{
#ifdef DSP
	if (m_isInited)
	{
		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterFramerate::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_nFramerate);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_nFramerate);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init; called after constructor but before graph is connected
bool CFilterFramerate::Init()
{
	// Perform any required initialization here

	// Initialize output mailboxes (each filter is responsible for allocating its output buffer;
	// the input mailboxes and buffers are assumed to be created by the previous filter in the
	// chain).
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	m_isInited=1;
	return true;
}

// This function is called to indicate to filter to start operating. At this point the
// graph is fully connected. This function normally creates the task that performs the
// filter's function.
bool CFilterFramerate::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stacksize = 0x400;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}

	// work around a bug that causes kernel/object view to crash CCS
	shuffle_task_list();

	return true;
}

// DSP Filter C-callable task function. This is required because DSP/BIOS can't directly call
// a class method.
void RunFilterTask(CFilterFramerate * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterFramerate::FilterTask()
{
	stMediaBuffer sInBuf;
	int frameCnt = 1;

	while(true)
	{
		// only pass through one in every 'm_nFramerate' frames

		// receive input frame
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		frameCnt--;
		// check if we want to use this frame
		if (frameCnt <= 0)
		{
			// send frame to next filter
			MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);
			MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
			frameCnt = m_nFramerate;
		}

		// return to sender
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
