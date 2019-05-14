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
using GMFBridgeLib;

namespace FutureConcepts.Media.Client.StreamViewer.Graphs
{
    /// <summary>
    /// Base graph class
    /// </summary>
    public abstract class BaseGraph : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Hold this lock while performing any sort of self-modification, especially involving RCW's
        /// </summary>
        protected readonly object instanceMutex = new object();

        protected IGraphBuilder _graphBuilder;
        protected ICaptureGraphBuilder2 _captureGraphBuilder;
        protected IMediaControl _mediaControl;
        protected IBaseFilter _netSrc;
        protected IFileSourceFilter _fileSource;
        protected IBaseFilter _decoderFilter;
        protected IBaseFilter _videoRender;
        protected Control _hostControl;

        // Recording graph

        protected IBaseFilter _fileSink;
        protected IFileSinkFilter _fileSinkFilter;
        protected IBaseFilter _muxer;
        protected IGraphBuilder _fileSinkGraphBuilder;
        protected ICaptureGraphBuilder2 _fileSinkCaptureGraphBuilder;
        protected IMediaControl _fileSinkMediaControl;

        // GMF Bridge

        protected IGMFBridgeController _bridgeController;
        protected IBaseFilter _bridgeSink;
        protected IBaseFilter _bridgeSource;

        /// <summary>
        /// Raised when the graph has received a telemetry information updated
        /// </summary>
        public event EventHandler<XmlTelemetryEventArgs> TelemetryUpdate;

        /// <summary>
        /// Raised when a property of the graph has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the graph has received the EC_COMPLETE message
        /// </summary>
        public event EventHandler Complete;

        protected IBaseFilter _infPinTee;

        protected System.Threading.Timer _telemetryTimer = null;

        protected AMMediaType _netSrcMediaType;

        private Boolean _hasAudio;

        public Boolean HasAudio
        {
            get
            {
                return _hasAudio;
            }
            set
            {
                if (value != _hasAudio)
                {
                    _hasAudio = value;
                    NotifyPropertyChanged("HasAudio");
                }
            }
        }

        /// <summary>
        /// Instantiates a playback and recording graph suitable to view/record the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">URL for the graph to conenct to. Should be in the format "ltsf://"</param>
        public BaseGraph(string url)
        {
            Url = url;

        }

        private String _streamName = "BaseGraph";

        public String StreamName
        {
            get
            {
                return _streamName;
            }
            set
            {
                _streamName = value;
            }
        }

        private void Log(String msg)
        {
            Debug.WriteLine(String.Format("{0} graph: {1}", _streamName, msg));
        }

        public virtual void Setup()
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

                AddVideoRender();
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
        protected void CreateRecordingGraph(string filename)
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

                _muxer = AddFilterByName(_fileSinkGraphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD MPEG2 Multiplexer (2.0)");
                //add the File Writer
                _fileSink = AddFilterByName(_fileSinkGraphBuilder, FilterCategory.LegacyAmFilterCategory, "File writer");
                _fileSinkFilter = (IFileSinkFilter)_fileSink;
                hr = _fileSinkFilter.SetFileName(filename, null);
                DsError.ThrowExceptionForHR(hr);
                ConnectFilters(_fileSinkGraphBuilder, _bridgeSource, "Output 1", _muxer, "Input 01", true);
                ConnectFilters(_fileSinkGraphBuilder, _muxer, "Output 01", _fileSink, "in", false);
            }
        }

        /// <summary>
        ///  stops any current recording and deletes the recording graph
        /// </summary>
        protected void StopAndDeleteRecordingGraph()
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
                    Marshal.FinalReleaseComObject(_bridgeSource);
                    _bridgeSource = null;
                }
                if (_muxer != null)
                {
                    Marshal.FinalReleaseComObject(_muxer);
                    _muxer = null;
                }
                if (_fileSink != null)
                {
                    Marshal.FinalReleaseComObject(_fileSink);
                    _fileSink = null;
                }
                if (_fileSinkGraphBuilder != null)
                {
                    Marshal.FinalReleaseComObject(_fileSinkGraphBuilder);
                    _fileSinkGraphBuilder = null;
                }
                if (_fileSinkCaptureGraphBuilder != null)
                {
                    Marshal.FinalReleaseComObject(_fileSinkCaptureGraphBuilder);
                    _fileSinkCaptureGraphBuilder = null;
                }
            }
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        /// <seealso cref="M:FutureConcepts.Media.Client.StreamViewer.DSGraph.Dispose"/>
        ~BaseGraph()
        {
            Dispose();
        }

        /// <summary>
        /// Destroys the DirectShow graph and releases all resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
            }
            // get rid of unmanaged resources
            lock (instanceMutex)
            {
                //remove event handlers
                TelemetryUpdate = null;
                PropertyChanged = null;
                Complete = null;

                //detach from HWND
                DetachControl();
                if (_netSrc != null)
                {
                    Marshal.FinalReleaseComObject(_netSrc);
                    _netSrc = null;
                }
                if (_decoderFilter != null)
                {
                    Marshal.FinalReleaseComObject(_decoderFilter);
                    _decoderFilter = null;
                }
                if (_telemetryTimer != null)
                {
                    _telemetryTimer.Dispose();
                    _telemetryTimer = null;
                }
                if (_windowlessControl != null)
                {
                    Marshal.FinalReleaseComObject(_windowlessControl);
                    _windowlessControl = null;
                }
                if (_videoRender != null)
                {
                    Marshal.FinalReleaseComObject(_videoRender);
                    _videoRender = null;
                }

                //recording graph Disposal
                if (_infPinTee != null)
                {
                    Marshal.FinalReleaseComObject(_infPinTee);
                    _infPinTee = null;
                }
                if (_fileSink != null)
                {
                    Marshal.FinalReleaseComObject(_fileSink);
                    _fileSink = null;
                }
                if (_bridgeSink != null)
                {
                    Marshal.FinalReleaseComObject(_bridgeSink);
                    _bridgeSink = null;
                }
                if (_bridgeSource != null)
                {
                    Marshal.FinalReleaseComObject(_bridgeSource);
                    _bridgeSource = null;
                }
                if (_bridgeController != null)
                {
                    Marshal.FinalReleaseComObject(_bridgeController);
                    _bridgeController = null;
                }
                if (_fileSinkMediaControl != null)
                {
                    Marshal.FinalReleaseComObject(_fileSinkMediaControl);
                    _fileSinkMediaControl = null;
                }
                if (_fileSinkCaptureGraphBuilder != null)
                {
                    Marshal.FinalReleaseComObject(_fileSinkCaptureGraphBuilder);
                    _fileSinkCaptureGraphBuilder = null;
                }
                if (_fileSinkGraphBuilder != null)
                {
                    Marshal.FinalReleaseComObject(_fileSinkGraphBuilder);
                    _fileSinkGraphBuilder = null;
                }
                if (_fileSinkFilter != null)
                {
                    Marshal.FinalReleaseComObject(_fileSinkFilter);
                    _fileSinkFilter = null;
                }
                if (_muxer != null)
                {
                    Marshal.FinalReleaseComObject(_muxer);
                    _muxer = null;
                }

                if (_captureGraphBuilder != null)
                {
                    Marshal.FinalReleaseComObject(_captureGraphBuilder);
                    _captureGraphBuilder = null;
                }
                if (_graphBuilder != null)
                {
                    Marshal.FinalReleaseComObject(_graphBuilder);    //TODO race on RCW cleanup during app quit
                    _graphBuilder = null;
                }
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

        protected void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        protected void NotifyTelemetryUpdate(String message)
        {
            if (TelemetryUpdate != null)
            {
                TelemetryUpdate(this, new XmlTelemetryEventArgs(XmlReader.Create(new StringReader(message))));
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
        public virtual void Run()
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
                    if (_telemetryTimer != null)
                    {
                        _telemetryTimer.Change(5000, -1);   //invoke timer in 5 sec
                    }

                }
            }
        }

        /// <summary>
        /// Stops the stream.
        /// </summary>
        public virtual void Stop()
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

        protected void NotifyComplete()
        {
            if (Complete != null)
            {
                Complete(this, new EventArgs());
            }
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
                while (mediaEvent.GetEvent(out eventCode, out param1, out param2, 0) >= 0)
                {
                    if (eventCode == EventCode.Complete)
                    {
                        fireComplete = true;
                    }
                    Log(String.Format("IMediaEventEx.GetEvent got {0}", eventCode.ToString()));
                    mediaEvent.FreeEventParams(eventCode, param1, param2);
                    if (fireComplete && Complete != null)
                    {
                        Complete(this, new EventArgs());
                        break;
                    }
                }
            }
        }

        protected void LoadNetSrcURL()
        {
            int hr;

            hr = _fileSource.Load(Url, _netSrcMediaType);
            DsError.ThrowExceptionForHR(hr);
        }

        private int _avgBitRate = 0;
        /// <summary>
        /// Gets the last known average bit rate, in bps. May be 0 if unknown.
        /// </summary>
        public int AvgBitRate
        {
            set
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

        private IVMRWindowlessControl _windowlessControl;

        protected void AddVideoRender()
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
            Log("PaintHandler");
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
                    Log("ResizeMoveHandler");
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
                    Log("DisplayChangedHandler");
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

        protected IBaseFilter AddFilterByDevicePath(IGraphBuilder gb, string devicePath, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByDevicePath(gb, devicePath, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        protected IBaseFilter AddFilterByName(IGraphBuilder gb, Guid filterCategory, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByName(gb, filterCategory, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        protected void ConnectFilters(IGraphBuilder gb, IBaseFilter upFilter, string upPin, IBaseFilter downFilter, string downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(gb, upFilter, upPin, downFilter, downPin, useIntelligentConnect);
        }

        protected void ConnectFilters(IGraphBuilder gb, IPin upPin, IPin downPin, bool useIntelligentConnect)
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
    }
}
