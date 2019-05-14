/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterG711ADec.cpp
 *
 * GENERAL DESCRIPTION: G.711 Audio Decoding Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * <9 05 2006> 		<Moshe Shimoni>
 *
 ******************************************************************************/
#include "FilterG711ADec.h"

#ifdef DSP
static int filterNum = 0;
#endif

void CFilterG711ADec::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_A_G711);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_A_PCM);

	strcpy(m_strName, "CFilterG711ADec");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Constructor
CFilterG711ADec::CFilterG711ADec()
{
	SetDefaults();
}

CFilterG711ADec::CFilterG711ADec(stG711Adec_h2dparams& params)
{
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterG711ADec::~CFilterG711ADec()
{
#ifdef DSP
	if (m_isInited)
	{		
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
int CFilterG711ADec::Serialize(char *pBuff, bool bStore)
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
bool CFilterG711ADec::Init()
{    
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

bool CFilterG711ADec::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stackseg = seg_sdram;
	ta.stacksize = 0x800;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

// DSP Filter C-callable task function
void RunFilterTask(CFilterG711ADec * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterG711ADec::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	
	while(true)
	{
		// receive new audio to encode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// receive empty buffer to encode into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
		{
			sOutBuf.nSize = 0;
			goto skip;
		}

		G711DecodeFrame((eG711VocoderType)m_sParams.eVocoderType, 
						(char*)sInBuf.pBuffer, 	 
						(short*)sOutBuf.pBuffer,    
						sInBuf.nSize);
		
		sOutBuf.nSize = sInBuf.nSize * sizeof(short);

		if (sOutBuf.nSize > sOutBuf.nMaxSize)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_buf_overflow);
			return;
		}
		CACHE_invL2(sInBuf.pBuffer, sInBuf.nSize, CACHE_WAIT);
		CACHE_wbInvL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

skip:		
		// send encoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free image buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
