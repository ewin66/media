using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using FutureConcepts.Media.Client.InBandData;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using Interop.GMFBridge;
using Interop.LMNetDmx2;
using Interop.LMNetSrc2;
using System.Threading;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// DirectShow graph used to render LEADTOOLS streams
    /// </summary>
    public class DSGraph : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Hold this lock while performing any sort of self-modification, especially involving RCW's
        /// </summary>
        private readonly object instanceMutex = new object();

        private IGraphBuilder _graphBuilder;
        private ICaptureGraphBuilder2 _captureGraphBuilder;
        private IMediaControl _mediaControl;
        private IBaseFilter _netSrc;
        private IFileSourceFilter _fileSource;
        private IBaseFilter _netDmx;
        private IBaseFilter _decoderFilter;
        private IBaseFilter _videoRender;
        private Control _hostControl;

        // Recording graph

        private IBaseFilter _fileSink;
        private IFileSinkFilter _fileSinkFilter;
        private IBaseFilter _muxer;
        private IGraphBuilder _fileSinkGraphBuilder;
        private ICaptureGraphBuilder2 _fileSinkCaptureGraphBuilder;
        private IMediaControl _fileSinkMediaControl;

        // GMF Bridge

        private IGMFBridgeController _bridgeController;
        private IBaseFilter _bridgeSink;
        private IBaseFilter _bridgeSource;

        /// <summary>
        /// Raised when a property of the graph has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raised when the graph has received a telemetry information updated
        /// </summary>
        public event EventHandler<XmlTelemetryEventArgs> TelemetryUpdate;
        /// <summary>
        /// Raised when the graph has received the EC_COMPLETE message
        /// </summary>
        public event EventHandler Complete;

        private IBaseFilter _infPinTee;

        private System.Threading.Timer _telemetryTimer = null;

        /// <summary>
        /// Instantiates a playback and recording graph suitable to view/record the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">URL for the graph to conenct to. Should be in the format "ltsf://"</param>
        public DSGraph(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Builds the actual DirectShow graph and performs the connection to the URL specified in the constructor.
        /// </summary>
        public void Setup()
        {
            lock (instanceMutex)
            {
                int hr;

                // An exception is thrown if cast fail

                //create playback graph
                _graphBuilder = (IGraphBuilder)new FilterGraph();
                _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                _mediaControl = (IMediaControl)_graphBuilder;

                // Attach the filter graph to the capture graph
                hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
                DsError.ThrowExceptionForHR(hr);

                //create bridge controller
                _bridgeController = new GMFBridgeControllerClass();
                //add a video stream to the bridge
                //1 = Video stream 0 = Audio stream, format type, 1 = dispose stream going into disconnected sink 0 = buffer stream going into disconnected sink
                _bridgeController.AddStream(1, eFormatType.eAny, 1);

                AddLeadNetSrc();
                AddLeadNetDemux();
                ConnectFilters(_graphBuilder, _netSrc, "Output", _netDmx, "Input 01", true);

                //insert Infinite Pin Tee into playback graph
                _infPinTee = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Infinite Pin Tee Filter");

                //add a bridge sink to the playback graph
                _bridgeSink = (IBaseFilter)_bridgeController.InsertSinkFilter(_graphBuilder);

                //finish building playback/source graph, so we can negotiate data types on bridge
                AddVideoRender();
                RenderNetDemux();

                _telemetryTimer = new System.Threading.Timer(new System.Threading.TimerCallback(TelemetryTimer_Tick));
            }
        }

        /// <summary>
        /// Builds a recording graph that saves the incoming H264 stream to an Ogg container to add
        /// support for seeking.
        /// </summary>
        /// <param name="filename">file to record to</param>
        /// <remarks>
        /// Since this recording graph does not recompress or decompress any data, it performs its job
        /// with little overhead.
        /// </remarks>
        private void CreateRecordingGraph(string filename)
        {
            lock (instanceMutex)
            {
                //create file recording graph
                _fileSinkGraphBuilder = (IGraphBuilder)new FilterGraph();
                _fileSinkCaptureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                _fileSinkMediaControl = (IMediaControl)_fileSinkGraphBuilder;

                //attach filter graph to capture graph, for recording
                int hr = _fileSinkCaptureGraphBuilder.SetFiltergraph(_fileSinkGraphBuilder);
                DsError.ThrowExceptionForHR(hr);

                //add bridge source to the recording graph, linked to the bridge sink
                _bridgeSource = (IBaseFilter)_bridgeController.InsertSourceFilter(_bridgeSink, _fileSinkGraphBuilder);

                _muxer = AddFilterByName(_fileSinkGraphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD Ogg Multiplexer");

                //add the File Writer
                _fileSink = AddFilterByName(_fileSinkGraphBuilder, FilterCategory.LegacyAmFilterCategory, "File writer");
                _fileSinkFilter = (IFileSinkFilter)_fileSink;
                hr = _fileSinkFilter.SetFileName(filename, null);
                DsError.ThrowExceptionForHR(hr);

                ConnectFilters(_fileSinkGraphBuilder, _bridgeSource, "Output 1", _muxer, "Stream 0", true);
                ConnectFilters(_fileSinkGraphBuilder, _muxer, "Ogg Stream", _fileSink, "in", false);
            }
        }

        /// <summary>
        ///  stops any current recording and deletes the recording graph
        /// </summary>
        private void StopAndDeleteRecordingGraph()
        {
            lock (instanceMutex)
            {
                if (_fileSinkMediaControl != null)
                {
                    _fileSinkMediaControl.StopWhenReady();
                }
                if (_bridgeController != null)
                {
                    _bridgeController.BridgeGraphs(null, null);
                }
                if (_bridgeSource != null)
                {
                    Marshal.ReleaseComObject(_bridgeSource);
                    _bridgeSource = null;
                }
                if (_muxer != null)
                {
                    Marshal.ReleaseComObject(_muxer);
                    _muxer = null;
                }
                if (_fileSink != null)
                {
                    Marshal.ReleaseComObject(_fileSink);
                    _fileSink = null;
                }
                if (_fileSinkGraphBuilder != null)
                {
                    Marshal.ReleaseComObject(_fileSinkGraphBuilder);
                    _fileSinkGraphBuilder = null;
                }
                if (_fileSinkCaptureGraphBuilder != null)
                {
                    Marshal.ReleaseComObject(_fileSinkCaptureGraphBuilder);
                    _fileSinkCaptureGraphBuilder = null;
                }
            }
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        /// <seealso cref="M:FutureConcepts.Media.Client.StreamViewer.DSGraph.Dispose"/>
        ~DSGraph()
        {
            Dispose();
        }

        /// <summary>
        /// Destroys the DirectShow graph and releases all resources
        /// </summary>
        public void Dispose()
        {
            lock (instanceMutex)
            {
                GC.SuppressFinalize(this);

                //remove event handlers
                PropertyChanged = null;
                Complete = null;
                TelemetryUpdate = null;

                //detach from HWND
                DetachControl();

                if (_netSrc != null)
                {
                    Marshal.ReleaseComObject(_netSrc);
                    Marshal.ReleaseComObject(_netSrc);
                    Marshal.ReleaseComObject(_netSrc);
                    Marshal.ReleaseComObject(_netSrc);
                    _netSrc = null;
                }
                if (_decoderFilter != null)
                {
                    Marshal.ReleaseComObject(_decoderFilter);
                    _decoderFilter = null;
                }
                if (_telemetryTimer != null)
                {
                    _telemetryTimer.Dispose();
                    _telemetryTimer = null;
                }
                if (_netDmx != null)
                {
                    Marshal.ReleaseComObject(_netDmx);
                    _netDmx = null;
                    _lmNetDmx = null;
                }
                if (_windowlessControl != null)
                {
                    Marshal.ReleaseComObject(_windowlessControl);
                    _windowlessControl = null;
                }
                if (_videoRender != null)
                {
                    Marshal.ReleaseComObject(_videoRender);
                    _videoRender = null;
                }

                //recording graph Disposal
                if (_infPinTee != null)
                {
                    Marshal.ReleaseComObject(_infPinTee);
                    _infPinTee = null;
                }
                if (_fileSink != null)
                {
                    Marshal.ReleaseComObject(_fileSink);
                    _fileSink = null;
                }
                if (_bridgeSink != null)
                {
                    Marshal.ReleaseComObject(_bridgeSink);
                    _bridgeSink = null;
                }
                if (_bridgeSource != null)
                {
                    Marshal.ReleaseComObject(_bridgeSource);
                    _bridgeSource = null;
                }
                if (_bridgeController != null)
                {
                    Marshal.ReleaseComObject(_bridgeController);
                    _bridgeController = null;
                }
                if (_fileSinkMediaControl != null)
                {
                    Marshal.ReleaseComObject(_fileSinkMediaControl);
                    _fileSinkMediaControl = null;
                }
                if (_fileSinkCaptureGraphBuilder != null)
                {
                    Marshal.ReleaseComObject(_fileSinkCaptureGraphBuilder);
                    _fileSinkCaptureGraphBuilder = null;
                }
                if (_fileSinkGraphBuilder != null)
                {
                    Marshal.ReleaseComObject(_fileSinkGraphBuilder);
                    _fileSinkGraphBuilder = null;
                }
                if (_fileSinkFilter != null)
                {
                    Marshal.ReleaseComObject(_fileSinkFilter);
                    _fileSinkFilter = null;
                }
                if (_muxer != null)
                {
                    Marshal.ReleaseComObject(_muxer);
                    _muxer = null;
                }

                if (_captureGraphBuilder != null)
                {
                    Marshal.ReleaseComObject(_captureGraphBuilder);
                    _captureGraphBuilder = null;
                }
                if (_graphBuilder != null)
                {
                    Marshal.ReleaseComObject(_graphBuilder);    //TODO race on RCW cleanup during app quit
                    _graphBuilder = null;
                }
            }
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
                                    if ((mediaType.subType == new Guid("34363268-0000-0010-8000-00AA00389B71")) || // H264
                                        (mediaType.subType == new Guid("3436324c-0000-0010-8000-00aa00389b71")))
                                    {
                                        _decoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD H264 Decoder (3.0)");
                                        ConnectFilters(_graphBuilder, _netDmx, pinInfo.name, _infPinTee, "Input", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output1", _decoderFilter, "XForm In", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output2", _bridgeSink, "Input 1", true);
                                        ConnectFilters(_graphBuilder, _decoderFilter, "XForm Out", _videoRender, "VMR Input0", true);
                                    }
                                    else if (mediaType.subType == new Guid("4B324A4C-0000-0010-8000-00AA00389B71"))
                                    {
                                        _decoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD MJ2K Decoder (2.0)");
                                        ConnectFilters(_graphBuilder, _netDmx, pinInfo.name, _infPinTee, "Input", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output1", _decoderFilter, "XForm In", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output2", _bridgeSink, "Input 1", true);
                                        ConnectFilters(_graphBuilder, _decoderFilter, "XForm Out", _videoRender, "VMR Input0", true);
                                    }
                                    else if (mediaType.subType == MediaSubType.MJPG)
                                    {
                                        _decoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD MCMP/MJPEG Decoder (2.0)");
                                        ConnectFilters(_graphBuilder, _netDmx, pinInfo.name, _infPinTee, "Input", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output1", _decoderFilter, "XForm In", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output2", _bridgeSink, "Input 1", true);
                                        ConnectFilters(_graphBuilder, _decoderFilter, "XForm Out", _videoRender, "VMR Input0", true);
                                    }
                                    else
                                    {
                                        throw new Exception("Can't Render--Unsupported codec in stream");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Can't Render--Unsupported type - audio? not supported");
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

        private string _url;
        /// <summary>
        /// Gets or sets the URL to connect to. You cannot change this after <see cref="M:FutureConcepts.Media.Client.StreamViewer.DSGraph.Setup"/> has been called.
        /// </summary>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        /// <summary>
        /// Setting this property to a non-null string begins recording to that file
        /// Setting this property to null stops recording
        /// </summary>
        public string RecordingFileName
        {
            set
            {
                lock (instanceMutex)
                {
                    //under normal circumstances, use H264 in an Ogg container
                    if (value != null)
                    {
                        CreateRecordingGraph(value);

                        int hr = _fileSinkMediaControl.Run();
                        DsError.ThrowExceptionForHR(hr);

                        _bridgeController.BridgeGraphs(_bridgeSink, _bridgeSource);
                    }
                    else
                    {
                        StopAndDeleteRecordingGraph();
                    }
                }
            }
            get
            {
                if (_fileSink != null)
                {
                    IFileSinkFilter fileSink = (IFileSinkFilter)_fileSink;
                    string outFilename = "";
                    fileSink.GetCurFile(out outFilename, null);
                    return outFilename;
                }
                return null;
            }
        }

        /// <summary>
        /// Pauses playback of the stream. Generally not recommended to use.
        /// </summary>
        public void Pause()
        {
            lock (instanceMutex)
            {
                int hr;

                if (_mediaControl != null)
                {
                    if (_fileSinkMediaControl != null)
                    {
                        _fileSinkMediaControl.Pause();
                    }
                    hr = _mediaControl.Pause();
                    DsError.ThrowExceptionForHR(hr);
                }
            }
        }

        private bool _resetRequiredNextRun = false;

        /// <summary>
        /// Starts the stream (runs the graph)
        /// </summary>
        public void Run()
        {
            lock (instanceMutex)
            {
                int hr;

                if (_mediaControl != null)
                {
                    if (_resetRequiredNextRun)
                    {
                        LoadNetSrcURL();
                        _mediaControl.Stop();
                    }
                    hr = _mediaControl.Run();
                    DsError.ThrowExceptionForHR(hr);
                    _telemetryTimer.Change(5000, -1);   //invoke timer in 5 sec

                }
            }
        }

        /// <summary>
        /// Stops the stream.
        /// </summary>
        public void Stop()
        {
            lock (instanceMutex)
            {
                int hr;

                if (_mediaControl != null)
                {
                    hr = _mediaControl.Stop();
                    DsError.ThrowExceptionForHR(hr);
                    _resetRequiredNextRun = true;

                }
                if (_telemetryTimer != null)
                {
                    _telemetryTimer.Change(-1, -1); //stop timer
                }
            }
        }

        /// <summary>
        /// Used to bind the directshow graph to a message loop for notifications
        /// </summary>
        /// <param name="control">control to bind to</param>
        /// <param name="msg">message to listen to</param>
        public void SetNotifyWindow(Control control, int msg)
        {
            IMediaEventEx mediaEvent = (IMediaEventEx)_graphBuilder;
            int hr = mediaEvent.SetNotifyWindow(control.Handle, msg, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Called by the owning HWND when an event the graph cares about occurs
        /// </summary>
        public void NotifyMediaEvent()
        {
            IMediaEventEx mediaEvent = (IMediaEventEx)_graphBuilder;
            if (mediaEvent != null)
            {
                bool fireComplete = false;
                EventCode eventCode;
                IntPtr param1;
                IntPtr param2;
                int hr = mediaEvent.GetEvent(out eventCode, out param1, out param2, -1);
                if (hr == 0)
                {
                    if (eventCode == EventCode.Complete)
                    {
                        fireComplete = true;
                    }
                    mediaEvent.FreeEventParams(eventCode, param1, param2);
                }
                Debug.WriteLine("GetEvent got " + eventCode.ToString());
                if (fireComplete && Complete != null)
                {
                    Complete(this, new EventArgs());
                }
            }
        }

        private void AddLeadNetSrc()
        {
            int hr;

            _netSrc = (IBaseFilter)new LMNetSrcClass();
            hr = _graphBuilder.AddFilter(_netSrc, "LEAD Network Source (2.0)");
            DsError.ThrowExceptionForHR(hr);
            _fileSource = (IFileSourceFilter)_netSrc;
            if (_fileSource == null)
            {
                throw new Exception("IFileSourceFilter not found on lmNetSrc");
            }
            LoadNetSrcURL();
        }

        private void LoadNetSrcURL()
        {
            int hr;

            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = MediaType.Stream;
            mediaType.subType = MediaSubType.LeadToolsStreamFormat;
            hr = _fileSource.Load(Url, mediaType);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(mediaType);
        }

        private LMNetDmx _lmNetDmx;

        private int _avgBitRate = 0;
        /// <summary>
        /// Gets the last known average bit rate, in bps. May be 0 if unknown.
        /// </summary>
        public int AvgBitRate
        {
            private set
            {
                if (value != _avgBitRate)
                {
                    _avgBitRate = value;
                    NotifyPropertyChanged("AvgBitRate");
                }
            }
            get
            {
                return _avgBitRate;
            }
        }

        private void AddLeadNetDemux()
        {
            int hr;

            _netDmx = (IBaseFilter)new LMNetDmx();
            hr = _graphBuilder.AddFilter(_netDmx, "LEAD NetDmx");
            DsError.ThrowExceptionForHR(hr);
            _lmNetDmx = (LMNetDmx)_netDmx;
        }

        private IVMRWindowlessControl _windowlessControl;

        private void AddVideoRender()
        {
            int hr;

            _videoRender = (IBaseFilter)new VideoMixingRenderer();
            hr = _graphBuilder.AddFilter(_videoRender, "VMR");
            DsError.ThrowExceptionForHR(hr);

            // Configure the video renderer

            IVMRFilterConfig vmrFilterConfig = (IVMRFilterConfig)_videoRender;
            hr = vmrFilterConfig.SetRenderingMode(VMRMode.Windowless);
            DsError.ThrowExceptionForHR(hr);
            _windowlessControl = (IVMRWindowlessControl)_videoRender;
            hr = _windowlessControl.SetAspectRatioMode(VMRAspectRatioMode.LetterBox);
            DsError.ThrowExceptionForHR(hr);
            NotifyPropertyChanged("VideoRender");
        }

        /// <summary>
        /// Binds a control to the VMR. This will be the surface the VMR renders onto.
        /// </summary>
        /// <param name="hostControl">control to render on</param>
        /// <seealso cref="M:FutureConcepts.Media.Client.StreamViewer.DSGraph.DetachControl"/>
        public void AttachControl(Control hostControl)
        {
            int hr;

            // Configure the video renderer

            if (hostControl != null)
            {
                _hostControl = hostControl;
                _windowlessControl = (IVMRWindowlessControl)_videoRender;
                hr = _windowlessControl.SetVideoClippingWindow(hostControl.Handle);
                DsError.ThrowExceptionForHR(hr);
                ResizeMoveHandler(null, null);
                _hostControl.Paint += new PaintEventHandler(PaintHandler);
                _hostControl.Resize += new EventHandler(ResizeMoveHandler);
                _hostControl.Move += new EventHandler(ResizeMoveHandler);
                Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(DisplayChangedHandler); // for WM_DISPLAYCHANGE
            }
        }

        /// <summary>
        /// Detaches all event listeners from the host control.
        /// </summary>
        /// <seealso cref="M:FutureConcepts.Media.Client.StreamViewer.DSGraph.AttachControl"/>
        public void DetachControl()
        {
            if (_hostControl != null)
            {
                _hostControl.Paint -= new PaintEventHandler(PaintHandler);
                _hostControl.Resize -= new EventHandler(ResizeMoveHandler);
                _hostControl.Move -= new EventHandler(ResizeMoveHandler);
            }
            _hostControl = null;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= new EventHandler(DisplayChangedHandler);
        }

        /// <summary>
        /// Captures the current frame
        /// </summary>
        /// <returns>an instance of the <see cref="FutureConcepts.Media.Client.StreamViewer.Snapshot"/> class containing the current frame.</returns>
        public Snapshot CreateSnapshot()
        {
            return new Snapshot(_windowlessControl);
        }

        private void PaintHandler(object sender, PaintEventArgs e)
        {
            Debug.WriteLine("PaintHandler");
            try
            {
                lock (instanceMutex)
                {
                    if (_windowlessControl != null)
                    {
                        Graphics hostControlGraphics = _hostControl.CreateGraphics();
                        IntPtr hdc = hostControlGraphics.GetHdc();
                        _windowlessControl.RepaintVideo(_hostControl.Handle, hdc);
                        hostControlGraphics.ReleaseHdc(hdc);
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
            }
        }

        private void ResizeMoveHandler(object sender, EventArgs e)
        {
            try
            {
                lock (instanceMutex)
                {
                    Debug.WriteLine("ResizeMoveHandler");
                    if (_windowlessControl != null)
                    {
                        _windowlessControl.SetVideoPosition(null, DsRect.FromRectangle(_hostControl.ClientRectangle));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void DisplayChangedHandler(object sender, EventArgs e)
        {
            try
            {
                lock (instanceMutex)
                {
                    Debug.WriteLine("DisplayChangedHandler");
                    if (_windowlessControl != null)
                    {
                        _windowlessControl.DisplayModeChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private IBaseFilter AddFilterByDevicePath(IGraphBuilder gb, string devicePath, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByDevicePath(gb, devicePath, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }
        
        private IBaseFilter AddFilterByName(IGraphBuilder gb, Guid filterCategory, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByName(gb, filterCategory, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        private void ConnectFilters(IGraphBuilder gb, IBaseFilter upFilter, string upPin, IBaseFilter downFilter, string downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(gb, upFilter, upPin, downFilter, downPin, useIntelligentConnect);
        }

        private void ConnectFilters(IGraphBuilder gb, IPin upPin, IPin downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(gb, upPin, downPin, useIntelligentConnect);
        }

        /// <summary>
        /// Writes the currently constructed DirectShow graph to a .GRF file
        /// </summary>
        /// <param name="filename">file to write to</param>
        public void SaveGraphFile(string filename)
        {
            FilterGraphTools.SaveGraphFile(_graphBuilder, filename);
        }

        private void TelemetryTimer_Tick(object sender)
        {
            try
            {
                _telemetryTimer.Change(-1, -1);

                if (_lmNetDmx != null)
                {
                    AvgBitRate = _lmNetDmx.BitRate;
                    if (TelemetryUpdate != null)
                    {
                        string message = null;
                        do
                        {
                            message = _lmNetDmx.ReadMessage();
                            if (message != null)
                            {
                                Debug.WriteLine("DSGraph TelemetryTick_Tick got LMNetDmx Message");
                                Debug.WriteLine(message);
                                TelemetryUpdate(this, new XmlTelemetryEventArgs(XmlReader.Create(new StringReader(message))));
                            }
                        } while (message != null);
                    }
                }

                _telemetryTimer.Change(5000, -1);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }
    }
}
