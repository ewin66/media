#include "FCRTPSession.h"

FCRTPSession::FCRTPSession(InetHostAddress & serverIP, tpport_t port)
	: RTPSession(serverIP, port),
	  formatter(NULL)
{
	setSchedulingTimeout(10000);	//10 ms
	setExpireTimeout(1000000);		//1 second
}

FCRTPSession::~FCRTPSession()
{
	if(formatter)
	{
		delete formatter;
	}
}

bool FCRTPSession::Start()
{
	try
	{
		this->startRunning();
		
		return this->isActive();
	}
	catch(...)
	{
		return false;
	}
}

void FCRTPSession::SetPayloadFormat(PayloadFormatter * customPayload)
{
	if(formatter)
	{
		delete formatter;
	}

	formatter = customPayload;

	if(formatter)
	{
		this->setPayloadFormat(*(formatter->GetPayloadFormat()));
	}
}

bool FCRTPSession::GetPayloadType(PayloadType *outPayloadType)
{
	if((formatter) && (outPayloadType))
	{
		(*outPayloadType) = formatter->GetPayloadFormat()->getPayloadType();
		return true;
	}
	return false;
}