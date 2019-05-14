//------------------------------------------------------------------------------
// File: DVRWriter.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

class CDVRWriterInputPin;
class CDVRWriter;
class CDVRFilter;


//  CDVRWriter object which has filter and pin members

class CDVRWriter : public CUnknown, public IFileSinkFilter, public IDVRWriterApi
{
    friend class CDVRWriterFilter;
    friend class CDVRWriterInputPin;

    CDVRWriterFilter   *m_pFilter;       // Methods for filter interfaces
    CDVRWriterInputPin *m_pPin;          // A simple rendered input pin

    CCritSec m_Lock;                // Main renderer critical section
    CCritSec m_ReceiveLock;         // Sublock for received samples

    CPosPassThru *m_pPosition;      // Renderer position controls
   
	HANDLE		m_hFile;               // Handle to file for dumping
    LPOLESTR m_pFileName;           // The filename where we dump

    BOOL     m_fWriteError;

	int		m_currentChunkSize;

public:

    DECLARE_IUNKNOWN

    CDVRWriter(LPUNKNOWN pUnk, HRESULT *phr);
    ~CDVRWriter();

    static CUnknown * WINAPI CreateInstance(LPUNKNOWN punk, HRESULT *phr);

    // FILE IO
	HRESULT Read(PBYTE pbData, LONG lBufferLength, LPDWORD bytesRead);
    HRESULT Write(PBYTE pbData, LONG lDataLength, ULONG* pcbWritten);
	HRESULT Seek(LARGE_INTEGER pos, PLARGE_INTEGER pNewFilePointer);

    // Implements the IFileSinkFilter interface
    STDMETHODIMP SetFileName(LPCOLESTR pszFileName,const AM_MEDIA_TYPE *pmt);
    STDMETHODIMP GetCurFile(LPOLESTR * ppszFileName,AM_MEDIA_TYPE *pmt);

	// IDVRWriterApi


	STDMETHODIMP StartRecording()
	{
		return OpenFile();
	}
	STDMETHODIMP StopRecording()
	{
		return CloseFile();
	}
	STDMETHODIMP get_CurrentChunkSize(int* lSize)
	{
		*lSize = m_currentChunkSize;
		return S_OK;
	}

private:

    // Overriden to say what interfaces we support where
    STDMETHODIMP NonDelegatingQueryInterface(REFIID riid, void ** ppv);

    // Open and write to the file
    HRESULT OpenFile();
    HRESULT CloseFile();

	HRESULT HandleWriterFailure();

private:
};
