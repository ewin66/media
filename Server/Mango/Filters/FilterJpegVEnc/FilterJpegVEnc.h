/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterJpegVEnc.h
 *
 * GENERAL DESCRIPTION: (M)Jpeg video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/12/2005 		Kfir Yehezkel
 *
 ******************************************************************************/
#ifndef _FILTER_JPEG_VENC_H
#define _FILTER_JPEG_VENC_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"
#include "ijpegenc_mecoso.h"
extern far SEM_Obj shared_scratch_SEM;
extern "C" {
#include <alg.h>
#include <_alg.h>
}
#endif

struct stJpegV_h2dparams
{
	int nNumBufs;
	int nMaxOutputSize;
	int width;
	int height;
	int pitch;
	int jfif;
	int quality;
	enJpegFormat format;
};

// Runtime commands to filter (via input pin 1)
typedef struct {
	stJpegV_h2dparams params;
} stJpegVEncCmd;

class CFilterJpegVEnc: public CMangoXFilter
{
protected:

	// Member parameter variables
	stJpegV_h2dparams m_sParams;

#ifdef DSP
	// Internal member variables	
	int m_nFilterNum;
	char m_cmdStrName[MAX_FILTER_NAME];
	TSK_Handle m_hCmdTask;

	unsigned char** p_outputBuffs;
	unsigned int m_numBuffs;
	IJPEGENC_MECOSO_Params m_encParams;
	unsigned char* m_jpegencHandle;

#endif
	void SetDefaults();

public:
	CFilterJpegVEnc();
	CFilterJpegVEnc(stJpegV_h2dparams& params);
	~CFilterJpegVEnc();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
	void FilterCmdTask();
	void Reopen(stJpegV_h2dparams& params);
#endif
};

#ifdef DSP
// DSP Filter C-callable task function
void RunFilterTask(CFilterJpegVEnc * pFilter);
void RunFilterCmdTask(CFilterJpegVEnc * pFilter);
#endif

#endif // #ifndef _FILTER_JPEG_VENC_H
