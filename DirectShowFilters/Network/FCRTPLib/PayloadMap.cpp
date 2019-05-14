#include "PayloadMap.h"

PayloadMap::PayloadMap(void)
	:mappings()
{
}

PayloadMap::~PayloadMap(void)
{
}

void PayloadMap::ApplyDefaultMappings()
{
	AddMapping(sptPCMU, MEDIATYPE_Audio, MEDIASUBTYPE_PCM);

	AddMapping(sptMPA, MEDIATYPE_Audio, MEDIASUBTYPE_MPEG1Packet);
	AddMapping(sptMPA, MEDIATYPE_Audio, MEDIASUBTYPE_MPEG1Payload);
	AddMapping(sptMPA, MEDIATYPE_Stream, MEDIASUBTYPE_MPEG1Audio);
	AddMapping(sptMPA, MEDIATYPE_Audio, MEDIASUBTYPE_MPEG2_AUDIO);

	AddMapping(sptJPEG, MEDIATYPE_Video, MEDIASUBTYPE_MJPG);

	AddMapping(sptMPV, MEDIATYPE_Video, MEDIASUBTYPE_MPEG1Packet);
	AddMapping(sptMPV, MEDIATYPE_Video, MEDIASUBTYPE_MPEG1Payload);
	AddMapping(sptMPV, MEDIATYPE_Stream, MEDIASUBTYPE_MPEG1Video);
	AddMapping(sptMPV, MEDIATYPE_Video, MEDIASUBTYPE_MPEG2_VIDEO);

	AddMapping(sptMP2T, MEDIATYPE_Stream, MEDIASUBTYPE_MPEG2_TRANSPORT);
}

void PayloadMap::AddMappingForH264(PayloadType payloadType)
{
	AddMapping(payloadType, MEDIATYPE_Video, MEDIASUBTYPE_H264_NALU_AVC1);
	AddMapping(payloadType, MEDIATYPE_Video, MEDIASUBTYPE_H264_NALU_avc1);
	AddMapping(payloadType, MEDIATYPE_Video, MEDIASUBTYPE_H264_NALU_LT);
}

void PayloadMap::Clear()
{
	mappings.clear();
}

size_t PayloadMap::Count()
{
	size_t items = 0;
	for(BackingStore::iterator i = mappings.begin(); i != mappings.end(); i++)
	{
		items += i->second.size();
	}
	return items;
}

bool PayloadMap::GetMapping(PayloadType payloadType, GUID * outMajor, GUID * outSub)
{
	if(ContainsKey(payloadType))
	{
		if(mappings[payloadType].size() > 0)
		{
			(*outMajor) = mappings[payloadType][0].first;
			(*outSub) = mappings[payloadType][0].second;
			return true;
		}
	}
	return false;
}


bool PayloadMap::GetAllMappings(PayloadType payloadType, vector<pair<GUID,GUID>> * outMappings)
{
	if(ContainsKey(payloadType))
	{
		(*outMappings) = mappings[payloadType];
		return true;
	}
	return false;
}

bool PayloadMap::GetMapping(const GUID & majortype, const GUID & subtype, PayloadType * outPayloadType)
{
	GuidPairList::iterator i;
	return FindMapping(majortype, subtype, outPayloadType, &i);
}

bool PayloadMap::GetMappingAt(int i, GUID * outMajorType, GUID * outSubType)
{
	int progress = 0;
	for(BackingStore::iterator m = mappings.begin(); m != mappings.end(); m++)
	{
		if((progress + m->second.size()) < i)
		{
			progress += m->second.size();
		}
		else
		{
			for(GuidPairList::iterator u = m->second.begin(); u != m->second.end(); u++)
			{
				if(i == progress)
				{
					(*outMajorType) = u->first;
					(*outSubType) = u->second;
					return true;
				}
				progress++;
			}
		}
	}
	return false;
}

void PayloadMap::AddMapping(PayloadType payloadType, const GUID & majortype, const GUID & subtype)
{
	if(!ContainsKey(payloadType))
	{
		mappings[payloadType] = GuidPairList();
	}

	mappings[payloadType].push_back(GuidPair(majortype, subtype));
}

bool PayloadMap::RemoveMapping(PayloadType payloadType)
{
	return (mappings.erase(payloadType) > 0);
}

bool PayloadMap::RemoveMapping(const GUID & majortype, const GUID & subtype)
{
	PayloadType key;
	GuidPairList::iterator pos;
	if(FindMapping(majortype, subtype, &key, &pos))
	{
		if(mappings[key].size() == 1)
		{
			mappings.erase(key);
		}
		else
		{
			mappings[key].erase(pos);
		}
		return true;
	}
	return false;
}

bool PayloadMap::ContainsKey(PayloadType p)
{
	return mappings.end() != mappings.find(p);
}

bool PayloadMap::FindMapping(const GUID & major, const GUID & sub, PayloadType * key, GuidPairList::iterator * position)
{
	for(BackingStore::iterator k = mappings.begin(); k != mappings.end(); k++)
	{
		if(FindMappingInKey(major, sub, k->first, position))
		{
			(*key) = k->first;
			return true;
		}
	}
	return false;
}

bool PayloadMap::FindMappingInKey(const GUID & major, const GUID & sub, PayloadType key, GuidPairList::iterator * position)
{
	for(GuidPairList::iterator t = mappings[key].begin(); t != mappings[key].end(); t++)
	{
		if((t->first == major) && (t->second == sub))
		{
			(*position) = t;
			return true;
		}
	}
	return false;
}