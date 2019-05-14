#ifndef _MANGO_HH
#define _MANGO_HH

class Mango {
public:
	static void Init();
	static void Finish();
	static void showError(const char* api, MANGOERROR_error_t mangoStatus);

	static MangoC64Boards_handle_t	sCard;
	static int						sCardNum;
	static h2d_pci_device_t**		sPCIDevices;
	static const char*				sMangoDSPObjectFilename;
	static LONG						sReferenceCount;
};

#endif
