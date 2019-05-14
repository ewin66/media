using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.TV;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.Graphs
{
    /// <summary>
    /// CLSID_FilterGraph
    /// </summary>
    [ComImport, Guid("D2185A40-0398-11D3-A53E-00A0C9EF506A")]
    public class VacCtrlProp
    {
    }

    // This is for PVR250/250

    [ComImport, Guid("D2185A40-0398-11D3-A53E-00A0C9EF506A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVacCtrlProp
    {
        [PreserveSig]
        int put_Device(IBaseFilter device);
    }

    public abstract class WinTVAnalogBase : StreamingTVGraph
    {
        #region Class enums

        static public readonly Guid HauppaugeGuid = new Guid(0x432a0da4, 0x806a, 0x43a0, 0xb4, 0x26, 0x4f, 0x2a, 0x23, 0x4a, 0xa6, 0xb8);

        public enum PropertyId
        {
            StreamType = 0,
            VideoClosedOpenGop = 101, //bool											
            VideoBitRate = 102,
            VideoGopSize = 103,
            InverseTelecine = 104,  //bool
            AudioBitRate = 200,
            AudioSampleRate = 201,
            AudioOutput = 202,
            CardModel = 400,
            DriverVersion = 401
        };

        public enum StreamType
        {
            Program = 100,
            DVD = 102,
            Mediacenter = 103,
            SVCD = 104,
            MPEG1 = 201,
            MPEG1_VCD = 202,
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(true)]
        public struct VideoBitRate
        {
            public uint size;
            public uint isnull;
            public uint isvbr;
            public uint bitrate; //kb/sec
            public uint maxBitrate;// kb/sec
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(true)]
        public struct GopSize
        {
            public uint size;
            public uint isnull;
            public uint PictureCount;
            public uint SpaceBetweenPandB;
        };

        public enum AudioBitRateEnum
        {
            Khz32 = 1,
            Khz48 = 2,
            Khz56 = 3,
            Khz64 = 4,
            Khz80 = 5,
            Khz96 = 6,
            Khz112 = 7,
            Khz128 = 8,
            Khz160 = 9,
            Khz192 = 10,
            Khz224 = 11,
            Khz256 = 12,
            Khz320 = 13,
            Khz384 = 14,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(true)]
        public struct AudioBitRate
        {
            public uint size;
            public uint isnull;
            public uint audiolayer;
            public AudioBitRateEnum bitrate;
        }

        public enum AudioSampleRate
        {
            KHz_32 = 0,
            Khz_44_1 = 1,
            Khz_48 = 2
        };

        public enum AudioOutput
        {
            Stereo = 0,
            JointStereo = 1,
            Dual = 2,
            Mono = 3
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1), ComVisible(true)]
        public struct DriverVersion
        {
            public uint size;
            public uint isnull;
            public uint major;
            public uint minor;
            public uint revision;
            public uint build;
        };

        #endregion

        protected IAMTVTuner _amTvTuner;
        protected IAMCrossbar _amCrossbar;
        private IBaseFilter _tuner;
        private IBaseFilter _tvaudio;
        private IBaseFilter _crossbar2;
        private IBaseFilter _videoEncoder;
        private IBaseFilter _audioEncoder;
        private IBaseFilter _mpeg2Demux;
        private IBaseFilter _hcwColorConverter;
        //private IBaseFilter _leadYUVColorConverter;
        //private IBaseFilter _frameRateController;

        private TVSourceInfo _tvConfig;

        private int _deviceIndex;

        protected DsDevice _captureDevice;
        protected DsDevice _tunerDevice; 
        protected DsDevice _audioDevice;
        protected DsDevice _crossbarDevice;
        protected DsDevice _encoderDevice;

        public WinTVAnalogBase(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            _tvConfig = SourceConfig.TVTuner;
            _deviceIndex = _tvConfig.DeviceIndex;
        }

        public override void Dispose(bool disposing)
        {
            AppLogger.Message("WinTV.Dispose");
            if (disposing)
            {
                if (_tuner != null)
                {
                    Marshal.ReleaseComObject(_tuner);
                    _tuner = null;
                }
                if (_tvaudio != null)
                {
                    Marshal.ReleaseComObject(_tvaudio);
                    _tvaudio = null;
                }
                if (_crossbar2 != null)
                {
                    Marshal.ReleaseComObject(_crossbar2);
                    _crossbar2 = null;
                }
                if (_videoEncoder != null)
                {
                    Marshal.ReleaseComObject(_videoEncoder);
                    _videoEncoder = null;
                }
                if (_audioEncoder != null)
                {
                    Marshal.ReleaseComObject(_audioEncoder);
                    _audioEncoder = null;
                }
                if (_mpeg2Demux != null)
                {
                    Marshal.ReleaseComObject(_mpeg2Demux);
                    _mpeg2Demux = null;
                }
                if (_captureFilter != null)
                {
                    Marshal.ReleaseComObject(_captureFilter);
                    _captureFilter = null;
                }
                if (_hcwColorConverter != null)
                {
                    Marshal.ReleaseComObject(_hcwColorConverter);
                    _hcwColorConverter = null;
                }
                if (_amTvTuner != null)
                {
                    Marshal.ReleaseComObject(_amTvTuner);
                    _amTvTuner = null;
                }
                if (_amCrossbar != null)
                {
                    Marshal.ReleaseComObject(_amCrossbar);
                    _amCrossbar = null;
                }
            }
            base.Dispose(disposing);
        }

        public override void Run()
        {
            if (!ChannelScanInProgress)
            {
                TVMode = TVMode.Satellite;
            }
            base.Run();
        }

        public override void Stop()
        {
            _mediaControl.Pause();
            base.Stop();
        }

        protected override Channel GetChannel()
        {
            if (_amTvTuner == null)
            {
                throw new ArgumentNullException("_amTvTuner", "Can't fetch channel!");
            }

            AMTunerSubChannel videoSubChannel;
            AMTunerSubChannel audioSubChannel;
            int newChannel;
            int hr = _amTvTuner.get_Channel(out newChannel, out videoSubChannel, out audioSubChannel);
            DsError.ThrowExceptionForHR(hr);
            return new Channel(newChannel, newChannel, -1);
        }

        protected override void SetChannel(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel", "Can't set channel!");
            }
            if ((channel.PhysicalChannel == -1) && (channel.MajorChannel == -1))
            {
                throw new ArgumentException("Can't set channel, PhysicalChannel or MajorChannel must be set", "channel");
            }
            if (_amTvTuner == null)
            {
                throw new ArgumentNullException("_amTvTuner", "Can't set channel!");
            }

            int hr = _amTvTuner.put_Channel(channel.PhysicalChannel != -1 ? channel.PhysicalChannel : channel.MajorChannel, 0, 0);
            DsError.ThrowExceptionForHR(hr);
        }

        public override TVMode TVMode
        {
            get
            {
                return base.TVMode;
            }
            set
            {
                int hr;

                if (_amCrossbar != null)
                {
                    int i;
                    int j;
                    i = FindCrossbarPin(DirectShowLib.PhysicalConnectorType.Video_VideoDecoder, false);
                    if (i != -1)
                    {
                        j = FindCrossbarPin((value == TVMode.Broadcast) ? DirectShowLib.PhysicalConnectorType.Video_Tuner : DirectShowLib.PhysicalConnectorType.Video_Composite/* DirectShowLib.PhysicalConnectorType.Video_SVideo*/, true);
                        if (j != -1)
                        {
                            hr = _amCrossbar.Route(i, j);
                            DsError.ThrowExceptionForHR(hr);
                        }
                    }
                    // Look for the Audio Decoder output pin.
                    i = FindCrossbarPin(DirectShowLib.PhysicalConnectorType.Audio_AudioDecoder, false);
                    if (i != -1)
                    {
                        // Look for the Audio input pin
                        j = FindCrossbarPin((value == TVMode.Broadcast) ? DirectShowLib.PhysicalConnectorType.Audio_Tuner : DirectShowLib.PhysicalConnectorType.Audio_Line, true);
                        if (j != -1)
                        {
                            hr = _amCrossbar.Route(i, j);
                            DsError.ThrowExceptionForHR(hr);
                        }
                    }
                }

                base.TVMode = value;
            }
        }

        /// <summary>
        /// Returns the AMTunerSignalStrength for the currently tuned channel
        /// </summary>
        protected override int GetSignalStrength()
        {
            if (_amTvTuner != null)
            {
                int hr;
                AMTunerSignalStrength signalStrength;
                hr = _amTvTuner.SignalPresent(out signalStrength);
                DsError.ThrowExceptionForHR(hr);
                return (int)signalStrength;
            }
            else
            {
                return (int)AMTunerSignalStrength.HasNoSignalStrength;
            }
        }

        private int _minChannel = -1, _maxChannel = -1;

        protected override Channel MinChannel
        {
            get
            {
                if (_minChannel < 0)
                {
                    int hr = _amTvTuner.ChannelMinMax(out _minChannel, out _maxChannel);
                    DsError.ThrowExceptionForHR(hr);
                }
                return new Channel(_minChannel);
            }
        }

        protected override Channel MaxChannel
        {
            get
            {
                if (_maxChannel < 0)
                {
                    int hr = _amTvTuner.ChannelMinMax(out _minChannel, out _maxChannel);
                    DsError.ThrowExceptionForHR(hr);
                }
                return new Channel(_maxChannel);
            }
        }

        protected void CreateWinTVGraph()
        {
            _tuner = AddFilterByDevicePath(_tunerDevice.DevicePath, _tunerDevice.Name);
            _tvaudio = AddFilterByDevicePath(_audioDevice.DevicePath, _audioDevice.Name);
            _crossbar2 = AddFilterByDevicePath(_crossbarDevice.DevicePath, _crossbarDevice.Name);
            _captureFilter = AddFilterByDevicePath(_captureDevice.DevicePath, _captureDevice.Name);

            ConnectFilters(_tuner, "Analog Video", _crossbar2, "0: Video Tuner In");
            ConnectFilters(_tuner, "Analog Audio", _tvaudio, "TVAudio In");
            if (_crossbarDevice.Name == "Hauppauge WinTV PVR PCI II Crossbar")
            {
                ConnectFilters(_tvaudio, "TVAudio Out", _crossbar2, "1: Audio Tuner In");
            }
            else if (_crossbarDevice.Name == "Hauppauge WinTV 418 Crossbar")
            {
                ConnectFilters(_tvaudio, "TVAudio Out", _crossbar2, "5: Audio Tuner In");
            }
            ConnectFilters(_crossbar2, "0: Video Decoder Out", _captureFilter, "Analog Video In");
            if (_tvConfig.UseHardwareEncoder == true)
            {
                SetupHardwareEncoder();
            }
            else
            {
                SetupH264Encoder();
            }
            ConnectNetMuxToNetSnk();

            ConfigTV();

            TVMode = TVMode.Satellite;

            SaveGraphFile("WinTV.GRF");
        }

        private void ConfigTV()
        {
            int hr;
            object o;

            hr = _captureGraphBuilder.FindInterface(null, null, _captureFilter, typeof(IAMTVTuner).GUID, out o);
            if (hr >= 0)
            {
                _amTvTuner = (IAMTVTuner)o;
                o = null;
                hr = _captureGraphBuilder.FindInterface(null, null, _captureFilter, typeof(IAMCrossbar).GUID, out o);
                if (hr >= 0)
                {
                    _amCrossbar = (IAMCrossbar)o;
                    o = null;
                }
                else
                {
                    throw new Exception("IAMCrossbar interface not found");
                }
            }
            else
            {
                throw new Exception("IAMTVTuner interface not found");
            }
            hr = _amTvTuner.put_InputType(0, (DirectShowLib.TunerInputType)_tvConfig.AnalogTunerInputType);
            DsError.ThrowExceptionForHR(hr);
        }


        public int FindCrossbarPin(DirectShowLib.PhysicalConnectorType PhysicalType, bool bInput)
        {
            int hr;

            int cOut, cIn;
            hr = _amCrossbar.get_PinCounts(out cOut, out cIn);
            DsError.ThrowExceptionForHR(hr);
            // Enumerate pins and look for a matching pin.
            int count = (bInput ? cIn : cOut);
            for (int i = 0; i < count; i++)
            {
                int iRelated = 0;
                DirectShowLib.PhysicalConnectorType ThisPhysicalType = 0;
                hr = _amCrossbar.get_CrossbarPinInfo(bInput, i, out iRelated, out ThisPhysicalType);
                DsError.ThrowExceptionForHR(hr);
                if (ThisPhysicalType == (DirectShowLib.PhysicalConnectorType)PhysicalType)
                {
                    return i;
                }
            }
            return (-1);
        }

        public void RouteCrossbarPins(DirectShowLib.PhysicalConnectorType outputConnectorType, DirectShowLib.PhysicalConnectorType inputConnectorType)
        {
            int hr;

            if (_amCrossbar != null)
            {
                int i;
                int j;
                i = FindCrossbarPin(outputConnectorType, false);
                if (i != -1)
                {
                    j = FindCrossbarPin(inputConnectorType, true);
                    if (j != -1)
                    {
                        hr = _amCrossbar.Route(i, j);
                        DsError.ThrowExceptionForHR(hr);
                    }
                }
            }
        }

        private void SetupHardwareEncoder()
        {
            _videoEncoder = AddFilterByDevicePath(_encoderDevice.DevicePath, _encoderDevice.Name);
            SetParameters();
            ConnectFilters(_captureFilter, "656", _videoEncoder, "656");
            SetupMPEG2Demultiplexer();
            ConnectFilterToNetMux(_mpeg2Demux, "Video", "Input 01");
            ConnectFilterToNetMux(_mpeg2Demux, "Audio", "Input 02");
        }

        private void SetupMPEG2Demultiplexer()
        {
            int hr;

            _mpeg2Demux = (IBaseFilter)new MPEG2Demultiplexer();
            hr = _graphBuilder.AddFilter(_mpeg2Demux, "MPEG2 Demultiplexer");
            DsError.ThrowExceptionForHR(hr);

            IPin demuxVideoPin = CreateMPEG2DemultiplexerOutputPin("Video", MediaType.Video, MediaSubType.Mpeg2Video);
            IPin demuxAudioPin = CreateMPEG2DemultiplexerOutputPin("Audio", MediaType.Audio, MediaSubType.MPEG1Audio);

            ConnectFilters(_videoEncoder, "MPEG", _mpeg2Demux, "MPEG-2 Stream");

            PinMapStreamId(demuxVideoPin, 0xe0);
            PinMapStreamId(demuxAudioPin, 0xc0);
            Marshal.ReleaseComObject(demuxVideoPin);
            Marshal.ReleaseComObject(demuxAudioPin);
        }

        private IPin CreateMPEG2DemultiplexerOutputPin(string pinName, Guid majorType, Guid subType)
        {
            int hr;

            IMpeg2Demultiplexer demux = (IMpeg2Demultiplexer)_mpeg2Demux;

            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = majorType;
            mediaType.subType = subType;
            IPin pin;
            hr = demux.CreateOutputPin(mediaType, pinName, out pin);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(mediaType);
            return pin;
        }

        private void PinMapStreamId(IPin pin, int streamId)
        {
            int hr;

            IMPEG2StreamIdMap map = (IMPEG2StreamIdMap)pin;
            hr = map.MapStreamId(streamId, MPEG2Program.ElementaryStream, 0, 0);
            DsError.ThrowExceptionForHR(hr);
        }

        private void SetupH264Encoder()
        {
            throw new Exception("SetupH264Encoder disabled.");
#if COMMENT
            _hcwColorConverter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, @"Hauppauge WinTV Color Format Converter 2");
            ConnectFilters(_captureFilter, "Capture", _hcwColorConverter, "Input");

            _leadYUVColorConverter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD Video YUV Converter");
            ConnectFilters(_hcwColorConverter, "Out", _leadYUVColorConverter, "Input");

            _frameRateController = (IBaseFilter)new LMVFramCtrlClass();
            ILMVFrameRateCtrl iFrc = (ILMVFrameRateCtrl)_frameRateController;
            Profiles profiles = Profiles.LoadFromFile(SourceConfig.ProfileGroupName);
            Profile profile = profiles[SourceConfig.ProfileName];
            iFrc.FrameRate = profile.VideoFrameRate;
            iFrc.Enable = true;
            AddFilter((IBaseFilter)_frameRateController, "LEAD Video Frame Rate Controller Filter (2.0)");
            ConnectFilters(_leadYUVColorConverter, "XForm Out", (IBaseFilter)_frameRateController, "XForm In");

            _videoEncoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD H264 Encoder (3.0)");
            LMH264Encoder enc = (LMH264Encoder)_videoEncoder;
            enc.OutputFormat = eH264OUTPUTFORMAT.H264FORMAT_STANDARD_H264;
            enc.EnableRateControl = (Profile.VideoIsVBR == false);
            enc.BitRate = Profile.VideoMaxBitRate * 1024;
            ConnectFilters((IBaseFilter)_frameRateController, "XForm Out", _videoEncoder, "XForm In");

            _audioEncoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD MPEG Audio Encoder (2.0)");
            ConnectFilters(_captureFilter, "Audio Out", _audioEncoder, "XForm In");
            ConnectFilterToNetMux(_videoEncoder, "XForm Out", "Input 01");
            ConnectFilterToNetMux(_audioEncoder, "XForm Out", "Input 02");
#endif
        }

        private void SetPinVideoImageSize(string pinName)
        {
            int hr;
            IPin pin = DsFindPin.ByDirection(_videoEncoder, PinDirection.Output, 0);
            if (pin != null)
                AppLogger.Message("VideoCaptureDevice: found output pin");

            // get video stream interfaces
            AppLogger.Message("VideoCaptureDevice:get Video stream control interface (IAMStreamConfig)");

            IAMStreamConfig streamConfig = (IAMStreamConfig)pin;
            AMMediaType media;
            hr = streamConfig.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);
            VideoInfoHeader v = new VideoInfoHeader();
            v.BmiHeader = new BitmapInfoHeader();
            v.BmiHeader.Width = 320;
            v.BmiHeader.Height = 240;
            media.formatPtr = Marshal.AllocCoTaskMem(1024);
            Marshal.StructureToPtr(v, media.formatPtr, true);
            hr = streamConfig.SetFormat(media);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(media);
        }

        public void SetParameters()
        {
            VideoSettings video = CurrentProfile.Video;
            if (video != null)
            {
                if (video.VBR != null)
                {
                    VBR vbr = video.VBR;
                    SetVideoBitRate(vbr.MinBitRate, vbr.MaxBitRate, true);
                }
                else
                {
                    SetVideoBitRate(video.ConstantBitRate, video.ConstantBitRate, false);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is hauppage.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is hauppage; otherwise, <c>false</c>.
        /// </value>
        public bool IsHauppage
        {
            get
            {
                if (_captureFilter == null) return false;
                IKsPropertySet propertySet = _captureFilter as IKsPropertySet;
                if (propertySet == null) return false;
                Guid propertyGuid = HauppaugeGuid;
                KSPropertySupport IsTypeSupported = 0;
                int hr = propertySet.QuerySupported(propertyGuid, (int)PropertyId.DriverVersion, out IsTypeSupported);
                if (hr != 0 || (IsTypeSupported & KSPropertySupport.Get) == 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the video bit rate.
        /// </summary>
        /// <param name="minKbps">The min KBPS.</param>
        /// <param name="maxKbps">The max KBPS.</param>
        /// <param name="isVBR">if set to <c>true</c> [is VBR].</param>
        /// <returns></returns>
        public bool GetVideoBitRate(out int minKbps, out int maxKbps, out bool isVBR)
        {
            VideoBitRate bitrate = new VideoBitRate();
            bitrate.size = (uint)Marshal.SizeOf(bitrate);
            object obj = GetStructure(HauppaugeGuid, (int)PropertyId.VideoBitRate, typeof(VideoBitRate));
            try
            {
                bitrate = (VideoBitRate)obj;
            }
            catch (Exception exc)
            {
                AppLogger.Message(String.Format("GetVideoBitRate exception: {0}", exc.Message));
            }
            isVBR = bitrate.isvbr != 0;
            minKbps = (int)bitrate.bitrate;
            maxKbps = (int)bitrate.maxBitrate;
            AppLogger.Message(String.Format("WinTV: current VideoProfileSettings: min:{0} max:{1} vbr:{2}", minKbps, maxKbps, isVBR));
            return true;
        }

        /// <summary>
        /// Sets the video bit rate.
        /// </summary>
        /// <param name="minKbps">The min KBPS.</param>
        /// <param name="maxKbps">The max KBPS.</param>
        /// <param name="isVBR">if set to <c>true</c> [is VBR].</param>
        public void SetVideoBitRate(int minKbps, int maxKbps, bool isVBR)
        {
            AppLogger.Message(String.Format("WinTV: SetVideoBitrate min:{0} max:{1} vbr:{2} {3}", minKbps, maxKbps, isVBR, Marshal.SizeOf(typeof(VideoBitRate))));
            VideoBitRate bitrate = new VideoBitRate();
            if (isVBR)
            {
                bitrate.isvbr = 1;
            }
            else
            {
                bitrate.isvbr = 0;
            }
            bitrate.size = (uint)Marshal.SizeOf(typeof(VideoBitRate));
            bitrate.bitrate = (uint)minKbps;
            bitrate.maxBitrate = (uint)maxKbps;
            SetStructure(HauppaugeGuid, (int)PropertyId.VideoBitRate, typeof(VideoBitRate), (object)bitrate);
            GetVideoBitRate(out minKbps, out maxKbps, out isVBR);
        }

        /// <summary>
        /// Gets the audio bit rate.
        /// </summary>
        /// <param name="Kbps">The KBPS.</param>
        /// <returns></returns>
        public bool GetAudioBitRate(out int Kbps)
        {
            AudioBitRate bitrate = new AudioBitRate();
            bitrate.size = (uint)Marshal.SizeOf(bitrate);

            try
            {
                object obj = GetStructure(HauppaugeGuid, (int)PropertyId.AudioBitRate, typeof(AudioBitRate));
                bitrate = (AudioBitRate)obj;
            }
            catch (Exception exc)
            {
                AppLogger.Message(String.Format("GetAudioBitRate exception: {0}", exc.Message));
            }
            Kbps = (int)bitrate.bitrate;
            AppLogger.Message(String.Format("WinTV: current AudioBitrate: {0} kbps ", Kbps));
            return true;
        }

        /// <summary>
        /// Sets the audio bit rate.
        /// </summary>
        /// <param name="Kbps">The KBPS.</param>
        public void SetAudioBitRate(int Kbps)
        {
            AppLogger.Message(String.Format("WinTV: SetAudioBitrate {0}", Kbps));
            AudioBitRate bitrate = new AudioBitRate();

            bitrate.size = (uint)Marshal.SizeOf(typeof(AudioBitRate));
            //Allow explicit setting of this in the future
            bitrate.bitrate = AudioBitRateEnum.Khz192;
            SetStructure(HauppaugeGuid, (int)PropertyId.AudioBitRate, typeof(AudioBitRate), (object)bitrate);
            GetAudioBitRate(out Kbps);
        }

        /// <summary>
        /// Gets the version info.
        /// </summary>
        /// <value>The version info.</value>
        public string VersionInfo
        {
            get
            {
                AppLogger.Message(String.Format("WinTV: get version info {0}", Marshal.SizeOf(typeof(DriverVersion))));
                DriverVersion version = new DriverVersion();
                version.size = (uint)Marshal.SizeOf(typeof(DriverVersion));
                object obj = GetStructure(HauppaugeGuid, (int)PropertyId.DriverVersion, typeof(DriverVersion));
                try
                {
                    version = (DriverVersion)obj;
                }
                catch (Exception exc)
                {
                    AppLogger.Message(String.Format("WinTV.VersionInfo exception {0}", exc.Message));
                }
                return String.Format("{0}.{1}.{2}.{3}", version.major, version.minor, version.revision, version.build);
            }
        }

        public object GetStructure(Guid guidPropSet, int propId, System.Type structureType)
        {
            Guid propertyGuid = guidPropSet;
            IKsPropertySet propertySet = _captureFilter as IKsPropertySet;
            KSPropertySupport IsTypeSupported = 0;
            int uiSize;
            if (propertySet == null)
            {
                AppLogger.Message("GetStructure() properySet=null");
                return null;
            }
            int hr = propertySet.QuerySupported(propertyGuid, propId, out IsTypeSupported);
            if (hr != 0 || (IsTypeSupported & KSPropertySupport.Get) == 0)
            {
                AppLogger.Message("GetString() GetStructure is not supported");
                return null;
            }
            object objReturned = null;
            IntPtr pDataReturned = Marshal.AllocCoTaskMem(1000);
            hr = propertySet.Get(propertyGuid, propId, IntPtr.Zero, 0, pDataReturned, 1000, out uiSize);
            if (hr == 0)
            {
                objReturned = Marshal.PtrToStructure(pDataReturned, structureType);
            }
            else
            {
                AppLogger.Message(String.Format("GetStructure() failed 0x{0:X}", hr));
            }
            Marshal.FreeCoTaskMem(pDataReturned);
            return objReturned;
        }

        protected virtual void SetStructure(Guid guidPropSet, int propId, System.Type structureType, object structValue)
        {
            Guid propertyGuid = guidPropSet;
            IKsPropertySet propertySet = _captureFilter as IKsPropertySet;
            KSPropertySupport IsTypeSupported = 0;
            if (propertySet == null)
            {
                AppLogger.Message("SetStructure() properySet=null");
                return;
            }
            int hr = propertySet.QuerySupported(propertyGuid, propId, out IsTypeSupported);
            if (hr != 0 || (IsTypeSupported & KSPropertySupport.Set) == 0)
            {
                AppLogger.Message("GetString() GetStructure is not supported");
                return;
            }
            int iSize = Marshal.SizeOf(structureType);
            IntPtr pDataReturned = Marshal.AllocCoTaskMem(iSize);
            Marshal.StructureToPtr(structValue, pDataReturned, true);
            hr = propertySet.Set(propertyGuid, propId, pDataReturned, (int)iSize, pDataReturned, (int)iSize);
            if (hr != 0)
            {
                AppLogger.Message(String.Format("SetStructure() failed 0x{0:X}", hr));
            }
            Marshal.FreeCoTaskMem(pDataReturned);
        }

        protected DsDevice FindDevice(Guid filterCategory, string friendlyName)
        {
            List<DsDevice> deviceList = new List<DsDevice>();
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(filterCategory))
            {
                if (device.Name != null)
                {
                    if (device.Name == friendlyName)
                    {
                        deviceList.Add(device);
                    }
                }
            }
            if (deviceList.Count > _deviceIndex)
            {
                return deviceList[_deviceIndex];
            }
            else
            {
                throw new Exception("Device instance " + _deviceIndex + " for " + friendlyName + " not found.");
            }
        }


        private object GetStreamConfigSetting(IAMStreamConfig streamConfig, string fieldName)
        {
            object returnValue = null;
            try
            {
                if (streamConfig == null)
                    throw new NotSupportedException();

                IntPtr pmt = IntPtr.Zero;
                AMMediaType mediaType = new AMMediaType();

                try
                {
                    // Get the current format info
                    mediaType.formatType = FormatType.VideoInfo2;
                    int hr = streamConfig.GetFormat(out mediaType);
                    if (hr != 0)
                    {
                        AppLogger.Message(String.Format("VideoCaptureDevice:getStreamConfigSetting() FAILED to get:{0} (not supported)", fieldName));
                        Marshal.ThrowExceptionForHR(hr);
                    }
                    // The formatPtr member points to different structures
                    // dependingon the formatType
                    object formatStruct;
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() find formattype"); 
                    if (mediaType.formatType == FormatType.WaveEx)
                        formatStruct = new WaveFormatEx();
                    else if (mediaType.formatType == FormatType.VideoInfo)
                        formatStruct = new VideoInfoHeader();
                    else if (mediaType.formatType == FormatType.VideoInfo2)
                        formatStruct = new VideoInfoHeader2();
                    else if (mediaType.formatType == FormatType.Mpeg2Video)
                        formatStruct = new MPEG2VideoInfo();
                    else if (mediaType.formatType == FormatType.None)
                    {
                        //Log.Info("VideoCaptureDevice:getStreamConfigSetting() FAILED no format returned");
                        //throw new NotSupportedException("This device does not support a recognized format block.");
                        return null;
                    }
                    else
                    {
                        //Log.Info("VideoCaptureDevice:getStreamConfigSetting() FAILED unknown fmt:{0} {1} {2}", mediaType.formatType, mediaType.majorType, mediaType.subType);
                        //throw new NotSupportedException("This device does not support a recognized format block.");
                        return null;
                    }

                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() get formatptr");
                    // Retrieve the nested structure
                    Marshal.PtrToStructure(mediaType.formatPtr, formatStruct);

                    // Find the required field
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() get field");
                    Type structType = formatStruct.GetType();
                    FieldInfo fieldInfo = structType.GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        //Log.Info("VideoCaptureDevice.getStreamConfigSetting() FAILED to to find member:{0}", fieldName);
                        //throw new NotSupportedException("VideoCaptureDevice:FAILED to find the member '" + fieldName + "' in the format block.");
                        return null;
                    }

                    // Extract the field's current value
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() get value");
                    returnValue = fieldInfo.GetValue(formatStruct);
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() done");	
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pmt);
                }
            }
            catch (Exception)
            {
                AppLogger.Message("  VideoCaptureDevice.getStreamConfigSetting() FAILED ");
            }
            return (returnValue);
        }

        private object SetStreamConfigSetting(IAMStreamConfig streamConfig, string fieldName, object newValue)
        {
            try
            {
                object returnValue = null;
                IntPtr pmt = IntPtr.Zero;
                AMMediaType mediaType = new AMMediaType();

                try
                {
                    // Get the current format info
                    int hr = streamConfig.GetFormat(out mediaType);
                    if (hr != 0)
                    {
                        AppLogger.Message(String.Format("  VideoCaptureDevice:setStreamConfigSetting() FAILED to set:{0} (getformat) hr:{1}", fieldName, hr));
                        return null;//Marshal.ThrowExceptionForHR(hr);
                    }
                    //Log.Info("  VideoCaptureDevice:setStreamConfigSetting() get formattype");
                    // The formatPtr member points to different structures
                    // dependingon the formatType
                    object formatStruct;
                    if (mediaType.formatType == FormatType.WaveEx)
                        formatStruct = new WaveFormatEx();
                    else if (mediaType.formatType == FormatType.VideoInfo)
                        formatStruct = new VideoInfoHeader();
                    else if (mediaType.formatType == FormatType.VideoInfo2)
                        formatStruct = new VideoInfoHeader2();
                    else if (mediaType.formatType == FormatType.Mpeg2Video)
                        formatStruct = new MPEG2VideoInfo();
                    else if (mediaType.formatType == FormatType.None)
                    {
                        AppLogger.Message("  VideoCaptureDevice:setStreamConfigSetting() FAILED no format returned");
                        return null;// throw new NotSupportedException("This device does not support a recognized format block.");
                    }
                    else
                    {
                        AppLogger.Message("  VideoCaptureDevice:setStreamConfigSetting() FAILED unknown fmt");
                        return null;//throw new NotSupportedException("This device does not support a recognized format block.");
                    }
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() get formatptr");
                    // Retrieve the nested structure
                    Marshal.PtrToStructure(mediaType.formatPtr, formatStruct);

                    // Find the required field
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() get field");
                    Type structType = formatStruct.GetType();
                    FieldInfo fieldInfo = structType.GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        AppLogger.Message(String.Format("  VideoCaptureDevice:setStreamConfigSetting() FAILED to to find member:{0}", fieldName));
                        throw new NotSupportedException("FAILED to find the member '" + fieldName + "' in the format block.");
                    }
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() set value");
                    // Update the value of the field
                    fieldInfo.SetValue(formatStruct, newValue);

                    // PtrToStructure copies the data so we need to copy it back
                    Marshal.StructureToPtr(formatStruct, mediaType.formatPtr, false);

                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() set format");
                    // Save the changes
                    hr = streamConfig.SetFormat(mediaType);
                    if (hr != 0)
                    {
                        AppLogger.Message(String.Format("  VideoCaptureDevice:setStreamConfigSetting() FAILED to set:{0} {1}", fieldName, hr));
                        return null;//Marshal.ThrowExceptionForHR(hr);
                    }
                    //else Log.Info("  VideoCaptureDevice.setStreamConfigSetting() set:{0}",fieldName);
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() done");
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pmt);
                }
                return (returnValue);
            }
            catch (Exception)
            {
                AppLogger.Message("  VideoCaptureDevice.:setStreamConfigSetting() FAILED ");
            }
            return null;
        }

        public Size FrameSize
        {
            get
            {
                if (_interfaceStreamConfigVideoCapture != null)
                {
                    try
                    {
                        BitmapInfoHeader bmiHeader;
                        object obj = GetStreamConfigSetting(_interfaceStreamConfigVideoCapture, "BmiHeader");
                        if (obj != null)
                        {
                            bmiHeader = (BitmapInfoHeader)obj;
                            return new Size(bmiHeader.Width, bmiHeader.Height);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return new Size(720, 576);
            }
            set
            {
                if (value.Width > 0 && value.Height > 0)
                {
                    if (_interfaceStreamConfigVideoCapture != null)
                    {
                        try
                        {
                            BitmapInfoHeader bmiHeader;
                            object obj = GetStreamConfigSetting(_interfaceStreamConfigVideoCapture, "BmiHeader");
                            if (obj != null)
                            {
                                bmiHeader = (BitmapInfoHeader)obj;
                                AppLogger.Message(String.Format("VideoCaptureDevice:change capture Framesize :{0}x{1} ->{2}x{3}", bmiHeader.Width, bmiHeader.Height, FrameSize.Width, FrameSize.Height));
                                bmiHeader.Width = value.Width;
                                bmiHeader.Height = value.Height;
                                SetStreamConfigSetting(_interfaceStreamConfigVideoCapture, "BmiHeader", bmiHeader);
                            }
                        }
                        catch (Exception)
                        {
                            AppLogger.Message(String.Format("VideoCaptureDevice:FAILED:could not set capture  Framesize to {0}x{1}!", FrameSize.Width, FrameSize.Height));
                        }
                    }
                }
            }
        }

        public double FrameRate
        {
            get
            {
                if (_interfaceStreamConfigVideoCapture != null)
                {
                    try
                    {
                        object obj = GetStreamConfigSetting(_interfaceStreamConfigVideoCapture, "AvgFrameRate");
                        if (obj != null)
                        {
                            long frameRate = (long)(10000000d / (double)obj);
                            return frameRate;
                        }
                    }
                    catch (Exception exc)
                    {
                        AppLogger.Message(String.Format("SteamingTVGraph.FrameRate exception: {0}", exc.Message));
                    }
                }
                return 0;
            }
            set
            {
                if (value >= 1d && value <= 30d)
                {
                    if (_interfaceStreamConfigVideoCapture != null)
                    {
                        try
                        {
                            AppLogger.Message(String.Format("SWGraph:capture FrameRate set to {0}", value));
                            long avgTimePerFrame = (long)(10000000d / value);
                            SetStreamConfigSetting(_interfaceStreamConfigVideoCapture, "AvgTimePerFrame", avgTimePerFrame);
                        }
                        catch (Exception exc)
                        {
                            AppLogger.Message(String.Format("StreamingTVGraph.FrameRate threw exception: {0}", exc.Message));
                        }
                    }
                }
            }
        }

        private IAMStreamConfig _interfaceStreamConfigVideoCapture = null;

        public void SetupCaptureFormat(IBaseFilter filter)
        {
            AppLogger.Message("VideoCaptureDevice:get Video stream control interface (IAMStreamConfig)");
            object o;
            int hr = _captureGraphBuilder.FindInterface(PinCategory.Preview, null, (IBaseFilter)filter, typeof(IAMStreamConfig).GUID, out o);
            if (hr == 0)
            {
                _interfaceStreamConfigVideoCapture = o as IAMStreamConfig;
                if (_interfaceStreamConfigVideoCapture != null)
                {
                    AppLogger.Message(String.Format("FrameRate before set {0}", FrameRate));
                    FrameRate = 15d;
                    //FrameSize = new Size(720, 576);
                    // Size size = FrameSize;
                    // if (size.Width != 720 || size.Height != 576)
                    //  {
                    //  FrameSize = new Size(640, 480);
                    //   FrameSize = new Size(352, 240);
                    // }
                }
            }
            else
            {
                AppLogger.Message("Failed to find Preview interface on filter");
            }
            AppLogger.Message(String.Format("FrameRate after set {0}", FrameRate));
            return;
        }
    }
}
