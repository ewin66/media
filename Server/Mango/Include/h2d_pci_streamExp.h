/******************************************************************************
 * Copyright 2005 MangoDSP, Inc., all rights reserved.
 * Software and information contained herein is
 * confidential and proprietary to MangoDSP, and any unauthorized copying,
 * dissemination, or use is strictly prohibited.
 *
 *
 * FILENAME: h2d_pci_streamExp.h
 *
 * GENERAL DESCRIPTION: h2d_pci_stream library exported api declarations
 *                              
 * DATE CREATED     	AUTHOR
 *  
 * <May 22 2005> 	<Nachum Kanovsky> 	
 *    
 * REMARKS: 
 *    
 *    
 ******************************************************************************/
#ifndef __H2D_PCI_STREAMEXP_H__
#define __H2D_PCI_STREAMEXP_H__

#include "MangoShared.h"
#include "MangoError.h"
#include "MangoBios.h"
#include "MngUtilitiesExp.h"

#include "h2d_pci_streamSharedExp.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef MANGOERROR_error_t (*PCI_READ_t)(void * handle, int dsp, void * hst_adr, unsigned int dsp_adr, unsigned int bytes_to_read);
typedef MANGOERROR_error_t (*PCI_WRITE_t)(void * handle, int dsp, void * hst_adr, unsigned int dsp_adr, unsigned int bytes_to_write);
typedef MANGOERROR_error_t (*PCI_H2D_INTERRUPT_t)(void * handle, int dsp);

enum{
	H2D_PCI_GET_STATUS,
	H2D_PCI_WAIT_CONNECT
	};

typedef struct QUE_Elem_s{
	struct QUE_Elem_s *next;
	struct QUE_Elem_s *prev;
} QUE_Elem_t;

typedef struct{
	void * local;
	unsigned int pci;
} PCI_STREAM_ptr_t;

typedef struct{
	void * card;
	int dsp;
	PCI_READ_t read_fxn;
	PCI_WRITE_t write_fxn;
	PCI_H2D_INTERRUPT_t h2d_interrupt_fxn;
	unsigned int shared_memory_start;
	unsigned int shared_memory_size;
	int use_polling;
} h2d_pci_device_params_t;

typedef struct{
	/* when running PCI_STREAM_reclaim, the timeout determines how
	   \long to wait before returning a timeout on the stream when
	   there is no data - MANGOBIOS_FOREVER or MANGOBIOS_POLL    */
	int timeout;
} h2d_pci_channel_params_t;

typedef struct{
	void * card;
	int dsp;
	PCI_READ_t read_fxn;
	PCI_WRITE_t write_fxn;
	PCI_H2D_INTERRUPT_t h2d_interrupt_fxn;

	unsigned int db_sync_resp; //from remote
	unsigned int mb_sync_resp; //from remote
	unsigned int db_sync_resp_ack; //to remote

	unsigned int db_data_resp; //from remote
	unsigned int mb_data_resp; //from remote
	unsigned int db_data_resp_ack; //to remote

	unsigned int db_sync_init; //to remote
	unsigned int mb_sync_init; //to remote
	unsigned int db_sync_init_ack; //from remote

	unsigned int db_data_init; //to remote
	unsigned int mb_data_init; //to remote
	unsigned int db_data_init_ack; //from remote

	int db_s_resp_exp;
	int db_d_resp_exp;
	int db_s_init_ack_exp;
	int db_d_init_ack_exp;

	int db_s_init_exp;
	int db_d_init_exp;
	int db_s_resp_ack_exp;
	int db_d_resp_ack_exp;
	int use_polling;

	MngSem syncsem;
	MngSem datasem;
	MngSem mutex;
	QUE_Elem_t * channels;
} h2d_pci_device_t;
	
typedef struct{
	QUE_Elem_t link;
	int chan_num;
	unsigned int remote_chan;
	MngSem connected;
	MngPipe local_que_in, local_que_out;
	int mode;
	char name[MAX_CHANNEL_NAME_LENGTH];
	
	h2d_pci_device_t * dev;
	int timeout;
	int open_count;
} h2d_pci_channel_t;

/* Summary
   Gets MANGOBIOS_version_t
   
   Description
   Gets version information for h2d_pci_stream Library
   
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   MANGOBIOS_version_t version;
   
   errorCode = h2d_pci_stream_Get_Version(
    &version
    );
   </CODE>                                  */
MANGOERROR_error_t
h2d_pci_stream_Get_Version(
	MANGOBIOS_version_t * version
);

/* Summary
   Creates a h2d_pci_stream target device
   
   Description
   Creates a h2d_pci_device_t. This will be a future target for
   PCI_STREAM_create calls. h2d_pci_device_t holds all of the
   information for referring to a single DSP destination on a
   Mango DSP card.
   
   Parameters
   params :  The parameters for the target
   Returns
   Pointer to the newly allocated h2d_pci_device_t
   
   NULL is returned if:
   
   \- params is NULL
   
   \- params-\>shared_memory_size is less than required for library
   
   \- Failed memory allocation
   
   \- params-\>fm_type is not PCI_FM_off
   
   Remarks
   This can be called at any time. This does not require that
   the DSP is running to work.
   
   Example
   
   <CODE>h2d_pci_device_t * dev;
   PCI_DEVICE_PARAMS_t params;
   params.card = &asb_handle;
   params.dsp = 0;
   params.fm_type = PCI_FM_off;
   params.fm_event = NULL;
   params.read_fxn = asb_handle.read_memory;
   params.write_fxn = asb_handle.write_memory;
   params.h2d_interrupt = asb_handle.h2d_interrupt;
   params.shared_memory_start = 0x80000000;
   params.shared_memory_size = 0x100;
   dev = PCI_STREAM_devInit(
    &params
   );
   </CODE>                                                      */
h2d_pci_device_t * 
PCI_STREAM_devInit(
	h2d_pci_device_params_t * params
);


/* Summary
   Deletes a h2d_pci_stream target device
   
   Description
   Deletes a h2d_pci_device_t.
   
   Parameters
   dev :  Pointer to h2d_pci_device_t
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :     Success
   MANGOERROR_INVALID_PARAMETER :  'dev' is NULL
   
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   errorCode = PCI_STREAM_devShutdown(
    dev
   );
   </CODE>                                                      */
int
PCI_STREAM_devShutdown(
	h2d_pci_device_t * dev
);


/* Summary
   Creates a h2d_pci_stream channel
   
   Description
   Creates a h2d_pci_channel_t on 'dev'. This channel will
   connect to the corresponding channel created on the target
   dsp synchronizing on 'name' and 'mode'. See Remarks.
   
   Parameters
   dev :      The device object
   name :     The name of the channel. This must be identical to
              the name used on the DSP.
   bufsize :  Maximum size in bytes of the chunks that this stream
              can transfer
   mode :     PCI_STREAM_INPUT or PCI_STREAM_OUTPUT, see remarks
   params :   The parameters for timeout and type
   Returns
   Pointer to the newly allocated h2d_pci_channel_t
   
   NULL is returned if:
   
   \- params is NULL
   
   \- name is longer than MAX_CHANNEL_NAME_LENGTH
   
   \- Failed memory allocation
   
   \- an error occured with synchronization with the DSP (reset
   the DSP and host, and try again)
   
   Remarks
   Must be run only after DSP has performed device
   initialization. This occurs before the DSP reaches main().
   
   Channels are created on the DSP side using SIO_create. 'name'
   needs to be identical to the name used on the dsp. This
   function will not initialize the channel with any empty
   buffers.
   
   'bufsize' will be the maximum size of any buffer going
   through this channel. 'params' will determine the timeout of
   this channel, and the channel's transfer type (eg
   PCI_dsp_master..).
   
   For a channel on the host to connect to a channel on the DSP,
   \one side must use input, and the other side must use output.
   (On the DSP, the input is SIO_INPUT, and the output is
   SIO_OUTPUT)
   
   Example
   
   <CODE>h2d_pci_channel_t * chan;
   PCI_CHANNEL_PARAMS_t params;
   params.type = PCI_dsp_master;
   params.timeout = PCI_STREAM_FOREVER;
   chan = PCI_STREAM_create(
    dev,
    "/dioPciDataChannel1",
    PCI_STREAM_OUTPUT,
    0x1000,
    &params
   );
   </CODE>                                                         */
h2d_pci_channel_t *
PCI_STREAM_create(
	h2d_pci_device_t * dev,
	char * name,
	int mode,
	unsigned int bufsize,
	h2d_pci_channel_params_t * params
);


/* Summary
   Deletes a h2d_pci_stream channel
   
   Description
   Deletes a h2d_pci_channel_t.
   
   Parameters
   chan :     The channel object
   Returns
   Status
   
   Return Value List
   PCI_OK :     Success
   PCI_ERROR :  'chan' is NULL
   Remarks
   None
   
   Example
   
   <CODE>PCI_STREAM_delete(
	chan,
   );
   </CODE>                                                         */
int
PCI_STREAM_delete(
	h2d_pci_channel_t * chan
);

/* Summary
   Issues a buffer to a stream
   
   Description
   Places a buffer in this stream's output que, and signals the
   DSP to update the same channel on the DSP. If there is a
   buffer issued on both ends, then a transaction takes place.
   
   Parameters
   chan :    The channel object
   pbuf :    Pointer to the buffer structure
   nmadus :  Size in bytes of the buffer
   Returns
   Status
   
   Return Value List
   PCI_OK :     Success
   PCI_ERROR :  'chan' is NULL<P>
                'pbuf'.local or 'pbuf'.pci is NULL<P>
                Failed memory allocation
   Remarks
   Must be run only after DSP has performed device
   initialization. This occurs before the DSP reaches main().
   
   Example
   
   <CODE>PCI_STREAM_ptr_t pbuf;
   MANGOBIOS_memoryHandle_t handle;
   MANGOBIOS_memoryAlloc(
    0x1000,
    &handle,
    &pbuf.pci,
    NULL
   );
   MANGOBIOS_memoryMap(
    handle,
    &pbuf.local,
    NULL
   );
   PCI_STREAM_issue(
    chan,
    pbuf,
    0x1000
   );
   </CODE>                                                      */
int
PCI_STREAM_issue(
	h2d_pci_channel_t * chan,
	PCI_STREAM_ptr_t pbuf,
	unsigned int nmadus
);

/* Summary
   Reclaims a buffer from a stream
   
   Description
   Gets a buffer from this stream's output que if available. If
   there is no buffer available, then it pends for up to the
   timeout set for this channel.
   
   Parameters
   chan :  The channel object
   pbuf :  Pointer to the buffer structure
   Returns
   Status
   
   Return Value List
   PCI_OK :       Success
   PCI_ERROR :    'chan' is NULL<P>
                  'pbuf' is NULL<P>
                  No buffers are outstanding in this stream. ie\:
                  either no buffers have been issued, or every buffer
                  issued has already been reclaimed.
   PCI_TIMEOUT :  No buffer was avaiable within the timeout set for
                  this channel.
   Remarks
   Must be run only after DSP has performed device
   initialization. This occurs before the DSP reaches main().
   
   If the DSP side has not performed an SIO_create which
   connects to this channel, then the host will not receive any
   buffers, and either pend forever until the DSP creates the
   channel, and issues a buffer, or it will continously return a
   PCI_TIMEOUT
   
   Example
   
   <CODE>int size = 0;
   size = PCI_STREAM_reclaim(
    chan,
    &pbuf
   );
   if(size \< 0) //error
   </CODE>                                                            */
int
PCI_STREAM_reclaim(
	h2d_pci_channel_t * chan,
	PCI_STREAM_ptr_t * pbuf
);

int
PCI_STREAM_ctrl(
	h2d_pci_channel_t * chan,
	unsigned int cmd,
	void * args
);

void
PCI_STREAM_intCb(
	h2d_pci_device_t * dev
);

#ifdef __cplusplus
}
#endif

#endif /* __H2D_PCI_STREAMEXP_H__ */

