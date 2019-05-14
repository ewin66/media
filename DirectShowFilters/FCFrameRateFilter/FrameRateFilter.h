#pragma once
/*
	FrameRateFilter.h
	Declaration of FrameRateFilter class (transform filter)

	Kevin Dixon
	(c) 2009 Future Concepts
	01/20/2009
*/

#include <streams.h>
#include <math.h>

#include "IFCFrameRateAPI.h"

class FrameRateFilter : public CTransInPlaceFilter, public IFCFrameRateAPI
{
private:
	double m_TargetFramerate;				//target frames per second

	REFERENCE_TIME m_InputAvgTimePerFrame;	//duration of input frame in REFERENCE TIME
	double m_InputFramerate;				//input frames per second

	double m_partialIncrement;		//the number of partial frames per frame
	long m_integerKeep;					//this is the total number of frames in a cycle.

	double m_partialCounter;				//the current total of partial frames seen. When this >= 1, we discard a frame
	long m_integerCounter;					//the current frames seen. When this == m_integerDiscard, we discard that frame

	const double m_OneSecond;

public:

	//auto-implements the IUnknown junk
	DECLARE_IUNKNOWN

    // constructor & destructor
    FrameRateFilter(LPUNKNOWN pUnk, HRESULT *phr);
    virtual ~FrameRateFilter();
    static CUnknown * WINAPI CreateInstance(LPUNKNOWN pUnk, HRESULT * phr);

    virtual HRESULT CheckInputType(const CMediaType * mtIn);
    virtual HRESULT Transform(IMediaSample * sample);

	STDMETHODIMP set_InputFramerate(double inputFPS);
	STDMETHODIMP get_InputFramerate(double * inputFPS);

	STDMETHODIMP set_TargetFramerate(double targetFPS);
	STDMETHODIMP get_TargetFramerate(double * targetFPS);

private:
	// Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

	void UpdateCalculations();
	inline bool ApproxEqualTo(double a, double b);
	inline bool GreaterThanOrEqualTo(double lhs, double rhs);
	inline bool LessThanOrEqualTo(double lhs, double rhs);
};