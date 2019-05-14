//------------------------------------------------------------------------------
// File: FMangoCapture.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

#pragma warning(disable:4710)  // 'function': function not inlined (optimzation)

// Setup data

const AMOVIESETUP_MEDIATYPE sudOpPinTypes =
{
    &MEDIATYPE_Video,       // Major type
    &MEDIASUBTYPE_NULL      // Minor type
};

const AMOVIESETUP_MEDIATYPE sudOpPinTypesAudio =
{
	&MEDIATYPE_Audio,		// Major
	&MEDIASUBTYPE_NULL		// Minor
};

const AMOVIESETUP_PIN sudOpPin[] =
{
	{
    L"Output",              // Pin string name
    FALSE,                  // Is it rendered
    TRUE,                   // Is it an output
    FALSE,                  // Can we have none
    FALSE,                  // Can we have many
    &CLSID_NULL,            // Connects to filter
    NULL,                   // Connects to pin
    1,                      // Number of types
    &sudOpPinTypes
	},
	{
    L"Output2",             // Pin string name
    FALSE,                  // Is it rendered
    TRUE,                   // Is it an output
    FALSE,                  // Can we have none
    FALSE,                  // Can we have many
    &CLSID_NULL,            // Connects to filter
    NULL,                   // Connects to pin
    1,                      // Number of types
    &sudOpPinTypes
	},
	{
    L"Output3",             // Pin string name
    FALSE,                  // Is it rendered
    TRUE,                   // Is it an output
    FALSE,                  // Can we have none
    FALSE,                  // Can we have many
    &CLSID_NULL,            // Connects to filter
    NULL,                   // Connects to pin
    1,                      // Number of types
    &sudOpPinTypesAudio
	},
	{
    L"Output4",             // Pin string name
    FALSE,                  // Is it rendered
    TRUE,                   // Is it an output
    FALSE,                  // Can we have none
    FALSE,                  // Can we have many
    &CLSID_NULL,            // Connects to filter
    NULL,                   // Connects to pin
    1,                      // Number of types
    &sudOpPinTypesAudio
	}
}; // Pin details

const AMOVIESETUP_FILTER sudMangoCaptureax =
{
    &CLSID_MangoCapture,    // Filter CLSID
    L"Mango Capture",       // String name
    MERIT_NORMAL,           // Filter merit
    4,                      // Number pins
    sudOpPin               // Pin details
};


// COM global table of objects in this dll

CFactoryTemplate g_Templates[] = {
  { L"Mango Capture"
  , &CLSID_MangoCapture
  , CMangoCapture::CreateInstance
  , NULL
  , &sudMangoCaptureax },
  // This entry is for the property page.
  { L"AVC Parameters Props"
  , &CLSID_AVCParametersProp
  , CAVCPropPage::CreateInstance
  , NULL
  , NULL }
};
int g_cTemplates = sizeof(g_Templates) / sizeof(g_Templates[0]);

////////////////////////////////////////////////////////////////////////
//
// Exported entry points for registration and unregistration 
// (in this case they only call through to default implementations).
//
////////////////////////////////////////////////////////////////////////

//
// DllRegisterServer
//
// Exported entry points for registration and unregistration
//
STDAPI DllRegisterServer()
{
    return AMovieDllRegisterServer2(TRUE);

} // DllRegisterServer


//
// DllUnregisterServer
//
STDAPI DllUnregisterServer()
{
    return AMovieDllRegisterServer2(FALSE);

} // DllUnregisterServer


//
// DllEntryPoint
//
extern "C" BOOL WINAPI DllEntryPoint(HINSTANCE, ULONG, LPVOID);

BOOL APIENTRY DllMain(HANDLE hModule, 
                      DWORD  dwReason, 
                      LPVOID lpReserved)
{
    switch (dwReason)
    {

    case DLL_PROCESS_ATTACH:
		OutputDebugStringA("MangoCapture DLL_PROCESS_ATTACH");
	//	Mango::Init();
        break;

    case DLL_PROCESS_DETACH:
		OutputDebugStringA("MangoCapture DLL_PROCESS_DETACH");
	//	Mango::Finish();
		break;
    }
	return DllEntryPoint((HINSTANCE)(hModule), dwReason, lpReserved);
}

//
// CreateInstance
//
CUnknown * WINAPI CMangoCapture::CreateInstance(LPUNKNOWN lpunk, HRESULT *phr)
{
    ASSERT(phr);

    CUnknown *punk = new CMangoCapture(lpunk, phr);
    if(punk == NULL)
    {
        if(phr)
            *phr = E_OUTOFMEMORY;
    }
    return punk;

} // CreateInstance


//
// Constructor
//
// Initialise a CMangoCapture object so that we have a pin.
//
CMangoCapture::CMangoCapture(LPUNKNOWN lpunk, HRESULT *phr) :
    CSource(NAME("Mango Capture"), lpunk, CLSID_MangoCapture)
{
    ASSERT(phr);
    CAutoLock cAutoLock(&m_cStateLock);

    m_paStreams = (CSourceStream **) new CSourceStream*[4];
    if(m_paStreams == NULL)
    {
        if(phr)
            *phr = E_OUTOFMEMORY;

        return;
    }

    m_paStreams[0] = new H264Stream(phr, this, L"H264");
    if(m_paStreams[0] == NULL)
    {
        if(phr)
            *phr = E_OUTOFMEMORY;

        return;
    }
    m_paStreams[1] = new YUVStream(phr, this, L"YUV");
    if(m_paStreams[1] == NULL)
    {
        if(phr)
            *phr = E_OUTOFMEMORY;

        return;
    }
    m_paStreams[2] = new PCMStream(phr, this, L"PCM");
    if(m_paStreams[2] == NULL)
    {
        if(phr)
            *phr = E_OUTOFMEMORY;

        return;
    }
    m_paStreams[3] = new MP3Stream(phr, this, L"MP3");
    if(m_paStreams[3] == NULL)
    {
        if(phr)
            *phr = E_OUTOFMEMORY;

        return;
    }
} // (Constructor)
