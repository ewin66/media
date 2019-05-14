#include "FCNSourcePin.h"
#include "FCNetworkSourceFilter.h"

FCNSourcePin::FCNSourcePin(LPCSTR pObjectName, CSource * pParent, HRESULT * phr, LPCWSTR pName)
	: CSourceStream(pObjectName, phr, pParent, pName),
	  input(NULL), inputLock(new CCritSec()),
	  senderIP(NULL), senderPort(0), isMulticast(false), senderType()
{
}

FCNSourcePin::~FCNSourcePin(void)
{
	if(input)
	{
		delete input;
	}
	if(inputLock)
	{
		delete inputLock;
	}
}

bool FCNSourcePin::ConnectToSender(BSTR serverIP, tpport_t dataPort, PayloadType payloadType)
{
	//TODO spin off another thread to do this?

	CAutoLock lock(inputLock);

	DisconnectFromSender();

	this->senderIP = ConvertBSTRToString(serverIP);
	this->senderPort = dataPort;
	this->isMulticast = false;
	this->senderType = payloadType;

	if(this->senderIP)
	{
		return DoSenderConnection();
	}
	return false;
}

bool FCNSourcePin::DoSenderConnection()
{
	if((input) || (!senderIP))
	{
		return false;
	}

	try
	{
		//TODO select local port
		input = new FCNSourceRTPSession(InetHostAddress(senderIP), senderPort, 5002);
	}
	catch(ost::SockException sockEx)
	{
		OutputDebugString(L"FCNSourcePin::DoSenderConnection -> ccrtp threw SockException");
		OutputDebugStringA(sockEx.getString());
		OutputDebugStringA(sockEx.getSystemErrorString());
		return false;
	}
	catch(ost::Socket * failedSocket)
	{
		OutputDebugString(L"FCNSourcePin::DoSenderConnection -> ccrtp threw pointer to Socket");
		char buffer[256];
		sprintf(buffer, "Socket error: %d, sys error: %d", failedSocket->getErrorNumber(), failedSocket->getSystemError());
		OutputDebugStringA(buffer);
		if(failedSocket->isActive())
		{
			OutputDebugString(L"Failed socket isActive");
		}
		else
		{
			OutputDebugString(L"Failed socket is NOT Active");
		}
		return false;
	}
	catch(...)
	{
		OutputDebugString(L"FCNSourcePin::DoSenderConnection - create RTP Session -> Unknown error occurred");
		return false;
	}
	

	if(input)
	{
		PayloadFormatter * p = PayloadFormatterFactory::Create(&(((FCNetworkSourceFilter*)this->m_pFilter)->payloadMap), this->senderType);
		if(p != NULL)
		{
			input->SetPayloadFormat(p);
			return true;
		}
	}

	return false;
}

void FCNSourcePin::DisconnectFromSender()
{
	if(input)
	{
		delete input;
		input = NULL;
	}
}

//transition to Stopped
HRESULT FCNSourcePin::Inactive()
{
	HRESULT hr = CSourceStream::Inactive();
	if(SUCCEEDED(hr))
	{
		CAutoLock stateLock(m_pFilter->pStateLock());
		CAutoLock lock(inputLock);

		DisconnectFromSender();
	}
	return hr;
}

//transition from Stopped to Paused
HRESULT FCNSourcePin::Active()
{
	//TODO -- on Run, maybe we will flush the buffer, keep up latency? or not to keep up reliability
	//TODO -- or other option, actually perform connect at this point

	CAutoLock stateLock(m_pFilter->pStateLock());
	CAutoLock lock(inputLock);

	if(!input)
	{
		if(!DoSenderConnection())
		{
			return E_FAIL;
		}
	}

	if(!input->Start())
	{
		return E_FAIL;
	}

	return CSourceStream::Active();
}

HRESULT FCNSourcePin::DecideBufferSize(IMemAllocator * pAlloc, ALLOCATOR_PROPERTIES * pProperties)
{
	CheckPointer(pAlloc, E_POINTER);
	CheckPointer(pProperties, E_POINTER);

	CAutoLock lock(m_pFilter->pStateLock());
	//TODO figure out what its going to take to push the data we're receiving

    HRESULT hr = NOERROR;

	VIDEOINFO *pvi = (VIDEOINFO *) m_mt.Format();
    pProperties->cBuffers = 2;
    pProperties->cbBuffer = MAX_BUFSIZE;

    ASSERT(pProperties->cbBuffer);

    // Ask the allocator to reserve us some sample memory, NOTE the function
    // can succeed (that is return NOERROR) but still not have allocated the
    // memory that we requested, so we must check we got whatever we wanted

    ALLOCATOR_PROPERTIES Actual;
    hr = pAlloc->SetProperties(pProperties, &Actual);
    if (FAILED(hr))
	{
        return hr;
    }

    if(Actual.cbBuffer < pProperties->cbBuffer)
    {
        return E_FAIL;
    }

	return S_OK;
}

HRESULT FCNSourcePin::FillBuffer(IMediaSample * pSample)
{
	CAutoLock lock(inputLock);

	if(input)
	{
		DSPayload payload;
		if(!WaitForPayload(&payload))
		{
			return S_FALSE;
		}

		//set timestamps
		HRESULT hr = pSample->SetTime(&(payload.Timestamp), NULL);
		if(FAILED(hr)) return hr;

		BYTE * pData;
		pSample->GetPointer(&pData);
		if(FAILED(hr)) return hr;
		long lDataLen = pSample->GetSize();
		ZeroMemory(pData, lDataLen);

		//copy data to sample
		memcpy(pData, payload.Data, payload.Length);

		if((payload.ReleaseData) && (payload.Data))
		{
			delete [] payload.Data;
		}

		//set actual data length
		hr = pSample->SetActualDataLength(payload.Length);
		if(FAILED(hr)) return hr;

		return S_OK;
	}

	return S_FALSE;
}

// blocks until a payload is delivered
bool FCNSourcePin::WaitForPayload(DSPayload * outPayload)
{
	//input->ProcessWaitingData();
	//get something to deliver!
	while(!input->Dequeue(outPayload))
	{
		Command c;
		if(CheckRequest(&c))
		{
			if((c == CMD_STOP) || (c == CMD_EXIT))
			{
				return false;
			}
		}
		::Sleep(1);
		input->ProcessWaitingData();
	}
	return true;
}

HRESULT FCNSourcePin::CheckMediaType(const CMediaType * pmt)
{
	if(input)
	{
		PayloadType currentPT;
		PayloadMap * pmap = &((FCNetworkSourceFilter*)this->m_pFilter)->payloadMap;	

		if(input->GetPayloadType(&currentPT))
		{
			vector<pair<GUID,GUID>> curMappings = vector<pair<GUID,GUID>>();

			if(pmap->GetAllMappings(currentPT, &curMappings))
			{
				for(vector<pair<GUID,GUID>>::iterator i = curMappings.begin(); i != curMappings.end(); i++)
				{
					if((pmt->majortype == i->first) &&
						(pmt->subtype == i->second))
					{
						return S_OK;
					}
				}
			}
		}
	}
	return E_FAIL;
}

HRESULT FCNSourcePin::GetMediaType(int iPosition, CMediaType * pMediaType)
{
	if(iPosition < 0)
	{
		return E_INVALIDARG;
	}

	if(input)
	{
		PayloadMap * pmap = &((FCNetworkSourceFilter*)this->m_pFilter)->payloadMap;
		PayloadType pt;
		if(input->GetPayloadType(&pt))
		{
			vector<pair<GUID,GUID>> curMappings = vector<pair<GUID,GUID>>();
			if(pmap->GetAllMappings(pt, &curMappings))
			{
				if(iPosition >= curMappings.size())
				{
					return VFW_S_NO_MORE_ITEMS;
				}

				pMediaType->SetType(&curMappings[iPosition].first);
				pMediaType->SetSubtype(&curMappings[iPosition].second);
				pMediaType->SetSampleSize(MAX_BUFSIZE);

				//HACK all of this format info needs to be sent via SessionDescription
				pMediaType->SetFormatType(&FORMAT_VideoInfo);
				pMediaType->SetTemporalCompression(FALSE);
				VIDEOINFO * pvi = (VIDEOINFO*)pMediaType->AllocFormatBuffer(sizeof(VIDEOINFO));
				if(!pvi)
				{
					return E_OUTOFMEMORY;
				}
				ZeroMemory(pvi, sizeof(VIDEOINFO));

				//pvi->bmiHeader.biCompression = MAKEFOURCC('H','2','6','4');
				pvi->bmiHeader.biBitCount    = 16;
				pvi->bmiHeader.biSize       = sizeof(BITMAPINFOHEADER);
				pvi->bmiHeader.biWidth      = 640;//m_iImageWidth;
				pvi->bmiHeader.biHeight     = 480;//m_iImageHeight;
				pvi->bmiHeader.biPlanes     = 1;
				pvi->bmiHeader.biSizeImage  = GetBitmapSize(&pvi->bmiHeader);
				pvi->bmiHeader.biClrImportant = 0;

				SetRectEmpty(&(pvi->rcSource));
				SetRectEmpty(&(pvi->rcTarget));

				return S_OK;
			}
		}
	}
	return S_FALSE;
}

STDMETHODIMP FCNSourcePin::Notify(IBaseFilter *pSender, Quality q)
{
	//TODO react appropriately
	return E_NOTIMPL;
}