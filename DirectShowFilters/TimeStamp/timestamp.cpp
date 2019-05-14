//------------------------------------------------------------------------------
// File: timestamp.cpp
//
//------------------------------------------------------------------------------


//     Base classes used (refer to docs for diagram of what they inherit):
//
// CTransInPlaceFilter

//=============================================================================
//=============================================================================

#include <stdio.h>
#include <streams.h>

// Eliminate two expected level 4 warnings from the Microsoft compiler.
// The class does not have an assignment or copy operator, and so cannot
// be passed by value.  This is normal.  This file compiles clean at the
// highest (most picky) warning level (-W4).
#pragma warning(disable: 4511 4512)


#include <initguid.h>
#if (1100 > _MSC_VER)
#include <olectlid.h>
#else
#include <olectl.h>
#endif
#include "timestampguids.h"             // Our own uuids


//------------------------------------------------------------------------
// CTimeStamp - the TimeStamp filter class
//------------------------------------------------------------------------

class CTimeStamp
    // Inherited classes
    : public CTransInPlaceFilter       // Main DirectShow interfaces

{

public:

    static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    DECLARE_IUNKNOWN;

    //
    // --- CTransInPlaceFilter Overrides --
    //

    HRESULT CheckInputType(const CMediaType *mtIn);

private:

    // Constructor
    CTimeStamp(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr);
	~CTimeStamp();

    // Overrides the PURE virtual Transform of CTransInPlaceFilter base class
    // This is where the "real work" is done.
    HRESULT Transform(IMediaSample *pSample);

    // Overrides a CTransformInPlace function.  Called as part of connecting.
    virtual HRESULT SetMediaType(PIN_DIRECTION direction, const CMediaType *pmt);

	void WriteSampleProperties(AM_SAMPLE2_PROPERTIES* props, double realTimeDelta);

private:
	REFERENCE_TIME		m_rtSampleTime;
	REFERENCE_TIME		m_tStartLast;
	LARGE_INTEGER		m_tickFrequency;
	LARGE_INTEGER		m_tickLast;
	FILE*				m_fp;
	int					m_frameCounter;
	REFERENCE_TIME		m_startTime;

}; // class CTimeStamp


//------------------------------------------------------------------------
// Implementation
//------------------------------------------------------------------------


// Put out the name of a function and instance on the debugger.
// Invoke this at the start of functions to allow a trace.
#define DbgFunc(a) DbgLog(( LOG_TRACE                        \
                          , 2                                \
                          , TEXT("CTimeStamp::%s") \
                          , TEXT(a)                          \
                         ));


// Self-registration data structures

const AMOVIESETUP_MEDIATYPE
sudPinTypes =   { &MEDIATYPE_Video        // clsMajorType
                , &MEDIASUBTYPE_NULL };   // clsMinorType

const AMOVIESETUP_PIN
psudPins[] = { { L"Input"            // strName
               , FALSE               // bRendered
               , FALSE               // bOutput
               , FALSE               // bZero
               , FALSE               // bMany
               , &CLSID_NULL         // clsConnectsToFilter
               , L"Output"           // strConnectsToPin
               , 1                   // nTypes
               , &sudPinTypes        // lpTypes
               }
             , { L"Output"           // strName
               , FALSE               // bRendered
               , TRUE                // bOutput
               , FALSE               // bZero
               , FALSE               // bMany
               , &CLSID_NULL         // clsConnectsToFilter
               , L"Input"            // strConnectsToPin
               , 1                   // nTypes
               , &sudPinTypes        // lpTypes
               }
             };

const AMOVIESETUP_FILTER
sudTimeStamp = { &CLSID_TimeStamp                   // class id
            , L"TimeStamp"                       // strName
            , MERIT_DO_NOT_USE                // dwMerit
            , 2                               // nPins
            , psudPins                        // lpPin
            };

// Needed for the CreateInstance mechanism
CFactoryTemplate g_Templates[1]= { { L"TimeStamp"
                                   , &CLSID_TimeStamp
                                   , CTimeStamp::CreateInstance
                                   , NULL
                                   , &sudTimeStamp
                                   }
                                 };

int g_cTemplates = sizeof(g_Templates)/sizeof(g_Templates[0]);

//
// CTimeStamp::Constructor
//
// Construct a CTimeStamp object.
//
CTimeStamp::CTimeStamp(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr)
    : CTransInPlaceFilter (tszName, punk, CLSID_TimeStamp, phr)
{
    DbgFunc("CTimeStamp");

	m_rtSampleTime = 0;
	m_tStartLast = 0;
	m_frameCounter = 0;
	m_startTime;
	m_fp = fopen("c:\\temp\\timestamp.log", "w");
	fprintf(m_fp, "sample#,1st byte,length,start,end,flags,stamp delta,real delta\n");
	QueryPerformanceFrequency(&m_tickFrequency);
	QueryPerformanceCounter(&m_tickLast);

} // (CTimeStamp constructor)

CTimeStamp::~CTimeStamp()
{
	fclose(m_fp);
}

//
// CreateInstance
//
// Override CClassFactory method.
// Provide the way for COM to create a CTimeStamp object.
//
CUnknown * WINAPI CTimeStamp::CreateInstance(LPUNKNOWN punk, HRESULT *phr)
{
    ASSERT(phr);
    
    CTimeStamp *pNewObject = new CTimeStamp(NAME("TimeStamp Filter"), punk, phr);
    if (pNewObject == NULL) {
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
HRESULT CTimeStamp::Transform(IMediaSample *pSample)
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
			WriteSampleProperties(&outProps, realTimeDelta);
		}
		pSample2->Release();
	}
	m_frameCounter++;
    return NOERROR;

} // Transform

void CTimeStamp::WriteSampleProperties(AM_SAMPLE2_PROPERTIES* props, double realTimeDelta)
{
	REFERENCE_TIME delta = props->tStart - m_tStartLast;
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
	m_tStartLast = props->tStart;
	fprintf(m_fp, ",%I64d,%f\n", delta, realTimeDelta);
}

//
// CheckInputType
//
// Override CTransformFilter method.
// Part of the Connect process.
// Ensure that we do not get connected to formats that we can't handle.
// We only work for wave audio, 8 or 16 bit, uncompressed.
//
HRESULT CTimeStamp::CheckInputType(const CMediaType *pmt)
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
HRESULT CTimeStamp::SetMediaType(PIN_DIRECTION direction,const CMediaType *pmt)
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

////////////////////////////////////////////////////////////////////////
//
// Exported entry points for registration and unregistration 
// (in this case they only call through to default implementations).
//
////////////////////////////////////////////////////////////////////////

STDAPI DllRegisterServer()
{
  return AMovieDllRegisterServer2( TRUE );
}


STDAPI DllUnregisterServer()
{
  return AMovieDllRegisterServer2( FALSE );
}

//
// DllEntryPoint
//
extern "C" BOOL WINAPI DllEntryPoint(HINSTANCE, ULONG, LPVOID);

BOOL APIENTRY DllMain(HANDLE hModule, 
                      DWORD  dwReason, 
                      LPVOID lpReserved)
{
	return DllEntryPoint((HINSTANCE)(hModule), dwReason, lpReserved);
}

#pragma warning(disable: 4514) // "unreferenced inline function has been removed"



