using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using FutureConcepts.Tools.Utilities;

using LMDVRSinkLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class BaseDVRSinkGraph : BaseGraph
    {
        protected IBaseFilter _mux;
        protected IBaseFilter _sink;
        private ILMDVRSink _sinkControl;

        public static DVRSettings _settings = null;

        public static DVRSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = DVRSettings.LoadFromFile();
                }
                return _settings;
            }
        }

        public BaseDVRSinkGraph(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            _mux = AddFilterByName(FilterCategory.LegacyAmFilterCategory, @"LEAD MPEG2 Transport Multiplexer");
            _sink = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD DVR Sink");
            _sinkControl = (ILMDVRSink)_sink;
        }

        /// <summary>
        /// This method must be called to initialize the network sink. Make sure the SinkURL is properly populated before calling this method.
        /// </summary>
        protected void InitializeSink()
        {
            _sinkControl.StartChangingAttributes();
            IFileSinkFilter fileSink = (IFileSinkFilter)_sink;
            if (fileSink == null)
            {
                throw new Exception("IFileSourceFilter not found on DVR Sink");
            }
            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = DirectShowLib.MediaType.Stream;
            mediaType.subType = MediaSubType.Null;
            String sinkFolderPath = null;
            sinkFolderPath = Settings.RootFolder + SourceConfig.SourceName + @"/";
            if (Directory.Exists(sinkFolderPath) == false)
            {
                Directory.CreateDirectory(sinkFolderPath);
            }
            int hr = fileSink.SetFileName(sinkFolderPath+"Stream.LBL", mediaType);
            DsError.ThrowExceptionForHR(hr);
            _sinkControl.SetBufferSize(0, Settings.NumberOfFiles, Settings.FileSize);
            Debug.WriteLine(String.Format("DVR for {0} is writing to {1}--{2} files with {3} bytes per file", SourceConfig.SourceName,
                sinkFolderPath, Settings.NumberOfFiles, Settings.FileSize));
            _sinkControl.StopChangingAttributes(false);
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sink != null)
                {
                    Marshal.ReleaseComObject(_sink);
                    _sink = null;
                    Marshal.ReleaseComObject(_sinkControl);
                    _sinkControl = null;
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
