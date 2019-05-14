using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using GMFBridgeLib;
using LMNetDmxLib;

namespace FutureConcepts.Media.Client.StreamViewer.Graphs
{
    /// <summary>
    /// Base graph class
    /// </summary>
    public class LTNetSrcGraph : BaseGraph
    {
        private IBaseFilter _netDmx;
        private LMNetDmx _lmNetDmx;

        /// <summary>
        /// Instantiates a playback and recording graph suitable to view/record the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">URL for the graph to conenct to. Should be in the format "ltsf://"</param>
        public LTNetSrcGraph(string url) : base(url)
        {
        }

        public override void Setup()
        {
            base.Setup();
            lock (instanceMutex)
            {
                //create bridge controller
                _bridgeController = new GMFBridgeController();
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
                RenderNetDemux();

                _telemetryTimer = new System.Threading.Timer(new System.Threading.TimerCallback(TelemetryTimer_Tick));
            }
        }

        /// <summary>
        /// Destroys the DirectShow graph and releases all resources
        /// </summary>
        protected override void Dispose(bool isDisposing)
        {
            lock (instanceMutex)
            {
                if (_netDmx != null)
                {
                    Marshal.ReleaseComObject(_netDmx);
                    _netDmx = null;
                    _lmNetDmx = null;
                }
            }
            base.Dispose(isDisposing);
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
                                    if ((mediaType.subType == new Guid("34363268-0000-0010-8000-00AA00389B71")) ||
                                        (mediaType.subType == new Guid("3436324c-0000-0010-8000-00aa00389b71")) ||
                                        (mediaType.subType == new Guid("31637661-5349-4d4f-4544-494154595045")))  // H264

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

        private void AddLeadNetSrc()
        {
            int hr;

            _netSrcMediaType = new AMMediaType();
            _netSrcMediaType.majorType = MediaType.Stream;
            _netSrcMediaType.subType = MediaSubType.LeadToolsStreamFormat;

            _netSrc = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD Network Source (2.0)");
            _fileSource = (IFileSourceFilter)_netSrc;
            if (_fileSource == null)
            {
                throw new Exception("IFileSourceFilter not found on lmNetSrc");
            }
            LoadNetSrcURL();
        }

        private void AddLeadNetDemux()
        {
            int hr;

            _netDmx = (IBaseFilter)new LMNetDmx();
            hr = _graphBuilder.AddFilter(_netDmx, "LEAD NetDmx");
            DsError.ThrowExceptionForHR(hr);
            _lmNetDmx = (LMNetDmx)_netDmx;
        }

        private void TelemetryTimer_Tick(object sender)
        {
            try
            {
                _telemetryTimer.Change(-1, -1);

                if (_lmNetDmx != null)
                {
                    AvgBitRate = _lmNetDmx.BitRate;
                    string message = null;
                    do
                    {
                        message = _lmNetDmx.ReadMessage();
                        if (message != null)
                        {
                            Debug.WriteLine("DSGraph TelemetryTick_Tick got LMNetDmx Message");
                            Debug.WriteLine(message);
                            NotifyTelemetryUpdate(message);
                        }
                    } while (message != null);
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
