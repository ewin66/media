/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH2D_PCI.h
 *
 * GENERAL DESCRIPTION: Host to DSP communications filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERH2D_PCI_H
#define _FILTERH2D_PCI_H

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

class CFilterH2D_PCI: public CMangoXFilter
{
protected:
	// Member parameter variables
	int m_nNumBufs;
	int m_nMaxXferSize;
	char m_cStreamName[NAME_MAX_SIZE];

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
	SIO_Handle m_dataIn;
	SEM_Handle m_streamSem;
	SEM_Handle m_killSignalSem;
#else
	// these are parameters
	int m_nTimeout;
	h2d_pci_device_t * m_stPciDev;

	int m_nSioOutstanding;
	static int m_nTotalStreamsNum;
	h2d_pci_channel_t *m_outData; // one out channel
	PCI_STREAM_ptr_t *m_outDataBuf; // array of m_nNumBufs buffers
	MANGOBIOS_memoryHandle_t *m_outDataHandle; // array of m_nNumBufs handles
#endif
	void SetDefaults();

public:
	CFilterH2D_PCI();
	~CFilterH2D_PCI();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
#else
	CFilterH2D_PCI(int nNumBufs, int nMaxXferSize,
								int nTimeout, h2d_pci_device_t * stPciDev);
	MANGOERROR_error_t Connect();
	MANGOERROR_error_t GetEmptyBuffer(PCI_STREAM_ptr_t* buf);
	MANGOERROR_error_t SubmitBuffer(PCI_STREAM_ptr_t* buf, int size);
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterH2D_PCI * pFilter);
#endif

#endif // #ifndef _FILTERH2D_PCI_H
