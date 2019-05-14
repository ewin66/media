#include "FCNetworkSinkFilter.h"

PayloadMap * FCNetworkSinkFilter::supportedPayloads = PayloadFormatterFactory::GetSupportedPayloadTypes();

//Implementation of FCNetworkSinkFilter methods

FCNetworkSinkFilter::FCNetworkSinkFilter(LPUNKNOWN pUnk, HRESULT *phr)
	: CBaseFilter(NAME("CFCNetworkSinkFilter"), pUnk, new CCritSec(), CLSID_FCNetworkSink),
	  inputs(), lifetimePinCount(0), configuredPayloads()
{
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

#pragma region IFCNSink Methods

STDMETHODIMP FCNetworkSinkFilter::NonDelegatingQueryInterface(REFIID riid, void **ppv)
{
	CheckPointer(ppv, E_POINTER);

	if (riid == IID_IFCNSink)
	{
		return GetInterface(static_cast<IFCNSink*>(this), ppv);
	}
	else
	{
		return CBaseFilter::NonDelegatingQueryInterface(riid, ppv);
	}
}

STDMETHODIMP FCNetworkSinkFilter::GetStreamCount(int * count)
{
	(*count) = 0;

	for(int i = 0; i < (int)inputs.size(); i++)
	{
		IPin * test;
		if(FAILED(inputs[i]->ConnectedTo(&test)))
			break;
		test->Release();
		(*count)++;
	}

	return S_OK;
}

STDMETHODIMP FCNetworkSinkFilter::SetPortAllocation(int index, unsigned short data, unsigned short control)
{
	if(index < 0)
	{
		return E_INVALIDARG;
	}
	if(index >= (int)inputs.size())
	{
		return VFW_S_NO_MORE_ITEMS;
	}

	inputs[index]->SetPortAllocation(data, control);

	return S_OK;
}

STDMETHODIMP FCNetworkSinkFilter::GetPortAllocation(int index, unsigned short * data, unsigned short * control)
{
	if(index < 0)
	{
		return E_INVALIDARG;
	}
	if(index >= (int)inputs.size())
	{
		return VFW_S_NO_MORE_ITEMS;
	}

	if(!inputs[index]->GetPortAllocation(data, control))
	{
		return S_FALSE;
	}

	return S_OK;
}

STDMETHODIMP FCNetworkSinkFilter::GetStream(int index, FCRTPStreamDescription * description)
{
	CheckPointer(description, E_POINTER);

	memset(description, 0, sizeof(FCRTPStreamDescription));

	if(index < 0)
	{
		return E_INVALIDARG;
	}
	if(index >= (int)inputs.size())
	{
		return VFW_S_NO_MORE_ITEMS;
	}

	//check if input is connected
	IPin * inputFeeder;
	if(FAILED(inputs[index]->ConnectedTo(&inputFeeder)))
		return S_FALSE;
	inputFeeder->Release();

	//get connection media type
	if (FAILED(inputs[index]->ConnectionMediaType(&(description->dsType))))
		return E_FAIL;

	//get the payload that will be used for the media type
	if (!configuredPayloads.GetMapping(description->dsType.majortype, description->dsType.subtype, &(description->payloadType)))
		return E_FAIL;

	//get the endpoint data
	//description->server.ipAddress = "";
	//TODO get these values correct when they are actually dynamic
	description->server.isIPv6 = false;
	description->server.isMulticast = false;
	inputs[index]->GetPortAllocation(&(description->server.dataPort), &(description->server.controlPort));

	return S_OK;
}

STDMETHODIMP FCNetworkSinkFilter::AddDestination(int index, const FCRTPEndpoint * destination)
{
	if((index < -1) || (index >= inputs.size()))
	{
		return VFW_S_NO_MORE_ITEMS;
	}

	if(index == -1)
	{
		for(int i = 0; i < inputs.size(); i++)
		{
			if(!inputs[i]->AddDestination(destination))
			{
				return E_FAIL;
			}
		}
	}
	else
	{
		if(!inputs[index]->AddDestination(destination))
		{
			return E_FAIL;
		}
	}
	return S_OK;
}

STDMETHODIMP FCNetworkSinkFilter::RemoveDestination(int index, const FCRTPEndpoint * destination)
{
	if((index < -1) || (index >= inputs.size()))
	{
		return VFW_S_NO_MORE_ITEMS;
	}

	if(index == -1)
	{
		for(int i = 0; i < inputs.size(); i++)
		{
			//remove this endpoint from all, but we don't care if they weren't there or not
			inputs[i]->RemoveDestination(destination);
		}
	}
	else
	{
		if(!inputs[index]->RemoveDestination(destination))
		{
			return S_FALSE;
		}
	}
	return S_OK;
}

#pragma endregion

#pragma region Pin-Callable Methods

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

#pragma endregion