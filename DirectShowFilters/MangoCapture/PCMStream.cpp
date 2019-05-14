//------------------------------------------------------------------------------
// File: PCMStream.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

#pragma warning(disable:4710)  // 'function': function not inlined (optimzation)

//
// Constructor
//
PCMStream::PCMStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName) :
    MangoStream(phr, pParent, pPinName),
	m_iMic(0),
	m_pAudio_filter(NULL)
{
} // (Constructor)

//
// Destructor
//
PCMStream::~PCMStream()
{
}


//
// GetMediaType
//
HRESULT PCMStream::GetMediaType(CMediaType* pmt)
{
    CheckPointer(pmt, E_POINTER);

    CAutoLock cAutoLock(m_pFilter->pStateLock());

    WAVEFORMATEX *pwf = (WAVEFORMATEX*) pmt->AllocFormatBuffer(sizeof(WAVEFORMATEX));
    if(NULL == pwf)
	{
        return(E_OUTOFMEMORY);
	}

    ZeroMemory(pwf, sizeof(WAVEFORMATEX));

	pwf->cbSize = 0;
	pwf->nBlockAlign = 2;
	pwf->nChannels = 1;
	pwf->wBitsPerSample = 16;
	pwf->nSamplesPerSec = 48000;
	pwf->nAvgBytesPerSec = 96000;
	pwf->wFormatTag = WAVE_FORMAT_PCM;

    pmt->SetType(&MEDIATYPE_Audio);
    pmt->SetFormatType(&FORMAT_WaveFormatEx);
    pmt->SetTemporalCompression(FALSE);

    pmt->SetSubtype(&MEDIASUBTYPE_PCM);
    pmt->SetSampleSize(MAX_BUFSIZ);
    return NOERROR;

} // GetMediaType

//
// DecideBufferSize
//
// This will always be called after the format has been sucessfully
// negotiated. So we have a look at m_mt to see what size image we agreed.
// Then we can ask for buffers of the correct size to contain them.
//
HRESULT PCMStream::DecideBufferSize(IMemAllocator *pAlloc, ALLOCATOR_PROPERTIES *pProperties)
{
    CheckPointer(pAlloc,E_POINTER);
    CheckPointer(pProperties,E_POINTER);

    CAutoLock cAutoLock(m_pFilter->pStateLock());
    HRESULT hr = NOERROR;

    WAVEFORMATEX* pwf = (WAVEFORMATEX*) m_mt.Format();
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

int PCMStream::GetD2HTimeout()
{
	return MANGOBIOS_FOREVER;
}

void PCMStream::AddFilters()
{
	// FilterAudioIn

	m_pAudio_filter = new CFilterAudioIn(2, m_iMic, 0, 2, MX_LINE_IN );
	m_iNumOfFilters++;

	MangoStream::AddFilters();
}

void PCMStream::SetAndAttachPins()
{
	m_pGraph->SetFilter(m_pAudio_filter, 0);
	m_pGraph->SetFilter(m_pD2H_filter, 1);
	m_pGraph->SetFilter(m_pTimeTag_filter, 2);
	m_pGraph->Attach(0, 1, 0, 0);
	m_pGraph->Attach(0, 2, 1, 0);
}

void PCMStream::ConnectFilters()
{
	MANGOERROR_error_t mangoStatus;
	PCI_STREAM_ptr_t buf;
	int bufsiz;

	if (m_pTimeTag_filter != NULL)
	{
		mangoStatus = m_pTimeTag_filter->Connect();
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			throw mangoStatus;
		}
	}
	if ((mangoStatus = m_pTimeTag_filter->GetFullBuffer(&buf, &bufsiz)) != MANGOERROR_SUCCESS)
	{
		throw mangoStatus;
	}
	assert(bufsiz == 8);
	memcpy(&m_lastTimeTag, buf.local, 8);
	m_pTimeTag_filter->FreeBuffer(&buf);
}
