#ifndef __MANGOSTREAM_H
#define __MANGOSTREAM_H

//------------------------------------------------------------------------------
// File: MangoStream.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

// {45FC6808-86AF-449c-9E13-002F38DE15E9}
DEFINE_GUID(CLSID_AVCParametersProp, 
0x45fc6808, 0x86af, 0x449c, 0x9e, 0x13, 0x0, 0x2f, 0x38, 0xde, 0x15, 0xe9);

// {8A30671F-1428-49d9-8711-958FD0DBB515}
DEFINE_GUID(CODECAPIPARAM_IMAGESIZE, 
0x8a30671f, 0x1428, 0x49d9, 0x87, 0x11, 0x95, 0x8f, 0xd0, 0xdb, 0xb5, 0x15);

// {577C8A55-FFC7-4705-A546-C61A4BC093BD}
DEFINE_GUID(CODECAPIPARAM_FRAMERATE, 
0x577c8a55, 0xffc7, 0x4705, 0xa5, 0x46, 0xc6, 0x1a, 0x4b, 0xc0, 0x93, 0xbd);

// {BB6CCFD1-1ECC-4090-A080-5CA9A49FD6D0}
DEFINE_GUID(CODECAPIPARAM_KEYFRAMERATE, 
0xbb6ccfd1, 0x1ecc, 0x4090, 0xa0, 0x80, 0x5c, 0xa9, 0xa4, 0x9f, 0xd6, 0xd0);

// {CD5C00C3-57A2-4fc0-8158-635D794D062C}
DEFINE_GUID(CODECAPIPARAM_RESET, 
0xcd5c00c3, 0x57a2, 0x4fc0, 0x81, 0x58, 0x63, 0x5d, 0x79, 0x4d, 0x6, 0x2c);

// {321a884b-0d97-49f3-816b-5ad7263e044a}
DEFINE_GUID(CODECAPIPARAM_DSP,
0x321a884b, 0x0d97, 0x49f3, 0x81, 0x6b, 0x5a, 0xd7, 0x26, 0x3e, 0x04, 0x4a);

// {6394a130-cd8e-481e-aac7-0e581722839c}
DEFINE_GUID(CODECAPIPARAM_CAM,
0x6394a130, 0xcd8e, 0x481e, 0xaa, 0xc7, 0x0e, 0x58, 0x17, 0x22, 0x83, 0x9c);

// {403e6a13-3079-4ca9-bafb-9a0c34a7b6e4}
DEFINE_GUID(CODECAPIPARAM_MIC,
0x403e6a13, 0x3079, 0x4ca9, 0xba, 0xfb, 0x9a, 0x0c, 0x34, 0xa7, 0xb6, 0xe4);

// {6DBBD171-C8B0-4fee-B7CB-8CF2E33F7722}
DEFINE_GUID(CODECAPIPARAM_AUDIOBITRATE, 
0x6dbbd171, 0xc8b0, 0x4fee, 0xb7, 0xcb, 0x8c, 0xf2, 0xe3, 0x3f, 0x77, 0x22);

//------------------------------------------------------------------------------
// Class MangoStream
//
//------------------------------------------------------------------------------
class MangoStream : ISpecifyPropertyPages, ICodecAPI, public CSourceStream
{
public:

    MangoStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName);
    ~MangoStream();

    // gets a frame from MangoX
    HRESULT FillBuffer(IMediaSample *pms);

    // Ask for buffers of the size appropriate to the agreed media type
    virtual HRESULT DecideBufferSize(IMemAllocator *pIMemAlloc,
                             ALLOCATOR_PROPERTIES *pProperties) = 0;

    // Set the agreed media type, and set up the necessary parameters
    HRESULT SetMediaType(const CMediaType *pMediaType);

    HRESULT CheckMediaType(const CMediaType *pMediaType);
    virtual HRESULT GetMediaType(CMediaType *pmt) = 0;

    // Resets the stream time to zero
    HRESULT OnThreadCreate(void);
	HRESULT OnThreadDestroy(void);

    // Quality control notifications sent to us
    STDMETHODIMP Notify(IBaseFilter * pSender, Quality q);

	bool SetSampleTime(IMediaSample* pms);

	// ISpecifyPropertyPages

	virtual STDMETHODIMP GetPages(CAUUID* pPages) = 0;

	// ICodecAPI

	HRESULT STDMETHODCALLTYPE GetAllSettings(IStream* pStream) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE GetDefaultValue(const GUID* Api, VARIANT* Value) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE GetParameterRange(const GUID* Api, VARIANT* ValueMin, VARIANT* ValueMax, VARIANT* SteppingDelta) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE GetParameterValues(const GUID* Api, VARIANT** Values, ULONG* ValuesCount) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE GetValue(const GUID* Api, VARIANT* Value) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE IsModifiable(const GUID* Api) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE IsSupported(const GUID* Api) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE RegisterForEvent(const GUID* Api, LONG_PTR userData) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE SetAllDefaults() { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE SetAllDefaultsWithNotify(GUID** ChangedParam, ULONG* ChangedParamCount) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE SetAllSettings(IStream* pStream) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE SetAllSettingsWithNotify(IStream* pStream, GUID** ChangedParam, ULONG* ChangedParamCount) { return S_FALSE; }

	virtual HRESULT STDMETHODCALLTYPE SetValue(const GUID* api, VARIANT* Value) = 0;

	HRESULT STDMETHODCALLTYPE SetValueWithNotify(const GUID* Api, VARIANT* Value, GUID** ChangedParam, ULONG* ChanagedParamCount) { return S_FALSE; }

	HRESULT STDMETHODCALLTYPE UnregisterForEvent(const GUID* Api) { return S_FALSE; }

	// IUnknown

	DECLARE_IUNKNOWN;

	STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void **ppv)
	{
		if (riid == IID_ISpecifyPropertyPages)
		{
			return GetInterface(static_cast<ISpecifyPropertyPages*>(this), ppv);
	    }
		else if (riid == IID_ICodecAPI)
		{
			return GetInterface(static_cast<ICodecAPI*>(this), ppv);
		}
		else return CSourceStream::NonDelegatingQueryInterface(riid, ppv);
	}

protected:
	virtual void AddFilters();
	virtual void SetAndAttachPins() = 0;
	virtual void ConnectFilters();
	virtual void DoFramePostProcessing(IMediaSample* pms) = 0;
	virtual void DeleteFilters();

	/*
		Gets the timeout for the DSP to Host and TimeTag filter.
		Returns the timeout in milliseconds
	*/
	inline virtual int GetD2HTimeout();

protected:
	void CreateGraph();
	void DeleteGraph();

protected:

	static const int MAX_BUFSIZ = 0x80000;

    CCritSec			m_cSharedState;
    CRefTime			m_rtSampleTime;

	CMangoXGraph*		m_pGraph;
	CFilterD2H_PCI*		m_pD2H_filter;
	CFilterD2H_PCI*		m_pTimeTag_filter;

	int					m_iNumOfFilters;
	int					m_iGraphNum;
	int					m_iDSP;

	int					m_iFrameCount;

	BOOL				m_bGraphIsCreated;

	Time_tag_T			m_lastTimeTag;


}; // MangoStream

#endif
