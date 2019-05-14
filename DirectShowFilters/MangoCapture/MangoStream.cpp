//------------------------------------------------------------------------------
// File: MangoStream.cpp
//
// Base Class for MangoCapture Output Pins.
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

#pragma warning(disable:4710)  // 'function': function not inlined (optimzation)

//
// Constructor
//
MangoStream::MangoStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName) :
    CSourceStream(pPinName, phr, pParent, pPinName),
	m_pGraph(NULL),
	m_pD2H_filter(NULL),
	m_pTimeTag_filter(NULL),
	m_iNumOfFilters(0),
	m_iFrameCount(0),
	m_iGraphNum(-1),
	m_iDSP(0),
	m_bGraphIsCreated(FALSE)
{
    ASSERT(phr);

	m_lastTimeTag.t_lsb = 0;
	m_lastTimeTag.t_msb = 0;

} // (Constructor)

/*
	Default timeout of 10000ms.
*/
int MangoStream::GetD2HTimeout()
{
	return 10000;
}

void
MangoStream::AddFilters()
{
	// FilterD2H - TimeTag

	m_pTimeTag_filter = new CFilterD2H_PCI(1, 0x256, GetD2HTimeout(), Mango::sPCIDevices[m_iDSP]);
	m_iNumOfFilters++;
}

void MangoStream::ConnectFilters()
{
	MANGOERROR_error_t mangoStatus;
	PCI_STREAM_ptr_t buf;
	int bufsiz;

	if (m_pTimeTag_filter != NULL)
	{
		mangoStatus = m_pTimeTag_filter->Connect();
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			//throw mangoStatus;
			throw new MXException("Connect TimeTag Filter", mangoStatus);
		}
	}
	if ((mangoStatus = m_pTimeTag_filter->GetFullBuffer(&buf, &bufsiz)) != MANGOERROR_SUCCESS)
	{
		//throw mangoStatus;
		throw new MXException("Get TimeTag filter on first connect", mangoStatus);
	}
	assert(bufsiz == 8);
	memcpy(&m_lastTimeTag, buf.local, 8);
	m_pTimeTag_filter->FreeBuffer(&buf);
}

void MangoStream::DeleteFilters()
{
	m_pTimeTag_filter = NULL;
}

void
MangoStream::CreateGraph()
{
	MANGOERROR_error_t mangoStatus;

	if (m_bGraphIsCreated == TRUE)
	{
		return;
	}

	Mango::Init();

	// FilterD2H

	m_pD2H_filter = new CFilterD2H_PCI(2, MAX_BUFSIZ, GetD2HTimeout(), Mango::sPCIDevices[m_iDSP]);
	m_iNumOfFilters++;

	AddFilters();

	m_pGraph = new CMangoXGraph(m_iNumOfFilters);

	SetAndAttachPins();

	mangoStatus = MangoX_SubmitGraph(&m_iGraphNum, Mango::sCardNum, m_iDSP, m_pGraph);
	if (mangoStatus != MANGOERROR_SUCCESS)
	{
		throw MXException("SubmitGraph", mangoStatus);
	}
	if (m_pD2H_filter != NULL)
	{
	    mangoStatus = m_pD2H_filter->Connect();
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			throw MXException("D2H Connect", mangoStatus);
		}
	}

	ConnectFilters();

	m_bGraphIsCreated = TRUE;
}

void
MangoStream::DeleteGraph()
{
	if (m_bGraphIsCreated == TRUE)
	{
		if (m_iGraphNum != -1)
		{
			MangoX_DeleteGraph(m_iGraphNum, Mango::sCardNum, m_iDSP);
			m_iGraphNum = -1;
		}
		if (m_pGraph != NULL)
		{
			delete m_pGraph;
			m_pGraph = NULL;
		}
		m_iNumOfFilters = 0;

		m_pD2H_filter = NULL;
		m_pTimeTag_filter = NULL;

		DeleteFilters();

		m_bGraphIsCreated = FALSE;

		Mango::Finish();
	}
}

//
// Destructor
//
MangoStream::~MangoStream()
{
	if (m_bGraphIsCreated == TRUE)
	{
		try
		{
			DeleteGraph();
		}
		catch(...)
		{
			OutputDebugStringA("Exception occurred while destructing MangoStream!");
		}
	}
}

//
// FillBuffer
//
HRESULT MangoStream::FillBuffer(IMediaSample *pms)
{
    CheckPointer(pms, E_POINTER);

    BYTE *pData;
    long lDataLen;

	pms->GetPointer(&pData);
    lDataLen = pms->GetSize();

    ZeroMemory(pData, lDataLen);
    {
        CAutoLock cAutoLockShared(&m_cSharedState);
		MANGOERROR_error_t mangoStatus;
		PCI_STREAM_ptr_t buf;
		int bufsiz;
		mangoStatus = m_pD2H_filter->GetFullBuffer(&buf, &bufsiz);
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			Mango::showError("GetFullBuffer", mangoStatus);
			return HRESULT_FROM_WIN32(ERROR_READ_FAULT);
		}
		m_iFrameCount++;
		if (bufsiz > MAX_BUFSIZ)
		{
			bufsiz = MAX_BUFSIZ;
		}
		memcpy(pData, buf.local, bufsiz);
		pms->SetActualDataLength(bufsiz);
		m_pD2H_filter->FreeBuffer(&buf);
		if (SetSampleTime(pms) == FALSE)
		{
			return HRESULT_FROM_WIN32(ERROR_READ_FAULT);
		}
		DoFramePostProcessing(pms);
	}
    return S_OK;
} // FillBuffer

bool MangoStream::SetSampleTime(IMediaSample* pms)
{
	MANGOERROR_error_t mangoStatus;
	CRefTime rtStart = m_rtSampleTime;
	PCI_STREAM_ptr_t buf;
	int bufsiz;
	Time_tag_T timeTag;
	LONG hundredNanoSecs;

	if ((mangoStatus = m_pTimeTag_filter->GetFullBuffer(&buf, &bufsiz)) != MANGOERROR_SUCCESS) {
		Mango::showError("GetFullBuffer TimeTag", mangoStatus);
		return false;
	}
	assert(bufsiz == 8);
	memcpy(&timeTag, buf.local, 8);
	m_pTimeTag_filter->FreeBuffer(&buf);
	if (timeTag.t_msb != m_lastTimeTag.t_msb)
	{
		hundredNanoSecs = ((2^32 - m_lastTimeTag.t_lsb + timeTag.t_lsb)*10) / 75;
	} else
	{
		hundredNanoSecs = ((timeTag.t_lsb - m_lastTimeTag.t_lsb)*10) / 75;
	}
	m_rtSampleTime = m_rtSampleTime + hundredNanoSecs;
	pms->SetTime((REFERENCE_TIME *) &rtStart,(REFERENCE_TIME *) &m_rtSampleTime);
	m_lastTimeTag = timeTag;
	return true;
}

//
// Notify
//
// Alter the repeat rate according to quality management messages sent from
// the downstream filter (often the renderer).  Wind it up or down according
// to the flooding level - also skip forward if we are notified of Late-ness
//
STDMETHODIMP MangoStream::Notify(IBaseFilter* pSender, Quality q)
{
#if 0
	fprintf(stderr, "MangoStream::Notify proportion=%d late=%i64\n", q.Proportion, q.Late);
    // Adjust the repeat rate.
    if(q.Proportion <= 0)
    {
		m_iFrameDuration = 10000000;
    }
    else
    {
		m_iFrameDuration = m_iFrameDuration*10000000 / q.Proportion;
        if(m_iFrameDuration>10000000)
        {
            m_iFrameDuration = 10000000;    // We don't go slower than 1 per second
        }
        else if(m_iFrameDuration<100000)
        {
            m_iFrameDuration = 100000;      // We don't go faster than 100/sec
        }
    }

    // skip forwards
    if(q.Late > 0)
	{
        m_rtSampleTime += q.Late;
	}
#endif
    return NOERROR;
} // Notify

//
// CheckMediaType
//
// Returns E_INVALIDARG if the mediatype is not acceptable
//
HRESULT MangoStream::CheckMediaType(const CMediaType *pMediaType)
{
    CheckPointer(pMediaType,E_POINTER);

    return S_OK;  // This format is acceptable.

} // CheckMediaType

//
// SetMediaType
//
// Called when a media type is agreed between filters
//
HRESULT MangoStream::SetMediaType(const CMediaType *pMediaType)
{
    CAutoLock cAutoLock(m_pFilter->pStateLock());

    // Pass the call up to my base class

    HRESULT hr = CSourceStream::SetMediaType(pMediaType);

    if(SUCCEEDED(hr))
    {
        return NOERROR;
    } 
    return hr;

} // SetMediaType

//
// OnThreadCreate
//
// As we go active reset the stream time to zero
//
HRESULT MangoStream::OnThreadCreate()
{
    CAutoLock cAutoLockShared(&m_cSharedState);
    m_rtSampleTime = 0;

    // we need to also reset the repeat time in case the system
    // clock is turned off after m_iRepeatTime gets very big
	if (m_bGraphIsCreated == FALSE)
	{
		try
		{
			CreateGraph();
		}
		catch (MXException& mxe)
		{
			OutputDebugStringA("Caught MX Exception OnThreadCreate!");
			OutputDebugStringA(mxe.GetFunc());
			OutputDebugStringA(mxe.GetStatusString());
//			Mango::Finish();
			m_bGraphIsCreated = FALSE;

			return mxe.ToHRESULT();
		}
		catch (...)
		{
			return MXException::MXE_E_FAIL;
		}
	}
    return NOERROR;

} // OnThreadCreate

HRESULT MangoStream::OnThreadDestroy()
{
	CAutoLock cAutoLockShared(&m_cSharedState);
	try
	{
		DeleteGraph();
		return S_OK;
	}
	catch(MXException & mxe)
	{
		OutputDebugStringA("Caught MX Exception OnThreadDestroy!");
		OutputDebugStringA(mxe.GetFunc());
		OutputDebugStringA(mxe.GetStatusString());
		return mxe.ToHRESULT();
	}
	catch(MANGOERROR_error_t errorCode)
	{
		return MXException("OnThreadDestroy", errorCode).ToHRESULT();
	}
	catch(...)
	{
		return MXException::MXE_E_FAIL;
	}
}
