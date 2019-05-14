#pragma once
/*
	uuids.h
	Provides CLSID for the FCVolumeFilter class
	Provides IID for the IFCVolumeMute interface

	Kevin Dixon
	(c) 2009 Future Concepts
	01/20/2009
*/

#include <initguid.h>

// {EFBE0B3E-10A9-4a34-9159-CB0997812601}
DEFINE_GUID(CLSID_FCVolumeFilter, 
0xefbe0b3e, 0x10a9, 0x4a34, 0x91, 0x59, 0xcb, 0x9, 0x97, 0x81, 0x26, 0x1);

// {B28F2C9C-8951-45ad-A42B-29A75A47E7E7}
DEFINE_GUID(IID_IFCVolumeMute, 
0xb28f2c9c, 0x8951, 0x45ad, 0xa4, 0x2b, 0x29, 0xa7, 0x5a, 0x47, 0xe7, 0xe7);
