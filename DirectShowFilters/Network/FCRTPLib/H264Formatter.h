#pragma once
/*
	H264Formatter.h
	This class is used to perform the payload formatting as described in RFC3984.

	Kevin C. Dixon
	Future Concepts
	03/16/2009
*/
#include "PayloadFormatter.h"

//#define DEBUGLOGGING

class FCRTPLIB_API H264Formatter : public PayloadFormatter
{
private:
	//these NALU types are defined in RFC3984
	enum NALUType
	{
		RFC3984Min = 24,
		STAP_A = 24,
		STAP_B = 25,
		MTAP16 = 26,
		MTAP24 = 27,
		FU_A = 28, 
		FU_B = 29,
		RFC3984Max = 29
	};

	static const BYTE NALUFMask = 0x80;
	static const BYTE NALUNRIMask = 0x60;
	static const BYTE NALUTypeMask = 0x1F;
	static const BYTE FUHStartMask = 0x80;
	static const BYTE FUHEndMask = 0x40;

	//state variables for Send
	size_t maxRTPPayload;
	DSPayload * prevPayloadToSend;
	bool timestampChanged;
	uint32 prevNaluTime;

	//state variables for Receive
	//buffer of received RTP Payloads that cannot be processed yet
	deque<RTPPayload> * rtpFragments; 

public:
	H264Formatter(size_t largestRTPPayload, const DynamicPayloadFormat * negotiated);
	~H264Formatter(void);

	virtual bool SendFormat(const DSPayload & dsPayload, deque<RTPPayload> * outRTPPayloads);

	virtual bool ReceiveParse(RTPPayload & rtpPayload, deque<DSPayload> * outDSPayloads);

	virtual bool SendFlush(deque<RTPPayload> * outRTPPayloads);

private:

#pragma region Sending Methods

	/*
		Properly converts a DirectShow Payload to an RTP Payload and enqueues it in the output
		dsPayload		[in]	payload to enqueue
		naluTime		[in]	NALU / RTP timestamp
		endsAccessUnit	[in]	true if this is the last payload in an access unit
		outRTPPayloads	[out]	vector to output in
	*/
	void EnqueuePayload(const DSPayload & dsPayload,
						uint32 naluTime,
						bool endsAccessUnit,
						deque<RTPPayload> * outRTPPayloads);

	/*
		Creates a single fragmentation unit from the selected data range
		dsPayload		[in]	DirectShow Payload to extract a fragment from
		naluTime		[in]	the timestamp of the NALU being fragmented
		endsAccessUnit	[in]	set to true if this is the last DSPayload with this time stamp
		offset			[in]	0-indexed offset where to begin extraction
		length			[in]	number of bytes to include in this fragment
		outRTPPayloads	[out]	vector to put extracted FU in.
		Returns the actual number of bytes taken from the dsPayload.Data
	*/
	void ExtractFragmentationUnit(const DSPayload & dsPayload,
								  uint32 naluTime,
								  bool endsAccessUnit,
								  uint64 offset,
								  size_t length,
								  deque<RTPPayload> * outRTPPayloads);

#pragma endregion

#pragma region Receiving Methods

	bool AddPendingFragment(RTPPayload & rtpPayload, deque<DSPayload> * outDSPayloads);
	bool AddFragmentIfRelated(RTPPayload & rtpPayload);

	bool ConcatPendingFragments(deque<DSPayload> * outDSPayloads);

	/*
		Called by the ReceiveParse method to Delete pending fragments.
		If any fragments are in the waiting queue, then they are deleted.
	*/
	void DropPendingFragments();

	void DropPayload(RTPPayload & p);

#pragma endregion

#pragma region Header Bit-Twiddling

	/*
		Parses a NALU header to get the NRI field
		header	the header byte to parse
		Returns the value of the NRI field
	*/
	static inline BYTE GetNALUHeaderNRI(const BYTE & header)
	{
		return ((header & NALUNRIMask) >> 5);
	}

	/*
		Parses a NALU header to get the Type field
		header	the header byte to parse
		Returns the value of the Type field.
	*/
	static inline BYTE GetNALUHeaderType(const BYTE & header)
	{
		return (header & NALUTypeMask);
	}

	/*
		Determines if a Fragmentation Unit Header is a Start header
		header	the Fragmentation Unit Header
		Returns true if valid Start header, returns false if not a Start header, or uses invalid syntax
	*/
	static inline bool FUHIsStart(const BYTE & header)
	{
		return ((header & 0xC0) >> 6) == 0x02;
	}

	/*
		Determines if a Fragmentation Unit Header is an End header
		header	the Fragmentation Unit Header
		Returns true if valid End header, returns false if not an End header, or uses invalid syntax
	*/
	static inline bool FUHIsEnd(const BYTE & header)
	{
		return ((header & 0xC0) >> 6) == 0x01;
	}

	/*
		Returns true if the Fragmentation Unit Header indicates this is not a Start or End unit
		header	the Fragmentation Unit Header
		returns true if the Start and End bit are 0
	*/
	static inline bool FUHIsIntermediate(const BYTE & header)
	{
		return (header & 0xC0) == 0x00;
	}

#pragma endregion
};
