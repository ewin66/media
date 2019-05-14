#pragma once

#include <streams.h>
#include <ccrtp/rtp.h>

#include "FCRTPLibExports.h"
#include "FCRTPLibFormatters.h"
#include "PayloadMap.h"
#include "uuids.h"

using namespace ost;

class FCRTPLIB_API PayloadFormatterFactory
{
private:
	PayloadFormatterFactory(void) { }
public:

	static inline PayloadFormatter * Create(PayloadMap * mappings, const GUID & majortype, const GUID & subtype);
	static inline PayloadFormatter * Create(PayloadMap * mappings, const PayloadType & payloadType);

	static inline bool IsH264NALU(const GUID & major, const GUID & sub);

private:
	static inline PayloadFormatter * Create(PayloadMap * mappings, const PayloadType & t, const GUID & major, const GUID & sub);
};
