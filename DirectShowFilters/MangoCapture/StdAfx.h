//------------------------------------------------------------------------------
// File: Standard Include
//
// Copyright (c) Future Concepts.  All rights reserved.
//------------------------------------------------------------------------------

#include <streams.h>
#include <dshow.h>
#include <olectl.h>
#include <initguid.h>
#include <mmreg.h>
#include <time.h>

#include <stdio.h>
#include <string>
using namespace std;


#include "MangoXExp.h"
#include "MangoC64BoardsExp.h"
#include "MangoBoards.h"
#include "MangoX_SharedExp.h"
#include "MangoAV_SharedExp.h"
#include "FilterD2H_PCI/FilterD2H_PCI.h"
#include "FilterH2D_PCI/FilterH2D_PCI.h"
#include "FilterAudioIn/FilterAudioIn.h"
#include "FilterVideoIn/FilterVideoIn.h"
#include "FilterH264VEnc/FilterH264VEnc.h"
#include "FilterMp3AEnc/FilterMp3AEnc.h"
#include "FilterMp2AEnc/FilterMp2AEnc.h"
#include "FilterJpegVEnc/FilterJpegVEnc.h"
#include "FilterFramerate/FilterFramerate.h"

#include "Mango.h"

#include "MXException.h"

#include "MangoCapture.h"
#include "MangoStream.h"
#include "VideoStream.h"
#include "H264Stream.H"
#include "YUVStream.h"
#include "PCMStream.h"
#include "MP3Stream.h"

#include "CAVCPropPage.h"

#include "resource.h"

