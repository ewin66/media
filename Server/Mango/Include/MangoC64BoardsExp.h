/******************************************************************************
 * Copyright 2005 MangoDSP, Inc., all rights reserved.
 * Software and information contained herein is
 * confidential and proprietary to MangoDSP, and any unauthorized copying,
 * dissemination, or use is strictly prohibited.
 *
 *
 * FILENAME: MangoC64BoardsExp.h
 *
 * GENERAL DESCRIPTION: MangoC64Boards library exported api declarations
 *
 * DATE					AUTHOR
 *
 * <May 22 2005>		<Nachum Kanovsky>
 *
 * REMARKS:
 *
 *
 ******************************************************************************/
#ifndef __MANGOC64BOARDSEXP_H__
#define __MANGOC64BOARDSEXP_H__

#include "MangoShared.h"
#include "MangoError.h"
#include "MangoBios.h"
#include "CoffExp.h"
#include "MangoBoards.h"
#include "MngUtilitiesExp.h"

#include <stdio.h>

#if defined(WIN32)
#include <windows.h>
#elif defined(POSIX_COMPLIANT)
#include <pthread.h>
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* typedef of struct MANGOBIOS_dummy_t */
typedef struct{
	unsigned int bus_scan_start;
} MangoC64Boards_attrs_t;

typedef struct dsp_s{
	device_type_t device_type;
	MANGOBIOS_deviceHandle_t handle;
	int emif_init_index;
	int bridge_index;
	int device_index;
	int bus_width;
	MngMutex device_mutex;
} dsp_t;

typedef struct bridge_s{
	device_type_t device_type;
	MANGOBIOS_deviceHandle_t handle;
	MngMutex device_mutex;
} bridge_t;

/* structure for using the MangoC64Boards */
typedef struct MangoC64Boards_handle_s
{
	MANGOCARD_card_t card;
	int num_devices;
	int _num_bridges;
	bridge_t * _bridges;
	int _num_dsps;
	dsp_t * _dsps;
	int _num_buses;
	int _low_bus;
	emif_init_array_t _default_emif_init_array;
	//_handles is deprecated
	MANGOBIOS_deviceHandle_t * _handles;
	MANGOERROR_error_t (*close)(struct MangoC64Boards_handle_s * handle);
	MANGOERROR_error_t (*read_config)(struct MangoC64Boards_handle_s * handle, int dsp, int offset, void * data, int size);
	MANGOERROR_error_t (*write_config)(struct MangoC64Boards_handle_s * handle, int dsp, int offset, const void * data, int size);
	MANGOERROR_error_t (*read_memory)(struct MangoC64Boards_handle_s * handle, int dsp, void * hst_adr, unsigned int dsp_adr, unsigned int bytes);
	MANGOERROR_error_t (*write_memory)(struct MangoC64Boards_handle_s * handle, int dsp, const void * hst_adr, unsigned int dsp_adr, unsigned int bytes);
	MANGOERROR_error_t (*h2d_interrupt)(struct MangoC64Boards_handle_s * handle, int dsp);
	MANGOERROR_error_t (*reset)(struct MangoC64Boards_handle_s * handle, int dsp);
	MANGOERROR_error_t (*init_emif)(struct MangoC64Boards_handle_s * handle, emif_init_t * emif, int dsp);
	MANGOERROR_error_t (*load_from_file)(struct MangoC64Boards_handle_s * handle, int dsp, const char * file);
} MangoC64Boards_handle_t;

/* Summary
   Opens access to a single PCI board of specified type.
   
   Description
   This function receives a device list as created by
   MANGOBIOS_getDeviceHandles and the type of card requested. It
   will then scan the list and, if it finds a group of devices
   matching this board type's "footprint" it will mark those
   devices as open and initialize a handle for the application
   to use for accessing the board.
   
   Parameters
   %PAR0% :  Pointer for handle
   %PAR1% :  Array of MANGOBIOS_deviceHandle_t devices as generated by
             MANGOBIOS_getDeviceHandles.
   %PAR2% :  Length of device array, as given by MANGOBIOS_getNumDevices.
   %PAR3% :  A "footprint" corresponding to the board type to open.
             You should use the footprints supplied in the
             MangoBoards.c file.
   %PAR4% :  Normally NULL, but can control bus_scan_start. Normally
             the first board that will be opened is the one containing
             the lowest bus number, but you can force a different
             order by setting bus_scan_start to the minimal bus number
             that will be scanned (any board containing a lower bus
             number will be ignored).
   
   Returns
       * MANGOERROR_SUCCESS - Success
       * MANGOERROR_INVALID_CONFIGURATION - Could not find a
         device arrangement corresponding to the requested board type.
       * Other value - Error from MANGOBIOS_deviceOpen or
         MANGOBIOS_deviceGetProperty.
   
   Remarks
       * The 'devices' array given should always be the start of
         the device array. There is no need to increment this pointer
         as opened devices are internally marked.
   
   Example
   
   <CODE>MangoC64Boards_handle_t card;
   int num_dev;
   MANGOBIOS_deviceHandle_t * devices;
   if(MANGOBIOS_getNumDevices(NULL, &num_dev) != MANGOERROR_SUCCESS)
       return -1;
   devices = (MANGOBIOS_deviceHandle_t *)malloc(sizeof(MANGOBIOS_deviceHandle_t) * num_dev);
   if (MANGOBIOS_getDeviceHandles(NULL, devices) != MANGOERROR_SUCCESS)
       return -1;
   if (MangoC64Boards_Open(&card, devices, num_dev, &SEAGULL_PMC_BOARD, NULL) != MANGOERROR_SUCCESS)
       return -1;
   </CODE>                                                                                          */
MANGOERROR_error_t
MangoC64Boards_Open(
	MangoC64Boards_handle_t * handle,
	MANGOBIOS_deviceHandle_t * devices,
	int num_handles,
	const board_footprint_t * footprint,
	const MangoC64Boards_attrs_t * attrs
);

/* Summary
   Gets MANGOBIOS_version_t
   
   Description
   Gets version information for MangoC64Boards Library
   
   Parameters
   \version :  Pointer for version information
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   MANGOBIOS_version_t version;
   
   errorCode = MangoC64Boards_Get_Version(
    &version
    );
   </CODE>                                          */
MANGOERROR_error_t
MangoC64Boards_Get_Version(
	MANGOBIOS_version_t * version
);

#ifdef __cplusplus
}
#endif

#endif /* __MANGOC64BOARDSEXP_H__ */
