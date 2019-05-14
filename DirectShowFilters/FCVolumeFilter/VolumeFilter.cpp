/*
	VolumeFilter.cpp
	Implementation of VolumeFilter class (transform filter)

	Kevin Dixon
	(c) 2009 Future Concepts
	01/20/2009
*/

#include "VolumeFilter.h"
#include "uuids.h"

VolumeFilter::VolumeFilter(LPUNKNOWN pUnk, HRESULT * phr)
	: CTransformFilter(L"FC Volume Filter", pUnk, CLSID_FCVolumeFilter)
{
	//initialize member variables
	m_volumeDB = 0;
	m_volumeScaleFactor = 1.0;
	m_muted = false;

    if (phr)
	{
		*phr = NOERROR;
	}
}

VolumeFilter::~VolumeFilter()
{
}

CUnknown *WINAPI VolumeFilter::CreateInstance(LPUNKNOWN pUnk, HRESULT * phr)
{
    VolumeFilter * vol = new VolumeFilter(pUnk, phr);
    if (!vol)
	{
        if (phr)
		{
			*phr = E_OUTOFMEMORY;
		}
    }
    return vol;
}

HRESULT VolumeFilter::CheckInputType(const CMediaType * mtIn)
{
    // we only want raw audio
    if (mtIn->majortype != MEDIATYPE_Audio)
		return E_FAIL;
    if (mtIn->subtype != MEDIASUBTYPE_PCM)
		return E_FAIL;
    if (mtIn->formattype != FORMAT_WaveFormatEx)
		return E_FAIL;

   // and we only want 16-bit
    WAVEFORMATEX    *wfx = (WAVEFORMATEX*)mtIn->pbFormat;
    if (wfx->wBitsPerSample != 16)
		return E_FAIL;

    return NOERROR;
}

HRESULT VolumeFilter::CheckTransform(const CMediaType * mtIn, const CMediaType * mtOut)
{
    HRESULT hr = CheckInputType(mtIn);
    if (FAILED(hr))
		return hr;

   // must also be PCM audio
    if (mtOut->majortype != MEDIATYPE_Audio)
		return E_FAIL;
    if (mtOut->subtype != MEDIASUBTYPE_PCM)
		return E_FAIL;

	//verify that i/o sample rate and bit-depth are the same
	WAVEFORMATEX    *wfxIn = (WAVEFORMATEX*)mtIn->pbFormat;
	WAVEFORMATEX    *wfxOut= (WAVEFORMATEX*)mtOut->pbFormat;
    if ((wfxIn->nSamplesPerSec != wfxOut->nSamplesPerSec) ||
		(wfxIn->wBitsPerSample != wfxOut->wBitsPerSample))
		return E_FAIL;

    return NOERROR;
}

HRESULT VolumeFilter::SetMediaType(PIN_DIRECTION direction, const CMediaType * pmt)
{
    // ask the baseclass if it’s okay to go
    HRESULT    hr = CTransformFilter::SetMediaType(direction, pmt);
    if (FAILED(hr))
		return hr;

    // keep a local copy of the type
    if (direction == PINDIR_INPUT)
	{
        mtIn = *pmt;
    }

    return NOERROR;
}

HRESULT VolumeFilter::GetMediaType(int iPosition, CMediaType *pmt)
{
    if (iPosition < 0)
		return E_INVALIDARG;
    if (iPosition > 0)
		return VFW_S_NO_MORE_ITEMS;

    // the input pin must be connected first
    if (m_pInput->IsConnected() == FALSE)
		return VFW_S_NO_MORE_ITEMS;

   // we offer only one type - the same as input
    *pmt = mtIn;
    return NOERROR;
}

HRESULT VolumeFilter::DecideBufferSize(IMemAllocator * pAlloc, ALLOCATOR_PROPERTIES * pProp)
{
    WAVEFORMATEX * wfx = (WAVEFORMATEX*)mtIn.pbFormat;

   // this might be put too simly but
    // we should be able to deliver max 1 second
    // of raw audio
    pProp->cbBuffer = wfx->nAvgBytesPerSec;

    // when working with audio always try to have
    // some spare buffer free
    pProp->cBuffers = 3;

    ALLOCATOR_PROPERTIES act;
    HRESULT hr = pAlloc->SetProperties(pProp, &act);
    if (FAILED(hr))
		return hr;

    if (act.cbBuffer < pProp->cbBuffer)
		return E_FAIL;

    return NOERROR;
}

HRESULT VolumeFilter::Transform(IMediaSample * pIn, IMediaSample * pOut)
{
    BYTE * bufin, * bufout;
    long sizein;

    // get the input and output buffers
    pIn->GetPointer(&bufin);
    pOut->GetPointer(&bufout);

    // and get the data size
    sizein = pIn->GetActualDataLength();

	// since we’re dealing with 16-bit PCM
    // it might be convenient to use "short"
    short * source = (short*)bufin;
    short * dest = (short*)bufout;
    int samples = sizein / sizeof(short);

	//for the sake of "some" performance gain, only do the comparison once
	if(m_muted)
	{
		for(int i = 0; i < samples; i++)
		{
			dest[i] = 0;
		}
	}
	else
	{
		for(int i = 0; i < samples; i++)
		{
			dest[i] = (short)(source[i] * m_volumeScaleFactor);
		}
	}

	// and set the data size
    pOut->SetActualDataLength(samples * sizeof(short));
	
    return NOERROR;
}

//
// NonDelegatingQueryInterface
//
// Override this to say what interfaces we support where
//
STDMETHODIMP VolumeFilter::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
    CheckPointer(ppv, E_POINTER);
    //CAutoLock lock(&m_Lock);

	if (riid == IID_IFCVolumeMute)
	{
		return GetInterface(static_cast<IFCVolumeMute*>(this), ppv);
	} 
	else if (riid == IID_IBaseFilter || riid == IID_IMediaFilter || riid == IID_IPersist)
	{
		return CTransformFilter::NonDelegatingQueryInterface(riid, ppv);
    }

    return CUnknown::NonDelegatingQueryInterface(riid, ppv);
}


/*
	Implementation of the IFCVolumeMute interface
*/

/*
	Sets the volume property, and calculates the scale factor
	attenuationInDB		the amount of gain in decibels.
*/
STDMETHODIMP VolumeFilter::set_Volume(int attenuationInDB)
{
	m_volumeDB = attenuationInDB;
	m_volumeScaleFactor = pow(10.0, (attenuationInDB / 20.0));

	return S_OK;
}

/*
	Gets the volume property
	attenuationInDB		the current gain in decibels.
*/
STDMETHODIMP VolumeFilter::get_Volume(int * attenuationInDB)
{
	(*attenuationInDB) = m_volumeDB;

	return S_OK;
}

/*
	Sets the mute property.
	muted	true means pass no audio
*/
STDMETHODIMP VolumeFilter::set_Mute(BOOL muted)
{
	m_muted = muted;

	return S_OK;
}

/*
	Gets the mute property
	muted	true if passing no audio, false if passing audio
*/
STDMETHODIMP VolumeFilter::get_Mute(BOOL * muted)
{
	(*muted) = m_muted;

	return S_OK;
}