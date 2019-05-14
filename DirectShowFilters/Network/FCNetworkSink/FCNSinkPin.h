#pragma once

#include <streams.h>
#include "FCNSinkRTPSession.h"

#include "../uuids.h"

class FCNSinkPin : public CRenderedInputPin
{
private:
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

private:

	HRESULT CreateOutput();
	
	bool SetPayloadFormatter();

	/*
		Deletes the output module
	*/
	void DeleteOutput();



};