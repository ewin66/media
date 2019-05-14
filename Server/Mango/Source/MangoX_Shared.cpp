/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	MangoX_Shared.cpp
 *
 * GENERAL DESCRIPTION: MangoX Host/DSP shared classes.
 *
 * DATE CREATED		AUTHOR
 *
 * 28/02/05 		Yishay Hayardeni
 *
 ******************************************************************************/
#include "MangoX_SharedExp.h"

int CMangoXPin::Serialize(char *pBuff, bool bStore)
{
	if (bStore)
	{
		SERIALIZE(pBuff, m_Type);
	}
	else
	{
		DESERIALIZE(pBuff, m_Type);
	}
	return sizeof(m_Type);
}

int CMangoXInPin::Serialize(char *pBuff, bool bStore)
{
	char *pTempBuff=pBuff;
	pTempBuff+=CMangoXPin::Serialize(pBuff, bStore);

	if (bStore)
	{
		SERIALIZE(pTempBuff, m_nSourceFilterId);
	}
	else
	{
		DESERIALIZE(pTempBuff, m_nSourceFilterId);
	}
	
	pTempBuff+=sizeof(m_nSourceFilterId);
	return pTempBuff-pBuff;
}

bool CMangoXInPin::Attach(CMangoXOutPin *pSourcePin, int nSourceFilterId)
{
	if (pSourcePin->GetType() == MT_ANY || m_Type == MT_ANY || pSourcePin->GetType()==m_Type)
	{
		m_nSourceFilterId = nSourceFilterId;
		return true;
	}
	return false;
}

int CMangoXOutPin::Serialize(char *pBuff, bool bStore)
{
	char *pTempBuff=pBuff;
	pTempBuff+=CMangoXPin::Serialize(pBuff, bStore);
	
	if (bStore)
	{
		SERIALIZE(pTempBuff, m_nDestFilterId);
		SERIALIZE(pTempBuff, m_nDestPinId);
	}
	else
	{
		DESERIALIZE(pTempBuff, m_nDestFilterId);
		DESERIALIZE(pTempBuff, m_nDestPinId);
	}
	
	pTempBuff+=sizeof(m_nDestFilterId);
	return pTempBuff-pBuff;
}

CMangoXInPin *CMangoXFilter::GetInPin(int nInPinIndex)
{
	if (nInPinIndex<m_nNumInPins) 
		return &(m_pInPins[nInPinIndex]);
	return NULL;
}

CMangoXOutPin *CMangoXFilter::GetOutPin(int nOutPinIndex)
{
	if (nOutPinIndex<m_nNumOutPins) 
		return &(m_pOutPins[nOutPinIndex]);
	return NULL;
}

bool CMangoXFilter::Attach(CMangoXOutPin *pSourcePin, int nSourceFiltIndex, int nDestFiltIndex, int nDestPinIndex)
{
	if (nDestPinIndex>=m_nNumInPins || !pSourcePin)
		return false;
	return (m_pInPins[nDestPinIndex].Attach(pSourcePin, nSourceFiltIndex));
}	

int CMangoXFilter::Serialize(char *pBuff, bool bStore)
{
	char *pTempBuff;
	pTempBuff = pBuff;

	if (bStore)
	{
		SERIALIZE(pTempBuff, m_nNumInPins);
		SERIALIZE(pTempBuff, m_nNumOutPins);
	}
	else
	{
		DESERIALIZE(pTempBuff, m_nNumInPins);
		DESERIALIZE(pTempBuff, m_nNumOutPins);
	}
	for (int nInPinIndex=0; nInPinIndex<m_nNumInPins; nInPinIndex++)
		pTempBuff+=m_pInPins[nInPinIndex].Serialize(pTempBuff, bStore);
	for (int nOutPinIndex=0; nOutPinIndex<m_nNumOutPins; nOutPinIndex++)
		pTempBuff+=m_pOutPins[nOutPinIndex].Serialize(pTempBuff, bStore);
	return pTempBuff-pBuff;
}

#ifdef DSP
void CMangoXFilter::Stop()
{
	if (m_hTask)
	{
		TSK_delete(m_hTask);
		m_hTask = NULL;
	}
}
#endif

bool CMangoXGraph::Attach(int nSrcFiltIndex, int nDstFiltIndex, int nSrcPinIndex, int nDstPinIndex)
{
	// Test index validity and prevent loop inside a filter
	if (nSrcFiltIndex >= m_nNumFilters || nDstFiltIndex>= m_nNumFilters || nSrcFiltIndex==nDstFiltIndex)
		return false;

	CMangoXOutPin *pOutPin = m_pFilters[nSrcFiltIndex]->GetOutPin(nSrcPinIndex);
	pOutPin->SetDestFilterId(nDstFiltIndex);
	pOutPin->SetDestPinId(nDstPinIndex);
	return (m_pFilters[nDstFiltIndex]->Attach(pOutPin, nSrcFiltIndex, nDstFiltIndex, nDstPinIndex));
}

int CMangoXGraph::Serialize(char *pBuff, bool bStore)
{
	char *pTempBuff;
	pTempBuff = pBuff;

	if (bStore)
	{
		SERIALIZE(pTempBuff, m_nNumFilters);
		for (int nFiltIndex=0; nFiltIndex<m_nNumFilters; nFiltIndex++)
		{
			strncpy(pTempBuff, m_pFilters[nFiltIndex]->GetName(), MAX_FILTER_NAME);
			pTempBuff+=MAX_FILTER_NAME;
			pTempBuff+=m_pFilters[nFiltIndex]->Serialize(pTempBuff, bStore);
		}
	}
	else
	{
		DESERIALIZE(pTempBuff, m_nNumFilters);		
		if (m_pFilters)
			delete m_pFilters;
		m_pFilters = (CMangoXFilter**) new CMangoXFilter*[m_nNumFilters];
		for (int nFiltIndex=0; nFiltIndex<m_nNumFilters; nFiltIndex++)
		{
			// at this point, pTempBuff points to textual name of filter class
			if (m_pfnConstructFunc)
			{
				m_pFilters[nFiltIndex] = m_pfnConstructFunc(pTempBuff);
				if (!m_pFilters[nFiltIndex])
				{
					delete m_pFilters;
					return 0;
				}
			}
			else
			{
				delete m_pFilters;
				return 0;
			}
			pTempBuff+=MAX_FILTER_NAME;
			pTempBuff+=m_pFilters[nFiltIndex]->Serialize(pTempBuff, bStore);
		}
	}

	return pTempBuff-pBuff;
}

CMangoXGraph::CMangoXGraph()
{
	m_nNumFilters = 0;
	m_pFilters = NULL;
}

CMangoXGraph::CMangoXGraph(int nNumFilters)
{
	m_nNumFilters = nNumFilters;
	m_pFilters = (CMangoXFilter**) new CMangoXFilter*[m_nNumFilters];
	for (int nFilterIndex=0; nFilterIndex<m_nNumFilters; nFilterIndex++)
		m_pFilters[nFilterIndex] = NULL;
}

CMangoXGraph::~CMangoXGraph()
{
#ifdef DSP
	for (int nFilterIndex=0; nFilterIndex<m_nNumFilters; nFilterIndex++)
		if (m_pFilters[nFilterIndex])
			m_pFilters[nFilterIndex]->Stop();
#endif
		
	for (int nFilterIndex=0; nFilterIndex<m_nNumFilters; nFilterIndex++)
		if (m_pFilters[nFilterIndex])
			delete m_pFilters[nFilterIndex];
	
	delete m_pFilters;
}

void CMangoXGraph::SetConstructFunc(cbConstructFunc *pConstructFunc)
{
	m_pfnConstructFunc = pConstructFunc;
}

bool CMangoXGraph::SetFilter(CMangoXFilter *pFilter, int nFilterIndex)
{
	if (nFilterIndex<m_nNumFilters && m_pFilters[nFilterIndex]==NULL)
	{
		m_pFilters[nFilterIndex] = pFilter;
		return true;
	}
	return false;
}

CMangoXFilter *CMangoXGraph::GetFilter(int nFilterIndex)
{
	if (nFilterIndex<m_nNumFilters)
		return m_pFilters[nFilterIndex];
	else
		return NULL;
}

#ifdef DSP
bool CMangoXInPin::Connect(CMangoXOutPin *pSourcePin)
{
	m_pSourcePin = pSourcePin;
	m_bConnected = true;
	pSourcePin->SetConnected(true);

	return true;
}

bool CMangoXFilter::Connect(CMangoXOutPin *pSourcePin)
{
	return m_pInPins[pSourcePin->GetDestPinId()].Connect(pSourcePin);
}

bool CMangoXGraph::Create()
{
	// Connect all filters
	for (int nFilterIndex=0; nFilterIndex<m_nNumFilters; nFilterIndex++)
		for (int nPinIndex=0; nPinIndex<m_pFilters[nFilterIndex]->GetOutPinsCount(); nPinIndex++)
		{
			CMangoXOutPin *pOutPin = m_pFilters[nFilterIndex]->GetOutPin(nPinIndex);
			if (pOutPin->GetDestFilterId() != -1)
			{
				if (!m_pFilters[pOutPin->GetDestFilterId()]->Connect(pOutPin))
					return false;
			}
			else
			{
				if (pOutPin->IsRequired())
					return false;
			}
		}

	// Call Init() for all filters
	for (int nFilterIndex=0; nFilterIndex<m_nNumFilters; nFilterIndex++)
		if (!m_pFilters[nFilterIndex]->IsInited() && !m_pFilters[nFilterIndex]->Init())
			return false;

	return true;
}

bool CMangoXGraph::Start()
{
	for (int nFilterIndex=0; nFilterIndex<m_nNumFilters; nFilterIndex++)
		if (!m_pFilters[nFilterIndex]->Start())
			return false;
	return true;
}

#endif

char * CMangoXFilter::GetName()
{
	return m_strName;
}
