DEFINE_GUID(IID_IDVRWriterApi, 0xE0FFBD03, 0x1FCA, 0x4dc7, 0x82, 0xC7, 0xD8, 0x59, 0x06, 0x1A, 0x41, 0xF9);
	interface IDVRWriterApi : public IUnknown
	{
		STDMETHOD(StartRecording)() = 0;
		STDMETHOD(StopRecording)() = 0;
		STDMETHOD(get_CurrentChunkSize)(__out int* lSize) = 0;
	};

