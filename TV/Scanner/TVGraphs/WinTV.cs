using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.TV.Scanner.Config;
//using LMVFrameRateCtrlLib;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    #region helper classes and interfaces

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

    #endregion

    public class WinTV: AnalogTuner
    {
        private IBaseFilter _colorConverter;
        private DsDevice _colorConverterDevice;
        protected DsDevice _encoderDevice;
        

        #region Guids

        static public readonly Guid HauppaugeGuid = new Guid(0x432a0da4, 0x806a, 0x43a0, 0xb4, 0x26, 0x4f, 0x2a, 0x23, 0x4a, 0xa6, 0xb8);

        #endregion

        #region enums

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
        #endregion

        #region structs

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

        public WinTV(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
            CaptureRequiresNewGraph = true;
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            if (disposing)
            {
                ForceReleaseComObject(_colorConverter);
                _colorConverter = null;
                if (_colorConverterDevice != null)
                {
                    ForceReleaseComObject(_colorConverterDevice.Mon);
                    _colorConverterDevice = null;
                }

                DsUtils.FreeAMMediaType(_emptyAMMediaType);
            }
            base.Dispose(disposing);
        }

        public override void Render()
        {
            if (_tuner == null)
                throw new ArgumentNullException("_tuner", "No Tuner Device present");

            string version = VersionInfo;

            _encoderDevice = FindDevice(FilterCategory.WDMStreamingEncoderDevices, Config.FilterType.VideoEncoder);

            _colorConverterDevice = FindDevice(FilterCategory.LegacyAmFilterCategory, Config.FilterType.ColorConverter);
            _colorConverter = AddFilterByDevicePath(_colorConverterDevice.DevicePath, _colorConverterDevice.Name);

            //connect tuner video to crossbar
            ConnectFilters(_tuner, "Analog Video", _crossbar2, GraphConfig[FilterType.Crossbar].InPin[0], true);
            //connect tuner audio to TvAudio, then to crossbar
            ConnectFilters(_tuner, "Analog Audio", _tvaudio, "TVAudio In", true);
            ConnectFilters(_tvaudio, "TVAudio Out", _crossbar2, GraphConfig[FilterType.Crossbar].InPin[1], true);
            

            ConnectFilters(_crossbar2, "0: Video Decoder Out", _captureFilter, "Analog Video In", true);
            ConnectFilters(_crossbar2, "1: Audio Decoder Out", _captureFilter, "Analog Audio In", true);

            ConnectFilters(_captureFilter, "Capture", _colorConverter, "Input", true);
            ConnectFilters(_colorConverter, "Out", _videoRender, "VMR Input0", true);

            AddAudioRenderer();

            ConnectToAudio(_captureFilter, "Audio Out", true);

            ConfigTV();

            RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Video_VideoDecoder, DirectShowLib.PhysicalConnectorType.Video_Tuner);
            RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Audio_AudioDecoder, DirectShowLib.PhysicalConnectorType.Audio_Tuner);

            this.SaveGraphFile();
            CaptureRequiresNewGraph = false;

            base.Render();
        }

        public override void RenderCapture(string fileName)
        {
            int hr;

            

            if (_encoderDevice == null)
            {
                throw new Exception("Encoder device \"" + GraphConfig[FilterType.VideoEncoder].Name + "\" could not be loaded!");
            }
            if (_writerDevice == null)
            {
                throw new Exception("Writer device \"" + GraphConfig[FilterType.Writer].Name + "\" could not be loaded!");
            }

            _videoEncoder = AddFilterByDevicePath(_encoderDevice.DevicePath, _encoderDevice.Name);
            _fileWriter = AddFilterByDevicePath(_writerDevice.DevicePath, _writerDevice.Name);

            SetVideoBitRate(GraphConfig.VideoBitrate.MinBitRate, GraphConfig.VideoBitrate.MaxBitRate, GraphConfig.VideoIsVBR);
            ConnectFilters(_captureFilter, "656", _videoEncoder, "656", true);
            ConnectFilters(_videoEncoder, "MPEG", _fileWriter, "Input", true);

            IFileSinkFilter fileSinkControl = (IFileSinkFilter)_fileWriter;
            hr = fileSinkControl.SetFileName(fileName + ".mpg", _emptyAMMediaType);
            DsError.ThrowExceptionForHR(hr);

            StartDVRWriter(_fileWriter);
        }

        /// <summary>
        /// Stops capturing to a file.
        /// </summary>
        public override void StopCapture()
        {
            StopWhenReady();
            int hr = 0;
            if (r_dvrWriter != null)
            {
                hr = r_dvrWriter.StopRecording();
            }
            DsError.ThrowExceptionForHR(hr);

            hr = _graphBuilder.RemoveFilter(_videoEncoder);
            DsError.ThrowExceptionForHR(hr);
            hr = _graphBuilder.RemoveFilter(_fileWriter);
            DsError.ThrowExceptionForHR(hr);

            ReleaseComObject(_videoEncoder);
            ReleaseComObject(_fileWriter);
        }

        private bool _captureRequiresNewGraph;
        public override bool CaptureRequiresNewGraph
        {
            get
            {
                return _captureRequiresNewGraph;
            }
            protected set
            {
                _captureRequiresNewGraph = value;
            }
        }

        private void SetPinVideoImageSize(string pinName)
        {
            int hr;
            IPin pin = DsFindPin.ByDirection(_videoEncoder, PinDirection.Output, 0);
            if (pin != null)
                Console.WriteLine("VideoCaptureDevice: found output pin");

            // get video stream interfaces
            Console.WriteLine("VideoCaptureDevice:get Video stream control interface (IAMStreamConfig)");

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

            ReleaseComObject(pin);
            DsUtils.FreeAMMediaType(media);
            Marshal.FreeCoTaskMem(media.formatPtr);
            DsError.ThrowExceptionForHR(hr);
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
                Console.WriteLine("GetVideoBitRate exception: {0}", exc.Message);
            }
            isVBR = bitrate.isvbr != 0;
            minKbps = (int)bitrate.bitrate;
            maxKbps = (int)bitrate.maxBitrate;
            Console.WriteLine("WinTV: current VideoProfileSettings: min:{0} max:{1} vbr:{2}", minKbps, maxKbps, isVBR);
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
            Console.WriteLine("WinTV: SetVideoBitrate min:{0} max:{1} vbr:{2} {3}", minKbps, maxKbps, isVBR, Marshal.SizeOf(typeof(VideoBitRate)));
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
                Console.WriteLine("GetAudioBitRate exception: {0}", exc.Message);
            }
            Kbps = (int)bitrate.bitrate;
            Console.WriteLine("WinTV: current AudioBitrate: {0} kbps ", Kbps);
            return true;
        }

        /// <summary>
        /// Sets the audio bit rate.
        /// </summary>
        /// <param name="Kbps">The KBPS.</param>
        public void SetAudioBitRate(int Kbps)
        {
            Console.WriteLine("WinTV: SetAudioBitrate {0}", Kbps);
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
        public string CardModel
        {
            get
            {
                object obj = GetStructure(HauppaugeGuid, (int)PropertyId.CardModel, typeof(string));
#if COMMENT
                try
                {
                }
                catch (Exception exc)
                {
                    Console.WriteLine("WinTV.VersionInfo exception {0}", exc.Message);
                }
                return String.Format("{0}.{1}.{2}.{3}", version.major, version.minor, version.revision, version.build);
#endif
                return (string)obj;
            }
        }

        /// <summary>
        /// Gets the version info.
        /// </summary>
        /// <value>The version info.</value>
        public string VersionInfo
        {
            get
            {
                Console.WriteLine("WinTV: get version info {0}", Marshal.SizeOf(typeof(DriverVersion)));
                DriverVersion version = new DriverVersion();
                version.size = (uint)Marshal.SizeOf(typeof(DriverVersion));
                object obj = GetStructure(HauppaugeGuid, (int)PropertyId.DriverVersion, typeof(DriverVersion));
                try
                {
                    version = (DriverVersion)obj;
                }
                catch (Exception exc)
                {
                    Console.WriteLine("WinTV.VersionInfo exception {0}", exc.Message);
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
                Console.WriteLine("GetStructure() properySet=null");
                return null;
            }
            int hr = propertySet.QuerySupported(propertyGuid, propId, out IsTypeSupported);
            if (hr != 0 || (IsTypeSupported & KSPropertySupport.Get) == 0)
            {
                Console.WriteLine("GetString() GetStructure is not supported");
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
                Console.WriteLine("GetStructure() failed 0x{0:X}", hr);
            }
            Marshal.FreeCoTaskMem(pDataReturned);
            return objReturned;
        }

        private void SetStructure(Guid guidPropSet, int propId, System.Type structureType, object structValue)
        {
            Guid propertyGuid = guidPropSet;
            IKsPropertySet propertySet = _captureFilter as IKsPropertySet;
            KSPropertySupport IsTypeSupported = 0;
            if (propertySet == null)
            {
                Console.WriteLine("SetStructure() properySet=null");
                return;
            }
            int hr = propertySet.QuerySupported(propertyGuid, propId, out IsTypeSupported);
            if (hr != 0 || (IsTypeSupported & KSPropertySupport.Set) == 0)
            {
                Console.WriteLine("GetString() GetStructure is not supported");
                return;
            }
            int iSize = Marshal.SizeOf(structureType);
            IntPtr pDataReturned = Marshal.AllocCoTaskMem(iSize);
            Marshal.StructureToPtr(structValue, pDataReturned, true);
            hr = propertySet.Set(propertyGuid, propId, pDataReturned, (int)iSize, pDataReturned, (int)iSize);
            if (hr != 0)
            {
                Console.WriteLine("SetStructure() failed 0x{0:X}", hr);
            }
            Marshal.FreeCoTaskMem(pDataReturned);
        }
    }
}
