using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.DirectShowLib.BDA;
using FutureConcepts.Media.TV.AtscPsip;
using FutureConcepts.Media.TV.Scanner.Config;
using FutureConcepts.Tools;
using Interop.AudioDecoderL;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    /// <summary>
    /// Concrete BDA Graph can tune ATSC for pretty much any tuner, which also supports recording the TS to disk.
    /// Card-specific inhertances are needed only to control specific things like oddities in the VirtualChannelTable or Signal Strength
    /// </summary>
    public class ATSCTuner : DigitalTuner 
    {
        //these filters r_ for recording use
        private IBaseFilter r_videoTee;
        private IBaseFilter r_audioTee;
        private IBaseFilter r_oggMux;
        private IMLAudioDecoder mediaLooksAudioDecoder = null;

        public ATSCTuner(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
            tunerFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDASourceFiltersCategory, GraphConfig[FilterType.Tuner].Name);
            captureFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDAReceiverComponentsCategory, GraphConfig[FilterType.Capture].Name);

            r_videoTee = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Infinite Pin Tee Filter");
            r_audioTee = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Infinite Pin Tee Filter");

            if (audioDecoder != null)
            {
                mediaLooksAudioDecoder = audioDecoder as IMLAudioDecoder;
                if (mediaLooksAudioDecoder != null)
                {
                    Debug.WriteLine("MediaLooks Audio Decoder detected! Setting Pass-through Input Time Stamps = true");
                    mediaLooksAudioDecoder.UseInputTimeStamps(1);
                }

                AddAudioVolumeFilter();
                AddAudioRenderer();
            }

            if ((AppUser.Current.ChannelIndex >= 0) && (AppUser.Current.ChannelIndex < this.KnownChannels.Count))
            {
                this.StartupChannel = this.KnownChannels.Items[AppUser.Current.ChannelIndex];
            }
        }

        /// <summary>
        /// Instantiate this guy by passing the *connected* mpeg2data filter to it in your Render method
        /// </summary>
        protected AtscPsip.Parser _parser;

        protected override ITuningSpace GetTuningSpace()
        {
            ITuningSpaceContainer system = (ITuningSpaceContainer)new SystemTuningSpaces();

            IntPtr retreivedSpaceCount = Marshal.AllocCoTaskMem(4);
            try
            {
                IEnumTuningSpaces systemEnum;
                int hr = system.get_EnumTuningSpaces(out systemEnum);
                DsError.ThrowExceptionForHR(hr);

                ITuningSpace[] retreivedSpaces = new ITuningSpace[1];
                do
                {
                    hr = systemEnum.Next(1, retreivedSpaces, retreivedSpaceCount);
                    DsError.ThrowExceptionForHR(hr);

                    if (Marshal.ReadInt32(retreivedSpaceCount) > 0)
                    {
                        Guid networkType;
                        hr = retreivedSpaces[0].get__NetworkType(out networkType);
                        //string name;
                        //retreivedSpaces[0].get_FriendlyName(out name);
                        //Debug.WriteLine("  found \"" + name + "\", network type: " + networkType.ToString());
                        DsError.ThrowExceptionForHR(hr);
                        if (networkType.Equals(CLSID_ATSCNetworkProvider))
                        {
                            return retreivedSpaces[0];
                        }
                    }
                } while (Marshal.ReadInt32(retreivedSpaceCount) > 0);

                throw new Exception("Could not locate ATSC Tuning Space!");
            }
            finally
            {
                ReleaseComObject(system);
                Marshal.FreeCoTaskMem(retreivedSpaceCount);
            }
        }

        protected override void Dispose(bool disposeManaged)
        {
            ReleaseComObject(mediaLooksAudioDecoder);
            ReleaseComObject(r_oggMux);
            ReleaseComObject(r_videoTee);
            ReleaseComObject(r_audioTee);

            base.Dispose(disposeManaged);
        }

        /// <summary>
        /// Builds the graph for normal playback
        /// </summary>
        public override void Render()
        {
            try
            {
                //connect network provider to BDA tuner
                ConnectFilters(networkProvider, "Antenna Out", (IBaseFilter)tunerFilter, "Input0", false);
                ConnectFilters((IBaseFilter)tunerFilter, "MPEG2 Transport", captureFilter, "MPEG2 Transport", false);

                //These filters need to be connected via their explicit pins,
                //  because Hauppauge saw fit to name all the pins on the capture filter the same name.
                IPin captureFilterOut = DsFindPin.ByDirection(captureFilter, PinDirection.Output, 0);
                IPin mpeg2DemuxIn = DsFindPin.ByDirection(mpeg2Demux, PinDirection.Input, 0);
                ConnectFilters(captureFilterOut, mpeg2DemuxIn, false);
                //clean up the two pins
                ReleaseComObject(captureFilterOut);
                ReleaseComObject(mpeg2DemuxIn);

                //add and connect meta data filters and connect them to the Demux
                //ConnectFilters(mpeg2Demux, "1", bdaTransportInfo, "Input", false);
                ConnectFilters(mpeg2Demux, MediaType.Mpeg2Sections, MediaSubType.AtscSI,
                               bdaTransportInfo, MediaType.Mpeg2Sections, MediaSubType.AtscSI,
                               false);
                //ConnectFilters(mpeg2Demux, "5", mpeg2SectionsAndTables, "In", false);
                ConnectFilters(mpeg2Demux, MediaType.Mpeg2Sections, MediaSubType.Mpeg2Data,
                               mpeg2SectionsAndTables, "In", false);

                //this interface must be queried after the S&T filter has been connected.
                mpeg2Data = (IMpeg2Data)mpeg2SectionsAndTables;
                _parser = new Parser(mpeg2Data);

                //ConnectFilters(mpeg2Demux, "2", r_videoTee, "Input", false);
                ConnectFilters(mpeg2Demux, MediaType.Video, MediaSubType.Mpeg2Video, r_videoTee, "Input", false);
                ConnectFilters(r_videoTee, VideoTeeOutputName, videoDecoder, GraphConfig[FilterType.VideoDecoder].InPin[0], false);
                ConnectFilters(videoDecoder, GraphConfig[FilterType.VideoDecoder].OutPin[0], _videoRender, "VMR Input0", false);

                //ConnectFilters(mpeg2Demux, "3", r_audioTee, "Input", false);
                ConnectFilters(mpeg2Demux, MediaType.Audio, MediaSubType.DolbyAC3, r_audioTee, "Input", false);

                if (audioDecoder != null)
                {
                    ConnectFilters(r_audioTee, AudioTeeOutputName, audioDecoder, GraphConfig[FilterType.AudioDecoder].InPin[0], false);
                    ConnectToAudio(audioDecoder, GraphConfig[FilterType.AudioDecoder].OutPin[0], false);
                }

                base.Render();
            }
            finally
            {
                this.SaveGraphFile();
            }
        }

        #region Render Capture

        private int r_videoTee_outputCount = 0;
        /// <summary>
        /// Returns the next Video Tee's output name
        /// </summary>
        private string VideoTeeOutputName
        {
            get
            {
                r_videoTee_outputCount++;
                return "Output" + r_videoTee_outputCount.ToString();
            }
        }

        private int r_audioTee_outputCount = 0;
        /// <summary>
        /// Returns the next Audio Tee's output name
        /// </summary>
        private string AudioTeeOutputName
        {
            get
            {
                r_audioTee_outputCount++;
                return "Output" + r_audioTee_outputCount.ToString();
            }
        }

        public override void RenderCapture(string fileName)
        {
            //add new filters
            r_oggMux = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD Ogg Multiplexer");
            if (r_oggMux == null)
            {
                throw new Exception("Could not instantiate OGG Multiplexer!");
            }
            _fileWriter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, GraphConfig[FilterType.Writer].Name);
            if (_fileWriter == null)
            {
                throw new Exception("Could not instatiate \"" + GraphConfig[FilterType.Writer].Name + "\"!");
            }

            //and path to OGG mux
            ConnectFilters(r_videoTee, VideoTeeOutputName, r_oggMux, "Stream 0", false);

            //path to OGG mux
            ConnectFilters(r_audioTee, AudioTeeOutputName, r_oggMux, "Stream 1", false);

            //configure File Sink / File Writer
            IFileSinkFilter fileSink = (IFileSinkFilter)_fileWriter;
            int hr = fileSink.SetFileName(fileName + ".ogm", _emptyAMMediaType);
            if (hr != 0)
            {
                this.CaptureRequiresNewGraph = true;
            }

            DsError.ThrowExceptionForHR(hr);

            StartDVRWriter(_fileWriter);


            ConnectFilters(r_oggMux, "Ogg Stream", _fileWriter, GraphConfig[FilterType.Writer].InPin[0], false);

            CaptureRequiresNewGraph = false;

            this.SaveGraphFile();
        }

        public override void StopCapture()
        {
            try
            {
                StopWhenReady();
                int hr = 0;
                StopDVRWriter();

                DsError.ThrowExceptionForHR(hr);

                hr = _graphBuilder.RemoveFilter(r_oggMux);
                DsError.ThrowExceptionForHR(hr);
                ForceReleaseComObject(r_oggMux);

                hr = _graphBuilder.RemoveFilter(_fileWriter);
                r_dvrWriter = null;
                DsError.ThrowExceptionForHR(hr);
                ForceReleaseComObject(_fileWriter);
            }
            catch (Exception ex)
            {
                this.CaptureRequiresNewGraph = true;
                throw ex;
            }
        }

        private bool _captureRequiresNewGraph = false;
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

        #endregion

        #region Channel Implementation

        /// <summary>
        /// Maximum ATSC Channel
        /// </summary>
        public override Channel MaxChannel
        {
            get
            {
                //TODO after June 2009, the max digital channel should be 51.
                return new Channel(69);
            }
        }

        public override TVMode TVMode
        {
            get
            {
                return TVMode.Broadcast;
            }
            set
            {
            }
        }

        public override Channel Channel
        {
            get
            {
                return GetCurrentChannel();
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                //if the carrier frequency is already defined, tune directly.
                //   This allows Favorite Channels that are no longer known to be tuned.
                if (value.CarrierFrequency != -1)
                {
                    SetCurrentChannel(value);
                }
                else
                {
                    Channel closest = KnownChannels.FindClosest(value);
                    if (closest != null)
                    {
                        SetCurrentChannel(closest);
                        //TODO parse VCT and update if needed
                    }
                    else
                    {
                        return;
                    }
                }

                NotifyPropertyChanged("Channel");
            }
        }

        /// <summary>
        /// Tries to Set the indicated channel. Gets the actually set channel. Returns null if no Major channel was found on this physical channel.
        /// </summary>
        public override Channel TryChannel
        {
            set
            {
                SetCurrentChannel(value);
            }
            get
            {
                Channel real = GetCurrentChannel();
                if (real.MajorChannel == -1)
                {
                    return null;
                }
                else
                {
                    return real;
                }
            }
        }

        private bool channelSet = false;

        protected virtual Channel GetCurrentChannel()
        {
            if (!channelSet || (this.State != GraphState.Running))
            {
                return new Channel();
            }

       //     Debug.WriteLine("null -> get_TuneRequest");
            ITuneRequest request;
            int hr = tuner.get_TuneRequest(out request);
     //       Debug.WriteLine(hr.ToString("X8"));
            DsError.ThrowExceptionForHR(hr);

         //   Debug.WriteLine("ITuneRequest -> IATSCChannelTuneRequest");
            IATSCChannelTuneRequest atscRequest = (IATSCChannelTuneRequest)request;
            ILocator locator;
      //      Debug.WriteLine("null -> get_Locator");
            hr = atscRequest.get_Locator(out locator);
       //     Debug.WriteLine(hr.ToString("X8"));
            DsError.ThrowExceptionForHR(hr);
      //      Debug.WriteLine("ILocator -> IATSCLocator");
            IATSCLocator atscLocator = (IATSCLocator)locator;

            int freq, channel, major, minor;
            hr = atscLocator.get_CarrierFrequency(out freq);
            hr = atscLocator.get_PhysicalChannel(out channel);
            hr = atscRequest.get_Channel(out major);
            hr = atscRequest.get_MinorChannel(out minor);

            Channel known = KnownChannels.FindClosest(new Channel(-1, -1, major, minor));

            if (known != null)
            {
                if ((known.MajorChannel == major) &&
                    (known.MinorChannel == minor))
                {
                    return known;
                }
            }

            return new Channel(freq, channel, major, minor);
        }

        protected virtual void SetCurrentChannel(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            if (this.State != GraphState.Running) return;

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

            channelSet = true;
        }

        /// <summary>
        /// Retrieves all of the virtual channels for the currently tuned physical channel
        /// </summary>
        /// <returns></returns>
        public override List<Channel> GetVirtualChannels()
        {
            if (State != GraphState.Running)
            {
                throw new InvalidOperationException("Graph must be running to retreive Virtual Channel Table");
            }

            List<Channel> items = new List<Channel>();
            try
            {
                VirtualChannelTable vct = _parser.GetVirtualChannelTable();
                if (vct.Items.Count > 0)
                {
                    Channel physical = this.GetCurrentChannel();

                    foreach (VirtualChannelTable.Entry e in vct.Items)
                    {
                        if ((e.ServiceType == VCTServiceTypes.ATSCDigitalTelevision) ||
                            (e.ServiceType == VCTServiceTypes.ATSCAudio))
                        {
                            Channel ch = e.ToChannel();
                            ch.PhysicalChannel = physical.PhysicalChannel;
                            ch.CarrierFrequency = physical.CarrierFrequency;
                            items.Add(ch);
                        }
                    }
                }

                return items;
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                return items;
            }
        }

        #endregion
    }
}
