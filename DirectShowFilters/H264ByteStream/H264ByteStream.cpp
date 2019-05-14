//------------------------------------------------------------------------------
// File: H264ByteStream.cpp
//
//------------------------------------------------------------------------------

#include <windows.h>
#include <stdio.h>
#include <streams.h>
#include <initguid.h>

#if (1100 > _MSC_VER)
#include <olectlid.h>
#else
#include <olectl.h>
#endif

#include "guids.h"
#include "H264ByteStream.h"

// Setup information

const AMOVIESETUP_MEDIATYPE sudPinTypes =
{
    &MEDIATYPE_Video,       // Major type
    &MEDIASUBTYPE_NULL      // Minor type
};

const AMOVIESETUP_PIN sudpPins[] =
{
    { L"Input",             // Pins string name
      FALSE,                // Is it rendered
      FALSE,                // Is it an output
      FALSE,                // Are we allowed none
      FALSE,                // And allowed many
      &CLSID_NULL,          // Connects to filter
      NULL,                 // Connects to pin
      1,                    // Number of types
      &sudPinTypes          // Pin information
    },
    { L"Output",            // Pins string name
      FALSE,                // Is it rendered
      TRUE,                 // Is it an output
      FALSE,                // Are we allowed none
      FALSE,                // And allowed many
      &CLSID_NULL,          // Connects to filter
      NULL,                 // Connects to pin
      1,                    // Number of types
      &sudPinTypes          // Pin information
    }
};

const AMOVIESETUP_FILTER sudH264ByteStream =
{
    &CLSID_H264ByteStream,         // Filter CLSID
    L"H.264 Byte Stream Transform",       // String name
    MERIT_DO_NOT_USE,       // Filter merit
    2,                      // Number of pins
    sudpPins                // Pin information
};


// List of class IDs and creator functions for the class factory. This
// provides the link between the OLE entry point in the DLL and an object
// being created. The class factory will call the static CreateInstance

CFactoryTemplate g_Templates[] = {
    { L"H.264 Byte Stream Transform"
    , &CLSID_H264ByteStream
    , CH264ByteStream::CreateInstance
    , NULL
    , &sudH264ByteStream }
};
int g_cTemplates = sizeof(g_Templates) / sizeof(g_Templates[0]);


////////////////////////////////////////////////////////////////////////
//
// Exported entry points for registration and unregistration 
// (in this case they only call through to default implementations).
//
////////////////////////////////////////////////////////////////////////

//
// DllRegisterServer
//
// Handles sample registry and unregistry
//
STDAPI DllRegisterServer()
{
    return AMovieDllRegisterServer2( TRUE );

} // DllRegisterServer


//
// DllUnregisterServer
//
STDAPI DllUnregisterServer()
{
    return AMovieDllRegisterServer2( FALSE );

} // DllUnregisterServer


//
// DllEntryPoint
//
extern "C" BOOL WINAPI DllEntryPoint(HINSTANCE, ULONG, LPVOID);

BOOL APIENTRY DllMain(HANDLE hModule, 
                      DWORD  dwReason, 
                      LPVOID lpReserved)
{
	return DllEntryPoint((HINSTANCE)(hModule), dwReason, lpReserved);
}


//
// Constructor
//
CH264ByteStream::CH264ByteStream(TCHAR *tszName,
                   LPUNKNOWN punk,
                   HRESULT *phr) :
    CTransformFilter(tszName, punk, CLSID_H264ByteStream)
{
	char* debugFilename = getenv("AntaresX_Video_H264_BYTE_STREAM_DEBUG_FILE");
	if (debugFilename != NULL)
	{
		m_fp = fopen(debugFilename, "w");
	}
	else
	{
		m_fp = NULL;
	}
} // (Constructor)

//
// CreateInstance
//
// Provide the way for COM to create a H264ByteStream object
//
CUnknown *CH264ByteStream::CreateInstance(LPUNKNOWN punk, HRESULT *phr)
{
    ASSERT(phr);
    
    CH264ByteStream *pNewObject = new CH264ByteStream(NAME("H.264 Byte Stream Transform"), punk, phr);

    if (pNewObject == NULL)
	{
        if (phr)
		{
            *phr = E_OUTOFMEMORY;
		}
    }
    return pNewObject;

} // CreateInstance

//
// Transform
//
//
HRESULT CH264ByteStream::Transform(IMediaSample *pIn, IMediaSample *pOut)
{
	HRESULT hr = S_OK;
	REFERENCE_TIME timeStart;
	REFERENCE_TIME timeEnd;
    BYTE *pInBuffer;
    long lInSize;

	CheckPointer(pIn, E_POINTER);   
    CheckPointer(pOut, E_POINTER);

	m_frameCount++;
    lInSize = pIn->GetActualDataLength();
	hr = pIn->GetPointer(&pInBuffer);
	if (hr != S_OK)
	{
		return hr;
	}
	hr = pIn->GetTime(&timeStart, &timeEnd);
	if (timeStart != m_currentTime)
	{
		if (timeStart < m_currentTime)
		{
			char buf[256];
			sprintf(buf, "H.264 sample time error - timeStart(%I64u) < currentTime(%I64u)\n", timeStart, m_currentTime);
			OutputDebugStringA(buf);
		}
		if (m_bufsiz != 0)
		{
			BYTE* pOutBuffer;
			hr = pOut->GetPointer(&pOutBuffer);
			if (hr != S_OK)
			{
				return hr;
			}
			CopyMemory(pOutBuffer, m_buffer, m_bufsiz);
			pOut->SetActualDataLength(m_bufsiz);
			hr = pOut->SetTime(&m_currentTime, &timeStart);
			if (hr != S_OK)
			{
				return hr;
			}
			m_currentTime = timeStart;
			pOut->SetSyncPoint(m_hasSyncPoint);
			pOut->SetDiscontinuity(FALSE);
			pOut->SetPreroll(FALSE);
			m_hasSyncPoint = FALSE;
			m_bufsiz = 0;
		}
	}
	else
	{
		hr = S_FALSE;
	}
	if (pInBuffer[0] == 0x65)
	{
		m_hasSyncPoint = TRUE;
	}
	AppendPrefix();
	AppendSample(pIn);
	return hr;
} // Transform

HRESULT CH264ByteStream::StartStreaming()
{
	OutputDebugStringW(L"*** H264ByteStream (Annex B) StartStreaming ***");
	m_frameCount = 0;
	m_alterQualityCount = 0;
	m_currentTime = 0;
	m_bufsiz = 0;
	m_hasSyncPoint = FALSE;
	return CTransformFilter::StartStreaming();
}

HRESULT CH264ByteStream::StopStreaming()
{
	OutputDebugStringW(L"*** H264ByteStream (Annex B) StopStreaming ***");
	if (m_alterQualityCount > 0)
	{
		wchar_t buf[256];
		swprintf(buf, 256, L"%d AlterQuality messages sent--timing issues with stream!!", m_alterQualityCount);
		OutputDebugStringW(buf);
	}
	return CTransformFilter::StopStreaming();
}

HRESULT CH264ByteStream::AlterQuality(Quality q)
{
	m_alterQualityCount++;
	return CTransformFilter::AlterQuality(q);
}

// Check the input type is OK - return an error otherwise

HRESULT CH264ByteStream::CheckInputType(const CMediaType *mtIn)
{
    CheckPointer(mtIn,E_POINTER);

    // check this is a VIDEOINFOHEADER type

    if (*mtIn->FormatType() != FORMAT_VideoInfo)
	{
        return E_INVALIDARG;
    }

    return NOERROR;
}

//
// Checktransform
//
// Check a transform can be done between these formats
//
HRESULT CH264ByteStream::CheckTransform(const CMediaType *mtIn, const CMediaType *mtOut)
{
    CheckPointer(mtIn,E_POINTER);
    CheckPointer(mtOut,E_POINTER);

    return NOERROR;
  
} // CheckTransform

//
// DecideBufferSize
//
// Tell the output pin's allocator what size buffers we
// require. Can only do this when the input is connected
//
HRESULT CH264ByteStream::DecideBufferSize(IMemAllocator *pAlloc,ALLOCATOR_PROPERTIES *pProperties)
{
    // Is the input pin connected

    if (m_pInput->IsConnected() == FALSE)
	{
        return E_UNEXPECTED;
    }

    CheckPointer(pAlloc,E_POINTER);
    CheckPointer(pProperties,E_POINTER);
    HRESULT hr = NOERROR;

    pProperties->cBuffers = 3;
    pProperties->cbBuffer = MAX_BUFSIZ;
    ASSERT(pProperties->cbBuffer);

    // Ask the allocator to reserve us some sample memory, NOTE the function
    // can succeed (that is return NOERROR) but still not have allocated the
    // memory that we requested, so we must check we got whatever we wanted

    ALLOCATOR_PROPERTIES Actual;
    hr = pAlloc->SetProperties(pProperties,&Actual);
    if (FAILED(hr))
	{
        return hr;
    }

    if (pProperties->cBuffers > Actual.cBuffers || pProperties->cbBuffer > Actual.cbBuffer)
	{
        return E_FAIL;
    }
    return NOERROR;

} // DecideBufferSize


//
// GetMediaType
//
//
HRESULT CH264ByteStream::GetMediaType(int iPosition, CMediaType *pmt)
{
	if (iPosition > 1)
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

	pvi->bmiHeader.biCompression = MAKEFOURCC('H','2','6','4');
	pvi->bmiHeader.biBitCount    = 24;

	// (Adjust the parameters common to all formats...)

	pvi->bmiHeader.biSize       = sizeof(BITMAPINFOHEADER);
	pvi->bmiHeader.biWidth      = pSourceVideoInfo->bmiHeader.biWidth;
	pvi->bmiHeader.biHeight     = pSourceVideoInfo->bmiHeader.biHeight;
	pvi->bmiHeader.biPlanes     = 1;
	pvi->bmiHeader.biSizeImage  = GetBitmapSize(&pvi->bmiHeader);
	pvi->bmiHeader.biClrImportant = 0;

	SetRectEmpty(&(pvi->rcSource)); // we want the whole image area rendered.
	SetRectEmpty(&(pvi->rcTarget)); // no particular destination rectangle
	pmt->SetType(&MEDIATYPE_Video);
	pmt->SetFormatType(&FORMAT_VideoInfo);
	pmt->SetTemporalCompression(FALSE);

	// Work out the GUID for the subtype from the header info.
	static GUID MEDIASUBTYPE_H264_MS_ASF = { MAKEFOURCC('h','2','6','4'), 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71};
	static GUID MEDIASUBTYPE_H264_X264 = { MAKEFOURCC('X','2','6','4'), 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71};
	static GUID MEDIASUBTYPE_H264 = { 0x8d2d71cb, 0x243f, 0x45e3, 0xb2, 0xd8, 0x5f, 0xd7, 0x96, 0x7e, 0xc0, 0x9b};
	if (iPosition == 0)
	{
		pmt->SetSubtype(&MEDIASUBTYPE_H264_MS_ASF);
	} else if (iPosition == 1)
	{
		pmt->SetSubtype(&MEDIASUBTYPE_H264);
	}
	pmt->SetSampleSize(MAX_BUFSIZ);

	return NOERROR;
} // GetMediaType
