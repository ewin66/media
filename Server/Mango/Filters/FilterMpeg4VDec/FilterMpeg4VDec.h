/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg4VDec.h
 *
 * GENERAL DESCRIPTION: Mpeg-4 video decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERMPEG4VDEC_H
#define _FILTERMPEG4VDEC_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"

#include "mp4spdec.h"
#include "mp4spdecv271_prs.h"

#define MPEG4_DEC_V_MAJ 2
#define MPEG4_DEC_V_MIN 71

extern far SEM_Obj shared_scratch_SEM;

#endif

class CFilterMpeg4VDec: public CMangoXFilter
{
protected:

	// Member parameter variables
	int m_nNumBufs;
	int m_bFastMode;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	unsigned char** p_outputBuffs;
	int m_nMaxOutputSize;
	MP4SPDEC_InfoFirst m_sInfoFirst;
	MP4SPDEC_InfoFrame m_sInfoFrame;
	MP4SPDEC_Handle m_sDecHandle;
	unsigned char *m_pCurrent, *m_pPrevious;
	int m_nMp4First;
#endif
	void SetDefaults();

public:
	CFilterMpeg4VDec();
	CFilterMpeg4VDec(int nNumBufs, bool bFastMode);
	~CFilterMpeg4VDec();
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
void RunFilterTask(CFilterMpeg4VDec * pFilter);
#endif

#endif // #ifndef _FILTERMPEG4VDEC_H
