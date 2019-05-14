#if SUPPORT_SRM314

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutureConcepts.Media.DirectShowLib;
using System.Drawing;
using System.Runtime.InteropServices;
using FutureConcepts.Media.Tools;

namespace FutureConcepts.Media.Server.Graphs
{
    /// <summary>
    /// Implements support for Sensoray M314 Frame Grabber
    /// </summary>
    /// <author>kdixon 10/27/2009</author>
    public class SRM314 : StreamingGraph
    {
        private IAMAnalogVideoDecoder _captureDecoder;

        private IBaseFilter _crossbar;
        private IAMCrossbar _crossbarAPI;

        private IBaseFilter _ves;
        private IPin _vesCapturePin;

        private IBaseFilter _frameRateFilter;
        private IFCFrameRateAPI _frameRate;

        private VideoSettings _currentVideoSettings;

        public SRM314(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest)
            : base(sourceConfig, openGraphRequest)
        {
            InitializeNetworkSink();

            _currentVideoSettings = new VideoSettings();

            _crossbar = AddFilterByName(FilterCategory.AMKSCrossbar, "Sensoray 314 Crossbar");
            _captureFilter = AddFilterByName(FilterCategory.AMKSCapture, "Sensoray 314 A/V Capture");

            //instantiate BDA VES and get reference to capture pin
            _ves = AddFilterByName(FilterCategory.WDMStreamingEncoderDevices, "Sensoray 314 BDA MPEG VES Encoder");
            _vesCapturePin = DsFindPin.ByName(_ves, "Capture");

            //instantiate frame rate filter and get control interface
            _frameRateFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "FC Frame Rate Filter");
            _frameRate = (IFCFrameRateAPI)_frameRateFilter;

           

            SetInputAndSystem();

            _frameRate.set_InputFramerate(15.0);

            ChangeProfile(CurrentProfile);

            ConnectFilters(_crossbar, "0: Video Decoder Out", _captureFilter, "Analog Video In", false);
            ConnectFilters(_captureFilter, "Analog ITU Video", _ves, "Analog ITU Video", false);
           // ConnectFilterToNetMux(_ves, "Capture", "Input 01");
            ConnectFilters(_ves, "Capture", _frameRateFilter, "Input", false);
            ConnectFilterToNetMux(_frameRateFilter, "Output", "Input 01");
            ConnectNetMuxToNetSnk();
        }

        public override void ChangeProfile(Profile newProfile)
        {
            if (newProfile.Video == null)
            {
                //TODO support audio
                throw new SourceConfigException("SRM314 only supports Video");
            }
            if (newProfile.Video.CodecType != VideoCodecType.MJPEG)
            {
                throw new SourceConfigException("SRM314 only supports MJPEG codec");
            }

            if ((_currentVideoSettings.CodecType != newProfile.Video.CodecType) ||
                (_currentVideoSettings.ImageSize != newProfile.Video.ImageSize) ||
                (_currentVideoSettings.ConstantBitRate != newProfile.Video.ConstantBitRate) ||
                (_currentVideoSettings.VBR != newProfile.Video.VBR))
            {
                if (this.State == ServerGraphState.Running)
                {
                    throw new ServerGraphRebuildException("A hardware property changed!");
                }

                SRM314HWProxy.SetVideoConfig(Marshal.GetIUnknownForObject(_vesCapturePin),
                                             Marshal.GetIUnknownForObject(_ves),
                                             newProfile.Video);

                //these settings are valid if we passed SetEncoder
                _currentVideoSettings.CodecType = newProfile.Video.CodecType;
                _currentVideoSettings.ImageSize = newProfile.Video.ImageSize;
                _currentVideoSettings.ConstantBitRate = newProfile.Video.ConstantBitRate;
                _currentVideoSettings.VBR = newProfile.Video.VBR;
            }

            //do frame rate adjustment
            if ((_currentVideoSettings.FrameRate != newProfile.Video.FrameRate) ||
                (_currentVideoSettings.FrameRateUnits != newProfile.Video.FrameRateUnits))
            {
                if (newProfile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerMinute)
                {
                    _frameRate.set_TargetFramerate(newProfile.Video.FrameRate / 60.0);
                }
                else if (newProfile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerSecond)
                {
                    _frameRate.set_TargetFramerate(newProfile.Video.FrameRate);
                }
                else
                {
                    throw new UnsupportedFrameRateUnitsException(newProfile.Video.FrameRateUnits);
                }
                _currentVideoSettings.FrameRate = newProfile.Video.FrameRate;
                _currentVideoSettings.FrameRateUnits = newProfile.Video.FrameRateUnits;
            }

            base.ChangeProfile(newProfile);
        }

        private void SetInputAndSystem()
        {
            //TODO how can we support more than 1 card?

            if (SourceConfig.DeviceAddress == null)
            {
                throw new SourceConfigException("You must specify a DeviceAddress!");
            }

            //Route the SourceConfig's selected input
            _crossbarAPI = (IAMCrossbar)_crossbar;
            int hr = _crossbarAPI.Route(0, SourceConfig.DeviceAddress.Input);
            DsError.ThrowExceptionForHR(hr);

            //declare that our input is NTSC
            _captureDecoder = (IAMAnalogVideoDecoder)_captureFilter;
            hr = _captureDecoder.put_TVFormat(AnalogVideoStandard.NTSC_M);  //HACK NTSC is hard coded
            DsError.ThrowExceptionForHR(hr);
        }

        public override void Dispose(bool disposing)
        {
            ReleaseComObject(_crossbarAPI);
            ReleaseComObject(_captureDecoder);
            ReleaseComObject(_frameRate);
            ReleaseComObject(_frameRateFilter);
            ReleaseComObject(_ves);
            ReleaseComObject(_vesCapturePin);
            ReleaseComObject(_crossbar);
            base.Dispose(disposing);
        }
    }
}

#endif