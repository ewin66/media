#ifndef __H264STREAM_H
#define __H264STREAM_H

//------------------------------------------------------------------------------
// File: H264Stream.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Class H264Stream
//
// This class implements the stream which is used to output the H.264 Video
// data from the source filter. It inherits from DirectShows's base
// CSourceStream class.
//------------------------------------------------------------------------------

class H264Stream : public VideoStream
{
public:

    H264Stream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName);
    ~H264Stream();

    HRESULT GetMediaType(CMediaType *pmt);

	// ICodecAPI

	HRESULT STDMETHODCALLTYPE SetValue(const GUID* api, VARIANT* Value);

protected:
	void AddFilters();
	void SetAndAttachPins();
	void ConnectFilters();
	void DoFramePostProcessing(IMediaSample* pms);
	void DeleteFilters();
	void ResetH264();

private:

	CFilterH2D_PCI*		m_pH264ctl_filter;
	CFilterH264VEnc*	m_pH264enc_filter;
	int					m_iBitRate;
	int					m_iKeyFrameRate;

}; // MangoStream

#endif
