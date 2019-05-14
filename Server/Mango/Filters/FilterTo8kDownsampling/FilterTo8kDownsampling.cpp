/******************************************************************************
 * Copyright 2006 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterTo8kDownsampling.cpp
 *
 * GENERAL DESCRIPTION: From 48K/32K Hz To 8K Hz in 2 stages. 
 *                      Anti-Aliasing Filter then Decimation in each stage.
 *
 * DATE CREATED		AUTHOR
 *
 * May 06 			Ilan sinai
 *
 * REMARKS			Accepts buffers of any size, but maximum is defined by
 *					first received buffer.
 ******************************************************************************/
extern "C" {
//#include "DSP_fltoq15.h"
//#include "DSP_blk_move.h"
}

#include "FilterTo8kDownsampling.h"

#ifdef DSP
static int filterNum = 0;
#endif

#ifdef DSP
static const float s_fIFilterCoeffs[c_DecIFilterNumOfCoeffs+1+1] = {
		1.9476095109E-04,
		6.8929442361E-04,
		1.4592943467E-03,
		2.0100028447E-03,
		1.2740424714E-03,
		-2.1086664071E-03,
		-8.9932176832E-03,
		-1.8635849362E-02,
		-2.7875538502E-02,
		-3.1282013873E-02,
		-2.2661409255E-02,
		2.3581859470E-03,
		4.3665737772E-02,
		9.5276707180E-02,
		1.4621278601E-01,
		1.8368621162E-01,
		1.9747669571E-01,
		0 };

static const float s_fIIFilterCoeffs[c_DecIIFilterNumOfCoeffs+1+1] = {

	9.3537120146E-05,
	-1.9131677409E-04,
	-2.0169959347E-03,
	-5.7627244530E-03,
	-8.9135697735E-03,
	-7.3498536102E-03,
	-7.3865114889E-04,
	4.8238332407E-03,
	3.1840966707E-03,
	-2.9752425422E-03,
	-4.4651701828E-03,
	1.3157357398E-03,
	5.4344057925E-03,
	6.4467495820E-04,
	-6.0168373440E-03,
	-3.0912839089E-03,
	5.8964496902E-03,
	5.9459678544E-03,
	-4.7278032081E-03,
	-8.9102359967E-03,
	2.2516507521E-03,
	1.1531724313E-02,
	1.6583044023E-03,
	-1.3253366647E-02,
	-6.9768167108E-03,
	1.3439464131E-02,
	1.3509632980E-02,
	-1.1392829577E-02,
	-2.0891294857E-02,
	6.3402104024E-03,
	2.8616473314E-02,
	2.7069843791E-03,
	-3.6097425069E-02,
	-1.7455808815E-02,
	4.2727049330E-02,
	4.2134189024E-02,
	-4.7939604594E-02,
	-9.2544926908E-02,
	5.1271472778E-02,
	3.1367975516E-01,
	4.4758242510E-01,
	0 };  // Last one is dummy

// WARNING: Dont use sizeof fIFilterCoeffs since I inflated the size in order to have even number 
static short s_q15IFilterCoeffs[c_DecIFilterNumOfCoeffs+1+1];
static short s_q15IIFilterCoeffs[c_DecIIFilterNumOfCoeffs+1+1];
#endif
// This function is called by all constructors.
void CFilterTo8kDownsampling::SetDefaults()
{
	// Set number of input and output pins for this filter here.
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];

	// Set the allowed input type and the output type for your pins here.
	m_pInPins[0].SetType(MT_A_PCM);
	m_pOutPins[0].SetType(MT_A_PCM);

	// This should be the same as the class's name
	strcpy(m_strName, "CFilterTo8kDownsampling");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterTo8kDownsampling::CFilterTo8kDownsampling()
{
	SetDefaults();
}

// Constructor with parameters
CFilterTo8kDownsampling::CFilterTo8kDownsampling(int nNumBufs)
{
	m_nNumBufs = nNumBufs;
	SetDefaults();
}

// Destructor
CFilterTo8kDownsampling::~CFilterTo8kDownsampling()
{
#ifdef DSP
	if (m_isInited)
	{
		// at this point the processing task has already been killed.

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free output buffer(s)
		if (m_sizeOutputBuffs)
			for (int i=0; i<m_nNumBufs; i++)
				MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(m_sizeOutputBuffs));
		delete []p_outputBuffs;

		// Free algorithm buffers
		if (m_in_aud)
		{
			MEM_free(seg_sdram, m_in_aud, m_in_aud_size);
			MEM_free(seg_sdram, m_TempBuffer1, m_TempBuffer1_size);
			MEM_free(seg_sdram, m_TempBuffer2, m_TempBuffer2_size);
		}
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterTo8kDownsampling::Serialize(char *pBuff, bool bStore)
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

// DSP side init; called after constructor but before graph is connected
bool CFilterTo8kDownsampling::Init()
{
	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	m_in_aud = m_TempBuffer1 = m_TempBuffer2 = NULL;
	m_sizeOutputBuffs = 0;

	p_outputBuffs = new unsigned char*[m_nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	m_isInited=1;
	return true;
}

// This function is called to indicate to filter to start operating. At this point the
// graph is fully connected. This function normally creates the task that performs the
// filter's function.
bool CFilterTo8kDownsampling::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stackseg = seg_isram;
	ta.stacksize = 0x400;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}

	// work around a bug that causes kernel/object view to crash CCS
	shuffle_task_list();

	return true;
}

// DSP Filter C-callable task function. This is required because DSP/BIOS can't directly call
// a class method.
void RunFilterTask(CFilterTo8kDownsampling * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterTo8kDownsampling::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	int maxInSize;

	// WARNING :due to the DSP_fltoq15 we need even number of coeff, so the last one is dummy.
	DSP_fltoq15((float *)s_fIFilterCoeffs, s_q15IFilterCoeffs, c_DecIFilterNumOfCoeffs+1+1);
	DSP_fltoq15((float *)s_fIIFilterCoeffs, s_q15IIFilterCoeffs, c_DecIIFilterNumOfCoeffs+1+1);

	while(true)
	{
		// receive input data
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);

		// Allocate output buffers if this is the first time
		if (!m_sizeOutputBuffs)
		{
			maxInSize = sInBuf.nSize;
			m_sizeOutputBuffs = maxInSize / (c_DecimationfactorI*c_DecimationfactorII);

			for (int i=0; i<m_nNumBufs; i++)
			{
				p_outputBuffs[i] = (unsigned char*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_sizeOutputBuffs), CACHE_L2_LINESIZE);
				if (!p_outputBuffs[i])
				{
					report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
					return;
				}

				sOutBuf.pBuffer = p_outputBuffs[i];
				sOutBuf.nMaxSize = m_sizeOutputBuffs;
				sOutBuf.nSize = 0;
				if (!MBX_post(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_POLL))
				{
					report_error(ERRTYPE_FATAL, m_strName, glob_err_mbx_post);
					return;
				}
			}
		}

		// receive empty output buffer
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// input buffer can't be larger than first received buffer
		if (sInBuf.nSize > maxInSize)
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_buf_overflow);
			return;	
		}

		// read data from sInBuf.pBuffer (size is sInBuf.nSize) and write output
		// data to sOutBuf.pBuffer. Set sOutBuf.nSize to the output buffer size.
		if (!Perform((short *)sInBuf.pBuffer, (short *)sOutBuf.pBuffer, sInBuf.nSize, &sOutBuf.nSize))
		{
			report_error(ERRTYPE_ERROR, m_strName, "Algorithm internal error");
			return;	
		}

		CACHE_invL2(sInBuf.pBuffer, sInBuf.nSize, CACHE_WAIT);
		CACHE_wbL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
	
		// send data buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free input buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

bool CFilterTo8kDownsampling::Perform(short *pDataIn, short *pDataOut, int nInSize, int* nOutSize)
{
	int Len = 0;
	int TargetLen = 0;
	int SourceLen = nInSize/sizeof(short);

	if (!m_in_aud)
	{
		m_in_aud_size = (SourceLen + c_DecFilterIPrefix)*sizeof(short);
		m_TempBuffer1_size = SourceLen*sizeof(short);
		m_TempBuffer2_size = (SourceLen/c_DecimationfactorI + c_DecIIFilterNumOfCoeffs*2)*sizeof(short);
		m_in_aud = (short*)MEM_alloc(seg_sdram, m_in_aud_size, 4);
		m_TempBuffer1 = (short*)MEM_alloc(seg_sdram, m_TempBuffer1_size, 4);
		m_TempBuffer2 = (short*)MEM_alloc(seg_sdram, m_TempBuffer2_size, 4);

		if (!m_in_aud || !m_TempBuffer1 || !m_TempBuffer2)
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
			return false;
		}
	}

	// Copy end of previous buffer to start of new buffer
	memcpy(m_in_aud, m_in_aud + SourceLen, c_DecFilterIPrefix*sizeof(short));
	// Copy new buffer to end of input buffer
	memcpy(m_in_aud + c_DecFilterIPrefix, pDataIn, nInSize);

	memcpy(m_TempBuffer2, m_TempBuffer2+SourceLen/c_DecimationfactorI, c_DecFilterIIPrefix*sizeof(short));

	// Phase I: AntiAliasing Filter From 48K->16K, then decimate by 3
	DownSampling_FirSym(m_in_aud, s_q15IFilterCoeffs, m_TempBuffer1, c_DecIFilterNumOfCoeffs,
						SourceLen);
	Decimate(m_TempBuffer1, m_TempBuffer2+c_DecFilterIIPrefix, SourceLen,
				&Len, c_DecimationfactorI);

	// Phase II: Antialiasing Filter From 16K->8K, then decimate by 2
	// assume that TempBuffer2 has margin/extra for the last rigths points.margin is (2*IIFilterNumOfCoeffs+1) long
	DownSampling_FirSym(m_TempBuffer2, s_q15IIFilterCoeffs, m_TempBuffer1, c_DecIIFilterNumOfCoeffs,
						(SourceLen/c_DecimationfactorI));
	Decimate(m_TempBuffer1, pDataOut, (SourceLen/c_DecimationfactorI), &TargetLen,
					c_DecimationfactorII);

	*nOutSize = TargetLen*sizeof(short);

	return true;
}

// Sort of "external" function. basiclly could be a static protected
bool CFilterTo8kDownsampling::Decimate(const short* restrict pSource, short* restrict pTarget,  
                               const int SourceLen, int *pTargetLen,const  int DecimateFactor)
{
	int i,j=0;
	ALIGNED_ARRAY(pSource);
	ALIGNED_ARRAY(pTarget);

	if (DecimateFactor <= 0 || SourceLen < 0)
		return false;

	*pTargetLen = 0;
	for (i=0; i < SourceLen; i+=DecimateFactor)
		pTarget[j++] = pSource[i];			

	*pTargetLen = j;
	return true;

} 

void CFilterTo8kDownsampling::DownSampling_FirSym(short* restrict x, short* restrict h, short* restrict r,
												  int nh, int nr)
{
// we assume h are 0.15, x are 15.0. nh <= 128 so takes 7 bits
// On in all y0 store (15.0*0.15)*7.0 = Q22.15.
// r is short so usually we trunc with s = 15 to get rid of the fraction.
// In addition we takes the lower 15 bits of the 22 non-fraction part 
#define s 15 
	int i, j;
	long y0;
	long round = (long) 1 << (s - 1);
	for (j = 0; j < nr; j++)
	{
		y0 = round;
		for (i = 0; i < nh; i++)
			y0 += (long)((x[j + i] + x[j + 2 * nh - i]) * h[i]);
		y0 += (long)(x[j + nh] * h[nh]);
		r[j] = (short) (y0 >> s);
	}
}

#endif // #ifdef DSP
