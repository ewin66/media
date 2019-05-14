/*
	IFCFrameRateAPI.h
	Interface to set Input and Target frame rates.

	Kevin Dixon
	(c) 2009 Future Concepts
	01/28/2009
*/
#include <objidl.h>
#include "uuids.h"

interface IFCFrameRateAPI : public IUnknown
{
	STDMETHOD(set_InputFramerate)(__in double inputFPS) = 0;
	STDMETHOD(get_InputFramerate)(__out double * inputFPS) = 0;

	STDMETHOD(set_TargetFramerate)(__in double targetFPS) = 0;
	STDMETHOD(get_TargetFramerate)(__out double * targetFPS) = 0;
};