/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterAudioIn.h
 *
 * GENERAL DESCRIPTION: Analog source audio input filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 31/03/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERAUDIOIN_H
#define _FILTERAUDIOIN_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoAV_SharedExp.h"
#include "MangoAV_HWExp.h"
#include "MangoX.h"
#endif

// any definitions (enums, structs etc) can go here.

class CFilterAudioIn: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	int m_nNumBufs;
	int m_nMicNum;
	int m_nIsStereo;
	int m_nSampleRateDiv;
	enAudioInputType m_eAudInputType;

#ifdef DSP
	// Internal DSP-only member variables go here
	int m_nFilterNum;
	unsigned char** m_pOutputBuffs;
	Time_tag_T* m_pTimeTag;
	int m_nOutputSize;
#endif
	void SetDefaults();

public:
	CFilterAudioIn();
	CFilterAudioIn(int nNumBufs, int nMicNum, int nIsStereo, int nSampleRateDiv,
					enAudioInputType eAudInputType);
	~CFilterAudioIn();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterAudioIn * pFilter);
#endif

#endif // #ifndef _FILTERAUDIOIN_H
