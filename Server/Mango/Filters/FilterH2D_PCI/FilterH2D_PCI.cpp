/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH2D_PCI.cpp
 *
 * GENERAL DESCRIPTION: Host to DSP communications filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterH2D_PCI.h"
#if defined(POSIX_COMPLIANT)
#include <unistd.h>
#endif
#ifdef DSP
static int filterNum = 0;
#else
#include <stdio.h>
int CFilterH2D_PCI::m_nTotalStreamsNum = 0;
extern MngSem mangox_atomic_sem;
#endif

void CFilterH2D_PCI::SetDefaults()
{
	m_nNumInPins=0; 
	m_nNumOutPins=1;

	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_ANY);

	strcpy(m_strName, "CFilterH2D_PCI");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#else
	int stream_num;

	m_outDataBuf = new PCI_STREAM_ptr_t[m_nNumBufs];
	m_outDataHandle = new MANGOBIOS_memoryHandle_t[m_nNumBufs];
	MngSemTake(mangox_atomic_sem, -1);
	stream_num = m_nTotalStreamsNum++;
	MngSemGive(mangox_atomic_sem);

	sprintf(m_cStreamName, "/DioPciDat%08x", stream_num);

	for (int i=0; i<m_nNumBufs; i++)
	{
		if (MANGOBIOS_memoryAlloc(m_nMaxXferSize, &m_outDataHandle[i], &m_outDataBuf[i].pci, NULL) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
		if (MANGOBIOS_memoryMap(m_outDataHandle[i], &m_outDataBuf[i].local, NULL) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
	}
	m_nSioOutstanding = 0;
#endif
}

// Constructor
CFilterH2D_PCI::CFilterH2D_PCI()
{
	SetDefaults();
}

#ifndef DSP
// Host constructor
CFilterH2D_PCI::CFilterH2D_PCI(int nNumBufs, int nMaxXferSize,
	int nTimeout, h2d_pci_device_t * stPciDev)
{
	m_nNumBufs = nNumBufs;
	m_nMaxXferSize = nMaxXferSize;
	m_nTimeout = nTimeout;
	m_stPciDev = stPciDev;
	SetDefaults();
}
#endif

// Destructor
CFilterH2D_PCI::~CFilterH2D_PCI()
{
#ifdef DSP
	Ptr dummy;
	int i;

	if (m_isInited)
	{
		// reclaim all outstanding buffs
		while(SIO_ready(m_dataIn) == TRUE)
			SIO_reclaim(m_dataIn, &dummy, NULL);

		SIO_delete(m_dataIn);
		SEM_delete(m_streamSem);

		// delete mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free data buffer(s)
		for (i=0; i<m_nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_nMaxXferSize));
		delete []p_outputBuffs;
	}
#else
	// close PCI stream
	if (PCI_STREAM_delete(m_outData) != MANGOERROR_SUCCESS)
		throw MANGOERROR_FAILURE;
	
	// free resources
	for (int i=0; i<m_nNumBufs; i++)
	{
		if (MANGOBIOS_memoryUnmap(m_outDataHandle[i]) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
		if (MANGOBIOS_memoryFree(m_outDataHandle[i]) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
	}

	delete []m_outDataBuf;
	delete []m_outDataHandle;
#endif
	delete []m_pOutPins;
}

// Serializer
int CFilterH2D_PCI::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_nMaxXferSize);
		memcpy(p, m_cStreamName, NAME_MAX_SIZE);
		p+=NAME_MAX_SIZE;

	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_nMaxXferSize);
		memcpy(m_cStreamName, p, NAME_MAX_SIZE);
		p+=NAME_MAX_SIZE;
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init
bool CFilterH2D_PCI::Init()
{
    SIO_Attrs sio_attrs;
    sio_attrs = SIO_ATTRS;
    sio_attrs.model = SIO_ISSUERECLAIM;
	sio_attrs.nbufs = m_nNumBufs;
	m_dataIn = SIO_create(m_cStreamName, SIO_INPUT, 0, &sio_attrs);
	if (!m_dataIn)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_sio_create);
		return false;
	}

	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), 1, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), 1, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	// Allocate input buffer(s) and submit to PCI stream
	p_outputBuffs = new unsigned char*[m_nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	for (int i=0; i<m_nNumBufs; i++)
	{
		p_outputBuffs[i] = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxXferSize), CACHE_L2_LINESIZE);
		if (!p_outputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}
		if (SIO_issue(m_dataIn, p_outputBuffs[i], m_nMaxXferSize, NULL) != SYS_OK)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_sio_issue);
			return false;
		}
	}
	m_streamSem = SEM_create(1, NULL);

	m_isInited=1;
	return true;
}

bool CFilterH2D_PCI::Start()
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

// override Stop function to flush stream
void CFilterH2D_PCI::Stop()
{
	if (m_hTask)
	{
		while(SEM_pend(m_streamSem, 10) != TRUE)
			SIO_flush(m_dataIn);
		TSK_delete(m_hTask);
		m_hTask = NULL;
	}
}

// DSP Filter C-callable task function
void RunFilterTask(CFilterH2D_PCI * pFilter)
{
	pFilter->FilterTask();
}


// DSP Filter task
void CFilterH2D_PCI::FilterTask()
{
	stMediaBuffer sOutBuf;
	Ptr pInData;
	int size;

	SIO_ctrl(m_dataIn, H2D_PCI_WAIT_CONNECT, NULL);	

	while(true)
	{
		SEM_pend(m_streamSem, SYS_FOREVER);
		size = SIO_reclaim(m_dataIn, &pInData, NULL);
		if (size < 0)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_sio_reclaim);
			return;
		}
		SEM_post(m_streamSem);

		// Make sure received buffer is not cached
		large_CACHE_invL2(pInData, size, CACHE_WAIT);

		// send buffer to next filter
		sOutBuf.pBuffer = pInData;
		sOutBuf.nMaxSize = m_nMaxXferSize;
		sOutBuf.nSize = size;
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
		
		// wait for filter to return buffer (SIO driver takes care of multi-buffering)
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		SEM_pend(m_streamSem, SYS_FOREVER);
		if (SIO_issue(m_dataIn, pInData, m_nMaxXferSize, NULL) != SYS_OK)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_sio_issue);
			return;
		}
		SEM_post(m_streamSem);
	}
}

#else

// HOST side

MANGOERROR_error_t CFilterH2D_PCI::Connect()
{
	int ret;

	h2d_pci_channel_params_t PCI_CHANNEL_Params;
	PCI_CHANNEL_Params.timeout = m_nTimeout;

	m_outData = PCI_STREAM_create(m_stPciDev, m_cStreamName, MANGOBIOS_OUTPUT, m_nMaxXferSize, &PCI_CHANNEL_Params);
	if (!m_outData)
		return MANGOERROR_FAILURE;

	ret = PCI_STREAM_ctrl(m_outData, H2D_PCI_WAIT_CONNECT, NULL);
	if (ret != MANGOERROR_SUCCESS)
		return (MANGOERROR_error_t)ret;

	return MANGOERROR_SUCCESS;
}

MANGOERROR_error_t CFilterH2D_PCI::GetEmptyBuffer(PCI_STREAM_ptr_t* buf)
{
	int size;
	if (m_nSioOutstanding < m_nNumBufs)
		*buf = m_outDataBuf[m_nSioOutstanding++];
	else
	{
		size = PCI_STREAM_reclaim(m_outData, buf);
		if (size < 0)
			return MANGOERROR_FAILURE;
	}
	return MANGOERROR_SUCCESS;
}

MANGOERROR_error_t CFilterH2D_PCI::SubmitBuffer(PCI_STREAM_ptr_t* buf, int size)
{
	return (MANGOERROR_error_t)PCI_STREAM_issue(m_outData, *buf, size);
}

#endif // #ifdef DSP
