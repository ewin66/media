	
// CustomConnectionPoint.h : Declaration of the CCustomConnectionPoint

#ifndef __CustomCONNECTIONPOINT_H_
#define __CustomCONNECTIONPOINT_H_

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CCustomConnectionPoint
class ATL_NO_VTABLE CCustomConnectionPoint : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CCustomConnectionPoint, &CLSID_CustomConnectionPoint>,
	public ILMNetConnectionPoint
{
public:
	CCustomConnectionPoint()
	{
		m_pUnkMarshaler = NULL;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CUSTOMCONNECTIONPOINT)
DECLARE_GET_CONTROLLING_UNKNOWN()

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CCustomConnectionPoint)
	COM_INTERFACE_ENTRY(ILMNetConnectionPoint)
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
   BOOL EventWait(long nEvents, DWORD nTimeout);

   long m_blocking;
   SOCKET m_socket;
   WSAEVENT m_evsocket[2];
   WSANETWORKEVENTS m_netevents;

};

#endif //__CustomCONNECTIONPOINT_H_
