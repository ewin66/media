/*
	Structures.h
	This struct represents all the data the server needs to share with a client to create 

	Kevin C. Dixon
	02/17/2010
*/

#pragma once

#include <windows.h>
#include <ccrtp/rtp.h>
#include <streams.h>
#include "FCRTPLibExports.h"

struct FCRTPLIB_API FCRTPEndpoint
{
	BOOL isMulticast;
	BOOL isIPv6;
	unsigned short dataPort;
	unsigned short controlPort;
	char * ipAddress;
};

struct FCRTPLIB_API FCRTPStreamDescription
{
	PayloadType payloadType;
	AM_MEDIA_TYPE dsType;
	FCRTPEndpoint server;
};