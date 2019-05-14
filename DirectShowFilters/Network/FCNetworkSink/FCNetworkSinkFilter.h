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


class FCNetworkSinkFilter : public CBaseFilter
{
	friend FCNSinkPin;

private:
	deque<FCNSinkPin*> inputs;
	unsigned long lifetimePinCount;
	PayloadMap payloadMap;

public:
	FCNetworkSinkFilter(LPUNKNOWN pUnk, HRESULT *phr);
	~FCNetworkSinkFilter(void);

	static CUnknown * WINAPI CreateInstance(LPUNKNOWN pUnk, HRESULT * phr);

	STDMETHODIMP Run(REFERENCE_TIME tStart);

	virtual int GetPinCount();
	virtual CBasePin * GetPin(int n);

private:
	//Pin-callable methods
	bool SpawnInputPin();
	void InputPinDisconnected(FCNSinkPin * pin);
};
