//------------------------------------------------------------------------------
// File: CAMEvent.cpp
//
//------------------------------------------------------------------------------

#define STRSAFE_NO_DEPRECATE
//#include <strsafe.h>
#include "StdAfx.h"

// --- CAMEvent -----------------------
CAMEvent::CAMEvent(BOOL fManualReset, __inout_opt HRESULT *phr)
{
    m_hEvent = CreateEvent(NULL, fManualReset, FALSE, NULL);
    if (NULL == m_hEvent) {
        if (NULL != phr && SUCCEEDED(*phr)) {
            *phr = E_OUTOFMEMORY;
        }
    }
}

CAMEvent::CAMEvent(__inout_opt HRESULT *phr)
{
    m_hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
    if (NULL == m_hEvent) {
        if (NULL != phr && SUCCEEDED(*phr)) {
            *phr = E_OUTOFMEMORY;
        }
    }
}

CAMEvent::~CAMEvent()
{
    if (m_hEvent) {
	EXECUTE_ASSERT(CloseHandle(m_hEvent));
    }
}
