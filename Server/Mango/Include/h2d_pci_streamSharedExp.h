/******************************************************************************
 * Copyright 2005 MangoDSP, Inc., all rights reserved.
 * Software and information contained herein is
 * confidential and proprietary to MangoDSP, and any unauthorized copying,
 * dissemination, or use is strictly prohibited.
 *
 *
 * FILENAME: h2d_pci_streamSharedExp.h
 *
 * GENERAL DESCRIPTION: h2d_pci_stream shared exported header file
 *                              
 * DATE CREATED     	AUTHOR
 *  
 * <May 22 2005> 	<Nachum Kanovsky> 	
 *    
 * REMARKS: 
 *    
 *    
 ******************************************************************************/
#ifndef __H2D_PCI_STREAMSHAREDEXP_H__
#define __H2D_PCI_STREAMSHAREDEXP_H__

#ifdef __cplusplus
extern "C" {
#endif

/* defines */
#define MAX_CHANNEL_NAME_LENGTH 32
#define MAX_STREAM_NAME_LENGTH 64

typedef struct h2d_pci_status_args_s{
	/* equal to TRUE when connected  */
	unsigned int connected;
} h2d_pci_status_args_t;

#ifdef __cplusplus
}
#endif

#endif /* __H2D_PCI_STREAMSHAREDEXP_H__ */



