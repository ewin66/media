/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterG711AEnc.h
 *
 * GENERAL DESCRIPTION: G.711 Audio Encoding Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * <11 01 2003> 	<Yakov Shkolnik > 	
 *
 ******************************************************************************/
#ifndef _FILTERG711AENC_H
#define _FILTERG711AENC_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"
#include <alg.h>
#include "audio_g711.h"
#endif

typedef struct {
	enG711AudioMode eVocoderType;
	int nNumBufs;
	int nMaxOutputSize;
} stG711Aenc_h2dparams;

class CFilterG711AEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stG711Aenc_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
#endif
	void SetDefaults();

public:
	CFilterG711AEnc();
	CFilterG711AEnc(stG711Aenc_h2dparams& params);
	~CFilterG711AEnc();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterG711AEnc * pFilter);
#endif

#endif // #ifndef _FILTERG711AENC_H
