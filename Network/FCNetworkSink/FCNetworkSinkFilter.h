#pragma once
/*
	FCNetworkSinkFilter.h
	Top-level "filter" object. Exposes pins, etc...

	Kevin C. Dixon
	Future Concepts
	03/10/09
*/

#include <deque>
using std::deque;

#include "../FCRTPLib/FCRTPLib.h"
#include "FCNSinkPin.h"
#include "IFCNSink.h"

class FCNetworkSinkFilter : public CBaseFilter, IFCNSink
{
	friend FCNSinkPin;

private:
	deque<FCNSinkPin*> inputs;
	unsigned long lifetimePinCount;
	PayloadMap configuredPayloads;
	static PayloadMap * supportedPayloads;

public:
	FCNetworkSinkFilter(LPUNKNOWN pUnk, HRESULT *phr);
	~FCNetworkSinkFilter(void);

	DECLARE_IUNKNOWN;

	static CUnknown * WINAPI CreateInstance(LPUNKNOWN pUnk, HRESULT * phr);

	STDMETHODIMP Run(REFERENCE_TIME tStart);

	virtual int GetPinCount();
	virtual CBasePin * GetPin(int n);

#pragma region IFCNSink Methods

	STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void **ppv);


	STDMETHODIMP GetStreamCount(int * count);
	STDMETHODIMP SetPortAllocation(int index, unsigned short data, unsigned short control);
	STDMETHODIMP GetPortAllocation(int index, unsigned short * data, unsigned short * control);
	STDMETHODIMP GetStream(int index, FCRTPStreamDescription * description);
	STDMETHODIMP AddDestination(int index, const FCRTPEndpoint * destination);
	STDMETHODIMP RemoveDestination(int index, const FCRTPEndpoint * destination);

#pragma endregion

private:
	
#pragma region Pin-Callable Methods

	bool SpawnInputPin();
	void InputPinDisconnected(FCNSinkPin * pin);

#pragma endregion
};
