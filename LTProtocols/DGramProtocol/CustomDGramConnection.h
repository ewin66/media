	
// CustomDGramConnection.h : Declaration of the CCustomConnection

#ifndef __CustomDGramCONNECTION_H_
#define __CustomDGramCONNECTION_H_

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CCustomConnection
class ATL_NO_VTABLE CCustomDGramConnection : 
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CCustomDGramConnection, &CLSID_CustomDGramConnection>,
	public ILMNetConnection
{
public:
	CCustomDGramConnection()
	{
		m_pUnkMarshaler = NULL;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_CUSTOMDGRAMCONNECTION)
DECLARE_GET_CONTROLLING_UNKNOWN()

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CCustomDGramConnection)
	COM_INTERFACE_ENTRY(ILMNetConnection)
	COM_INTERFACE_ENTRY_AGGREGATE(IID_IMarshal, m_pUnkMarshaler.p)
END_COM_MAP()

	HRESULT FinalConstruct()
	{
		m_packetsSent = 0;
		m_packetsRecv = 0;
		m_bytesInBuffer = 0;
		m_bufferPosition = 0;
		m_nextSeqNo = 0;
		m_packetReaderThread = NULL;
      m_controlSocket = INVALID_SOCKET;
	  m_dataSocket = INVALID_SOCKET;
	  char* maxPacketSize = getenv("AntaresX_Video_DGRAM_MAX_PACKET_SIZE");
	  if (maxPacketSize != NULL)
	  {
		  m_maxPacketSize = atoi(maxPacketSize);
	  }
	  else
	  {
		  m_maxPacketSize = 576;
	  }
	  return CoCreateFreeThreadedMarshaler(GetControllingUnknown(), &m_pUnkMarshaler.p);
	}

	void FinalRelease()
	{
      Disconnect();
      m_pUnkMarshaler.Release();
	}

	CComPtr<IUnknown> m_pUnkMarshaler;

// ILMNetConnection
public:
	STDMETHOD(CancelBlockingCall)();
	STDMETHOD(GetPeerName)(/*[out, string]*/ LPWSTR* ppszPeer);
	STDMETHOD(IsConnected)();
	STDMETHOD(Disconnect)();
	STDMETHOD(Recv)(/*[in, size_is(cbBuffer)]*/ BYTE* pbBuffer,  /*[in]*/ DWORD cbBuffer,  /*[out]*/ DWORD* pcbReceived, /*[in]*/ DWORD nTimeout);
	STDMETHOD(Send)(/*[in, size_is(cbBuffer)]*/ BYTE* pbBuffer,  /*[in]*/ DWORD cbBuffer,  /*[out]*/ DWORD* pcbSent, /*[in]*/ DWORD nTimeout);

   HRESULT Attach(SOCKET controlSocket, SOCKET dataSocket, SOCKADDR_IN dest_sin);
   HRESULT Connect(LPCWSTR pszURL, DWORD nTimeout);

public:
	bool RecvAndQueue();
	bool RecvToBuffer();

protected:
   int SendChunk(BYTE* pbBuffer, int cbBuffer, DWORD nTimeout);
   int RecvChunk(BYTE* pbBuffer, int cbBuffer);
   void DebugLog(char* method, char* event, int arg0, int arg1);
   
   SOCKET m_controlSocket;
   SOCKET m_dataSocket;
   SOCKADDR_IN m_dest_sin;
   	char m_buffer[128 * 1024];
	char m_sendBuf[128 * 1024];
	int m_bytesInBuffer;
	int m_bufferPosition;

	int m_nRecvBlocks;

	int m_maxPacketSize;

	unsigned long m_packetsSent;
	unsigned long m_packetsRecv;

	unsigned long m_nextSeqNo;

	PacketQueue	m_packetQueue;

	PacketReaderThread* m_packetReaderThread;
 };

#endif //__CustomDGramCONNECTION_H_
