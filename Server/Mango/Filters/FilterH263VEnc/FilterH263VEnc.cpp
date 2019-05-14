/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH263VEnc.cpp
 *
 * GENERAL DESCRIPTION: H.263 video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 22/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterH263VEnc.h"

#ifdef DSP
static IH263ENCP_Handle encParent = NULL;
static int filterNum = 0;

extern short x_sizes[MX_NUM_IMGSIZES];
extern short y_sizes[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES];

static char h263_formats[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES] =
{
	{
		H263_SRCFMT_SRCFBDN,	// MX_SQCIF
		H263_SRCFMT_SRCFBDN,	// MX_QCIF
		H263_SRCFMT_SRCFBDN,	// MX_CIF
		H263_SRCFMT_SRCFBDN,	// MX_2CIF
		H263_SRCFMT_SRCFBDN,	// MX_4CIF
		H263_SRCFMT_SRCFBDN,	// MX_QVGA 
		H263_SRCFMT_SRCFBDN		// MX_VGA
	},
	{
		H263_SRCFMT_SQCIF,		// MX_SQCIF,
		H263_SRCFMT_QCIF,		// MX_QCIF,
		H263_SRCFMT_CIF,		// MX_CIF,
		H263_SRCFMT_SRCFBDN,	// MX_2CIF,
		H263_SRCFMT_SRCFBDN,	// MX_4CIF,
		H263_SRCFMT_SRCFBDN,	// MX_QVGA
		H263_SRCFMT_SRCFBDN		// MX_VGA
	}
};

#endif // #ifdef DSP

void CFilterH263VEnc::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_PLANAR_420);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_H263);

	strcpy(m_strName, "CFilterH263VEnc");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
	if (!encParent)
	{
		// create encoder parent instance
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		encParent = (IH263ENCP_Handle)H263ENCP_create((const IH263ENCP_Fxns   *)&H263ENCP_TI_IALG,
		                      							(const IH263ENCP_Params *)NULL);
		SEM_post(&shared_scratch_SEM);
	}
#endif
}

// Constructor
CFilterH263VEnc::CFilterH263VEnc()
{
	SetDefaults();
}

CFilterH263VEnc::CFilterH263VEnc(stH263V_h2dparams& params)
{
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterH263VEnc::~CFilterH263VEnc()
{
#ifdef DSP
	if (m_isInited)
	{
		H263ENC_delete(m_sEncHandle);

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());
	
		// Free output buffer(s)
		for (int i=0; i<m_sParams.nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_sParams.nMaxOutputSize));
		delete []p_outputBuffs;
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer
int CFilterH263VEnc::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_sParams);
	}
	else
	{
		DESERIALIZE(p, m_sParams);
	}
	
	return p-pBuff;
}

#ifdef DSP

// DSP side init
bool CFilterH263VEnc::Init()
{
	IH263ENC_Params eparams = IH263ENC_PARAMS;

	if (!encParent)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Can't create encoder parent");
		return false;
	}
	
	eparams.srcFormat = h263_formats[m_sParams.eVideoStandard][m_sParams.eImageSize];
	if (eparams.srcFormat == H263_SRCFMT_SRCFBDN)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Invalid format");
		return false;
	}
    eparams.bitRate = m_sParams.nBitrate;
	eparams.frameRate = m_sParams.nFramerate;
	eparams.intraRate = m_sParams.nNumGOPFrames;
    eparams.maxQ = m_sParams.nQmax;
    eparams.minQ = m_sParams.nQmin;
    eparams.qi = m_sParams.nQI; 
	eparams.nMB2proc = x_sizes[m_sParams.eImageSize] / 16;
	
	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	m_sEncHandle = (IH263ENC_Handle)H263ENC_create((const IH263ENC_Fxns *)&H263ENC_TI_IH263ENC,
							encParent,
							(const IH263ENC_Params *)&eparams);
	SEM_post(&shared_scratch_SEM);
	if (!m_sEncHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}

	// clear encoder status structure
	H263ENC_control((IH263ENC_Handle)m_sEncHandle, IH263ENC_CLRSTATUS, &m_sEs);

	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_sParams.nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_sParams.nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	// Allocate output buffer(s) and submit to free mailbox
	stMediaBuffer sOutBuf;
	p_outputBuffs = new unsigned char*[m_sParams.nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}
	
	for (int i=0; i<m_sParams.nNumBufs; i++)
	{
		p_outputBuffs[i] = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_sParams.nMaxOutputSize), CACHE_L2_LINESIZE);
		if (!p_outputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}
		sOutBuf.pBuffer = p_outputBuffs[i];
		sOutBuf.nMaxSize = m_sParams.nMaxOutputSize;
		sOutBuf.nSize = 0;
		if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
			return false;
		}
	}
	m_isInited=1;
	return true;
}

bool CFilterH263VEnc::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stackseg = seg_isram;
	ta.stacksize = 0x1000;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

void CFilterH263VEnc::Stop()
{
	if (m_hTask)
	{
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		TSK_delete(m_hTask);
		SEM_post(&shared_scratch_SEM);
		m_hTask = NULL;
	}
}

// DSP Filter C-callable task function
void RunFilterTask(CFilterH263VEnc * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterH263VEnc::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	unsigned char* encIn[3];
	int frame_size_x, frame_size_y;

	while(true)
	{
		// receive new image to encode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// receive empty buffer to encode into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
		{
			sOutBuf.nSize = 0;
			goto skip;
		}

		frame_size_x = x_sizes[m_sParams.eImageSize];
		frame_size_y = y_sizes[m_sParams.eVideoStandard][m_sParams.eImageSize];

		encIn[0] = (Uint8*)sInBuf.pBuffer;
		encIn[1] = encIn[0] + frame_size_x * frame_size_y;
		encIn[2] = encIn[0] + frame_size_x * frame_size_y * 5/4;

		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		H263ENC_encode((IH263ENC_Handle)m_sEncHandle, (unsigned char**)&encIn, (unsigned int*)sOutBuf.pBuffer);
		H263ENC_control((IH263ENC_Handle)m_sEncHandle, IH263ENC_GETSTATUS, &m_sEs);
		SEM_post(&shared_scratch_SEM);

		sOutBuf.nSize = m_sEs.nWords * 4;

		if (sOutBuf.nSize >= sOutBuf.nMaxSize)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_buf_overflow);
			return;
		}
		large_CACHE_wbL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
		large_CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

/*  Remember to flip endianness before writing to PC file! Uncomment the
	following code. */

/*		for (i=0; i<Align_32Bit(sOutBuf.nSize)/4; i++)
			((unsigned int*)sOutBuf.pBuffer)[i] = 
				_rotl(_swap4(((unsigned int*)sOutBuf.pBuffer)[i]), 16);

		// write back and invalidate (clean buffer out of cache)
		CACHE_wbInvL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
*/
skip:		
		// send encoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free image buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
