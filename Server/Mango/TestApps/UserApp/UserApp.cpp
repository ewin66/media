/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	UserApp.cpp
 *
 * GENERAL DESCRIPTION: MangoX multiple example application
 *
 * DATE CREATED		AUTHOR
 *
 * 7/08/05 		Itay Chamiel
 *
 * REMARKS:		This application contains multiple examples demonstrating
 *				usage of MangoX. Each example is activated by defining its
 *				appropriate preprocessor label at compile time.
 *
 ******************************************************************************/
//#error "To activate an example, comment out this line and #define one of the test names."

// Define one of:

// MP4LOOP, VIDIN_MP4_FILE, VIDIN_MP2_FILE, VIDIN_H263_FILE,
// WAV_TO_MP3, MP3_TO_PCM, VIDLOOP_OSD.
// H264_ENC_LOOP VIDIN_H264_FILE, G711_LOOP

#define VIDIN_H264_FILE

/* Define the following to properly close (reset) the board before exiting the
   program. It is currently undefined because if you are debugging with a JTAG,
   it may cause CCS to crash.
   Do define it if you are currently not debugging the DSP with a JTAG, and for
   product release.
   Also, always define it when running under Solaris.
*/
//#define PROPER_SHUTDOWN

#include "MangoBios.h"

#include <stdlib.h>
#include <stdio.h>

#include "MangoXExp.h"
#include "MangoC64BoardsExp.h"

#include "MangoBoards.h"

#include "MangoX_SharedExp.h"
#include "FilterD2H_PCI/FilterD2H_PCI.h"
#include "FilterH2D_PCI/FilterH2D_PCI.h"
#include "FilterVideoIn/FilterVideoIn.h"
#include "FilterVideoOut/FilterVideoOut.h"
#include "FilterMpeg4VDec/FilterMpeg4VDec.h"
#include "FilterMpeg4VEnc/FilterMpeg4VEnc.h"
#include "FilterMpeg2VDec/FilterMpeg2VDec.h"
#include "FilterMpeg2VEnc/FilterMpeg2VEnc.h"
#include "FilterH263VDec/FilterH263VDec.h"
#include "FilterH263VEnc/FilterH263VEnc.h"
#include "FilterMp3AEnc/FilterMp3AEnc.h"
#include "FilterH264VEnc/FilterH264VEnc.h"
#include "FilterMp3ADec/FilterMp3ADec.h"
#include "FilterG711AEnc/FilterG711AEnc.h"
#include "FilterG711ADec/FilterG711ADec.h"
#include "FilterOSD/FilterOSD.h"

#define CIF_SIZE (352*288*3/2)
#define CIF4_SIZE (704*576*3/2)
#define DSP0_NUM 0
#define DSP1_NUM 1

// Define how many times should the stream run
#define STREAM_ITERATIONS 10

#define WAV_HEADER_LENGTH 44

// This is the error callback function called if the DSP reports an error.
void err_callback(void * err_arg, int card_num, int dsp_num, Error_Severity_E err_type, int err_code, char * err_string){

	printf("Card %d DSP %d ", card_num, dsp_num);

	switch(err_type)
	{
	case ERRTYPE_WARNING:
		printf("Warning ");
		break;
	case ERRTYPE_ERROR:
		printf("Error ");
		break;
	case ERRTYPE_FATAL:
		printf("Fatal ");
		break;
	default:
		printf("Undefined error type (should never happen)! ");
		break;
	}
	printf("code %d: %s\n", err_code, err_string);
}

// In case of a runtime error, a clean abort must be done rather than just return or exit.
void clean_abort()
{
	int ret;

#ifdef PROPER_SHUTDOWN
	if ((ret=MangoX_Close()) != MANGOERROR_SUCCESS)
		printf("Error 0x%08x in MangoX_close!\n", ret);
#endif

	printf("Error, aborting\n");
	exit(-1);
}

// Main program function, performs different tasks depending on compile
int main(int argc, char *argv[])
{
	MANGOBIOS_deviceHandle_t *devices;
	MangoC64Boards_handle_t card;
	int num_dev, card_num;
	int i, num_dsps, ret;
	int counter = 0;
	h2d_pci_device_t ** pciDevArr;
	char ** coff_fnames;
	char ** fpga_fnames;
	MANGOX_ERR_CB * err_cbs;
	int cnt=0;
	
	CMangoXGraph *pGraph, *pGraphIn, *pGraphOut;
	CFilterH2D_PCI *h2d_filter;
	CFilterH2D_PCI *h2d_filter_osd;
	CFilterVideoIn *vidin_filter;
	CFilterVideoOut *vidout_filter;
	stMp4V_h2dparams mp4_params;
	stG711Aenc_h2dparams g711_enc_params;
	stG711Adec_h2dparams g711_dec_params;
	CFilterMpeg4VEnc *mp4enc_filter;
	CFilterD2H_PCI *d2h_filter;
	CFilterG711AEnc* g711enc_filter;
	CFilterG711ADec* g711dec_filter;
	int graph_num;
	int size;
	FILE* fp, *fpw;
	PCI_STREAM_ptr_t buf, buf2;
	int stream_iter;
	int camin_dsp = 0;
	int tvout_dsp;
	int camin_cam = 0;
	stMp2V_h2dparams mp2enc_params;
	CFilterMpeg2VEnc *mp2enc_filter;
	CFilterH263VEnc *h263enc_filter;
	stH263V_h2dparams h263enc_params;
	stMP3A_h2dparams mp3enc_params;
	CFilterMp3AEnc *mp3enc_filter;
	CFilterMp3ADec *mp3dec_filter;
	CFilterOSD*	osd_filter;
	int extra_cnt, bytes_read;
	int graphIn_num, graphOut_num;
	stOSDData osd_params;

	if (argc > 1)
	{
		fprintf(stderr, "argv[1]=%s\n", argv[1]);
		char* sourceName = argv[1];
		if (strcmp(sourceName, "Vid1") == 0)
		{
			camin_dsp = 0;
			camin_cam = 0;
		}
		else if (strcmp(sourceName, "Vid2") == 0)
		{
			camin_dsp = 0;
			camin_cam = 2;
		}
		else if (strcmp(sourceName, "Vid3") == 0)
		{
			camin_dsp = 1;
			camin_cam = 0;
		} else if (strcmp(sourceName, "Vid4") == 0)
		{
			camin_dsp = 1;
			camin_cam = 2;
		}
		else
		{
			fprintf(stderr, "usage: userapp [Vid1|Vid2|Vid3|Vid4]\n");
			exit(1);
		}
	}

	char osd_text[] = "Mango DSP";
	char* out_fname;
	char fpga_fullname[50];

	char out_fname_6415[] = 
#if defined(WIN32)
#ifdef _DEBUG
		"MangoX_C6415_D.out";
#else
		"/Mango/MangoBIOS/Libraries/MangoX/DSP/Release_C6415/MangoX_C6415_R.out";
#endif
#elif defined(linux) || defined(__SVR4)
		"/usr/Mango/binHost/MangoX_C6415_R.out";
#endif

	char out_fname_dm[] = 
#if defined(WIN32)
#ifdef _DEBUG
		"MangoX_DM_D.out";
#else
		"/Mango/MangoBIOS/Libraries/MangoX/DSP/Release_DM/MangoX_DM_R.out";
#endif
#elif defined(linux) || defined(__SVR4)
		"/usr/Mango/binHost/MangoX_DM_R.out";
#endif

	char fpga_path[] =
#if defined(WIN32)
		"";
#elif defined(linux) || defined(__SVR4)
		"";
#elif defined(__vxworks)
		"Host:/Mango/binHost/";
#endif

	if(MANGOBIOS_open(NULL) != MANGOERROR_SUCCESS)
	{
		fprintf(stderr, "MANGOBIOS_open(NULL) not SUCCESS\n");
		return -1;
	}
	if(MANGOBIOS_getNumDevices(NULL, &num_dev) != MANGOERROR_SUCCESS)
		return -1;
	if (num_dev < 1)
	{
		printf("No DSP devices found!\n");
		return -1;
	}

	devices = (MANGOBIOS_deviceHandle_t*)malloc(num_dev * sizeof(MANGOBIOS_deviceHandle_t));
	if (MANGOBIOS_getDeviceHandles(NULL, devices) != MANGOERROR_SUCCESS)
		return -1;

	printf("Attempting to open board\n");
	if (MangoC64Boards_Open(&card, devices, num_dev, &SEAGULL_PC104_PLUS_BOARD, NULL) == MANGOERROR_SUCCESS)
	{
		printf("Found %s\n", SEAGULL_PC104_PLUS_BOARD.name);
		out_fname = out_fname_dm;
		camin_dsp = DSP0_NUM;
		tvout_dsp = -1;
	}
	else if (MangoC64Boards_Open(&card, devices, num_dev, &PHOENIXHX_BOARD, NULL) == MANGOERROR_SUCCESS)
	{
		printf("Found %s\n", PHOENIXHX_BOARD.name);
		out_fname = out_fname_dm;
		camin_dsp = DSP0_NUM;
		tvout_dsp = DSP0_NUM;
	}
	else if (MangoC64Boards_Open(&card, devices, num_dev, &SEAGULL_PCI_BOARD, NULL) == MANGOERROR_SUCCESS)
	{
		printf("Found %s\n", SEAGULL_PCI_BOARD.name);
		out_fname = out_fname_6415;
		camin_dsp = -1;
		tvout_dsp = -1;
	}
	else if (MangoC64Boards_Open(&card, devices, num_dev, &SEAGULL_PMC_BOARD, NULL) == MANGOERROR_SUCCESS)
	{
		printf("Found %s\n", SEAGULL_PMC_BOARD.name);
		out_fname = out_fname_6415;
		camin_dsp = DSP1_NUM;
		tvout_dsp = DSP0_NUM;
	}
	else
	{
		printf("No board found!\n");
		return -1;
	}

	free(devices);
	num_dsps = card.num_devices;

	if (MangoX_Open(NULL) != MANGOERROR_SUCCESS)
		return -1;

	coff_fnames = (char**)malloc(num_dsps * sizeof(char*));
	err_cbs = (MANGOX_ERR_CB*)malloc(num_dsps * sizeof(MANGOX_ERR_CB));
	for (i=0; i<num_dsps; i++)
	{
		coff_fnames[i] = out_fname;
		err_cbs[i] = err_callback;
	}

	fpga_fnames = (char**)calloc(num_dsps, sizeof(char*));
	switch(card.card)
	{
	case MANGOCARD_SEAGULL_PMC:
		strcpy(fpga_fullname, fpga_path);
		strcat(fpga_fullname, "sgl_pmc_fpga.rbf");
		fpga_fnames[0] = fpga_fullname;
		break;
	}

	printf("Loading DSPs and FPGA...\n");
	while ((ret=MangoX_CardEnable(&card, coff_fnames, fpga_fnames, &card_num, &pciDevArr, err_cbs, NULL)) == MANGOERROR_TIMEOUT)
		printf("Timeout, retrying\n");

	if (ret != MANGOERROR_SUCCESS)
	{
		printf("Error %x\n", ret);
		return -1;
	}
		
	free(coff_fnames);
	free(err_cbs);

	// Create and run streams several times
	for (stream_iter=0; stream_iter<STREAM_ITERATIONS; stream_iter++)
	{

// There are several different examples, each is activated by a different preprocessor label.

// Example 1: MP4LOOP. Convert CIF YUV file into Mpeg-4 Elementary Stream.
#ifdef MP4LOOP
		printf("Mp4Loop Example, iteration %d\n", stream_iter);
		try 
		{
			h2d_filter = new CFilterH2D_PCI(2, CIF_SIZE, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
			d2h_filter = new CFilterD2H_PCI(2, CIF_SIZE, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}
		mp4_params.nNumBufs = 2;
		mp4_params.nMaxOutputSize = 0x20000;
		mp4_params.eVideoStandard = MX_PAL;
		mp4_params.eImageSize = MX_CIF;
		mp4_params.nFramerateDivisor = 1;
		mp4_params.nNumGOPFrames = 50;
		mp4_params.eRC_mode = RC_CONSTANT_Q;
		mp4_params.nQinitial = 4;

		mp4enc_filter = new CFilterMpeg4VEnc(mp4_params);

		// create graphs.
		pGraph = new CMangoXGraph(3);
		if (!pGraph->SetFilter(h2d_filter,    0)) clean_abort();
		if (!pGraph->SetFilter(mp4enc_filter, 1)) clean_abort();
		if (!pGraph->SetFilter(d2h_filter,    2)) clean_abort();

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		if (MangoX_SubmitGraph(&graph_num, card_num, DSP0_NUM, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();
		h2d_filter->Connect();
		d2h_filter->Connect();

		fp = fopen("IN.yuv", "rb");
		if (!fp)
		{
			printf("Cannot open input YUV file!\n");
			clean_abort();
		}
		fpw = fopen("out.mp4v", "wb");
		if (!fpw)
			clean_abort();

		while(1)
		{
			if (h2d_filter->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
			if (fread(buf.local, CIF_SIZE, 1, fp) < 1)
				break;
			if (h2d_filter->SubmitBuffer(&buf, CIF_SIZE) != MANGOERROR_SUCCESS)
				clean_abort();
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf(".");
#if defined(linux) || defined(__SVR4)
			fflush(stdout);
#endif
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", stream_iter);
		fclose(fp);
		fclose(fpw);

		if (MangoX_DeleteGraph(graph_num, card_num, DSP0_NUM) != MANGOERROR_SUCCESS)
			clean_abort();
		
		delete pGraph;
#endif

// Example 2: VIDIN_MP4_FILE. Receive video input and encode as MPEG-4 Elementary Stream.
#ifdef VIDIN_MP4_FILE
		printf("VIDIN_MP4_FILE Example, iteration %d\n", stream_iter);
		if (camin_dsp == -1) clean_abort();

		if (card.card == MANGOCARD_SEAGULL_PMC)
		{
			if (MangoX_ConfigVideo(card_num, DSP0_NUM, LOAD_VIDIN_I2C, MX_PAL, 0) != MANGOERROR_SUCCESS)
				clean_abort();
		}

		vidin_filter = new CFilterVideoIn(2, 0, MX_4CIF, MX_PAL, MX_COMPOSITE_VIDEO, 3);

		try 
		{
			d2h_filter = new CFilterD2H_PCI(2, 0x40000, MANGOBIOS_FOREVER, pciDevArr[camin_dsp]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}

		mp4_params.nNumBufs = 2;
		mp4_params.nMaxOutputSize = 0x40000;
		mp4_params.eVideoStandard = MX_PAL;
		mp4_params.eImageSize = MX_4CIF;
		mp4_params.nFramerateDivisor = 1;
		mp4_params.nNumGOPFrames = 100;
		mp4_params.eRC_mode = RC_CBR;
		mp4_params.nQinitial = 4;
		mp4_params.nCodecBufsize = 2000000;
		mp4_params.nConstantBitrate = 1000000;
		mp4_params.nQmax = 31;
		mp4_params.nQmin = 2;

		mp4enc_filter = new CFilterMpeg4VEnc(mp4_params);

		// create graphs.
		pGraph = new CMangoXGraph(3);
		pGraph->SetFilter(vidin_filter, 0);
		pGraph->SetFilter(mp4enc_filter, 1);
		pGraph->SetFilter(d2h_filter, 2);

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		printf("Opening stream on DSP %d\n", camin_dsp);
		if (MangoX_SubmitGraph(&graph_num, card_num, camin_dsp, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();

		printf("Waiting for PCI stream connect\n");
		d2h_filter->Connect();

		fpw = fopen("out.mp4v", "wb");

		printf("Waiting for frames from video-in\n");

		for (int n=0; n<25*60; n++)
		{
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf("Got frame %d bytes\n", size);
			fflush(stdout);
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", cnt++);
		fclose(fpw);
		if (MangoX_DeleteGraph(graph_num, card_num, camin_dsp) != MANGOERROR_SUCCESS)
		{
			printf("Failed!\n");
			clean_abort();
		}
		
		delete pGraph;
#endif

// Example 3: VIDIN_MP2_FILE. Receive video input and encode as MPEG-2.
#ifdef VIDIN_MP2_FILE
		printf("VIDIN_MP2_FILE Example, iteration %d\n", stream_iter);
		if (camin_dsp == -1) clean_abort();

		if (card.card == MANGOCARD_SEAGULL_PMC)
		{
			if (MangoX_ConfigVideo(card_num, DSP0_NUM, LOAD_VIDIN_I2C, MX_PAL, 0) != MANGOERROR_SUCCESS)
				clean_abort();
		}

		vidin_filter = new CFilterVideoIn(2, 0, MX_4CIF, MX_PAL, MX_COMPOSITE_VIDEO, 3);

		try 
		{
			d2h_filter = new CFilterD2H_PCI(2, 0x40000, MANGOBIOS_FOREVER, pciDevArr[camin_dsp]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}

		mp2enc_params.nNumBufs = 2;
		mp2enc_params.eImageSize = MX_4CIF;
		mp2enc_params.eVideoStandard = MX_PAL;
		mp2enc_params.nConstantBitrate = 4000000;
		mp2enc_params.nMaxOutputSize = 0x40000;
		mp2enc_params.nNumBFrames = 2; // must be 0, 1 or 2
		mp2enc_params.nNumGOPFrames = 15; // must be a multiple of (nNumBFrames+1)

		mp2enc_filter = new CFilterMpeg2VEnc(mp2enc_params);

		// create graphs.
		pGraph = new CMangoXGraph(3);
		pGraph->SetFilter(vidin_filter, 0);
		pGraph->SetFilter(mp2enc_filter, 1);
		pGraph->SetFilter(d2h_filter, 2);

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		printf("Opening stream on DSP %d\n", camin_dsp);
		if (MangoX_SubmitGraph(&graph_num, card_num, camin_dsp, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();

		printf("Waiting for PCI stream connect\n");
		d2h_filter->Connect();

		fpw = fopen("out.mp2v", "wb");

		printf("Waiting for frames from video-in\n");

		for (int n=0; n<25*60; n++)
		{
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf("Got frame %d bytes\n", size);
			fflush(stdout);
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", cnt++);
		fclose(fpw);
		if (MangoX_DeleteGraph(graph_num, card_num, camin_dsp) != MANGOERROR_SUCCESS)
		{
			printf("Failed!\n");
			clean_abort();
		}
		
		delete pGraph;
#endif

// Example 4: VIDIN_H263_FILE. Receive video input and encode as H.263.
#ifdef VIDIN_H263_FILE
		printf("VIDIN_H263_FILE Example, iteration %d\n", stream_iter);
		if (camin_dsp == -1) clean_abort();

		if (card.card == MANGOCARD_SEAGULL_PMC)
		{
			if (MangoX_ConfigVideo(card_num, DSP0_NUM, LOAD_VIDIN_I2C, MX_PAL, 0) != MANGOERROR_SUCCESS)
				clean_abort();
		}

		vidin_filter = new CFilterVideoIn(2, 0, MX_CIF, MX_PAL, MX_COMPOSITE_VIDEO, 3);
		try 
		{
			d2h_filter = new CFilterD2H_PCI(2, 0x8000, MANGOBIOS_FOREVER, pciDevArr[camin_dsp]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}

		h263enc_params.eImageSize = MX_CIF;
		h263enc_params.eVideoStandard = MX_PAL;
		h263enc_params.nBitrate = 750000;
		h263enc_params.nFramerate = 25;
		h263enc_params.nMaxOutputSize = 0x8000;
		h263enc_params.nNumBufs = 2;
		h263enc_params.nNumGOPFrames = 75;
		h263enc_params.nQI = 5;
		h263enc_params.nQmax = 31;
		h263enc_params.nQmin = 2;
		h263enc_filter = new CFilterH263VEnc(h263enc_params);

		// create graphs.
		pGraph = new CMangoXGraph(3);
		pGraph->SetFilter(vidin_filter, 0);
		pGraph->SetFilter(h263enc_filter, 1);
		pGraph->SetFilter(d2h_filter, 2);

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		printf("Opening stream on DSP %d\n", camin_dsp);
		if (MangoX_SubmitGraph(&graph_num, card_num, camin_dsp, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();

		printf("Waiting for PCI stream connect\n");
		d2h_filter->Connect();

		fpw = fopen("out.263", "wb");

		printf("Waiting for frames from video-in\n");

		for (int n=0; n<25*60; n++)
		{
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf("Got frame %d bytes\n", size);
			fflush(stdout);

			// H.263 bitstream must be endian-reversed. This can be done in DSP instead (see DSP source).
			for (int i=0; i<size/4; i++)
			{
				int d = SWAP32(((int*)buf.local)[i]);
				fwrite(&d, 4, 1, fpw);
			}

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", cnt++);
		fclose(fpw);
		if (MangoX_DeleteGraph(graph_num, card_num, camin_dsp) != MANGOERROR_SUCCESS)
		{
			printf("Failed!\n");
			clean_abort();
		}
		
		delete pGraph;
#endif

// Example 5: WAV_TO_MP3. Convert WAV audio file into MP3.
#ifdef WAV_TO_MP3
		printf("WAV_TO_MP3 Example, iteration %d\n", stream_iter);
		try 
		{
			h2d_filter = new CFilterH2D_PCI(2, AUDIO_SAMPLES_PER_FRAME*2*sizeof(short), MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
			d2h_filter = new CFilterD2H_PCI(2, 0x1000, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}
		mp3enc_params.nBitrate = 128;
		mp3enc_params.nIsStereo = 1;
		mp3enc_params.nMaxOutputSize = 0x1000;
		mp3enc_params.nNumBufs = 2;
		mp3enc_params.nSampleFreq = 44100;

		mp3enc_filter = new CFilterMp3AEnc(mp3enc_params);

		// create graphs.
		pGraph = new CMangoXGraph(3);
		if (!pGraph->SetFilter(h2d_filter,    0)) clean_abort();
		if (!pGraph->SetFilter(mp3enc_filter, 1)) clean_abort();
		if (!pGraph->SetFilter(d2h_filter,    2)) clean_abort();

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		if (MangoX_SubmitGraph(&graph_num, card_num, DSP0_NUM, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();
		h2d_filter->Connect();
		d2h_filter->Connect();

		fp = fopen("in.wav", "rb");
		if (!fp)
		{
			printf("Cannot open input WAV file!\n");
			clean_abort();
		}
		fpw = fopen("out.mp3", "wb");
		if (!fpw)
			clean_abort();

		fseek(fp, WAV_HEADER_LENGTH, SEEK_SET); // skip wav header, assume 16-bit stereo 44.1khz
		
		// Utilize double buffer: Send 2 buffers before receiving first output buffer
		if (h2d_filter->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
			clean_abort();
		if (fread(buf.local, AUDIO_SAMPLES_PER_FRAME*2*sizeof(short), 1, fp) < 1)
			clean_abort();
		if (h2d_filter->SubmitBuffer(&buf, AUDIO_SAMPLES_PER_FRAME*2*sizeof(short)) != MANGOERROR_SUCCESS)
			clean_abort();

		extra_cnt = 0;
		while(1)
		{
			if (h2d_filter->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
			bytes_read = fread(buf.local, 1, AUDIO_SAMPLES_PER_FRAME*2*sizeof(short), fp);
			if (bytes_read < 1)
			{
				// send 2 extra blank frames at end of file
				memset(buf.local, 0, AUDIO_SAMPLES_PER_FRAME*2*sizeof(short));
				extra_cnt++;
				if (extra_cnt >= 2)
					break;
			}
			if (h2d_filter->SubmitBuffer(&buf, bytes_read) != MANGOERROR_SUCCESS)
				clean_abort();
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf("Got %d bytes\n", size);
#if defined(linux) || defined(__SVR4)
			fflush(stdout);
#endif
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
			clean_abort();

		printf("Got %d bytes\n", size);
		fwrite(buf.local, size, 1, fpw);

		if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
			clean_abort();

		printf("\nClosing %d\n", stream_iter);
		fclose(fp);
		fclose(fpw);

		if (MangoX_DeleteGraph(graph_num, card_num, DSP0_NUM) != MANGOERROR_SUCCESS)
			clean_abort();
		
		delete pGraph;
#endif

// Example 6: MP3_TO_PCM. Convert MP3 audio file into raw PCM.
#ifdef MP3_TO_PCM
		printf("MP3_TO_PCM Example, iteration %d\n", stream_iter);

		try 
		{
			h2d_filter = new CFilterH2D_PCI(2, 0x800, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
			d2h_filter = new CFilterD2H_PCI(2, 0x10000, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}
		mp3dec_filter = new CFilterMp3ADec(2, 0x10000);

		// create graphs.
		pGraph = new CMangoXGraph(3);
		if (!pGraph->SetFilter(h2d_filter,    0)) clean_abort();
		if (!pGraph->SetFilter(mp3dec_filter, 1)) clean_abort();
		if (!pGraph->SetFilter(d2h_filter,    2)) clean_abort();

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		if (MangoX_SubmitGraph(&graph_num, card_num, DSP0_NUM, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();
		h2d_filter->Connect();
		d2h_filter->Connect();

		fp = fopen("in.mp3", "rb");
		if (!fp)
		{
			printf("Cannot open input MP3 file!\n");
			clean_abort();
		}
		fpw = fopen("out.raw", "wb");
		if (!fpw)
			clean_abort();

		while(1)
		{
			if (h2d_filter->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
			bytes_read = fread(buf.local, 1, 0x800, fp);
			if (bytes_read < 1)
				break;
			if (h2d_filter->SubmitBuffer(&buf, bytes_read) != MANGOERROR_SUCCESS)
				clean_abort();
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf("Got %d bytes\n", size);
#if defined(linux) || defined(__SVR4)
			fflush(stdout);
#endif
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", stream_iter);
		fclose(fp);
		fclose(fpw);

		if (MangoX_DeleteGraph(graph_num, card_num, DSP0_NUM) != MANGOERROR_SUCCESS)
			clean_abort();
		
		delete pGraph;

#endif

// Example 7: VIDLOOP_OSD. Run video-in to video-out, overlay some text.
#ifdef VIDLOOP_OSD
		printf("VIDLOOP_OSD Example, iteration %d\n", stream_iter);
		if (card.card != MANGOCARD_SEAGULL_PMC)
			clean_abort();

		if (MangoX_ConfigVideo(card_num, DSP0_NUM, LOAD_VIDIN_I2C, MX_PAL, 0) != MANGOERROR_SUCCESS)
			clean_abort();
		if (MangoX_ConfigVideo(card_num, DSP0_NUM, LOAD_VIDOUT_I2C, MX_PAL, 0) != MANGOERROR_SUCCESS)
			clean_abort();

		vidin_filter = new CFilterVideoIn(2, 0, MX_4CIF, MX_PAL, MX_COMPOSITE_VIDEO, 3);
		vidout_filter = new CFilterVideoOut(MX_4CIF, MX_PAL, 3, 0);
		osd_filter = new CFilterOSD(MX_4CIF, MX_PAL);
		
		try 
		{
			h2d_filter = new CFilterH2D_PCI(2, CIF4_SIZE, MANGOBIOS_FOREVER, pciDevArr[tvout_dsp]);
			d2h_filter = new CFilterD2H_PCI(2, CIF4_SIZE, MANGOBIOS_FOREVER, pciDevArr[camin_dsp]);
			h2d_filter_osd = new CFilterH2D_PCI(1, 0x100, MANGOBIOS_FOREVER, pciDevArr[tvout_dsp]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}

		// create graphs.
		pGraphIn = new CMangoXGraph(2);
		pGraphIn->SetFilter(vidin_filter, 0);
		pGraphIn->SetFilter(d2h_filter, 1);

		pGraphOut = new CMangoXGraph(4);
		pGraphOut->SetFilter(h2d_filter, 0);
		pGraphOut->SetFilter(osd_filter, 1);
		pGraphOut->SetFilter(vidout_filter, 2);
		pGraphOut->SetFilter(h2d_filter_osd, 3);

		// attach graphs
		if (!pGraphIn->Attach(0, 1, 0, 0)) clean_abort();

		if (!pGraphOut->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraphOut->Attach(1, 2, 0, 0)) clean_abort();
		if (!pGraphOut->Attach(3, 1, 0, 1)) clean_abort();

		// submit graphs
		if (MangoX_SubmitGraph(&graphOut_num, card_num, tvout_dsp, pGraphOut) != MANGOERROR_SUCCESS)
			clean_abort();
		if (MangoX_SubmitGraph(&graphIn_num, card_num, camin_dsp, pGraphIn) != MANGOERROR_SUCCESS)
			clean_abort();

		h2d_filter->Connect();
		h2d_filter_osd->Connect();
		d2h_filter->Connect();

		// Create OSD text entry
		if (h2d_filter_osd->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
			clean_abort();

		osd_params.cmd = MX_OSD_ADD_ENTRY;
		osd_params.entryNum = 0;
		osd_params.type = MX_OSD_TEXT;
		osd_params.textSize = MX_OSD_TEXT_SIZE_MEDIUM;
		osd_params.isVisible = 1;
		osd_params.isTransparent = 1;
		osd_params.isYOnly = 0;
		osd_params.fore_Y = 0xF0;
		osd_params.fore_Cb = 0x10;
		osd_params.fore_Cr = 0x10;
		osd_params.back_Y =
		osd_params.back_Cb =
		osd_params.back_Cr = 0xFF;
		osd_params.x = 20;
		osd_params.y = 20;

		memcpy(buf.local, &osd_params, sizeof(stOSDData));
		strcpy((char*)buf.local+sizeof(stOSDData),osd_text);
		if (h2d_filter_osd->SubmitBuffer(&buf, sizeof(stOSDData)+strlen(osd_text)+1) != MANGOERROR_SUCCESS)
			clean_abort();

		// stream video and modify OSD parameters
		while(1)
		{
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf("Got frame %d bytes\n", size);
			fflush(stdout);
			if (h2d_filter->GetEmptyBuffer(&buf2) != MANGOERROR_SUCCESS)
				clean_abort();
			memcpy(buf2.local, buf.local, size);
			if (h2d_filter->SubmitBuffer(&buf2, size) != MANGOERROR_SUCCESS)
				clean_abort();

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();

			// Modify location of text.
			if (h2d_filter_osd->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();

			osd_params.cmd = MX_OSD_MODIFY_ENTRY;
			osd_params.entryNum = 0;
			osd_params.x = (osd_params.x>400? 20 : osd_params.x+2);
			osd_params.y = (osd_params.y>400? 20 : osd_params.y+2);

			memcpy(buf.local, &osd_params, sizeof(stOSDData));
			if (h2d_filter_osd->SubmitBuffer(&buf, sizeof(stOSDData)) != MANGOERROR_SUCCESS)
				clean_abort();
		}
#endif

// Example 8: H264_ENC_LOOP. Convert CIF YUV file into H264 Elementary Stream.

#ifdef H264_ENC_LOOP
		printf("H264 Encoder Loop Example, iteration %d\n", stream_iter);
		try 
		{
			h2d_filter = new CFilterH2D_PCI(2, CIF_SIZE, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
			d2h_filter = new CFilterD2H_PCI(2, CIF_SIZE, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}

		stH264V_h2dparams h264_params;
		h264_params.eImageSize = MX_CIF;
		h264_params.eVideoStandard = MX_NTSC;
		h264_params.nMaxOutputSize = 0x40000;
		h264_params.nNumBufs = 1;
		h264_params.nFrameRate = 30;
		h264_params.nBitrate = 1500000;
		h264_params.nQp0 = 32;
		h264_params.nQpN = 32;
		h264_params.nNumGOPFrames = 30;

		// create graphs.
		pGraph = new CMangoXGraph(3);
		if (!pGraph->SetFilter(h2d_filter, 0))
			clean_abort();
		if (!pGraph->SetFilter(new CFilterH264VEnc(h264_params), 1))
			clean_abort();
		if (!pGraph->SetFilter(d2h_filter, 2))
			clean_abort();

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0))
			clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0))
			clean_abort();

		if (MangoX_SubmitGraph(&graph_num, card_num, DSP0_NUM, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();
		h2d_filter->Connect();
		d2h_filter->Connect();

		fp = fopen("IN.yuv", "rb");
		if (!fp)
		{
			printf("Cannot open input YUV file!\n");
			clean_abort();
		}

		fpw = fopen("out.264", "wb");
		if (!fpw)
			clean_abort();

		while(1) 
		{	
	
			if (h2d_filter->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
			if (fread(buf.local, CIF_SIZE, 1, fp) < 1)
				break;
			if (h2d_filter->SubmitBuffer(&buf, CIF_SIZE) != MANGOERROR_SUCCESS)
				clean_abort();
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();
			printf(".");
			fflush(stdout);
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", stream_iter);
		fclose(fp);
		fclose(fpw);		

		if (MangoX_DeleteGraph(graph_num, card_num, DSP0_NUM) != MANGOERROR_SUCCESS)
			clean_abort();
		
		delete pGraph;
#endif // H264_ENC_LOOP

// Example 9: VIDIN_H264_FILE. Receive video input and encode as H.264 Elementary Stream.
#ifdef VIDIN_H264_FILE
		printf("Video In to H264 file Example, iteration %d\n", stream_iter);
		if (camin_dsp == -1)
			clean_abort();

		// init h264 params.
		stH264V_h2dparams h264_params;
		h264_params.eImageSize = MX_CIF;
		h264_params.eVideoStandard = MX_NTSC;
		h264_params.nMaxOutputSize = 0x40000;
		h264_params.nNumBufs = 2;
		h264_params.nFrameRate = 15;
		h264_params.nQp0 = 12;
		h264_params.nQpN = 12;
		h264_params.nNumGOPFrames = 30;
		h264_params.nBitrate = 0x100000;

		if (card.card == MANGOCARD_SEAGULL_PMC)
		{
			if (MangoX_ConfigVideo(card_num, DSP0_NUM, LOAD_VIDIN_I2C, h264_params.eVideoStandard, 0) != MANGOERROR_SUCCESS)
				clean_abort();
		}

		vidin_filter = new CFilterVideoIn(2, camin_cam, h264_params.eImageSize, h264_params.eVideoStandard, MX_COMPOSITE_VIDEO, 3);

		try 
		{
			d2h_filter = new CFilterD2H_PCI(2, 0x40000, MANGOBIOS_FOREVER, pciDevArr[camin_dsp]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}

		int bit_count = 0;
		
		// create graphs.
		pGraph = new CMangoXGraph(3);
		pGraph->SetFilter(vidin_filter, 0);
		pGraph->SetFilter(new CFilterH264VEnc(h264_params), 1);
		pGraph->SetFilter(d2h_filter, 2);

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		printf("Opening stream on DSP %d\n", camin_dsp);
		if (MangoX_SubmitGraph(&graph_num, card_num, camin_dsp, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();

		printf("Waiting for PCI stream connect\n");
		d2h_filter->Connect();

		fpw = fopen("h264Demo.264", "wb");
		printf("Waiting for frames from video-in\n");

		int n;
		for (n = 0; n < 750; n++)
		{
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			bit_count += size;

			fwrite(buf.local, size, 1, fpw);
			printf(".");
			fflush(stdout);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", cnt++);
		fclose(fpw);
		printf("%d frames at bit rate of %d bits/Sec\n", n, bit_count * 8 * h264_params.nFrameRate / n);
		if (MangoX_DeleteGraph(graph_num, card_num, camin_dsp) != MANGOERROR_SUCCESS)
		{
			printf("Failed!\n");
			clean_abort();
		}
		
		delete pGraph;
#endif

// Example 10: G711_LOOP. Encode and decode back Audio WAV file as G711
#ifdef G711_LOOP
		printf("G711 Example, iteration %d\n", stream_iter);
		try 
		{
			h2d_filter = new CFilterH2D_PCI(2, 100000, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
			d2h_filter = new CFilterD2H_PCI(2, 100000, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}
		g711_enc_params.nNumBufs = 2;
		g711_enc_params.nMaxOutputSize = 100000;
		g711_enc_params.eVocoderType = MX_G711_A_LAW;
		
		g711_dec_params.nNumBufs = 2;
		g711_dec_params.nMaxOutputSize = 100000;
		g711_dec_params.eVocoderType = MX_G711_A_LAW;
		
		g711enc_filter = new CFilterG711AEnc(g711_enc_params);
		g711dec_filter = new CFilterG711ADec(g711_dec_params);

		// create graphs.
		pGraph = new CMangoXGraph(3);
		if (!pGraph->SetFilter(h2d_filter,		0)) clean_abort();
		if (!pGraph->SetFilter(g711enc_filter,	1)) clean_abort();
		if (!pGraph->SetFilter(d2h_filter,		2)) clean_abort();

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		if (MangoX_SubmitGraph(&graph_num, card_num, DSP0_NUM, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();
		h2d_filter->Connect();
		d2h_filter->Connect();

		fp = fopen("chunk-0.Wav", "rb");
		if (!fp)
		{
			printf("Cannot open input YUV file!\n");
			clean_abort();
		}
		fpw = fopen("out_g711.wav", "wb");
		if (!fpw)
			clean_abort();

		char wav_header[44];
		char g711_wav_header[44];
		int size_read;
		size_read = fread(wav_header, 1, 44, fp);
		memcpy(g711_wav_header, wav_header, 44);
		int* chunkSize = (int*)&g711_wav_header[4];
		*chunkSize = (*chunkSize - 36) / 2 + 36;
		chunkSize = (int*)&g711_wav_header[40];
		*chunkSize = *chunkSize / 2;
		
		short* BitsPerSample = (short*)&g711_wav_header[34];
		*BitsPerSample /= 2;
		
		short* ByteRate = (short*)&g711_wav_header[28];
		*ByteRate /= 2;

		short* BlockAlign = (short*)&g711_wav_header[32];
		*BlockAlign /= 2;

		short* AudioFormat = (short*)&g711_wav_header[20];
		*AudioFormat = g711_enc_params.eVocoderType + 6; //6 is a law, 7 mu-law

		fwrite(g711_wav_header, 1, 44, fpw);

		while(1)
		{
			if (h2d_filter->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
			if ((size_read = fread(buf.local, 1, 80000, fp)) < 1)
				break;
			if (h2d_filter->SubmitBuffer(&buf, size_read) != MANGOERROR_SUCCESS)
				clean_abort();
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf(".");
#if defined(linux) || defined(__SVR4)
			fflush(stdout);
#endif
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", stream_iter);
		fclose(fp);
		fclose(fpw);

		if (MangoX_DeleteGraph(graph_num, card_num, DSP0_NUM) != MANGOERROR_SUCCESS)
			clean_abort();
		
		delete pGraph;

		try 
		{
			h2d_filter = new CFilterH2D_PCI(2, 100000, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
			d2h_filter = new CFilterD2H_PCI(2, 100000, MANGOBIOS_FOREVER, pciDevArr[DSP0_NUM]);
		}
		catch (MANGOERROR_error_t err)
		{
			printf("Error %08x allocating shared memory\n", err);
			clean_abort();
		}

		// create graph.
		pGraph = new CMangoXGraph(3);
		if (!pGraph->SetFilter(h2d_filter,		0)) clean_abort();
		if (!pGraph->SetFilter(g711dec_filter,	1)) clean_abort();
		if (!pGraph->SetFilter(d2h_filter,		2)) clean_abort();

		// attach graphs
		if (!pGraph->Attach(0, 1, 0, 0)) clean_abort();
		if (!pGraph->Attach(1, 2, 0, 0)) clean_abort();

		if (MangoX_SubmitGraph(&graph_num, card_num, DSP0_NUM, pGraph) != MANGOERROR_SUCCESS)
			clean_abort();
		h2d_filter->Connect();
		d2h_filter->Connect();

		fp = fopen("out_g711.wav", "rb");
		if (!fp)
		{
			printf("Cannot open input YUV file!\n");
			clean_abort();
		}
		fpw = fopen("out.wav", "wb");
		if (!fpw)
			clean_abort();

		fread(g711_wav_header, 1, 44, fp);
		fwrite(wav_header, 1, 44, fpw);

		while(1)
		{
			if (h2d_filter->GetEmptyBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
			if ((size_read = fread(buf.local, 1, 40000, fp)) < 1)
				break;
			if (h2d_filter->SubmitBuffer(&buf, size_read) != MANGOERROR_SUCCESS)
				clean_abort();
			if (d2h_filter->GetFullBuffer(&buf, &size) != MANGOERROR_SUCCESS)
				clean_abort();

			printf(".");
#if defined(linux) || defined(__SVR4)
			fflush(stdout);
#endif
			fwrite(buf.local, size, 1, fpw);

			if (d2h_filter->FreeBuffer(&buf) != MANGOERROR_SUCCESS)
				clean_abort();
		}
		printf("\nClosing %d\n", stream_iter);
		fclose(fp);
		fclose(fpw);

		if (MangoX_DeleteGraph(graph_num, card_num, DSP0_NUM) != MANGOERROR_SUCCESS)
			clean_abort();
		
		delete pGraph;
#endif
	}

#ifdef PROPER_SHUTDOWN
	if (MangoX_CardDisable(card_num) != MANGOERROR_SUCCESS)
		clean_abort();

	if ((ret=MangoX_Close()) != MANGOERROR_SUCCESS)
	{
		printf("Error 0x%08x in MangoX_close!\n", ret);
		return -1;
	}

	card.close(&card);
	MANGOBIOS_close();
#endif

	return 0;
}
