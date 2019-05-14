/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterMpeg2VEnc.cpp
 *
 * GENERAL DESCRIPTION: Mpeg-2 video encoder filter.
 *
 * DATE CREATED		AUTHOR
 *
 * 04/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "MangoBios.h"
#include "FilterMpeg2VEnc.h"

#ifdef DSP
#include "MangoAV_HWExp.h"
static int filterNum = 0;
void set_mpeg2_params(IMPEG2VENC_Params* mpeg2veparam, int width, int height,
				  int bitrate, int framerate, int framesInGOP, int b_frames_count);

static char max_fps_tvsystem[] = {30, 25};

extern short x_sizes[MX_NUM_IMGSIZES];
extern short y_sizes[MX_NUM_VIDEOSTDS][MX_NUM_IMGSIZES];

#endif // #ifdef DSP

void CFilterMpeg2VEnc::SetDefaults()
{
	m_nNumInPins=1; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pInPins[0].SetType(MT_V_PLANAR_420);
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];
	m_pOutPins[0].SetType(MT_V_MPEG2);

	strcpy(m_strName, "CFilterMpeg2VEnc");
#ifdef DSP
	m_isInited=0;
	m_hTask = NULL;
#endif
}

// Constructor
CFilterMpeg2VEnc::CFilterMpeg2VEnc()
{
	SetDefaults();
}

CFilterMpeg2VEnc::CFilterMpeg2VEnc(stMp2V_h2dparams& params)
{
	m_sParams = params;
	SetDefaults();
}

// Destructor
CFilterMpeg2VEnc::~CFilterMpeg2VEnc()
{
#ifdef DSP
	int i;
	
	if (m_isInited)
	{
		MPEG2VENC_delete(m_sEncHandle);
		
		// Free reference buffers
		for (i=0; i<MPEG2ENC_HISTORY_BUFFS; i++)
			if (m_pFrameBuffer[i])
				MEM_free(seg_sdram, m_pFrameBuffer[i], ROUND_UP_CACHE_L2_LINESIZE(m_nInputFrameSize));

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
int CFilterMpeg2VEnc::Serialize(char *pBuff, bool bStore)
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
bool CFilterMpeg2VEnc::Init()
{
	IMPEG2VENC_Params eparams;
	int frame_size_x, frame_size_y;
	int i;
	
	frame_size_x = x_sizes[m_sParams.eImageSize];
	frame_size_y = y_sizes[m_sParams.eVideoStandard][m_sParams.eImageSize];

	m_nInputFrameSize = frame_size_x * frame_size_y * 3/2;
	
	// check parameters
	if (m_sParams.nNumGOPFrames % (m_sParams.nNumBFrames+1))
	{
		report_error(ERRTYPE_ERROR, m_strName, "nNumGOPFrames must be multiple of (nNumBFrames+1)");
		return false;
	}
	set_mpeg2_params(&eparams,
						frame_size_x, frame_size_y,
						m_sParams.nConstantBitrate,
						max_fps_tvsystem[m_sParams.eVideoStandard],
						m_sParams.nNumGOPFrames,
						m_sParams.nNumBFrames);

	SEM_pend(&shared_scratch_SEM, SYS_FOREVER);

	// Create Mpeg-2 encoder instance
	m_sEncHandle = (IMPEG2VENC_Handle)MPEG2VENC_create(
								(const IMPEG2VENC_Fxns *)&MPEG2VENC_TI_IMPEG2VENC,
								NULL, (const IMPEG2VENC_Params *)&eparams);
	SEM_post(&shared_scratch_SEM);
	if (!m_sEncHandle)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_alg_handle);
		return false;
	}

	// Allocate several frame buffers (reference frames)
	for (i=0; i<MPEG2ENC_HISTORY_BUFFS; i++)
	{
		m_pFrameBuffer[i] = (unsigned char*)MEM_alloc(seg_sdram, ROUND_UP_CACHE_L2_LINESIZE(m_nInputFrameSize), CACHE_L2_LINESIZE);
		if (!m_pFrameBuffer[i])
		{
			report_error(ERRTYPE_ERROR, m_strName, "can't allocate reference frames");
			return false;
		}
	}
	m_nFrameBufNum = 0;

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

bool CFilterMpeg2VEnc::Start()
{
	TSK_Attrs ta;
	ta = TSK_ATTRS;
	sprintf(m_strName+strlen(m_strName), "_%04x", filterNum);
	m_nFilterNum = filterNum++;
	ta.name = m_strName;
	ta.stackseg = seg_isram;
	ta.stacksize = 0x2000;
	m_hTask = TSK_create((Fxn)RunFilterTask, &ta, this);
	if (!m_hTask)
	{
		report_error(ERRTYPE_ERROR, m_strName, glob_err_tsk_create);
		return false;
	}
	shuffle_task_list();

	return true;
}

void CFilterMpeg2VEnc::Stop()
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
void RunFilterTask(CFilterMpeg2VEnc * pFilter)
{
	pFilter->FilterTask();
}


// DSP Filter task
void CFilterMpeg2VEnc::FilterTask()
{
	stMediaBuffer sInBuf, sOutBuf;
	unsigned char* encIn[3];
	int frame_size_x, frame_size_y, encoded_counter=0;

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

		// copy to history buffer (mpeg-2 needs 4 frames of history)
		qdma_memcpy(m_pFrameBuffer[m_nFrameBufNum], sInBuf.pBuffer, sInBuf.nSize);
		qdma_memcpy_wait();
		
		frame_size_x = x_sizes[m_sParams.eImageSize];
		frame_size_y = y_sizes[m_sParams.eVideoStandard][m_sParams.eImageSize];

		encIn[0] = m_pFrameBuffer[m_nFrameBufNum];
		encIn[1] = encIn[0] + frame_size_x * frame_size_y;
		encIn[2] = encIn[0] + frame_size_x * frame_size_y * 5/4;

		SEM_pend(&shared_scratch_SEM, SYS_FOREVER);
		
		// Activate Encoder Instance
		MPEG2VENC_TI_activate((IMPEG2VENC_Handle)m_sEncHandle);
		
		// Encode the frame
		MPEG2VENC_encode((IMPEG2VENC_Handle)m_sEncHandle,
							(unsigned char **)&encIn, (unsigned int*)sOutBuf.pBuffer, encoded_counter++);
		
		// Read encoder status
		MPEG2VENC_TI_control((IMPEG2VENC_Handle)m_sEncHandle,
								IMPEG2VENC_GETSTATUS, &m_sEstatus);
		
		// Deactivate Encoder Instance
		MPEG2VENC_TI_deactivate((IMPEG2VENC_Handle)m_sEncHandle);

		SEM_post(&shared_scratch_SEM);

		sOutBuf.nSize = m_sEstatus.nWords*4;

		m_nFrameBufNum = (m_nFrameBufNum + 1) % MPEG2ENC_HISTORY_BUFFS;

		// to know what kind of frame encoded: m_sEncHandle.pict_type = 1-I, 2-P, 3-B

		if ((unsigned int)sOutBuf.nSize >= sOutBuf.nMaxSize)
		{
			report_error(ERRTYPE_FATAL, m_strName, glob_err_buf_overflow);
			return;
		}

		large_CACHE_wbL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
		large_CACHE_invL2(sOutBuf.pBuffer, sOutBuf.nSize, CACHE_WAIT);
skip:		
		// send encoded buffer
		MBX_post(m_pOutPins[0].GetFullBuffsMbx(), &sOutBuf, SYS_FOREVER);

		// free image buffer
		MBX_post(m_pInPins[0].GetFreeBuffsMbx(), &sInBuf, SYS_FOREVER);
	}
}

/******************************************************************************
 *
 * FUNCTION: set_mpeg2_params
 *
 * GENERAL DESCRIPTION:	Fills parameters in Mpeg-2 encoder structs.
 *                              
 * Input:	mpeg2veparam - pointer to parameter struct
 *			width - width of image
 *			height - height of image
 *			bitrate - bitrate for encoding, in bits/sec
 *			framerate - frames per second, accepts 25 or 30.
 *			framesInGOP - size of GOP, standard is 15.
 *			IPframeDistance - standard is 2, use 0 to disable B-frames
 *
 * Output:	none
 *
 ******************************************************************************/
void set_mpeg2_params(IMPEG2VENC_Params* mpeg2veparam, int width, int height,
				  int bitrate, int framerate, int framesInGOP, int b_frames_count)
{
	int i;
	int IPframeDistance = (b_frames_count > framesInGOP? 1 : b_frames_count+1);
	char* temp_string = "-";
//	mpeg2veparam->size = sizeof(IMPEG2VENC_Params); // prob not needed
	mpeg2veparam->h_recon_all = 0;
	memcpy(mpeg2veparam->h_id_string, temp_string, strlen(temp_string)+1);
	memcpy(mpeg2veparam->h_tplorg, temp_string, strlen(temp_string)+1); /* name of source file */
	memcpy(mpeg2veparam->h_tplref, temp_string, strlen(temp_string)+1); /* name of reconstructed images ("-": don't store) */
	memcpy(mpeg2veparam->h_iqname, temp_string, strlen(temp_string)+1); /* name of intra quant matrix file ("-": default matrix) */
	memcpy(mpeg2veparam->h_niqname, temp_string, strlen(temp_string)+1); /* name of non intra quant matrix file ("-": default matrix) */ 
	memcpy(mpeg2veparam->h_statname, temp_string, strlen(temp_string)+1); /* name of statistics file ("-": stdout ) */
	mpeg2veparam->h_inputtype = 0; /* input picture file format: 0=*.Y,*.U,*.V, 1=*.yuv, 2=*.ppm */
	mpeg2veparam->h_nframes = 0x7fffffff; /* number of frames */
	mpeg2veparam->h_frame0 = 0; /* number of first frame */
	mpeg2veparam->h_h = mpeg2veparam->h_m = 
	mpeg2veparam->h_s = mpeg2veparam->h_f1 = 0; /* timecode of first frame */
	mpeg2veparam->h_N = framesInGOP;        	/* N (# of frames in GOP) */
	mpeg2veparam->h_M = IPframeDistance; /* M (I/P frame distance. Enables B-frames, 1:IPP, 2:IBPBP, 3:IBBPBBP) */
	mpeg2veparam->h_mpeg1 = 0;         	/* ISO/IEC 11172-2 stream */
	mpeg2veparam->h_fieldpic = 0;         	/* 0:frame pictures, 1:field pictures */
	mpeg2veparam->h_horizontal_size = width; //720;          /* horizontal size */
	mpeg2veparam->h_vertical_size = height; // 480;          /* vertical size */
	mpeg2veparam->h_aspectratio = 2;         	/* aspect_ratio_information 1=square pel, 2=4:3, 3=16:9, 4=2.11:1 */
	mpeg2veparam->h_frame_rate_code = (framerate == 30? 4 : 3); //5;         	/* frame_rate_code 1=23.976, 2=24, 3=25, 4=29.97, 5=30 frames/sec. */
	mpeg2veparam->h_bit_rate = bitrate; // 15000000.0; 	/* bit_rate (bits/s) */
	mpeg2veparam->h_vbv_buffer_size = 112;        	/* vbv_buffer_size (in multiples of 16 kbit) */
	mpeg2veparam->h_low_delay = 0;         	/* low_delay  */
	mpeg2veparam->h_constrparms = 0;         	/* constrained_parameters_flag */
	mpeg2veparam->h_profile = 4;         	/* Profile ID: Simple = 5, Main = 4, SNR = 3, Spatial = 2, High = 1 */
	mpeg2veparam->h_level = 8;         	/* Level ID:   Low = 10, Main = 8, High 1440 = 6, High = 4          */
	mpeg2veparam->h_prog_seq = 0;         	/* progressive_sequence */
	mpeg2veparam->h_chroma_format = 1;         	/* chroma_format: 1=4:2:0, 2=4:2:2, 3=4:4:4 */
	mpeg2veparam->h_video_format = (framerate == 30? 2:1); 	/* !!!! video_format: 0=comp., 1=PAL, 2=NTSC, 3=SECAM, 4=MAC, 5=unspec. */
	mpeg2veparam->h_color_primaries = 5;         	/* color_primaries */
	mpeg2veparam->h_transfer_characteristics = 5;         	/* transfer_characteristics */
	mpeg2veparam->h_matrix_coefficients = 4;         	/* matrix_coefficients */
	mpeg2veparam->h_display_horizontal_size = width; //720;          /* horizontal display size */
	mpeg2veparam->h_display_vertical_size = height; //480;          /* vertical display size */
	mpeg2veparam->h_dc_prec = 0;         	/* intra_dc_precision (0: 8 bit, 1: 9 bit, 2: 10 bit, 3: 11 bit */
	mpeg2veparam->h_topfirst = 0;         	/* top_field_first */
	
	mpeg2veparam->h_frame_pred_dct_tab[0] = mpeg2veparam->h_frame_pred_dct_tab[1] =
	mpeg2veparam->h_frame_pred_dct_tab[2] = 0; /* frame_pred_frame_dct (I P B) */
	
	mpeg2veparam->h_conceal_tab[0] = mpeg2veparam->h_conceal_tab[1] =
	mpeg2veparam->h_conceal_tab[2] = 0; /* concealment_motion_vectors (I P B) */

	mpeg2veparam->h_qscale_tab[0] = mpeg2veparam->h_qscale_tab[1] = 
		mpeg2veparam->h_qscale_tab[2] = 1; /* q_scale_type  (I P B) */

	mpeg2veparam->h_intravlc_tab[0] = 1;
	mpeg2veparam->h_intravlc_tab[1] =
		mpeg2veparam->h_intravlc_tab[2] = 0; /* intra_vlc_format (I P B)*/
	
	mpeg2veparam->h_altscan_tab[0] = mpeg2veparam->h_altscan_tab[1] =
		mpeg2veparam->h_altscan_tab[2] = 0; /* alternate_scan (I P B) */
	
	mpeg2veparam->h_repeatfirst = 0;         	/* repeat_first_field */
	mpeg2veparam->h_prog_frame = 0;         	/* progressive_frame */
  
	/* ---------------------------------------------------------------------- */
	/* intra slice interval refresh period                                    */
	/* ---------------------------------------------------------------------- */
	mpeg2veparam->h_P        = 0;         	/* P distance between complete intra slice refresh */
	mpeg2veparam->h_r        = 0;         	/* rate control: r (reaction parameter) */
	mpeg2veparam->h_avg_act  = 0;         	/* rate control: avg_act (initial average activity) */
	mpeg2veparam->h_Xi       = 0;         	/* rate control: Xi (initial I frame global complexity measure) */
	mpeg2veparam->h_Xp       = 0;         	/* rate control: Xp (initial P frame global complexity measure) */
	mpeg2veparam->h_Xb       = 0;         	/* rate control: Xb (initial B frame global complexity measure) */
	mpeg2veparam->h_d0i      = 0;         	/* rate control: d0i (initial I frame virtual buffer fullness) */
	mpeg2veparam->h_d0p      = 0;         	/* rate control: d0p (initial P frame virtual buffer fullness) */
	mpeg2veparam->h_d0b      = 0;         	/* rate control: d0b (initial B frame virtual buffer fullness) */
	
	/* ---------------------------------------------------------------------- */
	/* For P pictures. Get motion_picdata in motion_picdata[h_M-1]            */
	/* Parameters in following order                                          */
	/* int forw_hor_f_code,forw_vert_f_code,int sxf,int syf                   */
	/* int back_hor_f_code,back_vert_f_code,int sxb,int syb                   */
	/* ---------------------------------------------------------------------- */
  
	/* P:  forw_hor_f_code forw_vert_f_code search_width/height */
	mpeg2veparam->h_motion_picdata[(mpeg2veparam->h_M-1)*8] = 6;
	mpeg2veparam->h_motion_picdata[(mpeg2veparam->h_M-1)*8+1] = 5;
	mpeg2veparam->h_motion_picdata[(mpeg2veparam->h_M-1)*8+2] = 15;
	mpeg2veparam->h_motion_picdata[(mpeg2veparam->h_M-1)*8+3] = 15;
	
	// This was originally a for (i=0; i<mpeg2veparam->h_M-1; i++) loop
	// h_M = 3, therefore i=0 and 1.
	
	i=0;
	/* B1: forw_hor_f_code forw_vert_f_code search_width/height */
	mpeg2veparam->h_motion_picdata[i*8] = 4;
	mpeg2veparam->h_motion_picdata[i*8+1] = 4;
	mpeg2veparam->h_motion_picdata[i*8+2] = 31;
	mpeg2veparam->h_motion_picdata[i*8+3] = 31;
	
	/* B1: back_hor_f_code back_vert_f_code search_width/height */
	mpeg2veparam->h_motion_picdata[i*8+4] = 5;
	mpeg2veparam->h_motion_picdata[i*8+5] = 5;
	mpeg2veparam->h_motion_picdata[i*8+6] = 31;
	mpeg2veparam->h_motion_picdata[i*8+7] = 31;
	
	i=1;
	/* B2: forw_hor_f_code forw_vert_f_code search_width/height */
	mpeg2veparam->h_motion_picdata[i*8] = 5;
	mpeg2veparam->h_motion_picdata[i*8+1] = 5;
	mpeg2veparam->h_motion_picdata[i*8+2] = 15;
	mpeg2veparam->h_motion_picdata[i*8+3] = 15;
	
	/* B2: back_hor_f_code back_vert_f_code search_width/height */
	mpeg2veparam->h_motion_picdata[i*8+4] = 4;
	mpeg2veparam->h_motion_picdata[i*8+5] = 4;
	mpeg2veparam->h_motion_picdata[i*8+6] = 31;
	mpeg2veparam->h_motion_picdata[i*8+7] = 31;
	
}

#endif // #ifdef DSP
