#pragma once
/*
	Payloads.h
	Defines structs and classes that describe and handle various ways of representing payloads

	Kevin C. Dixon
	Future Concepts
	03/19/2009
*/


#include <windows.h>
#include <ccrtp/rtp.h>
#include <streams.h>

using namespace ost;

#include "FCRTPLibExports.h"

class FCRTPLIB_API BasePayload
{
public:
	BasePayload() : Data(0), ReleaseData(false) { }
	BasePayload(BYTE * data) : Data(data), ReleaseData(false) { }
	BasePayload(BYTE * data, bool releaseData) : Data(data), ReleaseData(releaseData) { }

	bool ReleaseData;	//Set to true if Data needs to be deleted by the application code
	BYTE * Data;		//The Data for this payload
};

/*
	Represents an RTP payload that is compatible with ccRTP
*/
class FCRTPLIB_API RTPPayload : public BasePayload
{
public:
	RTPPayload() : BasePayload(0), Timestamp(0), Length(0), Marker(false) {}

	RTPPayload(uint32 time, BYTE * data, size_t len)
		: BasePayload(data), Timestamp(time), Length(len), Marker(false) {}

	uint32 Timestamp;	//RTP timestamp
	size_t Length;		//Number of bytes in Data
	bool Marker;		//True if the RTP header Marker field should be set for this payload
};

/*
	Represents a DirectShow payload that is compatible with IMediaSample
*/
class FCRTPLIB_API DSPayload : public BasePayload
{
public:
	DSPayload() : BasePayload(0), Timestamp(0), Length(0) {}

	DSPayload(REFERENCE_TIME time, BYTE * data, long len)
		: BasePayload(data), Timestamp(time), Length(len) {}

	REFERENCE_TIME Timestamp;	//DirectShow timestamp
	long Length;				//Number of bytes in Data
};