#pragma once

#include <streams.h>
#include "FCNSinkRTPSession.h"

#include "../uuids.h"

class FCNSinkPin : public CRenderedInputPin
{
private:
	//the port this session will receive data on
	tpport_t receiveDataPort;
	//the port this session will send data on
	tpport_t receiveControlPort;

	FCNSinkRTPSession * output;
	CCritSec * outputMutex;

public:
	FCNSinkPin(LPCSTR pObjectName, CBaseFilter * pParent, CCritSec * pLock, HRESULT * phr, LPCWSTR pName);
	~FCNSinkPin(void);

	HRESULT CompleteConnect(IPin * pReceivePin);
	HRESULT BreakConnect();
	STDMETHODIMP Disconnect();

	//TODO BeginFlush
	//TODO EndFlush
	STDMETHODIMP EndOfStream();

	STDMETHODIMP Receive(IMediaSample *pSample);

	HRESULT CheckMediaType(const CMediaType * pmt);
	HRESULT GetMediaType(int iPosition, CMediaType * pMediaType);

	HRESULT Inactive();
	HRESULT Active();
	HRESULT Run(REFERENCE_TIME tStart);

#pragma region RTP Control Interface

	void SetPortAllocation(tpport_t receiveDataPort, tpport_t receiveControlPort = 0);
	bool GetPortAllocation(tpport_t * receiveDataPort, tpport_t * receiveControlPort);

	bool AddDestination(const FCRTPEndpoint * client);
	bool RemoveDestination(const FCRTPEndpoint * client);

#pragma endregion

private:

	HRESULT CreateOutput();
	
	bool SetPayloadFormatter();

	/*
		Deletes the output module
	*/
	void DeleteOutput();



};