//------------------------------------------------------------------------------
// File: H264Stream.cpp
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include "stdafx.h"

#pragma warning(disable:4710)  // 'function': function not inlined (optimzation)

//
// Constructor
//
H264Stream::H264Stream(HRESULT *phr,
                         CMangoCapture *pParent,
                         LPCWSTR pPinName) :
    VideoStream(phr, pParent, pPinName),
	m_pH264ctl_filter(NULL),
	m_pH264enc_filter(NULL)
{
    ASSERT(phr);

//	CAutoLock cAutoLock(&m_cSharedState);
	m_iKeyFrameRate = 100;

} // (Constructor)

//
// Destructor
//
H264Stream::~H264Stream()
{
}

//
// GetMediaType
//
HRESULT H264Stream::GetMediaType(CMediaType* pmt)
{
    CheckPointer(pmt, E_POINTER);

	CAutoLock autoLock(m_pFilter->pStateLock());
    VIDEOINFO *pvi = (VIDEOINFO *) pmt->AllocFormatBuffer(sizeof(VIDEOINFO));
    if(NULL == pvi)
        return(E_OUTOFMEMORY);

    ZeroMemory(pvi, sizeof(VIDEOINFO));

    pvi->bmiHeader.biCompression = MAKEFOURCC('H','2','6','4');
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

    // Work out the GUID for the subtype from the header info.
	static GUID MEDIASUBTYPE_H264 = { MAKEFOURCC('h','2','6','4'), 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71};
    pmt->SetSubtype(&MEDIASUBTYPE_H264);
    pmt->SetSampleSize(MAX_BUFSIZ);
    return NOERROR;

} // GetMediaType

HRESULT STDMETHODCALLTYPE H264Stream::SetValue(const GUID* Api, VARIANT* Value)
{
	if (IsEqualGUID(*Api, ENCAPIPARAM_BITRATE))
	{
		if (Value->intVal != m_iBitRate)
		{
			m_iBitRate = Value->intVal;
			ResetH264();
		}
		return S_OK;
	}
	else if (IsEqualGUID(*Api, CODECAPIPARAM_KEYFRAMERATE))
	{
		if (Value->intVal != m_iKeyFrameRate)
		{
			m_iKeyFrameRate = Value->intVal;
		//	ResetH264(); ??
		}
		return S_OK;
	}
	else return VideoStream::SetValue(Api, Value);
}

void H264Stream::AddFilters()
{
	VideoStream::AddFilters();

	// FilterH264VEnc

	stH264V_h2dparams h264_params;

	h264_params.nNumBufs = 2;
	h264_params.nMaxOutputSize = MAX_BUFSIZ;
	h264_params.eVideoStandard = MX_NTSC;
	h264_params.eImageSize = m_eImagesize;
	h264_params.nBitrate = m_iBitRate * 1024;
	h264_params.nFrameRate = 30 / m_iFrameRate;
	h264_params.nNumGOPFrames = m_iKeyFrameRate;
	h264_params.nQpN = 1;
	h264_params.nQp0 = 1;
	h264_params.nCustomWidth = 0;
	h264_params.nCustomHeight = 0;

	m_pH264enc_filter = new CFilterH264VEnc(h264_params);
	m_iNumOfFilters++;

	// FilterH2D - H.264 control filter (host to DSP)

	m_pH264ctl_filter = new CFilterH2D_PCI(2, MAX_BUFSIZ, MANGOBIOS_FOREVER, Mango::sPCIDevices[m_iDSP]);
	m_iNumOfFilters++;
}

void H264Stream::SetAndAttachPins()
{
	m_pGraph->SetFilter(m_pVidin_filter, 0);
	m_pGraph->SetFilter(m_pFramerate_filter, 1);
	m_pGraph->SetFilter(m_pH264enc_filter, 2);
	m_pGraph->SetFilter(m_pD2H_filter, 3);
	m_pGraph->SetFilter(m_pH264ctl_filter, 4);
	m_pGraph->SetFilter(m_pFramerate2_filter, 5);
	m_pGraph->SetFilter(m_pTimeTag_filter, 6);
	m_pGraph->Attach(0, 1, 0, 0);
	m_pGraph->Attach(1, 2, 0, 0);
	m_pGraph->Attach(2, 3, 0, 0);
	m_pGraph->Attach(4, 2, 0, 1);
	m_pGraph->Attach(0, 5, 1, 0);
	m_pGraph->Attach(5, 6, 0, 0);
}

void H264Stream::ConnectFilters()
{
	MANGOERROR_error_t mangoStatus;
	PCI_STREAM_ptr_t buf;
	int bufsiz;

	if (m_pH264ctl_filter != NULL)
	{
		mangoStatus = m_pH264ctl_filter->Connect();
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			throw MXException("H264 Ctrl Connect", mangoStatus);
		}
	}
	VideoStream::ConnectFilters();
}

void H264Stream::DoFramePostProcessing(IMediaSample* pms)
{
	BYTE *pData;
    long lDataLen;

	pms->GetPointer(&pData);
    lDataLen = pms->GetSize();
	if (pData[4] == 0x67 || pData[4] == 0x68)
	{
		pms->SetSyncPoint(TRUE);
	}
	else
	{
		pms->SetSyncPoint(FALSE);
	}
}

void H264Stream::DeleteFilters()
{
	m_pH264ctl_filter = NULL;
	m_pH264enc_filter = NULL;
	VideoStream::DeleteFilters();
}

void H264Stream::ResetH264()
{
	MANGOERROR_error_t mangoStatus;
	PCI_STREAM_ptr_t buf;
	stH264VEncCmd h264_cmd;

	if (m_pH264ctl_filter == NULL)
	{
		return;
	}
	CAutoLock cAutoLockShared(&m_cSharedState);
	{
		mangoStatus = m_pH264ctl_filter->GetEmptyBuffer(&buf);
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			Mango::showError("GetEmptyBuffer", mangoStatus);
			return;
		}
		h264_cmd.cmd = H264CMD_REOPEN;
		h264_cmd.params.nNumBufs = 2;
		h264_cmd.params.nMaxOutputSize = MAX_BUFSIZ;
		h264_cmd.params.eVideoStandard = MX_NTSC;
		h264_cmd.params.eImageSize = m_eImagesize;
		h264_cmd.params.nBitrate = m_iBitRate * 1024;
		h264_cmd.params.nFrameRate = 30 / m_iFrameRate;
		h264_cmd.params.nNumGOPFrames = m_iKeyFrameRate;
		h264_cmd.params.nQp0 = 1;
		h264_cmd.params.nQpN = 1;
		h264_cmd.params.nCustomWidth = 0;
		h264_cmd.params.nCustomHeight = 0;
		memcpy(buf.local, &h264_cmd, sizeof(stH264VEncCmd));
		mangoStatus = m_pH264ctl_filter->SubmitBuffer(&buf, sizeof(stH264VEncCmd));
		if (mangoStatus != MANGOERROR_SUCCESS)
		{
			Mango::showError("SubmitBuffer", mangoStatus);
		}
	}
}
