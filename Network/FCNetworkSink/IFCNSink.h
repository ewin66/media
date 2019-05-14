#pragma once

#include <objidl.h>
#include "uuids.h"


interface IFCNSink : public IUnknown
{
	/*
		Retreives the count of connected streams
		count	[out]	the number of connected streams
	*/
	STDMETHOD(GetStreamCount)(__out int * count) PURE;

	/*
		Sets the port allocation that a stream's RTP session should listen on
		index	[in]	index of the stream
		data	[in]	the RTP data port number. Should be even.
		control	[in]	the RTCP control port number. A value of 0 means auto-select.
		Returns S_OK on success, or S_FALSE if the RTP Session has already started
	*/
	STDMETHOD(SetPortAllocation)(__in int index, __in unsigned short data, __in unsigned short control) PURE;

	/*
		Gets the port allocation set for a particular stream
		index	[in]	index of the stream
		data	[out]	The assigned data port. Will be 0 if SetPortAllocation has not been called.
		control	[out]	The assigned control port. Will be 0 if SetPortAllocation has not been called.
		Returns S_OK
	*/
	STDMETHOD(GetPortAllocation)(__in int index, __out unsigned short * data, __out unsigned short * control) PURE;

	/*
		After connecting up the Sink, call this method in order to get all of the description info
		index		[in]	starting from 0, increment to enumerate the pins.
		description	[out]	a description of the connection.
		Returns:
		S_OK				success
		S_FALSE				not connected
		VFW_S_NO_MORE_ITEMS	index out of range
	*/
	STDMETHOD(GetStream)(__in int index, __out FCRTPStreamDescription * description) PURE;

	/*
		Adds a destination to a particular stream, or all streams
		index		[in]	index of stream to add destination to, or -1 to add to all
		destination	[in]	destination endpoint to add
		returns S_OK if add destination succeeded.
		returns E_FAIL if add destination failed.
		If you receive E_FAIL when adding to index == -1, you should RemoveDestination from all before trying
		again, because it is possible that some additions succeeded before failing
	*/
	STDMETHOD(AddDestination)(__in int index, __in const FCRTPEndpoint * destination) PURE;

	/*
		Removes a destination from a stream, or from all streams
		index		[in]	index of stream to add destination to, or -1 to add to all
		destination	[in]	destination endpoint to remove
		returns S_OK if removed from destination, or if index is -1.
		returns S_FALSE if the endpoint specified was not a destination for the specified index
	*/
	STDMETHOD(RemoveDestination)(__in int index, __in const FCRTPEndpoint * destination) PURE;
};