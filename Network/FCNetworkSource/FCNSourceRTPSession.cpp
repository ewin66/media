#include "FCNSourceRTPSession.h"

FCNSourceRTPSession::FCNSourceRTPSession(InetHostAddress & serverIP, tpport_t serverPort, tpport_t localPort)
	: FCRTPSession(InetHostAddress("0.0.0.0"), localPort),
	  samples(new deque<DSPayload>),
	  sampleQueue(new CCritSec())
{
	
	//linkage back to server for reporting
	addDestination(serverIP, serverPort);

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
			outPayload->Length = rtpPacket->getSize();

			//outPayload->Data = (BYTE*)rtpPacket->getData();

			outPayload->Data = new BYTE[outPayload->Length];
			memcpy(outPayload->Data, rtpPacket->getData(), outPayload->Length);

			outPayload->Marker = rtpPacket->isMarked();
			outPayload->Timestamp = stamp;
			outPayload->ReleaseData = true;

			delete rtpPacket;

			return true;
		}
		//TODO just because we fail to get a proper packet may not neccesarily mean there is no more data -- research
	}
	return false;
}