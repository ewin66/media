using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media;
using System.Threading;

namespace FutureConcepts.Media.MicrowaveReceiverController
{
    public class TestReciever : MicrowaveReceiver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tcpAddress"></param>
        /// <param name="port"></param>
        /// <param name="useTCP"></param>
        /// <param name="comport"></param>
        /// <param name="baud">9600</param>
        public TestReciever(string tcpAddress, int port, bool useTCP, string comport, int baud)
            : base(tcpAddress, port, useTCP, comport, baud)
        {
            ReceiveTimeout = 10000;

            MicrowaveCapabilities c = new MicrowaveCapabilities();
            c.FrequencyResolution = 250000;
            c.MinimumFrequency = 4400000000;
            c.MaximumFrequency = 5000000000;

            c.ReceivedCarrierLevelMinimum = -100;
            c.ReceivedCarrierLevelMaximum = 0;
            c.SignalToNoiseRatioMaximum = 40;
            
            c.SupportedTransports = new List<RFVideoStandard>();
            c.SupportedTransports.Add(RFVideoStandard.DVB_T);

            c.SupportedModulations = new List<RFModulationType>();
            c.SupportedModulations.Add(RFModulationType.QPSK);
            c.SupportedModulations.Add(RFModulationType.QAM16);
            c.SupportedModulations.Add(RFModulationType.QAM32);
            c.SupportedModulations.Add(RFModulationType.QAM64);
            c.SupportedModulations.Add(RFModulationType.Auto);

            c.SupportedGuardIntervals = new List<Interval>();
            c.SupportedGuardIntervals.Add(Interval._1_32);
            c.SupportedGuardIntervals.Add(Interval._1_16);
            c.SupportedGuardIntervals.Add(Interval._1_8);
            c.SupportedGuardIntervals.Add(Interval._1_4);
            c.SupportedGuardIntervals.Add(Interval._1_3);
            c.SupportedGuardIntervals.Add(Interval.Auto);

            c.SupportedBandwidth = new List<ulong>();
            c.SupportedBandwidth.Add(6000000);  // 6MHz
            c.SupportedBandwidth.Add(7000000);  // 7MHz
            c.SupportedBandwidth.Add(8000000);  // 8MHz
            c.SupportedBandwidth.Add(0);        // auto

            c.SupportedForwardErrorCorrection = new List<Interval>();
            c.SupportedForwardErrorCorrection.Add(Interval.Auto);
            c.SupportedForwardErrorCorrection.Add(Interval.Auto);   //documentation says index1 is "RS only" not clear what that means
            c.SupportedForwardErrorCorrection.Add(Interval._1_2);
            c.SupportedForwardErrorCorrection.Add(Interval._2_3);
            c.SupportedForwardErrorCorrection.Add(Interval._3_4);
            c.SupportedForwardErrorCorrection.Add(Interval._5_6);
            c.SupportedForwardErrorCorrection.Add(Interval._7_8);

            c.SupportedEncryption = new EncryptionCapabilities();
            c.SupportedEncryption.SupportedTypesAndKeyLengths = new Dictionary<EncryptionType, List<int>>();
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.None, null);
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.AES_Legacy, new int[] { 128, 256 }.ToList());
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.AES_Bcrypt, new int[] { 128, 256 }.ToList());
            //the BISS key lengths are derived from http://tech.ebu.ch/docs/tech/tech3292.pdf spec
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.BISS_1, new int[] { 48 }.ToList());
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.BISS_E, new int[] { 64 }.ToList());
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.DES, new int[] { 56 }.ToList());

            c.AutoTuning = new MicrowaveTuning();
            c.AutoTuning.ChannelBandwidth = 0;
            c.AutoTuning.Encryption = null;
            c.AutoTuning.ForwardErrorCorrection = Interval.Auto;
            c.AutoTuning.Frequency = 0;
            c.AutoTuning.GuardInterval = Interval.Auto;
            c.AutoTuning.Interleaver = false;
            c.AutoTuning.Modulation = RFModulationType.Auto;
            c.AutoTuning.PacketDiversityControl = false;
            c.AutoTuning.SpectralInversion = false;
            c.AutoTuning.TransportMode = RFVideoStandard.DVB_T;

            c.SupportedTuningParameters = (int)(MicrowaveTuning.Parameters.Frequency |
                                                MicrowaveTuning.Parameters.ChannelBandwidth |
                                                MicrowaveTuning.Parameters.ForwardErrorCorrection |
                                                MicrowaveTuning.Parameters.GuardInterval |
                                                MicrowaveTuning.Parameters.Modulation |
                                                MicrowaveTuning.Parameters.TransportMode |
                                                MicrowaveTuning.Parameters.Encryption |
                                                MicrowaveTuning.Parameters.SpectralInversion);
            c.AutoTuningRequirements = (int)MicrowaveTuning.Parameters.Frequency;

            c.SupportedLinkQualityParameters = (int)(MicrowaveLinkQuality.Parameters.BitErrorRatioPost |
                                                     MicrowaveLinkQuality.Parameters.BitErrorRatioPre |
                                                     MicrowaveLinkQuality.Parameters.DecoderLocked |
                                                     MicrowaveLinkQuality.Parameters.DemodulatorLocked |
                                                     MicrowaveLinkQuality.Parameters.FECLocked |
                                                     MicrowaveLinkQuality.Parameters.ReceivedCarrierLevel |
                                                     MicrowaveLinkQuality.Parameters.SignalToNoiseRatio |
                                                     MicrowaveLinkQuality.Parameters.TransportStreamLocked |
                                                     MicrowaveLinkQuality.Parameters.TunerLocked);

            _capabilities = c;

            MicrowaveTuning t = new MicrowaveTuning();
            t.ChannelBandwidth = c.SupportedBandwidth[0];
            t.Encryption = new EncryptionInfo();
            t.Encryption.Type = EncryptionType.None;
            t.ForwardErrorCorrection = c.SupportedForwardErrorCorrection[0];
            t.Frequency = c.MinimumFrequency;
            t.GuardInterval = c.SupportedGuardIntervals[0];
            t.Interleaver = false;
            t.Modulation = c.SupportedModulations[0];
            t.PacketDiversityControl = false;
            t.SpectralInversion = false;
            t.TransportMode = c.SupportedTransports[0];
            t.ValidParameterData = (int)MicrowaveTuning.Parameters.ALL;

            _tuning = t;

            MicrowaveLinkQuality lq = new MicrowaveLinkQuality();
            lq.BitErrorRatioPost = 0;
            lq.BitErrorRatioPre = 0.01;
            lq.DecoderLocked = true;
            lq.DemodulatorLocked = true;
            lq.FECLocked = true;
            lq.ReceivedCarrierLevel = -47;
            lq.SignalToNoiseRatio = 25;
            lq.TransportStreamLocked = true;
            lq.TunerLocked = true;
            lq.ValidParameterData = (int)MicrowaveLinkQuality.Parameters.ALL;

            _linkQuality = lq;
        }

        
        #region Test-Specific API

        private MicrowaveCapabilities _capabilities;
        /// <summary>
        /// The current tuning data the fixture will send/return
        /// </summary>
        public MicrowaveCapabilities Capabilities
        {
            get
            {
                return _capabilities;
            }
            set
            {
                _capabilities = value;
            }
        }

        private MicrowaveTuning _tuning;
        /// <summary>
        /// The current tuning data the fixture will send/return
        /// </summary>
        public MicrowaveTuning Tuning
        {
            get
            {
                return _tuning;
            }
            set
            {
                _tuning = value;
            }
        }

        private MicrowaveLinkQuality _linkQuality;
        /// <summary>
        /// The current link quality data the fixture will send/return
        /// </summary>
        public MicrowaveLinkQuality LinkQuality
        {
            get
            {
                return _linkQuality;
            }
            set
            {
                _linkQuality = value;
            }
        }

        internal override string RawCommand(string userinput)
        {
            return "not implemented";
        }

        #endregion

        #region Public API

        public override string GetDeviceInfo()
        {
            return "Test Receiver. This is a dummy implementation that responds to API calls";
        }

        public override MicrowaveCapabilities GetCapabilities()
        {
            return this.Capabilities;
        }

        public override void SetTuning(MicrowaveTuning tuning)
        {
            Thread.Sleep(1000);
            tuning.ValidParameterData = this.Tuning.ValidParameterData;
            this.Tuning = tuning;
            DoTuningChangeEvent();            
        }

        public override MicrowaveTuning GetTuning()
        {
            Thread.Sleep(1000);
            return this.Tuning;
        }

        public override MicrowaveLinkQuality GetLinkQuality()
        {
            Thread.Sleep(1000);

            return this.LinkQuality;
        }

        public override bool TestConnection()
        {
            Thread.Sleep(300);
            return true;
        }

        #endregion
    }
}
