using System;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class ScreenCapture: BaseDVRSinkGraph
    {
        private IBaseFilter _videoEncoder = null;

        public ScreenCapture(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            InitializeSink();

            _captureFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD Screen Capture (2.0)");
            if (_captureFilter == null)
            {
                throw new Exception("screen capture filter not found");
            }
            _videoEncoder = AddFilterByName(FilterCategory.VideoCompressorCategory, "LEAD Screen Capture Encoder (2.0)");
            ConnectFilters(_captureFilter, "Capture", _videoEncoder, "XForm In");
            ConnectFilterToMux(_videoEncoder, "XForm Out", "Input 01");
            ConnectMuxToSink();
        }
    }
}
