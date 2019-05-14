#include "H264AnnexBtoNALU.h"

H264AnnexBtoNALU::H264AnnexBtoNALU(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr)
	:CTransformFilter(tszName, punk, CLSID_H264AnnexBtoNALU)
{
}

H264AnnexBtoNALU::~H264AnnexBtoNALU(void)
{
}

CUnknown * H264AnnexBtoNALU::CreateInstance(LPUNKNOWN punk, HRESULT *phr)
{
    ASSERT(phr);
    
    H264AnnexBtoNALU * pNewObject = new H264AnnexBtoNALU(NAME("H.264 AnnexB to NALU"), punk, phr);

    if (pNewObject == NULL)
	{
        if (phr)
		{
            *phr = E_OUTOFMEMORY;
		}
    }
    return pNewObject;

}

HRESULT H264AnnexBtoNALU::Transform(IMediaSample * pIn, IMediaSample * pOut)
{
	CheckPointer(pIn, E_POINTER);
	CheckPointer(pOut, E_POINTER);

	//AnnexB will travel in clumps of NALU all with the same time stamp

	REFERENCE_TIME startTime, endTime;
	HRESULT hr = pIn->GetTime(&startTime, &endTime);
	if(FAILED(hr))
	{
		return hr;
	}

	long totalSize = pIn->GetActualDataLength();
	BYTE * data = NULL;
	hr = pIn->GetPointer(&data);
	if(hr != S_OK)
	{
		return hr;
	}


	//break up all NALU and deliver them first
	long startIndex = 0;
	long endIndex = 0;
	while(GetNextNALURange(data, totalSize, &startIndex, &endIndex))
	{
		hr = DeliverRange(data, startIndex, endIndex, startTime);
		if(FAILED(hr))
		{
			return hr;
		}
		else
		{
			startIndex = endIndex;	//advance the startIndex so we don't waste time re-parseing the same block
		}
	}

	//do the last NALU
	PopulateOutputSample(&pOut, data, startIndex, endIndex, startTime);

	return S_OK;
}

/*
	Determines the next range of indicies (inclusive) that delimits a NAL Unit
	data		[in]	array of bytes we're parsing
	dataLength	[in]	length of the data array
	startIndex	[in]	position within data to start searching
				[out]	beginning index of NALU (inclusive)
	endIndex	[out]	ending index of NALU (inclusive)

	Returns true if both startIndex and endIndex are determined by parsing
	Returns false if the endIndex is the last index in the data array
*/
bool H264AnnexBtoNALU::GetNextNALURange(const BYTE * data, const long & dataLength, long * startIndex, long * endIndex)
{
	(*endIndex) = -1;
	for(long s = (*startIndex); s < dataLength - 4; s++)
	{
		if(IsAnnexBPrefix(&data[s]))
		{
			(*startIndex) = s + 4;
			for(long e = s + 4; e < dataLength - 4; e++)
			{
				if(IsAnnexBPrefix(&data[e]))
				{
					(*endIndex) = e - 1;
					return true;
				}
			}
			if((*endIndex) == -1)
			{
				(*endIndex) = dataLength - 1;
			}
		}
	}

	if((*endIndex) == -1)
	{
		(*endIndex) = dataLength - 1;
	}

	return false;
}

/*
	Returns true if the first four elements of this array form an Annex B Start Code
	See ITU Recommendation H.264, Annex B, Part 2  (pp. 291)
*/
bool H264AnnexBtoNALU::IsAnnexBPrefix(const BYTE * data)
{
	return ((data[0] == 0x00) &&
			(data[1] == 0x00) &&
			(data[2] == 0x00) &&
			(data[3] == 0x01));
}

/*
	Does the work of delivering a range of data downstream, as a distinct sample
	data		[in]	the data to extract from
	startIndex	[in]	the beginning index in data (inclusive) to send downstream
	endIndex	[in]	the ending index in data (inclusive) to send downstream
	startTime	[in]	REFERENCE_TIME at which the generated sample starts at
*/
HRESULT H264AnnexBtoNALU::DeliverRange(const BYTE * data, const long & startIndex, const long & endIndex, REFERENCE_TIME & startTime)
{
	//fetch a sample to write to from the output pin
	IMediaSample * pOutSample = NULL;
	HRESULT hr = this->m_pOutput->GetDeliveryBuffer(&pOutSample, &startTime, NULL, 0);
	if(FAILED(hr))
	{
		return hr;
	}

	//populate the sample to output with data and time stamp it properly
	hr = PopulateOutputSample(&pOutSample, data, startIndex, endIndex, startTime);
	if(FAILED(hr))
	{
		return hr;
	}

	//deliver the sample downstream
	hr = this->m_pOutput->Deliver(pOutSample);
	if(FAILED(hr))
	{
		return hr;
	}

	//release this sample, because the downstream filter has added a refcount to it.
	pOutSample->Release();

	return S_OK;
}

/*
	Populates a sample for output
	ppOutSample	[in][out]	a valid IMediaSample to be populated
	data		[in]		data to populate from
	startIndex	[in]		the beginning index in data (inclusive) to copy to the output sample
	endIndex	[in]		the ending index in data (inclusive) to copy to the output sample
	startTime	[in]		the REFERENCE_TIME when this sample begins
*/
HRESULT H264AnnexBtoNALU::PopulateOutputSample(IMediaSample ** ppOutSample,
											   const BYTE * data,
											   const long & startIndex,
											   const long & endIndex,
											   REFERENCE_TIME & startTime)
{
	BYTE * outBuffer = NULL;
	HRESULT hr = (*ppOutSample)->GetPointer(&outBuffer);
	long bufferLen = (*ppOutSample)->GetSize();
	if((endIndex - startIndex + 1) > bufferLen)
	{
		return E_OUTOFMEMORY;
	}

	memcpy(outBuffer, &data[startIndex], endIndex - startIndex + 1);

	hr = (*ppOutSample)->SetActualDataLength(endIndex - startIndex + 1);
	if(FAILED(hr))
	{
		return hr;
	}

	hr = (*ppOutSample)->SetTime(&startTime, NULL);
	if(FAILED(hr))
	{
		return hr;
	}

	return S_OK;
}


HRESULT H264AnnexBtoNALU::DecideBufferSize(IMemAllocator * pAllocator, ALLOCATOR_PROPERTIES * pprop)
{
   // Is the input pin connected

    if (m_pInput->IsConnected() == FALSE)
	{
        return E_UNEXPECTED;
    }

    CheckPointer(pAllocator, E_POINTER);
    CheckPointer(pprop, E_POINTER);
    HRESULT hr = NOERROR;

    pprop->cBuffers = 3;
    pprop->cbBuffer = MAX_BUFSIZ;
    ASSERT(pprop->cbBuffer);

    // Ask the allocator to reserve us some sample memory, NOTE the function
    // can succeed (that is return NOERROR) but still not have allocated the
    // memory that we requested, so we must check we got whatever we wanted

    ALLOCATOR_PROPERTIES Actual;
    hr = pAllocator->SetProperties(pprop, &Actual);
    if (FAILED(hr))
	{
        return hr;
    }

    if (pprop->cBuffers > Actual.cBuffers || pprop->cbBuffer > Actual.cbBuffer)
	{
        return E_FAIL;
    }
    return NOERROR;
}

HRESULT H264AnnexBtoNALU::CheckInputType(const CMediaType *mtIn)
{
	//TODO make more restrictive

    CheckPointer(mtIn,E_POINTER);

    // check this is a VIDEOINFOHEADER type

    if (*mtIn->FormatType() != FORMAT_VideoInfo)
	{
        return E_INVALIDARG;
    }

    return NOERROR;
}

HRESULT H264AnnexBtoNALU::CheckTransform(const CMediaType *mtIn, const CMediaType *mtOut)
{
    CheckPointer(mtIn,E_POINTER);
    CheckPointer(mtOut,E_POINTER);

    return NOERROR;
  
}


HRESULT H264AnnexBtoNALU::GetMediaType(int iPosition, CMediaType *pmt)
{
	if (iPosition > 2)
	{
        return VFW_S_NO_MORE_ITEMS;
    }
	VIDEOINFO* pSourceVideoInfo = (VIDEOINFO*)m_pInput->CurrentMediaType().pbFormat;

    VIDEOINFO *pvi = (VIDEOINFO *) pmt->AllocFormatBuffer(sizeof(VIDEOINFO));
	if(NULL == pvi)
	{
		return(E_OUTOFMEMORY);
	}

	ZeroMemory(pvi, sizeof(VIDEOINFO));

	pvi->bmiHeader.biCompression = MAKEFOURCC('a','v','c','1');
	pvi->bmiHeader.biBitCount    = 24;

	// (Adjust the parameters common to all formats...)

	pvi->bmiHeader.biSize       = sizeof(BITMAPINFOHEADER);
	if(pSourceVideoInfo == NULL)
	{
		pvi->bmiHeader.biWidth = 0;
		pvi->bmiHeader.biHeight = 0;
	}
	else
	{
		pvi->bmiHeader.biWidth      = pSourceVideoInfo->bmiHeader.biWidth;
		pvi->bmiHeader.biHeight     = pSourceVideoInfo->bmiHeader.biHeight;
	}
	pvi->bmiHeader.biPlanes     = 1;
	pvi->bmiHeader.biSizeImage  = GetBitmapSize(&pvi->bmiHeader);
	pvi->bmiHeader.biClrImportant = 0;

	SetRectEmpty(&(pvi->rcSource)); // we want the whole image area rendered.
	SetRectEmpty(&(pvi->rcTarget)); // no particular destination rectangle
	pmt->SetType(&MEDIATYPE_Video);
	pmt->SetFormatType(&FORMAT_VideoInfo);
	pmt->SetTemporalCompression(FALSE);

	static GUID MEDIASUBTYPE_LT_avc1 = { MAKEFOURCC('a','v','c','1'), 0x5349, 0x4D4F, 0x45, 0x44, 0x49, 0x41, 0x54, 0x59, 0x50, 0x45 };
	static GUID MEDIASUBTYPE_avc1 = { MAKEFOURCC('a', 'v', 'c', '1'), 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71 };
	static GUID MEDIASUBTYPE_AVC1 = { MAKEFOURCC('A', 'V', 'C', '1'), 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71 };

	//TODO these are input types
	// Work out the GUID for the subtype from the header info.
//	static GUID MEDIASUBTYPE_H264_MS_ASF = { MAKEFOURCC('h','2','6','4'), 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71};
//	static GUID MEDIASUBTYPE_H264_X264 = { MAKEFOURCC('X','2','6','4'), 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71};
//	static GUID MEDIASUBTYPE_H264 = { 0x8d2d71cb, 0x243f, 0x45e3, 0xb2, 0xd8, 0x5f, 0xd7, 0x96, 0x7e, 0xc0, 0x9b};
	if (iPosition == 0)
	{
		pmt->SetSubtype(&MEDIASUBTYPE_LT_avc1);
	}
	else if (iPosition == 1)
	{
		pmt->SetSubtype(&MEDIASUBTYPE_avc1);
	}
	else if (iPosition == 2)
	{
		pmt->SetSubtype(&MEDIASUBTYPE_AVC1);
	}
	pmt->SetSampleSize(MAX_BUFSIZ);

	return NOERROR;
}

HRESULT H264AnnexBtoNALU::StartStreaming()
{
	OutputDebugStringW(L"*** H264 AnnexB to NALU StartStreaming ***");
//	m_frameCount = 0;
//	m_alterQualityCount = 0;
//	m_currentTime = 0;
//	m_bufsiz = 0;
//	m_hasSyncPoint = FALSE;
	return CTransformFilter::StartStreaming();
}

HRESULT H264AnnexBtoNALU::StopStreaming()
{
	OutputDebugStringW(L"*** H264 AnnexB to NALU StopStreaming ***");
//	if (m_alterQualityCount > 0)
//	{
//		wchar_t buf[256];
//		swprintf(buf, L"%d AlterQuality messages sent--timing issues with stream!!", m_alterQualityCount);
//		OutputDebugStringW(buf);
//	}
	return CTransformFilter::StopStreaming();
}

HRESULT H264AnnexBtoNALU::AlterQuality(Quality q)
{
	//m_alterQualityCount++;
	return CTransformFilter::AlterQuality(q);
}