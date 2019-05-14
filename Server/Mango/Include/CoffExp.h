/******************************************************************************
 * Copyright 2005 MangoDSP, Inc., all rights reserved.
 * Software and information contained herein is
 * confidential and proprietary to MangoDSP, and any unauthorized copying,
 * dissemination, or use is strictly prohibited.
 *
 *
 * FILENAME: CoffExp.h
 *
 * GENERAL DESCRIPTION: COFF library exported api declarations
 *                              
 * DATE CREATED     	AUTHOR
 *  
 * <May 22 2005> 	<Nachum Kanovsky> 	
 *    
 * REMARKS: 
 *    
 *    
 ******************************************************************************/
#ifndef __COFFEXP_H__
#define __COFFEXP_H__

#include "MangoShared.h"
#include "MangoError.h"
#include "MangoBios.h"

#include <stdio.h>

#ifdef __cplusplus
extern "C" {
#endif

/* typedef for structure Coff_Write_s */
typedef struct Coff_Write_s Coff_Write_t;

/* structure used for coff-formatted file parsing */
struct Coff_Write_s
{
	/* \offset from beginning of file to perform write from */
	unsigned int src_offset;
	/* address on target to write to */
	int dest_adr;
	/* run address of this code (useful if code is to be relocated by a boot loader) */
	int run_adr;
	/* number of bytes to write */
	unsigned int bytes;
};

/* Summary
   Gets MANGOBIOS_version_t
   
   Description
   Gets version information for Coff Library
   
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :  Success
   Remarks
   None
   
   Example
   
   <CODE>int errorCode;
   MANGOBIOS_version_t version;
   
   errorCode = Coff_Get_Version(
    &version
    );
   </CODE>                                  */
MANGOERROR_error_t
Coff_Get_Version(
	MANGOBIOS_version_t * version
);

/* Summary
   Translates a coff-formatted file
   
   Description
   Translates a coff-formatted file into a series of
   Coff_Write_s structures
   
   Parameters
   fp :          Pointer to open file which is ready for parsing
   writes :      Pointer to first element of an array of Coff_Write_s
                 structures, or NULL (see Remarks)
   num_writes :  Pointer to the number of writes required for this file
   Returns
   Status
   
   Return Value List
   MANGOERROR_SUCCESS :                Success
   MANGOERROR_INVALID_PARAMETERS :     'fp' is NULL
   MANGOERROR_COFF_FORMAT_ERROR :      'fread', 'fseek', 'fsetpos', 'fgetpos',
                                       return an error<P>
                                       'optional_header_size' in coff\-formatted
                                       file's header is nonzero and not 28
   MANGOERROR_INVALID_CONFIGURATION :  The coff\-formatted file is not for the
                                       c6000 dsp<P>
                                       The coff\-formatted file is not marked as
                                       an executable
   Remarks
   To get the needed size to be allocated for the writes array,
   you should call Coff_file2writes once with a NULL writes
   pointer. It will then return the maximum possible number of
   writes for this coff file. On the next call, with an
   appropriately allocated writes pointer, it will fill the
   writes pointer, and return the actual number of writes filled
   in the array.
   
   Example
   
   <CODE>int num_writes, errorCode;
   Coff_Write_t * writes;
   if((errorCode = Coff_file2writes(
    fp,
    NULL,
    &num_writes
    )) != MANGOERROR_SUCCESS)
     return errorCode;
   writes = malloc(
    sizeof(Coff_Write_t) * num_writes
    );
   if((errorCode = Coff_file2writes(
    fp,
    writes,
    &num_writes
    )) != MANGOERROR_SUCCESS)
     return errorCode;
   //parsing and writing of writes array free(writes);
   </CODE>                                                                       */
MANGOERROR_error_t
Coff_file2writes(
	FILE * fp,
	Coff_Write_t * writes,
	unsigned int * num_writes
);

#ifdef __cplusplus
}
#endif /* extern "C" */

#endif /* __COFFEXP_H__ */
