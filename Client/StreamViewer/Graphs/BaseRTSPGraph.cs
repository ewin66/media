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
    /// DirectShow graph used to render LEADTOOLS streams
    /// </summary>
    public abstract class BaseRTSPGraph : BaseGraph
    {
        protected IBaseFilter _audioDecoderFilter;
        protected IBaseFilter _audioResamplerFilter;
        protected IBaseFilter _audioRenderFilter;

        /// <summary>
        /// Instantiates a playback and recording graph suitable to view/record the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">URL for the graph to conenct to. Should be in the format "rtsp://"</param>
        public BaseRTSPGraph(string url)
            : base(url)
        {
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
            AddRTSPSource();

            //insert Infinite Pin Tee into playback graph
            _infPinTee = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Infinite Pin Tee Filter");

            //add a bridge sink to the playback graph
            _bridgeSink = (IBaseFilter)_bridgeController.InsertSinkFilter(_graphBuilder);

            RenderSource();
        }

        protected abstract void AddRTSPSource();

        protected abstract void RenderSource();

        protected override void Dispose(bool disposing)
        {
            if (_audioDecoderFilter != null)
            {
                Marshal.FinalReleaseComObject(_audioDecoderFilter);
                _audioDecoderFilter = null;
            }
            if (_audioResamplerFilter != null)
            {
                Marshal.FinalReleaseComObject(_audioResamplerFilter);
                _audioResamplerFilter = null;
            }
            if (_audioRenderFilter != null)
            {
                Marshal.FinalReleaseComObject(_audioRenderFilter);
                _audioRenderFilter = null;
            }
            base.Dispose(disposing);
        }

        protected void RenderAudio(PinInfo pinInfo, AMMediaType mediaType)
        {
            int hr;

            try
            {
                if (mediaType.subType == MediaSubType.PCM)
                {
                    if (mediaType.formatType == FormatType.WaveEx)
                    {
                        _audioResamplerFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Elecard Audio Resampler");
                        _audioRenderFilter = AddFilterByName(_graphBuilder, FilterCategory.AudioRendererCategory, "Default DirectSound Device");
                        ConnectFilters(_graphBuilder, _netSrc, pinInfo.name, _audioResamplerFilter, "Input", true);
                        ConnectFilters(_graphBuilder, _audioResamplerFilter, "Output", _audioRenderFilter, "Audio Input pin (rendered)", true);
                    }
                }
                else if (mediaType.subType == new Guid("000000FF-0000-0010-8000-00AA00389B71")) // AAC Audio
                {
                    _audioDecoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD AAC Decoder");
                    _audioRenderFilter = AddFilterByName(_graphBuilder, FilterCategory.AudioRendererCategory, "Default DirectSound Device");
                    ConnectFilters(_graphBuilder, _netSrc, pinInfo.name, _audioDecoderFilter, "XForm In", true);
                    ConnectFilters(_graphBuilder, _audioDecoderFilter, "XForm Out", _audioRenderFilter, "Audio Input pin (rendered)", true);
                }
            }
            finally
            {
            }
        }
    }
}
