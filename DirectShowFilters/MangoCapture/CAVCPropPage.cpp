#include <windows.h>
#include <streams.h>
#include <stdio.h>
#include <commctrl.h>
#include "CAVCPropPage.h"

CAVCPropPage::CAVCPropPage(IUnknown* pUnk) : CBasePropertyPage(NAME("AVCProp"), pUnk, IDD_OLE_PROPPAGE_SMALL, IDS_STRING102)
{
	m_bDirty = FALSE;
	m_pCodecAPI = NULL;
}

CUnknown * WINAPI CAVCPropPage::CreateInstance(LPUNKNOWN lpunk, HRESULT *phr)
{
	return new CAVCPropPage(lpunk);
}

HRESULT
CAVCPropPage::OnConnect(IUnknown* pUnk)
{
	HRESULT result;
	if (pUnk == NULL)
    {
        return E_POINTER;
    }
    ASSERT(m_pCodecAPI == NULL);
    result = pUnk->QueryInterface(IID_ICodecAPI, reinterpret_cast<void**>(&m_pCodecAPI));
	return result;
}

HRESULT
CAVCPropPage::OnActivate(void)
{
	LRESULT result;
    INITCOMMONCONTROLSEX icc;
    icc.dwSize = sizeof(INITCOMMONCONTROLSEX);
    icc.dwICC = ICC_BAR_CLASSES;
    if (InitCommonControlsEx(&icc) == FALSE)
    {
        return E_FAIL;
    }

	/* Initialize BitRate COMBOBOX */

	result = SendDlgItemMessageA(m_Dlg, IDC_BITRATE, CB_ADDSTRING, 0, (LPARAM)"64");
	result = SendDlgItemMessageA(m_Dlg, IDC_BITRATE, CB_ADDSTRING, 0, (LPARAM)"128");
	result = SendDlgItemMessageA(m_Dlg, IDC_BITRATE, CB_ADDSTRING, 0, (LPARAM)"256");
	m_iBitRate = SetCurrentCBItem(IDC_BITRATE, result);
	result = SendDlgItemMessageA(m_Dlg, IDC_BITRATE, CB_ADDSTRING, 0, (LPARAM)"512");
	result = SendDlgItemMessageA(m_Dlg, IDC_BITRATE, CB_ADDSTRING, 0, (LPARAM)"768");
	result = SendDlgItemMessageA(m_Dlg, IDC_BITRATE, CB_ADDSTRING, 0, (LPARAM)"1024");
	result = SendDlgItemMessageA(m_Dlg, IDC_BITRATE, CB_ADDSTRING, 0, (LPARAM)"1500");

	/* Initialize FrameRate COMBOBOX */

	result = SendDlgItemMessageA(m_Dlg, IDC_FRAMERATE, CB_ADDSTRING, 0, (LPARAM)"1");
	result = SendDlgItemMessageA(m_Dlg, IDC_FRAMERATE, CB_ADDSTRING, 0, (LPARAM)"3");
	result = SendDlgItemMessageA(m_Dlg, IDC_FRAMERATE, CB_ADDSTRING, 0, (LPARAM)"5");
	result = SendDlgItemMessageA(m_Dlg, IDC_FRAMERATE, CB_ADDSTRING, 0, (LPARAM)"6");
	result = SendDlgItemMessageA(m_Dlg, IDC_FRAMERATE, CB_ADDSTRING, 0, (LPARAM)"10");
	result = SendDlgItemMessageA(m_Dlg, IDC_FRAMERATE, CB_ADDSTRING, 0, (LPARAM)"15");
	m_iFrameRate = SetCurrentCBItem(IDC_FRAMERATE, result);
	result = SendDlgItemMessageA(m_Dlg, IDC_FRAMERATE, CB_ADDSTRING, 0, (LPARAM)"30");

	/* Initialize KeyFrameKey COMBOBOX */

	result = SendDlgItemMessageA(m_Dlg, IDC_KEYFRAMERATE, CB_ADDSTRING, 0, (LPARAM)"1");
	result = SendDlgItemMessageA(m_Dlg, IDC_KEYFRAMERATE, CB_ADDSTRING, 0, (LPARAM)"10");
	result = SendDlgItemMessageA(m_Dlg, IDC_KEYFRAMERATE, CB_ADDSTRING, 0, (LPARAM)"30");
	m_iKeyFrameRate = SetCurrentCBItem(IDC_KEYFRAMERATE, result);
	result = SendDlgItemMessageA(m_Dlg, IDC_KEYFRAMERATE, CB_ADDSTRING, 0, (LPARAM)"50");
	result = SendDlgItemMessageA(m_Dlg, IDC_KEYFRAMERATE, CB_ADDSTRING, 0, (LPARAM)"100");

	return S_OK;
}

INT_PTR
CAVCPropPage::OnReceiveMessage(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
   switch (uMsg)
    {
    case WM_COMMAND:
        if (LOWORD(wParam) == IDC_BITRATE)
        {
			if (HIWORD(wParam) == CBN_SELCHANGE)
			{
				m_iBitRate = GetCurrentCBValue(IDC_BITRATE);
	            SetDirty();
		        return (LRESULT) 1;
			}
        }
		else if (LOWORD(wParam) == IDC_FRAMERATE)
		{
			if (HIWORD(wParam) == CBN_SELCHANGE)
			{
				m_iFrameRate = GetCurrentCBValue(IDC_FRAMERATE);
	            SetDirty();
		        return (LRESULT) 1;
			}
		}
		else if (LOWORD(wParam) == IDC_KEYFRAMERATE)
		{
			if (HIWORD(wParam) == CBN_SELCHANGE)
			{
				m_iFrameRate = GetCurrentCBValue(IDC_KEYFRAMERATE);
	            SetDirty();
		        return (LRESULT) 1;
			}
		}
        break;
	default:
		break;
    } // Switch.
    
    // Let the parent class handle the message.
    return CBasePropertyPage::OnReceiveMessage(hwnd,uMsg,wParam,lParam);
}


HRESULT
CAVCPropPage::OnApplyChanges(void)
{
	return S_OK;
}

HRESULT
CAVCPropPage::OnDisconnect(void)
{
	HRESULT hr;

	if (m_pCodecAPI)
    {
		if (m_bDirty)
		{
			VARIANT v;
			v.vt = VT_I4;
			v.intVal = m_iBitRate;
			hr = m_pCodecAPI->SetValue(&ENCAPIPARAM_BITRATE, &v);
			if (hr != S_OK)
			{
				return hr;
			}
			v.vt = VT_I4;
			v.intVal = m_iFrameRate;
			hr = m_pCodecAPI->SetValue(&CODECAPIPARAM_FRAMERATE, &v);
			if (hr != S_OK)
			{
				return hr;
			}
			v.vt = VT_I4;
			v.intVal = m_iKeyFrameRate;
			hr = m_pCodecAPI->SetValue(&CODECAPIPARAM_KEYFRAMERATE, &v);
			if (hr != S_OK)
			{
				return hr;
			}
		}
        // If the user clicked OK, m_lVal holds the new value.
        // Otherwise, if the user clicked Cancel, m_lVal is the old value.
        m_pCodecAPI->Release();
        m_pCodecAPI = NULL;
    }
	return S_OK;
}

int
CAVCPropPage::GetCurrentCBValue(int cb)
{
	char buf[64];
    int cursel = SendDlgItemMessageA(m_Dlg, cb, CB_GETCURSEL, 0, 0);
	SendDlgItemMessageA(m_Dlg, cb, CB_GETLBTEXT, cursel, (LPARAM)buf);
	return atoi(buf);
}

int
CAVCPropPage::SetCurrentCBItem(int cb, int index)
{
	SendDlgItemMessageA(m_Dlg, cb, CB_SETCURSEL, index, 0);
	return GetCurrentCBValue(cb);
}

