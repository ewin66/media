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
   Error codes for all MangoBios based libraries and functions
   
   Remarks
   Bugs
   TODO
   History
   
   <TABLE>
   \Author           Change Description
   ----------------  -------------------
   Nachum Kanovsky   Created
   </TABLE>                                                    */

#ifndef __MANGOERROR_H__
#define __MANGOERROR_H__

/* enum of possible errors returned in MangoBios
   based projects                                           */
enum MANGOERROR_error_e
{
	/* success */
	MANGOERROR_SUCCESS = 0x20000000
	,
	/* failure */
	MANGOERROR_FAILURE
	,
	/* timeout */
	MANGOERROR_TIMEOUT
	,
	/* invalid handle */
	MANGOERROR_ERR_INVALID_HANDLE
	,
	/* not implemented */
	MANGOERROR_ERR_NOT_IMPLEMENTED
	,
	/* coff format error */
	MANGOERROR_COFF_FORMAT_ERROR
	,
	/* invalid parameter */
	MANGOERROR_ERR_INVALID_PARAMETER
	,
	/* insufficient resources */
	MANGOERROR_INSUFFICIENT_RESOURCES
	,
	/* invalid configuration */
	MANGOERROR_INVALID_CONFIGURATION
	,
	/* resource not ready */
	MANGOERROR_RESOURCE_NOT_READY
};

/* typedef of structure MANGOERROR_error_e */
typedef enum MANGOERROR_error_e MANGOERROR_error_t;

#endif //__MANGOERROR_H__
