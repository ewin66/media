using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutureConcepts.Media.DirectShowLib.Framework;
using System.Diagnostics;
using LMNetMuxLib;
using LMNetSnkLib;

namespace FutureConcepts.Media.DirectShowLib.Graphs
{
    /// <summary>
    /// Encapsulates an LTNetworkSink as a modular graph
    /// </summary>
    /// <author>kdixon 01/05/2010 --- based from Server.Graphs.StreamingGraph by darnold</author>
    public class LTNetworkSink : BaseDSGraph, ISinkSubGraph
    {
        private IBaseFilter input;

        /// <summary>
        /// the LT Net Mux filter
        /// </summary>
        protected IBaseFilter netMux;
        /// <summary>
        /// The LT Net Sink filter
        /// </summary>
        protected IBaseFilter netSnk;
        /// <summary>
        /// The LMNetMux interface on the net mux filter
        /// </summary>
        protected LMNetMux lmNetMux;
        /// <summary>
        /// The LMNetSnk interface on the net mux filter
        /// </summary>
        protected LMNetSnk lmNetSnk;

        /// <summary>
        /// The URL that we are sinking data to
        /// </summary>
        public string SinkURL { get; private set; }

        /// <summary>
        /// True if this was constructed as a live source
        /// </summary>
        public bool LiveSource { get; private set; }

        /// <summary>
        /// The maximum queue duration that is used
        /// </summary>
        public double MaxQueueDuration { get; private set; }

        /// <summary>
        /// The running average of the bitrate being sunk
        /// </summary>
        public int AverageBitRate
        {
            get
            {
                return lmNetMux.BitRate;
            }
        }

        /// <summary>
        /// Write a message to all clients using the LT Net Mux
        /// </summary>
        /// <param name="msg">message to send</param>
        public void WriteLTNetMessage(string msg)
        {
            if (lmNetMux != null)
            {
                lmNetMux.WriteMessage(msg);
            }
        }

        /// <summary>
        /// Creates a new instance of the LTNetworkSink sub graph
        /// </summary>
        /// <param name="sinkUrl">url to sink to</param>
        /// <param name="liveSource">true if this is to be a live source</param>
        /// <param name="maxQueueDuration">the value of the queue duration in seconds</param>
        public LTNetworkSink(string sinkUrl, bool liveSource, double maxQueueDuration)
        {
            this.SinkURL = sinkUrl;
            this.LiveSource = liveSource;
            this.MaxQueueDuration = maxQueueDuration;
        }

        /// <summary>
        /// Attach this graph to a source
        /// </summary>
        /// <param name="source">the source sub graph</param>
        public void SetSource(ISourceSubGraph source)
        {
            Debug.WriteLine("LTNetworkSink.SetSource: " + source.GetType().ToString());

            List<DetailPinInfo> pins = null;

            try
            {
                netMux = (IBaseFilter)new LMNetMuxClass();
                graph.AddFilter(netMux, @"LEAD Network Multiplexer (2.0)");
                lmNetMux = (LMNetMux)netMux;
                lmNetMux.LiveSource = this.LiveSource;
                Debug.WriteLine(String.Format("LiveSource={0}", lmNetMux.LiveSource));

                lmNetMux.MaxQueueDuration = this.MaxQueueDuration;
                netSnk = (IBaseFilter)new LMNetSnkClass();
                graph.AddFilter(netSnk, "LEAD Network Sink (2.0)");

                InitializeNetworkSink();

                input = (IBaseFilter)source.Controller.InsertSourceFilter(source.Output, this.graph);

                pins = input.EnumPinsDetails();

                int muxInputNumber = 1;
                foreach (DetailPinInfo i in pins)
                {
                    IPin muxPin = null;
                    try
                    {
                        muxPin = DsFindPin.ByName(netMux, "Input " + muxInputNumber.ToString("D2"));
                        FilterGraphTools.ConnectFilters(this.graph, i.Pin, muxPin, false);
                    }
                    finally
                    {
                        if (muxPin != null)
                        {
                            muxPin.Release();
                        }
                    }
                    muxInputNumber++;


                }

                FilterGraphTools.ConnectFilters(this.graph, netMux, "Output 01", netSnk, "Input", false);
            }
            finally
            {
                if (pins != null)
                {
                    pins.Release();
                }
            }

        }

        /// <summary>
        /// Closes all connections and releases resources
        /// </summary>
        public override void Dispose()
        {
            if (netSnk != null)
            {
                if (lmNetSnk != null)
                {
                    lmNetSnk.CloseAll();
                }
            }

            Release(netMux);
            Release(netSnk);
            Release(this.Input);
            base.Dispose();
        }

        /// <summary>
        /// Retreives the GMF Sink filter which is the input for this graph
        /// </summary>
        public IBaseFilter Input
        {
            get
            {
                if (input == null)
                {
                    throw new InvalidOperationException("SetSource() must be called before you can retreive the input");
                }
                return input;
            }
        }

        /// <summary>
        /// initializes the sink
        /// </summary>
        protected virtual void InitializeNetworkSink()
        {
            IFileSinkFilter fileSink = (IFileSinkFilter)netSnk;
            if (fileSink == null)
            {
                throw new Exception("IFileSourceFilter not found on LMNetSink");
            }
            lmNetSnk = (LMNetSnk)netSnk;
            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = MediaType.Stream;
            mediaType.subType = MediaSubType.Null;
            int hr = fileSink.SetFileName(this.SinkURL, mediaType);
            DsError.ThrowExceptionForHR(hr);
        }
    }
}
