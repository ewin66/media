//
// PassThru.cpp
//
// Implementation of in-place transform base classes
// based on pass-through allocator proxy.
//
// Geraint Davies www.gdcl.co.uk

#include "inline.h"

CInplaceTransform2::CInplaceTransform2(TCHAR * pName, LPUNKNOWN pUnk, REFCLSID clsid, HRESULT *phr)
: CTransInPlaceFilter(pName, pUnk, clsid, phr),
  m_pUpstreamAlloc(NULL),
  m_bPassThrough(true),
  m_bReadOnly(false),
  m_bOutputUsingUpstream(false),
  m_pSampleChange(NULL)
{
}

CInplaceTransform2::~CInplaceTransform2()
{
    if (m_pUpstreamAlloc)
    {
        m_pUpstreamAlloc->Release();
    }
}

CBasePin*
CInplaceTransform2::GetPin(int n)
{
    HRESULT hr = S_OK;
    if (!m_pOutput)
    {
        m_pOutput = new CInplaceOutput( NAME("CInplaceOutput")
                                        , this       // Owner filter
                                        , &hr        // Result code
                                        , L"Output"  // Pin name
                                      );
    }
    if (!m_pInput)
    {
        m_pInput = new CInplaceInput( NAME("CInplaceInput")
                                      , this       // Owner filter
                                      , &hr        // Result code
                                      , L"Input"  // Pin name
                                    );
    }
    return CTransInPlaceFilter::GetPin(n);
}

//
// Choice of allocators:
// 
// We can get into a copy situation because the pins won't accept a common
// allocator, because the allocator properties differ, or because the upstream
// allocator is notified as read-only. So there are four cases:
// 
// 1: Upstream pin insists on its own allocator, and downstream accepts it.
// 2: Upstream pin insists on its own allocator, but downstream pin uses a separate one.
// 3: Upstream pin accepts pass-through allocator, which uses the downstream alloc.
// 4: Upstream pin accepts pass-through, but downstream has a separate allocator.
//

HRESULT 
CInplaceTransform2::NotifyAllocator(IMemAllocator * pAllocator, BOOL bReadOnly, bool bOurAllocator)
{
    m_bPassThrough = false;
    bool bReconnect = false;

    if (Output()->IsConnected())
    {
        if (m_bOutputUsingUpstream)
        {
            if (bOurAllocator || (pAllocator != m_pUpstreamAlloc) || bReadOnly)
            {
                // the output pin agreed to use the upstream allocator, 
                // which has now changed -- make a reconnect

                bReconnect = true;
            } else
            {
                // case 1 -- no copy
                m_bPassThrough = true;
            }
        } else if (bOurAllocator)
        {
            // upstream pin has agreed to use pass-through allocator --
            // we need to assign a real allocator to the pass-through.
            HRESULT hr = E_FAIL;
            if (!bReadOnly)
            {
                // can he use downstream alloc?
                //(probably he has this already)
                hr = Input()->SetOutputAllocator(Output()->PeekAllocator());
            }
            if (FAILED(hr))
            {
                // can't use downstream -- 
                // must create special allocator
                IMemAllocator* pAlloc;
                HRESULT hr = CreateMemoryAllocator(&pAlloc);
                if (SUCCEEDED(hr))
                {
                    hr = Input()->SetOutputAllocator(pAlloc);
                    pAlloc->Release();
                }
                if (FAILED(hr))
                {
                    // not much we can do to save the connection now?
                    return hr;
                }
            } else
            {
                m_bPassThrough = true;
            }
        }

    }

    if (m_pUpstreamAlloc)
    {
        m_pUpstreamAlloc->Release();
        m_pUpstreamAlloc = NULL;
    }
    if (!bOurAllocator)
    {
        m_pUpstreamAlloc = pAllocator;
        pAllocator->AddRef();
    }
    m_bReadOnly = bReadOnly?true:false;


    if (bReconnect)
    {
        ReconnectPin(m_pOutput, &m_pInput->CurrentMediaType());
    }

    return S_OK;
}

HRESULT 
CInplaceTransform2::DecideAllocator(IMemInputPin * pPin, IMemAllocator ** ppAlloc)
{
    // Remember that the input is usually reconnected after the
    // output, so setting the state of m_bPassThrough is not important here
    m_bPassThrough = false;
    m_bOutputUsingUpstream = false;

    // if upstream insists on its own allocator, and its not read-only
    // we should try that first
    HRESULT hr;
    if (m_pUpstreamAlloc && !m_bReadOnly)
    {
        hr = pPin->NotifyAllocator(m_pUpstreamAlloc, FALSE);
        if (SUCCEEDED(hr))
        {
            m_bOutputUsingUpstream = true;
            m_bPassThrough = true;
            *ppAlloc = m_pUpstreamAlloc;
            m_pUpstreamAlloc->AddRef();
            return hr;
        }
    }

    // otherwise try the downstream's preferred allocator
    bool bNewAlloc = false;
    IMemAllocator* pAlloc;
    hr = pPin->GetAllocator(&pAlloc);
    if (FAILED(hr))
    {
        // downstream has no allocator -- unlikely but logically possible
        hr = CreateMemoryAllocator(&pAlloc);
        if (FAILED(hr))
        {
            // even more unlikely and cause for connection failure
            return hr;
        }
        bNewAlloc = true;
    }

    // this is a new allocator and needs properties setting
    ALLOCATOR_PROPERTIES prop;
    ZeroMemory(&prop, sizeof(prop));

    // Try to get requirements from downstream
    // -- doesn't matter if it has no requirements
    pPin->GetAllocatorRequirements(&prop);


    // if he doesn't care about alignment, then set it to 1
    if (prop.cbAlign == 0)
    {
        prop.cbAlign = 1;
    }

    if (SUCCEEDED(hr))
    {
        hr = DecideBufferSize(pAlloc, &prop);
    }
    if (SUCCEEDED(hr))
    {
        hr = pPin->NotifyAllocator(pAlloc, false);
    }

    if (FAILED(hr))
    {
        *ppAlloc = NULL;
        pAlloc->Release();
    } else
    {
        *ppAlloc = pAlloc;
        // we have now agreed an allocator for the
        // downstream connection. Try to give this to the proxy
        hr = Input()->SetOutputAllocator(pAlloc);
        if (SUCCEEDED(hr))
        {
            m_bPassThrough = true;
        }
    }
    return hr;
}

HRESULT 
CInplaceTransform2::Receive(IMediaSample *pSample)
{
    // no MT added yet
    bool bStripMT = false;

    // make a copy if we are not using the same allocator
    if (!m_bPassThrough)
    {
        pSample = Copy(pSample);
        if (!pSample)
        {
            return VFW_E_RUNTIME_ERROR;
        }
    }

    HRESULT hr;
    if (m_pSampleChange == pSample)
    {
        // this one *should* have a dynamic type change
        AM_MEDIA_TYPE* pmt;
        HRESULT hr = pSample->GetMediaType(&pmt);
        if (S_OK == hr)
        {
            // alls well
            DeleteMediaType(pmt);
        } else
        {
            // media type data has been stripped by
            // upstream filter -- reattach it here
            pSample->SetMediaType(&m_mtChange);
            // remember to strip it after processing
            bStripMT = true;
        }

        // perform the switch now
        hr = CheckInputType(&m_mtChange);
        if (FAILED(hr))
        {
            return hr;
        }
        SetMediaType(PINDIR_INPUT, &m_mtChange);
        SetMediaType(PINDIR_OUTPUT, &m_mtChange);

        // done this one now
        m_pSampleChange = NULL;
    }


    // pass to derived class to process
    hr = Transform(pSample);

    // after processing, strip a dynamic change if we added it here
    if (bStripMT)
    {
        pSample->SetMediaType(NULL);
    }

    // return values from Transform can be S_OK to deliver,
    // S_FALSE to skip or an error.
    if (hr == S_OK)
    {
        hr = m_pOutput->Deliver(pSample);
    } else if (hr == S_FALSE)
    {
        // skip the sample
        m_bSampleSkipped = true;
        if (!m_bQualityChanged)
        {
            NotifyEvent(EC_QUALITY_CHANGE,0,0);
            m_bQualityChanged = TRUE;
            // we cannot return S_FALSE since this has a
            // different meaning when returned from Receive
            hr = S_OK;
        }
    }
    // else it's an error

    if (!m_bPassThrough)
    {
        // we created the sample, so we must release it
        pSample->Release();
    }
    return hr;
}


void 
CInplaceTransform2::RecordDynamicChange(IMediaSample* pSample, CMediaType* pmt)
{
    CAutoLock lock(&m_csFilter);

    // the allocator is reporting a dynamic type change
    // initiated by the downstream filter.
    // There is a chance that the upstream filter will strip this
    // information before it reaches our Receive -- in this case, we must
    // remember it ourselves.
    m_mtChange = *pmt;
    m_pSampleChange = pSample;
}


// overide to make reconnections
HRESULT 
CInplaceTransform2::CompleteConnect(PIN_DIRECTION dir, IPin *pReceivePin)
{
    if (!m_pGraph)
    {
        return VFW_E_NOT_IN_GRAPH;
    }

    if (dir == PINDIR_OUTPUT)
    {
        if (Input()->IsConnected())
        {

            // when we connect the output we should
            // reconnect the input to make sure the allocator is 
            // sorted out -- but if the output has chosen the 
            // upstream allocator, we don't need a reconnect.
            if (!m_bOutputUsingUpstream || 
                (Input()->CurrentMediaType() != Output()->CurrentMediaType()))
            {

                ReconnectPin(Input(), &Output()->CurrentMediaType());
            }
        }
    } else if (Output()->IsConnected())
    {
        // on input connection, only reconnect output
        // if wrong type
        if (Input()->CurrentMediaType() != Output()->CurrentMediaType())
        {
            ReconnectPin(Output(), &Input()->CurrentMediaType());
        }
    }

    return S_OK;
}

// Make a copy on the right allocator.
// Note that we cannot afford a dynamic mediatype change here
// since our source has already delivered the data
IMediaSample * 
CInplaceTransform2::Copy(IMediaSample *pSource)
{
    // pass NULL times to ensure no dynamic change
    IMediaSample* pCopy;
    HRESULT hr = Output()->PeekAllocator()->GetBuffer(&pCopy, NULL, NULL, 0);
    if (FAILED(hr))
    {
        return NULL;
    }

    AM_MEDIA_TYPE* pmt;
    hr = pCopy->GetMediaType(&pmt);
    if (hr == S_OK)
    {
        pCopy->Release();
        DeleteMediaType(pmt);
        return NULL;
    }

    IMediaSample2 *pCopyS2;
    // can we copy the v2 props?
    hr = pCopy->QueryInterface(IID_IMediaSample2, (void**)&pCopyS2);

    if (SUCCEEDED(hr))
    {
        // the v2 props were parsed by CBaseInputPin::Receive
        BYTE* pSrcProps = (BYTE*)m_pInput->SampleProps();

        // write the whole structure except the buffer pointer
        int cProps = FIELD_OFFSET(AM_SAMPLE2_PROPERTIES, pbBuffer);
        hr = pCopyS2->SetProperties(cProps, pSrcProps);
        pCopyS2->Release();

        if (FAILED(hr))
        {
            pCopy->Release();
            return NULL;
        }
    } else
    {
        // copy v1 properties individually
        REFERENCE_TIME tStart, tStop;
        hr = pSource->GetTime(&tStart, &tStop);
        if (S_OK == hr)
        {
            pCopy->SetTime(&tStart, &tStop);
        }

        if (S_OK == pSource->IsSyncPoint())
        {
            pCopy->SetSyncPoint(TRUE);
        }
        if (S_OK == pSource->IsDiscontinuity() || m_bSampleSkipped)
        {
            pCopy->SetDiscontinuity(TRUE);
        }
        if (S_OK == pSource->IsPreroll())
        {
            pCopy->SetPreroll(TRUE);
        }

        // Copy the media type -- dynamic media types
        // coming from upstream are ok (by us)
        AM_MEDIA_TYPE *pMediaType;
        if (S_OK == pSource->GetMediaType(&pMediaType))
        {
            pCopy->SetMediaType(pMediaType);
            DeleteMediaType(pMediaType);
        }
    }

    m_bSampleSkipped = FALSE;

    // Copy the sample media times
    REFERENCE_TIME TimeStart, TimeEnd;
    hr = pSource->GetMediaTime(&TimeStart,&TimeEnd);
    if (hr == S_OK)
    {
        pCopy->SetMediaTime(&TimeStart,&TimeEnd);
    }

    // Copy the actual data length and the actual data.
    {
        const long lDataLength = pSource->GetActualDataLength();
        pCopy->SetActualDataLength(lDataLength);

        // Copy the sample data
        {
            BYTE *pSourceBuffer, *pDestBuffer;
            long lSourceSize  = pSource->GetSize();
            long lDestSize = pCopy->GetSize();

            ASSERT(lDestSize >= lSourceSize && lDestSize >= lDataLength);

            pSource->GetPointer(&pSourceBuffer);
            pCopy->GetPointer(&pDestBuffer);
            ASSERT(lDestSize == 0 || pSourceBuffer != NULL && pDestBuffer != NULL);

            CopyMemory( (PVOID) pDestBuffer, (PVOID) pSourceBuffer, lDataLength );
        }
    }

    return pCopy;
}

CInplaceInput::CInplaceInput(
    TCHAR* pObjectName,
    CInplaceTransform2* pFilter,
    HRESULT* phr,
    LPCWSTR pName)
: m_pPassThru(pFilter),
  m_pProxy(NULL),
  CTransInPlaceInputPin(pObjectName, pFilter, phr, pName)
{
}

CProxyAllocator* 
CInplaceInput::InputProxy()
{
    CAutoLock lock(m_pLock);

    if (!m_pAllocator)
    {
        m_pProxy = new CProxyAllocator(m_pPassThru);
        if (!m_pProxy)
        {
            return NULL;
        }
        m_pProxy->QueryInterface(IID_IMemAllocator, (void**)&m_pAllocator);
    }
    return m_pProxy;
}

HRESULT 
CInplaceInput::SetOutputAllocator(IMemAllocator* pAlloc)
{
    CAutoLock lock(m_pLock);

    // pass downstream alloc to our alloc.
    CProxyAllocator* pOurs = InputProxy();
    HRESULT hr = VFW_E_NO_ALLOCATOR;
    if (pOurs)
    {
        hr = pOurs->SetAllocator(pAlloc);
    }
    return hr;
}



STDMETHODIMP 
CInplaceInput::GetAllocator(IMemAllocator **ppAllocator)
{
    CAutoLock lock(m_pLock);

    if (!InputProxy())
    {
        return VFW_E_NO_ALLOCATOR;
    }

    *ppAllocator = m_pAllocator;
    m_pAllocator->AddRef();
    return S_OK;
}


STDMETHODIMP 
CInplaceInput::NotifyAllocator(IMemAllocator * pAllocator, BOOL bReadOnly)
{
    CAutoLock lock(m_pLock);

    m_bReadOnly = bReadOnly;
    bool bOurAlloc;
    if (pAllocator == m_pAllocator)
    {
        bOurAlloc = true;
    } else
    {
        bOurAlloc = false;
    }

    return m_pPassThru->NotifyAllocator(pAllocator, bReadOnly, bOurAlloc);
}

// ---

CProxyAllocator::CProxyAllocator(CInplaceTransform2* pFilter)
: m_pFilter(pFilter),
  m_pAlloc(NULL),
  CUnknown(NAME("CProxyAllocator"), NULL)
{
    m_props.cBuffers = 0;
}

CProxyAllocator::~CProxyAllocator()
{
    if (m_pAlloc)
    {
        m_pAlloc->Release();
    }
}

STDMETHODIMP 
CProxyAllocator::NonDelegatingQueryInterface(REFIID riid, void** ppv)
{
    if (riid == IID_IMemAllocator)
    {
        return GetInterface((IMemAllocator *) this, ppv);
    } else
    {
        return CUnknown::NonDelegatingQueryInterface(riid, ppv);
    }
}

STDMETHODIMP 
CProxyAllocator::SetProperties(
    ALLOCATOR_PROPERTIES* pRequest,
    ALLOCATOR_PROPERTIES* pActual)
{
    CAutoLock lock(&m_Lock);

    m_props = *pRequest;
    HRESULT hr = S_OK;
    if (m_pAlloc)
    {
        hr = m_pAlloc->SetProperties(pRequest, pActual);
    } else
    {
        *pActual = m_props;
    }
    return hr;
}


STDMETHODIMP 
CProxyAllocator::ReleaseBuffer(IMediaSample *pSample)
{
    HRESULT hr = S_FALSE;
    if (m_pAlloc)
    {
        hr = m_pAlloc->ReleaseBuffer(pSample);
    }
    return hr;
}

STDMETHODIMP 
CProxyAllocator::GetBuffer(
                          IMediaSample **ppBuffer,
                          REFERENCE_TIME *pStart,
                          REFERENCE_TIME *pEnd,
                          DWORD dwFlags)
{
    // we must use the critsec to protect against
    // changes to the underlying allocator, but we cannot
    // hold the critsec across the blocking GetBuffer,
    // so we must take a refcounted copy of the allocator
    IMemAllocator* pAlloc = NULL;
    {
        CAutoLock lock(&m_Lock);

        pAlloc = m_pAlloc;
        if (pAlloc)
        {
            pAlloc->AddRef();
        }
    }

    HRESULT hr = VFW_E_NO_ALLOCATOR;
    if (pAlloc)
    {
        IMediaSample* pSample = NULL;
        hr = pAlloc->GetBuffer(&pSample, pStart, pEnd, dwFlags);

        if (SUCCEEDED(hr))
        {
            // is there a dynamic type change here?
            AM_MEDIA_TYPE* pmt;
            HRESULT hrType = pSample->GetMediaType(&pmt);
            if (S_OK == hrType)
            {
                // yes -- inform filter in case it is stripped
                CMediaType mt(*pmt);
                DeleteMediaType(pmt);
                m_pFilter->RecordDynamicChange(pSample, &mt);
            }
        }
        *ppBuffer = pSample;

        pAlloc->Release();
    }
    return hr;
}

STDMETHODIMP 
CProxyAllocator::GetProperties(ALLOCATOR_PROPERTIES *pProps)
{
    CAutoLock lock(&m_Lock);

    if (m_pAlloc)
    {
        return m_pAlloc->GetProperties(pProps);
    } else
    {
        *pProps = m_props;
        return S_OK;
    }
}

STDMETHODIMP 
CProxyAllocator::Commit(void)
{
    CAutoLock lock(&m_Lock);

    if (m_pAlloc)
    {
        return m_pAlloc->Commit();
    } else
    {
        return VFW_E_NO_ALLOCATOR;
    }
}

STDMETHODIMP 
CProxyAllocator::Decommit(void)
{
    CAutoLock lock(&m_Lock);

    if (m_pAlloc)
    {
        return m_pAlloc->Decommit();
    } else
    {
        return VFW_E_NO_ALLOCATOR;
    }
}

// called by filter when downstream allocator is decided
HRESULT 
CProxyAllocator::SetAllocator(IMemAllocator* pAlloc)
{
    CAutoLock lock(&m_Lock);

    if (m_pAlloc)
    {
        m_pAlloc->Release();
        m_pAlloc = NULL;
    }
    m_pAlloc = pAlloc;
    if (!pAlloc)
    {
        return VFW_E_NO_ALLOCATOR;
    }

    m_pAlloc->AddRef();
    HRESULT hr = S_OK;
    if (m_props.cBuffers > 0)
    {
        ALLOCATOR_PROPERTIES propActual;
        hr = pAlloc->SetProperties(&m_props, &propActual);
        if (FAILED(hr) || (m_props.cbBuffer != propActual.cbBuffer))
        {
            hr = VFW_E_NO_ALLOCATOR;
        }
    }

    return hr;
}


// --- Output pin ------------------------

HRESULT 
CInplaceOutput::DecideAllocator(IMemInputPin * pPin, IMemAllocator ** ppAlloc)
{
    CAutoLock lock(m_pLock);

    return m_pPassThru->DecideAllocator(pPin, ppAlloc);
}


