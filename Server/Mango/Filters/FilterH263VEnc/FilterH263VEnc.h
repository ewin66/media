/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH263VEnc.h
 *
 * GENERAL DESCRIPTION: H.263 video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 22/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERH263VENC_H
#define _FILTERH263VENC_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"

#include "h263.h"
#include "h263enc.h"
#include "ih263enc.h"
#include "h263enc_ti.h"
#include "h263encp.h"
#include "ih263encp.h"
#include "h263encp_ti.h"
#define H263_ENC_V_MAJ 0
#define H263_ENC_V_MIN 91

extern far SEM_Obj shared_scratch_SEM;

#endif

typedef struct {
	int nNumBufs;
	int nMaxOutputSize;
	enVideoStandard eVideoStandard;
	enImageSize eImageSize;
	int nBitrate;
	int nFramerate;
	int nNumGOPFrames;
	int nQmax;
	int nQmin;
	int nQI;
} stH263V_h2dparams;

class CFilterH263VEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stH263V_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
	IH263ENC_Handle m_sEncHandle;
	IH263ENC_Status m_sEs;
#endif
	void SetDefaults();

public:
	CFilterH263VEnc();
	CFilterH263VEnc(stH263V_h2dparams& params);
	~CFilterH263VEnc();
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
void RunFilterTask(CFilterH263VEnc * pFilter);
#endif

#endif // #ifndef _FILTERH263VENC_H
