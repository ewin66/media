using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib.Framework;
using GMFBridgeLib;
using LMNetDmxLib;
using LMNetSrcLib;


namespace FutureConcepts.Media.DirectShowLib.Graphs
{
    //TODO: when this is ported to 10.18, it needs to be integrated with the InBandData parser

    /// <summary>
    /// This class connects to a LEADTOOLS Network Source
    /// </summary>
    public class LTNetworkSource : BaseDSGraph, INetworkSource
    {
        private IBaseFilter netSource;
        private IFileSourceFilter netFileSource;

        private IBaseFilter netDemux;
        private LMNetDmx lmNetDemux;

        private IGMFBridgeController controller;
        private IBaseFilter output;

        private string ltsfUrl;

        /// <summary>
        /// Creates a connection to a LEADTOOLS Network Sink
        /// </summary>
        /// <param name="ltsfUrl">fully qualified ltsf:// url</param>
        public LTNetworkSource(string ltsfUrl)
        {
            this.Status = NetworkSourceStatus.Inactive;

            this.ltsfUrl = ltsfUrl;
        }

        #region INetworkSource Members

        /// <summary>
        /// Performs the network connection
        /// </summary>
        public void Connect()
        {
            try
            {
                StatusError = null;
                this.Status = NetworkSourceStatus.Connecting;

                Debug.WriteLine("LTNetworkSource add filters");
                AddLTNetSource();
                AddLTNetDemux();

                this.Status = NetworkSourceStatus.Buffering;
                Debug.WriteLine("LTNetworkSource.DoConnect");
                DoConnect(ltsfUrl);

                FilterGraphTools.ConnectFilters(graph, netSource, "Output", netDemux, "Input 01", false);

                Debug.WriteLine("LTNetworkSource create controller");
                controller = (IGMFBridgeController)new GMFBridgeControllerClass();
                Debug.WriteLine("LTNetworkSource.RenderNetDemux");
                RenderNetDemux();
                Debug.WriteLine("LTNetworkSource done constructing");
                this.Status = NetworkSourceStatus.Connected;
            }
            catch (Exception ex)
            {
                StatusError = ex;
                this.Status = NetworkSourceStatus.Faulted;
                throw ex;
            }
        }

        /// <summary>
        /// Drops the connection
        /// </summary>
        public void Disconnect()
        {
            this.Status = NetworkSourceStatus.Disconnecting;
            this.Stop();
            this.Status = NetworkSourceStatus.Inactive;
        }

        /// <summary>
        /// Any error associated with the last change in status
        /// </summary>
        public Exception StatusError { get; private set; }

        private NetworkSourceStatus _status;
        /// <summary>
        /// The current status of the network source/connection
        /// </summary>
        public NetworkSourceStatus Status
        {
            get
            {
                return _status;
            }
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        private int _bitrate;
        /// <summary>
        /// The currently received bitrate, in kbps
        /// </summary>
        public int Bitrate
        {
            get
            {
                return _bitrate;
            }
            private set
            {
                if (_bitrate != value)
                {
                    _bitrate = value;
                    NotifyPropertyChanged("Bitrate");
                }
            }
        }

        #endregion

        #region Graph-Building

        /// <summary>
        /// Adds the LT Net Source filter to the graph
        /// </summary>
        private void AddLTNetSource()
        {
            netSource = (IBaseFilter)new LMNetSrcClass();
            int hr = graph.AddFilter(netSource, "LEAD Network Source (2.0)");
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Adds the LT Net Demux filter to the graph
        /// </summary>
        private void AddLTNetDemux()
        {
            int hr;

            netDemux = (IBaseFilter)new LMNetDmx();
            hr = graph.AddFilter(netDemux, "LEAD NetDmx");
            DsError.ThrowExceptionForHR(hr);
            lmNetDemux = (LMNetDmx)netDemux;
        }

        /// <summary>
        /// Performs the actual connection to the sink, then connects the LTNetSource to the LTNetDmx
        /// </summary>
        /// <param name="ltsfUrl">fully qualified url in ltsf:// format</param>
        private void DoConnect(string ltsfUrl)
        {
            //do network connection at DirectShow level
            int hr;
            netFileSource = (IFileSourceFilter)netSource;
            if (netFileSource == null)
            {
                throw new Exception("IFileSourceFilter not found on netSource");
            }

            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = MediaType.Stream;
            mediaType.subType = MediaSubType.LeadToolsStreamFormat;
            hr = netFileSource.Load(ltsfUrl, mediaType);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(mediaType);
        }


        /// <summary>
        /// Determines what streams are available on the Net Demux.
        /// Creates channels in the GMF Bridge controller accordingly.
        /// Then, creates the GMF Bridge Sink, and connects the streams to their respective pins.
        /// </summary>
        private void RenderNetDemux()
        {

            List<DetailPinInfo> pins = null;

            try
            {
                //fetch all pins on this filter
                pins = netDemux.EnumPinsDetails();

                //create list of pins we care about
                List<IPin> demuxPins = new List<IPin>();

                //get output pins of type video or audio
                foreach (DetailPinInfo i in pins)
                {
                    if (i.Info.dir == PinDirection.Output)
                    {
                        if (i.Type.majorType == MediaType.Video)
                        {
                            controller.AddStream(1, eFormatType.eAny, 1);
                            demuxPins.Add(i.Pin);
                        }
                        else if (i.Type.majorType == MediaType.Audio)
                        {
                            controller.AddStream(0, eFormatType.eAny, 1);
                            demuxPins.Add(i.Pin);
                        }
                    }
                }

                //create GMF Sink
                output = (IBaseFilter)controller.InsertSinkFilter(graph);
                //connect Demux to GMF Sink
                for (int i = 0; i < demuxPins.Count; i++)
                {
                    IPin sinkPin;
                    int hr = output.FindPin("Input " + (i + 1).ToString(), out sinkPin);
                    if (hr == 0)
                    {
                        FilterGraphTools.ConnectFilters(graph, demuxPins[i], sinkPin, false);
                        Marshal.ReleaseComObject(sinkPin);
                    }
                }
            }
            catch (Exception ex)
            {
                Release(output);
                output = null;
                throw ex;
            }
            finally
            {
                if (pins != null)
                {
                    pins.Release();
                }
            }
        }

        #endregion

        #region ISourceSubGraph Members

        /// <summary>
        /// Retreives the GMFBridgeController used to 
        /// </summary>
        public IGMFBridgeController Controller
        {
            get
            {
                if (controller == null)
                {
                    throw new InvalidOperationException("The Controller cannot be retreived until the connection has been completed successfully.");
                }
                return controller;
            }
        }

        /// <summary>
        /// Gets the GMF Source filter that is the output of this graph
        /// </summary>
        public IBaseFilter Output
        {
            get
            {
                if ((controller == null) || (output == null))
                {
                    throw new InvalidOperationException("The Output cannot be retreived until the connection has been completed successfully.");
                }
                return output;
            }
        }

        #endregion

        /// <summary>
        /// Disconnects and disposes the graph
        /// </summary>
        public override void Dispose()
        {
            Release(output);
            Release(controller);

            Release(netSource);
            Release(netFileSource);

            Release(netDemux);
            Release(lmNetDemux);
            
            base.Dispose();
        }


    }
}
