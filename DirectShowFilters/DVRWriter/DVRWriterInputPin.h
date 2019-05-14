//------------------------------------------------------------------------------
// File: DVRWriterInputPin.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

class CDVRWriterInputPin;
class CDVRWriter;
class CDVRFilter;

//  Pin object

class CDVRWriterInputPin : public CRenderedInputPin, public IStream
{
    CDVRWriter  * const m_pWriter;           // Main renderer object
    CCritSec * const m_pReceiveLock;    // Sample critical section
    REFERENCE_TIME m_tLast;             // Last sample receive time

public:

    CDVRWriterInputPin(CDVRWriter *pWriter,
                  LPUNKNOWN pUnk,
                  CBaseFilter *pFilter,
                  CCritSec *pLock,
                  CCritSec *pReceiveLock,
                  HRESULT *phr);

    // Do something with this media sample
    STDMETHODIMP Receive(IMediaSample *pSample);
    STDMETHODIMP EndOfStream(void);
    STDMETHODIMP ReceiveCanBlock();

    // Check if the pin can support this specific proposed type and format
    HRESULT CheckMediaType(const CMediaType *);

    // Break connection
    HRESULT BreakConnect();

    // Track NewSegment
    STDMETHODIMP NewSegment(REFERENCE_TIME tStart,
                            REFERENCE_TIME tStop,
                            double dRate);

	// ISequentialStream

	STDMETHODIMP Read(void* pv, ULONG cb, ULONG* pcbRead);
	STDMETHODIMP Write(void const* pv, ULONG cb, ULONG* pcbWritten);

	// IStream

	DECLARE_IUNKNOWN;

	STDMETHODIMP Clone(__RPC__deref_out_opt IStream** ppstm)
	{
		OutputDebugStringA("IStream::Clone");
		return S_OK;
	}
	STDMETHODIMP Commit(DWORD grfCommitFlags)
	{
		OutputDebugStringA("IStream::Commit");
		return S_OK;
	}
	STDMETHODIMP CopyTo(IStream* pstm, ULARGE_INTEGER cb, ULARGE_INTEGER* pcbRead, ULARGE_INTEGER* pcbWritten)
	{
		OutputDebugStringA("IStream::CopyTo");
		return S_OK;
	}
	STDMETHODIMP LockRegion(ULARGE_INTEGER libOffset, ULARGE_INTEGER cb, DWORD dwLockType)
	{
		OutputDebugStringA("IStream::LockRegion");
		return STG_E_INVALIDFUNCTION;
	}
	STDMETHODIMP Revert(void)
	{
		OutputDebugStringA("IStream::Revert");
		return S_OK;
	}
	STDMETHODIMP Seek(LARGE_INTEGER dLibMove, DWORD dwOrigin, ULARGE_INTEGER* pLibNewPosition);
	STDMETHODIMP SetSize(ULARGE_INTEGER libNewSize);
	STDMETHODIMP Stat(__RPC__out STATSTG* pstatstg, DWORD grfStatFlag)
	{
		OutputDebugStringA("IStream::Stat");
		return S_OK;
	}
	STDMETHODIMP UnlockRegion(ULARGE_INTEGER libOffset, ULARGE_INTEGER cb, DWORD dwLockType)
	{
		OutputDebugStringA("IStream::UnlockRegion");
		return STG_E_INVALIDFUNCTION;
	}
	// Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

};
