using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.DirectShowLib.BDA;
using FutureConcepts.Media.TV;
using FutureConcepts.Media.TV.AtscPsip;
using FutureConcepts.Tools;
using LMH264EncoderLib;

namespace FutureConcepts.Media.Server.Graphs
{
    /// <summary>
    /// Provides ATSC TV from a WinTV418 card
    /// </summary>
    /// <summary>
    /// kdixon 02/2009 - 05/06/2009
    /// </summary>
    public class WinTV418ATSC : StreamingTVGraph, IVirtualChannelProvider
    {
        #region Member Variables

        private static readonly Guid CLSID_ATSCNetworkProvider = typeof(ATSCNetworkProvider).GUID;

        //our tuning space
        protected ITuningSpace tuningSpace;
        //Source
        //network provider
        protected IBaseFilter networkProvider;
        protected ITuner tuner;
        //tuner
        protected IBaseFilter tunerFilter;
        //capture
        protected IBaseFilter captureFilter;

        //MPEG2 Demultiplexer
        protected IBaseFilter mpeg2Demux;

        //Meta Data
        //transport information filter
        protected IBaseFilter bdaTransportInfo;
        //MPEG-2 sections and tables
        protected IBaseFilter mpeg2SectionsAndTables;
        protected IMpeg2Data mpeg2Data;
        protected Parser _vctParser;

        //transcoding
        protected IBaseFilter videoEncoder;
        protected ILMH264Encoder h264Encoder;

        private int _deviceIndex = 0;

        #endregion

        #region Constructor / Destructor

        public WinTV418ATSC(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest)
            : base(sourceConfig, openGraphRequest)
        {
            if (SourceConfig.TVTuner == null)
            {
                throw new SourceConfigException("TVTuner config section not found!");
            }

            _deviceIndex = sourceConfig.TVTuner.DeviceIndex;

            tuningSpace = GetTuningSpace();

            Guid networkType;
            tuningSpace.get__NetworkType(out networkType);
            if (networkType.Equals(Guid.Empty))
            {
                throw new Exception("Not a digital Network Type!");
            }

            networkProvider = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(networkType));
            int hr = _graphBuilder.AddFilter(networkProvider, "Network Provider");
            DsError.ThrowExceptionForHR(hr);
            tuner = (ITuner)networkProvider;

            //add MPEG2 Demux
            mpeg2Demux = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "MPEG-2 Demultiplexer");

            //add BDA MPEG-2 Transport Information Filter
            bdaTransportInfo = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDATransportInformationRenderersCategory, "BDA MPEG2 Transport Information Filter");
            //add MPEG-2 Sections and Tables Filter
            mpeg2SectionsAndTables = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDATransportInformationRenderersCategory, "MPEG-2 Sections and Tables");

            //tunerFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDASourceFiltersCategory, "Hauppauge WinTV 418 BDA Tuner");
            DsDevice tunerDevice = FindDevice(FilterCategory.BDASourceFiltersCategory, "Hauppauge WinTV 418 BDA Tuner");
            tunerFilter = FilterGraphTools.AddFilterByDevicePath(_graphBuilder, tunerDevice.DevicePath, tunerDevice.Name);
            //captureFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDAReceiverComponentsCategory, "Hauppauge WinTV 418 TS Capture");
            DsDevice captureDevice = FindDevice(FilterCategory.BDAReceiverComponentsCategory, "Hauppauge WinTV 418 TS Capture");
            captureFilter = FilterGraphTools.AddFilterByDevicePath(_graphBuilder, captureDevice.DevicePath, captureDevice.Name);

            //connect network provider to BDA tuner
            ConnectFilters(networkProvider, "Antenna Out", (IBaseFilter)tunerFilter, "Input0", false);
            ConnectFilters((IBaseFilter)tunerFilter, "MPEG2 Transport", captureFilter, "MPEG2 Transport", false);

            //These filters need to be connected via their explicit pins,
            //  because Hauppauge saw fit to name all the pins on the capture filter the same name.
            IPin captureFilterOut = DsFindPin.ByDirection(captureFilter, PinDirection.Output, 0);
            IPin mpeg2DemuxIn = DsFindPin.ByDirection(mpeg2Demux, PinDirection.Input, 0);
            FilterGraphTools.ConnectFilters(_graphBuilder, captureFilterOut, mpeg2DemuxIn, false);
            //clean up the two pins
            Marshal.ReleaseComObject(captureFilterOut);
            Marshal.ReleaseComObject(mpeg2DemuxIn);

            //add and connect meta data filters and connect them to the Demux
            ConnectFilters(mpeg2Demux, "1", bdaTransportInfo, "Input", false);
            ConnectFilters(mpeg2Demux, "5", mpeg2SectionsAndTables, "In", false);

            //this interface must be queried after the S&T filter has been connected.
            mpeg2Data = (IMpeg2Data)mpeg2SectionsAndTables;
            _vctParser = new Parser(mpeg2Data);

            //build out the rest of the graph for the given source config / profile
            BuildForProfile();

            ConnectNetMuxToNetSnk();

            if (KnownChannels.Count > 0)
            {
                this.Channel = KnownChannels.Items[0];
            }

            SaveGraphFile("WinTV418ATSC.grf");
        }

        /// <summary>
        /// Constructs the remainder of the graph (after the Demuxer) according to the source config and profile.
        /// </summary>
        private void BuildForProfile()
        {
            //Configure for specific codec / transcoding
            //if UseHardwareEncoder is checked, we will simply hand off the Elementry Streams from the Air
            if (SourceConfig.TVTuner.UseHardwareEncoder)
            {
                //connect video
                ConnectFilterToNetMux(mpeg2Demux, "2", "Input 01");
                //connect audio
                ConnectFilterToNetMux(mpeg2Demux, "3", "Input 02");
            }
            else // if they want to use software encoder, we will encode using the default profile
            {
                if (CurrentProfile.Video != null)
                {
                    if (CurrentProfile.Video.CodecType != VideoCodecType.H264)
                    {
                        throw new NotSupportedException("H.264 is the only supported target codec in software transcoding mode!");
                    }

                    if (CurrentProfile.Video.ImageSize != VideoImageSize.Undefined)
                    {
                        throw new NotSupportedException("Resizing is not supported!");
                    }

                    videoEncoder = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD H264 Encoder (4.0)");
                    if (videoEncoder == null)
                    {
                        throw new Exception("Could not instantiate H264 Encoder!");
                    }
                    h264Encoder = (ILMH264Encoder)videoEncoder;
                    if (h264Encoder == null)
                    {
                        throw new Exception("Could not query for ILMH264Encoder interface!");
                    }

                    FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Elecard MPEG-2 Video Decoder");

                    FilterGraphTools.ConnectFilters(_graphBuilder, mpeg2Demux, "2", videoEncoder, "XForm In", true);

                    h264Encoder.EncodingThreads = eH264ENCODINGTHREADS.H264THREAD_AUTO;
                    h264Encoder.FrameRate = -1;
                    h264Encoder.EncodingSpeed = eH264ENCODINGSPEED.H264SPEED_1;
                    //h264Encoder.EncodingSpeed = eH264ENCODINGSPEED.H264SPEED_2;

                    h264Encoder.EnableRateControl = true;
                    h264Encoder.BitRate = CurrentProfile.Video.ConstantBitRate * 1024;

                    h264Encoder.EnableSuperCompression = false;
                    h264Encoder.SymbolMode = eH264SYMBOLMODE.H264SYMBOL_CAVLC;
                    h264Encoder.OutputFormat = eH264OUTPUTFORMAT.H264FORMAT_STANDARD_H264;

                    h264Encoder.IFrameInterval = CurrentProfile.Video.KeyFrameRate;
                    h264Encoder.PFrameInterval = 0;

                    //connect video
                    ConnectFilterToNetMux(videoEncoder, "XForm Out", "Input 01");
                }
                if (CurrentProfile.Audio != null)
                {
                    if (CurrentProfile.Audio.CodecType != AudioCodecType.DolbyDigital)
                    {
                        throw new NotSupportedException("Audio transcoding is not implemented!");
                    }
                    //connect audio
                    ConnectFilterToNetMux(mpeg2Demux, "3", (CurrentProfile.Video != null) ? "Input 02" : "Input 01");
                }
            }
        }

        public override void Dispose(bool disposing)
        {
            ReleaseComObject(videoEncoder);
            ReleaseComObject(h264Encoder);

            ReleaseComObject(tuningSpace);
            ReleaseComObject(networkProvider);
            ReleaseComObject(tuner);
            ReleaseComObject(tunerFilter);
            ReleaseComObject(captureFilter);
            ReleaseComObject(mpeg2Demux);

            ReleaseComObject(bdaTransportInfo);
            ReleaseComObject(mpeg2SectionsAndTables);
            ReleaseComObject(mpeg2Data);
            base.Dispose(disposing);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Finds the ATSC Tuning space in the System Tuning Spaces and returns it
        /// </summary>
        /// <returns>an ITuningSpace, which is the System Tuning Spaces</returns>
        private ITuningSpace GetTuningSpace()
        {
            ITuningSpaceContainer system = (ITuningSpaceContainer)new SystemTuningSpaces();

            try
            {
                IEnumTuningSpaces systemEnum;
                int hr = system.get_EnumTuningSpaces(out systemEnum);
                DsError.ThrowExceptionForHR(hr);

                IntPtr retreivedSpaceCount = Marshal.AllocCoTaskMem(4);
                try
                {
                    ITuningSpace[] retreivedSpaces = new ITuningSpace[1];
                    do
                    {
                        hr = systemEnum.Next(1, retreivedSpaces, retreivedSpaceCount);
                        DsError.ThrowExceptionForHR(hr);

                        if (Marshal.ReadInt32(retreivedSpaceCount) > 0)
                        {
                            Guid networkType;
                            hr = retreivedSpaces[0].get__NetworkType(out networkType);
                            DsError.ThrowExceptionForHR(hr);
                            if (networkType.Equals(CLSID_ATSCNetworkProvider))
                            {
                                return retreivedSpaces[0];
                            }
                        }
                    } while (Marshal.ReadInt32(retreivedSpaceCount) > 0);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(retreivedSpaceCount);
                }

                throw new Exception("Could not locate ATSC Tuning Space!");
            }
            finally
            {
                Marshal.ReleaseComObject(system);
            }
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


        /// <summary>
        /// Get the signal strength for the currently tuned channel
        /// </summary>
        protected override int GetSignalStrength()
        {
            if (tuner != null)
            {
                int strength;
                int hr = tuner.get_SignalStrength(out strength);
                DsError.ThrowExceptionForHR(hr);

                if (strength == 0)
                {
                    return 0;
                }

                //translate the values to get "acceptable" signal strengths only
                strength = (-1 * strength) + 4000;
                if (strength == 0)
                {
                    strength += 1;
                }
                return strength;
            }
            return -1;
        }

        #endregion

        public override void Run()
        {
            base.Run();

            if (this.KnownChannels.Count == 0)
            {
                StartChannelScan();
            }

            if (!ChannelScanInProgress)
            {
                /*
                 * If there is no signal on a given channel, then the LEADTOOLS sink/source cannot handshake
                 * presumably because transfer of streaming data is a prerequisite for the handshake to succeed.
                 * This code prevents this issue by selecting a channel with valid signal.
                 * kdixon 01/30/2009
                 */
                int count = 0;
                while ((this.GetSignalStrength() <= 0) && (count < this.KnownChannels.Count))
                {
                    ChannelUp();
                    count++;
                }
                if (count >= this.KnownChannels.Count)
                {
                    StartChannelScan();
                }
            }
        }

        #region Channel Implementation

        protected override Channel GetChannel()
        {
            ITuneRequest request;
            int hr = tuner.get_TuneRequest(out request);
            DsError.ThrowExceptionForHR(hr);

            IATSCChannelTuneRequest atscRequest = (IATSCChannelTuneRequest)request;
            ILocator locator;
            atscRequest.get_Locator(out locator);
            IATSCLocator atscLocator = (IATSCLocator)locator;

            int freq, channel, major, minor;
            hr = atscLocator.get_CarrierFrequency(out freq);
            hr = atscLocator.get_PhysicalChannel(out channel);
            hr = atscRequest.get_Channel(out major);
            hr = atscRequest.get_MinorChannel(out minor);

            return new Channel(freq, channel, major, minor);      
        }

        protected override void SetChannel(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            ITuneRequest tuneRequest;
            int hr = tuningSpace.CreateTuneRequest(out tuneRequest);
            DsError.ThrowExceptionForHR(hr);

            IATSCChannelTuneRequest atscRequest = (IATSCChannelTuneRequest)tuneRequest;

            IATSCLocator locator = (IATSCLocator)new ATSCLocator();
            locator.put_CarrierFrequency(channel.CarrierFrequency);
            locator.put_PhysicalChannel(channel.PhysicalChannel);


            atscRequest.put_Locator(locator);
            atscRequest.put_Channel(channel.MajorChannel);
            atscRequest.put_MinorChannel(channel.MinorChannel);

            hr = tuner.put_TuneRequest(atscRequest);
            DsError.ThrowExceptionForHR(hr);

            ReleaseComObject(tuneRequest);
        }

        public override TVMode TVMode
        {
            get
            {
                return base.TVMode;
            }
            set
            {
                if (value != TVMode.Broadcast)
                {
                    value = TVMode.Broadcast;
                }
                base.TVMode = value;
            }
        }

        public List<Channel> GetVirtualChannels()
        {
            if (State != ServerGraphState.Running)
            {
                throw new InvalidOperationException("Graph must be running to retreive Virtual Channel Table");
            }

            List<Channel> items = new List<Channel>();
            try
            {
                VirtualChannelTable vct = _vctParser.GetVirtualChannelTable();
                foreach (VirtualChannelTable.Entry e in vct.Items)
                {
                    if ((e.ServiceType == VCTServiceTypes.ATSCDigitalTelevision) ||
                        (e.ServiceType == VCTServiceTypes.ATSCAudio))
                    {
                        items.Add(e.ToChannel());
                    }
                }

                return items;
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
                return items;
            }
        }

        protected override Channel MinChannel
        {
            get
            {
                return new Channel(2);
            }
        }

        protected override Channel MaxChannel
        {
            get
            {
                //TODO after Feburary 2009, the max digital channel should be 51.
                return new Channel(69);
            }
        }

        #endregion
    }
}
