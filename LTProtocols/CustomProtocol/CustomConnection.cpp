// CustomConnection.cpp : Implementation of CCustomConnection
#include "stdafx.h"
#include "Netcon2.h"
#include "NetProtocol.h"
#include "CustomConnection.h"
/////////////////////////////////////////////////////////////////////////////
// CCustomConnection

#define CHUNKSIZE (1024 * 16)


BOOL CCustomConnection::EventWait(long nEvents, DWORD nTimeout)
{
   if(InterlockedIncrement(&m_blocking) > 1)
   {
      InterlockedDecrement(&m_blocking);
		WSASetLastError(WSAEINPROGRESS);
      return FALSE;
   }
   for(;;)
   {
      DWORD dw = WSAWaitForMultipleEvents(2, m_evsocket, FALSE, nTimeout, FALSE);
      switch(dw)
      {
      case WSA_WAIT_TIMEOUT:
         WSASetLastError(WSAETIMEDOUT);
         InterlockedDecrement(&m_blocking);
         return FALSE;
         break;
      case (WSA_WAIT_EVENT_0 + 0):
         {
            if(WSAEnumNetworkEvents(m_socket, m_evsocket[0], &m_netevents) != SOCKET_ERROR)
            {
               if(m_netevents.lNetworkEvents & nEvents)
               {
                  InterlockedDecrement(&m_blocking);
                  return TRUE;
               }
               if(m_netevents.lNetworkEvents & FD_CLOSE)
               {
                  InterlockedDecrement(&m_blocking);
                  return TRUE;
               }
            }
            else
            {
               InterlockedDecrement(&m_blocking);
               return FALSE;
            }
         }
         break;
      case (WSA_WAIT_EVENT_0 + 1):
         WSASetLastError(WSAEINTR);
         InterlockedDecrement(&m_blocking);
         return FALSE;
         break;
      default:
         InterlockedDecrement(&m_blocking);
         return FALSE;
         break;
         
      }
   }
}

int CCustomConnection::SendChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout)
{
   int nResult;
   while ((nResult = send(m_socket, (char*) pbBuffer, cbBuffer, 0)) == SOCKET_ERROR)
   {
      if (WSAGetLastError() == WSAEWOULDBLOCK)
      {
         if (!EventWait(FD_WRITE, nTimeout))
            return SOCKET_ERROR;
      }
      else
      {
         break;
      }
   }
   return nResult;
}

int CCustomConnection::RecvChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout)
{
   int nResult;
   
   while((nResult = recv(m_socket, (char*) pbBuffer, cbBuffer, 0)) == SOCKET_ERROR)
   {
      if (WSAGetLastError() == WSAEWOULDBLOCK)
      {
         if (!EventWait(FD_READ, nTimeout))
            return SOCKET_ERROR;
      }
      else
      {
         break;
      }
   }
   return nResult;
}

STDMETHODIMP CCustomConnection::Send(BYTE *pbBuffer, DWORD cbBuffer, DWORD *pcbSent, DWORD nTimeout)
{
   HRESULT hr = S_OK;

   DWORD nLeft = cbBuffer;
   
   while (nLeft > 0)
   {
      int nWrite = min(nLeft, CHUNKSIZE);
      int nWritten = SendChunk(pbBuffer, nWrite, nTimeout);
      if (nWritten == SOCKET_ERROR)
      {
         int error = WSAGetLastError();
         hr = HRESULT_FROM_WIN32(error);
         break;
      }
      nLeft -= nWritten;
      pbBuffer += nWritten;
   }
   if(pcbSent)
      *pcbSent = cbBuffer - nLeft;
	return hr;
}

STDMETHODIMP CCustomConnection::Recv(BYTE *pbBuffer, DWORD cbBuffer, DWORD *pcbReceived, DWORD nTimeout)
{
   HRESULT hr = S_OK;

   DWORD nLeft = cbBuffer;
   
   while (nLeft > 0)
   {
      int nToRead = min(nLeft, CHUNKSIZE);
      int nRead = RecvChunk(pbBuffer, nToRead, nTimeout);
      if (nRead == SOCKET_ERROR)
      {
         int error = WSAGetLastError();
         // if we timed out, but have already read data, then return without error
         if(error == WSAETIMEDOUT && nLeft < cbBuffer)
            break;
         hr = HRESULT_FROM_WIN32(error);
         break;
      }
      if(!nRead)
         break;
      nTimeout = 0;
      nLeft -= nRead;
      pbBuffer += nRead;
   }
   if(pcbReceived)
      *pcbReceived = cbBuffer - nLeft;
	return hr;
}

STDMETHODIMP CCustomConnection::Disconnect()
{
   if(m_socket != INVALID_SOCKET)
   {
  
      CancelBlockingCall();

      closesocket(m_socket);
      if(m_evsocket[0] != WSA_INVALID_EVENT)
      {
         WSACloseEvent(m_evsocket[0]);
         m_evsocket[0] = WSA_INVALID_EVENT;
      }
      if(m_evsocket[1] != WSA_INVALID_EVENT)
      {
         WSACloseEvent(m_evsocket[1]);
         m_evsocket[1] = WSA_INVALID_EVENT;
      }
      m_socket = INVALID_SOCKET;
      memset(&m_netevents, 0, sizeof(m_netevents));
      WSACleanup();
   }
	return S_OK;
}

STDMETHODIMP CCustomConnection::IsConnected()
{
  if(m_socket == INVALID_SOCKET || EventWait(FD_CLOSE, 0))
     return S_FALSE;
  return S_OK;
}

HRESULT CCustomConnection::Attach(SOCKET socket)
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

      memset(&m_netevents, 0, sizeof(m_netevents));
      m_evsocket[0] = WSACreateEvent();
      if(m_evsocket[0] == WSA_INVALID_EVENT)
      {
         int error = WSAGetLastError();
         Disconnect();
         return HRESULT_FROM_WIN32(error);
      }
   
      m_evsocket[1] = WSACreateEvent();
      if(m_evsocket[1] == WSA_INVALID_EVENT)
      {
         int error = WSAGetLastError();
         Disconnect();
         return HRESULT_FROM_WIN32(error);
      }
      int v = 65536;
      setsockopt(m_socket, SOL_SOCKET, SO_RCVBUF, (const char*) &v, sizeof(v));

      WSAEventSelect(m_socket, m_evsocket[0], FD_READ | FD_WRITE | FD_OOB | FD_ACCEPT | FD_CONNECT | FD_CLOSE);
   }
   return S_OK;
}

HRESULT CCustomConnection::Connect(LPCWSTR pszURL, DWORD nTimeout)
{
   USES_CONVERSION;

   WSADATA wsaData;

   Disconnect();
   
   int iResult = WSAStartup(MAKEWORD(2,2), &wsaData);
   if (iResult != NO_ERROR)
      return E_FAIL;
   
   m_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
   if (m_socket == INVALID_SOCKET)
   {
      int error = WSAGetLastError();
      WSACleanup();
      return HRESULT_FROM_WIN32(error);
   }
   
   memset(&m_netevents, 0, sizeof(m_netevents));
   m_evsocket[0] = WSACreateEvent();
   if(m_evsocket[0] == WSA_INVALID_EVENT)
   {
      int error = WSAGetLastError();
      Disconnect();
      return HRESULT_FROM_WIN32(error);
   }
   
   m_evsocket[1] = WSACreateEvent();
   if(m_evsocket[1] == WSA_INVALID_EVENT)
   {
      int error = WSAGetLastError();
      Disconnect();
      return HRESULT_FROM_WIN32(error);
   }
   
   int v = 65536;
   setsockopt(m_socket, SOL_SOCKET, SO_RCVBUF, (const char*) &v, sizeof(v));
   
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
      uc.lpszHostName = _T("0.0.0.0");
   
   if(!uc.nPort)
      uc.nPort = 27015;
   
   
   sockaddr_in clientService; 
   memset(&clientService, 0, sizeof(clientService));
   
   clientService.sin_family = AF_INET;
   clientService.sin_addr.s_addr = inet_addr(uc.lpszHostName);
   clientService.sin_port = htons(uc.nPort);
   
   
   WSAEventSelect(m_socket, m_evsocket[0], FD_READ | FD_WRITE | FD_OOB | FD_ACCEPT | FD_CONNECT | FD_CLOSE);
   
   if (connect( m_socket, (SOCKADDR*) &clientService, sizeof(clientService) ) == SOCKET_ERROR)
   {
      int error = WSAGetLastError();
      if(error != WSAEWOULDBLOCK)
      {
         Disconnect();
         return HRESULT_FROM_WIN32(error);
      }
      else
      {
         if(!EventWait(FD_CONNECT, INFINITE))
         {
            error = WSAGetLastError();
            Disconnect();
            return HRESULT_FROM_WIN32(error);
         }
         else
         {
            error = m_netevents.iErrorCode[FD_CONNECT_BIT];
            if(error)
            {
               Disconnect();
               return HRESULT_FROM_WIN32(error);
            }
         }
      }
      
   }

   return S_OK;
}


STDMETHODIMP CCustomConnection::GetPeerName(LPWSTR *ppszPeer)
{
   USES_CONVERSION;

   if(!ppszPeer)
      return E_POINTER;

   *ppszPeer = NULL;

   SOCKADDR_IN sockAddr;
	memset(&sockAddr, 0, sizeof(sockAddr));

	int nSockAddrLen = sizeof(sockAddr);
   if(getpeername(m_socket, (SOCKADDR*)&sockAddr, &nSockAddrLen) == SOCKET_ERROR)
   {
      int error = WSAGetLastError();
      return HRESULT_FROM_WIN32(error);
   }

   LPTSTR psz = inet_ntoa(sockAddr.sin_addr);
   if(!psz)
      return E_FAIL;
   *ppszPeer = (LPWSTR) CoTaskMemAlloc((_tcslen(psz) + 1) * sizeof(WCHAR));
   if(!*ppszPeer)
      return E_OUTOFMEMORY;

   wcscpy(*ppszPeer, T2OLE(psz));

	return S_OK;
}

STDMETHODIMP CCustomConnection::CancelBlockingCall()
{
	if(m_blocking)
   {
      WSASetEvent(m_evsocket[1]);
      while(m_blocking)
         Sleep(0);
      WSAResetEvent(m_evsocket[1]);
   }
	return S_OK;
}
