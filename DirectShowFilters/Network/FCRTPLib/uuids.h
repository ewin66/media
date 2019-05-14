#pragma once
/*
	uuids.h
	Defines GUIDs for FCNetworkSink, FCNetworkSource, and common GUIDs

	Kevin C. Dixon
	Future Concepts
	03/19/2009
*/

#include <streams.h>

// {E8DEC16D-A648-4786-9733-BBC09FF343B6}
static const GUID CLSID_FCNetworkSink = {0xe8dec16d, 0xa648, 0x4786, 0x97, 0x33, 0xbb, 0xc0, 0x9f, 0xf3, 0x43, 0xb6};

// {40867870-8D0F-4f82-9FD7-13A2B1B8DD63}
static const GUID CLSID_FCNetworkSource = {0x40867870, 0x8d0f, 0x4f82, 0x9f, 0xd7, 0x13, 0xa2, 0xb1, 0xb8, 0xdd, 0x63};



//H.264 Annex B
static const GUID MEDIASUBTYPE_H264_NONSTD = {0x8d2d71cb, 0x243f, 0x45e3, 0xb2, 0xd8, 0x5f, 0xd7, 0x96, 0x7e, 0xc0, 0x9b};

//H.264 AnnexB
//static const GUID MEDIASUBTYPE_H264 = (GUID)FOURCCMap(MAKEFOURCC('H', '2', '6', '4'));
static const GUID MEDIASUBTYPE_h264 = (GUID)FOURCCMap(MAKEFOURCC('h', '2', '6', '4'));

//x264 opensource project -- AnnexB
static const GUID MEDIASUBTYPE_H264_X264 = (GUID)FOURCCMap(MAKEFOURCC('X', '2', '6', '4'));

//AVC1 -- NALU
static const GUID MEDIASUBTYPE_H264_NALU_AVC1 = (GUID)FOURCCMap(MAKEFOURCC('A', 'V', 'C', '1'));
//avc1 -- NALU
static const GUID MEDIASUBTYPE_H264_NALU_avc1 = (GUID)FOURCCMap(MAKEFOURCC('a', 'v', 'c', '1'));
//LEADTOOLS AVC1
static const GUID MEDIASUBTYPE_H264_NALU_LT = {MAKEFOURCC('a', 'v', 'c', '1'), 0x5349, 0x4D4F, {0x45, 0x44, 0x49, 0x41, 0x54, 0x59, 0x50, 0x45}};


