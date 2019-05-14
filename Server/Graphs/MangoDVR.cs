using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.Graphs
{
    public class MangoDVR : BaseDVRSinkGraph
    {
        private IBaseFilter _audioEncoder;
        private VideoSettings _currentVideoSettings;
        private IBaseFilter _transcoder;


        /// <summary>
        /// CodecAPI parameters for use with the MangoCapture
        /// </summary>
        /// <author>darnold</author>
        internal static class CodecAPIParam
        {
            public static readonly Guid ImageSize = new Guid(0x8a30671f, 0x1428, 0x49d9, 0x87, 0x11, 0x95, 0x8f, 0xd0, 0xdb, 0xb5, 0x15);
            public static readonly Guid FrameRate = new Guid(0x577c8a55, 0xffc7, 0x4705, 0xa5, 0x46, 0xc6, 0x1a, 0x4b, 0xc0, 0x93, 0xbd);
            public static readonly Guid KeyFrameRate = new Guid(0xbb6ccfd1, 0x1ecc, 0x4090, 0xa0, 0x80, 0x5c, 0xa9, 0xa4, 0x9f, 0xd6, 0xd0);
            public static readonly Guid Reset = new Guid(0xcd5c00c3, 0x57a2, 0x4fc0, 0x81, 0x58, 0x63, 0x5d, 0x79, 0x4d, 0x6, 0x2c);
            public static readonly Guid DSP = new Guid(0x321a884b, 0x0d97, 0x49f3, 0x81, 0x6b, 0x5a, 0xd7, 0x26, 0x3e, 0x04, 0x4a);
            public static readonly Guid Cam = new Guid(0x6394a130, 0xcd8e, 0x481e, 0xaa, 0xc7, 0x0e, 0x58, 0x17, 0x22, 0x83, 0x9c);
            public static readonly Guid Mic = new Guid(0x403e6a13, 0x3079, 0x4ca9, 0xba, 0xfb, 0x9a, 0x0c, 0x34, 0xa7, 0xb6, 0xe4);
            public static readonly Guid AudioBitRate = new Guid(0x6dbbd171, 0xc8b0, 0x4fee, 0xb7, 0xcb, 0x8c, 0xf2, 0xe3, 0x3f, 0x77, 0x22);
        }

        public MangoDVR(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            InitializeSink();

//            _transcoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD H264 Transcoder");

            _captureFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "Mango Capture");

            _currentVideoSettings = new VideoSettings();
            _currentVideoSettings.CodecType = CurrentProfile.Video.CodecType;

            SetDSP_and_Camera(sourceConfig);
            ChangeProfile(CurrentProfile);

//            ConnectFilters(_captureFilter, "H264", _transcoder, "Input");
//            ConnectFilterToMux(_transcoder, "Output", "Input 01");
            ConnectFilterToMux(_captureFilter, "H264", "Input 01");
            ConnectMuxToSink();
        }

        public override void Dispose(bool disposing)
        {
            _captureFilter = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles MXExceptions that occur on Run.
        /// </summary>
        public override void Run()
        {
            try
            {
                base.Run();
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == DsResults.MXE_E_MangoDead)
                {
                    ErrorLogger.WriteToEventLog("MX CardEnable Failure Detected! HRESULT " + ex.ErrorCode.ToString("X"), EventLogEntryType.Error);
                    MediaServer.ServerNeedsPowerCycle = true;
                }
                throw ex;
            }
        }

        /// <summary>
        /// Gets the ICodecAPI interface for whichever pin is in use for the current codec
        /// </summary>
        /// <returns>a reference to the ICodecAPI on the current pin</returns>
        private ICodecAPI GetCodecAPI()
        {
            string pinName;
            if (_currentVideoSettings.CodecType == VideoCodecType.H264)
            {
                pinName = "H264";
            }
            else
            {
                throw new NotImplementedException("GetCodecAPI not implemented for codectype " + _currentVideoSettings.CodecType.ToString());
            }
            IPin pin = DsFindPin.ByName(_captureFilter, pinName);
            if (pin == null)
            {
                throw new Exception(pinName + " pin not found on MangoCapture filter.");
            }
            ICodecAPI codecAPI = (ICodecAPI)pin;
            if (codecAPI == null)
            {
                throw new Exception("ICodecAPI interface not found on pin " + pinName);
            }
            return codecAPI;
        }
        
        protected void SetDSP_and_Camera(StreamSourceInfo sourceConfig)
        {
            int hr;

            IntPtr nativeVariant = Marshal.AllocCoTaskMem(64);

            ICodecAPI codecAPI_H264 = GetCodecAPI();

            if (sourceConfig.DeviceAddress == null)
            {
                throw new SourceConfigException("DeviceAddress may not be null!");
            }

            AppLogger.Message(String.Format("MangoDVR SetDSP_and_Camera Channel={0} Input={1}", sourceConfig.DeviceAddress.Channel, sourceConfig.DeviceAddress.Input));
            Marshal.GetNativeVariantForObject(sourceConfig.DeviceAddress.Channel, nativeVariant);
            hr = codecAPI_H264.SetValue(CodecAPIParam.DSP, nativeVariant);
            DsError.ThrowExceptionForHR(hr);

            Marshal.GetNativeVariantForObject(sourceConfig.DeviceAddress.Input, nativeVariant);
            hr = codecAPI_H264.SetValue(CodecAPIParam.Cam, nativeVariant);
            DsError.ThrowExceptionForHR(hr);

            hr = Marshal.ReleaseComObject(codecAPI_H264);
            DsError.ThrowExceptionForHR(hr);

            Marshal.FreeCoTaskMem(nativeVariant);
        }

        protected void SetMPEG2Parameters()
        {
#if COMMENT
            int hr;
            MPEG2AudioConfig config = SourceConfig.MPEG2Audio;

            if (Profile == null)
            {
                AppLogger.Message("SetMPEG2Parameters called, but Profile is not set");
                return;
            }

            IntPtr nativeVariant = Marshal.AllocCoTaskMem(64);

            ICodecAPI codecAPI_MP3 = GetCodecAPI_MP3();

            Marshal.GetNativeVariantForObject(config.DSP, nativeVariant);
            hr = codecAPI_MP3.SetValue(CodecAPIParam.DSP, nativeVariant);
            DsError.ThrowExceptionForHR(hr);

            Marshal.GetNativeVariantForObject(config.Mic, nativeVariant);
            hr = codecAPI_MP3.SetValue(CodecAPIParam.Mic, nativeVariant);
            DsError.ThrowExceptionForHR(hr);

            Marshal.GetNativeVariantForObject(config.BitRateKbps, nativeVariant);
            hr = codecAPI_MP3.SetValue(CodecAPIParam.AudioBitRate, nativeVariant);
            DsError.ThrowExceptionForHR(hr);

            hr = Marshal.ReleaseComObject(codecAPI_MP3);
            DsError.ThrowExceptionForHR(hr);

            Marshal.FreeCoTaskMem(nativeVariant);
#endif
        }

        public override void ChangeProfile(Profile newProfile)
        {
            try
            {
                int hr;

                if (newProfile.Video == null)
                {
                    AppLogger.Message("Mango.ChangeProfile called but newProfile.Video == null");
                }
                IntPtr nativeVariant = Marshal.AllocCoTaskMem(64);
                ICodecAPI codecAPI = GetCodecAPI();
                if (_captureFilter != null)
                {
                    //detect codec change
                    if (newProfile.Video.CodecType != _currentVideoSettings.CodecType)
                    {
                        throw new ServerGraphRebuildException("codec switch");
                    }

                    //detect resolution change
                    if (newProfile.Video.ImageSize != _currentVideoSettings.ImageSize)
                    {
                        AppLogger.Message(String.Format("ImageSize changing from {0} to {1}",
                                    _currentVideoSettings.ImageSize,
                                    newProfile.Video.ImageSize));

                        if (State == ServerGraphState.Running)
                        {
                            throw new ServerGraphRebuildException("ImageSize change");
                        }

                        Marshal.GetNativeVariantForObject(newProfile.Video.ImageSize, nativeVariant);
                        hr = codecAPI.SetValue(CodecAPIParam.ImageSize, nativeVariant);
                        DsError.ThrowExceptionForHR(hr);
                        _currentVideoSettings.ImageSize = newProfile.Video.ImageSize;
                    }

                    //set key frame rate
                    if (newProfile.Video.KeyFrameRate != _currentVideoSettings.KeyFrameRate)
                    {
                        Marshal.GetNativeVariantForObject(newProfile.Video.KeyFrameRate, nativeVariant);
                        hr = codecAPI.SetValue(CodecAPIParam.KeyFrameRate, nativeVariant);
                        DsError.ThrowExceptionForHR(hr);
                        _currentVideoSettings.KeyFrameRate = newProfile.Video.KeyFrameRate;
                    }

                    //set frame rate
                    if ((newProfile.Video.FrameRate != _currentVideoSettings.FrameRate) || (newProfile.Video.FrameRateUnits != _currentVideoSettings.FrameRateUnits))
                    {
                        int nFrames;
                        if (newProfile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerSecond)
                        {
                            nFrames = 30 / newProfile.Video.FrameRate;
                        }
                        else if (newProfile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerMinute)
                        {
                            nFrames = 1800 / newProfile.Video.FrameRate;
                        }
                        else
                        {
                            throw new UnsupportedFrameRateUnitsException(newProfile.Video.FrameRateUnits);
                        }
                        if (nFrames < 1)
                        {
                            throw new UnsupportedFrameRateUnitsException(newProfile.Video.FrameRateUnits);
                        }
                        Marshal.GetNativeVariantForObject(nFrames, nativeVariant);
                        hr = codecAPI.SetValue(CodecAPIParam.FrameRate, nativeVariant);
                        DsError.ThrowExceptionForHR(hr);
                        _currentVideoSettings.FrameRate = newProfile.Video.FrameRate;
                        _currentVideoSettings.FrameRateUnits = newProfile.Video.FrameRateUnits;
                    }

                    //set constatnt bit rate
                    if (newProfile.Video.ConstantBitRate != _currentVideoSettings.ConstantBitRate)
                    {
                        Marshal.GetNativeVariantForObject(newProfile.Video.ConstantBitRate, nativeVariant);
                        hr = codecAPI.SetValue(PropSetID.ENCAPIPARAM_BitRate, nativeVariant);
                        DsError.ThrowExceptionForHR(hr);
                        _currentVideoSettings.ConstantBitRate = newProfile.Video.ConstantBitRate;
                    }
                }
                Marshal.FreeCoTaskMem(nativeVariant);
                hr = Marshal.ReleaseComObject(codecAPI);
                DsError.ThrowExceptionForHR(hr);
                base.ChangeProfile(newProfile);
            }
            catch (COMException ex)
            {
                if ((ex.ErrorCode == DsResults.MXE_E_Timeout) ||
                    (ex.ErrorCode == DsResults.MXE_E_Fail))
                {
                    throw new ServerGraphRebuildException("Failure from MX card! " + ex.ErrorCode.ToString("X"));
                }
                else
                {
                    throw ex;
                }
            }
        }
    }
}
