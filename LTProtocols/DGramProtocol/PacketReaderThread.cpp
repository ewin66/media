// PacketReaderThread.cpp : Implementation of PacketReaderThread

#include "stdafx.h"

DWORD PacketReaderThread::ThreadProc()
{
	while (m_bRunning == true)
	{
		if (m_connection->RecvAndQueue() == false)
		{
			OutputDebugStringA("PacketReaderThread stopping");
			return 0;
		}
	}
	m_bRunning = false;
	return 0;
}
