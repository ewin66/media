#pragma once

#include <windows.h>
#include <ccrtp/rtp.h>
#include "../FCRTPLib/FCRTPLib.h"

using namespace ost;

class FCNSourceRTPSession : public FCRTPSession
{
private:
	CCritSec * sampleQueue;
	deque<DSPayload> * samples;

public:
	FCNSourceRTPSession(InetHostAddress & serverIP, tpport_t port);
	~FCNSourceRTPSession(void);

	void ProcessWaitingData();
	bool Dequeue(DSPayload * outPayload);

private:
	bool FetchWaitingData(RTPPayload * outPayload);
};
