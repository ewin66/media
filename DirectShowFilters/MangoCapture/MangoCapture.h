#ifndef MANGO_CAPTURE_H
#define MANGO_CAPTURE_H

//------------------------------------------------------------------------------
// File: FMangoCapture.h
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

// {B7E2778A-C635-4494-B7E1-B1D28F7F0022}
DEFINE_GUID(CLSID_MangoCapture, 
0xb7e2778a, 0xc635, 0x4494, 0xb7, 0xe1, 0xb1, 0xd2, 0x8f, 0x7f, 0x0, 0x22);

//------------------------------------------------------------------------------
// Class CMangoCapture
//
// This is the main class for the MangoCapture filter. It inherits from
// CSource, the DirectShow base class for source filters.
//------------------------------------------------------------------------------
class CMangoCapture : public CSource
{
public:

	DECLARE_IUNKNOWN;

    // The only allowed way to create Bouncing balls!
    static CUnknown * WINAPI CreateInstance(LPUNKNOWN lpunk, HRESULT *phr);

private:

    // It is only allowed to to create these objects with CreateInstance
    CMangoCapture(LPUNKNOWN lpunk, HRESULT *phr);

private:
}; // CMangoCapture

#endif
