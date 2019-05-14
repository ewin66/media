// CustomDGramConnection.cpp : Implementation of CCustomConnection
#include "stdafx.h"
#include "Netcon2.h"
#include "DGramProtocol.h"
#include "CustomDGramConnection.h"
#include "PortNegotiation.h"

int CCustomDGramConnection::RecvChunk(BYTE* pbBuffer, int cbBuffer)
{
	if (m_bytesInBuffer == 0)
	{
#if 0
		DataPacket dp = m_packetQueue.Remove();
		if (dp.dp_type == 99)
		{
			m_packetReaderThread->Close();
			DebugLog("RecvChunk", "EOF packet in queue", 0, 0);
			return SOCKET_ERROR;
		}
		memcpy(m_buffer, dp.dp_buffer, dp.dp_buflen);
		m_bufferPosition = 0;
		m_bytesInBuffer = dp.dp_buflen;
#endif
#if 1
		if (RecvToBuffer() == false)
		{
			return SOCKET_ERROR;
		}
#endif
	}
	int retLen = min(m_bytesInBuffer, cbBuffer);
	memcpy(pbBuffer, m_buffer + m_bufferPosition, retLen);
	m_bufferPosition += retLen;
	m_bytesInBuffer -= retLen;
	return retLen;
}

int CCustomDGramConnection::SendChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout)
{
	int hr;
	fd_set readfds;
	struct timeval tv = { 0, 0 };

	readfds.fd_count = 1;
	readfds.fd_array[0] = m_controlSocket;
	hr = select(0, &readfds, NULL, NULL, &tv);
	if (hr == SOCKET_ERROR)
	{
		return hr;
	}
	if (hr == 1)
	{
		char controlBuf[32];
		unsigned int packetsRecv;
		unsigned int percentRecv = 0;
		int byteCount = recv(m_controlSocket, controlBuf, sizeof(controlBuf), 0);
		if (byteCount == 4)
		{
			memcpy(&packetsRecv, controlBuf, 4);
			if (m_packetsSent > 0)
			{
				percentRecv = (packetsRecv * 100) / m_packetsSent; 
			}
			DebugLog("SendChunk", "reception report", packetsRecv, m_packetsSent);
			DebugLog("SendChunk", "reception report percentage", percentRecv, 0);
		}
		closesocket(m_dataSocket);
		m_dataSocket = INVALID_SOCKET;
		closesocket(m_controlSocket);
		m_controlSocket = INVALID_SOCKET;
		WSASetLastError(ERROR_INVALID_HANDLE);
		return SOCKET_ERROR;
	}

	DataPacket* dp = (DataPacket*)m_sendBuf;
	dp->dp_type = 0;
	dp->dp_buflen = cbBuffer;
	dp->dp_seqno = m_packetsSent;
	memcpy(dp->dp_buffer, (char*)pbBuffer, cbBuffer);
	hr = sendto(m_dataSocket, (char*)dp, sizeof(DataPacket) - DATAPACKET_BUFSIZ + cbBuffer, 0, (sockaddr*)&m_dest_sin, sizeof(m_dest_sin));
	if (hr == SOCKET_ERROR)
	{
		DebugLog("SendChunk", "sendto returned SOCKET_ERROR", hr, 0);
	}
	m_packetsSent++;
	return cbBuffer;
}

STDMETHODIMP CCustomDGramConnection::Send(BYTE *pbBuffer, DWORD cbBuffer, DWORD *pcbSent, DWORD nTimeout)
{
	HRESULT hr = S_OK;

	int nLeft = cbBuffer;

	while (nLeft > 0)
	{
		int nWrite = min(nLeft, m_maxPacketSize);
		int nWritten = SendChunk(pbBuffer, nWrite, nTimeout);
		if (nWritten == SOCKET_ERROR)
		{
			int error = WSAGetLastError();
			DebugLog("Send", "got SOCKET_ERROR", error, 0);
			hr = HRESULT_FROM_WIN32(error);
			break;
		}
		nLeft -= nWritten;
		pbBuffer += nWritten;
	}
	if(pcbSent)
	{
		*pcbSent = cbBuffer - nLeft;
	}
	return hr;
}

STDMETHODIMP CCustomDGramConnection::Recv(BYTE *pbBuffer, DWORD cbBuffer, DWORD *pcbReceived, DWORD nTimeout)
{
	HRESULT hr = S_OK;
	int nRead = RecvChunk(pbBuffer, cbBuffer);
	if (nRead == SOCKET_ERROR)
	{
		DebugLog("Recv", "RecvChunk returned SOCKET_ERROR", 0, 0);
		hr = HRESULT_FROM_WIN32(ERROR_READ_FAULT);
		nRead = 0;
	}
	if(pcbReceived)
	{
		*pcbReceived = nRead;
	}
	return hr;
}

STDMETHODIMP CCustomDGramConnection::Disconnect()
{
	DebugLog("Disconnect", "packets received/next expected sequence#", m_packetsRecv, m_nextSeqNo);
	if (m_packetReaderThread != NULL)
	{
		m_packetReaderThread->Stop();
		m_packetReaderThread->Close();
		m_packetReaderThread = NULL;
	}
	if(m_controlSocket != INVALID_SOCKET)
	{
		if (m_packetsRecv > 0)
		{
			char retBuf[32];
			int byteCount = send(m_controlSocket, (const char*)&m_packetsRecv, sizeof(m_packetsRecv), 0);
			byteCount = recv(m_controlSocket, retBuf, sizeof(retBuf), 0);
		}
		CancelBlockingCall();
		if (m_dataSocket != INVALID_SOCKET)
		{
			closesocket(m_dataSocket);
			m_dataSocket = INVALID_SOCKET;
		}
		closesocket(m_controlSocket);
		m_controlSocket = INVALID_SOCKET;
		WSACleanup();
	}
	return S_OK;
}

STDMETHODIMP CCustomDGramConnection::IsConnected()
{
	DebugLog("IsConnected", "Invoke", 0, 0);
	if(m_controlSocket == INVALID_SOCKET)
	{
		return S_FALSE;
	}
	return S_OK;
}

HRESULT CCustomDGramConnection::Attach(SOCKET controlSocket, SOCKET dataSocket, SOCKADDR_IN dest_sin)
{
	WSADATA wsaData;

	Disconnect();

	if(controlSocket != INVALID_SOCKET)
	{
		int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);
		if (iResult != NO_ERROR)
		{
			DebugLog("Attach", "WSAStartup failed", 0, 0);
			closesocket(controlSocket);
			closesocket(dataSocket);
			return E_FAIL;
		}

		m_controlSocket = controlSocket;
		m_dataSocket = dataSocket;
		m_dest_sin = dest_sin;

		int v = 16*1024;
		int hr = setsockopt(m_dataSocket, SOL_SOCKET, SO_SNDBUF, (const char*) &v, sizeof(v));
		if (hr == SOCKET_ERROR)
		{
			DebugLog("Attach", "setsockopt SNDBUF failed", WSAGetLastError(), 0);
			closesocket(controlSocket);
			closesocket(dataSocket);
			return E_FAIL;
		}
		DebugLog("Attach", "negotiated", 0, 0);
	}
	return S_OK;
}

HRESULT CCustomDGramConnection::Connect(LPCWSTR pszURL, DWORD nTimeout)
{
	USES_CONVERSION;

	WSADATA wsaData;

	Disconnect();
	time_t now = time(0);

	int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);
	if (iResult != NO_ERROR)
	{
		return E_FAIL;
	}
	m_controlSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (m_controlSocket == INVALID_SOCKET)
	{
		int error = WSAGetLastError();
		WSACleanup();
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
		uc.lpszHostName = _T("0.0.0.0");
	}

	if(!uc.nPort)
	{
		uc.nPort = 27015;
	}
	sockaddr_in clientService; 
	memset(&clientService, 0, sizeof(clientService));
	clientService.sin_family = AF_INET;
	clientService.sin_addr.s_addr = inet_addr(uc.lpszHostName);
	clientService.sin_port = htons(uc.nPort);

	DebugLog("Connect", inet_ntoa(clientService.sin_addr), ntohs(clientService.sin_port), 0);
	if (connect( m_controlSocket, (SOCKADDR*) &clientService, sizeof(clientService) ) == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		DebugLog("Connect", "socket.connect() failed", error, 0);
		Disconnect();
		return HRESULT_FROM_WIN32(error);
	}
	m_dataSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (m_dataSocket == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		DebugLog("Connect", "socket() failed", error, 0);
		Disconnect();
		return HRESULT_FROM_WIN32(error);
	}
	int v = 1024 * 1024;
	int hr = setsockopt(m_dataSocket, SOL_SOCKET, SO_RCVBUF, (const char*) &v, sizeof(v));
	if (hr == SOCKET_ERROR)
	{
		DebugLog("Connect", "setsockopt SO_RCVBUF failed", WSAGetLastError(), 0);
		Disconnect();
		return HRESULT_FROM_WIN32(WSAGetLastError());
	}
	sockaddr_in clientAddr;
	int clientAddrLen;
	clientAddrLen = sizeof(clientAddr);
	clientAddr.sin_family = AF_INET;
	clientAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	PortNegotiation pn;
	int ctlBytes = recv(m_controlSocket, (char*)&pn, sizeof(pn), 0);
	if (ctlBytes == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		DebugLog("Connect", "socket.recv() PortNegotiation failed", error, 0);
		Disconnect();
		return HRESULT_FROM_WIN32(error);
	}
	DebugLog("Connect", "Negotiate", pn.pn_low, pn.pn_high);
	short selectedPort = 0;
	for (int port = pn.pn_low; port <= pn.pn_high; port++)
	{
		clientAddr.sin_port = htons(port);
		if ( bind(m_dataSocket, (struct sockaddr*)&clientAddr, clientAddrLen) == 0)
		{
			selectedPort = port;
			break;
		}
	}
	if (selectedPort == 0)
	{
		DebugLog("Connect", "failed to find available socket in negotiation range", 0, 0);
		Disconnect();
		return HRESULT_FROM_WIN32(WSAEADDRNOTAVAIL);
	}
	pn.pn_selected = selectedPort;
	if ( send(m_controlSocket, (const char*)&pn, sizeof(pn), 0) == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		DebugLog("Connect", "socket.send() PortNegotiation failed", error, 0);
		Disconnect();
		return HRESULT_FROM_WIN32(error);
	}
	DebugLog("Connect", "selected port", selectedPort, 0);
#if 0
	m_packetReaderThread = new PacketReaderThread(this);
	if (m_packetReaderThread->Create() == FALSE)
	{
		DebugLog("Connect", "failed creating packet reader thread", 0, 0);
		Disconnect();
		return HRESULT_FROM_WIN32(WSAEADDRNOTAVAIL);
	}
#endif
	return S_OK;
}

STDMETHODIMP CCustomDGramConnection::GetPeerName(LPWSTR *ppszPeer)
{
	USES_CONVERSION;

	if(!ppszPeer)
	{
		return E_POINTER;
	}
	*ppszPeer = NULL;

	SOCKADDR_IN sockAddr;
	memset(&sockAddr, 0, sizeof(sockAddr));

	int nSockAddrLen = sizeof(sockAddr);
	if(getpeername(m_controlSocket, (SOCKADDR*)&sockAddr, &nSockAddrLen) == SOCKET_ERROR)
	{
		int error = WSAGetLastError();
		return HRESULT_FROM_WIN32(error);
	}

	LPTSTR psz = inet_ntoa(sockAddr.sin_addr);
	if(!psz)
	{
		return E_FAIL;
	}
	*ppszPeer = (LPWSTR) CoTaskMemAlloc((_tcslen(psz) + 1) * sizeof(WCHAR));
	if(!*ppszPeer)
	{
		return E_OUTOFMEMORY;
	}
	wcscpy(*ppszPeer, T2OLE(psz));

	return S_OK;
}

STDMETHODIMP CCustomDGramConnection::CancelBlockingCall()
{
#if 0
	u_long iMode = 1;
	ioctlsocket(m_controlSocket, FIONBIO, &iMode);
	ioctlsocket(m_dataSocket, FIONBIO, &iMode);
#endif
	return S_OK;
}

void CCustomDGramConnection::DebugLog(char* method, char* event, int arg0, int arg1)
{
	char buf[256];
	sprintf(buf, "DGramConnection::%s %s %d %d\n", method, event, arg0, arg1);
	OutputDebugStringA(buf);
}

bool CCustomDGramConnection::RecvAndQueue()
{
	int hr;
	fd_set readfds;
	struct timeval tv = { 5, 0 };
	readfds.fd_count = 2;
	readfds.fd_array[0] = m_controlSocket;
	readfds.fd_array[1] = m_dataSocket;
	hr = select(0, &readfds, NULL, NULL, &tv);
	if (hr == SOCKET_ERROR || hr == 0)
	{
		DebugLog("RecvAndQueue", "hr == SOCKET_ERROR || hr == 0", hr, 0);
		return false;
	}
	if (FD_ISSET(m_controlSocket, &readfds))
	{
		char controlBuf[32];
		int byteCount = recv(m_controlSocket, controlBuf, sizeof(controlBuf), 0);
		DebugLog("RecvAndQueue", "select indicates control socket event - param is bytes recv on control socket", byteCount, 0);
		m_packetQueue.InsertEnd();
		return false;
	}
	int bytesRecv = recvfrom(m_dataSocket, (char*)m_sendBuf, sizeof(m_sendBuf), 0, 0, 0);
	if (bytesRecv == SOCKET_ERROR)
	{
		DebugLog("RecvAndQueue", "recvfrom on data socket returned SOCKET_ERROR", WSAGetLastError(), 0);
		return false;
	}
	DataPacket* dp = (DataPacket*)m_sendBuf;
	m_packetQueue.Insert(*dp);
	m_packetsRecv++;
	return true;
}

bool CCustomDGramConnection::RecvToBuffer()
{
	int hr;
	fd_set readfds;
	struct timeval tv = { 15, 0 };
	readfds.fd_count = 2;
	readfds.fd_array[0] = m_controlSocket;
	readfds.fd_array[1] = m_dataSocket;
	hr = select(0, &readfds, NULL, NULL, &tv);
	if (hr == SOCKET_ERROR || hr == 0)
	{
		DebugLog("RecvToBuffer", "hr == SOCKET_ERROR || hr == 0", hr, 0);
		return false;
	}
	if (FD_ISSET(m_controlSocket, &readfds))
	{
		char controlBuf[32];
		int byteCount = recv(m_controlSocket, controlBuf, sizeof(controlBuf), 0);
		DebugLog("RecvToBuffer", "select indicates control socket event - param is bytes recv on control socket", byteCount, 0);
		m_packetQueue.InsertEnd();
		return false;
	}
	int bytesRecv = recvfrom(m_dataSocket, (char*)m_sendBuf, sizeof(m_sendBuf), 0, 0, 0);
	if (bytesRecv == SOCKET_ERROR)
	{
		DebugLog("RecvToBuffer", "recvfrom on data socket returned SOCKET_ERROR", WSAGetLastError(), 0);
		return false;
	}
	DataPacket* dp = (DataPacket*)m_sendBuf;
	if (dp->dp_type == 99)
	{
		m_packetReaderThread->Close();
		DebugLog("RecvToBuffer", "EOF packet in queue", 0, 0);
		return false;
	}
	memcpy(m_buffer, dp->dp_buffer, dp->dp_buflen);
	m_bufferPosition = 0;
	m_bytesInBuffer = dp->dp_buflen;
	return true;
}
