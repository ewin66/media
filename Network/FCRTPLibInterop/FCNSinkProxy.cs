using System.Collections.Generic;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Network.FCRTPLib
{
    public class FCNSinkProxy
    {
        private IFCNSink sink;

        public FCNSinkProxy(IBaseFilter fcNetworkSink)
        {
            sink = (IFCNSink)fcNetworkSink;
        }

        public IFCNSink Interface
        {
            get
            {
                return sink;
            }
        }

        public int StreamCount
        {
            get
            {
                int count = 0;
                sink.GetStreamCount(out count);
                return count;
            }
        }

        public List<RTPStreamDescription> Streams
        {
            get
            {
                List<RTPStreamDescription> streams = new List<RTPStreamDescription>();
                for (int i = 0; ; i++)
                {
                    FCRTPStreamDescriptionStruct s;
                    int hr = sink.GetStream(i, out s);

                    if (hr == DsResults.S_NoMoreItems) break;
                    if (DsError.Succeeded(hr))
                    {
                        if (hr == 0)
                        {
                            streams.Add(new RTPStreamDescription(s));
                        }
                    }
                    else
                    {
                        throw new COMException("Error on IFNCSink.GetStream", hr);
                    }
                }
                return streams;
            }
        }
    }
}
