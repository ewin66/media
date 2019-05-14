/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg2VDec.h
 *
 * GENERAL DESCRIPTION: Mpeg-2 video decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERMPEG2VDEC_H
#define _FILTERMPEG2VDEC_H

#include "MangoX_Shared.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"

#include "mpeg2vdec.h"
#include "mpeg2vdec_ti.h"
#define MPEG2DEC_IN_BUFFSIZE 0x20000
#define MPEG2_MAXPARAM 5

#define MPEG2_DEC_V_MAJ 1
#define MPEG2_DEC_V_MIN 2

extern far SEM_Obj shared_scratch_SEM;

#endif

class CFilterMpeg2VDec: public CMangoXFilter
{
protected:

	// Member parameter variables
	int m_nNumBufs;
	enImageSize m_eMaxImageSize;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
	int m_nMaxOutputSize;
	int m_nMp2First;

	MPEG2VDEC_Handle m_sDecHandle;
	
#endif
	void SetDefaults();

public:
	CFilterMpeg2VDec();
	CFilterMpeg2VDec(int nNumBufs, enImageSize eMaxImageSize);
	~CFilterMpeg2VDec();
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
void RunFilterTask(CFilterMpeg2VDec * pFilter);
#endif

#endif // #ifndef _FILTERMPEG2VDEC_H
