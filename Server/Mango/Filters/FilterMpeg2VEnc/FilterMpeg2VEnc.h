/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg2VEnc.h
 *
 * GENERAL DESCRIPTION: Mpeg-2 video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERMPEG2VENC_H
#define _FILTERMPEG2VENC_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"

#include "mpeg2venc.h"
#include "mpeg2vencp_ti.h"
#include "impeg2vencp.h"
#include "mpeg2venc_ti.h"
#include "impeg2venc.h"

#define MPEG2ENC_HISTORY_BUFFS 4

#define MPEG2_ENC_V_MAJ 1
#define MPEG2_ENC_V_MIN 2

extern far SEM_Obj shared_scratch_SEM;

#endif

typedef struct {
	int nNumBufs;
	int nMaxOutputSize;
	enVideoStandard eVideoStandard;
	enImageSize eImageSize;
	int nNumGOPFrames;
	int nNumBFrames;
	int nConstantBitrate;
} stMp2V_h2dparams;

class CFilterMpeg2VEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stMp2V_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	IMPEG2VENC_Handle m_sEncHandle;
	IMPEG2VENC_Status m_sEstatus;
	unsigned char* m_pFrameBuffer[MPEG2ENC_HISTORY_BUFFS];
	int m_nFrameBufNum;
	int m_nInputFrameSize;
	unsigned char** p_outputBuffs;
#endif
	void SetDefaults();

public:
	CFilterMpeg2VEnc();
	CFilterMpeg2VEnc(stMp2V_h2dparams& params);
	~CFilterMpeg2VEnc();
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
void RunFilterTask(CFilterMpeg2VEnc * pFilter);
#endif

#endif // #ifndef _FILTERMPEG2VENC_H
