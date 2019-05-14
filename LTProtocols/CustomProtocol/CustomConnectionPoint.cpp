// CustomConnectionPoint.cpp : Implementation of CCustomConnectionPoint
#include "stdafx.h"
#include "Netcon2.h"
#include "NetProtocol.h"
#include "CustomConnectionPoint.h"
#include "CustomConnection.h"


/////////////////////////////////////////////////////////////////////////////
// CCustomConnectionPoint

BOOL CCustomConnectionPoint::EventWait(long nEvents, DWORD nTimeout)
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

STDMETHODIMP CCustomConnectionPoint::GetConnection(ILMNetConnection **ppConnection, DWORD nTimeout)
{
   if(!ppConnection)
      return E_POINTER;
   *ppConnection = NULL;

   SOCKET AcceptSocket;
   while((AcceptSocket = accept(m_socket, NULL, NULL )) == SOCKET_ERROR)
   {
      if (WSAGetLastError() == WSAEWOULDBLOCK)
      {
         if (!EventWait(FD_ACCEPT, nTimeout))
         {   
            int error = WSAGetLastError();
            return HRESULT_FROM_WIN32(error);
         }
      }
      else
      {
         int error = WSAGetLastError();
         return HRESULT_FROM_WIN32(error);
      }
   }
   CCustomConnection* pConnection = new CComObject<CCustomConnection>();
   if(!pConnection)
   {
      closesocket(AcceptSocket);
      return E_OUTOFMEMORY;
   }
   pConnection->SetVoid(NULL);
   pConnection->InternalFinalConstructAddRef();
	HRESULT hr = pConnection->FinalConstruct();
	pConnection->InternalFinalConstructRelease();
   if(FAILED(hr))
   {
      closesocket(AcceptSocket);
      delete pConnection;
      return hr;
   }
   pConnection->AddRef();
   hr = pConnection->Attach(AcceptSocket);
   if(FAILED(hr))
   {
      // The socket is automatically closed by the Attach function
      pConnection->Release();
      return hr;
   }
   *ppConnection = pConnection;
   
   return S_OK;
}

STDMETHODIMP CCustomConnectionPoint::Close()
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


HRESULT CCustomConnectionPoint::Listen(LPCWSTR pszURL)
{
   USES_CONVERSION;

   WSADATA wsaData;
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
      Close();
      return HRESULT_FROM_WIN32(error);
   }
   
   m_evsocket[1] = WSACreateEvent();
   if(m_evsocket[1] == WSA_INVALID_EVENT)
   {
      int error = WSAGetLastError();
      Close();
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
      int error = GetLastError();
      Close();
      return HRESULT_FROM_WIN32(error);
   }
   
   if(!uc.lpszHostName)
      uc.lpszHostName = _T("0.0.0.0");
   
   if(!uc.nPort)
      uc.nPort = 27015;
   
   sockaddr_in service;
   memset(&service, 0, sizeof(service));
   
   service.sin_family = AF_INET;
   service.sin_addr.s_addr = inet_addr(uc.lpszHostName);
   service.sin_port = htons(uc.nPort);
   
   
   BOOL enable = TRUE;
   setsockopt(m_socket, IPPROTO_TCP, TCP_NODELAY, (const char*) &enable, sizeof(enable));
   
   WSAEventSelect(m_socket, m_evsocket[0], FD_READ | FD_WRITE | FD_OOB | FD_ACCEPT | FD_CONNECT | FD_CLOSE);

   if (bind(m_socket, (SOCKADDR*) &service, sizeof(service)) == SOCKET_ERROR)
   {
      int error = WSAGetLastError();
      Close();
      return HRESULT_FROM_WIN32(error);
   }
   
   //----------------------
   // Listen for incoming connection requests 
   // on the created socket
   if (listen( m_socket, SOMAXCONN) == SOCKET_ERROR)
   {
      int error = WSAGetLastError();
      Close();
      return HRESULT_FROM_WIN32(error);
   }
   return S_OK;
}

STDMETHODIMP CCustomConnectionPoint::CancelBlockingCall()
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
