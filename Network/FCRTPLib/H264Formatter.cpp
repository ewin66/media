#include "H264Formatter.h"

H264Formatter::H264Formatter(size_t largestRTPPayload, const DynamicPayloadFormat * negotiated)
	: PayloadFormatter(negotiated),
	  maxRTPPayload(largestRTPPayload), 
	  prevPayloadToSend(NULL),
	  timestampChanged(true),
	  prevNaluTime(0),
	  rtpFragments(new deque<RTPPayload>())
{
}

H264Formatter::~H264Formatter(void)
{
	if(rtpFragments)
	{
		rtpFragments->clear();
		delete rtpFragments;
	}
}

#pragma region Receiving Methods

bool H264Formatter::ReceiveParse(RTPPayload & rtpPayload, deque<DSPayload> * outDSPayloads)
{
	NALUType naluType = (NALUType)GetNALUHeaderType(rtpPayload.Data[0]);

#ifdef DEBUGLOGGING
	char buf[256];
	sprintf(buf, "\r\n::ReceiveParse - NALU Header = %X  RTP Time = %u; %d bytes\r\n", rtpPayload.Data[0], rtpPayload.Timestamp, rtpPayload.Length);
	OutputDebugStringA(buf);
#endif

	if(naluType < RFC3984Min)	//this is a normal NALU
	{
	//	DropPendingFragments();	//if we received a normal NALU, then we're not working on fragments!

		DSPayload p = DSPayload();
		p.Data = rtpPayload.Data;
		p.Length = rtpPayload.Length;
		p.ReleaseData = rtpPayload.ReleaseData;
		RTPToDSTime(rtpPayload.Timestamp, &p.Timestamp);

#ifdef DEBUGLOGGING
		char inbuf[256];
		sprintf(inbuf, "                            -  DS Time = %I64d\r\n", p.Timestamp);
		OutputDebugStringA(inbuf);
#endif

		outDSPayloads->push_back(p);

		return true;
	}
	else if(naluType == FU_A) //Fragmentation Unit - "A type"
	{
		return AddPendingFragment(rtpPayload, outDSPayloads);
	}
	else
	{
		DropPayload(rtpPayload);
	}

	return false;
}

bool H264Formatter::AddPendingFragment(RTPPayload & rtpPayload, deque<DSPayload> * outDSPayloads)
{
#ifdef DEBUGLOGGING
	char buf[256];
#endif

	BYTE fuHeader = rtpPayload.Data[1];
	if(FUHIsStart(fuHeader))
	{

#ifdef DEBUGLOGGING
		sprintf(buf, "   %X - FU-A Start\r\n", fuHeader);
		OutputDebugStringA(buf);
#endif

		DropPendingFragments();
		rtpFragments->push_back(rtpPayload);
		return false;
	}
	else if(FUHIsIntermediate(fuHeader))
	{
#ifdef DEBUGLOGGING
		sprintf(buf, "   %X - FU-A Intermediate\r\n", fuHeader);
		OutputDebugStringA(buf);
#endif

		AddFragmentIfRelated(rtpPayload);
		return false;
	}
	else if(FUHIsEnd(fuHeader))
	{
#ifdef DEBUGLOGGING
		sprintf(buf, "   %X - FU-A End\r\n", fuHeader);
		OutputDebugStringA(buf);
#endif

		if(AddFragmentIfRelated(rtpPayload))
		{
			return ConcatPendingFragments(outDSPayloads);
		}
	}

	DropPayload(rtpPayload);

	return false;
}

bool H264Formatter::AddFragmentIfRelated(RTPPayload & rtpPayload)
{
	if(rtpFragments->size() > 0)
	{
		if(rtpFragments->back().Timestamp == rtpPayload.Timestamp)
		{
			rtpFragments->push_back(rtpPayload);
			return true;
		}
	}

	DropPayload(rtpPayload);
	return false;
	
}

bool H264Formatter::ConcatPendingFragments(deque<DSPayload> * outDSPayloads)
{
	if(rtpFragments->size() > 1)
	{
		DSPayload p = DSPayload();
		for(std::deque<RTPPayload>::size_type i = 0; i < rtpFragments->size(); i++)
		{
			p.Length += rtpFragments->at(i).Length - 2;
		}
		p.Length += 1;	//+ 1 byte for NALU header
		p.Data = new BYTE[p.Length];
		//reconstruct header
		p.Data[0] = (rtpFragments->at(0).Data[0] & NALUNRIMask) | (rtpFragments->at(0).Data[1] & NALUTypeMask);

		long m = 1;
		for(std::deque<RTPPayload>::size_type i = 0; (m < p.Length) && (i < rtpFragments->size()); i++)
		{
			memcpy(&(p.Data[m]), &(rtpFragments->at(i).Data[2]), rtpFragments->at(i).Length - 2);
			m += rtpFragments->at(i).Length - 2;
		}
		p.ReleaseData = true;
		RTPToDSTime(rtpFragments->back().Timestamp, &p.Timestamp);

#ifdef DEBUGLOGGING
		char buf[256];
		sprintf(buf, "H264Formatter::ConcatPendingFragments - %d bytes @ RTP time %u / DS time %I64d\r\n", p.Length, rtpFragments->back().Timestamp, p.Timestamp);
		OutputDebugStringA(buf);
#endif

		outDSPayloads->push_back(p);

		DropPendingFragments();

		return true;
	}
	return false;
}

void H264Formatter::DropPendingFragments()
{
	if(rtpFragments->size() > 0)
	{
#ifdef DEBUGLOGGING
		char buf[256];
		sprintf(buf, "   H264Formatter::DropPendingFragments (%u fragments for RTP time %u)\r\n", rtpFragments->size(), rtpFragments->at(0).Timestamp);
		OutputDebugStringA(buf);
#endif

		for(std::deque<RTPPayload>::size_type i = 0; i < rtpFragments->size(); i++)
		{
			DropPayload(rtpFragments->at(i));
		}
		rtpFragments->clear();
	}
}

void H264Formatter::DropPayload(RTPPayload & p)
{
	if((p.ReleaseData) && (p.Data))
	{
		delete [] p.Data;
		p.Data = NULL;
		p.Length = 0;
		p.ReleaseData = false;
	}
}

#pragma endregion

#pragma region Sending Methods

bool H264Formatter::SendFormat(const DSPayload & dsPayload, deque<RTPPayload> * outRTPPayloads)
{
	//the only way to determine if this is the last NALU in an access unit is if the timestamp has changed
	prevNaluTime = this->lastSentRTPTime;
	uint32 naluTime;
	bool timestampChanged = DSToRTPTime(dsPayload.Timestamp, &naluTime);

	bool naluEnqueued = false;

	if(prevPayloadToSend)	//if we have a buffered NALU, send it
	{
#ifdef DEBUGLOGGING
		char buf[256];
		sprintf(buf, "\r\n::SendFormat - NALU Header = %X  RTP Time = %u; %d bytes", prevPayloadToSend->Data[0], prevNaluTime, prevPayloadToSend->Length);
		OutputDebugStringA(buf);
#endif

		EnqueuePayload(*prevPayloadToSend, prevNaluTime, timestampChanged, outRTPPayloads);
		naluEnqueued = true;
	}

	prevPayloadToSend = (DSPayload*)&dsPayload;
	prevNaluTime = naluTime;

	return naluEnqueued;
}

bool H264Formatter::SendFlush(deque<RTPPayload> * outRTPPayloads)
{
	if(prevPayloadToSend)	//if we have a buffered NALU, send it
	{
		if(prevPayloadToSend->Length < 0)
		{
			return false;
		}
		EnqueuePayload(*prevPayloadToSend, prevNaluTime, false, outRTPPayloads);
		prevPayloadToSend = NULL;
		prevNaluTime = 0;
		return true;
	}
	return false;
}

void H264Formatter::EnqueuePayload(const DSPayload & dsPayload,
								   uint32 naluTime,
								   bool endsAccessUnit,
								   deque<RTPPayload> * outRTPPayloads)
{
	//if this NALU cannot be fragmented...
	if(dsPayload.Length <= (long)maxRTPPayload)
	{
		RTPPayload p(naluTime, dsPayload.Data, (size_t)dsPayload.Length);
		p.Marker = endsAccessUnit;

		outRTPPayloads->push_back(p);
	}
	else	//do FU-A fragmentation
	{
		int frags = 0;
		uint64 offset = 0;
		size_t actualLength = maxRTPPayload;
		for(uint64 offset = 0; offset < dsPayload.Length; offset += maxRTPPayload)
		{
			ExtractFragmentationUnit(dsPayload, naluTime, endsAccessUnit, offset, maxRTPPayload, outRTPPayloads);
			frags++;
		}
	}
}

void H264Formatter::ExtractFragmentationUnit(const DSPayload & dsPayload,
											 uint32 naluTime,
											 bool endsAccessUnit,
											 uint64 offset, 
											 size_t length, 
											 deque<RTPPayload> * outRTPPayloads)
{
	RTPPayload p = RTPPayload();
	p.Timestamp = naluTime;

	bool startPacket = offset == 0;
	bool endPacket = ((offset + length) >= dsPayload.Length);

	p.Marker = endPacket && endsAccessUnit;

	if(endPacket)
	{
		p.Length = (uint32)(dsPayload.Length - offset);
	}
	else
	{
		p.Length = length - (startPacket ? 1 : 0);	//if start packet, remove 1 byte (strip NALU header)
	}
	//add space for FU Indicator and Header
	p.Length += 2;

	p.Data = new BYTE[p.Length];

	//Create Fragmentation Unit Indicator -- indicates FU-A format
	p.Data[0] = (dsPayload.Data[0] & NALUNRIMask) | FU_A;
	//Create Fragmentation Unit Header
	//3 Most Significant Bits: |S|E|R|  S = Start ; E = End ; R = Reservered (0)
	//	if(offset == 0) = 100
	//	if(endPacket)	= 010
	//	else			= 000
	p.Data[1] = ((startPacket) ? (0x02 << 6) : ((endPacket) ? (0x01 << 6) : 0)) |
				GetNALUHeaderType(dsPayload.Data[0]);
	
	memcpy(&(p.Data[2]),
		   &(dsPayload.Data[((startPacket) ? 1 : offset)]),	//if start packet, skip NALU header
		   p.Length - 2);

	p.ReleaseData = true;

	outRTPPayloads->push_back(p);
}

#pragma endregion