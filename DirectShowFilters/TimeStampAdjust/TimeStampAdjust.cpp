/*
	Implementation of TimeStampAdjust class

	Kevin Dixon
	Copyright (c) 2009 Future Concepts
	12/22/2009
*/

#include "TimeStampAdjust.h"

// Put out the name of a function and instance on the debugger.
// Invoke this at the start of functions to allow a trace.
#define DbgFunc(a) DbgLog(( LOG_TRACE                        \
                          , 2                                \
                          , TEXT("TimeStampAdjust::%s") \
                          , TEXT(a)                          \
                         ));

TimeStampAdjust::TimeStampAdjust(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr)
    : CTransInPlaceFilter (tszName, punk, CLSID_TimeStampAdjust, phr)
{
    DbgFunc("TimeStampAdjust");


	m_logging = Log_All;
	m_strategy = Strategy_ChangeStamp;

	m_lastStampDelta = 0;
	m_averageDelta = 0;

	m_rtSampleTime = 0;
	m_tStartLast = 0;
	m_frameCounter = 0;
	m_startTime;

	m_fp = NULL;

	QueryPerformanceFrequency(&m_tickFrequency);
	QueryPerformanceCounter(&m_tickLast);

} // (CTimeStamp constructor)

TimeStampAdjust::~TimeStampAdjust()
{
	if(m_fp != NULL)
	{
		fclose(m_fp);
	}
}

/*
	Fetch the 
*/
STDMETHODIMP TimeStampAdjust::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
    CheckPointer(ppv, E_POINTER);

    if (riid == IID_IFCTimeStampAdjust)
    {
		return GetInterface(static_cast<IFCTimeStampAdjust*>(this), ppv);
    } 
    else if (riid == IID_IBaseFilter || riid == IID_IMediaFilter || riid == IID_IPersist)
    {
		return CTransInPlaceFilter::NonDelegatingQueryInterface(riid, ppv);
    }

    return CUnknown::NonDelegatingQueryInterface(riid, ppv);
}


//
// CreateInstance
//
// Override CClassFactory method.
// Provide the way for COM to create a CTimeStamp object.
//
CUnknown * WINAPI TimeStampAdjust::CreateInstance(LPUNKNOWN punk, HRESULT *phr)
{
    ASSERT(phr);
    
    TimeStampAdjust *pNewObject = new TimeStampAdjust(NAME("TimeStampAdjust Filter"), punk, phr);

    if (pNewObject == NULL)
	{
        if (phr)
            *phr = E_OUTOFMEMORY;
    }

    return pNewObject;

} // CreateInstance

//
// Transform
//
// Override CTransInPlaceFilter method.
// Convert the input sample into the output sample.
//
HRESULT TimeStampAdjust::Transform(IMediaSample *pSample)
{
    CheckPointer(pSample,E_POINTER);   
	IMediaSample2 *pSample2;
	LARGE_INTEGER tickNow;
	QueryPerformanceCounter(&tickNow);
	//convert proc' ticks to reference time
	double realTimeDelta = (tickNow.LowPart - m_tickLast.LowPart) * 10000000.0 / m_tickFrequency.LowPart;
	m_tickLast = tickNow;

	if (SUCCEEDED(pSample->QueryInterface(IID_IMediaSample2,(void **)&pSample2)))
	{
	    /*  Modify it */
		AM_SAMPLE2_PROPERTIES outProps; 
		if (SUCCEEDED(pSample2->GetProperties(48, (PBYTE)&outProps)))
		{
			REFERENCE_TIME stampDelta = outProps.tStart - m_tStartLast;

			if(m_frameCounter > 0)
			{
				HRESULT outlier = IsOutlier(m_lastStampDelta, stampDelta);
				if(outlier == S_OK)
				{
					if(m_strategy == Strategy_SetDiscont)
					{
						outProps.dwSampleFlags |= AM_SAMPLE_TIMEDISCONTINUITY | AM_SAMPLE_DATADISCONTINUITY;
						pSample2->SetProperties(sizeof(AM_SAMPLE2_PROPERTIES), (const PBYTE)(&outProps));
						if((m_logging & Log_Changes) && (m_fp != NULL))
						{
							fprintf(m_fp, "%d\tSet Discontinuity TRUE\n", m_frameCounter);
						}
					}
					else if(m_strategy == Strategy_ChangeStamp)
					{
						REFERENCE_TIME newStart = m_tStartLast + m_averageDelta;
						REFERENCE_TIME newEnd = newStart + m_averageDelta;
						stampDelta = newStart - m_tStartLast;
						pSample2->SetTime(&newStart, &newEnd);
						if((m_logging & Log_Changes) && (m_fp != NULL))
						{
							fprintf(m_fp, "%d\tChange Time start %I64u => %I64u end %I64u => %I64u\n", m_frameCounter, outProps.tStart, newStart, outProps.tStop, newEnd);
						}
						
						pSample2->GetProperties(sizeof(AM_SAMPLE2_PROPERTIES), (PBYTE)&outProps);
					}
				}

				//if we are using the SetDiscont strat, or we didn't change the time stamp, record it as data
				if ((m_strategy == Strategy_SetDiscont) ||
					((outlier != S_OK) && (m_strategy == Strategy_ChangeStamp)))
				{
					m_lastStampDelta = stampDelta;
					UpdateRunningAverage(stampDelta);
				}
			}

			if((m_logging & Log_All) && (m_fp != NULL))
			{
				DumpSampleProperties(&outProps, stampDelta, realTimeDelta);
			}
			
			m_tStartLast = outProps.tStart;
		}
		pSample2->Release();
	}

	m_frameCounter++;
    return NOERROR;

}

/*
	Determines if the current time-delta should be considered an outlier from the last time-delta
	lastDelta	the last known delta, use "0" if unknown
	thisDelta	the current delta to check
	returns E_FAIL if lastDelta is 0, returns S_OK if it is an outlier, or S_FALSE if not
*/
HRESULT TimeStampAdjust::IsOutlier(REFERENCE_TIME lastDelta, REFERENCE_TIME thisDelta)
{
	if(lastDelta == 0) return E_FAIL;

	return ((thisDelta < (lastDelta / 4) || (thisDelta > lastDelta * 4))) ? S_OK : S_FALSE;
}

void TimeStampAdjust::UpdateRunningAverage(REFERENCE_TIME thisDelta)
{
	if(m_averageDelta == 0)
	{
		m_averageDelta = thisDelta;
	}
	else
	{
		m_averageDelta = (m_averageDelta + thisDelta) / 2;
	}
}

void TimeStampAdjust::DumpSampleProperties(AM_SAMPLE2_PROPERTIES* props, REFERENCE_TIME stampDelta, double realTimeDelta)
{
	char octet = props->pbBuffer[0];
	fprintf(m_fp, "%d,%x,%d,%I64u,%I64u,", m_frameCounter, octet, props->lActual, props->tStart, props->tStop);
	if (props->dwSampleFlags & AM_SAMPLE_SPLICEPOINT)
	{
		fprintf(m_fp, "SPLICEPOINT ");
	}
	if (props->dwSampleFlags & AM_SAMPLE_PREROLL)
	{
		fprintf(m_fp, "PREROLL ");
	}
	if (props->dwSampleFlags & AM_SAMPLE_DATADISCONTINUITY)
	{
		fprintf(m_fp, "DATADISCONTINUITY ");
	}
	if (props->dwSampleFlags & AM_SAMPLE_TYPECHANGED)
	{
		fprintf(m_fp, "TYPECHANGED ");
	}
	if (props->dwSampleFlags & AM_SAMPLE_TIMEDISCONTINUITY)
	{
		fprintf(m_fp, "TIMEDISCONTINUITY ");
	}
	if (props->dwSampleFlags & AM_SAMPLE_FLUSH_ON_PAUSE)
	{
		fprintf(m_fp, "FLUSH_ON_PAUSE ");
	}
	if (props->dwSampleFlags & AM_SAMPLE_STOPVALID)
	{
		fprintf(m_fp, "STOPVALID ");
	}
	if (props->dwSampleFlags & AM_SAMPLE_ENDOFSTREAM)
	{
		fprintf(m_fp, "ENDOFSTREAM ");
	}
	if (props->dwSampleFlags & AM_STREAM_MEDIA)
	{
		fprintf(m_fp, "MEDIA ");
	}
	if (props->dwSampleFlags & AM_STREAM_CONTROL)
	{
		fprintf(m_fp, "CONTROL ");
	}
	
	fprintf(m_fp, ",%I64d,%f\n", stampDelta, realTimeDelta);
}

STDMETHODIMP TimeStampAdjust::set_Logging(int level)
{
	m_logging = level;
	return S_OK;
}

STDMETHODIMP TimeStampAdjust::get_Logging(int * level)
{
	(*level) = m_logging;
	return S_OK;
}

STDMETHODIMP TimeStampAdjust::set_LogPath(BSTR path)
{
	if(m_fp != NULL)
	{
		fclose(m_fp);
	}
	if(path != NULL)
	{
		char * cPath = _com_util::ConvertBSTRToString(path);
		m_fp = fopen(cPath, "w");
		delete [] cPath;
		if(m_fp == NULL)
		{
			return E_FAIL;
		}
		fprintf(m_fp, "sample#,1st byte,length,start,end,flags,stamp delta,real delta\n");
	}

	return S_OK;
}

STDMETHODIMP TimeStampAdjust::set_Strategy(int strategy)
{
	m_strategy = strategy;
	return S_OK;
}

STDMETHODIMP TimeStampAdjust::get_Strategy(int * strategy)
{
	(*strategy) = m_strategy;
	return S_OK;
}

//
// CheckInputType
//
// Override CTransformFilter method.
// Part of the Connect process.
// Ensure that we do not get connected to formats that we can't handle.
// We only work for wave audio, 8 or 16 bit, uncompressed.
//
HRESULT TimeStampAdjust::CheckInputType(const CMediaType *pmt)
{
    CheckPointer(pmt,E_POINTER);

    DisplayType(TEXT("CheckInputType"), pmt);

    return NOERROR;

} // CheckInputType


//
// SetMediaType
//
// Override CTransformFilter method.
// Called when a connection attempt has succeeded. If the output pin
// is being connected and the input pin's media type does not agree then we
// reconnect the input (thus allowing its media type to change,) and vice versa.
//
HRESULT TimeStampAdjust::SetMediaType(PIN_DIRECTION direction,const CMediaType *pmt)
{
    CheckPointer(pmt,E_POINTER);
    DbgFunc("SetMediaType");

    // Record what we need for doing the actual transform

    // Call the base class to do its thing
    CTransInPlaceFilter::SetMediaType(direction, pmt);

    // Reconnect where necessary.
    if( m_pInput->IsConnected() && m_pOutput->IsConnected() )
    {
        FILTER_INFO fInfo;

        QueryFilterInfo( &fInfo );

        if (direction == PINDIR_OUTPUT && *pmt != m_pInput->CurrentMediaType() )
            fInfo.pGraph->Reconnect( m_pInput );

        QueryFilterInfoReleaseGraph( fInfo );

        ASSERT(!(direction == PINDIR_INPUT && *pmt != m_pOutput->CurrentMediaType()));
    }

    return NOERROR;

} // SetMediaType

