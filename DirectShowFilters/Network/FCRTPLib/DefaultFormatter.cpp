#include "DefaultFormatter.h"

DefaultFormatter::DefaultFormatter(const PayloadFormat * format)
	: PayloadFormatter(format)
{
}

DefaultFormatter::~DefaultFormatter(void)
{
}

bool DefaultFormatter::SendFormat(const DSPayload & dsPayload, deque<RTPPayload> * outRTPPayloads)
{
	RTPPayload p = RTPPayload(0, dsPayload.Data, dsPayload.Length);
	DSToRTPTime(dsPayload.Timestamp, &p.Timestamp);
	outRTPPayloads->push_back(p);
	return true;
}

bool DefaultFormatter::ReceiveParse(RTPPayload & rtpPayload, deque<DSPayload> * outDSPayloads)
{
	DSPayload p = DSPayload(0, rtpPayload.Data, rtpPayload.Length);
	RTPToDSTime(rtpPayload.Timestamp, &p.Timestamp);
	outDSPayloads->push_back(p);
	return true;
}