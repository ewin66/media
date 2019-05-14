/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterVideoOut.h
 *
 * GENERAL DESCRIPTION: Analog video output filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERVIDEOOUT_H
#define _FILTERVIDEOOUT_H

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

class CFilterVideoOut: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	enVideoStandard m_eVideoStandard;
	enImageSize m_eImageSize;
	int m_nVideoBufs;
	int m_bDropFrames;

#ifdef DSP
	// Internal DSP-only member variables go here
	int m_nFilterNum;
	Video_Standard_T m_eMAVVideoStandard;
	Image_Sizes_T m_eMAVImageSize;
#endif
	void SetDefaults();

public:
	CFilterVideoOut();
	CFilterVideoOut(enImageSize eImageSize, enVideoStandard eVideoStandard,
					int nVideoBufs, bool bDropFrames);
	~CFilterVideoOut();
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
void RunFilterTask(CFilterVideoOut * pFilter);
#endif

#endif // #ifndef _FILTERVIDEOOUT_H
