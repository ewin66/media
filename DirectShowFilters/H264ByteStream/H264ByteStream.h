//------------------------------------------------------------------------------
// File: H264ByteStream.h
//
//------------------------------------------------------------------------------


class CH264ByteStream : public CTransformFilter
{

public:
	static const int MAX_BUFSIZ = 0x40000;

    DECLARE_IUNKNOWN;
    static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    // Overrriden from CTransformFilter base class

    HRESULT Transform(IMediaSample *pIn, IMediaSample *pOut);
    HRESULT CheckInputType(const CMediaType *mtIn);
    HRESULT CheckTransform(const CMediaType *mtIn, const CMediaType *mtOut);
    HRESULT DecideBufferSize(IMemAllocator *pAlloc,
                             ALLOCATOR_PROPERTIES *pProperties);
    HRESULT GetMediaType(int iPosition, CMediaType *pMediaType);
	HRESULT StartStreaming();
	HRESULT StopStreaming();
	HRESULT AlterQuality(Quality q);

private:

    // Constructor
    CH264ByteStream(TCHAR *tszName, LPUNKNOWN punk, HRESULT *phr);

	void AppendSample(IMediaSample* pSample) {
		BYTE* pSampleBuf;
		long sampleLength = pSample->GetActualDataLength();
		pSample->GetPointer(&pSampleBuf);
		CopyMemory(m_buffer + m_bufsiz, pSampleBuf, sampleLength);
		m_bufsiz += sampleLength;
	}

	void AppendPrefix() {
		static long prefix = 0x01000000;
		CopyMemory(m_buffer + m_bufsiz, (BYTE*)&prefix, 4);
		m_bufsiz += 4;
	}

	void WriteSampleProperties(AM_SAMPLE2_PROPERTIES* props);

    CCritSec    m_lock;          // Private play critical section
	BOOL		m_hasSyncPoint;
	BYTE		m_buffer[MAX_BUFSIZ];
	int			m_bufsiz;
	REFERENCE_TIME m_currentTime;
	int			m_alterQualityCount;
	int			m_frameCount;
	FILE*		m_fp;

}; // H264ByteStream

