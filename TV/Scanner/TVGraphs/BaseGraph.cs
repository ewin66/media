using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.TV.Scanner.Config;
using System.Configuration;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    /// <summary>
    /// Base graph class. Handles Video renderer, as well as Audio Volume and Rendering
    /// </summary>
    /// <author>kdixon, darnold</author>
    public abstract class BaseGraph : IDisposable, INotifyPropertyChanged
    {
        #region Protected Memebers

        protected enum GraphState { Stopped, Paused, Running };

        protected IGraphBuilder _graphBuilder;
        protected IMediaControl _mediaControl;
        protected ICaptureGraphBuilder2 _captureGraphBuilder;
        protected IBaseFilter _captureFilter;
        protected IBaseFilter _videoRender;
        protected IBaseFilter _audioRender;
        protected IBaseFilter _fileWriter;
        protected Control _hostControl;
        protected IVMRWindowlessControl _vmrWindowlessControl;

        protected readonly AMMediaType _emptyAMMediaType = new AMMediaType();

        /// <summary>
        /// IDVRWriterApi, if available on writer
        /// </summary>
        protected IDVRWriterApi r_dvrWriter = null;

        private Config.Graph _graphConfig;
        /// <summary>
        /// The configuration used to build this graph
        /// </summary>
        protected Config.Graph GraphConfig
        {
            get { return _graphConfig; }
            set { _graphConfig = value; }
        }

        //this information is for the audio volume filter
        protected IBaseFilter _audioVolumeFilter;
        private IFCVolumeMute _audioVolumeFilterInterface;
        private static readonly string _audioVolumeFilterIn = "XForm In";
        private static readonly string _audioVolumeFilterOut = "XForm Out";

        #endregion

        #region Factory, Constructors, Destructors

        /// <summary>
        /// Factory Method to instantiate the graph specified in the configuration
        /// </summary>
        /// <param name="graph">configuration of graph needed</param>
        /// <param name="hostControl">control that the graph will render to</param>
        /// <returns>an instance of a subclass of BaseGraph</returns>
        public static BaseGraph CreateInstance(Config.Graph graph, Control hostControl)
        {
            switch (graph.Class)
            {
                case Config.GraphType.WinTV:
                    return new WinTV(graph, hostControl);
                case Config.GraphType.Network:
                    return new LTNetSource(graph, hostControl);
                case Config.GraphType.WinTVATSC:
                    return new WinTVATSC(graph, hostControl);
                case Config.GraphType.AverA307Analog:
                    return new AverA307Analog(graph, hostControl);
                case Config.GraphType.AverATSC:
                    return new AverATSC(graph, hostControl);
                case Config.GraphType.AverA317Analog:
                    return new AverA317Analog(graph, hostControl);
                case Config.GraphType.AverA323Analog:
                    return new AverA323Analog(graph, hostControl);
                case Config.GraphType.ATSCTuner:
                    return new ATSCTuner(graph, hostControl);
                default:
                    throw new Exception("Invalid SourceType specified in configuration");
            }
        }

        /// <summary>
        /// Constructor, must be called by sub-classes
        /// </summary>
        /// <remarks>
        /// Takes care of basic stuff, creates the basic DirectShow graph builder and such,
        /// and adds video renderer
        /// </remarks>
        /// <param name="graphConfig">configuration</param>
        /// <param name="hostControl">control that the graph will render to</param>
        protected BaseGraph(Config.Graph graphConfig, Control hostControl)
        {
            int hr;

            _graphConfig = graphConfig;
            _hostControl = hostControl;

            //Create the Graph
            _graphBuilder = (IGraphBuilder)new FilterGraph();
            _mediaControl = (IMediaControl)_graphBuilder;
            Debug.Assert(_mediaControl != null);

            //Create the Capture Graph Builder
            _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

            // Attach the filter graph to the capture graph
            hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
            DsError.ThrowExceptionForHR(hr);

            AddVideoRenderer();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            GC.SuppressFinalize(this);
            Process currentProc = Process.GetCurrentProcess();
            currentProc.PriorityClass = ProcessPriorityClass.Normal;

            _hostControl.Paint -= new PaintEventHandler(PaintHandler);
            _hostControl.Resize -= new EventHandler(ResizeMoveHandler);
            _hostControl.Move -= new EventHandler(ResizeMoveHandler);

            ForceReleaseComObject(_captureFilter);
            _captureFilter = null;
            ForceReleaseComObject(_fileWriter);
            _fileWriter = null;

            ForceReleaseComObject(_mediaControl);
            _mediaControl = null;
            ForceReleaseComObject(_graphBuilder);
            _graphBuilder = null;
            ForceReleaseComObject(_captureGraphBuilder);
            _captureGraphBuilder = null;

            ForceReleaseComObject(_videoRender);
            _videoRender = null;
            ForceReleaseComObject(_audioRender);
            _audioRender = null;
            ForceReleaseComObject(_audioVolumeFilter);
            _audioVolumeFilter = null;
            _audioVolumeFilterInterface = null;

            ForceReleaseComObject(_vmrWindowlessControl);
            _vmrWindowlessControl = null;

            DsUtils.FreeAMMediaType(_emptyAMMediaType);
        }

        /// <summary>
        /// Disposes a COM object
        /// </summary>
        /// <remarks>
        /// calls Marshal.ReleaseComObject and then sets the reference to null
        /// </remarks>
        /// <param name="o">COM object to dispose</param>
        protected virtual void ReleaseComObject(object o)
        {
            if (o != null)
            {
                int refcount = Marshal.ReleaseComObject(o);
                if (refcount <= 0)
                {
                    o = null;
                }
            }
        }

        /// <summary>
        /// Releases an object until it's reference count is 0
        /// </summary>
        /// <param name="o">COM object to forcefully release</param>
        protected virtual void ForceReleaseComObject(object o)
        {
            if (o != null)
            {
                while (Marshal.ReleaseComObject(o) > 0) ;
            }
            o = null;
        }

        ~BaseGraph()
        {
            Dispose(false);
        }

        #endregion

        #region PUBLIC - Render and Capture API

        /// <summary>
        /// Used to select the TVMode / Input
        /// </summary>
        public abstract TVMode TVMode { get; set; }

        private string _message;
        /// <summary>
        /// Used to forward progress messages to the UI. Use only during calls to Render and RenderCapture
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            protected set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        public abstract void Render();

        /// <summary>
        /// Returns true if transitioning from playback to capture (to disk) will require
        /// the construction of a new graph.
        /// If FALSE is returned, then the Engine will call Stop, RenderCapture, and Start on the current
        /// instance of the graph.
        /// If TRUE is returned, the Engine will instantiate a new instance of the graph, then call RenderCapture instead of Render
        /// </summary>
        /// <remarks>
        /// Default implementation returns true, must be overridden if you want different behavior.
        /// </remarks>
        public virtual bool CaptureRequiresNewGraph
        {
            get
            {
                return true;
            }
            protected set { }
        }

        /// <summary>
        /// Makes any neccesary changes to the graph to perform capture
        /// </summary>
        /// <param name="fileName">file name to save to</param>
        public abstract void RenderCapture(string fileName);

        /// <summary>
        /// Called when capture should be stopped.
        /// </summary>
        public virtual void StopCapture()
        {
            StopWhenReady();
        }

        #endregion

        #region PUBLIC - Audio Control API

        private int _minVolume;
        /// <summary>
        /// Retreives the minimum volume level supported by this graph
        /// </summary>
        public int MinVolume
        {
            get
            {
                return _minVolume;
            }
            protected set
            {
                _minVolume = value;
            }
        }

        private int _maxVolume;
        /// <summary>
        /// Retreives the maximum volume level supported by this graph
        /// </summary>
        public int MaxVolume
        {
            get
            {
                return _maxVolume;
            }
            protected set
            {
                _maxVolume = value;
            }
        }

        /// <summary>
        /// Gets or sets the current audio volume
        /// </summary>
        public virtual int Volume
        {
            get
            {
                if (_audioVolumeFilterInterface != null)
                {
                    int level;
                    _audioVolumeFilterInterface.get_Volume(out level);
                    return level;
                }

                return MaxVolume;
            }
            set
            {
                if (_audioVolumeFilterInterface != null)
                {
                    if ((value >= MinVolume) && (value <= MaxVolume))
                    {
                        _audioVolumeFilterInterface.set_Volume(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mute state of the graph
        /// </summary>
        public virtual bool Mute
        {
            get
            {
                if (_audioVolumeFilterInterface != null)
                {
                    bool muted;
                    _audioVolumeFilterInterface.get_Mute(out muted);
                    return muted;
                }

                return false;
            }
            set
            {
                if (_audioVolumeFilterInterface != null)
                {
                    _audioVolumeFilterInterface.set_Mute(value);
                }
            }
        }

        #endregion

        #region PUBLIC - Channel Control API

        public abstract Channel Channel
        {
            set;
            get;
        }

        public abstract void ChannelUp();

        public abstract void ChannelDown();

        private bool _channelForceChange;
        public bool ChannelForceChange
        {
            get { return _channelForceChange; }
            set { _channelForceChange = value; }
        }

        #endregion

        #region PROTECTED - Audio Signal Chain API

        /// <summary>
        /// Call this method to insert the Audio Volume filter into your graph. Input pin is "In" and output pin is "Out"
        /// </summary>
        protected void AddAudioVolumeFilter()
        {
            _audioVolumeFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, @"FC Volume Filter");
            _audioVolumeFilterInterface = (IFCVolumeMute)_audioVolumeFilter;

            MaxVolume = 0;
            MinVolume = -40;
        }

        /// <summary>
        /// Inserts the Default Audio Renderer into the graph
        /// </summary>
        protected void AddAudioRenderer()
        {
            _audioRender = (IBaseFilter)new AudioRender();
            int hr = _graphBuilder.AddFilter(_audioRender, "Default Audio Renderer");
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Connects the last filter in the audio chain to the audio volume filter (if AddAudioVolumeFilter has been called), and then
        /// to the audio renderer
        /// </summary>
        /// <remarks>
        /// First call AddAudioVolumeFilter() and AddAudioRenderer()
        /// </remarks>
        /// <param name="audioFilter">a filter in the graph that will be delivering the audio</param>
        /// <param name="audioOutPin">the pin on said filter that the audio stream comes from</param>
        /// <param name="useIntelligentConnect">true to use intelligent connect, false otherwise</param>
        protected void ConnectToAudio(IBaseFilter audioFilter, string audioOutPin, bool useIntelligentConnect)
        {
            IPin theAudioOutPin = null;
            try
            {
                theAudioOutPin = DsFindPin.ByName(audioFilter, audioOutPin);
                ConnectToAudio(theAudioOutPin, useIntelligentConnect);
            }
            finally
            {
                theAudioOutPin.Release();
            }
        }

        /// <summary>
        /// Connects the last filter in the audio chain to the audio volume filter (if AddAudioVolumeFilter has been called), and then
        /// to the audio renderer
        /// </summary>
        /// <remarks>
        /// First call AddAudioRenderer()
        /// </remarks>
        /// <param name="audioOutPin">the audio output pin</param>
        /// <param name="useIntelligentConnect">true to use intelligent connect, false to not</param>
        protected void ConnectToAudio(IPin audioOutPin, bool useIntelligentConnect)
        {
            IPin connectToRenderPin;

            if (_audioVolumeFilter != null)
            {
                IPin avfIn = DsFindPin.ByName(_audioVolumeFilter, _audioVolumeFilterIn);
                ConnectFilters(audioOutPin, avfIn, useIntelligentConnect);
                connectToRenderPin = DsFindPin.ByName(_audioVolumeFilter, _audioVolumeFilterOut);
            }
            else
            {
                connectToRenderPin = audioOutPin;
            }

            IPin renderInputPin = null;
            try
            {
                renderInputPin = DsFindPin.ByDirection(_audioRender, PinDirection.Input, 0);
                if (renderInputPin != null)
                {
                    FilterGraphTools.ConnectFilters(_graphBuilder, connectToRenderPin, renderInputPin, useIntelligentConnect);
                }
            }
            finally
            {
                ReleaseComObject(renderInputPin);
            }
        }

        #endregion

        #region PROTECTED - DVR Writer API

        /// <summary>
        /// start DVR Writer filter
        /// </summary>
        /// <param name="fileWriterFilter">fileWriterFilter to control -- sets r_dvrWriter variable</param>
        protected void StartDVRWriter(IBaseFilter fileWriterFilter)
        {
            r_dvrWriter = fileWriterFilter as IDVRWriterApi;
            StartDVRWriter();
        }

        /// <summary>
        /// Start DVR Writer filter
        /// </summary>
        protected void StartDVRWriter()
        {
            if (r_dvrWriter != null)
            {
                int hr = r_dvrWriter.StartRecording();
                DsError.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Stop DVR Writer filter
        /// </summary>
        protected void StopDVRWriter()
        {
            if (r_dvrWriter != null)
            {
                int hr = r_dvrWriter.StopRecording();
                DsError.ThrowExceptionForHR(hr);
            }
        }

        #endregion

        #region PRIVATE - Video Signal Chain / Callbacks

        private void AddVideoRenderer()
        {
            int hr;

            _videoRender = (IBaseFilter)new VideoMixingRenderer();
            Debug.Assert(_videoRender != null);
            hr = _graphBuilder.AddFilter(_videoRender, "VMR7");
            DsError.ThrowExceptionForHR(hr);

            // Configure the video renderer

            if (_hostControl != null)
            {
                IVMRFilterConfig vmrFilterConfig = (IVMRFilterConfig)_videoRender;
                hr = vmrFilterConfig.SetRenderingMode(VMRMode.Windowless);
                DsError.ThrowExceptionForHR(hr);
                _vmrWindowlessControl = (IVMRWindowlessControl)_videoRender;
                hr = _vmrWindowlessControl.SetAspectRatioMode(VMRAspectRatioMode.LetterBox);
                DsError.ThrowExceptionForHR(hr);
                hr = _vmrWindowlessControl.SetVideoClippingWindow(_hostControl.Handle);
                DsError.ThrowExceptionForHR(hr);
                ResizeMoveHandler(null, null);
                _hostControl.Paint += new PaintEventHandler(PaintHandler);
                _hostControl.Resize += new EventHandler(ResizeMoveHandler);
                _hostControl.Move += new EventHandler(ResizeMoveHandler);
            }
            else
            {
                throw new Exception("Host control not specified");
            }
        }

        private void PaintHandler(object sender, PaintEventArgs e)
        {
            if (_videoRender != null)
            {
                IntPtr hdc = e.Graphics.GetHdc();
                _vmrWindowlessControl.RepaintVideo(_hostControl.Handle, hdc);
                e.Graphics.ReleaseHdc(hdc);
            }
        }

        private void ResizeMoveHandler(object sender, EventArgs e)
        {
            if (_videoRender != null)
            {
                _vmrWindowlessControl.SetVideoPosition(null, DsRect.FromRectangle(_hostControl.ClientRectangle));
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string info)
        {
            
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        #region PUBLIC - Transport Control API

        /// <summary>
        /// Set or Get the state of the graph.
        /// </summary>
        protected virtual GraphState State
        {
            get;
            set;
        }

        public virtual void Run()
        {
            int hr = _mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
            Process currentProc = Process.GetCurrentProcess();
            currentProc.PriorityClass = ProcessPriorityClass.AboveNormal;
            State = GraphState.Running;
        }

        public virtual void Pause()
        {
            _mediaControl.Pause();
            State = GraphState.Paused;
        }

        public virtual void Stop()
        {
            _mediaControl.Stop();
            State = GraphState.Stopped;
        }

        protected void StopWhenReady()
        {
            _mediaControl.StopWhenReady();
            State = GraphState.Stopped;
        }

        #endregion

        #region PUBLIC - Debug / Utility

        /// <summary>
        /// Saves the current graph to a file in the output directory.
        /// This behavior can be enabled or disabled by the appSettings OutputDebugGRF key
        /// </summary>
        public void SaveGraphFile()
        {
            bool output;
            if (bool.TryParse(ConfigurationManager.AppSettings["OutputDebugGRF"], out output))
            {
                if (output)
                {
                    if (!System.IO.Directory.Exists(AppUser.TVScannerOutputRoot))
                    {
                        System.IO.Directory.CreateDirectory(AppUser.TVScannerOutputRoot);
                    }
                    FilterGraphTools.SaveGraphFile(_graphBuilder, AppUser.TVScannerOutputRoot + this.GetType().Name + ".grf");
                }
            }
        }

        public void SaveGraphFile(string filename)
        {
            FilterGraphTools.SaveGraphFile(_graphBuilder, filename);
        }

        #endregion

        #region PUBLIC Snapshot API

        /// <summary>
        /// Retreives a current snapshot from the stream.
        /// </summary>
        /// <returns>
        /// Returns a Snapshot. Caller must dispose of it.
        /// If no snapshot could be obtained, returns null
        /// </returns>
        public virtual Snapshot CreateSnapshot()
        {
            try
            {
                return new Snapshot(_vmrWindowlessControl);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region PROTECTED - Graph Construction API

        protected IBaseFilter AddFilterByDevicePath(string devicePath, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByDevicePath(_graphBuilder, devicePath, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        /// <summary>
        /// Finds a pin on the selected filter
        /// </summary>
        /// <param name="filter">filter to search</param>
        /// <param name="direction">direction the pin is in</param>
        /// <param name="major">major media type</param>
        /// <param name="sub">sub media type</param>
        /// <returns>matching IPin. Caller must release</returns>
        private IPin FindPinWithMediaType(IBaseFilter filter, PinDirection direction, Guid major, Guid sub)
        {
            List<DetailPinInfo> details = null;
            IPin preConnectedTo = null;
            try
            {
                details = filter.EnumPinsDetails();
                foreach (DetailPinInfo pin in details)
                {
                    if ((pin.Info.dir == direction) && (pin.Type != null))
                    {
                        if ((pin.Type.majorType == major) && (pin.Type.subType == sub))
                        {
                            //check if the pin we found matching is already connected
                            if (pin.Pin.ConnectedTo(out preConnectedTo) == 0)
                            {
                                if (preConnectedTo != null)
                                {
                                    preConnectedTo.Release();
                                    preConnectedTo = null;
                                    continue;
                                }
                            }

                            //make sure that releasing the details doesn't release the IPin
                            IPin toReturn = pin.Pin;
                            pin.Pin = null;
                            return toReturn;
                        }
                    }
                }

                throw new Exception("No pin found with major=" + major.ToString() + " and sub=" + sub.ToString() + ", or pin already connected!");

            }
            finally
            {
                details.Release();
            }
        }

        /// <summary>
        /// Connects an upstream filter, with a pin matching a specific media type, to the named pin on the downstream filter.
        /// Finds the 
        /// </summary>
        /// <param name="upFilter">upstream filter</param>
        /// <param name="upPinMajor">upstream pin major media type</param>
        /// <param name="upPinSub">upstream pin sub media type</param>
        /// <param name="downFilter">downstream filter</param>
        /// <param name="downPinName">downstream pin name</param>
        /// <param name="useIntelligentConnect">
        /// TRUE to use intelligent connect inserting filters if needed
        /// FALSE to directly connect filters
        /// </param>
        protected void ConnectFilters(IBaseFilter upFilter, Guid upPinMajor, Guid upPinSub, IBaseFilter downFilter, string downPinName, bool useIntelligentConnect)
        {          
            IPin downstreamPin = null;
            IPin upstreamPin = null;

            try
            {
                try
                {
                    upstreamPin = FindPinWithMediaType(upFilter, PinDirection.Output, upPinMajor, upPinSub);
                }
                catch (Exception ex)
                {
                    throw new Exception("Upstream filter has no such pin!", ex);
                }

                downFilter.FindPin(downPinName, out downstreamPin);
                if (downstreamPin == null)
                {
                    throw new Exception("Downstream filter has no pin \"" + downPinName + "\"!");
                }

                ConnectFilters(upstreamPin, downstreamPin, useIntelligentConnect);
            }
            finally
            {
                upstreamPin.Release();
                downstreamPin.Release();
            }
        }

        /// <summary>
        /// Connects two filters' pins together based on the media types specified
        /// </summary>
        /// <param name="upFilter">upstream filter</param>
        /// <param name="upMajor">upstream pin major media type</param>
        /// <param name="upSub">upstream pin sub media type</param>
        /// <param name="downFilter">downstream filter</param>
        /// <param name="downMajor">downstream pin major media type</param>
        /// <param name="downSub">downstream pin minor media type</param>
        /// <param name="useIntelligentConnect">
        /// TRUE to use intelligent connect inserting filters if needed
        /// FALSE to directly connect filters
        /// </param>
        protected void ConnectFilters(IBaseFilter upFilter, Guid upMajor, Guid upSub, IBaseFilter downFilter, Guid downMajor, Guid downSub, bool useIntelligentConnect)
        {
            IPin downstreamPin = null;
            IPin upstreamPin = null;

            try
            {
                try
                {
                    upstreamPin = FindPinWithMediaType(upFilter, PinDirection.Output, upMajor, upSub);
                }
                catch (Exception ex)
                {
                    throw new Exception("Upstream filter has no such pin", ex);
                }

                try
                {
                    downstreamPin = FindPinWithMediaType(downFilter, PinDirection.Input, downMajor, downSub);
                }
                catch (Exception ex)
                {
                    throw new Exception("Downstream filter has no such pin", ex);
                }

                ConnectFilters(upstreamPin, downstreamPin, useIntelligentConnect);
            }
            finally
            {
                downstreamPin.Release();
                upstreamPin.Release();
            }
        }

        protected void ConnectFilters(IBaseFilter upFilter, string upPin, IBaseFilter downFilter, string downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(_graphBuilder, upFilter, upPin, downFilter, downPin, useIntelligentConnect);
        }

        protected void ConnectFilters(IPin upPin, IPin downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(_graphBuilder, upPin, downPin, useIntelligentConnect);
        }

        #endregion
    }
}
