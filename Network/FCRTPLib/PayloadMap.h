#pragma once

#include <map>
#include <vector>
#include <streams.h>
#include <ccrtp/rtp.h>

#include "FCRTPLibExports.h"
#include "uuids.h"

using namespace std;
using namespace ost;

class FCRTPLIB_API PayloadMap
{
private:
	typedef pair<GUID,GUID> GuidPair;
	typedef vector<GuidPair> GuidPairList;
	typedef map<PayloadType, GuidPairList> BackingStore;
	BackingStore mappings;

	PayloadType assignedDynamic;

	bool knownGuidMode;

public:
	/*
		Constructs a new PayloadMap
		knownGuidMode	[in]	when set to true, the payload map does not do any validity checking
	*/
	PayloadMap(bool knownGuidMode = false);
	~PayloadMap(void);

public:
	/*
		Clears all known mappings.
	*/
	void Clear();

	/*
		Returns the total of all mappings currently known.
	*/
	size_t Count();

	/*
		Gets the primary mapping of PayloadType to DirectShow type GUIDs
		payloadType		[in]	RTP payload type to get
		outMajorType	[out]	DirectShow Major Type
		outSubType		[out]	DirectShow Sub Type
		Returns true if a mapping exists, returns false if no mapping exists
	*/
	bool GetMapping(PayloadType payloadType, GUID * outMajorType, GUID * outSubType);
	/*
		Gets a list of all the DirectShow type GUIDs that could be used for a given RTP Payload
		payloadType		[in]	RTP payload type to get
		outMappings		[out]	list of all major/sub DirectShow types
		Returns true if 1 or more mappings exist, returns false if no mapping exists
	*/
	bool GetAllMappings(PayloadType payloadType, vector<pair<GUID,GUID>> * outMappings);
	/*
		Gets the RTP payload type that corresponds to this DirectShow major/sub type
		majortype		[in]	DirectShow major type
		subtype			[in]	DirectShow sub type
		outPayloadType	[out]	RTP payload type
		Returns true if a mapping exists, returns false if no mapping exists
	*/
	bool GetMapping(const GUID & majortype, const GUID & subtype, PayloadType * outPayloadType);

	/*
		Fetches a mapping in a linear fashion. Not recommended for heavy use.
		i				[in]	linear index
		outMajorType	[out]	DirectShow major type
		outSubType		[out]	DirectShow sub type
		Returns true on success, false if failure
	*/
	bool GetMappingAt(int i, GUID * outMajorType, GUID * outSubType);

	/*
		Add a mapping from RTP payload type to DirectShow media type
		payloadType		[in]	RTP payload type. ptINVALID to assign a dynamic payload type
		majortype		[in]	DirectShow major type
		subtype			[in]	DirectShow sub type
		Returns the PayloadType assigned, or ptINVALID if out of types
	*/
	PayloadType AddMapping(PayloadType payloadType, const GUID & majortype, const GUID & subtype);
	/*
		Removes all mappings for a given RTP payload type
		payloadType		[in]	RTP payload type to remove all mappings for
		Returns true if the mapping was removed
	*/
	bool RemoveMapping(PayloadType payloadType);
	/*
		Removes a DirectShow mapping.
		majortype		[in]	DirectShow major type
		subtype			[in]	DirectShow sub type
		Returns true if the mapping was removed.
		
		- In the case where you have mapped the same GUID pair to multiple, this method
		  will remove the first occurance. Call while returns true to remove all mappings
	*/
	bool RemoveMapping(const GUID & majortype, const GUID & subtype);

private:
	/*
		Returns true if the given payload type is in the mappings
	*/
	inline bool ContainsKey(PayloadType p);

	/*
		Finds a given DirectShow mapping
		major		[in]	DirectShow major type
		sub			[in]	DirectShow sub type
		key			[out]	Associated payload type
		position	[out]	iterator for position of GUID pair in key
		Returns true if a mapping was found
	*/
	bool FindMapping(const GUID & major, const GUID & sub, PayloadType * key, GuidPairList::iterator * position);
	/*
		Searches for a given DirectShow mapping for a given RTP Payload Type
		major		[in]	DirectShow major type
		sub			[in]	DirectShow sub type
		key			[in]	Associated payload type
		position	[out]	iterator for position of GUID pair in key
		Returns true if a mapping was found for this payload
	*/
	bool FindMappingInKey(const GUID & major, const GUID & sub, PayloadType key, GuidPairList::iterator * position);

};
