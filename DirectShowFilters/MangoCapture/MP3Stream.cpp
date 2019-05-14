//------------------------------------------------------------------------------
// File: MP3Stream.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

#pragma warning(disable:4710)  // 'function': function not inlined (optimzation)

//
// Constructor
//
MP3Stream::MP3Stream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName) :
    MangoStream(phr, pParent, pPinName),
	m_iMic(0),
	m_iBitRate(32),
	m_pAudio_filter(NULL),
	m_pAudioEnc_filter(NULL)
{
    ASSERT(phr);
} // (Constructor)

HRESULT STDMETHODCALLTYPE MP3Stream::SetValue(const GUID* Api, VARIANT* Value)
{
	if (IsEqualGUID(*Api, CODECAPIPARAM_DSP))
	{
		m_iDSP = Value->intVal;
		return S_OK;
	}
	else if (IsEqualGUID(*Api, CODECAPIPARAM_MIC))
	{
		m_iMic = Value->intVal;
		return S_OK;
	}
	else if (IsEqualGUID(*Api, CODECAPIPARAM_AUDIOBITRATE))
	{
		m_iBitRate = Value->intVal;
		return S_OK;
	}
	return S_FALSE;
}

void MP3Stream::AddFilters()
{
	// FilterAudioIn

	m_pAudio_filter = new CFilterAudioIn(2, m_iMic, 0, 2, MX_LINE_IN );

	m_iNumOfFilters++;

	// FilterMp2AEnc

	stMP2A_h2dparams mp2_params;
	mp2_params.nNumBufs = 2;
	mp2_params.nMaxOutputSize = 0x1000;
	mp2_params.nBitrate = m_iBitRate;
	mp2_params.nSampleFreq = 48000;
	mp2_params.enMode = MX_MP2_SINGLE_CHANNEL;
	m_pAudioEnc_filter = new CFilterMp2AEnc(mp2_params);
	m_iNumOfFilters++;
}

void MP3Stream::SetAndAttachPins()
{
	m_pGraph->SetFilter(m_pAudio_filter, 0);
	m_pGraph->SetFilter(m_pAudioEnc_filter, 1);
	m_pGraph->SetFilter(m_pD2H_filter, 2);
	m_pGraph->Attach(0, 1, 0, 0);
	m_pGraph->Attach(1, 2, 0, 0);

	m_rtSampleTime = 0;
}

//
// Destructor
//
MP3Stream::~MP3Stream()
{
}

//
// GetMediaType
//
HRESULT MP3Stream::GetMediaType(CMediaType* pmt)
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
	pwf->wFormatTag = WAVE_FORMAT_MPEGLAYER3;

    pmt->SetType(&MEDIATYPE_Audio);
    pmt->SetFormatType(&FORMAT_WaveFormatEx);
    pmt->SetTemporalCompression(FALSE);

    pmt->SetSubtype(&MEDIASUBTYPE_MPEG1Audio);
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
HRESULT MP3Stream::DecideBufferSize(IMemAllocator *pAlloc, ALLOCATOR_PROPERTIES *pProperties)
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

int MP3Stream::GetDefaultFrameDuration()
{
	return 240000;
}
