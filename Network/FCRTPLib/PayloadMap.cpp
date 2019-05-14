#include "PayloadMap.h"

PayloadMap::PayloadMap(bool knownGuidMode)
	:mappings(), assignedDynamic(95), knownGuidMode(knownGuidMode)
{
}

PayloadMap::~PayloadMap(void)
{
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
		if((progress + m->second.size()) < (unsigned int)i)
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

PayloadType PayloadMap::AddMapping(PayloadType payloadType, const GUID & majortype, const GUID & subtype)
{
	if(!knownGuidMode)
	{
		if(payloadType == ptINVALID)
		{
			if(assignedDynamic >= ptINVALID) return ptINVALID;
			payloadType = ++assignedDynamic;
		}
	}

	if(!ContainsKey(payloadType))
	{
		mappings[payloadType] = GuidPairList();
	}

	mappings[payloadType].push_back(GuidPair(majortype, subtype));

	return payloadType;
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