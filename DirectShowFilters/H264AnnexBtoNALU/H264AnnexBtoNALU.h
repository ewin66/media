/*
	H264AnnexBtoNALU.h
	H264AnnexBToNALU

	Header for the H.264 AnnexB to NALU transform filter class
	
	06/17/2009
	Kevin Dixon
	Copyright (c) 2009 Future Concepts
*/

#include <streams.h>
#include "guids.h"

class H264AnnexBtoNALU : CTransformFilter
{
private:
	static const int MAX_BUFSIZ = 0x40000;

public:
	H264AnnexBtoNALU(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr);
	~H264AnnexBtoNALU(void);

	static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    DECLARE_IUNKNOWN;

	HRESULT Transform(IMediaSample * pIn, IMediaSample * pOut);

	HRESULT CheckInputType(const CMediaType * mtIn);
	HRESULT CheckTransform(const CMediaType * mtIn, const CMediaType * mtOut);

	HRESULT DecideBufferSize(IMemAllocator * pAllocator, ALLOCATOR_PROPERTIES * pprop);

	HRESULT GetMediaType(int iPosition, CMediaType * pMediaType);

	HRESULT StartStreaming();
	HRESULT StopStreaming();
	HRESULT AlterQuality(Quality q);

private:
	bool GetNextNALURange(const BYTE * data, const long & dataLength, long * startIndex, long * endIndex);
	HRESULT DeliverRange(const BYTE * data, const long & startIndex, const long & endIndex, REFERENCE_TIME & startTime);
	HRESULT PopulateOutputSample(IMediaSample ** ppOutSample, const BYTE * data, const long & startIndex, const long & endIndex, REFERENCE_TIME & startTime);

	inline bool IsAnnexBPrefix(const BYTE * data);
};
