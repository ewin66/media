/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterCustom.h
 *
 * GENERAL DESCRIPTION: Template for user-definable filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 02/03/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERCUSTOM_H
#define _FILTERCUSTOM_H

#include "MangoX_SharedExp.h"

#ifdef DSP
// any DSP-specific includes go here
#include "MangoAV_HWExp.h"
#include "MangoX.h"
#endif

// any definitions (enums, structs etc) can go here.

class CFilterCustom: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	int m_nNumBufs;
	int m_nMaxOutputSize;

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
	CFilterCustom();

	// This is the constructor which includes parameters. It can either be used by
	// the host (who will then serialize and send to the DSP) or by the DSP if it
	// creates the filter independently of host requests.
	// The parameters are just examples.
	CFilterCustom(int nNumBufs, int nMaxOutputSize);
	~CFilterCustom();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void FilterTask();
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterCustom * pFilter);
#endif

#endif // #ifndef _FILTERCUSTOM_H
