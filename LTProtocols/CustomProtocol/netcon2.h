/* this ALWAYS GENERATED file contains the definitions for the interfaces */


/* File created by MIDL compiler version 5.01.0164 */
/* at Mon Dec 11 09:48:33 2006
 */
/* Compiler settings for C:\mm\DShowFlt\Network\netcon\netcon.idl:
    Oicf (OptLev=i2), W1, Zp8, env=Win32, ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
*/
//@@MIDL_FILE_HEADING(  )


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __netcon2_h__
#define __netcon2_h__

#ifdef __cplusplus
extern "C"{
#endif 

/* Forward Declarations */ 

#ifndef __ILMNetConnection_FWD_DEFINED__
#define __ILMNetConnection_FWD_DEFINED__
typedef interface ILMNetConnection ILMNetConnection;
#endif 	/* __ILMNetConnection_FWD_DEFINED__ */


#ifndef __ILMNetConnectionPoint_FWD_DEFINED__
#define __ILMNetConnectionPoint_FWD_DEFINED__
typedef interface ILMNetConnectionPoint ILMNetConnectionPoint;
#endif 	/* __ILMNetConnectionPoint_FWD_DEFINED__ */


#ifndef __ILMNetProtocol_FWD_DEFINED__
#define __ILMNetProtocol_FWD_DEFINED__
typedef interface ILMNetProtocol ILMNetProtocol;
#endif 	/* __ILMNetProtocol_FWD_DEFINED__ */


#ifndef __ILMNetProtocolManager_FWD_DEFINED__
#define __ILMNetProtocolManager_FWD_DEFINED__
typedef interface ILMNetProtocolManager ILMNetProtocolManager;
#endif 	/* __ILMNetProtocolManager_FWD_DEFINED__ */


#ifndef __LMTcpipProtocol_FWD_DEFINED__
#define __LMTcpipProtocol_FWD_DEFINED__

#ifdef __cplusplus
typedef class LMTcpipProtocol LMTcpipProtocol;
#else
typedef struct LMTcpipProtocol LMTcpipProtocol;
#endif /* __cplusplus */

#endif 	/* __LMTcpipProtocol_FWD_DEFINED__ */


#ifndef __LMTcpipConnection_FWD_DEFINED__
#define __LMTcpipConnection_FWD_DEFINED__

#ifdef __cplusplus
typedef class LMTcpipConnection LMTcpipConnection;
#else
typedef struct LMTcpipConnection LMTcpipConnection;
#endif /* __cplusplus */

#endif 	/* __LMTcpipConnection_FWD_DEFINED__ */


#ifndef __LMTcpipConnectionPoint_FWD_DEFINED__
#define __LMTcpipConnectionPoint_FWD_DEFINED__

#ifdef __cplusplus
typedef class LMTcpipConnectionPoint LMTcpipConnectionPoint;
#else
typedef struct LMTcpipConnectionPoint LMTcpipConnectionPoint;
#endif /* __cplusplus */

#endif 	/* __LMTcpipConnectionPoint_FWD_DEFINED__ */


#ifndef __LMNetProtocolManager_FWD_DEFINED__
#define __LMNetProtocolManager_FWD_DEFINED__

#ifdef __cplusplus
typedef class LMNetProtocolManager LMNetProtocolManager;
#else
typedef struct LMNetProtocolManager LMNetProtocolManager;
#endif /* __cplusplus */

#endif 	/* __LMNetProtocolManager_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

void __RPC_FAR * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void __RPC_FAR * ); 


#ifndef __NETCONLib_LIBRARY_DEFINED__
#define __NETCONLib_LIBRARY_DEFINED__

/* library NETCONLib */
/* [helpstring][version][uuid] */ 

#ifndef __LMNetCon2_GUID__
#define __LMNetCon2_GUID__
const IID IID_ILMNetConnection =           {0xe2b7de08,0x38c5,0x11d5,{0x91,0xf6,0x00,0x10,0x4b,0xdb,0x8f,0xf9}};
const IID IID_ILMNetConnectionPoint =      {0xe2b7de09,0x38c5,0x11d5,{0x91,0xf6,0x00,0x10,0x4b,0xdb,0x8f,0xf9}};
const IID IID_ILMNetProtocol =             {0xe2b7de0a,0x38c5,0x11d5,{0x91,0xf6,0x00,0x10,0x4b,0xdb,0x8f,0xf9}};
const CLSID CLSID_LMTcpipProtocol =        {0xe2b7de0b,0x38c5,0x11d5,{0x91,0xf6,0x00,0x10,0x4b,0xdb,0x8f,0xf9}};
const CLSID CLSID_LMNetProtocolManager =   {0xe2b7de0c,0x38c5,0x11d5,{0x91,0xf6,0x00,0x10,0x4b,0xdb,0x8f,0xf9}};
const IID IID_ILMNetProtocolManager =      {0xe2b7de0d,0x38c5,0x11d5,{0x91,0xf6,0x00,0x10,0x4b,0xdb,0x8f,0xf9}};
#endif 

EXTERN_C const IID LIBID_NETCONLib;

#ifndef __ILMNetConnection_INTERFACE_DEFINED__
#define __ILMNetConnection_INTERFACE_DEFINED__

/* interface ILMNetConnection */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ILMNetConnection;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E2B7DE08-38C5-11D5-91F6-00104BDB8FF9")
    ILMNetConnection : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE Send( 
            /* [size_is][in] */ BYTE __RPC_FAR *pbBuffer,
            /* [in] */ DWORD cbBuffer,
            /* [out] */ DWORD __RPC_FAR *pcbSent,
            /* [in] */ DWORD nTimeout) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE Recv( 
            /* [size_is][in] */ BYTE __RPC_FAR *pbBuffer,
            /* [in] */ DWORD cbBuffer,
            /* [out] */ DWORD __RPC_FAR *pcbReceived,
            /* [in] */ DWORD nTimeout) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE Disconnect( void) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE IsConnected( void) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE GetPeerName( 
            /* [string][out] */ LPWSTR __RPC_FAR *ppszPeer) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE CancelBlockingCall( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILMNetConnectionVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *QueryInterface )( 
            ILMNetConnection __RPC_FAR * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void __RPC_FAR *__RPC_FAR *ppvObject);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *AddRef )( 
            ILMNetConnection __RPC_FAR * This);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *Release )( 
            ILMNetConnection __RPC_FAR * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *Send )( 
            ILMNetConnection __RPC_FAR * This,
            /* [size_is][in] */ BYTE __RPC_FAR *pbBuffer,
            /* [in] */ DWORD cbBuffer,
            /* [out] */ DWORD __RPC_FAR *pcbSent,
            /* [in] */ DWORD nTimeout);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *Recv )( 
            ILMNetConnection __RPC_FAR * This,
            /* [size_is][in] */ BYTE __RPC_FAR *pbBuffer,
            /* [in] */ DWORD cbBuffer,
            /* [out] */ DWORD __RPC_FAR *pcbReceived,
            /* [in] */ DWORD nTimeout);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *Disconnect )( 
            ILMNetConnection __RPC_FAR * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *IsConnected )( 
            ILMNetConnection __RPC_FAR * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *GetPeerName )( 
            ILMNetConnection __RPC_FAR * This,
            /* [string][out] */ LPWSTR __RPC_FAR *ppszPeer);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *CancelBlockingCall )( 
            ILMNetConnection __RPC_FAR * This);
        
        END_INTERFACE
    } ILMNetConnectionVtbl;

    interface ILMNetConnection
    {
        CONST_VTBL struct ILMNetConnectionVtbl __RPC_FAR *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILMNetConnection_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ILMNetConnection_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ILMNetConnection_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ILMNetConnection_Send(This,pbBuffer,cbBuffer,pcbSent,nTimeout)	\
    (This)->lpVtbl -> Send(This,pbBuffer,cbBuffer,pcbSent,nTimeout)

#define ILMNetConnection_Recv(This,pbBuffer,cbBuffer,pcbReceived,nTimeout)	\
    (This)->lpVtbl -> Recv(This,pbBuffer,cbBuffer,pcbReceived,nTimeout)

#define ILMNetConnection_Disconnect(This)	\
    (This)->lpVtbl -> Disconnect(This)

#define ILMNetConnection_IsConnected(This)	\
    (This)->lpVtbl -> IsConnected(This)

#define ILMNetConnection_GetPeerName(This,ppszPeer)	\
    (This)->lpVtbl -> GetPeerName(This,ppszPeer)

#define ILMNetConnection_CancelBlockingCall(This)	\
    (This)->lpVtbl -> CancelBlockingCall(This)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnection_Send_Proxy( 
    ILMNetConnection __RPC_FAR * This,
    /* [size_is][in] */ BYTE __RPC_FAR *pbBuffer,
    /* [in] */ DWORD cbBuffer,
    /* [out] */ DWORD __RPC_FAR *pcbSent,
    /* [in] */ DWORD nTimeout);


void __RPC_STUB ILMNetConnection_Send_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnection_Recv_Proxy( 
    ILMNetConnection __RPC_FAR * This,
    /* [size_is][in] */ BYTE __RPC_FAR *pbBuffer,
    /* [in] */ DWORD cbBuffer,
    /* [out] */ DWORD __RPC_FAR *pcbReceived,
    /* [in] */ DWORD nTimeout);


void __RPC_STUB ILMNetConnection_Recv_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnection_Disconnect_Proxy( 
    ILMNetConnection __RPC_FAR * This);


void __RPC_STUB ILMNetConnection_Disconnect_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnection_IsConnected_Proxy( 
    ILMNetConnection __RPC_FAR * This);


void __RPC_STUB ILMNetConnection_IsConnected_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnection_GetPeerName_Proxy( 
    ILMNetConnection __RPC_FAR * This,
    /* [string][out] */ LPWSTR __RPC_FAR *ppszPeer);


void __RPC_STUB ILMNetConnection_GetPeerName_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnection_CancelBlockingCall_Proxy( 
    ILMNetConnection __RPC_FAR * This);


void __RPC_STUB ILMNetConnection_CancelBlockingCall_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ILMNetConnection_INTERFACE_DEFINED__ */


#ifndef __ILMNetConnectionPoint_INTERFACE_DEFINED__
#define __ILMNetConnectionPoint_INTERFACE_DEFINED__

/* interface ILMNetConnectionPoint */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ILMNetConnectionPoint;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E2B7DE09-38C5-11D5-91F6-00104BDB8FF9")
    ILMNetConnectionPoint : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE GetConnection( 
            /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
            /* [in] */ DWORD nTimeout) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE Close( void) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE CancelBlockingCall( void) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILMNetConnectionPointVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *QueryInterface )( 
            ILMNetConnectionPoint __RPC_FAR * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void __RPC_FAR *__RPC_FAR *ppvObject);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *AddRef )( 
            ILMNetConnectionPoint __RPC_FAR * This);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *Release )( 
            ILMNetConnectionPoint __RPC_FAR * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *GetConnection )( 
            ILMNetConnectionPoint __RPC_FAR * This,
            /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
            /* [in] */ DWORD nTimeout);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *Close )( 
            ILMNetConnectionPoint __RPC_FAR * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *CancelBlockingCall )( 
            ILMNetConnectionPoint __RPC_FAR * This);
        
        END_INTERFACE
    } ILMNetConnectionPointVtbl;

    interface ILMNetConnectionPoint
    {
        CONST_VTBL struct ILMNetConnectionPointVtbl __RPC_FAR *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILMNetConnectionPoint_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ILMNetConnectionPoint_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ILMNetConnectionPoint_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ILMNetConnectionPoint_GetConnection(This,ppConnection,nTimeout)	\
    (This)->lpVtbl -> GetConnection(This,ppConnection,nTimeout)

#define ILMNetConnectionPoint_Close(This)	\
    (This)->lpVtbl -> Close(This)

#define ILMNetConnectionPoint_CancelBlockingCall(This)	\
    (This)->lpVtbl -> CancelBlockingCall(This)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnectionPoint_GetConnection_Proxy( 
    ILMNetConnectionPoint __RPC_FAR * This,
    /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
    /* [in] */ DWORD nTimeout);


void __RPC_STUB ILMNetConnectionPoint_GetConnection_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnectionPoint_Close_Proxy( 
    ILMNetConnectionPoint __RPC_FAR * This);


void __RPC_STUB ILMNetConnectionPoint_Close_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetConnectionPoint_CancelBlockingCall_Proxy( 
    ILMNetConnectionPoint __RPC_FAR * This);


void __RPC_STUB ILMNetConnectionPoint_CancelBlockingCall_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ILMNetConnectionPoint_INTERFACE_DEFINED__ */


#ifndef __ILMNetProtocol_INTERFACE_DEFINED__
#define __ILMNetProtocol_INTERFACE_DEFINED__

/* interface ILMNetProtocol */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ILMNetProtocol;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E2B7DE0A-38C5-11D5-91F6-00104BDB8FF9")
    ILMNetProtocol : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE Connect( 
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
            /* [in] */ DWORD nTimeout) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE CreateConnectionPoint( 
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnectionPoint __RPC_FAR *__RPC_FAR *ppConnectionPoint) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILMNetProtocolVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *QueryInterface )( 
            ILMNetProtocol __RPC_FAR * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void __RPC_FAR *__RPC_FAR *ppvObject);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *AddRef )( 
            ILMNetProtocol __RPC_FAR * This);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *Release )( 
            ILMNetProtocol __RPC_FAR * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *Connect )( 
            ILMNetProtocol __RPC_FAR * This,
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
            /* [in] */ DWORD nTimeout);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *CreateConnectionPoint )( 
            ILMNetProtocol __RPC_FAR * This,
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnectionPoint __RPC_FAR *__RPC_FAR *ppConnectionPoint);
        
        END_INTERFACE
    } ILMNetProtocolVtbl;

    interface ILMNetProtocol
    {
        CONST_VTBL struct ILMNetProtocolVtbl __RPC_FAR *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILMNetProtocol_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ILMNetProtocol_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ILMNetProtocol_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ILMNetProtocol_Connect(This,pszURL,ppConnection,nTimeout)	\
    (This)->lpVtbl -> Connect(This,pszURL,ppConnection,nTimeout)

#define ILMNetProtocol_CreateConnectionPoint(This,pszURL,ppConnectionPoint)	\
    (This)->lpVtbl -> CreateConnectionPoint(This,pszURL,ppConnectionPoint)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetProtocol_Connect_Proxy( 
    ILMNetProtocol __RPC_FAR * This,
    /* [string][in] */ LPCWSTR pszURL,
    /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
    /* [in] */ DWORD nTimeout);


void __RPC_STUB ILMNetProtocol_Connect_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetProtocol_CreateConnectionPoint_Proxy( 
    ILMNetProtocol __RPC_FAR * This,
    /* [string][in] */ LPCWSTR pszURL,
    /* [out] */ ILMNetConnectionPoint __RPC_FAR *__RPC_FAR *ppConnectionPoint);


void __RPC_STUB ILMNetProtocol_CreateConnectionPoint_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ILMNetProtocol_INTERFACE_DEFINED__ */


#ifndef __ILMNetProtocolManager_INTERFACE_DEFINED__
#define __ILMNetProtocolManager_INTERFACE_DEFINED__

/* interface ILMNetProtocolManager */
/* [unique][helpstring][uuid][object] */ 


EXTERN_C const IID IID_ILMNetProtocolManager;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E2B7DE0D-38C5-11D5-91F6-00104BDB8FF9")
    ILMNetProtocolManager : public IUnknown
    {
    public:
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE RegisterProtocol( 
            /* [string][in] */ LPCWSTR pszName,
            /* [in] */ REFCLSID rclsid) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE UnregisterProtocol( 
            /* [string][in] */ LPCWSTR pszName) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE Connect( 
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
            /* [in] */ DWORD nTimeout) = 0;
        
        virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE CreateConnectionPoint( 
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnectionPoint __RPC_FAR *__RPC_FAR *ppConnectionPoint) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ILMNetProtocolManagerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE __RPC_FAR *QueryInterface )( 
            ILMNetProtocolManager __RPC_FAR * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void __RPC_FAR *__RPC_FAR *ppvObject);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *AddRef )( 
            ILMNetProtocolManager __RPC_FAR * This);
        
        ULONG ( STDMETHODCALLTYPE __RPC_FAR *Release )( 
            ILMNetProtocolManager __RPC_FAR * This);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *RegisterProtocol )( 
            ILMNetProtocolManager __RPC_FAR * This,
            /* [string][in] */ LPCWSTR pszName,
            /* [in] */ REFCLSID rclsid);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *UnregisterProtocol )( 
            ILMNetProtocolManager __RPC_FAR * This,
            /* [string][in] */ LPCWSTR pszName);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *Connect )( 
            ILMNetProtocolManager __RPC_FAR * This,
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
            /* [in] */ DWORD nTimeout);
        
        /* [helpstring] */ HRESULT ( STDMETHODCALLTYPE __RPC_FAR *CreateConnectionPoint )( 
            ILMNetProtocolManager __RPC_FAR * This,
            /* [string][in] */ LPCWSTR pszURL,
            /* [out] */ ILMNetConnectionPoint __RPC_FAR *__RPC_FAR *ppConnectionPoint);
        
        END_INTERFACE
    } ILMNetProtocolManagerVtbl;

    interface ILMNetProtocolManager
    {
        CONST_VTBL struct ILMNetProtocolManagerVtbl __RPC_FAR *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ILMNetProtocolManager_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ILMNetProtocolManager_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ILMNetProtocolManager_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ILMNetProtocolManager_RegisterProtocol(This,pszName,rclsid)	\
    (This)->lpVtbl -> RegisterProtocol(This,pszName,rclsid)

#define ILMNetProtocolManager_UnregisterProtocol(This,pszName)	\
    (This)->lpVtbl -> UnregisterProtocol(This,pszName)

#define ILMNetProtocolManager_Connect(This,pszURL,ppConnection,nTimeout)	\
    (This)->lpVtbl -> Connect(This,pszURL,ppConnection,nTimeout)

#define ILMNetProtocolManager_CreateConnectionPoint(This,pszURL,ppConnectionPoint)	\
    (This)->lpVtbl -> CreateConnectionPoint(This,pszURL,ppConnectionPoint)

#endif /* COBJMACROS */


#endif 	/* C style interface */



/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetProtocolManager_RegisterProtocol_Proxy( 
    ILMNetProtocolManager __RPC_FAR * This,
    /* [string][in] */ LPCWSTR pszName,
    /* [in] */ REFCLSID rclsid);


void __RPC_STUB ILMNetProtocolManager_RegisterProtocol_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetProtocolManager_UnregisterProtocol_Proxy( 
    ILMNetProtocolManager __RPC_FAR * This,
    /* [string][in] */ LPCWSTR pszName);


void __RPC_STUB ILMNetProtocolManager_UnregisterProtocol_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetProtocolManager_Connect_Proxy( 
    ILMNetProtocolManager __RPC_FAR * This,
    /* [string][in] */ LPCWSTR pszURL,
    /* [out] */ ILMNetConnection __RPC_FAR *__RPC_FAR *ppConnection,
    /* [in] */ DWORD nTimeout);


void __RPC_STUB ILMNetProtocolManager_Connect_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


/* [helpstring] */ HRESULT STDMETHODCALLTYPE ILMNetProtocolManager_CreateConnectionPoint_Proxy( 
    ILMNetProtocolManager __RPC_FAR * This,
    /* [string][in] */ LPCWSTR pszURL,
    /* [out] */ ILMNetConnectionPoint __RPC_FAR *__RPC_FAR *ppConnectionPoint);


void __RPC_STUB ILMNetProtocolManager_CreateConnectionPoint_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ILMNetProtocolManager_INTERFACE_DEFINED__ */


EXTERN_C const CLSID CLSID_LMTcpipProtocol;

#ifdef __cplusplus

class DECLSPEC_UUID("E2B7DE0B-38C5-11D5-91F6-00104BDB8FF9")
LMTcpipProtocol;
#endif

EXTERN_C const CLSID CLSID_LMTcpipConnection;

#ifdef __cplusplus

class DECLSPEC_UUID("0A8FAF9C-2FAA-4DAC-90FD-1E4B56A697F1")
LMTcpipConnection;
#endif

EXTERN_C const CLSID CLSID_LMTcpipConnectionPoint;

#ifdef __cplusplus

class DECLSPEC_UUID("798C7C2C-5123-4F58-902F-182DE7572506")
LMTcpipConnectionPoint;
#endif

EXTERN_C const CLSID CLSID_LMNetProtocolManager;

#ifdef __cplusplus

class DECLSPEC_UUID("E2B7DE0C-38C5-11D5-91F6-00104BDB8FF9")
LMNetProtocolManager;
#endif
#endif /* __NETCONLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif
