#pragma once
/*
	PayloadFormatter.h
	Defines the abstract PayloadFormatter class.

	Kevin C. Dixon
	Future Concepts
	03/16/2009
*/

#include <windows.h>
#include <ccrtp/rtp.h>
#include <deque>
#include <streams.h>

using namespace ost;
using std::deque;

#include "FCRTPLibExports.h"

#include "Payloads.h"

/*
	Abstract base-class that can be used to do Profile or Payload-specific transforms to the data
	on sending or receiving
*/
class FCRTPLIB_API PayloadFormatter
{
private:
	const PayloadFormat * myFormat;

protected:
	REFERENCE_TIME lastSentDSTime;
	uint32 lastSentRTPTime;

	REFERENCE_TIME lastRecvDSTime;
	uint32 lastRecvRTPTime;

public:
	/*
		Constructs a payload formatter
		format	[in]	The ost::PayloadFormat this formatter represents
	*/
	PayloadFormatter(const PayloadFormat * format)
		: myFormat(format), lastSentDSTime(0), lastSentRTPTime(0), lastRecvDSTime(0), lastRecvRTPTime(0)
	{
	}
	virtual ~PayloadFormatter(void)
	{
	}

	/*
		Returns the PayloadFormat this formatter handles
	*/
	const PayloadFormat * GetPayloadFormat() const { return myFormat; }

	/*
		Changes the PayloadFormat this formatter handles
		format	[in]	the new PayloadFormat.
	*/
	void SetPayloadFormat(const PayloadFormat * format) { myFormat = format; }

	/*
		Sets the initial DirectShow reference point
		dsTime	[in]	initial DirectShow timestamp
	*/
	void SetInitialSendTimestamp(REFERENCE_TIME dsTime)
	{
		//TODO this is incorrectly implemented
		lastSentDSTime = 0; //dsTime;
		lastSentRTPTime = 0;
	}

	/*
		Sets the initial RTP reference point
		rtpTime	[in]	initial RTP timestamp
	*/
	void SetInitialRecvTimestamp(uint32 rtpTime)
	{
		lastRecvRTPTime = rtpTime;
		lastRecvDSTime = 0;
	}

	/*
		Called before putting data into the RTP outgoing queue
		dsPayload		[in]	The data in the current IMediaSample
		outRTPPayloads	[out]	queue of one or more RTP payloads ready to be enqueued for sending.

		return true if outRTPPayloads has had more data pushed onto it
		return false if an error occurred, or more DSPayloads are required before processing can complete.
	*/
	virtual bool SendFormat(const DSPayload & dsPayload, deque<RTPPayload> * outRTPPayloads) = 0;

	/*
		Called when packets are fetched from the RTP incoming queue.
		rtpPayload		[in]	The payload from the incoming RTP queue
		outDSPayloads	[out]	queue of one or more DirectShow payloads, ready to be pushed down the graph.

		return true if outDSPayloads has had more data pushed onto it
		return false if an error occurred or more RTPPayloads are required before processing can complete.
	*/
	virtual bool ReceiveParse(RTPPayload & rtpPayload, deque<DSPayload> * outDSPayloads) = 0;

	/*
		If the SendFormat method buffers any payloads, then this method can be called to flush them out
		outRTPPayloads	[out]	Output payloads
		Returns true if any data was added to the output queue
		Returns false if no data was buffered
	*/
	virtual bool SendFlush(deque<RTPPayload> * outRTPPayloads)
	{
		return false;
	}

	/*
		If the ReceiveParse method buffers any payloads, then this method can be called to flush them out
		outDSPayloads	[out]	Output payloads
		Returns true if any data was added to the output queue
		Returns false if no data was buffered
	*/
	virtual bool ReceiveFlush(deque<DSPayload> * outDSPayloads)
	{
		return false;
	}

protected:
	/*
		Converts DirectShow timestamps to RTP timestamps, while performing incrementing according to payload format
		dsTime		[in]	DirectShow time to convert
		outRTPTime	[out]	Corresponding RTP timestamp
		Returns true if the timestamp changed since the last time this method was called.
		Returns false if the timestamp is the same since the last call.
	*/
	bool DSToRTPTime(REFERENCE_TIME dsTime, uint32 * outRTPTime)
	{
		//if the timestamp did not change, then we are still working on the same frame
		if(dsTime == lastSentDSTime)
		{
			(*outRTPTime) = lastSentRTPTime;
			return false;
		}
		
		REFERENCE_TIME span = dsTime - lastSentDSTime;
		uint32 rtpStamp = lastSentRTPTime + (uint32)((span * 10e-8) * this->GetPayloadFormat()->getRTPClockRate());

		lastSentDSTime = dsTime;
		lastSentRTPTime = rtpStamp;

		(*outRTPTime) = rtpStamp;
		return true;
	}

	/*
		Converts RTP timestamps to DirectShow timestamps, while performing approrpirate incrementing
		rtpTime		[in]	RTP timestamp to convert
		outDSTime	[out]	Corresponding DirectShow timestamp
		Returns true if the timestamp changed since the last time this method was called.
		Returns false if the timestamp is the same since the last call.
	*/
	bool RTPToDSTime(uint32 rtpTime, REFERENCE_TIME * outDSTime)
	{
		if(rtpTime == lastRecvRTPTime)
		{
			(*outDSTime) = lastRecvDSTime;
			return false;
		}
		
		uint32 span = rtpTime - lastRecvRTPTime;
		REFERENCE_TIME dsStamp = (REFERENCE_TIME)(span / (10e-8 * this->GetPayloadFormat()->getRTPClockRate())) + lastRecvDSTime;

		lastRecvDSTime = dsStamp;
		lastRecvRTPTime = rtpTime;

		(*outDSTime) = dsStamp;

		return true;
	}
};
