/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	MangoX_Shared.cpp
 *
 * GENERAL DESCRIPTION: MangoX Host/DSP Shared definitions
 *
 * DATE CREATED		AUTHOR
 *
 * 21/02/05 		Itay Chamiel
 *
 ******************************************************************************/
#ifndef __MANGOX_SHARED_H__
#define __MANGOX_SHARED_H__

#include "MangoX_SharedExp.h"

// Define version of this lib
#define VERSION_MAJOR 1
#define VERSION_MINOR 3

#define Align_32Bit(length) ((length + 0x3) &~0x3)
#define MAX_CMD_SIZE 0x100
#define SERIALIZE_BUFLEN	(MAX_CMD_SIZE * 8)

#define SDRAM_MAILBOX				0x80000000
#define SDRAM_MAILBOX_SIZE			8
#define PCI_SHARED_MEMORY_LENGTH	0x100

#define H2D_CMD_NAME	"/DioPciCmdH2D"
#define D2H_CMD_NAME	"/DioPciCmdD2H"
#define D2H_ERR_NAME	"/DioPciErrD2H"

typedef enum {
	LOAD_FPGA = 0x43210000,
	LOAD_I2C,
	SUBMIT_GRAPH,
	DELETE_GRAPH,
	FREE_ERR_STREAM,
	GET_VERSIONS,
	CUSTOM_GRAPH_COMMAND,
	CUSTOM_DSP_COMMAND
} HostDSP_cmd_E;

typedef enum {
	ACK_OK = 0x0cee0000,
	ACK_ERR
} Ack_E;

typedef struct {
	HostDSP_cmd_E command;
	int params[3];
} HostDSP_Cmd_T;

typedef struct {
	Ack_E ack;
	int retvals[NUM_CUSTOM_CMD_RETVALS];
} DSP_Ack_T;

typedef struct {
	Error_Severity_E err_type;
	int	err_code;
	char err_string[0x80];
} DSP_Err_T;

#endif /* __MANGOX_SHARED_H__ */
