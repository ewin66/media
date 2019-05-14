// stdafx.h : include file for standard system include files,
//      or project specific include files that are used frequently,
//      but are changed infrequently

#if !defined(AFX_STDAFX_H__EAF15EAF_3D1F_413E_9AE3_E70C799BCF06__INCLUDED_)
#define AFX_STDAFX_H__EAF15EAF_3D1F_413E_9AE3_E70C799BCF06__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define STRICT
//#ifndef _WIN32_WINNT
//#define _WIN32_WINNT 0x0400
//#endif
#define _ATL_APARTMENT_THREADED

#define _CRT_SECURE_NO_WARNINGS

#include <atlbase.h>
//You may derive a class from CComModule and use it if you want to override
//something, but do not change the name of _Module
extern CComModule _Module;
#include <atlcom.h>
#include <winsock.h>
#include <wininet.h>
#include <time.h>
#include <assert.h>
#include <queue>
using namespace std;

#define EXECUTE_ASSERT(x) assert(x)
#include "CAMEvent.h"
#include "CCritSec.h"
#include "CAMThread.h"
#include "DataPacket.h"
#include "PacketQueue.h"
#include "netcon2.h"
#include "DGramProtocol.h"
#include "PacketReaderThread.h"
#include "CustomDGramConnection.h"

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_STDAFX_H__EAF15EAF_3D1F_413E_9AE3_E70C799BCF06__INCLUDED)
