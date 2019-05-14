/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterAudioOut.h
 *
 * GENERAL DESCRIPTION: Analog audio output filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERAUDIOOUT_H
#define _FILTERAUDIOOUT_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoAV_SharedExp.h"
#include "MangoAV_HWExp.h"
#include "MangoX.h"
#endif

class CFilterAudioOut: public CMangoXFilter
{
protected:

	// Member parameter variables
//	int m_nNumBufs;
	int m_nSpkNum;
	int m_nIsStereo;
	int m_nSampleRateDiv;

#ifdef DSP
	// Internal DSP-only member variables
	int m_nFilterNum;
#endif
	void SetDefaults();

public:
	CFilterAudioOut();
	CFilterAudioOut(int nSpkNum, int nIsStereo, int nSampleRateDiv);
	~CFilterAudioOut();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterAudioOut * pFilter);
#endif

#endif // #ifndef _FILTERAUDIOOUT_H
