	
// CustomConnection.h : Declaration of the CCustomConnection

#ifndef __CustomCONNECTION_H_
#define __CustomCONNECTION_H_

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CCustomConnection
class ATL_NO_VTABLE CCustomConnection : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CCustomConnection, &CLSID_CustomConnection>,
	public ILMNetConnection
{
public:
	CCustomConnection()
	{
		m_pUnkMarshaler = NULL;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CUSTOMCONNECTION)
DECLARE_GET_CONTROLLING_UNKNOWN()

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CCustomConnection)
	COM_INTERFACE_ENTRY(ILMNetConnection)
	COM_INTERFACE_ENTRY_AGGREGATE(IID_IMarshal, m_pUnkMarshaler.p)
END_COM_MAP()

	HRESULT FinalConstruct()
	{
      m_blocking = 0;
      m_socket = INVALID_SOCKET;
      m_evsocket[0] = WSA_INVALID_EVENT;
      m_evsocket[1] = WSA_INVALID_EVENT;
      memset(&m_netevents, 0, sizeof(m_netevents));
		return CoCreateFreeThreadedMarshaler(
			GetControllingUnknown(), &m_pUnkMarshaler.p);
	}

	void FinalRelease()
	{
      Disconnect();
      m_pUnkMarshaler.Release();
	}

	CComPtr<IUnknown> m_pUnkMarshaler;

// ILMNetConnection
public:
	STDMETHOD(CancelBlockingCall)();
	STDMETHOD(GetPeerName)(/*[out, string]*/ LPWSTR* ppszPeer);
	STDMETHOD(IsConnected)();
	STDMETHOD(Disconnect)();
	STDMETHOD(Recv)(/*[in, size_is(cbBuffer)]*/ BYTE* pbBuffer,  /*[in]*/ DWORD cbBuffer,  /*[out]*/ DWORD* pcbReceived, /*[in]*/ DWORD nTimeout);
	STDMETHOD(Send)(/*[in, size_is(cbBuffer)]*/ BYTE* pbBuffer,  /*[in]*/ DWORD cbBuffer,  /*[out]*/ DWORD* pcbSent, /*[in]*/ DWORD nTimeout);

   HRESULT Attach(SOCKET socket);
   HRESULT Connect(LPCWSTR pszURL, DWORD nTimeout);

protected:
   BOOL EventWait(long nEvents, DWORD nTimeout);
   int SendChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout);
   int RecvChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout);
   
   long m_blocking;
   SOCKET m_socket;
   WSAEVENT m_evsocket[2];
   WSANETWORKEVENTS m_netevents;

 };

#endif //__CustomCONNECTION_H_
