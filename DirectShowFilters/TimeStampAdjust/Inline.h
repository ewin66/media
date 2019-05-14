//
// PassThru.h
//
// Improved version of in-place transform base class
//
// Geraint Davies www.gdcl.co.uk
//
// 
// The DirectShow in-place transform has a number of problems when
// used with decompressed video:
// -- it does not connect to a downstream filter that insists on its own allocator
// -- it never uses the allocator from the downstream filter
// -- dynamic type change originating from downstream is missed
//
// We resolve this by using a proxy allocator on the input pin.

#include <streams.h>

#ifndef __PASSTHRU_H__
#define __PASSTHRU_H__

class CInplaceTransform2;       // the filter
class CInplaceInput;            // the input pin
class CInplaceOutput;           // the output pin
class CProxyAllocator;          // allocator used on input pin

// filter is derived from CTransInPlaceFilter as this 
// handles the media type negotatiation correctly

class CInplaceTransform2 
: public CTransInPlaceFilter
{
public:
    CInplaceTransform2(TCHAR * pName, LPUNKNOWN pUnk, REFCLSID clsid, HRESULT *phr);
    ~CInplaceTransform2();

    CBasePin *GetPin(int n);

    HRESULT NotifyAllocator(IMemAllocator * pAllocator, BOOL bReadOnly, bool bOurAllocator);
    HRESULT Receive(IMediaSample *pSample);
    HRESULT DecideAllocator(IMemInputPin * pPin, IMemAllocator ** pAlloc);
    HRESULT CompleteConnect(PIN_DIRECTION dir,IPin *pReceivePin);

    void RecordDynamicChange(IMediaSample* pSample, CMediaType* pmt);
protected:
    bool m_bPassThrough;
    bool m_bReadOnly;
    bool m_bOutputUsingUpstream;
    IMemAllocator* m_pUpstreamAlloc;

    CMediaType m_mtChange;
    IMediaSample* m_pSampleChange;

    CInplaceInput* Input() {
        return reinterpret_cast<CInplaceInput*>(m_pInput);
    }
    CInplaceOutput* Output() {
        return reinterpret_cast<CInplaceOutput*>(m_pOutput);
    }
    IMediaSample * Copy(IMediaSample *pSource);
};


// allocator used on input pin, redirects to memory allocator
// or to downstream.
class CProxyAllocator : public CUnknown, public IMemAllocator
{
public:

    CProxyAllocator(CInplaceTransform2* pFilter);
    ~CProxyAllocator();
    DECLARE_IUNKNOWN
    STDMETHODIMP NonDelegatingQueryInterface(REFIID iid, void** ppv);

    STDMETHODIMP SetProperties(
                              ALLOCATOR_PROPERTIES* pRequest,
                              ALLOCATOR_PROPERTIES* pActual);

    STDMETHODIMP ReleaseBuffer(IMediaSample *pSample);
    STDMETHODIMP GetBuffer(IMediaSample **ppBuffer,
                           REFERENCE_TIME *pStart,
                           REFERENCE_TIME *pEnd,
                           DWORD dwFlags);
    STDMETHODIMP GetProperties(ALLOCATOR_PROPERTIES *pProps);
    STDMETHODIMP Commit(void);
    STDMETHODIMP Decommit(void);

    // called by filter when downstream allocator is decided
    HRESULT SetAllocator(IMemAllocator* pAlloc);

protected:
    CCritSec m_Lock;
    CInplaceTransform2* m_pFilter;
    IMemAllocator* m_pAlloc;
    ALLOCATOR_PROPERTIES m_props;
};

// override input pin to handle allocator negotiation
class CInplaceInput : public CTransInPlaceInputPin
{
public:
    CInplaceInput(
                 TCHAR* pObjectName,
                 CInplaceTransform2* pFilter,
                 HRESULT* phr,
                 LPCWSTR pName);

    STDMETHODIMP GetAllocator(IMemAllocator **ppAllocator);
    STDMETHODIMP NotifyAllocator(IMemAllocator * pAllocator, BOOL bReadOnly);

    HRESULT SetOutputAllocator(IMemAllocator* pAlloc);
    CProxyAllocator* InputProxy();
protected:
    CInplaceTransform2* m_pPassThru;
    CProxyAllocator* m_pProxy;
};

class CInplaceOutput : public CTransInPlaceOutputPin
{
public:
    CInplaceOutput(
      TCHAR               *pObjectName,
      CInplaceTransform2 *pFilter,
      HRESULT             *phr,
      LPCWSTR              pName)
    : CTransInPlaceOutputPin(pObjectName, pFilter, phr, pName),
    m_pPassThru(pFilter)
    {}


    HRESULT DecideAllocator(IMemInputPin * pPin, IMemAllocator ** pAlloc);

protected:
    CInplaceTransform2* m_pPassThru;
};




#endif // __PASSTHRU_H__
