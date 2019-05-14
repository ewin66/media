using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.DirectShowFilters.FilterTestbed
{
    class FCNSource2VMR : BaseGraph
    {
        private IBaseFilter source;
        private IBaseFilter xform;
        private IBaseFilter dec;
        private IBaseFilter vmr;

        public FCNSource2VMR()
        {
            source = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "FC Network Source");
            xform = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "H.264 Byte Stream Transform");
            dec = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD H264 Decoder (3.0)");
            vmr = FilterGraphTools.AddFilterByDevicePath(_graphBuilder, @"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{6BC1CFFA-8FC1-4261-AC22-CFB4CC38DB50}", "VMR7");

            FilterGraphTools.ConnectFilters(_graphBuilder, source, "Output 1", xform, "XForm In", false);
            FilterGraphTools.ConnectFilters(_graphBuilder, xform, "XForm Out", dec, "XForm In", false);
            FilterGraphTools.ConnectFilters(_graphBuilder, dec, "XForm Out", vmr, "VMR Input0", false);
        }

        public override void Dispose()
        {
            ReleaseComObject(source);
            ReleaseComObject(xform);

            base.Dispose();
        }
    }
}
