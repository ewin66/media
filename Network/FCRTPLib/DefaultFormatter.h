#pragma once
/*
	DefaultFormatter.h
	This is a default, pass-through formatter. It only does time-stamp tracking

	Kevin C. Dixon
	Future Concepts
	03/17/2009
*/
#include "PayloadFormatter.h"

class FCRTPLIB_API DefaultFormatter : public PayloadFormatter
{
public:
	DefaultFormatter(const PayloadFormat * format);
	~DefaultFormatter(void);

	virtual bool SendFormat(const DSPayload & dsPayload, deque<RTPPayload> * outRTPPayloads);
	virtual bool ReceiveParse(RTPPayload & rtpPayload, deque<DSPayload> * outDSPayloads);
};
