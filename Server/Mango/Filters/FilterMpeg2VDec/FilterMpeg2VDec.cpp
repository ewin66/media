/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg2VDec.cpp
 *
 * GENERAL DESCRIPTION: Mpeg-2 video decoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "MangoBios.h"
#include "FilterMpeg2VDec.h"

#ifdef DSP
#include "MangoAV_HWExp.h"
static int filterNum = 0;

extern short x_sizes[MX_NUM_IMGSIZES];
extern short y_sizes[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES];

#endif

// Constructor
void CFilterMpeg2VDec::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_MPEG2);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_PLANAR_420);

	strcpy(m_strName, "CFilterMpeg2VDec");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}
// Constructor
CFilterMpeg2VDec::CFilterMpeg2VDec()
{
	SetDefaults();
}

CFilterMpeg2VDec::CFilterMpeg2VDec(int nNumBufs, enImageSize eMaxImageSize)
{
	m_nNumBufs = nNumBufs;
	m_eMaxImageSize = eMaxImageSize;

	SetDefaults();
}

// Destructor
CFilterMpeg2VDec::~CFilterMpeg2VDec()
{
#ifdef DSP
	if (m_isInited)
	{
		ALG_delete((ALG_Handle)m_sDecHandle);

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
int CFilterMpeg2VDec::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_eMaxImageSize);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_eMaxImageSize);
	}

	return p-pBuff;
}

#ifdef DSP

// DSP side init
bool CFilterMpeg2VDec::Init()
{
	IMPEG2VDEC_Params decParams;

	// Create Mpeg-2 decoder instance
	decParams.size = sizeof(IMPEG2VDEC_Params);
	decParams.maxFrameWidth = x_sizes[m_eMaxImageSize];
	decParams.maxFrameHeight = y_sizes[MX_PAL][m_eMaxImageSize];

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	m_sDecHandle = (MPEG2VDEC_Handle)ALG_create((IALG_Fxns *)&MPEG2VDEC_TI_IMPEG2VDEC, NULL, (IALG_Params *)&decParams);
	SEM_post(&shared_scratch_SEM);

	if (!m_sDecHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}
	m_nMp2First = 1;

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

bool CFilterMpeg2VDec::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
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

void CFilterMpeg2VDec::Stop()
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
void RunFilterTask(CFilterMpeg2VDec * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterMpeg2VDec::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	int *in[MPEG2_MAXPARAM];
	int *out[MPEG2_MAXPARAM];
	DECODE_OUT decode_out;
	int i;
	int frame_size_x=0, frame_size_y=0;

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

		// Swap endianness
#pragma MUST_ITERATE(4,,4)
		for (i=0; i<Align_32Bit(sInBuf.nSize/4); i++)
			((unsigned int*)sInBuf.pBuffer)[i] = 
				_rotl(_swap4(((unsigned int*)sInBuf.pBuffer)[i]), 16);

		large_CACHE_wbL2(sInBuf.pBuffer, sInBuf.nSize, CACHE_WAIT);
		in[2] = (int*)sInBuf.pBuffer;
		in[3] = (int*)sInBuf.nSize;
		out[1] = (int*)&decode_out;

		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		
		/* activate instance object */
		ALG_activate((ALG_Handle)m_sDecHandle);
		
		m_sDecHandle->fxns->decode(m_sDecHandle, in, out);
		
		/* deactivate instance object */
		ALG_deactivate((ALG_Handle)m_sDecHandle);
		
		SEM_post(&shared_scratch_SEM);
		
		if (m_nMp2First)
		{
			frame_size_x = decode_out.initial_params.horizontal_size;
			frame_size_y = decode_out.initial_params.vertical_size;
			m_nMaxOutputSize = frame_size_x * frame_size_y * 3/2;

			// Allocate output buffer(s) and submit to free mailbox
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
			m_nMp2First = 0;
		}
		
		// receive empty buffer to put decoded image into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (decode_out.outputing)
		{
			unsigned char *out_data = (unsigned char*)sOutBuf.pBuffer;
			
			// copy decoded image to output buffer
			qdma_memcpy(out_data, decode_out.outframe[0], frame_size_x*frame_size_y);

			// to save time, perform this cache_invL2 while transfer is occurring
			large_CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

			qdma_memcpy_wait();
			qdma_memcpy(out_data+frame_size_x*frame_size_y,
						decode_out.outframe[1], frame_size_x*frame_size_y/4);
			qdma_memcpy_wait();
			qdma_memcpy(out_data+frame_size_x*frame_size_y*5/4,
						decode_out.outframe[2], frame_size_x*frame_size_y/4);
			qdma_memcpy_wait();

			sOutBuf.nSize = m_nMaxOutputSize;
		}
		else
			sOutBuf.nSize = 0;

		// send decoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
skip:
		// free bitstream buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
