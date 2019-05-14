#pragma once
/*
	FCNetworkSourceFilter.h
	Top-level "filter" object. Exposes pins, etc...

	Kevin C. Dixon
	Future Concepts
	03/23/09
*/

#include <vector>
#include <streams.h>
#include "FCNSourcePin.h"
#include "../FCRTPLib/FCRTPLib.h"

using std::vector;

class FCNetworkSourceFilter : public CSource
{
	friend FCNSourcePin;
private:
	vector<FCNSourcePin*> outputs;
	PayloadMap payloadMap;

public:
	FCNetworkSourceFilter(LPUNKNOWN pUnk, HRESULT *phr);
	~FCNetworkSourceFilter(void);

	static CUnknown * WINAPI CreateInstance(LPUNKNOWN pUnk, HRESULT * phr);

	virtual int GetPinCount();
	virtual CBasePin * GetPin(int n);

	STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);
};
