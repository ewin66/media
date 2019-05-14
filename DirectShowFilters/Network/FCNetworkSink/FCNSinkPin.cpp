#include "FCNSinkPin.h"
#include "FCNetworkSinkFilter.h"
#include "../FCRTPLib/FCRTPLib.h"

FCNSinkPin::FCNSinkPin(LPCSTR pObjectName, CBaseFilter * pParent, CCritSec * pLock, HRESULT * phr, LPCWSTR pName)
	: CRenderedInputPin(pObjectName, pParent, pLock, phr, pName),
	  output(NULL),
	  outputMutex(new CCritSec())
{
	if(phr)
	{
		(*phr) = S_OK;
	}
}

FCNSinkPin::~FCNSinkPin(void)
{
	delete outputMutex;
	DeleteOutput();
}

HRESULT FCNSinkPin::CreateOutput()
{
	ASSERT(output == NULL);

	output = new FCNSinkRTPSession(5000);	//TODO figure out how to select port numbers
	if(output == NULL)
	{
		return E_FAIL;
	}

	if(!SetPayloadFormatter())
	{
		return E_FAIL;
	}
	
	return S_OK;
}

bool FCNSinkPin::SetPayloadFormatter()
{
	PayloadFormatter * p = PayloadFormatterFactory::Create(&(((FCNetworkSinkFilter*)this->m_pFilter)->payloadMap), m_mt.majortype, m_mt.subtype);

	if(p == NULL)
	{
		return false;
	}
	else
	{
		output->SetPayloadFormat(p);
		return true;
	}
}

void FCNSinkPin::DeleteOutput()
{
	if(output != NULL)
	{
		delete output;
		output = NULL;
	}
}

HRESULT FCNSinkPin::CompleteConnect(IPin * pReceivePin)
{
	if(this->m_pFilter)
	{
		((FCNetworkSinkFilter*)(this->m_pFilter))->SpawnInputPin();
	}

	return S_OK;
}


HRESULT FCNSinkPin::BreakConnect()
{
	CAutoLock stateLock(m_pLock);
	CAutoLock lock(outputMutex);

	HRESULT hr = CRenderedInputPin::BreakConnect();
	if(SUCCEEDED(hr))
	{
		DeleteOutput();
	}
	return hr;
}

STDMETHODIMP FCNSinkPin::Disconnect()
{
	HRESULT hr = CRenderedInputPin::Disconnect();
	if(SUCCEEDED(hr))
	{
		if(this->m_pFilter)
		{
			((FCNetworkSinkFilter*)(this->m_pFilter))->InputPinDisconnected(this);
		}
	}
	return hr;
}

STDMETHODIMP FCNSinkPin::EndOfStream()
{
	CAutoLock lock(outputMutex);
	if(output)
	{
		output->EndOfStream();
	}
	return CRenderedInputPin::EndOfStream();
}

STDMETHODIMP FCNSinkPin::Receive(IMediaSample * pSample)
{
	CAutoLock lock(outputMutex);

	long length = pSample->GetActualDataLength();

	//fetch pointer to data
	BYTE * pData;
	HRESULT hr = pSample->GetPointer(&pData);
	if(hr != S_OK)
	{
		return hr;
	}

	//fetch timestamps
	REFERENCE_TIME start, stop;
	hr = pSample->GetTime(&start, &stop);
	if(FAILED(hr))
	{
		return hr;
	}

	if(output)
	{
		output->Enqueue(start, (const BYTE*)pData, length);
	}

	pSample->Release();

	return S_OK;
}

HRESULT FCNSinkPin::CheckMediaType(const CMediaType * pmt)
{
	CheckPointer(pmt, E_POINTER);

	PayloadType temp;
	if(((FCNetworkSinkFilter*)this->m_pFilter)->payloadMap.GetMapping(pmt->majortype, pmt->subtype, &temp))
	{
		return S_OK;
	}

	return S_FALSE;
}

HRESULT FCNSinkPin::GetMediaType(int iPosition, CMediaType * pMediaType)
{
	PayloadMap * pmap = &((FCNetworkSinkFilter*)this->m_pFilter)->payloadMap;

	if(iPosition < 0)
	{
		return E_INVALIDARG;
	}
	if(iPosition >= pmap->Count())
	{
		return VFW_S_NO_MORE_ITEMS;
	}

	CheckPointer(pMediaType, E_POINTER);

	GUID major, sub;
	if(pmap->GetMappingAt(iPosition, &major, &sub))
	{
		pMediaType->SetType(&major);
		pMediaType->SetSubtype(&sub);

		return S_OK;
	}
	else
	{
		return S_FALSE;
	}
}

//transition to Stopped
HRESULT FCNSinkPin::Inactive()
{
	CAutoLock stateLock(m_pLock);
	CAutoLock lock(outputMutex);
	DeleteOutput();
	return S_OK;
}

//transition from Stopped to Paused
HRESULT FCNSinkPin::Active()
{
	CAutoLock stateLock(m_pLock);
	return S_OK;
}

//transistion to Run
HRESULT FCNSinkPin::Run(REFERENCE_TIME tStart)
{
	CAutoLock stateLock(m_pLock);
	CAutoLock lock(outputMutex);

	if(!IsConnected())
	{
		return S_OK;
	}

	if(output == NULL)
	{
		if(FAILED(CreateOutput()))
		{
			return E_FAIL;
		}
	}

	output->SetInitialTimestamp(tStart);
	if(!output->Start())
	{
		return E_FAIL;
	}

	return S_OK;
}