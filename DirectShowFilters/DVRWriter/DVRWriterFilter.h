//------------------------------------------------------------------------------
// File: DVRWriter.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

class CDVRWriterInputPin;
class CDVRWriter;
class CDVRFilter;

// Main filter object

class CDVRWriterFilter : public CBaseFilter
{
    CDVRWriter * const m_pWriter;

public:

    // Constructor
    CDVRWriterFilter(CDVRWriter *pWriter,
                LPUNKNOWN pUnk,
                CCritSec *pLock,
                HRESULT *phr);

    // Pin enumeration
    CBasePin * GetPin(int n);
    int GetPinCount();

    STDMETHODIMP Run(REFERENCE_TIME tStart);
    STDMETHODIMP Pause();
    STDMETHODIMP Stop();
};
