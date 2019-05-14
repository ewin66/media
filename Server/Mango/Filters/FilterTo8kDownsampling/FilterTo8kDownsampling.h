/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:		FilterTo8kDownsampling.h
 *
 * GENERAL DESCRIPTION: From 48K/32K Hz To 8K Hz in 2 stages. 
 *                      Anti-Aliasing Filter then Decimation in each stage.
 *
 * DATE CREATED		AUTHOR
 *
 * May 06 			Ilan sinai
 *
 * REMARKS			Accepts buffers of any size, but maximum is defined by
 *					first received buffer.
 ******************************************************************************/
#ifndef _FILTERTO8KDOWNSAMPLING_H
#define _FILTERTO8KDOWNSAMPLING_H

#include "MangoX_SharedExp.h"

#ifdef DSP
// any DSP-specific includes go here
#include "MangoAV_HWExp.h"
#include "MangoX.h"
#endif

// any definitions (enums, structs etc) can go here.
////////////////////////
#define ALIGNED_ARRAY(ptr) _nassert((int) ptr % 8 == 0)

enum {
	c_DecIFilterNumOfCoeffs = 16,   // Actually the number is 2*(IFilterNumOfCoeffs)+1
	c_DecIIFilterNumOfCoeffs = 40,  // Actually the number is 2*(IIFilterNumOfCoeffs)+1

	c_DecimationfactorI = 3,
	c_DecimationfactorII = 2,

	c_DecFilterIIPrefix = 2*c_DecIIFilterNumOfCoeffs,
	c_DecFilterIPrefix = 2*c_DecIFilterNumOfCoeffs
};

/////////////////////////////////////////////////


class CFilterTo8kDownsampling: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	int m_nNumBufs;

#ifdef DSP
	// Internal DSP-only member variables go here
	int m_nFilterNum;
	unsigned char** p_outputBuffs;

	short* m_in_aud;
	short* m_pCurrentAudio;
	int m_iFrameCounter;

	short* m_TempBuffer1;
	short* m_TempBuffer2;
	int m_sizeOutputBuffs;
	int m_in_aud_size;
	int m_TempBuffer1_size;
	int m_TempBuffer2_size;

#endif
	void SetDefaults();

public:
	// This is the constructor to be used on the DSP. When this constructor is used,
	// the DSP must deserialize a buffer from the host containing parameters
	// relevant to this filter.
	CFilterTo8kDownsampling();

	// This is the constructor which includes parameters. It can either be used by
	// the host (who will then serialize and send to the DSP) or by the DSP if it
	// creates the filter independently of host requests.
	// The parameters are just examples.
	CFilterTo8kDownsampling(int nNumBufs);
	~CFilterTo8kDownsampling();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
protected:
	bool ArrangeBuffers(short *pDataIn, short nSize, bool &bIsBufferFilledUp);
	static void Prepare();
	bool Perform(short *pDataIn, short *pDataOut, int nInSize, int* nOutSize);
	static bool Decimate(const short* restrict pSource, short* restrict pTarget, const int SourceLen, int *pTargetLen,const  int DecimateFactor);
	static void DownSampling_FirSym(short* restrict x, short* restrict h, short* restrict r, int nh, int nr);
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterTo8kDownsampling * pFilter);
#endif

#endif // #ifndef _FILTERCUSTOM_H
