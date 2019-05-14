/*
	FrameRateFilter.cpp
	Implementation of FrameRateFilter class (transform filter)

	Kevin Dixon
	(c) 2009 Future Concepts
	01/27/2009
*/

#include "FrameRateFilter.h"
#include "uuids.h"

FrameRateFilter::FrameRateFilter(LPUNKNOWN pUnk, HRESULT * phr)
	: CTransInPlaceFilter(L"FC Frame Rate Filter", pUnk, CLSID_FCFrameRateFilter, phr, false),
	  m_OneSecond(10000000.0)
{

	m_InputFramerate = 30.0;
	m_TargetFramerate = 10.0;

	m_partialIncrement = 0.0;
	m_integerKeep = 0;

	m_partialCounter = 0;
	m_integerCounter = 0;

	UpdateCalculations();

    if (phr)
	{
		*phr = NOERROR;
	}
}

FrameRateFilter::~FrameRateFilter()
{
}

CUnknown *WINAPI FrameRateFilter::CreateInstance(LPUNKNOWN pUnk, HRESULT * phr)
{
    FrameRateFilter * frameRateFilter = new FrameRateFilter(pUnk, phr);
    if (!frameRateFilter)
	{
        if (phr)
		{
			*phr = E_OUTOFMEMORY;
		}
    }
    return frameRateFilter;
}

HRESULT FrameRateFilter::CheckInputType(const CMediaType * mtIn)
{
	if (mtIn->majortype != MEDIATYPE_Video)
	{
		return E_FAIL;
	}

	if ((mtIn->subtype == MEDIASUBTYPE_YVU9) ||
		(mtIn->subtype == MEDIASUBTYPE_Y411) ||
		(mtIn->subtype == MEDIASUBTYPE_Y41P) ||
		(mtIn->subtype == MEDIASUBTYPE_YUY2) ||
		(mtIn->subtype == MEDIASUBTYPE_YVYU) ||
		(mtIn->subtype == MEDIASUBTYPE_UYVY) ||
		(mtIn->subtype == MEDIASUBTYPE_RGB1) ||
		(mtIn->subtype == MEDIASUBTYPE_RGB4) ||
		(mtIn->subtype == MEDIASUBTYPE_RGB8) ||
		(mtIn->subtype == MEDIASUBTYPE_RGB565) ||
		(mtIn->subtype == MEDIASUBTYPE_RGB555) ||
		(mtIn->subtype == MEDIASUBTYPE_RGB24) ||
		(mtIn->subtype == MEDIASUBTYPE_RGB32) ||
		(mtIn->subtype == MEDIASUBTYPE_MJPG))
	{
		return NOERROR;
	}

	return E_FAIL;
}

HRESULT FrameRateFilter::Transform(IMediaSample * sample)
{
    //pass all samples if they are trying to milk more frames than we're getting
    if (m_TargetFramerate >= m_InputFramerate)
    {
        return S_OK;
    }

    m_integerCounter += 1;

    HRESULT keep = S_FALSE;

    if ((m_integerCounter == m_integerKeep) && (m_integerKeep > 0))
    {
        m_integerCounter = 0;
        m_partialCounter += m_partialIncrement;

        keep = S_OK;
    }
    
    if (((ApproxEqualTo(m_partialCounter, (m_integerKeep * m_partialIncrement)) && (m_integerKeep != 1)) ||
        ((m_integerKeep == 1) && GreaterThanOrEqualTo(m_partialCounter, 1))) &&
         (m_partialCounter > 0))
    {
        m_partialCounter = 0;

        if (GreaterThanOrEqualTo(m_TargetFramerate, (m_InputFramerate / 2.0)))
        {
            keep = S_FALSE;
        }
        else
        {
            keep = S_OK;
        }
    }

    return keep;
}

void FrameRateFilter::UpdateCalculations()
{
    //calculate discard ratio and intervals
    double discardRatio = m_InputFramerate / m_TargetFramerate;

    m_integerKeep = (long)discardRatio;

    if (m_TargetFramerate < 1)
    {
        m_partialIncrement = 0;
    }
    else
    {
        m_partialIncrement = (discardRatio - ((long)discardRatio));
    }

	if (m_integerCounter >= m_integerKeep)
	{
		m_integerCounter = m_integerKeep - 1;
	}
	m_partialCounter = 0;
}

bool FrameRateFilter::ApproxEqualTo(double a, double b)
{
    double precision = 0.00001;
    double difference = a - b;
    return ((difference >= 0) && (difference < precision)) ||
           ((difference < 0) && (difference > (-1 * precision)));
}

bool FrameRateFilter::GreaterThanOrEqualTo(double lhs, double rhs)
{
    return (lhs > rhs) || ApproxEqualTo(lhs, rhs);
}

bool FrameRateFilter::LessThanOrEqualTo(double lhs, double rhs)
{
    return (lhs < rhs) || ApproxEqualTo(lhs, rhs);
}

//
// IFCFrameRateAPI Interface implementation
//
/*
	Sets the Input Framerate
	[in] inputFPS	the input frame rate, in frames per second
*/
STDMETHODIMP FrameRateFilter::set_InputFramerate(double inputFPS)
{
	m_InputFramerate = inputFPS;
	UpdateCalculations();
	return S_OK;
}

/*
	Gets the last set Input Framerate
	[out] inputFPS	the last set input frame rate, in frames per second
*/
STDMETHODIMP FrameRateFilter::get_InputFramerate(double * inputFPS)
{
	(*inputFPS) = m_InputFramerate;
	return S_OK;
}

/*
	Sets the Target Framerate
	[in] targetFPS	the target frame rate, in frames per second.
*/
STDMETHODIMP FrameRateFilter::set_TargetFramerate(double targetFPS)
{
	m_TargetFramerate = targetFPS;
	UpdateCalculations();
	return S_OK;
}

/*
	Gets the last set Target Framerate
	[out] targetFPS	the last set target frame rate, in frames per second
*/
STDMETHODIMP FrameRateFilter::get_TargetFramerate(double * targetFPS)
{
	(*targetFPS) = m_TargetFramerate;
	return S_OK;
}

//
// NonDelegatingQueryInterface
//
// Override this to say what interfaces we support where
//
STDMETHODIMP FrameRateFilter::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
    CheckPointer(ppv, E_POINTER);

	if (riid == IID_IFCFrameRateAPI)
	{
		return GetInterface(static_cast<IFCFrameRateAPI*>(this), ppv);
	} 
	else if (riid == IID_IBaseFilter || riid == IID_IMediaFilter || riid == IID_IPersist)
	{
		return CTransformFilter::NonDelegatingQueryInterface(riid, ppv);
    }

    return CUnknown::NonDelegatingQueryInterface(riid, ppv);
}