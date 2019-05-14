/*
	TimeStampAdjust class

	Kevin Dixon
	Copyright (c) 2009 Future Concepts
	12/22/2009
*/

#include <stdio.h>
#include <streams.h>
#include <initguid.h>
#include <olectl.h>
#include <comutil.h>
#include "Inline.h"
#include "TimeStampGuids.h"
#include "IFCTimeStampAdjust.h"

class TimeStampAdjust : public CTransInPlaceFilter, public IFCTimeStampAdjust
{
private:

	//passive, no changes made
	static const int Strategy_NoChange		= 0;
	//Sets the Discontinuity flag when an abrupt timestamp change occurs
	static const int Strategy_SetDiscont	= 1;
	//Alters the time stamp when an abrupt timestamp change occurs
	static const int Strategy_ChangeStamp	= 2;

	static const int Log_Off			= 0;
	static const int Log_Changes		= 1;
	static const int Log_All			= 3;

public:

    static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    DECLARE_IUNKNOWN;

    HRESULT CheckInputType(const CMediaType *mtIn);

	STDMETHODIMP set_Logging(int level);
	STDMETHODIMP get_Logging(int * level);

	STDMETHODIMP set_LogPath(BSTR path);

	STDMETHODIMP set_Strategy(int strategy);
	STDMETHODIMP get_Strategy(int * strategy);

private:

    // Constructor
    TimeStampAdjust(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr);
	~TimeStampAdjust();

    // Overrides the PURE virtual Transform of CTransInPlaceFilter base class
    // This is where the "real work" is done.
    HRESULT Transform(IMediaSample * pSample);

    // Overrides a CTransformInPlace function.  Called as part of connecting.
    virtual HRESULT SetMediaType(PIN_DIRECTION direction, const CMediaType * pmt);

	//override to support querying interfaces
	STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

	void DumpSampleProperties(AM_SAMPLE2_PROPERTIES * props, REFERENCE_TIME stampDelta, double realTimeDelta);

	inline HRESULT IsOutlier(REFERENCE_TIME lastDelta, REFERENCE_TIME thisDelta);

	void UpdateRunningAverage(REFERENCE_TIME thisDelta);

private:
	
	int					m_strategy;
	int					m_logging;

	REFERENCE_TIME		m_rtSampleTime;
	REFERENCE_TIME		m_tStartLast;
	LARGE_INTEGER		m_tickFrequency;
	LARGE_INTEGER		m_tickLast;
	FILE*				m_fp;
	int					m_frameCounter;
	REFERENCE_TIME		m_startTime;

	REFERENCE_TIME		m_lastStampDelta;

	REFERENCE_TIME		m_averageDelta;

};
