using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.DirectShowLib.BDA;
using FutureConcepts.Media.TV.Scanner.Config;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    /// <summary>
    /// Base class for BDA TV graphs
    /// </summary>
    public abstract class DigitalTuner : BaseLocalTuner, IVirtualChannelProvider
    {
        protected static readonly Guid CLSID_NetworkProvider = typeof(NetworkProvider).GUID;
        protected static readonly Guid CLSID_ATSCNetworkProvider = typeof(ATSCNetworkProvider).GUID;

        //our tuning space
        protected ITuningSpace tuningSpace;
        //Source
        //network provider
        protected IBaseFilter networkProvider;
        protected ITuner tuner;
        //tuner
        protected IBaseFilter tunerFilter;
        //capture
        protected IBaseFilter captureFilter;

        //MPEG2 Demultiplexer
        protected IBaseFilter mpeg2Demux;

        //Meta Data
        //transport information filter
        protected IBaseFilter bdaTransportInfo;
        //MPEG-2 sections and tables
        protected IBaseFilter mpeg2SectionsAndTables;
        protected IMpeg2Data mpeg2Data;

        //Video
        //MPEG-2 decoder
        protected IBaseFilter videoDecoder;

        //Audio
        //AC-3 decoder
        protected IBaseFilter audioDecoder;

        //SampleGrabber
    //    protected IBaseFilter sampleGrabberFilter;
     //   protected ISampleGrabber sampleGrabber;

        protected static bool UseGenericNetworkProvider { get; private set; }

        static DigitalTuner()
        {
            UseGenericNetworkProvider = false;

            //in windows 7 and up, use the Generic provider
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor >= 1)) ||
                    (Environment.OSVersion.Version.Major > 6))
                {
                    UseGenericNetworkProvider = true;
                }
            }
            Debug.WriteLine("Windows Version is " + Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor + "; Use Generic Network Provider: " + UseGenericNetworkProvider);
        }


        public DigitalTuner(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
            //deal with the Tuning Space and Network Provider
            tuningSpace = GetTuningSpace();

            Guid networkProviderCLSID;
            Guid networkTypeCLSID;

            tuningSpace.get__NetworkType(out networkTypeCLSID);
            if (networkTypeCLSID.Equals(Guid.Empty))
            {
                throw new Exception("Not a digital Network Type!");
            }

            if (UseGenericNetworkProvider)
            {
                networkProviderCLSID = CLSID_NetworkProvider;
            }
            else
            {
                networkProviderCLSID = networkTypeCLSID;
            }

            networkProvider = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(networkProviderCLSID));

            tuner = (ITuner)networkProvider;

         //   if (UseGenericNetworkProvider)
            {
                tuner.put_TuningSpace(tuningSpace);
            }

            int hr = _graphBuilder.AddFilter(networkProvider, "Network Provider");
            DsError.ThrowExceptionForHR(hr);

            //add MPEG2 Demux
            mpeg2Demux = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "MPEG-2 Demultiplexer");

            //add video decoder
            videoDecoder = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, GraphConfig[FilterType.VideoDecoder].Name);
            if (videoDecoder == null)
            {
                throw new Exception("Could not instantiate video decoder!");
            }

            //add audio decoder
            if (GraphConfig[FilterType.AudioDecoder] != null)
            {
                audioDecoder = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, GraphConfig[FilterType.AudioDecoder].Name);
                if (audioDecoder == null)
                {
                    throw new Exception("Could not instantiate audio decoder!");
                }
            }

            //add BDA MPEG-2 Transport Information Filter
            bdaTransportInfo = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDATransportInformationRenderersCategory, "BDA MPEG2 Transport Information Filter");
            //add MPEG-2 Sections and Tables Filter
            mpeg2SectionsAndTables = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.BDATransportInformationRenderersCategory, "MPEG-2 Sections and Tables");

    //        sampleGrabberFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "SampleGrabber");
    //        sampleGrabber = (ISampleGrabber)sampleGrabberFilter;
        }

        protected override void Dispose(bool disposeManaged)
        {
            ReleaseComObject(networkProvider);
            ReleaseComObject(tunerFilter);
            ReleaseComObject(tuner);
            ReleaseComObject(captureFilter);
            ReleaseComObject(mpeg2Demux);
            ReleaseComObject(bdaTransportInfo);
            ReleaseComObject(mpeg2SectionsAndTables);
            ReleaseComObject(mpeg2Data);
            ReleaseComObject(videoDecoder);
            ReleaseComObject(audioDecoder);
       //     ReleaseComObject(sampleGrabber);
       //     ReleaseComObject(sampleGrabberFilter);

            ReleaseComObject(tuningSpace);

            base.Dispose(disposeManaged);
        }

        private bool _graphStartedFirstTime = false;
        /// <summary>
        /// Set this to non-null to have a tuning occur when the graph is first Run.
        /// This is to work around the fact that you cannot set this.Channel in a BDA graph's constructor or risk explosion
        /// </summary>
        protected Channel StartupChannel { get; set; }

        public override void Run()
        {
            base.Run();
            if ((!_graphStartedFirstTime) && (StartupChannel != null))
            {
                this.Channel = StartupChannel;
            }
            _graphStartedFirstTime = true;
        }

        /// <summary>
        /// Must create or retreive the Tuning space to be used by this graph. The Network Provider is created based on this object.
        /// </summary>
        /// <returns>Returns the ITuningSpace to be used for this graph</returns>
        protected abstract ITuningSpace GetTuningSpace();

        public override Channel MinChannel
        {
            get
            {
                return new Channel(2);
            }
        }

        public override ChannelCollection KnownChannels
        {
            get;
            set;
        }
      
        public override int GetSignalStrength()
        {
            if (tuner != null)
            {
                int strength;
                tuner.get_SignalStrength(out strength);
                return strength;
            }
            return -1;
        }

        /// <summary>
        /// Retreives a list of virtual channels that can be identified on the currently tuned physical channel
        /// </summary>
        /// <returns>a list of virtual channels</returns>
        public abstract List<Channel> GetVirtualChannels();
    }
}
