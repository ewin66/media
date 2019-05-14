/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH264VEnc.h
 *
 * GENERAL DESCRIPTION: (M)Jpeg video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/12/2005 		Kfir Yehezkel
 *
 ******************************************************************************/
#ifndef _FILTERH264VENC_H
#define _FILTERH264VENC_H

#include "MangoBios.h"
#include "MangoX_SharedExp.h"

#ifdef DSP
#include <std.h>
#include <alg.h>
#include <ialg.h>
#include <csl.h>
#include "ih264enc.h"
#include "h264enc.h"
#include "MangoX.h"

extern far SEM_Obj shared_scratch_SEM;
#endif

struct stH264V_h2dparams
{
	int nNumBufs;
	int nMaxOutputSize;
	enVideoStandard eVideoStandard;
	enImageSize eImageSize;
	int nBitrate;
	int nFrameRate;
	int nNumGOPFrames;
	int nQp0;
	int nQpN;
//	int nSearchRange;
//	int nNumRefrenceFrames;
	int nCustomWidth;
	int nCustomHeight;
};

// Runtime commands to filter (via input pin 1)
typedef enum {
	H264CMD_REOPEN,
	H264CMD_FORCE_IFRAME
} enH264VEncCmd;

typedef struct {
	enH264VEncCmd cmd;
	stH264V_h2dparams params;
} stH264VEncCmd;

class CFilterH264VEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stH264V_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables	
	int m_nFilterNum;
	char m_cmdStrName[MAX_FILTER_NAME];
	TSK_Handle m_hCmdTask;

	unsigned char** p_outputBuffs;
	unsigned int m_numBuffs;
	// encoder handle
	IH264ENC_Handle m_h264encHandle;
	IH264ENC_Status  m_h264encstatus;

#endif
	void SetDefaults();

public:
	CFilterH264VEnc();
	CFilterH264VEnc(stH264V_h2dparams& params);
	~CFilterH264VEnc();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
	void FilterCmdTask();
	void Reopen(stH264V_h2dparams& params);
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterH264VEnc * pFilter);
void RunFilterCmdTask(CFilterH264VEnc * pFilter);
#endif

#endif // _FILTERH264VENC_H
