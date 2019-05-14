using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;

using FutureConcepts.Media.DirectShowLib;

using LMNetDmxLib;
using LMNetSrcLib;
using NETCONLib;
using FutureConcepts.Media.TV;

namespace FutureConcepts.Media.Server.ZViewer
{
    public class Graph : IDisposable
    {
        protected IGraphBuilder _graphBuilder;
        protected ICaptureGraphBuilder2 _captureGraphBuilder;
        protected IMediaControl _mediaControl;
        protected IBaseFilter _netSrc;
        protected IBaseFilter _netDmx;
        protected LMNetDmx _netDmxCtl;
        protected IBaseFilter _videoRender;
        protected IBaseFilter _textOverlayFilter;
        protected Control _hostControl;
        protected ILMNetProtocolManager _ltProtMgr;

        public Graph(string url, Control hostControl)
        {
            int hr = 0;

            _hostControl = hostControl;
            // An exception is thrown if cast fail
            _graphBuilder = (IGraphBuilder)new FilterGraph();
            _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            _mediaControl = (IMediaControl)_graphBuilder;

            // Attach the filter graph to the capture graph
            hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
            DsError.ThrowExceptionForHR(hr);

            AddLeadNetSrc(url);
            AddLeadNetDemux();
            ConnectFilters(_netSrc, "Output", _netDmx, "Input 01", true);
            AddLeadVideoTextOverlay();
            RenderNetDemux();
          
            SaveGraphFile("ZViewer.GRF");
            _readMsgTimer = new System.Timers.Timer(2000);
            _readMsgTimer.Elapsed += new ElapsedEventHandler(readMsgTimer_Elapsed);
        }

        ~Graph()
        {
            Dispose();
        }

        public void Dispose()
        {
            int hr;

            GC.SuppressFinalize(this);
			if( _mediaControl != null )
			{
                // Stop the graph
                hr = _mediaControl.Stop();
                _mediaControl = null;
			}
            if (_graphBuilder != null)
            {
                Marshal.ReleaseComObject(_graphBuilder);
                _graphBuilder = null;
            }
            if (_captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(_captureGraphBuilder);
                _captureGraphBuilder = null;
            }
        }

        private string _recordingFileName = null;

        public string RecordingFileName
        {
            get
            {
                return _recordingFileName;
            }
            set
            {
                _netDmxCtl.OutputFileName = value;
            }
        }

        public void Run()
        {
            int hr;

            if (_mediaControl != null)
            {
                hr = _mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);
                _readMsgTimer.Enabled = true;
            }
        }

        public void Stop()
        {
            int hr;

            if (_mediaControl != null)
            {
                hr = _mediaControl.Stop();
                DsError.ThrowExceptionForHR(hr);
                _readMsgTimer.Enabled = false;
            }
        }

        private void AddLeadNetSrc(string url)
        {
            int hr;

            _netSrc = AddFilterByDevicePath(@"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{E2B7DE03-38C5-11D5-91F6-00104BDB8FF9}", "LEAD Network Source (2.0");
            LMNetSrc lmNetSrc = (LMNetSrc)_netSrc;
            IFileSourceFilter fileSource = (IFileSourceFilter)_netSrc;
            if (fileSource == null)
            {
                throw new Exception("IFileSourceFilter not found on lmNetSrc");
            }
            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = MediaType.Stream;
            mediaType.subType = new Guid("8256426B-28BF-4EBD-8EF4-306913875F34");
            hr = fileSource.Load(url, mediaType);
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

        private void AddLeadVideoTextOverlay()
        {
            int hr;

            //_textOverlayFilter = (IBaseFilter)new LMVTextOverlayClass();
            //hr = _graphBuilder.AddFilter(_textOverlayFilter, "LEAD Video Text Overlay (2.0)");
            //DsError.ThrowExceptionForHR(hr);
            //_textOverlayCtrl = (LMVTextOverlay)_textOverlayFilter;
            //_textOverlayCtrl.ResetToDefaults();
            //_textOverlayCtrl.EnableTextOverLay = false;
            //_textOverlayCtrl.EnableTextFromFile = false;
            //_textOverlayCtrl.OverlayText = "HI DAVE!!!";
            //_textOverlayCtrl.EnableTextOverLay = false;
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
                        Console.WriteLine("rendering pin {0}", pinInfo.name);
                        if (pinInfo.dir == PinDirection.Output)
                        {
                            if (pinInfo.name == "Output 01")
                            {
                              //  FilterGraphTools.ConnectFilters(_graphBuilder, _netDmx, pinInfo.name, _textOverlayFilter, "XForm In", true);
                                FilterGraphTools.RenderPin(_graphBuilder, _netDmx, pinInfo.name);
//                                FilterGraphTools.RenderPin(_graphBuilder, _textOverlayFilter, "XForm Out");
                            }
                            else
                            {
                                FilterGraphTools.RenderPin(_graphBuilder, _netDmx, pinInfo.name);
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

        System.Timers.Timer _readMsgTimer = null;

        public void readMsgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string msg = _netDmxCtl.ReadMessage();
            if (msg != null && msg.Length > 0)
            {
                Console.WriteLine("Demux Received Message: " + msg);
            }
        }

        public IBaseFilter AddFilterByDevicePath(string devicePath, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByDevicePath(_graphBuilder, devicePath, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        public void ConnectFilters(IBaseFilter upFilter, string upPin, IBaseFilter downFilter, string downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(_graphBuilder, upFilter, upPin, downFilter, downPin, useIntelligentConnect);
        }

        private void ConnectFilters(IPin upPin, IPin downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(_graphBuilder, upPin, downPin, useIntelligentConnect);
        }

        public void SaveGraphFile(string filename)
        {
            FilterGraphTools.SaveGraphFile(_graphBuilder, filename);
        }
    }
}
