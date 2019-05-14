/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterG711ADec.h
 *
 * GENERAL DESCRIPTION: G.711 Audio Decoding Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * <9 05 2006> 	<Moshe Shimoni> 	
 *
 ******************************************************************************/
#ifndef _FILTERG711ADEC_H
#define _FILTERG711ADEC_H

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
} stG711Adec_h2dparams;

class CFilterG711ADec: public CMangoXFilter
{
protected:

	// Member parameter variables
	stG711Adec_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
#endif
	void SetDefaults();

public:
	CFilterG711ADec();
	CFilterG711ADec(stG711Adec_h2dparams& params);
	~CFilterG711ADec();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterG711ADec * pFilter);
#endif

#endif // #ifndef _FILTERG711ADEC_H
