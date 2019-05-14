
NetProtocolps.dll: dlldata.obj NetProtocol_p.obj NetProtocol_i.obj
	link /dll /out:NetProtocolps.dll /def:NetProtocolps.def /entry:DllMain dlldata.obj NetProtocol_p.obj NetProtocol_i.obj \
		kernel32.lib rpcndr.lib rpcns4.lib rpcrt4.lib oleaut32.lib uuid.lib \

.c.obj:
	cl /c /Ox /DWIN32 /D_WIN32_WINNT=0x0400 /DREGISTER_PROXY_DLL \
		$<

clean:
	@del NetProtocolps.dll
	@del NetProtocolps.lib
	@del NetProtocolps.exp
	@del dlldata.obj
	@del NetProtocol_p.obj
	@del NetProtocol_i.obj
