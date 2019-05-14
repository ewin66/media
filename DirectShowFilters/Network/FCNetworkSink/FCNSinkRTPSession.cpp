#include "FCNSinkRTPSession.h"

FCNSinkRTPSession::FCNSinkRTPSession(tpport_t dataPort)
	: FCRTPSession(InetHostAddress("0.0.0.0"), dataPort),
	  payloads(new deque<RTPPayload>())
{
	//HACK just to send data
	addDestination(InetHostAddress("127.0.0.1"), 5002);

	defaultApplication().setSDESItem(SDESItemTypeTOOL, "FC Network Sink");	//TODO put application name
}

FCNSinkRTPSession::~FCNSinkRTPSession()
{
	if(payloads)
	{
		delete payloads;
	}
}

void FCNSinkRTPSession::SetInitialTimestamp(REFERENCE_TIME timestamp)
{
	if(formatter)
	{
		formatter->SetInitialSendTimestamp(timestamp);
	}
}

void FCNSinkRTPSession::Enqueue(REFERENCE_TIME timestamp, const BYTE * data, long length)
{
	if((formatter) && (payloads))
	{
		if(formatter->SendFormat(DSPayload(timestamp, (BYTE*)data, length), payloads))
		{
			PlaceInOutgoing(payloads);
		}
	}
}

void FCNSinkRTPSession::PlaceInOutgoing(deque<RTPPayload> * payloads)
{
	RTPPayload cur;
	while(!payloads->empty())
	{
		this->setMark(payloads->front().Marker);
		this->putData(payloads->front().Timestamp,
					  payloads->front().Data,
					  payloads->front().Length);


		if((payloads->front().ReleaseData) && (payloads->front().Data))
		{
			delete [] (payloads->front().Data);
		}

		payloads->pop_front();
	}
}

void FCNSinkRTPSession::EndOfStream()
{
	if((formatter) && (payloads))
	{
		if(formatter->SendFlush(payloads))
		{
			PlaceInOutgoing(payloads);
		}
	}
}