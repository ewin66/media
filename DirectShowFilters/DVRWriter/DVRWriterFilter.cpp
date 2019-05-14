//------------------------------------------------------------------------------
// File: DVRWriter.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

// Setup data

const AMOVIESETUP_MEDIATYPE sudPinTypes =
{
    &MEDIATYPE_NULL,            // Major type
    &MEDIASUBTYPE_NULL          // Minor type
};

const AMOVIESETUP_PIN sudPins =
{
    L"Input",                   // Pin string name
    FALSE,                      // Is it rendered
    FALSE,                      // Is it an output
    FALSE,                      // Allowed none
    FALSE,                      // Likewise many
    &CLSID_NULL,                // Connects to filter
    L"Output",                  // Connects to pin
    1,                          // Number of types
    &sudPinTypes                // Pin information
};

const AMOVIESETUP_FILTER sudDump =
{
    &CLSID_DVRWriter,             // Filter CLSID
    L"DVR Writer",                // String name
    MERIT_DO_NOT_USE,           // Filter merit
    1,                          // Number pins
    &sudPins                    // Pin details
};

//
//  Object creation stuff
//
CFactoryTemplate g_Templates[]= {
    L"DVR Writer", &CLSID_DVRWriter, CDVRWriter::CreateInstance, NULL, &sudDump
};
int g_cTemplates = 1;


// Constructor

CDVRWriterFilter::CDVRWriterFilter(CDVRWriter *pDVRWriter,
                         LPUNKNOWN pUnk,
                         CCritSec *pLock,
                         HRESULT *phr) :
    CBaseFilter(NAME("CDVRWriterFilter"), pUnk, pLock, CLSID_DVRWriter),
    m_pWriter(pDVRWriter)
{
}


//
// GetPin
//
CBasePin * CDVRWriterFilter::GetPin(int n)
{
    if (n == 0)
	{
        return m_pWriter->m_pPin;
    }
	else 
	{
        return NULL;
    }
}

//
// GetPinCount
//
int CDVRWriterFilter::GetPinCount()
{
    return 1;
}

//
// Stop
//
// Overriden to close the dump file
//
STDMETHODIMP CDVRWriterFilter::Stop()
{
#if 0
    CAutoLock cObjectLock(m_pLock);

    if (m_pWriter)
	{
        m_pWriter->CloseFile();
	}
#endif
    return CBaseFilter::Stop();
}

//
// Pause
//
// Overriden to open the dump file
//
STDMETHODIMP CDVRWriterFilter::Pause()
{
    return CBaseFilter::Pause();
}

//
// Run
//
// Overriden to open the socket
//
STDMETHODIMP CDVRWriterFilter::Run(REFERENCE_TIME tStart)
{
    return CBaseFilter::Run(tStart);
}

////////////////////////////////////////////////////////////////////////
//
// Exported entry points for registration and unregistration 
// (in this case they only call through to default implementations).
//
////////////////////////////////////////////////////////////////////////

//
// DllRegisterSever
//
// Handle the registration of this filter
//
STDAPI DllRegisterServer()
{
    return AMovieDllRegisterServer2( TRUE );

} // DllRegisterServer

//
// DllUnregisterServer
//
STDAPI DllUnregisterServer()
{
    return AMovieDllRegisterServer2( FALSE );

} // DllUnregisterServer

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
