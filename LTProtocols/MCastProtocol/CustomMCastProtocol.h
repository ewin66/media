	
// CustomMCastProtocol.h : Declaration of the CCustomMCastProtocol

#ifndef __CustomMCastPROTOCOL_H_
#define __CustomMCastPROTOCOL_H_

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CCustomMCastProtocol
class ATL_NO_VTABLE CCustomMCastProtocol : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CCustomMCastProtocol, &CLSID_CustomMCastProtocol>,
	public ILMNetProtocol
{
public:
	CCustomMCastProtocol()
	{
		m_pUnkMarshaler = NULL;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CUSTOMMCASTPROTOCOL)
DECLARE_GET_CONTROLLING_UNKNOWN()

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CCustomMCastProtocol)
	COM_INTERFACE_ENTRY(ILMNetProtocol)
	COM_INTERFACE_ENTRY_AGGREGATE(IID_IMarshal, m_pUnkMarshaler.p)
END_COM_MAP()

	HRESULT FinalConstruct()
	{
      return CoCreateFreeThreadedMarshaler(
			GetControllingUnknown(), &m_pUnkMarshaler.p);
	}

	void FinalRelease()
	{
		m_pUnkMarshaler.Release();
	}

	CComPtr<IUnknown> m_pUnkMarshaler;

// ILMNetProtocol
public:
	STDMETHOD(CreateConnectionPoint)(/*[in, string]*/ LPCWSTR pszURL,  /*[out]*/ ILMNetConnectionPoint** ppConnectionPoint);
	STDMETHOD(Connect)(/*[in, string]*/ LPCWSTR pszURL, /*[out]*/ ILMNetConnection** ppConnection, /*[in]*/ DWORD nTimeout);
};

#endif //__CustomMCastPROTOCOL_H_
