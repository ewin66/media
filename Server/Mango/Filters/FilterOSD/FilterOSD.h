/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterOSD.h
 *
 * GENERAL DESCRIPTION: OSD filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 14/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef _FILTEROSD_H
#define _FILTEROSD_H

#include "MangoX_SharedExp.h"

#ifdef DSP
#include "MangoX.h"
#include "MangoAV_HWExp.h"
#include <string.h>
#endif

#define MAX_OSD_ENTRIES	16

// any definitions (enums, structs etc) can go here.
typedef enum {
	MX_OSD_ADD_ENTRY,
	MX_OSD_MODIFY_ENTRY,
	MX_OSD_MODIFY_DATA,
	MX_OSD_DELETE_ENTRY
} enOSDCommand;

typedef enum {
	MX_OSD_TEXT,
	MX_OSD_BMP, // 4:2:0 YUV image
	MX_OSD_LINE
} enOSDType;

typedef enum {
	MX_OSD_TEXT_SIZE_SMALL,
	MX_OSD_TEXT_SIZE_MEDIUM,
	MX_OSD_TEXT_SIZE_LARGE
} enOSDTextSize;
	
typedef struct {
	enOSDCommand cmd;
	int entryNum;
	enOSDType type;
	enOSDTextSize textSize;
	int bmp_width, bmp_height; // for bitmap only
	
// modifiable parameters. note MODIFIABLE_OFFSET define below!
	int isVisible;
	int isTransparent;
	int isYOnly;
	int fore_Y, fore_Cb, fore_Cr;
	int back_Y, back_Cb, back_Cr;
	int x, y;
	int x2, y2; // for line only
} stOSDData;

#ifdef DSP
#define MODIFIABLE_OFFSET	6

typedef struct {
	stOSDData data;
	int is_valid;
	unsigned char* buf;
	int bufsize;
	int buf_width;
	int disp_width;
	int disp_height;
	unsigned char* ptr_Y, *ptr_Cb, *ptr_Cr, *text_buf;
} stOSDInternalData;

typedef struct
{
	unsigned short data[16];
} Small_Char_T;

typedef struct
{
	unsigned int data[32];
} Large_Char_T;

#endif

class CFilterOSD: public CMangoXFilter
{
protected:

	// Member parameter variables (shared between host and dsp) go here.
	// For example, number of buffers and max. output size.
	int m_eImageSize;
	int m_eVideoStandard;

#ifdef DSP
	int m_nFilterNum;
	char m_cmdStrName[MAX_FILTER_NAME];

	// Internal DSP-only member variables go here
	int m_nFrameWidth;
	int m_nFrameHeight;
	stOSDInternalData osddb[MAX_OSD_ENTRIES];
	
	TSK_Handle m_hDataTask;
	SEM_Handle m_hMutexSem;
#endif
	void SetDefaults();

public:
	CFilterOSD();
	CFilterOSD(enImageSize eImageSize, enVideoStandard eVideoStandard);
	~CFilterOSD();
	int Serialize(char *pBuff, bool bStore);

#ifdef DSP
	bool Init();
	bool Start();
	void Stop();
	void FilterTask();
	void FilterDataTask();
#endif
};

#ifdef DSP
// This is the C-callable task function that runs on the DSP.
void RunFilterTask(CFilterOSD * pFilter);
void RunFilterDataTask(CFilterOSD * pFilter);
#endif

#endif // #ifndef _FILTEROSD_H
