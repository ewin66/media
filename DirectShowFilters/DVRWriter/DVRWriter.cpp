//------------------------------------------------------------------------------
// File: DVRWriter.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

STDMETHODIMP CDVRWriter::SetFileName(LPCOLESTR pszFileName, const AM_MEDIA_TYPE* pmt)
{
	OutputDebugStringA("DVRWriter::SetFileName");
	CheckPointer(pszFileName, E_POINTER);
	if (wcslen(pszFileName) > MAX_PATH)
	{
		return ERROR_FILENAME_EXCED_RANGE;
	}
	size_t len = 1+lstrlenW(pszFileName);
	m_pFileName = new WCHAR[len];
	if (m_pFileName == 0)
	{
		return E_OUTOFMEMORY;
	}
	HRESULT hr = StringCchCopyW(m_pFileName, len, pszFileName);

	m_fWriteError = FALSE;
	return S_OK;
}

STDMETHODIMP CDVRWriter::GetCurFile(LPOLESTR* ppszFileName, AM_MEDIA_TYPE* pmt)
{
	CheckPointer(ppszFileName, E_POINTER);
	*ppszFileName = NULL;

	if (m_pFileName != NULL)
	{
		size_t len = 1+lstrlenW(m_pFileName);
		*ppszFileName = (LPOLESTR)CoTaskMemAlloc(sizeof(WCHAR)*(len));
		if (*ppszFileName != NULL)
		{
			HRESULT hr = StringCchCopyW(*ppszFileName, len, m_pFileName);
		}
	}
	if (pmt)
	{
		ZeroMemory(pmt, sizeof(*pmt));
		pmt->majortype = MEDIATYPE_NULL;
		pmt->subtype = MEDIASUBTYPE_NULL;
	}
	return S_OK;
}

//
//  CDVRWriter class
//
CDVRWriter::CDVRWriter(LPUNKNOWN pUnk, HRESULT *phr) :
    CUnknown(NAME("CDVRWriter"), pUnk),
    m_pFilter(NULL),
    m_pPin(NULL),
    m_pPosition(NULL),
	m_hFile(INVALID_HANDLE_VALUE),
    m_fWriteError(0)
{
    ASSERT(phr);
    
	DbgSetModuleLevel(LOG_TRACE, 0);
    m_pFilter = new CDVRWriterFilter(this, GetOwner(), &m_Lock, phr);
    if (m_pFilter == NULL) {
        if (phr)
            *phr = E_OUTOFMEMORY;
        return;
    }

    m_pPin = new CDVRWriterInputPin(this,GetOwner(),
                               m_pFilter,
                               &m_Lock,
                               &m_ReceiveLock,
                               phr);
    if (m_pPin == NULL) {
        if (phr)
            *phr = E_OUTOFMEMORY;
        return;
    }
	m_currentChunkSize = 0;
}

// Destructor

CDVRWriter::~CDVRWriter()
{
    CloseFile();

    delete m_pPin;
    delete m_pFilter;
    delete m_pPosition;
	delete m_pFileName;
}

//1
// CreateInstance
//
// Provide the way for COM to create a dump filter
//
CUnknown * WINAPI CDVRWriter::CreateInstance(LPUNKNOWN punk, HRESULT *phr)
{
    ASSERT(phr);
    
    CDVRWriter *pNewObject = new CDVRWriter(punk, phr);
    if (pNewObject == NULL)
	{
        if (phr)
		{
			*phr = E_OUTOFMEMORY;
		}
    }

    return pNewObject;

} // CreateInstance


//
// NonDelegatingQueryInterface
//
// Override this to say what interfaces we support where
//
STDMETHODIMP CDVRWriter::NonDelegatingQueryInterface(REFIID riid, void ** ppv)
{
    CheckPointer(ppv,E_POINTER);
    CAutoLock lock(&m_Lock);

    // Do we have this interface

	if (riid == IID_IFileSinkFilter)
	{
		return GetInterface((IFileSinkFilter*) this, ppv);
	}
	else if (riid == IID_IDVRWriterApi)
	{
		return GetInterface(static_cast<IDVRWriterApi*>(this), ppv);
	}
	else if (riid == IID_IBaseFilter || riid == IID_IMediaFilter || riid == IID_IPersist)
	{
        return m_pFilter->NonDelegatingQueryInterface(riid, ppv);
    } 
    else if (riid == IID_IMediaPosition || riid == IID_IMediaSeeking) {
        if (m_pPosition == NULL) 
        {
            HRESULT hr = S_OK;
            m_pPosition = new CPosPassThru(NAME("DVR Writer Pass Through"),
                                           (IUnknown *) GetOwner(),
                                           (HRESULT *) &hr, m_pPin);
            if (m_pPosition == NULL) 
			{
                return E_OUTOFMEMORY;
			}
            if (FAILED(hr)) 
            {
                delete m_pPosition;
                m_pPosition = NULL;
                return hr;
            }
        }
        return m_pPosition->NonDelegatingQueryInterface(riid, ppv);
    } 

    return CUnknown::NonDelegatingQueryInterface(riid, ppv);

} // NonDelegatingQueryInterface

//
// OpenFile
//
// Opens the file ready for writing
//
HRESULT CDVRWriter::OpenFile()
{
	OutputDebugStringA("DVRWriter::OpenFile");
	CAutoLock lock(&m_Lock);
	if (m_hFile != INVALID_HANDLE_VALUE)
	{
		return NOERROR;
	}
	if (m_pFileName == NULL)
	{
		return ERROR_INVALID_NAME;
	}
	m_hFile = CreateFile((LPCTSTR)m_pFileName,
		GENERIC_WRITE,
		FILE_SHARE_READ,
		NULL,
		CREATE_ALWAYS,
		(DWORD)0,
		NULL);
	if (m_hFile == INVALID_HANDLE_VALUE)
	{
		DWORD dwErr = GetLastError();
		return HRESULT_FROM_WIN32(dwErr);
	}
	m_currentChunkSize = 0;
	return S_OK;

} // Open

//
// CloseFile
//
// Closes any file we have opened
//
HRESULT CDVRWriter::CloseFile()
{
	OutputDebugStringA("DVRWriter::CloseFile");
    // Must lock this section to prevent problems related to
    // closing the file while still receiving data in Receive()

	CAutoLock lock(&m_Lock);
	if (m_hFile == INVALID_HANDLE_VALUE)
	{
		return NOERROR;
	}
	CloseHandle(m_hFile);
	m_hFile = INVALID_HANDLE_VALUE;
    return NOERROR;
} // CloseFile

//
// Read
//
// Read raw data from the file
//
HRESULT CDVRWriter::Read(PBYTE pbData, LONG lBufferLength, LPDWORD actualBytesRead)
{
	if (ReadFile(m_hFile, pbData, lBufferLength, actualBytesRead, NULL) == FALSE)
	{
		return (HandleWriterFailure());
	}
	return S_OK;
}

//
// Write
//
// Write raw data to the socket
//
HRESULT CDVRWriter::Write(PBYTE pbData, LONG lDataLength, ULONG* pcbWritten)
{
	DWORD dwWritten;

	if (m_hFile == INVALID_HANDLE_VALUE)
	{
		return S_FALSE;
	}
	if (!WriteFile(m_hFile, (PVOID)pbData, (DWORD)lDataLength, pcbWritten, NULL))
	{
		return (HandleWriterFailure());
	}
	m_currentChunkSize += lDataLength;
    return S_OK;
}

HRESULT CDVRWriter::Seek(LARGE_INTEGER pos, PLARGE_INTEGER lpNewFilePointer)
{
	if (SetFilePointerEx(m_hFile, pos, lpNewFilePointer, FILE_BEGIN) == FALSE)
	{
		return HandleWriterFailure();
	}
	else
	{
		return S_OK;
	}
}

HRESULT CDVRWriter::HandleWriterFailure(void)
{
    DWORD dwErr = GetLastError();

    if (dwErr == ERROR_DISK_FULL)
    {
        // Close the dump file and stop the filter, 
        // which will prevent further write attempts
        m_pFilter->Stop();

        // Set a global flag to prevent accidental deletion of the dump file
        m_fWriteError = TRUE;

        // Display a message box to inform the developer of the write failure
        TCHAR szMsg[MAX_PATH + 80];
        HRESULT hr = StringCchPrintf(szMsg, MAX_PATH + 80, TEXT("The disk containing dump file has run out of space, ")
                  TEXT("so the dump filter has been stopped.\r\n\r\n")
                  TEXT("You must set a new dump file name or restart the graph ")
                  TEXT("to clear this filter error."));
		wprintf(L"%s", szMsg);
    }
    return HRESULT_FROM_WIN32(dwErr);
}
