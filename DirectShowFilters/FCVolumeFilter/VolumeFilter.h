#pragma once
/*
	VolumeFilter.h
	Declaration of VolumeFilter class (transform filter)

	Kevin Dixon
	(c) 2009 Future Concepts
	01/20/2009
*/

#include <streams.h>
#include <math.h>

#include "IFCVolumeMute.h"

class VolumeFilter : public CTransformFilter, public IFCVolumeMute
{
private:
    CMediaType mtIn;

	int m_volumeDB;
	double m_volumeScaleFactor;
	BOOL m_muted;
public:

	//auto-implements the IUnknown junk
	DECLARE_IUNKNOWN

    // constructor & destructor
    VolumeFilter(LPUNKNOWN pUnk, HRESULT *phr);
    virtual ~VolumeFilter();
    static CUnknown * WINAPI CreateInstance(LPUNKNOWN pUnk, HRESULT * phr);

   // CTransformFilter overriden
    virtual HRESULT CheckInputType(const CMediaType * mtIn);
    virtual HRESULT CheckTransform(const CMediaType * mtIn, const CMediaType * mtOut);
    virtual HRESULT DecideBufferSize(IMemAllocator * pAlloc, ALLOCATOR_PROPERTIES * pProp); 
    virtual HRESULT GetMediaType(int iPosition, CMediaType * pmt);
    virtual HRESULT SetMediaType(PIN_DIRECTION direction, const CMediaType * pmt);
    virtual HRESULT Transform(IMediaSample * pIn, IMediaSample * pOut);

	STDMETHODIMP set_Volume(int attenuationInDB);
	STDMETHODIMP get_Volume(int * attenuationInDB);

	STDMETHODIMP set_Mute(BOOL muted);
	STDMETHODIMP get_Mute(BOOL * muted);

private:
	// Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);
};