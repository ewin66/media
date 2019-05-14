/*
	DirectShow self registration and DllMain

	Kevin Dixon
	Copyright (c) 2009 Future Concepts
	12/22/2009
*/

#include "TimeStampAdjust.h"

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
sudTimeStamp = { &CLSID_TimeStampAdjust                   // class id
            , L"FC Time Stamp Adjust"                       // strName
            , MERIT_DO_NOT_USE                // dwMerit
            , 2                               // nPins
            , psudPins                        // lpPin
            };

// Needed for the CreateInstance mechanism
CFactoryTemplate g_Templates[1]= { { L"FC Time Stamp Adjust"
                                   , &CLSID_TimeStampAdjust
                                   , TimeStampAdjust::CreateInstance
                                   , NULL
                                   , &sudTimeStamp
                                   }
                                 };

int g_cTemplates = sizeof(g_Templates)/sizeof(g_Templates[0]);

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