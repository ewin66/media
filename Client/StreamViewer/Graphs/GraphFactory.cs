using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using FutureConcepts.Media.Client.InBandData;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using GMFBridgeLib;

namespace FutureConcepts.Media.Client.StreamViewer.Graphs
{
    /// <summary>
    /// Base graph class
    /// </summary>
    public class GraphFactory
    {
        public BaseGraph CreateGraph(SourceType sourceType, String url)
        {
            BaseGraph result = null;
            Debug.WriteLine(String.Format("BaseGraph.createInstance {0} {1}", sourceType, url));
            if (url.StartsWith(@"udp://"))
            {
                result = new MPEG2TSGraph(url);
            }
            else if (url.StartsWith("rtsp://"))
            {
                if (FilterGraphTools.FilterExists(FilterCategory.LegacyAmFilterCategory, "Elecard RTSP NetSource"))
                {
               //     if (FilterGraphTools.FilterExists(FilterCategory.LegacyAmFilterCategory, "Microsoft DTV-DVD Video Decoder"))
                    if (false)
                    {
                        result = new ElecardWithMicrosoftRTSP(url);
                    }
                    else if (FilterGraphTools.FilterExists(FilterCategory.LegacyAmFilterCategory, "Elecard AVC Video Decoder"))
                    {
                        result = new ElecardWithElecardRTSP(url);
                    }
                    else
                    {
                        throw new Exception("Video decoder not found");
                    }
                }
                else
                {
                    result = new LeadToolsRTSPGraph(url);
                }
            }
            else if (sourceType == SourceType.HTTP_GET)
            {
                result = new HTTPGetGraph(url);
            }
            else
            {
                result = new LTNetSrcGraph(url);
            }
            return result;
        }
    }
}
