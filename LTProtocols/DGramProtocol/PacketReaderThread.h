#ifndef _PACKET_READER_THREAD_H
#define _PACKET_READER_THREAD_H

class CCustomDGramConnection;

class PacketReaderThread : public CAMThread
{
public:
	PacketReaderThread(CCustomDGramConnection* connection) : m_connection(connection), m_bRunning(true)
	{
	}

	DWORD ThreadProc();

	void Stop()
	{
		m_bRunning = false;
	}

private:
	CCustomDGramConnection* m_connection;
	bool m_bRunning;
};

#endif
