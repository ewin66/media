#include "PayloadFormatterFactory.h"

/*
	Creates a PayloadFormatter based on the current mappings, and the indicated DirectShow media type
	mappings	[in]	mappings to utilize
	majortype	[in]	DirectShow media major type
	subtype		[in]	DirectShow media sub type
	Returns a new instance of a PayloadFormatter (must be freed by caller), or NULL if no formatter could be found
*/
PayloadFormatter * PayloadFormatterFactory::Create(PayloadMap * mappings, const GUID & majortype, const GUID & subtype)
{
	PayloadType t;
	if(mappings->GetMapping(majortype, subtype, &t))
	{
		return Create(mappings, t, majortype, subtype);
	}
	return NULL;
}

/*
	Creates a PayloadFormatter based on the current mappings and the indicated RTP Payload type
	mappings	[in]	mappings to utilize
	payloadType	[in]	RTP numeric payload type
	Returns a new instance of a PayloadFormatter (must be freed by caller), or NULL if no formatter could be found
*/
PayloadFormatter * PayloadFormatterFactory::Create(PayloadMap * mappings, const PayloadType & payloadType)
{
	GUID major, sub;
	if(mappings->GetMapping(payloadType, &major, &sub))
	{
		return Create(mappings, payloadType, major, sub);
	}
	return NULL;
}

/*
	Creates a PayloadFormatter for the fully described payload type
	mappings	[in]	mappings to utilize (not used)
	t			[in]	payload type
	major		[in]	DS media major type
	sub			[in]	DS media sub type
	Returns a new instance of a PayloadFormatter (must be freed by caller), or NULL if no formatter could be found
*/
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

/*
	Creates a new instance of a PayloadMap, populated with all supported payload types
	Caller must free this memory
*/
PayloadMap * PayloadFormatterFactory::GetSupportedPayloadTypes()
{
	PayloadMap * m = new PayloadMap(true);

	//static mappings
	m->AddMapping(sptPCMU, MEDIATYPE_Audio, MEDIASUBTYPE_PCM);

	m->AddMapping(sptMPA, MEDIATYPE_Audio, MEDIASUBTYPE_MPEG1Packet);
	m->AddMapping(sptMPA, MEDIATYPE_Audio, MEDIASUBTYPE_MPEG1Payload);
	m->AddMapping(sptMPA, MEDIATYPE_Stream, MEDIASUBTYPE_MPEG1Audio);
	m->AddMapping(sptMPA, MEDIATYPE_Audio, MEDIASUBTYPE_MPEG2_AUDIO);

	m->AddMapping(sptJPEG, MEDIATYPE_Video, MEDIASUBTYPE_MJPG);

	m->AddMapping(sptMPV, MEDIATYPE_Video, MEDIASUBTYPE_MPEG1Packet);
	m->AddMapping(sptMPV, MEDIATYPE_Video, MEDIASUBTYPE_MPEG1Payload);
	m->AddMapping(sptMPV, MEDIATYPE_Stream, MEDIASUBTYPE_MPEG1Video);
	m->AddMapping(sptMPV, MEDIATYPE_Video, MEDIASUBTYPE_MPEG2_VIDEO);

	m->AddMapping(sptMP2T, MEDIATYPE_Stream, MEDIASUBTYPE_MPEG2_TRANSPORT);

	//dynamic mappings

	//H.264
	m->AddMapping(ptINVALID, MEDIATYPE_Video, MEDIASUBTYPE_H264_NALU_AVC1);
	m->AddMapping(ptINVALID, MEDIATYPE_Video, MEDIASUBTYPE_H264_NALU_avc1);
	m->AddMapping(ptINVALID, MEDIATYPE_Video, MEDIASUBTYPE_H264_NALU_LT);

	return m;
}

/*
	Returns true if the media major/sub represents a stream of H.264 NALU
	major	[in]	DS media major type
	sub		[in]	DS media sub type
*/
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