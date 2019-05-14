#include "PayloadFormatterFactory.h"

PayloadFormatter * PayloadFormatterFactory::Create(PayloadMap * mappings, const GUID & majortype, const GUID & subtype)
{
	PayloadType t;
	if(mappings->GetMapping(majortype, subtype, &t))
	{
		return Create(mappings, t, majortype, subtype);
	}
	return NULL;
}

PayloadFormatter * PayloadFormatterFactory::Create(PayloadMap * mappings, const PayloadType & payloadType)
{
	GUID major, sub;
	if(mappings->GetMapping(payloadType, &major, &sub))
	{
		return Create(mappings, payloadType, major, sub);
	}
	return NULL;
}

PayloadFormatter * PayloadFormatterFactory::Create(PayloadMap * mappings, const PayloadType & t, const GUID & major, const GUID & sub)
{
	if((t >= firstStaticPayloadType) && (t <= lastStaticPayloadType))
	{
		return new DefaultFormatter(new StaticPayloadFormat((StaticPayloadType)t));
	}
	else if(IsH264NALU(major, sub))
	{
		return new H264Formatter(1400, new DynamicPayloadFormat(t, 90000));
	}
	return NULL;
}

bool PayloadFormatterFactory::IsH264NALU(const GUID & major, const GUID & sub)
{
	if(major == MEDIATYPE_Video)
	{
		if((sub == MEDIASUBTYPE_H264_NALU_AVC1) ||
		   (sub == MEDIASUBTYPE_H264_NALU_avc1) ||
		   (sub == MEDIASUBTYPE_H264_NALU_LT))
		{
			return true;
		}
	}
	return false;
}