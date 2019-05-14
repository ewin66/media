/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterFramerate.h
 *
 * GENERAL DESCRIPTION: Frame rate filter - allows one in every 'm_nFramerate'
 *						frames to pass through to the next filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 12/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERFRAMERATE_H
#define _FILTERFRAMERATE_H

#include "MangoX_SharedExp.h"

#ifdef DSP
// any DSP-specific includes go here
#include "MangoAV_HWExp.h"
#include "MangoX.h"
#endif

// any definitions (enums, structs etc) can go here.

class CFilterFramerate: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	int m_nNumBufs;
	int m_nFramerate;

#ifdef DSP
	// Internal DSP-only member variables go here
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
#endif
	void SetDefaults();

public:
	// This is the constructor to be used on the DSP. When this constructor is used,
	// the DSP must deserialize a buffer from the host containing parameters
	// relevant to this filter.
	CFilterFramerate();

	// This is the constructor which includes parameters. It can either be used by
	// the host (who will then serialize and send to the DSP) or by the DSP if it
	// creates the filter independently of host requests.
	// The parameters are just examples.
	CFilterFramerate(int nNumBufs, int nFramerate);
	~CFilterFramerate();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterFramerate * pFilter);
#endif

#endif // #ifndef _FILTERFRAMERATE_H
