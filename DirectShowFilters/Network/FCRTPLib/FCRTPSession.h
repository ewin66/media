#pragma once
/*
	FCRTPSession.h
	Base Class for an FC RTP Session

	Kevin C. Dixon
	Future Concepts
	03/24/2009
*/

#include <windows.h>
#include <ccrtp/rtp.h>
#include "FCRTPLibExports.h"
#include "PayloadFormatter.h"

class FCRTPLIB_API FCRTPSession : protected RTPSession
{
protected:
	PayloadFormatter * formatter;
protected:
	FCRTPSession(InetHostAddress & serverIP, tpport_t port);
public:
	~FCRTPSession();

	virtual bool Start();

	virtual void SetPayloadFormat(PayloadFormatter * customPayload);
	virtual bool GetPayloadType(PayloadType * outPayloadType);
};