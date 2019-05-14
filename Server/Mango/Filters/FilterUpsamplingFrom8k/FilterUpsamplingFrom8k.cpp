/******************************************************************************
 * Copyright 2006 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterUpsamplingFrom8k.cpp
 *
 * GENERAL DESCRIPTION: From 8K Hz to 48K/36K Hz in 2 stages.
 *			For each stage:
 *                      Inflate size then 
 *                      Anti-Replication Low Pass Filter.
 *
 * DATE CREATED		AUTHOR
 *
 * June 06 			Ilan sinai
 *
 * REMARKS			Accepts buffer sizes up to 1152 samples.
 ******************************************************************************/
extern "C" {
#include "DSP_fltoq15.h"
#include "DSP_blk_move.h"
}
#include "FilterUpsamplingFrom8k.h"

#ifdef DSP
static int filterNum = 0;
#endif

#ifdef DSP
static const float s_fIFilterCoeffs[c_IntIFilterNumOfCoeffs+1+1] =
{
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
	0
};

static const float s_fIIFilterCoeffs[c_IntIIFilterNumOfCoeffs+1+1] =
{
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
	0  // Last one is dummy
};

// WARNING: Dont use sizeof fIFilterCoeffs since I inflated the size in order to have even number 
static short s_q15IFilterCoeffs[2*c_IntIFilterNumOfCoeffs+1];
static short s_q15IIFilterCoeffs[2*c_IntIIFilterNumOfCoeffs+1];
#endif
// This function is called by all constructors.
void CFilterUpsamplingFrom8k::SetDefaults()
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
	strcpy(m_strName, "CFilterUpsamplingFrom8k");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Default constructor
CFilterUpsamplingFrom8k::CFilterUpsamplingFrom8k()
{
	SetDefaults();
}

// Constructor with parameters
CFilterUpsamplingFrom8k::CFilterUpsamplingFrom8k(int nNumBufs)
{
	m_nNumBufs = nNumBufs;
	SetDefaults();
}

// Destructor
CFilterUpsamplingFrom8k::~CFilterUpsamplingFrom8k()
{
#ifdef DSP
	if (m_isInited)
	{
		// at this point the processing task has already been killed.

		// Delete output mailboxes
		MBX_delete(m_pOutPins[0].GetFullBuffsMbx());
		MBX_delete(m_pOutPins[0].GetFreeBuffsMbx());

		// Free output buffer(s)
		for (int i=0; i<m_nNumBufs; i++)
			MEM_free(seg_sdram, p_outputBuffs[i], ROUND_UP_CACHE_L2_LINESIZE(AUDIO_SAMPLES_PER_FRAME*c_InterpolationFactorII*c_InterpolationFactorI*sizeof(unsigned short)));
		delete []p_outputBuffs;

		// Free accumulator buffer
		MEM_free(seg_sdram, m_InAudBuf, c_InAudBufSize*sizeof(short));
		MEM_free(seg_sdram, m_TempBuf, c_TempBufSize*sizeof(short));
		MEM_free(seg_sdram, m_OutAudBuf, c_OutAudBufSize*sizeof(short));
	}
#endif
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterUpsamplingFrom8k::Serialize(char *pBuff, bool bStore)
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
bool CFilterUpsamplingFrom8k::Init()
{
	// Initialize output mailboxes
	m_pOutPins[0].SetFullBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	m_pOutPins[0].SetFreeBuffsMbx(MBX_create(sizeof(stMediaBuffer), m_nNumBufs, NULL));
	if (!m_pOutPins[0].GetFullBuffsMbx() || !m_pOutPins[0].GetFreeBuffsMbx())
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mbx_create);
		return false;
	}

	// Allocate algorithm buffers
	m_InAudBuf = (short*)MEM_calloc(seg_sdram, c_InAudBufSize*sizeof(short), CACHE_L2_LINESIZE);
	m_TempBuf = (short*)MEM_calloc(seg_sdram, c_TempBufSize*sizeof(short), CACHE_L2_LINESIZE);
	m_OutAudBuf = (short*)MEM_calloc(seg_sdram, c_OutAudBufSize*sizeof(short), CACHE_L2_LINESIZE);

	if (!m_InAudBuf || !m_TempBuf || !m_OutAudBuf)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	// Allocate output buffer(s) and submit to free mailbox.
	stMediaBuffer sOutBuf;
	p_outputBuffs = new unsigned char*[m_nNumBufs];
	if (!p_outputBuffs)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutput);
		return false;
	}

	for (int i=0; i<m_nNumBufs; i++)
	{
		p_outputBuffs[i] = (unsigned char*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(AUDIO_SAMPLES_PER_FRAME*c_InterpolationFactorII*c_InterpolationFactorI*sizeof(short)),
														CACHE_L2_LINESIZE);
		if (!p_outputBuffs[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_mem_poutbuf);
			return false;
		}

		sOutBuf.pBuffer = p_outputBuffs[i];
		sOutBuf.nMaxSize = AUDIO_SAMPLES_PER_FRAME*c_InterpolationFactorII*c_InterpolationFactorI*sizeof(short);
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

// This function is called to indicate to filter to start operating. At this point the
// graph is fully connected. This function normally creates the task that performs the
// filter's function.
bool CFilterUpsamplingFrom8k::Start()
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
void RunFilterTask(CFilterUpsamplingFrom8k * pFilter)
{
	pFilter->FilterTask();
}

// DSP Filter task
void CFilterUpsamplingFrom8k::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	sOutBuf.nSize = 0;
//	int i;

	Prepare();

	while(true)
	{
		// receive input data
		MBX_pend(m_pInPins[0].GetFullBuffsMbx(), &sInBuf, SYS_FOREVER);
		if (!sInBuf.nSize)
		{
			MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);
			sOutBuf.nSize = 0;
			MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);
			goto skip;
		}

		if (sInBuf.nSize > AUDIO_SAMPLES_PER_FRAME*sizeof(short))
		{
			report_error(ERRTYPE_ERROR, m_strName, glob_err_buf_overflow);
			return;	
		}

		// Copy end of previous buffer to start of InAudBuf, for history
		memcpy(m_InAudBuf, m_InAudBuf+sInBuf.nSize/sizeof(short), c_IntFilterIIPrefix*sizeof(short));
		memcpy(m_InAudBuf+c_IntFilterIIPrefix, sInBuf.pBuffer, sInBuf.nSize);

		Perform(sInBuf.nSize/sizeof(short));
		
		// receive empty output buffer
		MBX_pend(m_pOutPins[0].GetFreeBuffsMbx(), &sOutBuf, SYS_FOREVER);

		sOutBuf.nSize = sInBuf.nSize*c_InterpolationFactorII*c_InterpolationFactorI;
		memcpy(sOutBuf.pBuffer, m_OutAudBuf, sOutBuf.nSize);
		CACHE_wbL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);

		// send data buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free input buffer
		CACHE_invL2(sInBuf.pBuffer, sInBuf.nSize, CACHE_WAIT);
skip:
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

void CFilterUpsamplingFrom8k::Prepare()
{
	int i,j;
	// WARNING :due to the DSP_fltoq15 we need even number of coeff
	//so, the last one is dummy.
	DSP_fltoq15((float *)s_fIFilterCoeffs, s_q15IFilterCoeffs, c_IntIFilterNumOfCoeffs+1+1);
	DSP_fltoq15((float *)s_fIIFilterCoeffs, s_q15IIFilterCoeffs, c_IntIIFilterNumOfCoeffs+1+1);
	
	j = c_IntIIFilterNumOfCoeffs + 1;
	for (i = c_IntIIFilterNumOfCoeffs-1; i >= 0; i--, j++)
		s_q15IIFilterCoeffs[j] = s_q15IIFilterCoeffs[i];
	j = c_IntIFilterNumOfCoeffs + 1;
	for (i =c_IntIFilterNumOfCoeffs-1; i>= 0; i--, j++)
		s_q15IFilterCoeffs[j] = s_q15IFilterCoeffs[i];
}

void CFilterUpsamplingFrom8k::Perform(int nInSamples)
{
	memcpy(m_TempBuf, m_TempBuf+nInSamples*c_InterpolationFactorII, c_IntFilterIPrefix*sizeof(short));

	// Phase II: Interpolation & Low Pass Filter From 8K->16K,Factor is 2
	Interpolate_FirSym((short *)m_InAudBuf, s_q15IIFilterCoeffs, m_TempBuf+c_IntFilterIPrefix,
								c_IntIIFilterNumOfCoeffs,  nInSamples*c_InterpolationFactorII,
								c_InterpolationFactorII);

	// Phase I: Interpolation & Low Pass Filter From 16K->48K,Factor is 3
	Interpolate_FirSym((short *)m_TempBuf, s_q15IFilterCoeffs, m_OutAudBuf,
								c_IntIFilterNumOfCoeffs, nInSamples * c_InterpolationFactorII * c_InterpolationFactorI,
								c_InterpolationFactorI);
}
 
int CFilterUpsamplingFrom8k::Interpolate_FirSym(short* restrict x, short* restrict h, short* restrict r,
												int nh, int nr, int InterpolationFactor)
{
// we assume h are 0.15, x are 15.0. nh <= 128 so takes 7 bits
// On in all y0 store (15.0*0.15)*7.0 = Q22.15.
// r is short so usually we trunc with s = 15 to get rid of the fraction.
// In addition we takes the lower 15 bits of the 22 non-fraction part 
// I slide here the Taps over the Data. For each Data entry, the calc is made only for part of the Taps
// That thier corresponded data entry were non zero. In addition I slide this "non zero" ruller with i_base
// Example: InterpolationFactor = 2: O1 = I1*H1+I3*H3...   . O2 = I3*H2+I5*H4.... O3 = I3*H1+I5*H3.....
// Here it is more efficient to BREAK the symmetry, since we jump. i.e. I do NOT use (x[j + i] + x[j + 2 * nh - i]) * h[i]
#define s 15 
	int i, j, t;
	long y0;
	int Taps_base = 0; // used to point the starting point of the Coeff mult
	int Input_base = 0;
	long round = (long) 1 << (s - 1);
	for (j = 0; j < nr; j++)
	{
		y0 = round;
		for (i = Input_base, t = Taps_base; t < (2*nh+1) ; t += InterpolationFactor, i++)
			y0 += (long)(x[i] * h[t]);

		r[j] = (short) (y0 >> s);
		r[j] *= InterpolationFactor; // Since I skip some h , I need to amplify. In average (only) InterpolationFactor 
		Taps_base--;
		if ( Taps_base < 0 )
		{
		 	Taps_base = InterpolationFactor-1;
			Input_base++;
		}	
	}
	return j;
}

#endif // #ifdef DSP
