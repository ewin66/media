using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.Client.StreamViewer.DeviceControl
{
    public class DeviceControlFactory
    {
        public static IDeviceControl Create(StreamSourceInfo streamSourceInfo, String serverAddress)
        {
            if (streamSourceInfo.SinkAddress.StartsWith(@"udp://"))
            {
                return new RTSPDeviceControl(streamSourceInfo, serverAddress);
            }
            else if (streamSourceInfo.SourceType == SourceType.RTSP || streamSourceInfo.SourceType == SourceType.RTSP_Elecard)
            {
                return new RTSPDeviceControl(streamSourceInfo, serverAddress);
            }
            else if (streamSourceInfo.SourceType == SourceType.HTTP_GET)
            {
                return new HttpGetDeviceControl(streamSourceInfo, serverAddress);
            }
            else
            {
                return new LTSFDeviceControl(streamSourceInfo, serverAddress);
            }
        }
    }
}
