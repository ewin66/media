#ifndef _MXEXCEPTION_HH
#define _MXEXCEPTION_HH

class MXException {
public:
	MXException(char* func, MANGOERROR_error_t mangoStatus);

	const char* GetFunc();
	MANGOERROR_error_t GetStatus() const;
	const char* GetStatusString();

	HRESULT ToHRESULT() const;

public:
	//error code for when a mango card is no longer responding
	const static HRESULT MXE_E_MANGODEAD	= 0x8004DEAD;
	const static HRESULT MXE_E_FAIL			= 0x80041708;

private:
	MANGOERROR_error_t m_status;
	string m_func;
	string m_statusString;
};

#endif
