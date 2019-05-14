//------------------------------------------------------------------------------
// File: CAMEvent.h
//------------------------------------------------------------------------------


#ifndef __CAMEVENT__
#define __CAMEVENT__

// eliminate spurious "statement has no effect" warnings.
#pragma warning(disable: 4705)


// wrapper for event objects
class CAMEvent
{

    // make copy constructor and assignment operator inaccessible

    CAMEvent(const CAMEvent &refEvent);
    CAMEvent &operator=(const CAMEvent &refEvent);

protected:
    HANDLE m_hEvent;
public:
    CAMEvent(BOOL fManualReset = FALSE, __inout_opt HRESULT *phr = NULL);
    CAMEvent(__inout_opt HRESULT *phr);
    ~CAMEvent();

    // Cast to HANDLE - we don't support this as an lvalue
    operator HANDLE () const { return m_hEvent; };

    void Set() {EXECUTE_ASSERT(SetEvent(m_hEvent));};
    BOOL Wait(DWORD dwTimeout = INFINITE) {
	return (WaitForSingleObject(m_hEvent, dwTimeout) == WAIT_OBJECT_0);
    };
    void Reset() { ResetEvent(m_hEvent); };
    BOOL Check() { return Wait(0); };
};

// old name supported for the time being
#define CTimeoutEvent CAMEvent

#endif

