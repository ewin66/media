/*
	FCNetworkSinkFilterRegistry
	Implements ActiveMovie Setup and DLL business

	Kevin C. Dixon
	Future Concepts
	03/10/2009
*/

#include "FCNetworkSinkFilter.h"

// Setup data

const AMOVIESETUP_MEDIATYPE sudPinTypes[] =
{
	{ &MEDIATYPE_Video, &MEDIASUBTYPE_NULL },
	{ &MEDIATYPE_Audio, &MEDIASUBTYPE_NULL }
};

const AMOVIESETUP_PIN psudPins[] =
{
	{
		L"Input",                   // Pin string name
		TRUE,                      // Is it rendered
		FALSE,                      // Is it an output
		FALSE,                      // Allowed none
		TRUE,                      // Allowed many
		&CLSID_NULL,                // Connects to filter
		L"(obselete)",                  // Connects to pin
		2,                          // Number of types
		sudPinTypes                // Pin information
	}
};

const AMOVIESETUP_FILTER sudNetworkSink =
{
    &CLSID_FCNetworkSink,             // Filter CLSID
    L"FC Network Sink",                // String name
    MERIT_DO_NOT_USE,           // Filter merit
    1,                          // Number pins
    psudPins                    // Pin details
};

//
//  Object creation stuff
//
CFactoryTemplate g_Templates[] =
{
	{ L"FC Network Sink", &CLSID_FCNetworkSink, FCNetworkSinkFilter::CreateInstance, NULL, &sudNetworkSink }
};
int g_cTemplates = 1;

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