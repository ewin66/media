/*
	IFCVolumeMute.h
	Declares the Interface for IFCVolumeMute, which exposes methods that
	clients of this library need to use the filter

	Kevin Dixon
	(c) 2009 Future Concepts
	01/20/2009
*/

#include <objidl.h>
#include "uuids.h"

interface IFCVolumeMute : public IUnknown
{
	STDMETHOD(set_Volume)(__in int attenuationInDB) = 0;
	STDMETHOD(get_Volume)(__out int * attenuationInDB) = 0;

	STDMETHOD(set_Mute)(__in BOOL muted) = 0;
	STDMETHOD(get_Mute)(__out BOOL * muted) = 0;
};