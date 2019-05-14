/* Copyright 2005, Future Concepts, La Verne, CA. */

#include "stdafx.h"

/*
	Instantiates an MXException
*/
MXException::MXException(char* func, MANGOERROR_error_t mangoStatus)
{
	m_func = func;
	m_status = mangoStatus;

	switch (mangoStatus) {
		case MANGOERROR_SUCCESS:
			m_statusString = "Success";
			break;
		case MANGOERROR_FAILURE:
			m_statusString = "Failure";
			break;
		case MANGOERROR_TIMEOUT:
			m_statusString = "Timeout";
			break;
		case MANGOERROR_ERR_INVALID_HANDLE:
			m_statusString = "Invalid handle";
			break;
		case MANGOERROR_ERR_NOT_IMPLEMENTED:
			m_statusString = "Not implemented";
			break;
		case MANGOERROR_COFF_FORMAT_ERROR:
			m_statusString = "COFF format error";
			break;
		case MANGOERROR_ERR_INVALID_PARAMETER:
			m_statusString = "Invalid parameter";
			break;
		case MANGOERROR_INSUFFICIENT_RESOURCES:
			m_statusString = "Insufficient resources";
			break;
		case MANGOERROR_INVALID_CONFIGURATION:
			m_statusString = "Invalid configuration";
			break;
		case MANGOERROR_RESOURCE_NOT_READY:
			m_statusString = "Resource not ready";
			break;
		default:
			m_statusString = "unknown MX error";
			break;
	}
}

/*
	Retrieves the associated MANGOERROR
*/
MANGOERROR_error_t MXException::GetStatus() const
{
	return m_status;
}

/*
	Retrieves the method name or action where the error occurred
*/
const char* MXException::GetFunc()
{
	return m_func.c_str();
}

/*
	Fetches the textual description of the MANGOERROR
*/
const char* MXException::GetStatusString()
{
	return m_statusString.c_str();
}

HRESULT MXException::ToHRESULT() const
{
	switch (m_status)
	{
		case (MANGOERROR_SUCCESS - 1):
			return MXException::MXE_E_MANGODEAD;
		case MANGOERROR_TIMEOUT:
			return HRESULT_FROM_WIN32(ERROR_NOT_READY);
		default:
			return MXException::MXE_E_FAIL;
	}
}
