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
#ifndef _FILTER_JPEG_VDEC_H
#define _FILTER_JPEG_VDEC_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"
#include "ijpegdec_mecoso.h"
extern far SEM_Obj shared_scratch_SEM;

extern "C" {
#include <alg.h>
#include <_alg.h>
}

#endif

class CFilterJpegVDec: public CMangoXFilter
{
protected:
	// Member parameter variables
	int m_nNumBufs;
	int m_pitch;
#ifdef DSP
	// Internal member variables	
	int m_nFilterNum;
	stMediaBuffer* outputBuffs;
	unsigned int m_numBuffs;	
	unsigned char* m_jpegdecHandle;
	unsigned int m_maxAllocatedBuff;
	unsigned char* m_bakbuf;

#endif
	void SetDefaults();

public:
	CFilterJpegVDec();
	CFilterJpegVDec(int nNumBufs, int pitch);
	~CFilterJpegVDec();
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
void RunFilterTask(CFilterJpegVDec * pFilter);
#endif

#endif // #ifndef _FILTER_JPEG_VDEC_H
