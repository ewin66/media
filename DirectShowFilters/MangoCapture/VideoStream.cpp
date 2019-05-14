//------------------------------------------------------------------------------
// File: VideoStream.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

#pragma warning(disable:4710)  // 'function': function not inlined (optimzation)

//
// Constructor
//
VideoStream::VideoStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName) :
    MangoStream(phr, pParent, pPinName),
	m_iCam(0),
	m_iFrameRate(2),		//2frameskip = 15fps
	m_eImagesize(MX_CIF),
    m_iImageWidth(352),
    m_iImageHeight(240),
	m_pFramerate_filter(NULL),
	m_pFramerate2_filter(NULL),
	m_pVidin_filter(NULL)
{
} // (Constructor)

	//
// Destructor
//
VideoStream::~VideoStream()
{
}

//
// DecideBufferSize
//
// This will always be called after the format has been sucessfully
// negotiated. So we have a look at m_mt to see what size image we agreed.
// Then we can ask for buffers of the correct size to contain them.
//
HRESULT VideoStream::DecideBufferSize(IMemAllocator *pAlloc,
                                      ALLOCATOR_PROPERTIES *pProperties)
{
    CheckPointer(pAlloc,E_POINTER);
    CheckPointer(pProperties,E_POINTER);

    CAutoLock cAutoLock(m_pFilter->pStateLock());
    HRESULT hr = NOERROR;

    VIDEOINFO *pvi = (VIDEOINFO *) m_mt.Format();
    pProperties->cBuffers = 2;
    pProperties->cbBuffer = MAX_BUFSIZ;

    ASSERT(pProperties->cbBuffer);

    // Ask the allocator to reserve us some sample memory, NOTE the function
    // can succeed (that is return NOERROR) but still not have allocated the
    // memory that we requested, so we must check we got whatever we wanted

    ALLOCATOR_PROPERTIES Actual;
    hr = pAlloc->SetProperties(pProperties,&Actual);
    if(FAILED(hr))
    {
        return hr;
    }

    // Is this allocator unsuitable

    if(Actual.cbBuffer < pProperties->cbBuffer)
    {
        return E_FAIL;
    }

    return NOERROR;

} // DecideBufferSize

/*
	Calculates an acceptable timeout based on the frame rate
*/
int VideoStream::GetD2HTimeout()
{
	//the minimal interval between frames + 5 seconds
	//  remember m_iFrameRate is in frameskip units
	return (int)((m_iFrameRate / 30.0) * 1000.0) + 5000;
}

void VideoStream::AddFilters()
{
	// FilterVideoIn

	m_pVidin_filter = new CFilterVideoIn(2, m_iCam, m_eImagesize, MX_NTSC, MX_COMPOSITE_VIDEO, 3);
	m_iNumOfFilters++;

	m_pFramerate_filter = new CFilterFramerate(2, m_iFrameRate);
	m_iNumOfFilters++;

	m_pFramerate2_filter = new CFilterFramerate(2, m_iFrameRate);
	m_iNumOfFilters++;

	MangoStream::AddFilters();
}

void VideoStream::SetAndAttachPins()
{
	m_pGraph->SetFilter(m_pVidin_filter, 0);
	m_pGraph->SetFilter(m_pFramerate_filter, 1);
	m_pGraph->SetFilter(m_pFramerate2_filter, 2);
	m_pGraph->SetFilter(m_pD2H_filter, 3);
	m_pGraph->SetFilter(m_pTimeTag_filter, 4);
	m_pGraph->Attach(0, 1, 0, 0);
	m_pGraph->Attach(0, 2, 1, 0);
	m_pGraph->Attach(1, 3, 0, 0);
	m_pGraph->Attach(2, 4, 0, 0);
}

void VideoStream::ConnectFilters()
{
	MangoStream::ConnectFilters();
}

void VideoStream::DeleteFilters()
{
	m_pVidin_filter = NULL;
	m_pFramerate_filter = NULL;
	m_pFramerate2_filter = NULL;
	MangoStream::DeleteFilters();
}

HRESULT STDMETHODCALLTYPE VideoStream::SetValue(const GUID* Api, VARIANT* Value)
{
	if (IsEqualGUID(*Api, CODECAPIPARAM_DSP))
	{
		m_iDSP = Value->intVal;
		return S_OK;
	}
	else if (IsEqualGUID(*Api, CODECAPIPARAM_IMAGESIZE))
	{
		m_eImagesize = (enImageSize)Value->intVal;
		switch (m_eImagesize)
		{
		case MX_SQCIF:
			m_iImageWidth = 128;
			m_iImageHeight = 96;
			break;
		case MX_QCIF:
			m_iImageWidth = 176;
			m_iImageHeight = 112;
			break;
		case MX_CIF:
			m_iImageWidth = 352;
			m_iImageHeight = 240;
			break;
		case MX_2CIF:
			m_iImageWidth = 704;
			m_iImageHeight = 240;
			break;
		case MX_4CIF:
			m_iImageWidth = 704;
			m_iImageHeight = 480;
			break;
		case MX_QVGA:
			m_iImageWidth = 320;
			m_iImageHeight = 240;
			break;
		case MX_VGA:
			m_iImageWidth = 640;
			m_iImageHeight = 480;
			break;
		}
		return S_OK;
	}
	else if (IsEqualGUID(*Api, CODECAPIPARAM_CAM))
	{
		m_iCam = Value->intVal;
		return S_OK;
	}
	else if (IsEqualGUID(*Api, CODECAPIPARAM_FRAMERATE))
	{
		if (Value->intVal != m_iFrameRate)
		{
			m_iFrameRate = Value->intVal;
			if (m_pGraph != NULL)
			{
				try
				{
					CAutoLock cAutoLockShared(&m_cSharedState);
					OutputDebugStringA("FrameRate change causing new MangoX graph");
					DeleteGraph();
					CreateGraph();
				}
				catch(MXException & mxe)
				{
					OutputDebugStringA("Caught MX Exception at VideoStream::SetValue!");
					OutputDebugStringA(mxe.GetFunc());
					OutputDebugStringA(mxe.GetStatusString());
					return mxe.ToHRESULT();
				}
				catch(MANGOERROR_error_t errCode)
				{
					return MXException("VideoStream::SetValue", errCode).ToHRESULT();
				}
				catch(...)
				{
					return MXException::MXE_E_FAIL;
				}
			}
		}
		return S_OK;
	}
	return S_FALSE;
}
