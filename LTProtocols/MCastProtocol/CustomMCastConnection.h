
// CustomMCastConnection.h : Declaration of the CCustomConnection

#ifndef __CustomMCastCONNECTION_H_
#define __CustomMCastCONNECTION_H_

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CCustomMCastConnection
class ATL_NO_VTABLE CCustomMCastConnection : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CCustomMCastConnection, &CLSID_CustomMCastConnection>,
	public ILMNetConnection
{
public:
	CCustomMCastConnection()
	{
		m_pUnkMarshaler = NULL;
	}

	DECLARE_REGISTRY_RESOURCEID(IDR_CUSTOMMCASTCONNECTION)
	DECLARE_GET_CONTROLLING_UNKNOWN()

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	BEGIN_COM_MAP(CCustomMCastConnection)
		COM_INTERFACE_ENTRY(ILMNetConnection)
		COM_INTERFACE_ENTRY_AGGREGATE(IID_IMarshal, m_pUnkMarshaler.p)
	END_COM_MAP()

	HRESULT FinalConstruct()
	{
		m_nRecvBlocks = 0;
		m_bytesInBuffer = 0;
		m_bufferPosition = 0;
		m_blocking = 0;
		m_socket = INVALID_SOCKET;
		return CoCreateFreeThreadedMarshaler(GetControllingUnknown(), &m_pUnkMarshaler.p);
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

	HRESULT Attach(SOCKET socket, SOCKADDR_IN dest_sin);
	HRESULT Connect(LPCWSTR pszURL, DWORD nTimeout);

protected:
	BOOL EventWait(long nEvents, DWORD nTimeout);
	int SendChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout);
	int RecvChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout);

	long m_blocking;
	SOCKET m_socket;
	WSAEVENT m_evsocket[2];
	WSANETWORKEVENTS m_netevents;

	SOCKADDR_IN m_dest_sin;

	char m_buffer[16 * 1024];
	int m_bytesInBuffer;
	int m_bufferPosition;

	int m_nRecvBlocks;

};

#endif //__CustomMCastCONNECTION_H_
