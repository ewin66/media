//------------------------------------------------------------------------------
// File: CAMThread.h
//
//------------------------------------------------------------------------------


#ifndef __CAMTHREAD__
#define __CAMTHREAD__

// eliminate spurious "statement has no effect" warnings.
#pragma warning(disable: 4705)

// support for a worker thread

// simple thread class supports creation of worker thread, synchronization
// and communication. Can be derived to simplify parameter passing
class CAMThread {

    // make copy constructor and assignment operator inaccessible

    CAMThread(const CAMThread &refThread);
    CAMThread &operator=(const CAMThread &refThread);

    CAMEvent m_EventSend;
    CAMEvent m_EventComplete;

    DWORD m_dwParam;
    DWORD m_dwReturnVal;

protected:
    HANDLE m_hThread;

    // thread will run this function on startup
    // must be supplied by derived class
    virtual DWORD ThreadProc() = 0;

public:
    CAMThread(__inout_opt HRESULT *phr = NULL);
    virtual ~CAMThread();

    CCritSec m_AccessLock;	// locks access by client threads
    CCritSec m_WorkerLock;	// locks access to shared objects

    // thread initially runs this. param is actually 'this'. function
    // just gets this and calls ThreadProc
    static DWORD WINAPI InitialThreadProc(__inout LPVOID pv);

    // start thread running  - error if already running
    BOOL Create();

    // signal the thread, and block for a response
    //
    DWORD CallWorker(DWORD);

    // accessor thread calls this when done with thread (having told thread
    // to exit)
    void Close() {
        HANDLE hThread = (HANDLE)InterlockedExchangePointer(&m_hThread, 0);
        if (hThread) {
            WaitForSingleObject(hThread, INFINITE);
            CloseHandle(hThread);
        }
    };

    // ThreadExists
    // Return TRUE if the thread exists. FALSE otherwise
    BOOL ThreadExists(void) const
    {
        if (m_hThread == 0) {
            return FALSE;
        } else {
            return TRUE;
        }
    }

	int Priority_get()
	{
		return GetThreadPriority(m_hThread);
	}

	BOOL Priority_set(int value)
	{
		return SetThreadPriority(m_hThread, value);
	}

    // wait for the next request
    DWORD GetRequest();

    // is there a request?
    BOOL CheckRequest(__out_opt DWORD * pParam);

    // reply to the request
    void Reply(DWORD);

    // If you want to do WaitForMultipleObjects you'll need to include
    // this handle in your wait list or you won't be responsive
    HANDLE GetRequestHandle() const { return m_EventSend; };

    // Find out what the request was
    DWORD GetRequestParam() const { return m_dwParam; };

    // call CoInitializeEx (COINIT_DISABLE_OLE1DDE) if
    // available. S_FALSE means it's not available.
    static HRESULT CoInitializeHelper();
};

#endif
