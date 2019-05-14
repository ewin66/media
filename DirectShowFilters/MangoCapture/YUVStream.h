#ifndef YUVSTREAM_H
#define YUVSTREAM_H

//------------------------------------------------------------------------------
// File: YUVStream.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

class YUVStream : public VideoStream
{
public:

    YUVStream(HRESULT *phr, CMangoCapture *pParent, LPCWSTR pPinName);
    ~YUVStream();

    HRESULT GetMediaType(CMediaType *pmt);

}; // YUVStream

#endif
