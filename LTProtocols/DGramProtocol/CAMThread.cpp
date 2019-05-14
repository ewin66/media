//------------------------------------------------------------------------------
// File: CAMThread.cpp
//
//------------------------------------------------------------------------------


#define STRSAFE_NO_DEPRECATE
//#include <strsafe.h>
#include "stdafx.h"

// --- CAMThread ----------------------


CAMThread::CAMThread(__inout_opt HRESULT *phr)
    : m_EventSend(TRUE, phr),     // must be manual-reset for CheckRequest()
      m_EventComplete(FALSE, phr)
{
    m_hThread = NULL;
}

CAMThread::~CAMThread() {
    Close();
}


// when the thread starts, it calls this function. We unwrap the 'this'
//pointer and call ThreadProc.
DWORD WINAPI
CAMThread::InitialThreadProc(__inout LPVOID pv)
{
    HRESULT hrCoInit = CAMThread::CoInitializeHelper();
    if(FAILED(hrCoInit)) {
//        DbgLog((LOG_ERROR, 1, TEXT("CoInitializeEx failed.")));
    }

    CAMThread * pThread = (CAMThread *) pv;

    HRESULT hr = pThread->ThreadProc();

    if(SUCCEEDED(hrCoInit)) {
        CoUninitialize();
    }

    return hr;
}

BOOL
CAMThread::Create()
{
    DWORD threadid;

    CAutoLock lock(&m_AccessLock);

    if (ThreadExists()) {
	return FALSE;
    }

    m_hThread = CreateThread(
		    NULL,
		    0,
		    CAMThread::InitialThreadProc,
		    this,
		    0,
		    &threadid);

    if (!m_hThread) {
	return FALSE;
    }

    return TRUE;
}

DWORD
CAMThread::CallWorker(DWORD dwParam)
{
    // lock access to the worker thread for scope of this object
    CAutoLock lock(&m_AccessLock);

    if (!ThreadExists()) {
	return (DWORD) E_FAIL;
    }

    // set the parameter
    m_dwParam = dwParam;

    // signal the worker thread
    m_EventSend.Set();

    // wait for the completion to be signalled
    m_EventComplete.Wait();

    // done - this is the thread's return value
    return m_dwReturnVal;
}

// Wait for a request from the client
DWORD
CAMThread::GetRequest()
{
    m_EventSend.Wait();
    return m_dwParam;
}

// is there a request?
BOOL
CAMThread::CheckRequest(__out_opt DWORD * pParam)
{
    if (!m_EventSend.Check()) {
	return FALSE;
    } else {
	if (pParam) {
	    *pParam = m_dwParam;
	}
	return TRUE;
    }
}

// reply to the request
void
CAMThread::Reply(DWORD dw)
{
    m_dwReturnVal = dw;

    // The request is now complete so CheckRequest should fail from
    // now on
    //
    // This event should be reset BEFORE we signal the client or
    // the client may Set it before we reset it and we'll then
    // reset it (!)

    m_EventSend.Reset();

    // Tell the client we're finished

    m_EventComplete.Set();
}

HRESULT CAMThread::CoInitializeHelper()
{
    // call CoInitializeEx and tell OLE not to create a window (this
    // thread probably won't dispatch messages and will hang on
    // broadcast msgs o/w).
    //
    // If CoInitEx is not available, threads that don't call CoCreate
    // aren't affected. Threads that do will have to handle the
    // failure. Perhaps we should fall back to CoInitialize and risk
    // hanging?
    //

    // older versions of ole32.dll don't have CoInitializeEx

    HRESULT hr = E_FAIL;
    HINSTANCE hOle = GetModuleHandle(TEXT("ole32.dll"));
    if(hOle)
    {
        typedef HRESULT (STDAPICALLTYPE *PCoInitializeEx)(
            LPVOID pvReserved, DWORD dwCoInit);
        PCoInitializeEx pCoInitializeEx =
            (PCoInitializeEx)(GetProcAddress(hOle, "CoInitializeEx"));
        if(pCoInitializeEx)
        {
            hr = (*pCoInitializeEx)(0, COINIT_DISABLE_OLE1DDE );
        }
    }
    else
    {
        // caller must load ole32.dll
//        DbgBreak("couldn't locate ole32.dll");
    }

    return hr;
}
