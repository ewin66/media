#include "FCRTPSession.h"

FCRTPSession::FCRTPSession(InetHostAddress & serverIP, tpport_t port, tpport_t controlPort)
	: RTPSession(serverIP, port, controlPort),
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

bool FCRTPSession::AddDestination(const FCRTPEndpoint * destination)
{
	//TODO support IPv6 and Multicast
	if((!destination->isIPv6) && (!destination->isMulticast))
	{
	/*	return this->addDestination(InetHostAddress(ConvertBSTRToString(destination->ipAddress)),
									destination->dataPort, destination->controlPort);*/
		return this->addDestination(InetHostAddress(destination->ipAddress),
									destination->dataPort, destination->controlPort);
	}
	else
	{
		return false;
	}
}

bool FCRTPSession::RemoveDestination(const FCRTPEndpoint * destination)
{
	//TODO support IPv6 and Multicast
	if((!destination->isIPv6) && (!destination->isMulticast))
	{
		/*return this->forgetDestination(InetHostAddress(ConvertBSTRToString(destination->ipAddress)),
										destination->dataPort, destination->controlPort);*/
		return this->forgetDestination(InetHostAddress(destination->ipAddress),
										destination->dataPort, destination->controlPort);
	}
	else
	{
		return false;
	}
}