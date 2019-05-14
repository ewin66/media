/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterCustom.cpp
 *
 * GENERAL DESCRIPTION: Template for user-definable filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 02/03/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterCustom.h"

#ifdef DSP
static int filterNum = 0;
#endif

/* Define the following if this is a PASS-THROUGH filter - this is a filter that only
	modifies the incoming buffer and passes it on, such as a text overlay, etc. */
//#define PASSTHRU

// This function is called by all constructors.
void CFilterCustom::SetDefaults()
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
	strcpy(m_strName, "CFilterCustom");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterCustom::CFilterCustom()
{
	SetDefaults();
}

// Constructor with parameters
CFilterCustom::CFilterCustom(int nNumBufs, int nMaxOutputSize)
{
	m_nNumBufs = nNumBufs;
	m_nMaxOutputSize = nMaxOutputSize;
	SetDefaults();
}

// Destructor
CFilterCustom::~CFilterCustom()
{
#ifdef DSP
	if (m_isInited)
	{
		// at this point the processing task has already been killed.

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

#ifndef PASSTHRU
		// Free output buffer(s)
		for (int i=0; i<m_nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize));
		delete []p_outputBuffs;
#endif
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterCustom::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_nMaxOutputSize);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_nMaxOutputSize);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init; called after constructor but before graph is connected
bool CFilterCustom::Init()
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

#ifndef PASSTHRU
	// Allocate output buffer(s) and submit to free mailbox.
	stMediaBuffer sOutBuf;
	p_outputBuffs = new unsigned char*[m_nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	for (int i=0; i<m_nNumBufs; i++)
	{
		p_outputBuffs[i] = (unsigned char*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize), CACHE_L2_LINESIZE);
		if (!p_outputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}
		sOutBuf.pBuffer = p_outputBuffs[i];
		sOutBuf.nMaxSize = m_nMaxOutputSize;
		sOutBuf.nSize = 0;
		if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
			return false;
		}
	}
#endif

	m_isInited=1;
	return true;
}

// This function is called to indicate to filter to start operating. At this point the
// graph is fully connected. This function normally creates the task that performs the
// filter's function.
bool CFilterCustom::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stackseg = seg_isram;
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

// This funtion is called to stop operation of this filter. There is no need to
// implement this function as it is already implemented in the parent class
// (CMangoXFilter). You may override this implementation if you wish.
/*void CFilterCustom::Stop()
{
	if (m_hTask)
	{
		TSK_delete(m_hTask);
		m_hTask = NULL;
	}
}*/

// DSP Filter C-callable task function. This is required because DSP/BIOS can't directly call
// a class method.
void RunFilterTask(CFilterCustom * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterCustom::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;

	while(true)
	{
#ifndef PASSTHRU
		// Normal mode - Filter receives data from source and creates new data based on it.

		// receive input data
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// receive empty output buffer
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// at this point, read data from sInBuf.pBuffer (size is sInBuf.nSize) and write output
		// data to sOutBuf.pBuffer. Set sOutBuf.nSize to the output buffer size.
		qdma_memcpy(sOutBuf.pBuffer, sInBuf.pBuffer, sInBuf.nSize);
		qdma_memcpy_wait();
		sOutBuf.nSize = sInBuf.nSize;
		
		// send data buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free input buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
#else
		// Pass-through mode - Filter receives data, modifies it in some way and passes it on.

		// receive input data
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// Modify data at sInBuf.pBuffer, sized sInBuf.nSize. You may modify the size but you
		// may not exceed sInBuf.nMaxSize.

		// send data buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// wait for buffer to return
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);

		// and return it to the sender
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
#endif
	}
}

#endif // #ifdef DSP
