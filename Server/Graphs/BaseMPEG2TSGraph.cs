using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using FutureConcepts.Tools.Utilities;

namespace FutureConcepts.Media.Server.Graphs
{
    public class BaseMPEG2TSGraph : BaseGraph
    {
        protected IBaseFilter _mux;
        protected IBaseFilter _sink;

        public BaseMPEG2TSGraph(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            _mux = AddFilterByName(FilterCategory.LegacyAmFilterCategory, @"LEAD MPEG2 Transport Multiplexer");
            _sink = AddFilterByName(FilterCategory.LegacyAmFilterCategory, @"LEAD MPEG2 Transport UDP Sink");
        }

        /// <summary>
        /// This method must be called to initialize the network sink. Make sure the SinkURL is properly populated before calling this method.
        /// </summary>
        protected void InitializeSink()
        {
            IFileSinkFilter fileSink = (IFileSinkFilter)_sink;
            if (fileSink == null)
            {
                throw new Exception("IFileSourceFilter not found on DVR Sink");
            }
            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = DirectShowLib.MediaType.Stream;
            mediaType.subType = MediaSubType.Null;
            int hr = fileSink.SetFileName(SourceConfig.SinkAddress, mediaType);
            DsError.ThrowExceptionForHR(hr);
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sink != null)
                {
                    Marshal.ReleaseComObject(_sink);
                    _sink = null;
                }
                if (_mux != null)
                {
                    Marshal.ReleaseComObject(_mux);
                    _mux = null;
                }
            }
            base.Dispose(disposing);
        }

        public void ConnectFilterToMux(IBaseFilter sourceFilter, string sourcePinName, string muxPinName)
        {
            ConnectFilters(sourceFilter, sourcePinName, _mux, muxPinName);
        }

        public void ConnectMuxTo(IBaseFilter downFilter, string pinName)
        {
            ConnectFilters(_mux, "Output 01", downFilter, pinName);
        }

        public void ConnectMuxToSink()
        {
            ConnectFilters(_mux, "Output 01", _sink, "Input");
        }

        public void ConnectToSink(IBaseFilter upFilter, string pinName)
        {
            ConnectFilters(upFilter, pinName, _sink, "Input");
        }
    }
}
