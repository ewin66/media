typedef enum
{
	PortNegotiate,
	PortNegotiated,
	SenderReport,
	Terminate
} CMTYPE;

struct ControlMessage
{
	CMTYPE	cm_type;
	int		cm_packet_count;
	short	cm_port_low;
	short	cm_port_high;
	short	cm_port_selected;
	BYTE	cm_pad[64];
};
