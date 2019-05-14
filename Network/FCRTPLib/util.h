#pragma once
/*
	util.h
	Collection of utility functions that make life better!

	Kevin C. Dixon
	Future Concepts
	03/24/2009
*/

#include <windows.h>

/*
	Convert from BSTR to char*
	pSrc	[in]	BSTR to convert
	returns 
*/
static inline char * ConvertBSTRToString(BSTR pSrc)
{
    if(!pSrc) return NULL;

    //convert even embeded NULL

    DWORD cb,cwch = SysStringLen(pSrc);

    char *szOut = NULL;

    if(cb = WideCharToMultiByte(CP_ACP, 0, pSrc, cwch + 1, NULL, 0, 0, 0))
    {
        szOut = new char[cb];
        if(szOut)
        {
            szOut[cb - 1]  = '\0';

            if(!WideCharToMultiByte(CP_ACP, 0, pSrc, cwch + 1, szOut, cb, 0, 0))
            {
                delete []szOut;//clean up if failed;

                szOut = NULL;
            }
        }
    }

    return szOut;
}