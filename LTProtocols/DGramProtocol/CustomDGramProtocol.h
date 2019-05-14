	
// CustomProtocol.h : Declaration of the CCustomProtocol

#ifndef __CustomDGramPROTOCOL_H_
#define __CustomDGramPROTOCOL_H_

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CCustomProtocol
class ATL_NO_VTABLE CCustomDGramProtocol : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CCustomDGramProtocol, &CLSID_CustomDGramProtocol>,
	public ILMNetProtocol
{
public:
	CCustomDGramProtocol()
	{
		m_pUnkMarshaler = NULL;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CUSTOMDGRAMPROTOCOL)
DECLARE_GET_CONTROLLING_UNKNOWN()

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CCustomDGramProtocol)
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

#endif //__CustomDGramPROTOCOL_H_
