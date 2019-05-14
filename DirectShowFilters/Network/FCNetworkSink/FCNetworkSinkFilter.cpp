#include "FCNetworkSinkFilter.h"

//Implementation of FCNetworkSinkFilter methods

FCNetworkSinkFilter::FCNetworkSinkFilter(LPUNKNOWN pUnk, HRESULT *phr)
	: CBaseFilter(NAME("CFCNetworkSinkFilter"), pUnk, new CCritSec(), CLSID_FCNetworkSink),
	  inputs(), lifetimePinCount(0), payloadMap()
{
	payloadMap.AddMappingForH264(100);	//HACK needs to be set by application

	if(!SpawnInputPin())
	{
		if(phr)
		{
			(*phr) = E_FAIL;
		}
	}
}

FCNetworkSinkFilter::~FCNetworkSinkFilter()
{
	while(inputs.size() > 0)
	{
		FCNSinkPin * p = inputs.back();
		inputs.pop_back();
		delete p;
	}
}

CUnknown *WINAPI FCNetworkSinkFilter::CreateInstance(LPUNKNOWN pUnk, HRESULT * phr)
{
    FCNetworkSinkFilter * filter = new FCNetworkSinkFilter(pUnk, phr);
    if (!filter)
	{
        if (phr)
		{
			*phr = E_OUTOFMEMORY;
		}
    }
    return filter;
}

STDMETHODIMP FCNetworkSinkFilter::Run(REFERENCE_TIME tStart)
{
	//TODO setup port assignments per Pin

	return CBaseFilter::Run(tStart);
}

int FCNetworkSinkFilter::GetPinCount()
{
	return inputs.size();
}


CBasePin * FCNetworkSinkFilter::GetPin(int n)
{
	return inputs[n];
}

bool FCNetworkSinkFilter::SpawnInputPin()
{
	try
	{
		HRESULT hr;
		wchar_t buf[50];
		swprintf(buf, L"Input %i", ++lifetimePinCount);
		FCNSinkPin * pin = new FCNSinkPin(NAME("Input"), this, m_pLock, &hr, buf);
		if(SUCCEEDED(hr))
		{
			if(pin)
			{
				inputs.push_back(pin);
				CBaseFilter::IncrementPinVersion();
				return true;
			}
		}
		return false;
	}
	catch(...)
	{
		return false;
	}
}

void FCNetworkSinkFilter::InputPinDisconnected(FCNSinkPin * pin)
{
	if(inputs.size() > 1)
	{
		int pinIndex = -1;
		for(int i = 0; i < inputs.size(); i++)
		{
			if(inputs[i] == pin)
			{
				pinIndex = i;
				break;
			}
		}
		if(pinIndex > -1)
		{
			inputs.erase(inputs.begin() + pinIndex);
			//TODO determine if this is leaking memory---right now it explodes
			//pin->Release();
			//delete pin;
			CBaseFilter::IncrementPinVersion();
		}
	}
}