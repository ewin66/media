/*
	FCNetworkSourceFilterRegistry
	Implements ActiveMovie Setup and DLL business

	Kevin C. Dixon
	Future Concepts
	03/23/2009
*/

#include "FCNetworkSourceFilter.h"

// Setup data

const AMOVIESETUP_MEDIATYPE sudPinTypes[] =
{
	{ &MEDIATYPE_Video, &MEDIASUBTYPE_NULL },
	{ &MEDIATYPE_Audio, &MEDIASUBTYPE_NULL }
};

const AMOVIESETUP_PIN psudPins[] =
{
	{
		L"Output",                   // Pin string name
		FALSE,                      // Is it rendered
		TRUE,                      // Is it an output
		TRUE,                      // Allowed none
		TRUE,                      // Allowed many
		&CLSID_NULL,                // Connects to filter
		L"(obselete)",                  // Connects to pin
		2,                          // Number of types
		sudPinTypes                // Pin information
	}
};

const AMOVIESETUP_FILTER sudNetworkSource =
{
    &CLSID_FCNetworkSource,             // Filter CLSID
    L"FC Network Source",                // String name
    MERIT_DO_NOT_USE,           // Filter merit
    1,                          // Number pins
    psudPins                    // Pin details
};

//
//  Object creation stuff
//
CFactoryTemplate g_Templates[] =
{
	{ L"FC Network Source", &CLSID_FCNetworkSource, FCNetworkSourceFilter::CreateInstance, NULL, &sudNetworkSource }
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