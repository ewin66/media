// CustomConnectionPoint.cpp : Implementation of CCustomConnectionPoint
#include "stdafx.h"
#include "Netcon2.h"
#include "MCastProtocol.h"
#include "CustomMCastConnectionPoint.h"
#include "CustomMCastConnection.h"

STDMETHODIMP CCustomMCastConnectionPoint::GetConnection(ILMNetConnection **ppConnection, DWORD nTimeout)
{
	if(!ppConnection)
	{
		return E_POINTER;
	}
	*ppConnection = NULL;
	if (m_pConnection != NULL)
	{
		WaitForSingleObject(m_hCloseEvent, INFINITE);
		return HRESULT_FROM_WIN32(WSAENOTSOCK);
	}
	CCustomMCastConnection* pConnection = new CComObject<CCustomMCastConnection>();
	if(!pConnection)
	{
		return E_OUTOFMEMORY;
	}
	m_pConnection = pConnection;
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
	hr = pConnection->Attach(m_socket, m_dest_sin);
	if(FAILED(hr))
	{
		pConnection->Release();
		return hr;
	}
	*ppConnection = pConnection;
	return S_OK;
}

STDMETHODIMP CCustomMCastConnectionPoint::Close()
{
	if (m_pConnection != NULL)
	{
		m_pConnection = NULL;
	}
	if(m_socket != INVALID_SOCKET)
	{
		CancelBlockingCall();
		closesocket(m_socket);
		m_socket = INVALID_SOCKET;
		WSACleanup();
	}
	return S_OK;
}

HRESULT CCustomMCastConnectionPoint::Listen(LPCWSTR pszURL)
{
	USES_CONVERSION;

	WSADATA wsaData;
	int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);
	if (iResult != NO_ERROR)
	{
		return E_FAIL;
	}
	m_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (m_socket == INVALID_SOCKET)
	{
		int error = WSAGetLastError();
		WSACleanup();
		return HRESULT_FROM_WIN32(error);
	}
	m_hCloseEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
	if (m_hCloseEvent == NULL)
	{
		return HRESULT_FROM_WIN32(GetLastError());
	}
	URL_COMPONENTS uc;
	memset(&uc, 0, sizeof(uc));
	uc.dwStructSize = sizeof(uc);

	uc.dwSchemeLength = INTERNET_MAX_SCHEME_LENGTH;
	uc.lpszScheme = (LPTSTR) _alloca(INTERNET_MAX_SCHEME_LENGTH+1);
	uc.dwHostNameLength = INTERNET_MAX_HOST_NAME_LENGTH;
	uc.lpszHostName = (LPTSTR) _alloca(INTERNET_MAX_HOST_NAME_LENGTH+1);
	uc.dwUrlPathLength = INTERNET_MAX_PATH_LENGTH;
	uc.lpszUrlPath = (LPTSTR) _alloca(INTERNET_MAX_PATH_LENGTH+1);
	uc.dwUserNameLength = INTERNET_MAX_USER_NAME_LENGTH;
	uc.lpszUserName = (LPTSTR) _alloca(INTERNET_MAX_USER_NAME_LENGTH+1);
	uc.dwPasswordLength = INTERNET_MAX_PASSWORD_LENGTH;
	uc.lpszPassword = (LPTSTR) _alloca(INTERNET_MAX_PASSWORD_LENGTH+1);

	if(!InternetCrackUrl(OLE2T(pszURL), 0, ICU_ESCAPE, &uc))
	{
		int error = GetLastError();
		Close();
		return HRESULT_FROM_WIN32(error);
	}
	if(!uc.lpszHostName)
	{
		uc.lpszHostName = _T("234.5.6.7");
	}
	if(!uc.nPort)
	{
		uc.nPort = 27015;
	}
	m_dest_sin.sin_family = AF_INET;
	m_dest_sin.sin_port = htons(uc.nPort);
	m_dest_sin.sin_addr.s_addr = inet_addr(uc.lpszHostName);

	return S_OK;
}

STDMETHODIMP CCustomMCastConnectionPoint::CancelBlockingCall()
{
	SetEvent(m_hCloseEvent);
	return S_OK;
}
