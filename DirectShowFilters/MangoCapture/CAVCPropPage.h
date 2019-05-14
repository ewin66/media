#if 0
#include "amfilter.h"
#include "resource.h"
#include "IAVCConfig.h"
#endif

#ifndef __AVCPROPPAGE_H
#define __AVCPROPPAGE_H

#include "StdAfx.h"

class CAVCPropPage : public CBasePropertyPage
{
public:
	CAVCPropPage(IUnknown* pUnk);
	HRESULT OnConnect(IUnknown* pUnk);
	HRESULT OnActivate(void);
	INT_PTR OnReceiveMessage(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
	HRESULT OnApplyChanges(void);
	HRESULT OnDisconnect(void);
	static CUnknown * WINAPI CreateInstance(LPUNKNOWN lpunk, HRESULT *phr);
private:
    void SetDirty()
    {
        m_bDirty = TRUE;
        if (m_pPageSite)
        {
            m_pPageSite->OnStatusChange(PROPPAGESTATUS_DIRTY);
        }
    }
	int GetCurrentCBValue(int);
	int SetCurrentCBItem(int, int);

private:
	ICodecAPI*			m_pCodecAPI;
	int					m_iBitRate;
	int					m_iFrameRate;
	int					m_iKeyFrameRate;
	BOOL				m_bDirty;
};

#endif
