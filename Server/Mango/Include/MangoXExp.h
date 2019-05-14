/* Copyright 2005 Mango DSP, Inc. All rights reserved. Software
   and information contained herein is confidential and
   proprietary to Mango DSP, and any unauthorized copying,
   dissemination or use is strictly prohibited.
   
   Filename: MangoXExp.h
   
   General Description: Exported header for MangoX library
   
   Created: 21/2/2005
   
   \Author: Itay Chamiel
*/
#ifndef __MANGOX_H__
#define __MANGOX_H__

#include "MangoShared.h"
#include "MangoError.h"
#include "MangoBios.h"
#include "MangoC64BoardsExp.h"
#include "h2d_pci_streamSharedExp.h"
#include "h2d_pci_streamExp.h"
#include "MangoX_SharedExp.h"

/* constants */
#ifndef IN
#define IN
#endif
#ifndef OUT
#define OUT
#endif

/* typedefs */
typedef void MangoX_Attrs_T;

typedef void Card_handle_T;

/* Summary
   This is a prototype for a user callback function that is
   called whenever a DSP reports an error.
   
   Description
   A pointer to this function should be given in the call to
   MangoX_CardEnable. The function will receive information
   regarding the source of the error, its severity and a
   human-readable string indicating the nature of the problem.
   \Note that it is very likely that the DSP will not be able to
   gracefully recover from an error reported here, so it is best
   to find the cause of the problem and prevent it from ever
   happening in your production system.
   
   Parameters
   %PAR0% :  This is the value given by the user in MangoX_CardEnable, which
             you may use for any purpose.
   %PAR1% :  The card number (same as returned by MangoX_CardEnable).
   %PAR2% :  The DSP number that reported the error.
   %PAR3% :  Severity of the error, as reported by the DSP.
   %PAR3% :  Numerical error code as reported by the DSP.
   %PAR4% :  Text string containing human-readable text indicating the name
             of the class (filter) that reported the error and a description
			 of the problem.

   Remarks
       * The context from which this function is called is a separate
	     thread, created specifically for this purpose. There is one
		 such thread for each enabled DSP.
       * The DSP will report errors of three possible types:
   
         1. Warning - minor problems
         2. Error - The DSP could not complete a task and has
            probably not recovered gracefully, but other tasks can
            probably continue to operate. This is still considered a
            problem.
         3. Fatal - A problem has occurred that could only
            happen in case of a severe problem, such as corruption of a
            stack or heap. It is recommended to halt the program
            immediately in this case.                                   
*/
   
typedef void (*MANGOX_ERR_CB)(void * arg, int card_num, int dsp_num, Error_Severity_E err_type, int err_num, char * err_string);

/* functions */

/* Summary
   Opens access to the library.
   
   Parameters
   %PAR0% :  Reserved; use NULL.
   
   Returns
   MANGOERROR_SUCCESS - Library opened successfully.

   Remarks
   This function must be called
   before any other functions. (Undefined results may occur if
   this function if not called.)
*/
MANGOERROR_error_t
MangoX_Open(
	IN MangoX_Attrs_T * attrs // void for now
	);

/* Summary
   Grant library permission to use a card and some (or all) of
   its DSPs; initializes DSPs.
   
   Description
   This function receives a handle to a card (which had been
   initialized using MangoC64Boards_Open) and an array
   indicating which DSPs the library is allowed to use, as well
   as indicating what DSP executable (out) file to load on each.
   The function will load the DSPs and perform relevant
   initializations such as FPGA loading where applicable.
   
   Parameters
   %PAR0% :  Handle to the card to be used. May be of any valid Mango
             card type.
   %PAR1% :  Array indicating which DSPs may be used, and if so, which
             executable (out file) to load. The length of the array
             should be equal to the number of DSPs on the card, and
             each entry should contain a character string containing the path
             and file name to load, or NULL if the DSP is not to be accessed
             by the library at all.
   %PAR2% :  Array of FPGA filenames. The length of the array
             should be equal to the number of DSPs on the card, and
             each entry should contain a character string containing the path
             and file name of an RBF-format FPGA configuration data file.
			 If this DSP does not need to load an FPGA, or if the FPGA is not
			 necessary for your application, set the value of the appropriate
			 entry to NULL. Set the parameter itself to NULL if you do not need
			 to load any FPGA.
   %PAR3% :  An integer value will be placed in this pointer. This
             value must be used for all subsequent calls that pertain
             to this card.
   %PAR4% :  A pointer to a table of PCI stream device pointers, one
             per DSP. This pointer is filled by this function and
             should be given to any filter that needs to open PCI
             streams.
   %PAR5% :  Array of callback functions to be used in case of an
             error, one per DSP. See typedef MANGOX_ERR_CB for a description
			 of this function. This function can be the same or
			 different for each DSP. The function can be NULL if you
			 do not need this functionality for a specific DSP. Or, if
			 you do not need this functionality for any DSP, you may
			 set this parameter as NULL. A NULL value will cause no
			 callback to be called, in which case reported errors
			 will be ignored.
   %PAR6% :  Array of void* values, one per DSP, that will be given as
             the first parameter (arg) to the callback function. Use NULL
			 instead of an array pointer if you don't need this functionality;
			 in this case the value of arg shall always be NULL.
   
   Returns
   Since this function performs many tasks, there are several
   possible error conditions:
   
       * MANGOERROR_INVALID_CONFIGURATION - File not found (rbf
         \or out); Make sure FPGA (.rbf) and executable (.out) files
		 are in the locations given by the fpga_fnames and coff_fnames
         parameters.
       * MANGOERROR_ERR_INVALID_HANDLE - card handle is
         problematic or card not supported.
       * MANGOERROR_INSUFFICIENT_RESOURCES - can't allocate
         shared memory.
       * MANGOERROR_RESOURCE_NOT_READY - error loading fpga.
       * MANGOERROR_TIMEOUT - DSP did not respond to initial request
	     within a reasonable time frame.
       * MANGOERROR_FAILURE - other unexpected error.
       * MANGOERROR_SUCCESS - Card opened successfully.
   
   Remarks
   After calling this function, the user application must not
   access the DSPs that are granted to the library in any way
   \other than via MangoX calls. The DSPs marked as NULL are
   untouched, and may be freely used.                                   */
MANGOERROR_error_t
MangoX_CardEnable(
	IN  MangoC64Boards_handle_t * handle,
	IN	char** coff_fnames,
	IN	char** fpga_fnames,
	OUT int* card_num,
	OUT	h2d_pci_device_t *** pciDevArr,
	IN  MANGOX_ERR_CB * err_cbs,
	IN  void ** err_args
	);

/* Summary
   Frees all resources associated with a previous successful call to
   MangoX_CardEnable() and disables all enabled DSPs.

   Parameters
   %PAR0% :  the card number that was returned by MangoX_CardEnable.

   Returns
       * MANGOERROR_ERR_INVALID_PARAMETER - Invalid card number
       * MANGOERROR_FAILURE - Failure communicating with DSP or
         closing PCI streams.
	   * MANGOERROR_RESOURCE_NOT_READY - DSP reported failure
	     when attempting to communicate.
       * MANGOERROR_INSUFFICIENT_RESOURCES - Failure freeing
         shared memory.
       * MANGOERROR_SUCCESS - Success freeing resources.
   
   Remarks
       * Calling this function without first deleting all graphs
         will result in memory leaks. The library does not keep track
         \of open graphs and cannot close them for you.
       * After calling this function the card number is invalid.
       * All DSPs that were enabled for MangoX on this card are now reset
	     and free for use by your application. (You must load them if you
         wish to use them.)
*/
MANGOERROR_error_t
MangoX_CardDisable(
	IN int card_num
	);

/* Summary
   Closes access to the library and frees all associated
   resources.
   
   Returns
       * MANGOERROR_FAILURE - Failure communicating with DSP or
         closing PCI streams.
	   * MANGOERROR_RESOURCE_NOT_READY - DSP reported failure
	     when attempting to communicate.
       * MANGOERROR_INSUFFICIENT_RESOURCES - Failure freeing
         shared memory.
       * MANGOERROR_SUCCESS - Success freeing resources.
   
   Remarks
       * This function will call MangoX_CardDisable to disable all
	     cards that haven't yet been disabled. All remarks from that
		 function apply here as well.
       * After calling this function no MangoX function may be called
	     except MangoX_Open.
	   * It is good practise to always call this function before exiting
	     your application, even if aborting in case of an error. Depending
		 on your OS, exiting the program without calling this function
		 may cause an application crash.
*/
MANGOERROR_error_t
MangoX_Close();

/* Summary
   Configure video chips.
   
   Description
   This function initializes the video input/output chips.
   
   Parameters
   %PAR0% :  Card number, given by MangoX_CardEnable.
   %PAR1% :  Dsp number to use. This DSP must have been enabled in
             the coff_fnames parameter of MangoX_CardEnable.
   %PAR2% :  Indicate whether you wish to access a video-in or
             video-out chip.
   %PAR3% :  The video standard (PAL/NTSC) you wish to use with this
             chip.
   %PAR4% :  The ID of the chip you wish to access, or -1 to access
             all chips of the requested type.
   
   Returns
       * MANGOERROR_ERR_INVALID_PARAMETER - attempted to use DSP
         not enabled in MangoX_CardEnable.
       * MANGOERROR_RESOURCE_NOT_READY - DSP failed to configure chip
	     or chip is not accessible from this DSP.
       * MANGOERROR_FAILURE - Other fatal error.
       * MANGOERROR_SUCCESS - Chip configured successfully.

   Remarks
       * This function is only needed on the Seagull PMC, Lark and
	     Raven-C.
	   * See the MangoAV_HW User's Manual for details on
		 chip IDs.
*/
MANGOERROR_error_t
MangoX_ConfigVideo(
	IN  int card_num,
	IN  int dsp,
	IN	enConfigVideoCmds cmd,
	IN	enVideoStandard std,
	IN	int chip_id
	);

/* Summary
   Send user-definable command and parameter to DSP
   (regardless of whether there are any open graphs).

   Parameters
   %PAR0% :  Card number, given by MangoX_CardEnable.
   %PAR1% :  Dsp number to use. This DSP must have been enabled in
             the coff_fnames parameter of MangoX_CardEnable.
   %PAR2% :  Custom command.
   %PAR3% :  First parameter for command.
   %PAR4% :  Second parameter for command.
   %PAR5% :  Pointer for return value array; may be NULL if none is expected.
             (The size of this array must be NUM_CUSTOM_CMD_RETVALS.)
  
   Returns
       * MANGOERROR_ERR_INVALID_PARAMETER - attempted to use DSP
         not enabled in MangoX_CardEnable.
       * MANGOERROR_RESOURCE_NOT_READY - Command sent, but
	     custom function responded with failure.
       * MANGOERROR_FAILURE - Other fatal error.
       * MANGOERROR_SUCCESS - Command sent and acknowledged.

   Remarks
   User must implement this command on the DSP side. */
MANGOERROR_error_t
MangoX_DspCustomCommand(
	IN  int card_num,
	IN  int dsp,
	IN  unsigned int cst_cmd,
	IN  unsigned int cst_param0,
	IN  unsigned int cst_param1,
	OUT int* ret_val
	);

/* Summary
   Opens a stream on a dsp.
   
   Description
   This function passes a new graph to the DSP.
   
   Parameters
   %PAR0% :  Pointer to graph number, which will be filled in by the
             library.
   %PAR1% :  Card number, given by MangoX_CardEnable.
   %PAR2% :  Dsp number to use. This DSP must have been enabled in
             the coff_fnames parameter of MangoX_CardEnable. Make
             sure that the DSP you are trying to use has the
             capability you are requesting; for example, on the
             Seagull PMC, only DSP 0 has the ability to output an
             image to a video monitor.
   %PAR3% :  A fully configured and connected CMangoXGraph object.
   
   Returns
       * MANGOERROR_ERR_INVALID_PARAMETER - attempted to use DSP
         not enabled in MangoX_CardEnable.
       * MANGOERROR_RESOURCE_NOT_READY - DSP failed to create
         graph.
       * MANGOERROR_FAILURE - Fatal error, or graph serialize
         buffer is too large.
       * MANGOERROR_SUCCESS - Graph submitted successully.           */
MANGOERROR_error_t
MangoX_SubmitGraph(
	OUT int* graph_num,
	IN  int card_num,
	IN  int dsp,
	IN  CMangoXGraph* pGraph
	);

/* Summary
   Closes a graph on a DSP.
   
   Description
   This call tells the DSP to closes a graph and free all
   associated resources, including all filters and tasks. The
   given graph number is invalidated by this function. Note that
   this call does not free the graph object on the host; the
   user must delete the graph object in the application after
   calling this function. (Deleting the graph object will delete
   all associated filters and resources on the host side.)
   
   Parameters
   %PAR0% :  Graph number given by MangoX_SubmitGraph.
   %PAR1% :  Card number given by MangoX_CardEnable.
   %PAR2% :  DSP number on card.
   
   Returns
       * MANGOERROR_ERR_INVALID_PARAMETER - attempted to use DSP
         not enabled in MangoX_CardEnable.
       * MANGOERROR_FAILURE - Fatal error.
       * MANGOERROR_RESOURCE_NOT_READY - DSP failed to delete
         the stream. Perhaps the graph number is incorrect or was not
         \opened on this DSP?
       * MANGOERROR_SUCCESS - stream successfully deleted.            */
MANGOERROR_error_t
MangoX_DeleteGraph(
	IN	int graph_num,
	IN  int card_num,
	IN  int dsp
	);

/* Summary
   \Returns lib version.
   
   Parameters
   %PAR0% :  Pointer for returned version number.
   
   Returns
       * MANGOERROR_ERR_INVALID_PARAMETER - if null pointer was
         received
       * MANGOERROR_SUCCESS - successful.
   
   Remarks
   \Version is returned as an unsigned int containing major and
   minor version numbers in this format: ((major \<\< 16) |
   minor)                                                       */
MANGOERROR_error_t
MangoX_GetVersion(
	OUT MANGOBIOS_version_t * version
	);

/* Summary
   Receive version information string from DSP.
   
   Parameters
   %PAR0% :  Card number given by MangoX_CardEnable.
   %PAR1% :  DSP number.
   %PAR2% :  Empty pointer, will point to string after this
             function call.
   
   Returns
       * MANGOERROR_ERR_INVALID_PARAMETER - attempted to use DSP
         not enabled in MangoX_CardEnable.
       * MANGOERROR_FAILURE - Fatal error.
	   * MANGOERROR_RESOURCE_NOT_READY - DSP reported failure.
       * MANGOERROR_SUCCESS - successful.
*/
MANGOERROR_error_t
MangoX_GetVersionString(
	IN  int card_num,
	IN  int dsp,
	OUT char** ver_string
	);


#endif /* __MANGOX_H__ */
