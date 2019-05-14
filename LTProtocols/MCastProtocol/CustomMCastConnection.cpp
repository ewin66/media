// CustomConnection.cpp : Implementation of CCustomConnection
#include "stdafx.h"
#include "Netcon2.h"
#include "MCastProtocol.h"
#include "CustomMCastConnection.h"

int CCustomMCastConnection::RecvChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout)
{
	while (m_bytesInBuffer == 0)
	{
		m_bytesInBuffer = recvfrom(m_socket, m_buffer, sizeof(m_buffer), 0, 0, 0);
		if (m_bytesInBuffer == SOCKET_ERROR)
		{
			m_bytesInBuffer = 0;
		}
		else
		{
			m_bufferPosition = 0;
		}
	}
	int retLen = min(m_bytesInBuffer, cbBuffer);
	memcpy(pbBuffer, m_buffer + m_bufferPosition, retLen);
	m_bufferPosition += retLen;
	m_bytesInBuffer -= retLen;
	return retLen;
}

STDMETHODIMP CCustomMCastConnection::Send(BYTE *pbBuffer, DWORD cbBuffer, DWORD *pcbSent, DWORD nTimeout)
{
	HRESULT hr = S_OK;

	int nWritten = sendto(m_socket, (char*)pbBuffer, cbBuffer, 0, (sockaddr*)&m_dest_sin, sizeof(m_dest_sin));
	if (nWritten == SOCKET_ERROR)
	{
		hr = HRESULT_FROM_WIN32(WSAGetLastError());
	} 
	else if(pcbSent)
	{
		*pcbSent = cbBuffer;
	}
	return hr;
}

STDMETHODIMP CCustomMCastConnection::Recv(BYTE *pbBuffer, DWORD cbBuffer, DWORD *pcbReceived, DWORD nTimeout)
{
	HRESULT hr = S_OK;
	int nRead = RecvChunk(pbBuffer, cbBuffer, nTimeout);
	if (nRead == SOCKET_ERROR)
	{
		hr = HRESULT_FROM_WIN32(WSAGetLastError());
		nRead = 0;
	}
	if(pcbReceived)
	{
		*pcbReceived = nRead;
	}
	return hr;
}

STDMETHODIMP CCustomMCastConnection::Disconnect()
{
	if(m_socket != INVALID_SOCKET)
	{
		CancelBlockingCall();
		closesocket(m_socket);
		m_socket = INVALID_SOCKET;
		memset(&m_netevents, 0, sizeof(m_netevents));
		WSACleanup();
	}
	return S_OK;
}

STDMETHODIMP CCustomMCastConnection::IsConnected()
{
	if(m_socket == INVALID_SOCKET)
	{
		return S_FALSE;
	}
	return S_OK;
}

HRESULT CCustomMCastConnection::Attach(SOCKET socket, SOCKADDR_IN dest_sin)
{
	WSADATA wsaData;

	Disconnect();

	if(socket != INVALID_SOCKET)
	{
		int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);
		if (iResult != NO_ERROR)
		{
			closesocket(socket);
			return E_FAIL;
		}
		m_socket = socket;
		m_dest_sin = dest_sin;

		memset(&m_netevents, 0, sizeof(m_netevents));
		int v = 1024 * 1024;
		if (setsockopt(m_socket, SOL_SOCKET, SO_RCVBUF, (const char*) &v, sizeof(v)) == SOCKET_ERROR)
		{
			int error = WSAGetLastError();
			Disconnect();
			return HRESULT_FROM_WIN32(error);
		}
	}
	return S_OK;
}

HRESULT CCustomMCastConnection::Connect(LPCWSTR pszURL, DWORD nTimeout)
{
	USES_CONVERSION;

	WSADATA wsaData;
	struct ip_mreq mreq1;

	Disconnect();

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
	int v = 1024 * 1024;

	if (setsockopt(m_socket, SOL_SOCKET, SO_RCVBUF, (const char*) &v, sizeof(v)) == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		Disconnect();
		return HRESULT_FROM_WIN32(error);
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
		DWORD error = GetLastError();
		Disconnect();
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
	sockaddr_in clientService; 
	memset(&clientService, 0, sizeof(clientService));

	clientService.sin_family = AF_INET;
	clientService.sin_addr.s_addr = htonl(INADDR_ANY);
	clientService.sin_port = htons(uc.nPort);

	if (bind(m_socket, (SOCKADDR*)&clientService, sizeof(clientService)) == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		return HRESULT_FROM_WIN32(error);
	}
	mreq1.imr_multiaddr.s_addr = inet_addr(uc.lpszHostName);
	mreq1.imr_interface.s_addr = INADDR_ANY;

	if (setsockopt(m_socket, IPPROTO_IP, IP_ADD_MEMBERSHIP, (char*)&mreq1, sizeof(mreq1)) == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		closesocket(m_socket);
		return HRESULT_FROM_WIN32(error);
	}
	return S_OK;
}

STDMETHODIMP CCustomMCastConnection::GetPeerName(LPWSTR *ppszPeer)
{
	USES_CONVERSION;
	BSTR peerName = (BSTR)CoTaskMemAlloc(64);
	memcpy(peerName, L"Multicast Peer", 30);
	*ppszPeer = peerName;
	return S_OK;
}

STDMETHODIMP CCustomMCastConnection::CancelBlockingCall()
{
	return S_OK;
}
