#pragma once

#include <windows.h>
#include <ccrtp/rtp.h>
#include <streams.h>

#include "../FCRTPLib/FCRTPLib.h"

using namespace ost;

class FCNSinkRTPSession : public FCRTPSession
{
private:
	deque<RTPPayload> * payloads;

public:
	FCNSinkRTPSession(tpport_t dataPort);
	~FCNSinkRTPSession();
	
	void SetInitialTimestamp(REFERENCE_TIME timestamp);
	
	void Enqueue(REFERENCE_TIME timestamp, const BYTE * data, long length);
	void EndOfStream();

private:
	inline void PlaceInOutgoing(deque<RTPPayload> * payloads);
};
