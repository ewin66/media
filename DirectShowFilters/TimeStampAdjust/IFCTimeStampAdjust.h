/*
	Interface for dealing with FC Time Stamp Adjust filter

	Kevin Dixon
	(c) 2009 Future Concepts
	12/22/2009
*/

#include <objidl.h>
#include "TimeStampGuids.h"

interface IFCTimeStampAdjust : public IUnknown
{
	STDMETHOD(set_Logging)(__in int level) PURE;
	STDMETHOD(get_Logging)(__out int * level) PURE;

	STDMETHOD(set_LogPath)(__in BSTR path) PURE;

	STDMETHOD(set_Strategy)(__in int strategy) PURE;
	STDMETHOD(get_Strategy)(__out int * strategy) PURE;
};