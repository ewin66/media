

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0555 */
/* at Wed Jun 10 15:45:08 2015
 */
/* Compiler settings for DGramProtocol.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 7.00.0555 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__


#ifndef __DGramProtocol_i_h__
#define __DGramProtocol_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __CustomDGramProtocol_FWD_DEFINED__
#define __CustomDGramProtocol_FWD_DEFINED__

#ifdef __cplusplus
typedef class CustomDGramProtocol CustomDGramProtocol;
#else
typedef struct CustomDGramProtocol CustomDGramProtocol;
#endif /* __cplusplus */

#endif 	/* __CustomDGramProtocol_FWD_DEFINED__ */


#ifndef __CustomDGramConnection_FWD_DEFINED__
#define __CustomDGramConnection_FWD_DEFINED__

#ifdef __cplusplus
typedef class CustomDGramConnection CustomDGramConnection;
#else
typedef struct CustomDGramConnection CustomDGramConnection;
#endif /* __cplusplus */

#endif 	/* __CustomDGramConnection_FWD_DEFINED__ */


#ifndef __CustomDGramConnectionPoint_FWD_DEFINED__
#define __CustomDGramConnectionPoint_FWD_DEFINED__

#ifdef __cplusplus
typedef class CustomDGramConnectionPoint CustomDGramConnectionPoint;
#else
typedef struct CustomDGramConnectionPoint CustomDGramConnectionPoint;
#endif /* __cplusplus */

#endif 	/* __CustomDGramConnectionPoint_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __DGramProtocolLib_LIBRARY_DEFINED__
#define __DGramProtocolLib_LIBRARY_DEFINED__

/* library DGramProtocolLib */
/* [helpstring][version][uuid] */ 

#ifndef __DGramProtocolLib__
#define __DGramProtocolLib__
#define SZ_CUSTOMDGRAMPROTOCOL L"dgram"
#endif 

EXTERN_C const IID LIBID_DGramProtocolLib;

EXTERN_C const CLSID CLSID_CustomDGramProtocol;

#ifdef __cplusplus

class DECLSPEC_UUID("42BE4D8C-9092-442c-8AF2-88A5E313EBF6")
CustomDGramProtocol;
#endif

EXTERN_C const CLSID CLSID_CustomDGramConnection;

#ifdef __cplusplus

class DECLSPEC_UUID("075107A5-60F7-44b8-A75B-33FAB189E81A")
CustomDGramConnection;
#endif

EXTERN_C const CLSID CLSID_CustomDGramConnectionPoint;

#ifdef __cplusplus

class DECLSPEC_UUID("FAE71338-1470-4650-97BF-607EC1040146")
CustomDGramConnectionPoint;
#endif
#endif /* __DGramProtocolLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


