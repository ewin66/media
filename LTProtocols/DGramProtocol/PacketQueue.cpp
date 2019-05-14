// PacketQueue.cpp : Implementation of PacketQueue

#include "StdAfx.h"

PacketQueue::PacketQueue() : m_packetReady(TRUE), m_waitSize(30)
{
	m_thresholdLow = 1;
	m_thresholdHigh = 1;
	m_waitSize = m_thresholdHigh;
}

