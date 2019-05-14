#include "FCNSinkPin.h"
#include "FCNetworkSinkFilter.h"
#include "../FCRTPLib/FCRTPLib.h"

FCNSinkPin::FCNSinkPin(LPCSTR pObjectName, CBaseFilter * pParent, CCritSec * pLock, HRESULT * phr, LPCWSTR pName)
	: CRenderedInputPin(pObjectName, pParent, pLock, phr, pName),
	  output(NULL),
	  outputMutex(new CCritSec()),
	  receiveDataPort(0),
	  receiveControlPort(0)
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

void FCNSinkPin::SetPortAllocation(tpport_t receiveDataPort, tpport_t receiveControlPort)
{
	if(output == NULL)
	{
		this->receiveDataPort = receiveDataPort;
		this->receiveControlPort = receiveControlPort;
	}
}

/*
	Returns true if the ports have been bound to, false if they are just indicated
*/
bool FCNSinkPin::GetPortAllocation(tpport_t * receiveDataPort, tpport_t * receiveControlPort)
{
	(*receiveDataPort) = this->receiveDataPort;
	(*receiveControlPort) = this->receiveControlPort;

	if(output)
	{
		return true;
	}
	else
	{
		return false;
	}
}

bool FCNSinkPin::AddDestination(const FCRTPEndpoint * client)
{
	if(output == NULL)
	{
		return false;
	}
	else
	{
		return output->AddDestination(client);
	}
}

bool FCNSinkPin::RemoveDestination(const FCRTPEndpoint * client)
{
	if(output == NULL)
	{
		return false;
	}
	else
	{
		return output->RemoveDestination(client);
	}
}

HRESULT FCNSinkPin::CreateOutput()
{
	ASSERT(output == NULL);

	if(receiveControlPort == 0)
	{
		receiveControlPort = receiveDataPort + 1;
	}
	output = new FCNSinkRTPSession(receiveDataPort, receiveControlPort);

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
	PayloadFormatter * p = PayloadFormatterFactory::Create(&(((FCNetworkSinkFilter*)this->m_pFilter)->configuredPayloads), m_mt.majortype, m_mt.subtype);

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
		FCNetworkSinkFilter * filter = (FCNetworkSinkFilter *)(this->m_pFilter);

		AM_MEDIA_TYPE connection;
		if(SUCCEEDED(this->ConnectionMediaType(&connection)))
		{
			//this bit of code is slick. The supportedPayloads has all dynamic payload types mapped to
			//  ptINVALID, and the configuredPayloads, when you add a new mapping of type ptINVALID, will
			//  interpret it as a request to assign a PayloadType for a dynamic type.
			PayloadType suppType, confType;
			bool isKnownPayload = filter->supportedPayloads->GetMapping(connection.majortype, connection.subtype, &suppType);
			bool isConfiguredPayload = filter->configuredPayloads.GetMapping(connection.majortype, connection.subtype, &confType);
			
			//check if we both support the mapping, and have not already added the mapping
			if (isKnownPayload && !isConfiguredPayload)
			{
				//if supported and not added, then we add the mapping
				confType = filter->configuredPayloads.AddMapping(suppType, connection.majortype, connection.subtype);
			}

			//this means that we have exhausted all dynamic mapping space
			if((!isConfiguredPayload) && (confType == ptINVALID))
			{
				return VFW_E_ENUM_OUT_OF_RANGE;
			}
			
			FreeMediaType(connection);
		}

		filter->SpawnInputPin();
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

	//commented out to allow any media type

	PayloadType temp;
	if(((FCNetworkSinkFilter*)this->m_pFilter)->supportedPayloads->GetMapping(pmt->majortype, pmt->subtype, &temp))
	{
		return S_OK;
	}

	return S_FALSE;
}

HRESULT FCNSinkPin::GetMediaType(int iPosition, CMediaType * pMediaType)
{
	PayloadMap * pmap = ((FCNetworkSinkFilter*)this->m_pFilter)->supportedPayloads;

	if(iPosition < 0)
	{
		return E_INVALIDARG;
	}

	CheckPointer(pMediaType, E_POINTER);

	if((size_t)iPosition >= pmap->Count())
	{
		return VFW_S_NO_MORE_ITEMS;
	}

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