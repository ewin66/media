/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	MangoX_SharedExp.cpp
 *
 * GENERAL DESCRIPTION: MangoX Host/DSP shared headers.
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Yishay Hayardeni
 *
 ******************************************************************************/
#ifndef __MANGOX_SHAREDEXP_H__
#define __MANGOX_SHAREDEXP_H__

#include "MangoBios.h"

typedef enum {
	MX_NTSC,
	MX_PAL,
	MX_NUM_VIDEOSTDS
} enVideoStandard;

typedef enum {
	MX_S_VIDEO,
	MX_COMPOSITE_VIDEO
} enVideoAnalogConnection;

typedef enum {
	MX_MIC,
	MX_LINE_IN
} enAudioInputType;

typedef enum
{
	MX_MP2_STEREO,
	MX_MP2_JOINT_STEREO,
	MX_MP2_DUAL_CHANNEL,
	MX_MP2_SINGLE_CHANNEL
} enMp2AudioMode;

typedef enum
{
	MX_JPG_YUV400 = 0, 
	MX_JPG_YUV420,
	MX_JPG_YUV422,
	MX_JPG_FORMAT_NUM
} enJpegFormat;


typedef enum
{
	MX_G711_A_LAW,
	MX_G711_U_LAW
} enG711AudioMode;

typedef enum {
	MX_SQCIF,
	MX_QCIF,
	MX_CIF,
	MX_2CIF,
	MX_4CIF,
	MX_QVGA,
	MX_VGA,

	MX_CUSTOMSIZE,
	MX_NUM_IMGSIZES = MX_CUSTOMSIZE
} enImageSize;

#define AUDIO_SAMPLES_PER_FRAME	1152

typedef enum
{
	LOAD_VIDIN_I2C,
	LOAD_VIDOUT_I2C
} enConfigVideoCmds;

#define MAX_FILTER_NAME 48
#define NUM_CUSTOM_CMD_RETVALS	2

#ifdef DSP
	#include <std.h>
	#include <mbx.h>
	#include <tsk.h>
	#include <cstring>
	#include <iom.h>
	#include <csl_stdinc.h>
	#include <csl_cache.h>
	using namespace std;
	extern int seg_isram, seg_sdram;

#else
	#if defined(WIN32)
		#include <windows.h>
	#endif
	#include <stdlib.h>
	#include <string.h>
#endif // #ifdef DSP

typedef enum
{
	MT_ANY,

	MT_V_PLANAR_420,
	MT_V_MPEG4,
	MT_V_MPEG2,
	MT_V_H263,
	MT_V_H264,
	MT_V_JPEG,

	MT_A_PCM,
	MT_A_MPEG12L2,
	MT_A_MPEG12L3,
	MT_A_G711,
	
	MT_OSD_CMD,
	MT_MP4V_CMD,
	MT_H264V_CMD,
	MT_JPEG_CMD,

	MT_TIME_TAG
} enMediaType;

typedef struct
{
	void* pBuffer;
	int nSize;
	int nMaxSize;
} stMediaBuffer;

typedef enum {
	ERRTYPE_WARNING,
	ERRTYPE_ERROR,
	ERRTYPE_FATAL
} Error_Severity_E;

#ifdef DSP
#define SERIALIZE(buf, var) \
		memcpy(buf, &var, sizeof(var)); \
		buf+=sizeof(var);
#else
#define SERIALIZE(buf, var) \
		{ \
		for (int nTemp=0; nTemp<sizeof(var)/sizeof(int); nTemp++) \
			*((int *)buf+nTemp) = PCI_SWAP32(*((int*)&var+nTemp)); \
		buf+=sizeof(var); \
		}
#endif

#ifdef DSP
#define DESERIALIZE(buf, var) \
		memcpy(&var, buf, sizeof(var)); \
		buf+=sizeof(var);
#else
#define DESERIALIZE(buf, var) \
		{ \
		for (int nTemp=0; nTemp<sizeof(var)/sizeof(int); nTemp++) \
			*((int *)&var+nTemp) = PCI_SWAP32(*((int*)buf+nTemp)); \
		buf+=sizeof(var); \
		}
#endif

class CMangoXPin
{
public:
	void SetType(enMediaType Type){m_Type = Type;};
	void SetRequired(bool bRequired){m_bRequired = bRequired;};
	enMediaType GetType(){return m_Type;};
	CMangoXPin(){m_bConnected=false; m_Type=MT_ANY; m_bRequired=true;};
	~CMangoXPin(){};
	virtual int Serialize(char *pBuff, bool bStore);
	bool IsConnected(){return m_bConnected;};
	bool IsRequired(){return m_bRequired;};

protected:
	bool m_bConnected;
	enMediaType m_Type;
	bool m_bRequired; // whether this pin must be connected to complete graph. Normally true
};

class CMangoXOutPin: public CMangoXPin
{
public:
	CMangoXOutPin(){m_nDestFilterId=-1; m_nDestPinId=-1;};
	~CMangoXOutPin(){};
	int GetDestFilterId(){return m_nDestFilterId;};
	void SetDestFilterId(int nDestFilterId){m_nDestFilterId=nDestFilterId;};
	int GetDestPinId(){return m_nDestPinId;};
	void SetDestPinId(int nDestPinId){m_nDestPinId=nDestPinId;};
	int Serialize(char *pBuff, bool bStore);
	void SetConnected(bool bConnected){m_bConnected=bConnected;};
#ifdef DSP
	inline MBX_Handle GetFullBuffsMbx(){return m_FullBuffsMbx;};
	inline MBX_Handle GetFreeBuffsMbx(){return m_FreeBuffsMbx;};
	inline void SetFullBuffsMbx(MBX_Handle m){m_FullBuffsMbx = m;};
	inline void SetFreeBuffsMbx(MBX_Handle m){m_FreeBuffsMbx = m;};
#endif
protected:
	int m_nDestFilterId;
	int m_nDestPinId;
#ifdef DSP
	MBX_Handle m_FullBuffsMbx;
	MBX_Handle m_FreeBuffsMbx;
#endif
};

class CMangoXInPin: public CMangoXPin
{
public:
	CMangoXInPin(){m_pSourcePin = NULL; m_nSourceFilterId=-1;};
	~CMangoXInPin(){};
	bool Connect(CMangoXOutPin *pSourcePin);
	int Serialize(char *pBuff, bool bStore);
	bool Attach(CMangoXOutPin *pSourcePin, int nSourceFilterId);
#ifdef DSP
	inline MBX_Handle GetFullBuffsMbx(){return m_pSourcePin->GetFullBuffsMbx();};
	inline MBX_Handle GetFreeBuffsMbx(){return m_pSourcePin->GetFreeBuffsMbx();};
#endif
protected:
	CMangoXOutPin *m_pSourcePin;
	int m_nSourceFilterId;
};

class CMangoXFilter
{
public:
	char * GetName();
	virtual int Serialize(char *pBuff, bool bStore);
	CMangoXInPin *GetInPin(int nInPinIndex);
	CMangoXOutPin *GetOutPin(int nOutPinIndex);
	int GetInPinsCount(){return m_nNumInPins;};
	int GetOutPinsCount(){return m_nNumOutPins;};
	bool Attach(CMangoXOutPin *pSourcePin, int nSourceFiltIndex, int nDestFiltIndex, int nDestPinIndex);
	virtual ~CMangoXFilter(){};
#ifdef DSP
	CMangoXFilter(){m_hTask = NULL;};
	CMangoXFilter(CMangoXFilter &Src){m_hTask = NULL;};
	virtual bool Start(){return true;};
	virtual void Stop();
	virtual bool Init(){m_nNumInPins=0; m_nNumOutPins=0; return false;};
	bool Connect(CMangoXOutPin *pSourcePin);
	virtual void FilterTask() = 0;
	bool IsInited(){return m_isInited;};
#else
	CMangoXFilter(){};
	CMangoXFilter(CMangoXFilter &Src){};
#endif
protected:
	CMangoXInPin *m_pInPins;
	CMangoXOutPin *m_pOutPins;
	int m_nNumInPins;
	int m_nNumOutPins;
	char m_strName[MAX_FILTER_NAME];
#ifdef DSP
	bool m_isInited;
	TSK_Handle m_hTask;
#endif
};

typedef CMangoXFilter* cbConstructFunc(char *strClassName);

class CMangoXGraph
{
public:
	bool SetFilter(CMangoXFilter *pFilter, int nFilterIndex);
	CMangoXFilter *GetFilter(int nFilterIndex);
	CMangoXGraph();
	CMangoXGraph(int nNumFilters);
	~CMangoXGraph();
	void SetConstructFunc(cbConstructFunc *pConstructFunc);
	bool Attach(int SrcFiltIndex, int DstFiltIndex, int nSrcPinIndex, int DstPinIndex);
	int Serialize(char *pBuff, bool bStore);
#ifdef DSP
	bool Create();
	bool Start();
#endif
protected:
	int m_nNumFilters;
	CMangoXFilter **m_pFilters;
	cbConstructFunc *m_pfnConstructFunc;
};

#endif /* __MANGOX_SHAREDEXP_H__ */
