/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg4VEnc.h
 *
 * GENERAL DESCRIPTION: Mpeg-4 video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTERMPEG4VENC_H
#define _FILTERMPEG4VENC_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"

#include "mp4spenc.h"
#include "mp4spencv393_prs.h"

#define MPEG4_ENC_V_MAJ 3
#define MPEG4_ENC_V_MIN 93

extern far SEM_Obj shared_scratch_SEM;

// Uncomment this definition to ignore encoder's request to skip frames.
//#define MP4ENC_IGNORE_FRAMESKIP

#endif

typedef enum {
	RC_CONSTANT_Q,
	RC_VBR,
	RC_VBR_NON_DROP,
	RC_CBR,
	RC_CBR_NON_DROP
} enRCMode;

typedef struct {
	int nNumBufs;
	int nMaxOutputSize;
	enVideoStandard eVideoStandard;
	enImageSize eImageSize;
	int nFramerateDivisor;
	int nNumGOPFrames;
	enRCMode eRC_mode;
	int nConstantBitrate;
	int nAvgBitrate;
	int nMaxBitrate;
	int nQinitial;
	int nQmax;
	int nQmin;
	int nCodecBufsize;
	int nCustomWidth;
	int nCustomHeight;
} stMp4V_h2dparams;

// Runtime commands to filter (via input pin 1)
typedef enum {
	MP4CMD_REOPEN,
	MP4CMD_FORCE_IFRAME
} enMpeg4VEncCmd;

typedef struct {
	enMpeg4VEncCmd cmd;
	stMp4V_h2dparams params;
} stMpeg4VEncCmd;

class CFilterMpeg4VEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stMp4V_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables
	int m_nFilterNum;
	char m_cmdStrName[MAX_FILTER_NAME];
	TSK_Handle m_hCmdTask;

	unsigned char** p_outputBuffs;
	MP4SPENC_Handle	m_sEncHandle;
	MP4SPENC_Client m_sClient;
	int m_nMsPerFrame;
	int m_nSkipRC;
	int m_nSkipCnt;
	int m_nMp4First;
	int m_bForceIFrame;
#endif
	void SetDefaults();

public:
	CFilterMpeg4VEnc();
	CFilterMpeg4VEnc(stMp4V_h2dparams& params);
	~CFilterMpeg4VEnc();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
	void FilterCmdTask();
	void Reopen(stMp4V_h2dparams& params);
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterMpeg4VEnc * pFilter);
void RunFilterCmdTask(CFilterMpeg4VEnc * pFilter);
#endif

#endif // #ifndef _FILTERMPEG4VENC_H
