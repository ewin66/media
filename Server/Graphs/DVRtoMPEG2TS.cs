using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;

using LMDVRSourceLib;
using LMMpgDmxTLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class DVRtoMPEG2TS : BaseMPEG2TSGraph
    {
        private IBaseFilter _demux;
        private ILMMpgDmx _demuxControl;
        private IBaseFilter _videoDecoder;
        private IBaseFilter _videoEncoder;
        private IBaseFilter _audioEncoder;
        private VideoSettings _currentVideoSettings;
        private IBaseFilter _transcoder;

        public DVRtoMPEG2TS(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            AppLogger.Message("In DVRtoMPEG2TS - CurrentProfile is " + CurrentProfile.Name);

            InitializeSink();

            _captureFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD DVR Source");
            IFileSourceFilter fileSource = (IFileSourceFilter)_captureFilter;
            String[] sourceNameParts = sourceConfig.SourceName.Split('-');
            DVRSettings dvrSettings = DVRSettings.LoadFromFile();
            int hr = fileSource.Load(dvrSettings.RootFolder + sourceNameParts[0] + @"/Stream.LBL", null);
            DsError.ThrowExceptionForHR(hr);
            ILMDVRSource sourceControl = (ILMDVRSource)_captureFilter;
            sourceControl.ResetToDefaultsEx(LMDVRSource_APILEVEL.LMDVRSource_APILEVEL_1);

            _demux = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD MPEG2 Transport Demultiplexer");
            _demuxControl = (ILMMpgDmx)_demux;
            _demuxControl.AutoLive = true;
            _demuxControl.AutoLiveTolerance = .5;

//            _currentVideoSettings = new VideoSettings();
//            _currentVideoSettings.CodecType = CurrentProfile.Video.CodecType;

            ConnectFilters(_captureFilter, "Output", _demux, "Input 01");

            if (CurrentProfile.Video != null)
            {
                VideoSettings settings = CurrentProfile.Video;
                _videoDecoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD H264 Decoder (3.0)");
                _videoEncoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD H264 Encoder (4.0)");
                ConnectFilters(_demux, "H.264 Video", _videoDecoder, "XForm In");
                ConnectFilters(_videoDecoder, "XForm Out", _videoEncoder, "XForm In");
                LMH264EncoderLib.ILMH264Encoder encoderConfig = (LMH264EncoderLib.ILMH264Encoder)_videoEncoder;
                encoderConfig.EnableRateControl = true;
                encoderConfig.BitRate = settings.ConstantBitRate * 1024;
                ConnectFilterToMux(_videoEncoder, "XForm Out", "Input 01");
            }
            else
            {
                ConnectFilterToMux(_demux, "H.264 Video", "Input 01");
            }
            ConnectMuxToSink();
        }

        public override void Dispose(bool disposing)
        {
            if (_demux != null)
            {
                Marshal.ReleaseComObject(_demux);
                _demux = null;
            }
            _captureFilter = null;
            base.Dispose(disposing);
        }
    }
}
