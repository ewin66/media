#pragma once

#include <vector>
#include <streams.h>
#include <ccrtp/rtp.h>
#include "FCNSourceRTPSession.h"

#include "../FCRTPLib/util.h"

using namespace ost;
using namespace std;

class FCNSourcePin : public CSourceStream
{
private:
	CCritSec * inputLock;
	FCNSourceRTPSession * input;

	bool isMulticast;				//the senderIP is a multicast address
	char * senderIP;				//the sender's IP address
	tpport_t senderPort;			//the sender's port
	PayloadType senderType;			//the expected PayloadType on this address/port

	const static ULONG MAX_BUFSIZE = 0x80000;

public:
	FCNSourcePin(LPCSTR pObjectName, CSource * pParent, HRESULT * phr, LPCWSTR pName);
	~FCNSourcePin(void);

	//bool ConnectToMulticast(BSTR serverIP, tpport_t dataPort, PayloadType payloadType);	//TODO implement multicast receive
	bool ConnectToSender(BSTR serverIP, tpport_t dataPort, PayloadType payloadType);
private:
	bool DoSenderConnection();
	inline void DisconnectFromSender();
public:

	HRESULT Inactive();
	HRESULT Active();

	HRESULT DecideBufferSize(IMemAllocator * pAlloc, ALLOCATOR_PROPERTIES * ppropInputRequest);
	HRESULT FillBuffer(IMediaSample * pSample);

	HRESULT CheckMediaType(const CMediaType * pmt);
	HRESULT GetMediaType(int iPosition, CMediaType * pMediaType);

	STDMETHODIMP Notify(IBaseFilter * pSender, Quality q);
private:

	bool WaitForPayload(DSPayload * outPayload);


	
};
