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

namespace FutureConcepts.Media.Client.StreamViewer.Graphs
{
    /// <summary>
    /// DirectShow graph used to render graphs using Elecard RTSP source filter
    /// and Microsoft Windows 7 decoder filters
    /// </summary>
    public class ElecardWithMicrosoftRTSP : BaseRTSPGraph
    {
        /// <summary>
        /// Instantiates a playback and recording graph suitable to view/record the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">URL for the graph to conenct to. Should be in the format "rtsp://"</param>
        public ElecardWithMicrosoftRTSP(string url)
            : base(url)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Run()
        {
            base.Run();
        }

        public override void Stop()
        {
            //            base.Run();
            base.Stop();
        }

        protected override void AddRTSPSource()
        {
            _netSrc = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Elecard RTSP NetSource");
            ElecardModuleConfig.IModuleConfig moduleConfig = (ElecardModuleConfig.IModuleConfig)_netSrc;
            Object val = 10;
            moduleConfig.SetValue(FutureConcepts.Media.DirectShowLib.ertspnws.ERTSP_rtp_cache_size, val);
            val = 1500;
            moduleConfig.SetValue(FutureConcepts.Media.DirectShowLib.ertspnws.ERTSP_latency, val);

            Object reason = null;
            moduleConfig.CommitChanges(out reason);
         //   Marshal.ReleaseComObject(moduleConfig);
            _fileSource = (IFileSourceFilter)_netSrc;
            if (_fileSource == null)
            {
                throw new Exception("IFileSourceFilter not found on lmNetSrc");
            }
            LoadNetSrcURL();
        }

        protected override void RenderSource()
        {
            int hr;
            Boolean videoComplete = false;

            IEnumPins enumPins;
            IPin[] pins = new IPin[1];

            hr = _netSrc.EnumPins(out enumPins);
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
                                        (mediaType.subType == new Guid("31637661-5349-4d4f-4544-494154595045")) ||
                                        (mediaType.subType == new Guid("8d2d71cb-243f-45e3-b2d8-5fd7967ec09b")))
                                    {
                                        _decoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Microsoft DTV-DVD Video Decoder");
                                        ConnectFilters(_graphBuilder, _netSrc, pinInfo.name, _infPinTee, "Input", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output1", _decoderFilter, "Video Input", true);
                                        ConnectFilters(_graphBuilder, _infPinTee, "Output2", _bridgeSink, "Input 1", true);
                                        ConnectFilters(_graphBuilder, _decoderFilter, "Video Output 1", _videoRender, "VMR Input0", true);
                                        videoComplete = true;
                                    }
                                    else
                                    {
                                        throw new Exception("Can't Render--Unsupported codec in stream");
                                    }
                                }
                                else if (mediaType.majorType == MediaType.Audio && HasAudio)
                                {
                                    if (mediaType.subType == MediaSubType.PCM)
                                    {
                                        if (mediaType.formatType == FormatType.WaveEx)
                                        {
//                                            _audioEncoderFilter = AddFilterByName(_graphBuilder, FilterCategory.AudioCompressorCategory, "WM Speech Encoder DMO");
//                                            _audioDecoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "WMSpeech Decoder DMO");
//                                            _audioRenderFilter = AddFilterByName(_graphBuilder, FilterCategory.AudioRendererCategory, "Default DirectSound Device");
//                                            ConnectFilters(_graphBuilder, _netSrc, pinInfo.name, _audioEncoderFilter, "in0", true);
//                                            ConnectFilters(_graphBuilder, _audioEncoderFilter, "out0", _audioDecoderFilter, "in0", true);
//                                            ConnectFilters(_graphBuilder, _audioDecoderFilter, "out0", _audioRenderFilter, "Audio Input pin (rendered)", true);
                                        }
                                    }
                                    else if (mediaType.subType == new Guid("000000FF-0000-0010-8000-00AA00389B71")) // AAC Audio
                                    {
                                        _audioDecoderFilter = AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Microsoft DTV-DVD Audio Decoder");
                                        _audioRenderFilter = AddFilterByName(_graphBuilder, FilterCategory.AudioRendererCategory, "Default DirectSound Device");
                                        ConnectFilters(_graphBuilder, _netSrc, pinInfo.name, _audioDecoderFilter, "XForm In", true);
                                        ConnectFilters(_graphBuilder, _audioDecoderFilter, "XFrom Out", _audioRenderFilter, "Audio Input pin (rendered)", true);
                                    }
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
            if (videoComplete == false)
            {
                throw new Exception("No video found");
            }
        }
    }
}
