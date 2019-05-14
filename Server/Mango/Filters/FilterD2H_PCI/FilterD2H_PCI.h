/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterD2H_PCI.h
 *
 * GENERAL DESCRIPTION: DSP to Host communications filter header.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERD2H_PCI_H
#define _FILTERD2H_PCI_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifndef INTERRUPT_DRIVEN
#define INTERRUPT_DRIVEN
#endif

#ifdef DSP
#include <sio.h>
#include "MangoX.h"
#endif

#include "MangoX_Shared.h"
#include "h2d_pci_streamSharedExp.h"
#include "h2d_pci_streamExp.h"

#define NAME_MAX_SIZE	32

class CFilterD2H_PCI: public CMangoXFilter
{
protected:

	// Member parameter variables
	int m_nNumBufs;
	int m_nMaxXferSize;
	char m_cStreamName[NAME_MAX_SIZE];

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	SIO_Handle m_dataOut;
	SEM_Handle m_streamSem;
#else
	// these are parameters
	int m_nTimeout;
	h2d_pci_device_t * m_stPciDev;

	static int m_nTotalStreamsNum;
	h2d_pci_channel_t *m_inData; // one in channel
	PCI_STREAM_ptr_t *m_inDataBuf; // array of m_nNumBufs buffers
	MANGOBIOS_memoryHandle_t *m_inDataHandle; // array of m_nNumBufs handles
#endif
	void SetDefaults();
	
public:
	CFilterD2H_PCI();
	~CFilterD2H_PCI();
	static void GlobalInit();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
#else
	CFilterD2H_PCI(int nNumBufs, int nMaxXferSize,
								int nTimeout, h2d_pci_device_t * stPciDev);
	MANGOERROR_error_t Connect();
	MANGOERROR_error_t GetFullBuffer(PCI_STREAM_ptr_t* buf, int* size);
	MANGOERROR_error_t FreeBuffer(PCI_STREAM_ptr_t* buf);
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterD2H_PCI * pFilter);
#endif

#endif // #ifndef _FILTERD2H_PCI_H
