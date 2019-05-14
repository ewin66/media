#ifndef MP3STREAM_H
#define MP3STREAM_H

//------------------------------------------------------------------------------
// File: MP3Stream.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

class MP3Stream : public MangoStream
{
public:

    MP3Stream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName);
    ~MP3Stream();

	// From CSourceStream

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

	HRESULT STDMETHODCALLTYPE SetValue(const GUID* api, VARIANT* Value);

	// Pure Virtuals from MangoStream

	void AddFilters();
	void SetAndAttachPins();
	void ConnectFilters() {}
	int GetDefaultFrameDuration();
	void DoFramePostProcessing(IMediaSample* pms) { pms->SetSyncPoint(TRUE); }
	void DeleteFilters() {};

private:

	static const int MAX_BUFSIZ = 1152 * 2; /* 1152 samples/frame, 2 bytes per sample */

	CFilterAudioIn*		m_pAudio_filter;
	CFilterMp2AEnc*		m_pAudioEnc_filter;
	int					m_iMic;
	int					m_iBitRate;
}; // MP3Stream

#endif
