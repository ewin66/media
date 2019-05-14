/******************************************************************************
 * Copyright 2005 MangoDSP, Inc., all rights reserved.
 * Software and information contained herein is
 * confidential and proprietary to MangoDSP, and any unauthorized copying,
 * dissemination, or use is strictly prohibited.
 *
 *
 * FILENAME: 
 *
 * GENERAL DESCRIPTION:
 *                              
 * DATE CREATED     	AUTHOR
 *  
 * <May 22 2005> 	<Nachum Kanovsky> 	
 *    
 * REMARKS: 
 *    
 *    
 ******************************************************************************/

/* Summary
   MangoBios header file
   
   Description
   MangoBios API declarations
   
   Remarks
   Bugs
   TODO
   History
   
   <TABLE>
   \Author           Change Description
   ----------------  -------------------
   Nachum Kanovsky   Created
   </TABLE>                              */

#ifndef __MANGOBIOS_H__
#define __MANGOBIOS_H__

#include <stdio.h>

#include "MangoShared.h"

#ifdef __cplusplus
extern "C" {
#endif

#if defined(__vxworks)
#if _BYTE_ORDER == _BIG_ENDIAN
#define IS_BIG_ENDIAN
#endif /*_BYTE_ORDER == _BIG_ENDIAN*/
#elif defined(linux) || defined(__SVR4)
#if defined(_BIG_ENDIAN)
#define IS_BIG_ENDIAN
#endif /*defined(_BIG_ENDIAN)*/
#endif /*defined(__vxworks) -> defined(linux) || defined(__SVR4)*/

#if defined(IS_BIG_ENDIAN)
#define PCI_SWAP32(x) SWAP32(x)
#define PCI_SWAP16(x) SWAP16(x)
#undef IS_BIG_ENDIAN
#else
#define PCI_SWAP32(a) (a)
#define PCI_SWAP16(a) (a)
#endif /*defined(IS_BIG_ENDIAN)*/

#if defined(linux) || defined(__SVR4) || defined(__vxworks)
#define POSIX_COMPLIANT
#endif /*defined(linux) || defined(__SVR4)*/

#if defined(WIN32)
#include <windows.h>
#endif

#include "MangoError.h"

enum MANGOBIOS_deviceProp_e{
	MANGOBIOS_deviceProp_Function,
	MANGOBIOS_deviceProp_Device,
	MANGOBIOS_deviceProp_Bus,
	MANGOBIOS_deviceProp_UI,

	NUM_BOARD_PROP
};

typedef enum MANGOBIOS_deviceProp_e MANGOBIOS_deviceProp_t;

enum MANGOBIOS_quanta_e{
	Q_8 = 1,
	Q_16 = 2,
	Q_32 = 4,
	Q_64 = 8,
	Q_ANY = (Q_8 | Q_16 | Q_32 | Q_64) /* quanta is chosen by the system */
};

typedef enum MANGOBIOS_quanta_e MANGOBIOS_quanta_t;

struct MANGOBIOS_dummy_s
{
	int dummy;
};

typedef struct MANGOBIOS_dummy_s MANGOBIOS_attrs_t;
typedef struct MANGOBIOS_dummy_s MANGOBIOS_deviceAttrs_t;
typedef struct MANGOBIOS_dummy_s MANGOBIOS_memoryAllocAttrs_t;
typedef void * MANGOBIOS_memoryHandle_t;
typedef int MANGOBIOS_version_t;

typedef struct MANGOBIOS_deviceType_s{
	unsigned int vendorId;
	unsigned int deviceId;
	unsigned int subVendorId;
	unsigned int subSystemId;
} MANGOBIOS_deviceType_t;

typedef struct MANGOBIOS_deviceHandle_s{
#if defined(WIN32)
	HANDLE handle;
#else
	int fd;
#endif
	MANGOBIOS_deviceType_t type;
	unsigned int dwIndex;
} MANGOBIOS_deviceHandle_t;

typedef struct MANGOBIOS_memoryMapAttrs_s{
	int cacheEnable;
} MANGOBIOS_memoryMapAttrs_t;

#if defined(__vxworks)
typedef int (*IsrInitFunc_T)(MANGOBIOS_deviceHandle_t * handle, void * param);
typedef void (*IsrFunc_T)(void * param);
typedef void (*IsrShutdownFunc_T)(MANGOBIOS_deviceHandle_t * handle, void * param);
#endif
/* builds versioning information */
#define MANGOBIOS_version_build(major, minor) ((major << 16) | minor)

/* Summary
   Gets MANGOBIOS_version_t
   
   Description
   Gets versioning information about this library
   
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
   errorCode = MANGOBIOS_getVersion(
    &version
    );
   </CODE>                                        */
MANGOERROR_error_t
MANGOBIOS_getVersion(
	MANGOBIOS_version_t * version
);


/* Summary
   Initializes MangoBios Library for this process
   
   Description
   Opens the MangoWdm driver and MangoMem driver.
   
   Parameters
   attrs :  NULL
   Returns
   Status
   
   Return Value List
   MANGOERROR_INVALID_CONFIGURATION :   Failed LoadLibrary for SetupApi.dll<P>
                                        Failed GetProcAddress for
                                        SetupDiEnumDeviceInterfaces or
                                        SetupDiGetDeviceInterfaceDetailA or
                                        SetupDiDestroyDeviceInfoList or
                                        SetupDiGetClassDevsA<P>
                                        No MangoMem device exists
   MANGOERROR_ERR_INVALID_HANDLE :      Failed SetupDiGetClassDevs_p for MangoWdm
                                        driver or MangoMem driver.<P>
                                        Failed CreateFile on MangoMem
   MANGOERROR_INSUFFICIENT_RESOURCES :  Failed malloc
   Other value :                        Error from OS
   Remarks
   There must be one MangoMem instance in the system, and the
   MangoWdm (pci device driver) must be known to the system for
   MANGOBIOS_open to pass.
   
   Example
   
   <CODE>int errorCode;
   errorCode = MANGOBIOS_open(
    NULL
    );
   </CODE>                                                                        */
MANGOERROR_error_t
MANGOBIOS_open(
	const MANGOBIOS_attrs_t * attrs
);


/* Summary
   Closes MangoBios library for this process
   
   Description
   Closes all handles previously opened in MANGOBIOS_open
   
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   errorCode = MANGOBIOS_close(
    );
   </CODE>                                                */
MANGOERROR_error_t
MANGOBIOS_close(
);


/* Summary
   Gets number of devices of MANGOBIOS_deviceType_t type
   
   Description
   Sets numDevices to number of matching devices to type found
   in the system
   
   Parameters
   type :        The device type being requested<P>
                 A NULL value will return all supported devices
   numDevices :  Pointer for number of devices found
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                 Success
   MANGOERROR_ERR_INVALID_PARAMETER :   numDevices is NULL
   MANGOERROR_INSUFFICIENT_RESOURCES :  Failed malloc
   MANGOERROR_ERR_INVALID_HANDLE :      Failed CreateFile for
                                        device (Needed for matching
                                        'type')
   Other value :                        Error from OS
   Remarks
   'type' should be the same in both MANGOBIOS_getNumDevices,
   and in MANGOBIOS_getDeviceHandles, otherwise the number of
   devices that the user will allocate could be insufficient to
   receive the number of devices that MANGOBIOS_getDeviceHandles
   will return, causing an overflow.
   
   Example
   
   <CODE>int errorCode;
   int num;
   MANGOBIOS_deviceType_t type = {0x8086, 0xB555, 0x0000, 0x0000};
   MANGOBIOS_deviceHandle_t * handles;
   
   errorCode = MANGOBIOS_getNumDevices(
    &type,
    &num
    );
   if(errorCode != MANGOERROR_SUCCESS)
    return -1;
   handles = (MANGOBIOS_deviceHandle_t *)malloc(
    sizeof(MANGOBIOS_deviceHandle_t) * num
    );
   errorCode = MANGOBIOS_getDeviceHandles(
    &type,
    handles
    );
   if(errorCode != MANGOERROR_SUCCESS)
    return -1;
   </CODE>                                                          */
MANGOERROR_error_t
MANGOBIOS_getNumDevices(
	const MANGOBIOS_deviceType_t * type,
	int * numDevices
);


/* Summary
   Fills a MANGOBIOS_deviceHandle_t array
   
   Description
   Fills in the handle array with devices matching type
   
   Parameters
   type :    The device type being requested<P>
             A NULL value will return all supported devices
   handle :  Previously allocated array of MANGOBIOS_deviceHandle_t
             handles
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  'handle' is NULL
   MANGOERROR_ERR_INVALID_HANDLE :     Failed CreateFile for
                                       device
   Other value :                       Error from OS
   Remarks
   MANGOBIOS_getDeviceHandles expects a non-NULL input for
   'handle.' To retrieve the number of devices matching 'type'
   in the system, call MANGOBIOS_getNumDevices first.
   
   'type' should be the same in both MANGOBIOS_getNumDevices,
   and in MANGOBIOS_getDeviceHandles, otherwise the number of
   devices that the user will allocate could be insufficient to
   receive the number of devices that MANGOBIOS_getDeviceHandles
   will return, causing an overflow.
   
   Example
   
   <CODE>int errorCode;
   int num;
   MANGOBIOS_deviceType_t type = {0x8086, 0xB555, 0x0000, 0x0000};
   MANGOBIOS_deviceHandle_t * handles;
   
   errorCode = MANGOBIOS_getNumDevices(
    &type,
    &num
    );
   if(errorCode != MANGOERROR_SUCCESS)
    return -1;
   handles = (MANGOBIOS_deviceHandle_t *)malloc(
    sizeof(MANGOBIOS_deviceHandle_t) * num
    );
   errorCode = MANGOBIOS_getDeviceHandles(
    &type,
    handles
    );
   if(errorCode != MANGOERROR_SUCCESS)
    return -1;
   </CODE>                                                          */
MANGOERROR_error_t
MANGOBIOS_getDeviceHandles(
	const MANGOBIOS_deviceType_t * type,
	MANGOBIOS_deviceHandle_t * handle
);


/* Summary
   Allocates physical memory to MANGOBIOS_memoryHandle_t
   
   Description
   Allocates 'size' bytes of physical memory using the MangoMem
   driver
   
   Parameters
   size :         Length in bytes of requested memory buffer
   handle :       handle for memory buffer
   physicalAdr :  Pointer for physical address of memory buffer
   attrs :        NULL
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Other value :         Error from OS
   Remarks
   Actual size of buffer is based on granularity in the MangoMem
   driver, therefore will most likely be up to 4Kbytes larger
   than requested.
   
   Example
   
   <CODE>int errorCode;
   int size = 32768;
   MANGOBIOS_memoryHandle_t handle;
   unsigned int physicalAdr;
   
   errorCode = MANGOBIOS_memoryAlloc(
    size,
    &handle,
    &physicalAdr,
    NULL
    );
   </CODE>                                                       */
MANGOERROR_error_t
MANGOBIOS_memoryAlloc(
	int size,
	MANGOBIOS_memoryHandle_t * handle,
	unsigned int * physicalAdr,
	const MANGOBIOS_memoryAllocAttrs_t * attrs
); 


/* Summary
   Frees a MANGOBIOS_memoryHandle_t
   
   Description
   Frees a non-mapped memory buffer which was previously
   allocated with MANGOBIOS_memoryAlloc
   
   Parameters
   handle :  Handle to memory buffer
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Other value :         Error from OS
   Remarks
   Will return an error if the memory is mapped. Call
   MANGOBIOS_memoryUnmap on a memory buffer to unmap it.
   
   Example
   
   <CODE>int errorCode;
   
   errorCode = MANGOBIOS_memoryFree(
    memory_handle
    );
   </CODE>                                               */
MANGOERROR_error_t
MANGOBIOS_memoryFree(
	MANGOBIOS_memoryHandle_t handle
);



/* Summary
   Maps physical memory of a MANGOBIOS_memoryHandle_t
   
   Description
   Maps memory associated with 'handle' to virtual memory
   
   Parameters
   handle :      handle of memory buffer
   virtualAdr :  Pointer for virtual address of memory buffer
   attrs :       Set of attributes used when mapping the memory
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Other value :         Error from OS
   Remarks
   Actual size of buffer is based on granularity in the MangoMem
   driver, therefore will most likely be up to 4Kbytes larger
   than requested.
   
   Example
   
   <CODE>int errorCode;
   int size = 32768;
   MANGOBIOS_memoryHandle_t handle;
   unsigned int physicalAdr;
   char * buffer;
   MANGOBIOS_memoryMapAttrs_t attrs;
   
   attrs.cacheEnable = 1;
   errorCode = MANGOBIOS_memoryAlloc(
    size,
    &handle,
    &physicalAdr,
    NULL
    );
   errorCode = MANGOBIOS_memoryMap(
    handle,
    &buffer,
    attrs
    );
   memset(buffer, 0, size);
   </CODE>                                                       */
MANGOERROR_error_t
MANGOBIOS_memoryMap(
	MANGOBIOS_memoryHandle_t handle,
	void ** virtualAdr,
	const MANGOBIOS_memoryMapAttrs_t * attrs
);


/* Summary
   Unmaps a MANGOBIOS_memoryHandle_t
   
   Description
   Unmaps a mapped memory buffer which was previously allocated
   with MANGOBIOS_memoryAlloc and mapped with
   MANGOBIOS_memoryMap
   
   Parameters
   handle :  Handle to mapped memory buffer
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Other value :         Error from OS
   Remarks
   Will fail if the memory was not successfully mapped using
   MANGOBIOS_memoryMap.
   
   Example
   
   <CODE>int errorCode;
   
   errorCode = MANGOBIOS_memoryUnmap(
    memory_handle
    );
   </CODE>                                                      */
MANGOERROR_error_t
MANGOBIOS_memoryUnmap(
	MANGOBIOS_memoryHandle_t handle
);


/* Summary
   Opens MANGOBIOS_deviceHandle_t
   
   Description
   Opens device handle
   
   Parameters
   handle :  Handle to device
   attrs :   NULL
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                 Success
   MANGOERROR_ERR_INVALID_PARAMETER :   Invalid handle
   MANGOERROR_INSUFFICIENT_RESOURCES :  Failed malloc
   Other value :                        Error from OS
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   errorCode = MANGOBIOS_deviceOpen(
    handles[i],
    NULL
    );
   </CODE>                                             */
MANGOERROR_error_t
MANGOBIOS_deviceOpen(
	MANGOBIOS_deviceHandle_t * handle,
	const MANGOBIOS_deviceAttrs_t * attrs
);


/* Summary
   Closes MANGOBIOS_deviceHandle_t
   
   Description
   Closes device handle which was previously opened with
   MANGOBIOS_deviceOpen
   
   Parameters
   handle :  Handle to device
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  Invalid handle
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   errorCode = MANGOBIOS_deviceClose(
    &device_handle
    );
   </CODE>                                               */
MANGOERROR_error_t
MANGOBIOS_deviceClose(
	MANGOBIOS_deviceHandle_t * handle
);


/* Summary
   Gets a MANGOBIOS_deviceProp_t
   
   Description
   Gets a property for this handle
   
   Parameters
   handle :    Handle to device
   property :  Property to get
   val :       Pointer for value
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  Invalid handle<P>
                                       Illegal property choice
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   int bus_no;
   errorCode = MANGOBIOS_deviceGetProperty(
    &device_handle,
    MANGOBIOS_deviceProp_Bus,
    &bus_no
    );
   </CODE>                                                     */
MANGOERROR_error_t
MANGOBIOS_deviceGetProperty(
	MANGOBIOS_deviceHandle_t * handle,
	MANGOBIOS_deviceProp_t property,
	int * val
);


/* Summary
   Sets a MANGOBIOS_deviceProp_t
   
   Description
   Sets a property for this handle
   
   Parameters
   handle :    Handle to device \: Value
   property :  Property to be set
   val :       Value
   Returns
   Status
   
   Return Value List
   MANGOERROR_ERR_INVALID_PARAMETER :  Failure, not implemented
   Remarks
   None                                                         */
MANGOERROR_error_t
MANGOBIOS_deviceSetProperty(
	MANGOBIOS_deviceHandle_t * handle,
	MANGOBIOS_deviceProp_t property,
	int val
);


/* Summary
   Reads a pci register from a MANGOBIOS_deviceHandle_t
   
   Description
   Reads a pci register from a device opened with
   MANGOBIOS_deviceOpen
   
   Parameters
   handle :     Handle to device
   regOffset :  Byte offset into pci register space
   regVal :     Pointer for value
   size :       Number of bytes to read (1,2,4)
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  Handle is invalid
   Other value :                       Error from OS
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   int dev_ven_id;
   errorCode = MANGOBIOS_devicePciRegRead(
    &device_handle,
    0x0, (Offset in PCI register space for the Device/Vendor ID)
    &dev_ven_id,
    4
    );
   </CODE>                                                       */
MANGOERROR_error_t
MANGOBIOS_devicePciRegRead(
	MANGOBIOS_deviceHandle_t * handle,
	int regOffset,
	void * regVal,
	int size
);

/* Summary
   Writes a pci register to a MANGOBIOS_deviceHandle_t
   
   Description
   Writes a pci register to a device opened with
   MANGOBIOS_deviceOpen
   
   Parameters
   handle :     Handle to device
   regOffset :  Byte offset into pci register space
   regVal :     Pointer to value
   size :       Number of bytes to read (1,2,4)
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  Handle is invalid
   Other value :                       Error from OS
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   int bar0 = 0xffa00000;
   errorCode = MANGOBIOS_devicePciRegWrite(
    &device_handle,
    0x10, (Offset in PCI register space for Base Address Register 0)
    &bar0,
    4
    );
   </CODE>                                                           */
MANGOERROR_error_t
MANGOBIOS_devicePciRegWrite(
	MANGOBIOS_deviceHandle_t * handle,
	int regOffset,
	const void * regVal,
	int size
);


/* Summary
   Reads from a MANGOBIOS_deviceHandle_t
   
   Description
   Reads from any bar on a device opened with MANGOBIOS_open
   
   Parameters
   handle :          Handle to device
   bar :             Number of PCI BAR
   offset :          Offset in bytes from start of given BAR
   buff :            Pointer for received data
   size :            Number of bytes to read
   quanta :          Number of bytes to be read on each access of the
                     PCI bus
   increment_flag :  True increments the address being read from by the
                     quanta being read after each read, False rereads
                     from the same PCI location each time
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  Handle is invalid<P>
                                       Increment_flag is false and the
                                       quanta is Q_ANY
   Other value :                       Error from OS
   Remarks
   Whether increment_flag is true or false, the buff variable
   will be fully filled. It is only the PCI address that is
   being incremented dependent on the increment_flag, the local
   buffer is always incremented.
   
   Example
   
   <CODE>int errorCode;
   void * buffer = malloc(0x1000);
   if(!buffer)
    return -1;
   errorCode = MANGOBIOS_deviceRead(
    &device_handle,
    2,
    0,
    buffer,
    0x1000,
    Q_32,
    1
    );
   </CODE>                                                              */
MANGOERROR_error_t
MANGOBIOS_deviceRead(
	MANGOBIOS_deviceHandle_t * handle,
	int bar,
	int offset,
	void * buff,
	int size,
	MANGOBIOS_quanta_t quanta,
	int increment_flag
);

/* Summary
   Writes to a MANGOBIOS_deviceHandle_t
   
   Description
   Writes to any bar on a device opened with MANGOBIOS_open
   
   Parameters
   handle :          Handle to device
   bar :             Number of PCI BAR
   offset :          Offset in bytes from start of given BAR
   buff :            Pointer for received data
   size :            Number of bytes to read
   quanta :          Number of bytes to be read on each access of the
                     PCI bus
   increment_flag :  True increments the address being read from by the
                     quanta being read after each read, False rereads
                     from the same PCI location each time
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  Handle is invalid<P>
                                       Increment_flag is false and the
                                       quanta is Q_ANY
   Other value :                       Error from OS
   Remarks
   Whether increment_flag is true or false, the buff variable
   will be fully filled. It is only the PCI address that is
   being incremented dependent on the increment_flag, the local
   buffer is always incremented.
   
   Example
   
   <CODE>int errorCode;
   void * buffer = malloc(0x1000);
   memset(buffer, 0, 0x1000);
   if(!buffer)
    return -1;
   errorCode = MANGOBIOS_deviceWrite(
    &device_handle,
    2,
    0,
    buffer,
    0x1000,
    Q_32,
    1
    );
   </CODE>                                                              */
MANGOERROR_error_t
MANGOBIOS_deviceWrite(
	MANGOBIOS_deviceHandle_t * handle,
	int bar,
	int offset,
	const void * buff,
	int size,
	MANGOBIOS_quanta_t quanta,
	int increment_flag
);


/* Summary
   Connects an ISR to a MANGOBIOS_deviceHandle_t
   
   Description
   Directs the MangoWDM driver to use an ISR that was compiled
   into the MangoWDM1 driver
   
   Parameters
   handle :      Handle to device
   isrInitNum :  Array index into IsrInitFuncs for initializing this
                 ISR
   isrNum :      Array index into IsrFuncs for attaching the
                 interrupt vector to
   buff :        Pointer to a buffer that IsrInitFuncs[isrInitNum]
                 will receive
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Status
   MANGOERROR_ERR_INVALID_PARAMETER :  'handle' is invalid
   Other value :                       Error from OS
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   int isrInitNum = 0;
   int isrNum = 0;
   HANDLE config;
   
   config = CreateEvent(
    NULL,
    FALSE,
    FALSE,
    NULL
    );
   errorCode = MANGOBIOS_isrConnect(
    &device_handle,
    isrInitNum,
    isrNum,
    &config (this will allow the isr to reference this object and set this event on each interrupt)
    );
   </CODE>                                                                                          */
MANGOERROR_error_t
MANGOBIOS_isrConnect(
	MANGOBIOS_deviceHandle_t * handle,
	int isrInitNum,
	int isrNum,
	int isrShutdownNum,
	void * buff
);


/* Summary
   Disconnects an ISR from a MANGOBIOS_deviceHandle_t
   
   Description
   Directs the MangoWDM driver to disconnect the ISR that was
   connected to 'handle'
   
   Parameters
   handle :          handle to device
   isrShutdownNum :  Array index into IsrShutdownFuncs for erasing
                     everything done with the previously used
                     IsrInitFuncs[isrNum] (as performed in
                     MANGOBIOS_isrConnect)
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_ERR_INVALID_PARAMETER :  'handle' is invalid
   Other value :                       Error from OS
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   int isrShutdownNum = 0;
   
   errorCode = MANGOBIOS_isrConnect(
    &device_handle,
    isrShutdownNum
    );
   </CODE>                                                         */
MANGOERROR_error_t
MANGOBIOS_isrDisconnect(
	MANGOBIOS_deviceHandle_t * handle
);


#ifdef __cplusplus
}
#endif

#endif /* __MANGOBIOS_H__ */
