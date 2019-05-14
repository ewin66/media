/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMp3AEnc.h
 *
 * GENERAL DESCRIPTION: MPEG-1 Layer 3 Audio Encoding Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/07/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERMP3AENC_H
#define _FILTERMP3AENC_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"

#include <alg.h>
#include "mp3enc_ittiam.h"

extern far SEM_Obj shared_scratch_SEM;

#endif

typedef struct {
	int nNumBufs;
	int nMaxOutputSize;
	int nBitrate;
	int nSampleFreq;
	int nIsStereo;
} stMP3A_h2dparams;

class CFilterMp3AEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stMP3A_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
    IMP3ENC_Handle  m_sEncHandle;
    IMP3ENC_Fxns    *m_fxns;
    IMP3ENC_Params  m_sEncParams;

    unsigned char*	m_pEncBuffer;
    int	m_nEncBufferSize;
#endif
	void SetDefaults();

public:
	CFilterMp3AEnc();
	CFilterMp3AEnc(stMP3A_h2dparams& params);
	~CFilterMp3AEnc();
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
void RunFilterTask(CFilterMp3AEnc * pFilter);
#endif

#endif // #ifndef _FILTERMP3AENC_H
