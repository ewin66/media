/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterVideoIn.h
 *
 * GENERAL DESCRIPTION: Analog source video input filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 31/03/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERVIDEOIN_H
#define _FILTERVIDEOIN_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"
#include "MangoAV_HWExp.h"
extern "C" {
#include <alg.h>
#include <_alg.h>
}
extern far SEM_Obj shared_scratch_SEM;
extern void* shared_isram;

#endif

// any definitions (enums, structs etc) can go here.

class CFilterVideoIn: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	int m_nNumBufs;
	int m_nCamNum;
	int m_nCamBufs;
	enVideoStandard m_eVideoStandard;
	enImageSize m_eImageSize;
	enVideoAnalogConnection m_eVideoConn;

#ifdef DSP
	// Internal DSP-only member variables go here
	int m_nFilterNum;
	unsigned char** m_pOutputBuffs;
	Time_tag_T* m_pTimeTag;
	int m_nOutputSize;
	Video_Standard_T m_eMAVVideoStandard;
	Image_Sizes_T m_eMAVImageSize;
	Video_Analog_Connection_Format_T m_eMAVConn;
#endif
	void SetDefaults();

public:
	CFilterVideoIn();
	CFilterVideoIn(int nNumBufs, int nCamNum,
							   enImageSize eImageSize,
							   enVideoStandard eVideoStandard,
							   enVideoAnalogConnection eVideoConn,
							   int nCamBufs);
	~CFilterVideoIn();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterVideoIn * pFilter);
#endif

#endif // #ifndef _FILTERVIDEOIN_H
