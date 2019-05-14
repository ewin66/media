/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:		FilterUpsamplingFrom8k.h
 *
 * GENERAL DESCRIPTION: From 8K Hz to 48K/36K Hz in 2 stages.
 *						For each stage:
 *                  	    "Inflate" Size then 
 *                  	    Anti-Replication Low Pass Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * June 06 			Ilan sinai
 *
 * REMARKS			Accepts buffer sizes up to 1152 samples.
 ******************************************************************************/
#ifndef _FILTERUPSAMPLINGFROM8K_H
#define _FILTERUPSAMPLINGFROM8K_H

#include "MangoX_SharedExp.h"

#ifdef DSP
// any DSP-specific includes go here
#include "MangoAV_HWExp.h"
#include "MangoX.h"
#define ALIGNED_ARRAY(ptr) _nassert((int) ptr % 8 == 0)
#endif

// any definitions (enums, structs etc) can go here.
enum {
	c_IntIFilterNumOfCoeffs = 16,   // Actually the number is 2*(IFilterNumOfCoeffs)+1
	c_IntIIFilterNumOfCoeffs = 40,  // Actually the number is 2*(IIFilterNumOfCoeffs)+1
	c_InterpolationFactorI = 3,
	c_InterpolationFactorII = 2,
	c_MaxInterpolationFactor = 3,

	c_IntFilterIIPrefix = 2*c_IntIIFilterNumOfCoeffs + c_MaxInterpolationFactor,
	c_IntFilterIPrefix = 2*c_IntIFilterNumOfCoeffs + c_MaxInterpolationFactor,

	c_InAudBufSize = c_IntFilterIIPrefix+AUDIO_SAMPLES_PER_FRAME,
	c_TempBufSize = c_IntFilterIPrefix+AUDIO_SAMPLES_PER_FRAME*c_InterpolationFactorII,
	c_OutAudBufSize = AUDIO_SAMPLES_PER_FRAME*c_InterpolationFactorII*c_InterpolationFactorI
};

/////////////////////////////////////////////////

class CFilterUpsamplingFrom8k: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	int m_nNumBufs;

#ifdef DSP
	// Internal DSP-only member variables go here
	int m_nFilterNum;
	unsigned char** p_outputBuffs;

	short* m_InAudBuf;
	short* m_TempBuf;
	short* m_OutAudBuf;

	short* m_pCurrentAudio;
	int m_iFrameCounter;

#endif
	void SetDefaults();

public:
	// This is the constructor to be used on the DSP. When this constructor is used,
	// the DSP must deserialize a buffer from the host containing parameters
	// relevant to this filter.
	CFilterUpsamplingFrom8k();

	// This is the constructor which includes parameters. It can either be used by
	// the host (who will then serialize and send to the DSP) or by the DSP if it
	// creates the filter independently of host requests.
	// The parameters are just examples.
	CFilterUpsamplingFrom8k(int nNumBufs);
	~CFilterUpsamplingFrom8k();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
protected:
	static void Prepare();
	void Perform(int nInSamples);
	static int Interpolate_FirSym(short* restrict x, short* restrict h, short* restrict r, int nh, int nr, int InterpolationFactor);
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterUpsamplingFrom8k * pFilter);
#endif

#endif
