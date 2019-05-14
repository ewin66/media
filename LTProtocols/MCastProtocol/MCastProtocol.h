

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0555 */
/* at Wed Dec 12 11:30:56 2012
 */
/* Compiler settings for MCastProtocol.idl:
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


#ifndef __MCastProtocol_h__
#define __MCastProtocol_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __CustomMCastProtocol_FWD_DEFINED__
#define __CustomMCastProtocol_FWD_DEFINED__

#ifdef __cplusplus
typedef class CustomMCastProtocol CustomMCastProtocol;
#else
typedef struct CustomMCastProtocol CustomMCastProtocol;
#endif /* __cplusplus */

#endif 	/* __CustomMCastProtocol_FWD_DEFINED__ */


#ifndef __CustomMCastConnection_FWD_DEFINED__
#define __CustomMCastConnection_FWD_DEFINED__

#ifdef __cplusplus
typedef class CustomMCastConnection CustomMCastConnection;
#else
typedef struct CustomMCastConnection CustomMCastConnection;
#endif /* __cplusplus */

#endif 	/* __CustomMCastConnection_FWD_DEFINED__ */


#ifndef __CustomMCastConnectionPoint_FWD_DEFINED__
#define __CustomMCastConnectionPoint_FWD_DEFINED__

#ifdef __cplusplus
typedef class CustomMCastConnectionPoint CustomMCastConnectionPoint;
#else
typedef struct CustomMCastConnectionPoint CustomMCastConnectionPoint;
#endif /* __cplusplus */

#endif 	/* __CustomMCastConnectionPoint_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __MCastProtocolLib_LIBRARY_DEFINED__
#define __MCastProtocolLib_LIBRARY_DEFINED__

/* library MCastProtocolLib */
/* [helpstring][version][uuid] */ 

#ifndef __MCastProtocolLib__
#define __MCastProtocolLib__
#define SZ_MCASTPROTOCOL L"mcast"
#endif 

EXTERN_C const IID LIBID_MCastProtocolLib;

EXTERN_C const CLSID CLSID_CustomMCastProtocol;

#ifdef __cplusplus

class DECLSPEC_UUID("5112A464-C8F7-4734-B87E-275E6660703B")
CustomMCastProtocol;
#endif

EXTERN_C const CLSID CLSID_CustomMCastConnection;

#ifdef __cplusplus

class DECLSPEC_UUID("0ABC7A4B-3578-4CFF-86BC-360BC27BC143")
CustomMCastConnection;
#endif

EXTERN_C const CLSID CLSID_CustomMCastConnectionPoint;

#ifdef __cplusplus

class DECLSPEC_UUID("9E161729-B335-4F4C-935E-57B7BBACE02F")
CustomMCastConnectionPoint;
#endif
#endif /* __MCastProtocolLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


