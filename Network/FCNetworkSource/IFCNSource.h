#pragma once

#include <objidl.h>
#include "uuids.h"
#include "Structures.h"

interface IFCNSource : public IUnknown
{
	/*
		Once you have received the stream metadata from the server, call this method to
		create the connections and pins on the source filter
		description	[in]	description of the stream
		index		[out]	index of the output pin assigned to this stream
	*/
	STDMETHOD(AddStream)(__in FCRTPStreamDescription * description, __out int * index) PURE;

	/*
		Remove a particular stream from the source
		index	[in]	index of the pin/stream to remove, or -1 to remove all
	*/
	STDMETHOD(RemoveStream)(__in int index) PURE;
};