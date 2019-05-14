// CustomDGramProtocol.cpp : Implementation of CCustomDGramProtocol
#include "stdafx.h"
#include "Netcon2.h"
#include "DGramProtocol.h"
#include "CustomDGramConnectionPoint.h"
#include "CustomDGramConnection.h"
#include "CustomDGramProtocol.h"

/////////////////////////////////////////////////////////////////////////////
// CCustomDGramProtocol

STDMETHODIMP CCustomDGramProtocol::Connect(LPCWSTR pszURL, ILMNetConnection** ppConnection, DWORD nTimeout)
{
   if(!ppConnection || !pszURL)
      return E_POINTER;
   *ppConnection = NULL;
   CCustomDGramConnection* pConnection = new CComObject<CCustomDGramConnection>();
   if(!pConnection)
      return E_OUTOFMEMORY;
   pConnection->SetVoid(NULL);
   pConnection->InternalFinalConstructAddRef();
	HRESULT hr = pConnection->FinalConstruct();
	pConnection->InternalFinalConstructRelease();
   if(FAILED(hr))
   {
      delete pConnection;
      return hr;
   }
   pConnection->AddRef();
   hr = pConnection->Connect(pszURL, nTimeout);
   if(FAILED(hr))
   {
      pConnection->Release();
      return hr;
   }
   *ppConnection = pConnection;
	return S_OK;
}


STDMETHODIMP CCustomDGramProtocol::CreateConnectionPoint(LPCWSTR pszURL, ILMNetConnectionPoint **ppConnectionPoint)
{
   if(!ppConnectionPoint || !pszURL)
      return E_POINTER;
   *ppConnectionPoint = NULL;
   CCustomDGramConnectionPoint* pConnectionPoint = new CComObject<CCustomDGramConnectionPoint>();
   if(!pConnectionPoint)
      return E_OUTOFMEMORY;
   pConnectionPoint->SetVoid(NULL);
   pConnectionPoint->InternalFinalConstructAddRef();
	HRESULT hr = pConnectionPoint->FinalConstruct();
	pConnectionPoint->InternalFinalConstructRelease();
   if(FAILED(hr))
   {
      delete pConnectionPoint;
      return hr;
   }
   pConnectionPoint->AddRef();
   hr = pConnectionPoint->Listen(pszURL);
   if(FAILED(hr))
   {
      pConnectionPoint->Release();
      return hr;
   }
   *ppConnectionPoint = pConnectionPoint;
	return S_OK;
}


