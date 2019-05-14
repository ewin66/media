using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.DirectShowFilters.FilterTestbed
{
    public class FVDO2FCNSink : BaseGraph
    {
        private IBaseFilter fastVDO;
        private IBaseFilter fcsink;

        public FVDO2FCNSink()
        {
            fastVDO = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "FastVDO-SmartCapture QBoxSplitter");
            fcsink = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "FC Network Sink");

            FilterGraphTools.ConnectFilters(_graphBuilder, fastVDO, "Video", fcsink, "Input 1", false);
        }

        public override void Dispose()
        {
            ReleaseComObject(fcsink);
            ReleaseComObject(fastVDO);

            base.Dispose();
        }
    }
}
