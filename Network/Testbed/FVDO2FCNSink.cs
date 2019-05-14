using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.Network.FCRTPLib;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using FutureConcepts.Media.DirectShowLib.Framework;

namespace FutureConcepts.Media.Network.Test
{
    public class FVDO2FCNSink : BaseDSGraph
    {
        private IBaseFilter fastVDO;
        private IBaseFilter fcsink;

        public FVDO2FCNSink()
        {
            fastVDO = FilterGraphTools.AddFilterByName(graph, FilterCategory.LegacyAmFilterCategory, "FastVDO-SmartCapture QBoxSplitter");
            fcsink = FilterGraphTools.AddFilterByName(graph, FilterCategory.LegacyAmFilterCategory, "FC Network Sink");

            FilterGraphTools.ConnectFilters(graph, fastVDO, "Video", fcsink, "Input 1", false);

            FCNSinkProxy p = new FCNSinkProxy(fcsink);


            UInt16 basePort = 5000;
            for (int i = 0; i < p.StreamCount; i++)
            {
                p.Interface.SetPortAllocation(i, basePort, 0);
                basePort += 2;
            }
            
            List<RTPStreamDescription> streams = p.Streams;

            //System.Diagnostics.Debug.WriteLine("streams.Count");

            //XmlSerializer s = new XmlSerializer(typeof(RTPStreamDescription));
            //MemoryStream mem = new MemoryStream();
            //s.Serialize(mem, streams[0]);
            
        }

        public override void Dispose()
        {
            fcsink.Release();
            fastVDO.Release();

            base.Dispose();
        }
    }
}
