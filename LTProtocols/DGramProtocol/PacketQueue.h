#ifndef _PACKET_QUEUE_H
#define _PACKET_QUEUE_H

class PacketQueue
{
public:
	PacketQueue();

	void Insert(DataPacket& packet)
	{
		CAutoLock lock(&m_lock);
		m_queue.push(packet);
		if (m_queue.size() > m_waitSize)
		{
			m_packetReady.Set();
		}
	}

	void InsertEnd()
	{
		CAutoLock lock(&m_lock);
		DataPacket packet;
		m_queue.push(packet);
		m_packetReady.Set();
		m_waitSize = 0;
	}

	DataPacket Remove()
	{
		if (m_packetReady.Wait() == TRUE)
		{
			CAutoLock lock(&m_lock);
			DataPacket dp = m_queue.top();
			m_queue.pop();
			if (m_waitSize == m_thresholdHigh)
			{
				m_waitSize = m_thresholdLow;
			}
			if (m_queue.size() <= m_waitSize)
			{
				m_packetReady.Reset();
				if (m_queue.size() <= m_thresholdLow)
				{
					m_waitSize = m_thresholdHigh;
				}
			}
			return dp;
		}
	}

public:
	priority_queue<DataPacket, vector<DataPacket>, less<vector<DataPacket>::value_type>> m_queue;
	CAMEvent m_packetReady;
	CCritSec m_lock;
	int m_waitSize;
	int m_thresholdLow;
	int m_thresholdHigh;
};


#endif
