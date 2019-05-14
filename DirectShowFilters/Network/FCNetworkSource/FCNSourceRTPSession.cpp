#include "FCNSourceRTPSession.h"

FCNSourceRTPSession::FCNSourceRTPSession(InetHostAddress & serverIP, tpport_t port)
	: FCRTPSession(serverIP, port),
	  samples(new deque<DSPayload>),
	  sampleQueue(new CCritSec())
{
	setMaxPacketMisorder(50);

	defaultApplication().setSDESItem(SDESItemTypeTOOL, "FC Network Source");	//TODO put application name

}

FCNSourceRTPSession::~FCNSourceRTPSession(void)
{
	if(samples)
	{
		delete samples;
	}
	if(sampleQueue)
	{
		delete sampleQueue;
	}
}

bool FCNSourceRTPSession::Dequeue(DSPayload * outPayload)
{
	CAutoLock lock(sampleQueue);

	if(samples->size() > 0)
	{
		(*outPayload) = samples->front();
		samples->pop_front();
		return true;
	}
	return false;
}

void FCNSourceRTPSession::ProcessWaitingData()
{
	CAutoLock lock(sampleQueue);

	RTPPayload data;
	while(FetchWaitingData(&data))
	{
		formatter->ReceiveParse(data, samples);
	}
}

bool FCNSourceRTPSession::FetchWaitingData(RTPPayload * outPayload)
{
	if(this->isWaiting())
	{
		const AppDataUnit * rtpPacket;
		uint32 stamp = this->getFirstTimestamp();
		rtpPacket = this->getData(stamp);
		if(rtpPacket)
		{
			outPayload->Data = (BYTE*)rtpPacket->getData();
			outPayload->Length = rtpPacket->getSize();
			outPayload->Marker = rtpPacket->isMarked();
			outPayload->Timestamp = stamp;
			outPayload->ReleaseData = false;

			return true;
		}
	}
	return false;
}