/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterJpegVEnc.cpp
 *
 * GENERAL DESCRIPTION: (M)Jpeg video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/12/2005 		Kfir Yehezkel
 *
 ******************************************************************************/
#include "MangoBios.h"
#include "FilterJpegVDec.h"

static int filterNum = 0;

void CFilterJpegVDec::SetDefaults()
{
	m_nNumInPins = 1; 
	m_nNumOutPins = 1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_JPEG);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_PLANAR_420);

	strcpy(m_strName, "CFilterJpegVDec");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Constructor
CFilterJpegVDec::CFilterJpegVDec()
{	
	SetDefaults();
}

CFilterJpegVDec::CFilterJpegVDec(int nNumBufs, int pitch)
{	
	m_nNumBufs = nNumBufs;
	m_pitch = pitch;
	SetDefaults();
}

// Destructor
CFilterJpegVDec::~CFilterJpegVDec()
{
#ifdef DSP
	if (m_isInited)
	{
		MEM_free(seg_sdram, m_jpegdecHandle, JPEGDEC_INSTANCE_SIZE);

		// TEMP
		MEM_free(seg_sdram, m_bakbuf, 0x40000);

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free memory allocated to output buffer(s)
		for (int i=0; i<m_nNumBufs; i++)
			if (outputBuffs[i].pBuffer) // only if allocated.
				MEM_free(seg_sdram, outputBuffs[i].pBuffer, outputBuffs[i].nSize);

		delete []outputBuffs;
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer
int CFilterJpegVDec::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_nNumBufs);
		SERIALIZE(p, m_pitch);
	}
	else
	{
		DESERIALIZE(p, m_nNumBufs);
		DESERIALIZE(p, m_pitch);
	}
	
	return p-pBuff;
}

#ifdef DSP

extern void* shared_isram;

extern "C" void jpegd_create(void* instance, void* isram);
extern "C" void jpegd_setpitch(void* instance, int pitch);
extern "C" int jpegd_decode(void* instance, 
							 unsigned char* bitstream,
							 unsigned char* image_ptr[3],
							 int bitstream_len);

// DSP side init

bool CFilterJpegVDec::Init()
{
	// allocate room in SDRAM for instance.
	m_jpegdecHandle = (unsigned char*)MEM_alloc(seg_sdram, JPEGDEC_INSTANCE_SIZE, 8);

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
	jpegd_create(m_jpegdecHandle, shared_isram);
	SEM_post(&shared_scratch_SEM);

	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	// Allocate output buffer(s)
	outputBuffs = new stMediaBuffer[m_nNumBufs];
	if (!outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
		return false;
	}

	for (int i=0; i<m_nNumBufs; i++)
	{		
		// submit to free mailbox
		outputBuffs[i].pBuffer = NULL;
		outputBuffs[i].nMaxSize = 0;
		outputBuffs[i].nSize = 0;
		if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), outputBuffs + i , SYS_POLL))
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
			return false;
		}			
	}
	
	// TEMP: allocate spare buffer
	m_bakbuf = (unsigned char*)MEM_alloc(seg_sdram, 0x40000, 4);
	m_isInited=1;
	return true;
}

bool CFilterJpegVDec::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stacksize = 2048;
	
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

void CFilterJpegVDec::Stop()
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
void RunFilterTask(CFilterJpegVDec * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterJpegVDec::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	unsigned char* out_ptr[3];
	IJPEGDEC_MECOSO_Status dec_stat;
	int output_size , stat;
	
	while(true)
	{
		// receive new image to encode
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// receive empty buffer to encode into
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		if (!sInBuf.nSize) // empty buffer?
		{
			sOutBuf.nSize = 0;
			goto skip; // skip to next buffer
		}

		// read header
		stat = jpegd_decode(m_jpegdecHandle, (unsigned char*)sInBuf.pBuffer, (unsigned char**)&dec_stat, -1);
		
		if (stat) // corrupt header?
		{			
			report_error(ERRTYPE_FATAL, m_strName, "decoder error parsing headers");
			goto skip; // skip to next buffer
		}
		output_size	= dec_stat.width * dec_stat.height;
		// set output size according to format
		if (dec_stat.format == 1) // yuv 420
			output_size = (output_size * 3) / 2;
		else if (dec_stat.format == 0) // grayscale			
			output_size *=1;			
		else if (dec_stat.format == 2) // yuv 422
			output_size *= 2;
		else // shouldn't happen
		{			
			report_error(ERRTYPE_FATAL, m_strName, "unknown format");
			goto skip; // skip to next buffer
		}
			
		// check that output buffer is big enough for this image, (re)allocate if needed
		sOutBuf.nSize = output_size;
		if (sOutBuf.nMaxSize < output_size)
		{
			// free if already allocated
			if (sOutBuf.pBuffer)				
				MEM_free(seg_sdram, sOutBuf.pBuffer, sOutBuf.nMaxSize);
								
			sOutBuf.pBuffer = MEM_alloc(seg_sdram, output_size, 8);
			if (!sOutBuf.pBuffer)
			{
				report_error(ERRTYPE_FATAL, m_strName, glob_err_mem_poutbuf);
 				return;
			}				
			sOutBuf.nMaxSize = output_size;
		}
					
 		// set Y, U and V output buffer pointers.
		out_ptr[0] = (unsigned char*) sOutBuf.pBuffer;

		if (dec_stat.format == 0) // gray scale
		{
			out_ptr[1] = out_ptr[0];
			out_ptr[2] = out_ptr[0];
		}
		else if (dec_stat.format == 1) // yuv 420
		{
			out_ptr[1] = out_ptr[0] + dec_stat.width * dec_stat.height;
			out_ptr[2] = out_ptr[1] + dec_stat.width * dec_stat.height / 4;
		}
		else if (dec_stat.format == 2) // yuv 422
		{
			out_ptr[1] = out_ptr[0] + dec_stat.width * dec_stat.height;
			out_ptr[2] = out_ptr[1] + dec_stat.width * dec_stat.height / 2;
		}

		// TEMP
		memcpy(m_bakbuf, sInBuf.pBuffer, sInBuf.nSize);

	    // call the decoder
		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		// TEMP
    	stat = jpegd_decode(m_jpegdecHandle, m_bakbuf, out_ptr, sInBuf.nSize);
    	//stat = jpegd_decode(m_jpegdecHandle, (unsigned char*)sInBuf.pBuffer, out_ptr, sInBuf.nSize);
		SEM_post(&shared_scratch_SEM);

    	if (stat < 0) // decoder error?
    	{    	
			report_error(ERRTYPE_WARNING, m_strName, "decoder error");
    	}

		if (stat != sInBuf.nSize) // decoded more or less bytes than received?
		{
			report_error(ERRTYPE_WARNING, m_strName, "bitstream length error");
		}
skip:
		// send encoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free image buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

#endif // #ifdef DSP
