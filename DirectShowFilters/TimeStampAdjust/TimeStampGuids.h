/*
	TimeStampGuids.h
	UUIDs relating to FC Time Stamp Adjust

	Kevin Dixon
	Copyright (c) 2009 Future Concepts
	12/22/2009
*/


#ifndef __TIMESTAMPADJUSTGUIDS__
#define __TIMESTAMPADJUSTGUIDS__

#ifdef __cplusplus
extern "C" {
#endif

// {E65E29C7-86D3-4c00-8C8C-46BFCC1B712D}
DEFINE_GUID(CLSID_TimeStampAdjust, 
0xe65e29c7, 0x86d3, 0x4c00, 0x8c, 0x8c, 0x46, 0xbf, 0xcc, 0x1b, 0x71, 0x2d);

// {E7A1FAEF-C25F-4eee-B26B-72C88B03742C}
DEFINE_GUID(IID_IFCTimeStampAdjust, 
0xe7a1faef, 0xc25f, 0x4eee, 0xb2, 0x6b, 0x72, 0xc8, 0x8b, 0x3, 0x74, 0x2c);


#ifdef __cplusplus
}
#endif

#endif // __TIMESTAMPGUIDS__
