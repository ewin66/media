#ifndef _DATA_PACKET_H
#define _DATA_PACKET_H

#define DATAPACKET_BUFSIZ 8192

class DataPacket
{
public:

	DataPacket()
	{
		dp_buflen = 0;
		dp_type = 99;
	}

	DataPacket(const DataPacket& rhs)
		: dp_type(rhs.dp_type),	dp_seqno(rhs.dp_seqno),	dp_param1(rhs.dp_param1), dp_param2(rhs.dp_param2),	dp_buflen(rhs.dp_buflen)
	{
		CopyMemory((void*)dp_buffer, (void*)rhs.dp_buffer, rhs.dp_buflen);
	}

public:
	int					dp_type;
	unsigned long		dp_seqno;
	int					dp_param1;
	int					dp_param2;
	unsigned long		dp_buflen;
	char				dp_buffer[DATAPACKET_BUFSIZ];
};

inline bool operator< (const DataPacket& packet1, const DataPacket& packet2)
{
	return packet1.dp_seqno > packet2.dp_seqno;
}

inline bool operator> (const DataPacket& packet1, const DataPacket& packet2)
{
	return packet1.dp_seqno < packet2.dp_seqno;
}

#endif
