/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterD2H_PCI.cpp
 *
 * GENERAL DESCRIPTION: DSP to Host communications filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterD2H_PCI.h"
#if defined(POSIX_COMPLIANT)
#include <unistd.h>
#endif

#ifdef DSP
static int filterNum = 0;
#else
#include <stdio.h>
int CFilterD2H_PCI::m_nTotalStreamsNum = 0;
extern MngSem mangox_atomic_sem;
#endif

void CFilterD2H_PCI::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=0;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_ANY);

	strcpy(m_strName, "CFilterD2H_PCI");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#else
	int stream_num;

	m_inDataBuf = new PCI_STREAM_ptr_t[m_nNumBufs];
	m_inDataHandle = new MANGOBIOS_memoryHandle_t[m_nNumBufs];
	MngSemTake(mangox_atomic_sem, -1);
	stream_num = m_nTotalStreamsNum++;
	MngSemGive(mangox_atomic_sem);

	sprintf(m_cStreamName, "/DioPciDat%08x", stream_num);

	for (int i=0; i<m_nNumBufs; i++)
	{
		if (MANGOBIOS_memoryAlloc(m_nMaxXferSize, &m_inDataHandle[i], &m_inDataBuf[i].pci, NULL) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
		if (MANGOBIOS_memoryMap(m_inDataHandle[i], &m_inDataBuf[i].local, NULL) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
	}
#endif
}

// Constructor
CFilterD2H_PCI::CFilterD2H_PCI()
{
	SetDefaults();
}

#ifndef DSP
// Host constructor
CFilterD2H_PCI::CFilterD2H_PCI(int nNumBufs, int nMaxXferSize,
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
CFilterD2H_PCI::~CFilterD2H_PCI()
{
#ifdef DSP
	Ptr dummy;

	if (m_isInited)
	{
		// reclaim all outstanding buffs
		while(SIO_ready(m_dataOut) == TRUE)
			SIO_reclaim(m_dataOut, &dummy, NULL);

		SIO_delete(m_dataOut);
		SEM_delete(m_streamSem);
	}
#else
	// close PCI stream
	if (PCI_STREAM_delete(m_inData) != MANGOERROR_SUCCESS)
		throw MANGOERROR_FAILURE;
	
	// free resources
	for (int i=0; i<m_nNumBufs; i++)
	{
		if (MANGOBIOS_memoryUnmap(m_inDataHandle[i]) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
		if (MANGOBIOS_memoryFree(m_inDataHandle[i]) != MANGOERROR_SUCCESS)
			throw MANGOERROR_FAILURE;
	}

	delete []m_inDataBuf;
	delete []m_inDataHandle;
#endif
	delete []m_pInPins;
}

// Serializer
int CFilterD2H_PCI::Serialize(char *pBuff, bool bStore)
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
bool CFilterD2H_PCI::Init()
{
    SIO_Attrs sio_attrs;
    sio_attrs = SIO_ATTRS;
    sio_attrs.model = SIO_ISSUERECLAIM;
	sio_attrs.nbufs = m_nNumBufs;
	m_dataOut = SIO_create(m_cStreamName, SIO_OUTPUT, 0, &sio_attrs);
	if (!m_dataOut)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_sio_create);
		return false;
	}
	m_streamSem = SEM_create(1, NULL);
	m_isInited=1;
	return true;
}

// DSP Filter C-callable task function
void RunFilterTask(CFilterD2H_PCI * pFilter)
{
	pFilter->FilterTask();
}

bool CFilterD2H_PCI::Start()
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
void CFilterD2H_PCI::Stop()
{
	if (m_hTask)
	{
		while(SEM_pend(m_streamSem, 10) != TRUE)
			SIO_flush(m_dataOut);
		TSK_delete(m_hTask);
		m_hTask = NULL;
	}
}

// DSP Filter task
void CFilterD2H_PCI::FilterTask()
{
	stMediaBuffer sOutBuf;
	Ptr dummy;
	int size;

	SIO_ctrl(m_dataOut, H2D_PCI_WAIT_CONNECT, NULL);	

	while(true)
	{
		// wait for buffer from data source
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
		
		// Write everything out from cache to SDRAM
		large_CACHE_wbL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
		
		// send to host
		SEM_pend(m_streamSem, SYS_FOREVER);
		if (SIO_issue(m_dataOut, sOutBuf.pBuffer, sOutBuf.nSize, NULL) != SYS_OK)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_sio_issue);
			return;
		}
		SEM_post(m_streamSem);

		SEM_pend(m_streamSem, SYS_FOREVER);
		size = SIO_reclaim(m_dataOut, &dummy, NULL);
		if (size < 0)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_sio_reclaim);
			return;
		}
		SEM_post(m_streamSem);

		// return buffer to sender
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
	}
}

#else

// HOST side

MANGOERROR_error_t CFilterD2H_PCI::Connect()
{
	int ret;

	h2d_pci_channel_params_t PCI_CHANNEL_Params;
	PCI_CHANNEL_Params.timeout = m_nTimeout;

	m_inData = PCI_STREAM_create(m_stPciDev, m_cStreamName, MANGOBIOS_INPUT, m_nMaxXferSize, &PCI_CHANNEL_Params);
	if (!m_inData)
		return MANGOERROR_FAILURE;

	ret = PCI_STREAM_ctrl(m_inData, H2D_PCI_WAIT_CONNECT, NULL);
	if (ret != MANGOERROR_SUCCESS)
		return (MANGOERROR_error_t)ret;

	// Issue empty buffers to DSP
	for (int i=0; i<m_nNumBufs; i++)
		if ((ret = PCI_STREAM_issue(m_inData, m_inDataBuf[i], m_nMaxXferSize)) != MANGOERROR_SUCCESS)
			return (MANGOERROR_error_t)ret;

	return MANGOERROR_SUCCESS;
}

MANGOERROR_error_t CFilterD2H_PCI::GetFullBuffer(PCI_STREAM_ptr_t* buf, int* size)
{
	*size = PCI_STREAM_reclaim(m_inData, buf);
	
	if (*size < 0)
		return (MANGOERROR_error_t)-(*size);

	return MANGOERROR_SUCCESS;
}

MANGOERROR_error_t CFilterD2H_PCI::FreeBuffer(PCI_STREAM_ptr_t* buf)
{
	return (MANGOERROR_error_t)PCI_STREAM_issue(m_inData, *buf, m_nMaxXferSize);
}

#endif // #ifdef DSP
