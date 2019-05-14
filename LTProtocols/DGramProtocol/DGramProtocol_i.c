

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


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

MIDL_DEFINE_GUID(IID, LIBID_DGramProtocolLib,0xBDA957AC,0x3D6C,0x4354,0x9B,0xBE,0xEB,0xD5,0x4C,0x3C,0xE5,0x6D);


MIDL_DEFINE_GUID(CLSID, CLSID_CustomDGramProtocol,0x42BE4D8C,0x9092,0x442c,0x8A,0xF2,0x88,0xA5,0xE3,0x13,0xEB,0xF6);


MIDL_DEFINE_GUID(CLSID, CLSID_CustomDGramConnection,0x075107A5,0x60F7,0x44b8,0xA7,0x5B,0x33,0xFA,0xB1,0x89,0xE8,0x1A);


MIDL_DEFINE_GUID(CLSID, CLSID_CustomDGramConnectionPoint,0xFAE71338,0x1470,0x4650,0x97,0xBF,0x60,0x7E,0xC1,0x04,0x01,0x46);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



