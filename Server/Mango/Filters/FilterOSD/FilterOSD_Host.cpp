/******************************************************************************
 * Copyright 2005 Mango DSP, Inc. All rights reserved. Software and information
 * contained herein is confidential and proprietary to Mango DSP, and any
 * unauthorized copying, dissemination or use is strictly prohibited.
 *
 * FILENAME:	FilterOSD.cpp
 *
 * GENERAL DESCRIPTION: OSD filter (host skeleton, does not include all #ifdef DSP sections).
 *
 * DATE CREATED		AUTHOR
 *
 * 14/04/05 		Itay Chamiel
 *
 ******************************************************************************/
#include "FilterOSD.h"

// This function is called by all constructors.
void CFilterOSD::SetDefaults()
{
	// Set number of input and output pins for this filter here.
	m_nNumInPins=2; 
	m_nNumOutPins=1;

	m_pInPins = new CMangoXInPin[m_nNumInPins];
	m_pOutPins = new CMangoXOutPin[m_nNumOutPins];

	// Set the allowed input type and the output type for your pins here.
	m_pInPins[0].SetType(MT_V_PLANAR_420);
	m_pInPins[1].SetType(MT_OSD_CMD);
	m_pOutPins[0].SetType(MT_V_PLANAR_420);

	// This should be the same as the class's name
	strcpy(m_strName, "CFilterOSD");
}

// Default constructor
CFilterOSD::CFilterOSD()
{
	SetDefaults();
}

// Constructor with parameters
CFilterOSD::CFilterOSD(enImageSize eImageSize, enVideoStandard eVideoStandard)
{
	m_eImageSize = eImageSize;
	m_eVideoStandard = eVideoStandard;
	SetDefaults();
}

// Destructor
CFilterOSD::~CFilterOSD()
{
	delete []m_pInPins;
	delete []m_pOutPins;
}

// Serializer. Must convert all parameters to a char buffer.
int CFilterOSD::Serialize(char *pBuff, bool bStore)
{
	char *p = pBuff;
	
	p += CMangoXFilter::Serialize(p, bStore);
	
	if (bStore)
	{
		SERIALIZE(p, m_eImageSize);
		SERIALIZE(p, m_eVideoStandard);
	}
	else
	{
		DESERIALIZE(p, m_eImageSize);
		DESERIALIZE(p, m_eVideoStandard);
	}
	
	return p-pBuff;
}

