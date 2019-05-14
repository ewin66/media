
// CustomMCastConnectionPoint.h : Declaration of the CCustomConnectionPoint

#ifndef __CustomMcastCONNECTIONPOINT_H_
#define __CustomMCastCONNECTIONPOINT_H_

#include "resource.h"       // main symbols

class CCustomMCastConnection;

/////////////////////////////////////////////////////////////////////////////
// CCustomMCastConnectionPoint
class ATL_NO_VTABLE CCustomMCastConnectionPoint : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CCustomMCastConnectionPoint, &CLSID_CustomMCastConnectionPoint>,
	public ILMNetConnectionPoint
{
public:
	CCustomMCastConnectionPoint()
	{
		m_pUnkMarshaler = NULL;
	}

	DECLARE_REGISTRY_RESOURCEID(IDR_CUSTOMMCASTCONNECTIONPOINT)
	DECLARE_GET_CONTROLLING_UNKNOWN()

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	BEGIN_COM_MAP(CCustomMCastConnectionPoint)
		COM_INTERFACE_ENTRY(ILMNetConnectionPoint)
		COM_INTERFACE_ENTRY_AGGREGATE(IID_IMarshal, m_pUnkMarshaler.p)
	END_COM_MAP()

	HRESULT FinalConstruct()
	{
		m_pConnection = NULL;
		m_blocking = 0;
		m_socket = INVALID_SOCKET;
		m_hCloseEvent = NULL;
		return CoCreateFreeThreadedMarshaler(GetControllingUnknown(), &m_pUnkMarshaler.p);
	}

	void FinalRelease()
	{
		Close();
		m_pUnkMarshaler.Release();
	}

	CComPtr<IUnknown> m_pUnkMarshaler;

	HRESULT Listen(LPCWSTR pszURL);

	// ILMNetConnectionPoint
public:
	STDMETHOD(CancelBlockingCall)();
	STDMETHOD(Close)();
	STDMETHOD(GetConnection)(/*[out]*/ ILMNetConnection** ppConnection, /*[in]*/ DWORD nTimeout);

protected:
#if 0
	BOOL EventWait(long nEvents, DWORD nTimeout);
#endif

	long m_blocking;
	SOCKET m_socket;
	SOCKADDR_IN m_dest_sin;
	CCustomMCastConnection* m_pConnection;
	HANDLE m_hCloseEvent;

};

#endif //__CustomMCastCONNECTIONPOINT_H_
