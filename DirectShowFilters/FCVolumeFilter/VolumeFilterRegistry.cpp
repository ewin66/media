/*
	VolumeFilterRegistry.cpp
	Handles registration of this filter

	Kevin Dixon
	(c) 2009 Future Concepts
	01/20/2009
*/

#include "VolumeFilter.h"
#include "uuids.h"

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
    &(CLSID_FCVolumeFilter), L"FC Volume Filter", MERIT_UNLIKELY, 2, psudPins
};                    

// Templates
CFactoryTemplate g_Templates[]=
{
    { L"FC Volume Filter", &(CLSID_FCVolumeFilter), VolumeFilter::CreateInstance, NULL, &sudAudioVolume }
};

int g_cTemplates = sizeof(g_Templates)/sizeof(g_Templates[0]);