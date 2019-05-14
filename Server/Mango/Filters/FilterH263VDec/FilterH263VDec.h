/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH263VDec.h
 *
 * GENERAL DESCRIPTION: H.263 video decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 22/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERH263VDEC_H
#define _FILTERH263VDEC_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include <std.h>
#include "MangoAV_HWExp.h"
#include "MangoX.h"

#include "h263dec.h"
#include "ih263dec.h"
#include "h263dec_ti.h"
#include "h263decp.h"
#include "ih263decp.h"
#include "h263decp_ti.h"
#include "h263decode.h"
#define H263_DEC_V_MAJ 0
#define H263_DEC_V_MIN 91

extern far SEM_Obj shared_scratch_SEM;
#endif

class CFilterH263VDec: public CMangoXFilter
{
protected:

	// Member parameter variables
	int m_nNumBufs;
	
	int m_nMaxOutputSize;
#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;

	IH263DEC_Handle m_sDecHandle;
	IH263DEC_Status m_sDs;
	int m_nH263First;
#endif
	void SetDefaults();

public:
	CFilterH263VDec();
	CFilterH263VDec(int nNumBufs);
	~CFilterH263VDec();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterH263VDec * pFilter);
#endif

#endif // #ifndef _FILTERH263VDEC_H
