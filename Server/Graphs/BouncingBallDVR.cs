using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class BouncingBallDVR: BaseDVRSinkGraph
    {
        private IBaseFilter _videoEncoder;

        public BouncingBallDVR(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            _captureFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "Bouncing Ball");
            _videoEncoder = AddFilterByName(FilterCategory.VideoCompressorCategory, "LEAD H264 Encoder (4.0)");
            ConnectFilters(_captureFilter, "A Bouncing Ball!", _videoEncoder, "XForm In");
            ConnectFilterToMux(_videoEncoder, "XForm Out", "Input 01");
            ConnectMuxToSink();
            InitializeSink();
            this.SaveGraphFile(@"C:\Temp\balldvr.grf");
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
