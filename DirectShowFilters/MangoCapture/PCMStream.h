#ifndef PCMSTREAM_H
#define PCMSTREAM_H

//------------------------------------------------------------------------------
// File: PCMStream.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

class PCMStream : public MangoStream
{

public:

    PCMStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName);
    ~PCMStream();

    // Ask for buffers of the size appropriate to the agreed media type
    HRESULT DecideBufferSize(IMemAllocator *pIMemAlloc,
                             ALLOCATOR_PROPERTIES *pProperties);

    HRESULT GetMediaType(CMediaType *pmt);

	STDMETHODIMP GetPages(CAUUID* pPages)
	{
		if (pPages == NULL)
		{
			return E_POINTER;
		}
		return S_FALSE;
	}

	// ICodecAPI

	HRESULT STDMETHODCALLTYPE SetValue(const GUID* api, VARIANT* Value) { return S_OK; }

	// Pure Virtuals from MangoStream
protected:
	int GetD2HTimeout();

	void AddFilters();
	void SetAndAttachPins();
	void ConnectFilters();
	void DoFramePostProcessing(IMediaSample* pms) { pms->SetSyncPoint(TRUE); }
	void DeleteFilters() {};

private:

	static const int MAX_BUFSIZ = 1152 * 2; /* 1152 samples/frame, 2 bytes per sample */

	CFilterAudioIn*		m_pAudio_filter;
	int					m_iMic;
}; // PCMStream

#endif
