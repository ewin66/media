/*
	dllmain.cpp
	DLL entry point and Un/Register Server calls

	Kevin Dixon
	(c) 2009 Future Concepts
	01/27/2009
*/

#include <windows.h>
#include <streams.h>

#include "FrameRateFilter.h"
#include "uuids.h"

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

STDAPI DllRegisterServer()
{
    return AMovieDllRegisterServer2(TRUE);
}

STDAPI DllUnregisterServer()
{
    return AMovieDllRegisterServer2(FALSE);
}

//TODO change this media type array to show what we accept
// Media Types
const AMOVIESETUP_MEDIATYPE sudPinTypes[] =  
{
    { &MEDIATYPE_Audio, &MEDIASUBTYPE_PCM }
};

// Pins
const AMOVIESETUP_PIN psudPins[] =
{
    { L"Input", FALSE, FALSE, FALSE, FALSE, &CLSID_NULL, NULL, 1, &sudPinTypes[0] },
    { L"Output", FALSE, TRUE, FALSE, FALSE, &CLSID_NULL, NULL, 1, &sudPinTypes[0] }
};  

// Filters
const AMOVIESETUP_FILTER sudAudioVolume =
{
    &(CLSID_FCFrameRateFilter), L"FC Frame Rate Filter", MERIT_UNLIKELY, 2, psudPins
};                    

// Templates
CFactoryTemplate g_Templates[]=
{
    { L"FC Frame Rate Filter", &(CLSID_FCFrameRateFilter), FrameRateFilter::CreateInstance, NULL, &sudAudioVolume }
};

int g_cTemplates = sizeof(g_Templates)/sizeof(g_Templates[0]);