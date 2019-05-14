//------------------------------------------------------------------------------
// File: YUVStream.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

#pragma warning(disable:4710)  // 'function': function not inlined (optimzation)

//
// Constructor
//
YUVStream::YUVStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName) :
    VideoStream(phr, pParent, pPinName)
{
} // (Constructor)

//
// Destructor
//
YUVStream::~YUVStream()
{
}

//
// GetMediaType
//
HRESULT YUVStream::GetMediaType(CMediaType* pmt)
{
    CheckPointer(pmt, E_POINTER);

    CAutoLock cAutoLock(m_pFilter->pStateLock());

    VIDEOINFO *pvi = (VIDEOINFO *) pmt->AllocFormatBuffer(sizeof(VIDEOINFO));
    if(NULL == pvi)
        return(E_OUTOFMEMORY);

    ZeroMemory(pvi, sizeof(VIDEOINFO));

    pvi->bmiHeader.biCompression = MAKEFOURCC('I','4','2','0');
    pvi->bmiHeader.biBitCount    = 24;

    // (Adjust the parameters common to all formats...)

    pvi->bmiHeader.biSize       = sizeof(BITMAPINFOHEADER);
    pvi->bmiHeader.biWidth      = m_iImageWidth;
    pvi->bmiHeader.biHeight     = m_iImageHeight;
    pvi->bmiHeader.biPlanes     = 1;
    pvi->bmiHeader.biSizeImage  = GetBitmapSize(&pvi->bmiHeader);
    pvi->bmiHeader.biClrImportant = 0;

    SetRectEmpty(&(pvi->rcSource)); // we want the whole image area rendered.
    SetRectEmpty(&(pvi->rcTarget)); // no particular destination rectangle
    pmt->SetType(&MEDIATYPE_Video);
    pmt->SetFormatType(&FORMAT_VideoInfo);
    pmt->SetTemporalCompression(FALSE);

    pmt->SetSubtype(&MEDIASUBTYPE_IYUV);
    pmt->SetSampleSize(MAX_BUFSIZ);
    return NOERROR;

} // GetMediaType
