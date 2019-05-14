using System;
using System.Runtime.Serialization;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Used to represent the parameters needed to tune an analog or digital microwave video link
    /// </summary>
    /// <author>kdixon 02/01/2011</author>
    [Serializable]
    [DataContract]
    public class MicrowaveTuning
    {
        /// <summary>
        /// Creates a blank MicrowaveTuning
        /// </summary>
        public MicrowaveTuning() { }

        /// <summary>
        /// Clones another MicrowaveTuning structure
        /// </summary>
        /// <param name="clone">clone</param>
        public MicrowaveTuning(MicrowaveTuning clone)
        {
            this.ChannelBandwidth = clone.ChannelBandwidth;
            if (clone.Encryption != null)
            {
                this.Encryption = new EncryptionInfo(clone.Encryption);
            }
            else
            {
                this.Encryption = null;
            }
            if (clone.ForwardErrorCorrection == null)
            {
                this.ForwardErrorCorrection = null;
            }
            else
            {
                this.ForwardErrorCorrection = new Interval(clone.ForwardErrorCorrection);
            }
            this.Frequency = clone.Frequency;
            if (clone.GuardInterval == null)
            {
                this.GuardInterval = null;
            }
            else
            {
                this.GuardInterval = new Interval(clone.GuardInterval);
            }
            this.Interleaver = clone.Interleaver;
            this.Modulation = clone.Modulation;
            this.PacketDiversityControl = clone.PacketDiversityControl;
            this.SpectralInversion = clone.SpectralInversion;
            this.TransportMode = clone.TransportMode;
        }

        #region MicrowaveTuning Parameters

        /// <summary>
        /// Microwave Tuning Parameters
        /// </summary>
        [Flags]
        public enum Parameters : int
        {
            /// <summary>
            /// Unspecified parameters
            /// </summary>
            Unspecified = 0,
            /// <summary>
            /// Frequency parameter
            /// </summary>
            Frequency = 1,
            /// <summary>
            /// TransportMode parameter
            /// </summary>
            TransportMode = 2,
            /// <summary>
            /// Modulation parameter
            /// </summary>
            Modulation = 4,
            /// <summary>
            /// GuardInterval parameter
            /// </summary>
            GuardInterval = 8,
            /// <summary>
            /// ForwardErrorCorrection parameter
            /// </summary>
            ForwardErrorCorrection = 16,
            /// <summary>
            /// ChannelBandwidth parameter
            /// </summary>
            ChannelBandwidth = 32,
            /// <summary>
            /// PacketDiversityControl parameter
            /// </summary>
            PacketDiversityControl = 64,
            /// <summary>
            /// SpectralInversion parameter
            /// </summary>
            SpectralInversion = 128,
            /// <summary>
            /// Interleaver parameter
            /// </summary>
            Interleaver = 256,
            /// <summary>
            /// Encryption parameter
            /// </summary>
            Encryption = 512,
            /// <summary>
            /// All parameters
            /// </summary>
            ALL = 1023
            
        }

        private int _setFields;
        /// <summary>
        /// Which parameters in this structure are actually valid (set by device)
        /// </summary>
        [DataMember]
        public int ValidParameterData
        {
            get
            {
                return _setFields;
            }
            set
            {
                _setFields = value;
            }
        }

        /// <summary>
        /// Determines if a parameter is marked as valid in the given bit field
        /// </summary>
        /// <param name="bitField">bit field to check</param>
        /// <param name="parameter">parameter to check for</param>
        /// <returns>true if parameter is set in the bitfield, false if not</returns>
        public static bool IsParameterValid(int bitField, Parameters parameter)
        {
            return (bitField & (int)parameter) == (int)parameter;
        }

        #endregion

        private UInt64 _frequency;
        /// <summary>
        /// Center Frequency. Specify in Hz
        /// </summary>
        [DataMember]
        public UInt64 Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
            }
        }

        /// <summary>
        /// Conveinence method for interoperating with systems declared in MHz.
        /// </summary>
        [IgnoreDataMember]
        public UInt64 FrequencyMHz
        {
            get
            {
                return this.Frequency / 1000000;
            }
            set
            {
                this.Frequency = value * 1000000;
            }
        }

        private RFVideoStandard _transport = RFVideoStandard.DVB_T;
        /// <summary>
        /// The "TV" standard being used as the main carrier/modulation scheme.
        /// Vislink calls this "Modulation" or "Radio Op Mode"
        /// </summary>
        [DataMember(IsRequired = false)]
        public RFVideoStandard TransportMode
        {
            get
            {
                return _transport;
            }
            set
            {
                _transport = value;
            }
        }

        private RFModulationType _modulation = RFModulationType.Auto;
        /// <summary>
        /// Modulation scheme being used.
        /// Vislink calls this "Constellation" in their GUI, but modulation in the back end.
        /// </summary>
        [DataMember(IsRequired = false)]
        public RFModulationType Modulation
        {
            get
            {
                return _modulation;
            }
            set
            {
                _modulation = value;
            }
        }

        private Interval _guardInterval = null;
        /// <summary>
        /// Guard Interval. Leave null for Auto
        /// </summary>
        [DataMember(IsRequired = false)]
        public Interval GuardInterval
        {
            get
            {
                return _guardInterval;
            }
            set
            {
                _guardInterval = value;
            }
        }

        private Interval _fec = null;
        /// <summary>
        /// FEC Interval. Leave null for auto
        /// </summary>
        [DataMember(IsRequired = false)]
        public Interval ForwardErrorCorrection
        {
            get
            {
                return _fec;
            }
            set
            {
                _fec = value;
            }
        }

        private UInt64 _bandwidth = 0;
        /// <summary>
        /// The channel bandwidth, in Hz. 0 for Auto
        /// </summary>
        [DataMember(IsRequired = false)]
        public UInt64 ChannelBandwidth
        {
            get
            {
                return _bandwidth;
            }
            set
            {
                _bandwidth = value;
            }
        }

        private bool _packetDiversity = false;
        /// <summary>
        /// True to enable Packet Diversity Control, False to disable.
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool PacketDiversityControl
        {
            get
            {
                return _packetDiversity;
            }
            set
            {
                _packetDiversity = value;
            }
        }

        private bool _spectralInversion = false;
        /// <summary>
        /// True to indicate spectrum is inverted, false to indicate normal
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool SpectralInversion
        {
            get
            {
                return _spectralInversion;
            }
            set
            {
                _spectralInversion = value;
            }
        }


        private bool _interleaver = false;
        /// <summary>
        /// True to enable Interleaver, False to disable
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool Interleaver
        {
            get
            {
                return _interleaver;
            }
            set
            {
                _interleaver = value;
            }
        }

        private EncryptionInfo _encryption;
        /// <summary>
        /// Decryption Key, if needed.
        /// </summary>
        [DataMember(IsRequired = false)]
        public EncryptionInfo Encryption
        {
            get
            {
                return _encryption;
            }
            set
            {
                _encryption = value;
            }
        }

    }
}
