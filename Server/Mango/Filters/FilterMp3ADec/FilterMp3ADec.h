/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMp3ADec.h
 *
 * GENERAL DESCRIPTION: MPEG-1 Layer 3 Audio decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/07/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERMP3ADEC_H
#define _FILTERMP3ADEC_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"
#include "MangoAV_HWExp.h"

#include <alg.h>
#include "mp3decoder_ittiam.h"

#define MIN_DEC_BUFF_SIZE 0x2000
#define MP4_HEADER_READ 1442

extern far SEM_Obj shared_scratch_SEM;
#endif

class CFilterMp3ADec: public CMangoXFilter
{
protected:

	// Member parameter variables
	int m_nNumBufs;
	int m_nMaxOutputSize;
	
#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;

    IMP3DECODER_Handle  m_sDecHandle;
    IMP3DECODER_Fxns    *m_fxns;
    IMP3DECODER_Params  m_sDecParams;
    
    unsigned char*	m_pDecBuffer;
    int	m_nDecBufferSize;
    
#endif
	void SetDefaults();

public:
	CFilterMp3ADec();
	CFilterMp3ADec(int nNumBufs, int nMaxOutputSize);
	~CFilterMp3ADec();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterMp3ADec * pFilter);
#endif

#endif // #ifndef _FILTERMP3ADEC_H
