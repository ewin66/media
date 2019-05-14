#include "FCNetworkSourceFilter.h"

FCNetworkSourceFilter::FCNetworkSourceFilter(LPUNKNOWN pUnk, HRESULT *phr)
	: CSource(NAME("CFCNetworkSourceFilter"), pUnk, CLSID_FCNetworkSource, phr),
	  outputs(), payloadMap()
{
	//TODO pins need to be generated upon remote-connect
	HRESULT hr;
	outputs.push_back(new FCNSourcePin((LPCSTR)NAME("pin 1"), this, &hr, L"Output 1"));
	
	payloadMap.AddMapping(96, MEDIATYPE_Video, MEDIASUBTYPE_H264_NALU_AVC1);

	//HACK just to transfer data....
	BSTR senderIP = SysAllocString(L"10.0.201.32");
	outputs[0]->ConnectToSender(senderIP, 5002, 96);
	SysFreeString(senderIP);
}

FCNetworkSourceFilter::~FCNetworkSourceFilter(void)
{
	while(outputs.size() > 0)
	{
		FCNSourcePin * p = outputs.back();
		outputs.pop_back();
		delete p;
	}
}

CUnknown *WINAPI FCNetworkSourceFilter::CreateInstance(LPUNKNOWN pUnk, HRESULT * phr)
{
    FCNetworkSourceFilter * filter = new FCNetworkSourceFilter(pUnk, phr);
    if (!filter)
	{
        if (phr)
		{
			*phr = E_OUTOFMEMORY;
		}
    }
    return filter;
}

int FCNetworkSourceFilter::GetPinCount()
{
	return outputs.size();
}

CBasePin * FCNetworkSourceFilter::GetPin(int n)
{
	return outputs[n];
}

STDMETHODIMP FCNetworkSourceFilter::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
	if((riid == IID_IMediaSeeking) ||
	   (riid == IID_IMediaPosition))
	{
		return E_NOINTERFACE;
	}
	return CSource::NonDelegatingQueryInterface(riid, ppv);
}