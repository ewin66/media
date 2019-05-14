#ifndef VIDEOSTREAM_H
#define VIDEOSTREAM_H

//------------------------------------------------------------------------------
// File: VideoStream.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

class VideoStream : public MangoStream
{
public:

    VideoStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName);
    ~VideoStream();

	// CSourceStream

	HRESULT DecideBufferSize(IMemAllocator *pIMemAlloc,
                             ALLOCATOR_PROPERTIES *pProperties);

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

protected:
	// MangoStream

	int GetD2HTimeout();

	void AddFilters();
	void SetAndAttachPins();
	void ConnectFilters();
	void DoFramePostProcessing(IMediaSample* pms) { pms->SetSyncPoint(TRUE); }
	void DeleteFilters();

protected:

    int					m_iImageHeight;
    int					m_iImageWidth;

	CFilterVideoIn*		m_pVidin_filter;
	CFilterFramerate*	m_pFramerate_filter;
	CFilterFramerate*	m_pFramerate2_filter;
	int					m_iCam;
	enImageSize			m_eImagesize;
	//frame rate defined in frame-skip units
	int					m_iFrameRate;

}; // VideoStream

#endif
