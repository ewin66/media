//------------------------------------------------------------------------------
// File: DVRWriterInputPin.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

//
//  Definition of CDumpInputPin
//
CDVRWriterInputPin::CDVRWriterInputPin(CDVRWriter *pWriter,
                             LPUNKNOWN pUnk,
                             CBaseFilter *pFilter,
                             CCritSec *pLock,
                             CCritSec *pReceiveLock,
                             HRESULT *phr) :

    CRenderedInputPin(NAME("CDVRWriterInputPin"),
                  pFilter,                   // Filter
                  pLock,                     // Locking
                  phr,                       // Return code
                  L"Input"),                 // Pin name
    m_pReceiveLock(pReceiveLock),
    m_pWriter(pWriter),
    m_tLast(0)
{
}

//
// CheckMediaType
//
// Check if the pin can support this specific proposed type and format
//
HRESULT CDVRWriterInputPin::CheckMediaType(const CMediaType *)
{
    return S_OK;
}

//
// BreakConnect
//
// Break a connection
//
HRESULT CDVRWriterInputPin::BreakConnect()
{
	OutputDebugStringA("DVRWriterInputPin::BreakConnect");
    if (m_pWriter->m_pPosition != NULL)
	{
        m_pWriter->m_pPosition->ForceRefresh();
    }
    return CRenderedInputPin::BreakConnect();
}

//
// ReceiveCanBlock
//
// We don't hold up source threads on Receive
//
STDMETHODIMP CDVRWriterInputPin::ReceiveCanBlock()
{
    return S_FALSE;
}

//
// Receive
//
// Do something with this media sample
//
STDMETHODIMP CDVRWriterInputPin::Receive(IMediaSample *pSample)
{
	HRESULT hr;
	char buf[128];
	LARGE_INTEGER timeStart;
	LARGE_INTEGER timeEnd;
	LARGE_INTEGER newSeek;
	ULONG cbWritten;
    CheckPointer(pSample,E_POINTER);

    CAutoLock lock(m_pReceiveLock);

    // Has the filter been stopped yet?
	if (m_pWriter->m_hFile == INVALID_HANDLE_VALUE)
	{
		return NOERROR;
	}

	//hr = pSample->GetTime((REFERENCE_TIME*)&timeStart, (REFERENCE_TIME*)&timeEnd);
	//if (FAILED(hr))
	//{
	//	return hr;
	//}
//	sprintf(buf, "DVRWriterInputPin::Receive %I64u %I64u %d", timeStart, timeEnd, pSample->GetActualDataLength());
//	OutputDebugStringA(buf);

	//hr = m_pWriter->Seek(timeStart, &newSeek);
	//if (hr != S_OK)
	//{
	//	return hr;
	//}

    PBYTE pbData;
    hr = pSample->GetPointer(&pbData);
    if (FAILED(hr))
	{
        return hr;
    }
    return m_pWriter->Write(pbData, pSample->GetActualDataLength(), &cbWritten);
}

//
// EndOfStream
//
STDMETHODIMP CDVRWriterInputPin::EndOfStream(void)
{
    CAutoLock lock(m_pReceiveLock);
	OutputDebugStringA("DVRWriterInputPin::EndOfStream");
    return CRenderedInputPin::EndOfStream();

} // EndOfStream


//
// NewSegment
//
// Called when we are seeked
//
STDMETHODIMP CDVRWriterInputPin::NewSegment(REFERENCE_TIME tStart,
                                       REFERENCE_TIME tStop,
                                       double dRate)
{
	OutputDebugStringA("DVRWriterInputPin::NewSegment");
    m_tLast = 0;
    return S_OK;

} // NewSegment

HRESULT CDVRWriterInputPin::Seek(LARGE_INTEGER dLibMove, DWORD dwOrigin, ULARGE_INTEGER* pLibNewPosition)
{
	char buf[128];
	sprintf(buf, "DVRWriterInputPin::Seek dLibMove=%I64u dwOrigin=%d pLibNewPosition=%x", dLibMove, dwOrigin, pLibNewPosition);
	OutputDebugStringA(buf);
	return m_pWriter->Seek(dLibMove, (PLARGE_INTEGER)pLibNewPosition);
}

HRESULT CDVRWriterInputPin::SetSize(ULARGE_INTEGER libNewSize)
{
	char buf[128];
	sprintf(buf, "DVRWriterInputPin::SetSize %I64u", libNewSize);
	OutputDebugStringA(buf);
	
	return S_OK;
}

HRESULT CDVRWriterInputPin::Read(void* pv, ULONG cb, LPDWORD pcbRead)
{
	char buf[128];
	sprintf(buf, "DVRWriterInputPin::Read %x %d %x", pv, cb, pcbRead);
	OutputDebugStringA(buf);
	return m_pWriter->Read((PBYTE)pv, cb, pcbRead);
}

HRESULT CDVRWriterInputPin::Write(void const* pv, ULONG cb, ULONG* pcbWritten)
{
	char buf[128];
	sprintf(buf, "DVRWriterInputPin::Write %x %d %x", pv, cb, pcbWritten);
	OutputDebugStringA(buf);
	return m_pWriter->Write((PBYTE)pv, cb, pcbWritten);
}

//
// NonDelegatingQueryInterface
//
// Override this to say what interfaces we support where
//
STDMETHODIMP CDVRWriterInputPin::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
	CAutoLock lock(m_pReceiveLock);

    CheckPointer(ppv,E_POINTER);

	// Do we have this interface

	if (riid == IID_IStream)
	{
		return GetInterface(static_cast<IStream*>(this), ppv);
	}
	else
	{
	    return CRenderedInputPin::NonDelegatingQueryInterface(riid, ppv);
	}

} // NonDelegatingQueryInterface
