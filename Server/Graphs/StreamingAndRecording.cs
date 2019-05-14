using System;
using System.Diagnostics;
using System.IO;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.Graphs
{
    public class StreamingAndRecording : StreamingGraph
    {
        // Recording graph

        protected IBaseFilter _tee;
        protected IBaseFilter _recordSink;
        protected IFileSinkFilter _recordSinkFilter;
        private IDVRWriterApi _dvrWriter;
        private int _sequenceNumber;
        private string _filenamePrefix;

        public StreamingAndRecording(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            InitializeNetworkSink();

            int hr;

            _tee = (IBaseFilter)new InfTee();
            hr = _graphBuilder.AddFilter(_tee, "Inf Tee");
            DsError.ThrowExceptionForHR(hr);

            _recordSink = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "DVR Writer");
            _recordSinkFilter = (IFileSinkFilter)_recordSink;
            _recordSinkFilter.SetFileName(@"c:\test.lts", new AMMediaType());
            _dvrWriter = (IDVRWriterApi)_recordSink;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        public int CurrentChunkSize
        {
            get
            {
                int size;
                int hr = _dvrWriter.get_CurrentChunkSize(out size);
                DsError.ThrowExceptionForHR(hr);
                return size;
            }
        }

        public int SequenceNumber
        {
            get
            {
                return _sequenceNumber;
            }
        }

        public void BeginSession()
        {
            _filenamePrefix = @"C:\AntaresXData\Recordings\";
            _filenamePrefix += DateTime.Now.ToString("MMddyy") + @"\";
            Directory.CreateDirectory(_filenamePrefix);
            _filenamePrefix += DateTime.Now.ToString("HHmmss");
            _filenamePrefix += @"_" + SourceConfig.SourceName;
            _sequenceNumber = 0;
        }

        public void StartRecording()
        {
            AppLogger.Message("StreamingAndRecording.StartRecording");
            string filename = _filenamePrefix + @"-" + _sequenceNumber + ".lts";
            _recordSinkFilter.SetFileName(filename, new AMMediaType());
            int hr = _dvrWriter.StartRecording();
            DsError.ThrowExceptionForHR(hr);
            _sequenceNumber++;
        }

        public void StopRecording()
        {
            AppLogger.Message("StreamingAndRecording.StopRecording");
            int hr = _dvrWriter.StopRecording();
            DsError.ThrowExceptionForHR(hr);
        }

        public void EndSession()
        {
            AppLogger.Message("StreamingAndRecording.EndSession");
        }
    }
}
