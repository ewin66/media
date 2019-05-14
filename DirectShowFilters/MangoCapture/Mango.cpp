/* Copyright 2005, Future Concepts, La Verne, CA. */

#include <stdio.h>
#include <string>
using namespace std;

#include "MangoXExp.h"

#include "Mango.h"
#include "MXException.h"

MangoC64Boards_handle_t	Mango::sCard;
int Mango::sCardNum = 0;
h2d_pci_device_t** Mango::sPCIDevices = 0;
LONG Mango::sReferenceCount = 0;

// This is the error callback function called if the DSP reports an error.
static void err_callback(void* err_arg, int card_num, int dsp_num, Error_Severity_E err_type, int err_code, char * err_string)
{
	char buf[256];
	OutputDebugStringA("dsp error - err_callback");
	string workString = "Mango DSP Operation failure (";
	sprintf(buf, "Card %d DSP %d", card_num, dsp_num);
	workString += buf;
	workString += ")\n";
	switch(err_type)
	{
	case ERRTYPE_WARNING:
		workString += "Warning: ";
		break;
	case ERRTYPE_ERROR:
		workString += "Error: ";
		break;
	case ERRTYPE_FATAL:
		workString += "Fatal: ";
		break;
	default:
		workString += "Unknown: ";
		break;
	}
	
	sprintf(buf, "(%d)\n", err_code);

	workString += buf;
	OutputDebugStringA(workString.c_str());
	OutputDebugStringA(err_string);
}

void
Mango::Init()
{
	MANGOERROR_error_t mangoStatus;
	MANGOBIOS_deviceHandle_t *devices;
	int num_dev;
	int i;
	int num_dsps;
	int counter = 0;
	char ** coff_fnames;
	MANGOX_ERR_CB * err_cbs;

	if (InterlockedIncrement(&sReferenceCount) == 1)
	{
		char* workingDirectory = getenv("AntaresX_Video_WorkingDirectory");
		if (workingDirectory == NULL)
		{
			workingDirectory = "C:\\Program Files\\Future Concepts\\AntaresX Media Server";
		}
		char* objectFilename = getenv("AntaresX_Video_MX_DSP_Binary");
		if (objectFilename == NULL)
		{
			objectFilename = "MangoX_DM_D.out";
		}
		if (workingDirectory == NULL || objectFilename == NULL)
		{
			return;
		}
		mangoStatus = MANGOBIOS_open(NULL);
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			throw MXException("MANGOBIOS_open", mangoStatus);
		}
		mangoStatus = MANGOBIOS_getNumDevices(NULL, &num_dev);
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			throw MXException("MANGOBIOS_getNumDevices", mangoStatus);
		}
		if (num_dev < 1)
		{
			OutputDebugStringA("No DSP devices found");
			throw MXException("MANGOBIOS_getNumDevices", MANGOERROR_FAILURE);
		}
		devices = (MANGOBIOS_deviceHandle_t*)malloc(num_dev * sizeof(MANGOBIOS_deviceHandle_t));
		mangoStatus = MANGOBIOS_getDeviceHandles(NULL, devices);
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			throw MXException("MANGOBIOS_getDeviceHandles", mangoStatus);
		}
		OutputDebugStringA("Determining Mango board type");
		if (MangoC64Boards_Open(&sCard, devices, num_dev, &SEAGULL_PC104_PLUS_BOARD, NULL) == MANGOERROR_SUCCESS)
		{
			OutputDebugStringA(SEAGULL_PC104_PLUS_BOARD.name);
		}
		else if (MangoC64Boards_Open(&sCard, devices, num_dev, &SEAGULL_PCI_BOARD, NULL) == MANGOERROR_SUCCESS)
		{
			OutputDebugStringA(SEAGULL_PCI_BOARD.name);
		}
		else if (MangoC64Boards_Open(&sCard, devices, num_dev, &SEAGULL_PMC_BOARD, NULL) == MANGOERROR_SUCCESS)
		{
			OutputDebugStringA(SEAGULL_PMC_BOARD.name);
		}
		else
		{
			OutputDebugStringA("No Mango board found");
			throw MXException("MangoC64Boards_Open", MANGOERROR_FAILURE);
		}
		num_dsps = sCard.num_devices;
		mangoStatus = MangoX_Open(NULL);
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			throw MXException("MangoX_Open", mangoStatus);
		}
		coff_fnames = (char**)malloc(num_dsps * sizeof(char*));
		err_cbs = (MANGOX_ERR_CB*)malloc(num_dsps * sizeof(MANGOX_ERR_CB));
		char mangoObjectPath[256];
		sprintf(mangoObjectPath, "%s\\%s", workingDirectory, objectFilename);
		OutputDebugStringA("Mango object path:");
		OutputDebugStringA(mangoObjectPath);
		if (mangoObjectPath != NULL)
		{
			for (i=0; i<num_dsps; i++)
			{
				coff_fnames[i] = mangoObjectPath;
				err_cbs[i] = err_callback;
			}
		}
		OutputDebugStringA("Loading DSPs and FPGA...");
		for (int i = 0; i < 10; i++)
		{
			mangoStatus = MangoX_CardEnable(&sCard, coff_fnames, NULL, &sCardNum, &sPCIDevices, err_cbs, NULL);
			if (mangoStatus != MANGOERROR_TIMEOUT)
			{
				break;
			}
			Sleep(1000);
			OutputDebugStringA("MangoX_CardEnable Timeout, retrying...");
		}
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			//HACK throw MANGOERROR_SUCCESS - 1 to indicate mango cannot be enabled!
			throw MXException("MangoX_CardEnable", (MANGOERROR_error_t)(MANGOERROR_SUCCESS - 1));
		}
		free(coff_fnames);
		free(err_cbs);
	}
}

void
Mango::Finish()
{
	MANGOERROR_error_t mangoStatus;

	if (InterlockedDecrement(&sReferenceCount) == 0)
	{
		if ((mangoStatus = MangoX_CardDisable(sCardNum)) != MANGOERROR_SUCCESS)
		{
			throw MXException("MangoX_CardDisable", mangoStatus);
		}
		if ((mangoStatus = MangoX_Close()) != MANGOERROR_SUCCESS)
		{
			throw MXException("MangoX_Close", mangoStatus);
		}
		sCard.close(&sCard);
		MANGOBIOS_close();
	}
}

void
Mango::showError(const char* api, MANGOERROR_error_t mangoStatus)
{
	string resultString = "An error occurred while calling MangoX services.\n";
	resultString += "Mango audio/video services will not function.\n";
	resultString += "Details:\n";
	resultString += "Operation/API: ";
	resultString += api;
	resultString += "\n";
	resultString += "Status: ";
	switch (mangoStatus) {
		case MANGOERROR_SUCCESS:
			resultString += "Success";
			break;
		case MANGOERROR_FAILURE:
			resultString += "Failure";
			break;
		case MANGOERROR_TIMEOUT:
			resultString += "Timeout";
			break;
		case MANGOERROR_ERR_INVALID_HANDLE:
			resultString += "Invalid handle";
			break;
		case MANGOERROR_ERR_NOT_IMPLEMENTED:	
			resultString += "Not implemented";
			break;
		case MANGOERROR_COFF_FORMAT_ERROR:
			resultString += "COFF format error";
			break;
		case MANGOERROR_ERR_INVALID_PARAMETER:
			resultString += "Invalid parameter";
			break;
		case MANGOERROR_INSUFFICIENT_RESOURCES:
			resultString += "Insufficient resources";
			break;
		case MANGOERROR_INVALID_CONFIGURATION:
			resultString += "Invalid configuration";
			break;
		case MANGOERROR_RESOURCE_NOT_READY:
			resultString += "Resource not ready";
			break;
		default:
			resultString += "unknown error";
			break;
	}
	OutputDebugStringA(resultString.c_str());
}
