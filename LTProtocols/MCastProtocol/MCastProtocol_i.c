

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


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


#ifdef __cplusplus
extern "C"{
#endif 


#include <rpc.h>
#include <rpcndr.h>

#ifdef _MIDL_USE_GUIDDEF_

#ifndef INITGUID
#define INITGUID
#include <guiddef.h>
#undef INITGUID
#else
#include <guiddef.h>
#endif

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        DEFINE_GUID(name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)

#else // !_MIDL_USE_GUIDDEF_

#ifndef __IID_DEFINED__
#define __IID_DEFINED__

typedef struct _IID
{
    unsigned long x;
    unsigned short s1;
    unsigned short s2;
    unsigned char  c[8];
} IID;

#endif // __IID_DEFINED__

#ifndef CLSID_DEFINED
#define CLSID_DEFINED
typedef IID CLSID;
#endif // CLSID_DEFINED

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

#endif !_MIDL_USE_GUIDDEF_

MIDL_DEFINE_GUID(IID, LIBID_MCastProtocolLib,0x86196AAE,0x0058,0x4E66,0x97,0xAC,0xED,0xCB,0x67,0xF8,0x80,0x3B);


MIDL_DEFINE_GUID(CLSID, CLSID_CustomMCastProtocol,0x5112A464,0xC8F7,0x4734,0xB8,0x7E,0x27,0x5E,0x66,0x60,0x70,0x3B);


MIDL_DEFINE_GUID(CLSID, CLSID_CustomMCastConnection,0x0ABC7A4B,0x3578,0x4CFF,0x86,0xBC,0x36,0x0B,0xC2,0x7B,0xC1,0x43);


MIDL_DEFINE_GUID(CLSID, CLSID_CustomMCastConnectionPoint,0x9E161729,0xB335,0x4F4C,0x93,0x5E,0x57,0xB7,0xBB,0xAC,0xE0,0x2F);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



