using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;

using LMDVRSourceLib;
using LMMpgDmxTLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class DVRLeadNetSink : StreamingGraph
    {
        private IBaseFilter _demux;
        private ILMMpgDmx _demuxControl;

        public DVRLeadNetSink(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            AppLogger.Message("In DVRNetSink constructor - CurrentProfile is " + CurrentProfile.Name);

            InitializeNetworkSink();

            _captureFilter = DVRGraphHelper.GetAndConfigureDVRSourceForSink(_graphBuilder, sourceConfig);

            _demux = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD MPEG2 Transport Demultiplexer");
            _demuxControl = (ILMMpgDmx)_demux;
            _demuxControl.AutoLive = true;
            _demuxControl.AutoLiveTolerance = .5;

            ConnectFilters(_captureFilter, "Output", _demux, "Input 01");

            ConnectFilterToNetMux(_demux, "H.264 Video", "Input 01");

            ConnectNetMuxToNetSnk();
        }

        public override void Dispose(bool disposing)
        {
            _captureFilter = null;
            if (_demux != null)
            {
                Marshal.ReleaseComObject(_demux);
                _demux = null;
            }
            base.Dispose(disposing);
        }
    }
}
