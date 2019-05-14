using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using FutureConcepts.Media.Client.InBandData;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;

using LMVCallbackLib;

using GMFBridgeLib;

namespace FutureConcepts.Media.Client.StreamViewer.Graphs
{
    /// <summary>
    /// DirectShow graph used to render LEADTOOLS MPEG2 UDP Source streams
    /// </summary>
    public class MPEG2TSGraph : BaseGraph, ILMVUserCallback
    {
        private IBaseFilter _videoCallbackFilter;
        private ILMVCallback _callback;
        private System.Timers.Timer _intervalTimer;
        private int _lastReceivedFrame;

        /// <summary>
        /// Instantiates a playback and recording graph suitable to view/record the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">URL for the graph to conenct to. Should be in the format "rtsp://"</param>
        public MPEG2TSGraph(string url) : base(url)
        {
            _intervalTimer = new System.Timers.Timer(10000);
            _intervalTimer.Elapsed += new System.Timers.ElapsedEventHandler(IntervalTimer_Elapsed);
        }

        public override void Setup()
        {
            base.Setup();

            //create bridge controller
            _bridgeController = new GMFBridgeController();
            //add a video stream to the bridge
            //1 = Video stream 0 = Audio stream, format type, 1 = dispose stream going into disconnected sink 0 = buffer stream going into disconnected sink
            _bridgeController.AddStream(1, eFormatType.eAny, 1);

            _netSrcMediaType = new AMMediaType();
            _netSrcMediaType.majorType = MediaType.Stream;
//            _netSrcMediaType.subType = new Guid(0x5fe31118, 0x25fb, 0x4d77, 0xbc, 0x62, 0x3d, 0x9c, 0xc3, 0x2b, 0xbe, 0x67);
            _netSrcMediaType.subType = Guid.Empty;
            AddSourceFilter();

            //insert Infinite Pin Tee into playback graph
            _infPinTee = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Infinite Pin Tee Filter");

            //add a bridge sink to the playback graph
            _bridgeSink = (IBaseFilter)_bridgeController.InsertSinkFilter(_graphBuilder);

            _videoCallbackFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD Video Callback Filter (2.0)");
            _callback = (ILMVCallback)_videoCallbackFilter;
            _callback.CallInSameThread = false;
            _callback.ReceiveProcObj = this;

            RenderSource();
        }

        public void IntervalTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            int frame = _callback.CurrentFramePosition;
            if (frame == _lastReceivedFrame)
            {
                // no new frames were received since last interval
                // abort
                NotifyComplete();
                _intervalTimer.Enabled = false;
            }
            _lastReceivedFrame = frame;
        }

        public override void Run()
        {
            base.Run();
            _lastReceivedFrame = -1;
            _intervalTimer.Enabled = true;
        }

        public override void Stop()
        {
            _intervalTimer.Enabled = false;
            base.Run();
        }

        private void AddSourceFilter()
        {
            _netSrc = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD MPEG2 Transport UDP Source");
            _fileSource = (IFileSourceFilter)_netSrc;
            if (_fileSource == null)
            {
                throw new Exception("IFileSourceFilter not found on lmNetSrc");
            }
            LoadNetSrcURL();
        }

        private void RenderSource()
        {
            int hr;

            IEnumPins enumPins;
            IPin[] pins = new IPin[1];

            _muxer = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD MPEG2 Transport Demultiplexer");
            ConnectFilters(_graphBuilder, _netSrc, "Output", _muxer, "Input 01", true);

            hr = _muxer.EnumPins(out enumPins);
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
                                        (mediaType.subType == new Guid("34363248-0000-0010-8000-00aa00389b71")) ||
                                        (mediaType.subType == new Guid("3436324c-0000-0010-8000-00aa00389b71")) ||
                                        (mediaType.subType == new Guid("31637661-5349-4d4f-4544-494154595045")))  // H264
                                    {
                                        _decoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD H264 Decoder (3.0)");
                                        ConnectFilters(_graphBuilder, _muxer, pinInfo.name, _infPinTee, "Input", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output1", _decoderFilter, "XForm In", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output2", _bridgeSink, "Input 1", true);
                                        ConnectFilters(_graphBuilder, _decoderFilter, "XForm Out", _videoCallbackFilter, "Input", true);
                                        ConnectFilters(_graphBuilder, _videoCallbackFilter, "Output", _videoRender, "VMR Input0", true);
                                    }
                                    else if (mediaType.subType == new Guid("4B324A4C-0000-0010-8000-00AA00389B71"))
                                    {
                                        _decoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD MJ2K Decoder (2.0)");
                                        ConnectFilters(_graphBuilder, _muxer, pinInfo.name, _decoderFilter, "Input", true);
                                        ConnectFilters(_graphBuilder, _decoderFilter, "XForm Out", _videoRender, "VMR Input0", true);
                                    }
                                    else if (mediaType.subType == MediaSubType.MJPG)
                                    {
                                        _decoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD MCMP/MJPEG Decoder (2.0)");
                                        ConnectFilters(_graphBuilder, _decoderFilter, "XForm Out", _videoRender, "VMR Input0", true);
                                    }
                                    else
                                    {
                                        throw new Exception("Can't Render--Unsupported codec in stream");
                                    }
                                }
                                else
                                {
//                                    RALPH
//                                    throw new Exception("Can't Render--Unsupported type - audio? not supported");
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

        public void ReceiveProc(int pData, int lWidth, int lHeight, int lBitCount, int lSize, int bTopDown)
        {
        }
    }
}
