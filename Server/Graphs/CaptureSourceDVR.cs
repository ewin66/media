using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;

using LMH264EncoderLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class CaptureSourceDVR : BaseDVRSinkGraph
    {
        private IBaseFilter _videoEncoder;
        private ILMH264Encoder _videoEncoderControl;
        private IBaseFilter _transcoder;

        public CaptureSourceDVR(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest)
            : base(sourceConfig, openGraphRequest)
        {
            AppLogger.Message("In CaptureSourceDVR constructor - CurrentProfile is " + CurrentProfile.Name);

            _captureFilter = AddFilterByName(FilterCategory.VideoInputDevice, sourceConfig.Description);

            _videoEncoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD H264 Encoder (4.0)");

//            _transcoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD H264 Transcoder");

            InitializeSink();

            ConnectFilters(_captureFilter, "Capture", _videoEncoder, "XForm In");

//            ConnectFilters(_videoEncoder, "XForm Out", _transcoder, "Input");

//            ConnectFilterToMux(_transcoder, "Output", "Input 01");

            ConnectFilterToMux(_videoEncoder, "XForm Out", "Input 01");

            ConnectMuxToSink();
            
            _videoEncoderControl = (ILMH264Encoder)_videoEncoder;
            _videoEncoderControl.EnableRateControl = true;
            _videoEncoderControl.BitRate = CurrentProfile.Video.ConstantBitRate * 1024;
            _videoEncoderControl.OutputFormat = eH264OUTPUTFORMAT.H264FORMAT_IPOD; // seems to work the best

        }

        public override void Dispose(bool disposing)
        {
            _captureFilter = null;
            base.Dispose(disposing);
        }
    }
}
