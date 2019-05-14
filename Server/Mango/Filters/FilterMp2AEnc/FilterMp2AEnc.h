/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMp2AEnc.h
 *
 * GENERAL DESCRIPTION: MPEG-1 Layer 2 Audio Encoding Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 22/01/06 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERMP2AENC_H
#define _FILTERMP2AENC_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"

#include <alg.h>
#include "imp3enc5064L2.h"
#include "mp3enc5064L2.h"
#include "mp3enc5064L2_CUTE.h"

extern far SEM_Obj shared_scratch_SEM;

#endif

typedef struct {
	int nNumBufs;
	int nMaxOutputSize;
	int nBitrate;
	int nSampleFreq;
	enMp2AudioMode enMode;
} stMP2A_h2dparams;

class CFilterMp2AEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stMP2A_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
	MP3ENC5064L2_Handle m_sEncHandle;
	IMP3ENC5064L2_Params m_sEncParams;
	IMP3ENC5064L2_Fxns *m_fxns;
#endif
	void SetDefaults();

public:
	CFilterMp2AEnc();
	CFilterMp2AEnc(stMP2A_h2dparams& params);
	~CFilterMp2AEnc();
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
void RunFilterTask(CFilterMp2AEnc * pFilter);
#endif

#endif // #ifndef _FILTERMP2AENC_H
