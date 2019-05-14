using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Describes the capabilities of a microwave receiver. Any MicrowaveTuning objects from this same device should comply with the capabilities.
    /// </summary>
    /// <author>kdixon 02/02/2011</author>
    [Serializable]
    [DataContract]
    public class MicrowaveCapabilities
    {
        #region Tuning Capabilities

        private UInt64 _minFreq;
        /// <summary>
        /// Minimum tunable center frequency, in Hz
        /// </summary>
        [DataMember]
        public UInt64 MinimumFrequency
        {
            get
            {
                return _minFreq;
            }
            set
            {
                _minFreq = value;
            }
        }

        private UInt64 _maxFreq;
        /// <summary>
        /// Maximum tunable center frequency, in Hz
        /// </summary>
        [DataMember]
        public UInt64 MaximumFrequency
        {
            get
            {
                return _maxFreq;
            }
            set
            {
                _maxFreq = value;
            }
        }

        private UInt64 _freqResolution;
        /// <summary>
        /// The resolution of the tuner (e.g. smallest step) in Hz
        /// </summary>
        [DataMember]
        public UInt64 FrequencyResolution
        {
            get
            {
                return _freqResolution;
            }
            set
            {
                _freqResolution = value;
            }
        }

        private double _snrMax;
        /// <summary>
        /// The maximum SNR in dB reported in MicrowaveLinkQuality
        /// </summary>
        [DataMember(IsRequired = false)]
        public double SignalToNoiseRatioMaximum
        {
            get
            {
                return _snrMax;
            }
            set
            {
                _snrMax = value;
            }
        }

        private double _rssiMin;
        /// <summary>
        /// minimum Received Carrier Level in dBm reported in MicrowaveLinkQuality
        /// </summary>
        [DataMember(IsRequired = false)]
        public double ReceivedCarrierLevelMinimum
        {
            get
            {
                return _rssiMin;
            }
            set
            {
                _rssiMin = value;
            }
        }

        private double _rssiMax;
        /// <summary>
        /// maximum Received Carrier Level in dBm reported in MicrowaveLinkQuality
        /// </summary>
        [DataMember(IsRequired = false)]
        public double ReceivedCarrierLevelMaximum
        {
            get
            {
                return _rssiMax;
            }
            set
            {
                _rssiMax = value;
            }
        }

        private List<RFVideoStandard> _supportedStandards;
        /// <summary>
        /// The list of supported video standards.
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<RFVideoStandard> SupportedTransports
        {
            get
            {
                return _supportedStandards;
            }
            set
            {
                _supportedStandards = value;
            }
        }

        private List<RFModulationType> _supportedModulation;
        /// <summary>
        /// The list of supported modulations
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<RFModulationType> SupportedModulations
        {
            get
            {
                return _supportedModulation;
            }
            set
            {
                _supportedModulation = value;
            }
        }

        private List<Interval> _supportGI;
        /// <summary>
        /// The list of supported Guard Intervals
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<Interval> SupportedGuardIntervals
        {
            get
            {
                return _supportGI;
            }
            set
            {
                _supportGI = value;
            }
        }

        private List<Interval> _supportFEC;
        /// <summary>
        /// The list of supported FEC intervals
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<Interval> SupportedForwardErrorCorrection
        {
            get
            {
                return _supportFEC;
            }
            set
            {
                _supportFEC = value;
            }
        }

        private List<UInt64> _supportedBW;
        /// <summary>
        /// The list of supported channel bandwidths (in Hz)
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<UInt64> SupportedBandwidth
        {
            get
            {
                return _supportedBW;
            }
            set
            {
                _supportedBW = value;
            }
        }

        private EncryptionCapabilities _supportEncryption;
        /// <summary>
        /// Non-null if the device supports encryption of some sort
        /// </summary>
        [DataMember(IsRequired = false)]
        public EncryptionCapabilities SupportedEncryption
        {
            get
            {
                return _supportEncryption;
            }
            set
            {
                _supportEncryption = value;
            }
        }

        private MicrowaveTuning _autoTune;
        /// <summary>
        /// A tuning that requires minimal user adjustment to successfully tune a broadcast
        /// </summary>
        [DataMember(IsRequired = false)]
        public MicrowaveTuning AutoTuning
        {
            get
            {
                return _autoTune;
            }
            set
            {
                _autoTune = value;
            }
        }

        #endregion

        #region Tuning Requirements

        private int _tuningRequirements;
        /// <summary>
        /// The MicrowaveTuning fields that are absolutely required for a tuning to be valid
        /// See the MicrowaveTuning.Parameters
        /// </summary>
        [DataMember(IsRequired = false)]
        public int SupportedTuningParameters
        {
            get
            {
                return _tuningRequirements;
            }
            set
            {
                _tuningRequirements = value;
            }
        }

        private int _autoTuningRequirements;
        /// <summary>
        /// The MicrowaveTuning fields that must be set to use the AutoTuning field.
        /// See the MicrowaveTuning.Parameters
        /// </summary>
        [DataMember(IsRequired = false)]
        public int AutoTuningRequirements
        {
            get
            {
                return _autoTuningRequirements;
            }
            set
            {
                _autoTuningRequirements = value;
            }
        }

        #endregion

        #region Supported Link Quality

        private int _supportedLinkQuality;
        /// <summary>
        /// Indicates what fields of the MicrowaveLinkQuailty structure are actually set by the device.
        /// See the MicrowaveLinkQuality.Parameters
        /// </summary>
        [DataMember(IsRequired = false)]
        public int SupportedLinkQualityParameters
        {
            get
            {
                return _supportedLinkQuality;
            }
            set
            {
                _supportedLinkQuality = value;
            }
        }

        #endregion
    }
}
