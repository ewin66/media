using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using FutureConcepts.Controls.AntaresX.AntaresXForms;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.TV.Scanner.Config;
using FutureConcepts.Tools;
using Interop.LMNetDmx2;
using Interop.LMNetSrc2;


namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    public class LTNetSource : BaseGraph, IChannelScanProvider
    {
        public enum ConnectionState { Disconnected, WCFOnly, Full };

        private IBaseFilter _netSrc;
        private IBaseFilter _netDmx;
        private IBaseFilter _videoDecoder;
        private IBaseFilter _audioDecoder;
        private IMediaEvent _mediaEvent;
        private LMNetDmx _netDmxCtl;
        private SessionDescription _sessionInfo;
        private TVStreamControl _graphControlClient;

        private System.Timers.Timer dmxMsgReceiveTimer;

        public LTNetSource(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
            this.Connected = ConnectionState.Disconnected;

            _mediaEvent = (IMediaEvent)_graphBuilder;
            dmxMsgReceiveTimer = new System.Timers.Timer();
            dmxMsgReceiveTimer.Elapsed += new ElapsedEventHandler(dmxMsgReceiveTimer_Elapsed);
            dmxMsgReceiveTimer.Interval = 1000;
        }

        protected override void Dispose(bool disposeManaged)
        {
            if (_eventListenerRunning == true)
            {
                _eventListenerRunning = false;
                while (_eventThread.IsAlive == true)
                {
                    Thread.Sleep(1000);
                }
            }
            if (_graphControlClient != null)
            {
                _graphControlClient.ChannelScanStarted -= new EventHandler(_graphControlClient_ChannelScanStarted);
                _graphControlClient.ChannelScanProgressUpdate -= new EventHandler(_graphControlClient_ChannelScanProgressUpdate);
                _graphControlClient.ChannelScanComplete -= new EventHandler<ChannelScanCompleteEventArgs>(_graphControlClient_ChannelScanComplete);
                _graphControlClient.Closed -= new EventHandler(_graphControlClient_Closed);
                _graphControlClient.Dispose();
                _graphControlClient = null;
            }

            Stop();

            if (_netSrc != null)
            {
                Marshal.ReleaseComObject(_netSrc);
                _netSrc = null;
            }

            this.ConnectionFaulted = null;

            dmxMsgReceiveTimer.Stop();
            dmxMsgReceiveTimer.Dispose();
            dmxMsgReceiveTimer = null;

            base.Dispose(disposeManaged);
        }

        public override void Run()
        {
            base.Run();
            dmxMsgReceiveTimer.Start();
        }

        public override void Stop()
        {
            dmxMsgReceiveTimer.Stop();
            base.Stop();
        }

        public override void Pause()
        {
            dmxMsgReceiveTimer.Stop();
            base.Pause();
        }

        public override TVMode TVMode
        {
            get
            {
                try
                {
                    return _graphControlClient.TVMode;
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                    return TVMode.Broadcast;
                }
            }
            set
            {
                try
                {
                    _graphControlClient.SetTVMode(value);
                }
                catch (Exception ex)
                {
                    FCMessageBox.Show("TV Streamer Unavailable", "The TV Mode could not be set because the TV server could not be contacted.");
                    ErrorLogger.DumpToDebug(ex);
                }
            }
        }

        /// <summary>
        /// Returns the current video profile, or null if the profile is unknown
        /// </summary>
        public Profile CurrentProfile
        {
            get
            {
                if (_graphControlClient != null)
                {
                    if (_graphControlClient.SessionDescription != null)
                    {
                        return _graphControlClient.SessionDescription.CurrentProfile;
                    }
                }
                return null;
            }
        }

        #region Channel Control

        void _graphControlClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Sets the channel on the server, and gets the last reported channel
        /// </summary>
        public override Channel Channel
        {
            set
            {
                try
                {
                    _graphControlClient.SetChannel(value);
                }
                catch(Exception ex)
                {
                    FCMessageBox.Show("TV Streamer Unavailable", "The TV Streamer is currently unavailable");
                    ErrorLogger.DumpToDebug(ex);
                }
            }
            get
            {
                if (_graphControlClient != null)
                {
                    return _graphControlClient.Channel;
                }
                else
                {
                    return null;
                }
            }
        }

        public override void ChannelUp()
        {
            try
            {
                _graphControlClient.ChannelUp();
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("TV Streamer Unavailable", "The TV Streamer is currently unavailable");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        public override void ChannelDown()
        {
            try
            {
                _graphControlClient.ChannelDown();
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("TV Streamer Unavailable", "The TV Streamer is currently unavailable");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #endregion

        public override bool CaptureRequiresNewGraph
        {
            get
            {
                return false;
            }
        }

        private Thread _eventThread;

        public override void Render()
        {
            _graphControlClient = new FutureConcepts.Media.Client.TVStreamControl(AppUser.Current.ServerAddress);
            _graphControlClient.Closed += new EventHandler(_graphControlClient_Closed);

            Message = "Connecting to " + _graphControlClient.ServerAddress + " " + AppUser.Current.ServerSourceName + "...";

            ClientConnectRequest clientRequest = new ClientConnectRequest(AppUser.Current.ServerSourceName);
            _sessionInfo = _graphControlClient.OpenGraph(clientRequest);

            //attach the event listeners only after successful connection
            _graphControlClient.ChannelScanStarted += new EventHandler(_graphControlClient_ChannelScanStarted);
            _graphControlClient.ChannelScanProgressUpdate += new EventHandler(_graphControlClient_ChannelScanProgressUpdate);
            _graphControlClient.ChannelScanComplete += new EventHandler<ChannelScanCompleteEventArgs>(_graphControlClient_ChannelScanComplete);
            _graphControlClient.PropertyChanged += new PropertyChangedEventHandler(_graphControlClient_PropertyChanged);

            this.Connected = ConnectionState.WCFOnly;

            if (_sessionInfo.TVSessionInfo == null)
            {
                throw new Exception("VideoServer Source is not a TV Source");
            }

            if (_sessionInfo.TVSessionInfo.ChannelScanInProgress)
            {
                //TODO...is this the best way?
                //throw new Exception("The TV server is currently in the middle of scanning for channels.");
                return;
            }

            Profile currentProfile = null;
            foreach (ProfileGroup g in _sessionInfo.ProfileGroups.Items)
            {
                foreach (Profile p in g.Items)
                {
                    if (_sessionInfo.CurrentProfileName.Equals(p.Name))
                    {
                        currentProfile = p;
                    }
                }
            }

            if (currentProfile != null)
            {
                //determine if we have a preferred video decoder for the specified Video codec
                if (currentProfile.Video != null)
                {
                    if (currentProfile.Video.CodecType != VideoCodecType.Undefined)
                    {
                        Filter vdFilter = this.GraphConfig.GetFilterForCodec(currentProfile.Video.CodecType, FilterType.VideoDecoder);
                        if (vdFilter != null)
                        {
                            _videoDecoder = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, vdFilter.Name);
                        }
                    }
                }
                //determine if we have a preferred audio decoder for the specified Audio codec
                if (currentProfile.Audio != null)
                {
                    if (currentProfile.Audio.CodecType != AudioCodecType.Undefined)
                    {
                        Filter adFilter = this.GraphConfig.GetFilterForCodec(currentProfile.Audio.CodecType, FilterType.AudioDecoder);
                        if (adFilter != null)
                        {
                            _audioDecoder = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, adFilter.Name);
                        }
                    }
                }
            }

            AddLeadNetSrc();
            AddLeadNetDemux();
            ConnectFilters(_netSrc, "Output", _netDmx, "Input 01", true);

            RenderNetDemux();

            NotifyPropertyChanged("Channel");
            NotifyPropertyChanged("TVMode");

            this.Connected = ConnectionState.Full;

            this.SaveGraphFile();
        }

        void _graphControlClient_Closed(object sender, EventArgs e)
        {
            //make sure the channel scan box is closed
            if (ChannelScanComplete != null)
            {
                ChannelScanComplete.Invoke(this, new ChannelScanCompleteEventArgs(true));
            }

            if ((ConnectionFaulted != null) && (this.Connected != ConnectionState.Disconnected))
            {
                ConnectionFaulted.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Event is raised when the connection to the server fails
        /// </summary>
        public event EventHandler ConnectionFaulted;

        public override void RenderCapture(string fileName)
        {
            if (_netDmxCtl != null)
            {
                _netDmxCtl.OutputFileName = fileName + ".lts";
            }
        }

        public override void StopCapture()
        {
            if (_netDmxCtl != null)
            {
                _netDmxCtl.OutputFileName = null;
            }
        }

        /// <summary>
        /// Fetches the current state of the connection
        /// </summary>
        public ConnectionState Connected { get; private set; }

        private void AddLeadNetSrc()
        {
            int hr;
            string sinkUrl = _sessionInfo.SinkURL;

           // _eventThread = new Thread(new ThreadStart(EventListener));
           // _eventThread.Start();

            _netSrc = AddFilterByDevicePath(@"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{E2B7DE03-38C5-11D5-91F6-00104BDB8FF9}", "LEAD Network Source (2.0");
            LMNetSrc lmNetSrc = (LMNetSrc)_netSrc;
            Message = "Buffering stream " + sinkUrl +"...";
            //lmNetSrc.CheckConnection(sinkUrl, 0, 5000);
            IFileSourceFilter fileSource = (IFileSourceFilter)_netSrc;
            if (fileSource == null)
            {
                throw new Exception("IFileSourceFilter not found on lmNetSrc");
            }
            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = MediaType.Stream;
            mediaType.subType = MediaSubType.LeadToolsStreamFormat;
            hr = fileSource.Load(sinkUrl, mediaType);
            DsError.ThrowExceptionForHR(hr);
        }

        public int AvgBitRate
        {
            get
            {
                return _netDmxCtl.BitRate;
            }
        }

        private void AddLeadNetDemux()
        {
            int hr;

            _netDmx = (IBaseFilter)new LMNetDmx();
            hr = _graphBuilder.AddFilter(_netDmx, "LEAD NetDmx");
            DsError.ThrowExceptionForHR(hr);
            _netDmxCtl = (LMNetDmx)_netDmx;
        }

        private void RenderNetDemux()
        {
            int hr;

            IEnumPins enumPins;
            IPin[] pins = new IPin[1];

            hr = _netDmx.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(hr);
            try
            {
                while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
                {
                    try
                    {
                        PinInfo pinInfo;
                        IPin upstreamPin = pins[0];
                        hr = upstreamPin.QueryPinInfo(out pinInfo);
                        DsError.ThrowExceptionForHR(hr);
                        if (pinInfo.dir == PinDirection.Output)
                        {
                            IEnumMediaTypes enumMediaTypes;
                            hr = upstreamPin.EnumMediaTypes(out enumMediaTypes);
                            DsError.ThrowExceptionForHR(hr);
                            AMMediaType[] mediaTypes = new AMMediaType[1];

                            if (enumMediaTypes.Next(1, mediaTypes, IntPtr.Zero) == 0)
                            {
                                AMMediaType mediaType = mediaTypes[0];
                                if (mediaType.majorType == MediaType.Video)
                                {
                                    hr = _captureGraphBuilder.RenderStream(null, null, upstreamPin, _videoDecoder, _videoRender);
                                }
                                else
                                {
                                    AddAudioVolumeFilter();
                                    AddAudioRenderer();
                                    ConnectToAudio(upstreamPin, true);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(pins[0]);
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(enumPins);
            }
        }

        private bool _eventListenerRunning = false;
        //TODO maybe we can replace this stuff with BaseDSGraph someday
        private void EventListener()
        {
            Debug.WriteLine("EventListener listening");
            _eventListenerRunning = true;

            IntPtr param1, param2;

            while (_eventListenerRunning)
            {
                int hr;
                
                EventCode eventCode;
                hr = _mediaEvent.GetEvent(out eventCode, out param1, out param2, 3000);
                if (hr == 0)
                {
                    Debug.WriteLine("EventListener got " + eventCode.ToString());
                    if (eventCode == EventCode.Complete)
                    {
                        Stop();
                        Run();
                    }
                    _mediaEvent.FreeEventParams(eventCode, param1, param2);
                }
            }

            Debug.WriteLine("EventListener exit");
        }

        /// <summary>
        /// Does the updates for telemetry data
        /// </summary>
        private void dmxMsgReceiveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            dmxMsgReceiveTimer.Stop();

            if (_netDmxCtl == null)
            {
                return;
            }

            string message = null;
            do
            {
                message = _netDmxCtl.ReadMessage();
            /*    if (message != null)
                {
                    XmlReader xml = null;
                    try
                    {
                        Debug.WriteLine("TVGraphs.LTNetSource got LMNetDmx Message");
                        Debug.WriteLine(message);

                        xml = XmlReader.Create(new StringReader(message));
                        if (xml.Read())
                        {
                            if (xml.Read())
                            {
                                if (xml.Name.Equals("Channel") && xml.IsStartElement())
                                {
                                    Channel ch = Client.InBandData.Parser.ReadTVChannel(xml);
                                    if (!ch.Equals(lastReportedChannel))
                                    {
                                        lastReportedChannel = ch;
                                        NotifyPropertyChanged("Channel");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.DumpToDebug(ex);
                    }
                    finally
                    {
                        if (xml != null)
                        {
                            xml.Close();
                        }
                    }
                } */
            } while (message != null);

            if (this.State == GraphState.Running)
            {
                dmxMsgReceiveTimer.Start();
            }
        }

        #region IChannelScanProvider Members

        public void StartChannelScan()
        {
            try
            {
                if (_graphControlClient != null)
                {
                    _graphControlClient.StartChannelScan();
                }
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Could not start channel scan!", "The channel scan could not be started on the remote server.");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        public void CancelChannelScan()
        {
            try
            {
                if (_graphControlClient != null)
                {
                    _graphControlClient.CancelChannelScan();
                }
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Could not cancel channel scan!", "The channel scan could not be canceled on the remote server.");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Raised when the server has started a channel scan.
        /// </summary>
        public event EventHandler ChannelScanStarted;

        /// <summary>
        /// Raised when the server reports a channel scan progress event.
        /// </summary>
        public event EventHandler ChannelScanProgressUpdate;

        /// <summary>
        /// Raised when the server reports a channel scan has completed.
        /// </summary>
        public event EventHandler<ChannelScanCompleteEventArgs> ChannelScanComplete;

        /// <summary>
        /// The current amount of channel scan progress
        /// </summary>
        public int ChannelScanProgress
        {
            get
            {
                if (_graphControlClient != null)
                {
                    return _graphControlClient.ChannelScanProgress;
                }
                return -1;
            }
        }

        void _graphControlClient_ChannelScanStarted(object sender, EventArgs e)
        {
            if (ChannelScanStarted != null)
            {
                ChannelScanStarted.Invoke(this, e);
            }
        }

        void _graphControlClient_ChannelScanProgressUpdate(object sender, EventArgs e)
        {
            if (ChannelScanProgressUpdate != null)
            {
                ChannelScanProgressUpdate.Invoke(this, e);
            }
        }

        void _graphControlClient_ChannelScanComplete(object sender, ChannelScanCompleteEventArgs e)
        {
            if (ChannelScanComplete != null)
            {
                ChannelScanComplete.Invoke(this, e);
            }
        }

        #endregion
    }
}
