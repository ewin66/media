using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.DirectShowLib.Framework;

namespace FutureConcepts.Media.Network.Test
{
    class FCNSource2VMR : BaseDSGraph
    {
        private IBaseFilter source;
        private IBaseFilter xform;
        private IBaseFilter dec;
        private IBaseFilter vmr;

        public FCNSource2VMR()
        {
            source = FilterGraphTools.AddFilterByName(graph, FilterCategory.LegacyAmFilterCategory, "FC Network Source");
            xform = FilterGraphTools.AddFilterByName(graph, FilterCategory.LegacyAmFilterCategory, "H.264 Byte Stream Transform");
            dec = FilterGraphTools.AddFilterByName(graph, FilterCategory.LegacyAmFilterCategory, "LEAD H264 Decoder (3.0)");
            vmr = FilterGraphTools.AddFilterByDevicePath(graph, @"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{6BC1CFFA-8FC1-4261-AC22-CFB4CC38DB50}", "VMR7");

            FilterGraphTools.ConnectFilters(graph, source, "Output 1", xform, "XForm In", false);
            FilterGraphTools.ConnectFilters(graph, xform, "XForm Out", dec, "XForm In", false);
            FilterGraphTools.ConnectFilters(graph, dec, "XForm Out", vmr, "VMR Input0", false);
        }

        public override void Dispose()
        {
            source.Release();
            xform.Release();

            base.Dispose();
        }
    }
}
