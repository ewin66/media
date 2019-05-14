/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterH263VDec.cpp
 *
 * GENERAL DESCRIPTION: H.263 video decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 22/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterH263VDec.h"

#ifdef DSP
static IH263DECP_Handle decParent = NULL;
static int filterNum = 0;
#endif

// Constructor
void CFilterH263VDec::SetDefaults()
{
	m_nNumInPins=1;
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_H263);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_PLANAR_420);

	strcpy(m_strName, "CFilterH263VDec");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
	if (!decParent)
	{
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		// create decoder parent instance
		decParent = (IH263DECP_Handle)H263DECP_create((const IH263DECP_Fxns   *)&H263DECP_TI_IALG,
		                      							(const IH263DECP_Params *)NULL);
		SEM_post(&shared_scratch_SEM);
	}
#endif
}
// Constructor
CFilterH263VDec::CFilterH263VDec()
{
	SetDefaults();
}

CFilterH263VDec::CFilterH263VDec(int nNumBufs)
{
	m_nNumBufs = nNumBufs;
	SetDefaults();
}

// Destructor
CFilterH263VDec::~CFilterH263VDec()
{
#ifdef DSP
	if (m_isInited)
	{
		H263DEC_delete(m_sDecHandle);

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free output buffer(s)
		for (int i=0; i<m_nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize));
		delete []p_outputBuffs;
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer
int CFilterH263VDec::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
	}

	return p-pBuff;
}

#ifdef DSP

// DSP side init
bool CFilterH263VDec::Init()
{
	if (!decParent)
	{
		report_error(ERRTYPE_ERROR, m_strName, "Can't create decoder parent");
		return false;
	}

	// Create H.263 decoder instance
	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	m_sDecHandle = (IH263DEC_Handle)H263DEC_create((const IH263DEC_Fxns *)&H263DEC_TI_IH263DEC,
                                     decParent, (const IH263DEC_Params *)NULL);
	SEM_post(&shared_scratch_SEM);
	if (!m_sDecHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}
	m_nH263First = 1;
	
	H263DEC_control((IH263DEC_Handle)m_sDecHandle, IH263DEC_CLRSTATUS, &m_sDs);

	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	p_outputBuffs = new unsigned char*[m_nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	m_isInited=1;
	return true;
}

bool CFilterH263VDec::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

void CFilterH263VDec::Stop()
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
void RunFilterTask(CFilterH263VDec * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterH263VDec::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	unsigned char* decOut[3];

	while(true)
	{
		// receive new bitstream to decode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
		{
			sOutBuf.nSize = 0;
			sOutBuf.pBuffer = NULL;
			MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
			MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
			goto skip;
		}
		
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		H263DEC_decode((IH263DEC_Handle)m_sDecHandle, (unsigned int*)sInBuf.pBuffer, (unsigned char **)&decOut);
		H263DEC_control((IH263DEC_Handle)m_sDecHandle, IH263DEC_GETSTATUS, &m_sDs);
		SEM_post(&shared_scratch_SEM);

		// Decode error? send buffer of size 0
		if (m_sDs.retVal)
		{
			report_error(ERRTYPE_WARNING, m_strName, "Decoder error");
			sOutBuf.nSize = 0;
			sOutBuf.pBuffer = NULL;
			MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
			MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
			goto skip;
		}
		
		if (m_nH263First)
		{
			// First decode - Find size, allocate output buffer and submit to free mailbox
			m_nMaxOutputSize = m_sDs.width * m_sDs.height * 3/2;
			for (int i=0; i<m_nNumBufs; i++)
			{
				p_outputBuffs[i] = (Uint8*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nMaxOutputSize), CACHE_L2_LINESIZE);
				if (!p_outputBuffs[i])
				{
					report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
					return;
				}
				sOutBuf.pBuffer = p_outputBuffs[i];
				sOutBuf.nMaxSize = m_nMaxOutputSize;
				sOutBuf.nSize = 0;
				if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
				{
					report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
					return;
				}
			}
			m_nH263First = 0;
		}

		// receive empty buffer to copy decoded frame into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
		
		qdma_memcpy(sOutBuf.pBuffer, m_sDs.y, m_nMaxOutputSize);
		large_CACHE_invL2(sOutBuf.pBuffer, m_nMaxOutputSize, CACHE_WAIT);
		qdma_memcpy_wait();

		sOutBuf.nSize = m_nMaxOutputSize;

		// send decoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
skip:
		// free bitstream buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
