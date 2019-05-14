using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Used to describe the link quality at an arbitrary point in time, typically whenever requested
    /// </summary>
    /// <author>kdixon 02/01/2011</author>
    [Serializable]
    [DataContract]
    public class MicrowaveLinkQuality
    {
        /// <summary>
        /// constructs an empty link quality object
        /// </summary>
        public MicrowaveLinkQuality() { }

        /// <summary>
        /// constructs a simple measure of quality
        /// </summary>
        /// <param name="rssi">Analog Signal Strength as percentage, or Digital Link Quality</param>
        public MicrowaveLinkQuality(int rssi)
        {
            this.ReceivedCarrierLevel = rssi;
        }

        /// <summary>
        /// Clones the specified MicrowaveLinkQuality
        /// </summary>
        /// <param name="clone">item to clone</param>
        public MicrowaveLinkQuality(MicrowaveLinkQuality clone)
        {
            this.BitErrorRatioPost = clone.BitErrorRatioPost;
            this.BitErrorRatioPre = clone.BitErrorRatioPre;
            this.DecoderLocked = clone.DecoderLocked;
            this.DemodulatorLocked = clone.DemodulatorLocked;
            this.FECLocked = clone.FECLocked;
            this.ReceivedCarrierLevel = clone.ReceivedCarrierLevel;
            this.SignalToNoiseRatio = clone.SignalToNoiseRatio;
            this.TransportStreamLocked = clone.TransportStreamLocked;
            this.TunerLocked = clone.TunerLocked;

            this.ValidParameterData = clone.ValidParameterData;
        }

        #region MicrowaveLinkQuality Parameters

        /// <summary>
        /// Microwave Link Quality Parameters
        /// </summary>
        [Flags]
        public enum Parameters : int
        {
            /// <summary>
            /// Unspecified parameters
            /// </summary>
            Unspecified = 0,
            /// <summary>
            /// ReceivedCarrierLevel parameter
            /// </summary>
            ReceivedCarrierLevel = 1,
            /// <summary>
            /// SignalToNoiseRatio parameter
            /// </summary>
            SignalToNoiseRatio = 2,
            /// <summary>
            /// BitErrorRatioPre parameter
            /// </summary>
            BitErrorRatioPre = 4,
            /// <summary>
            /// BitErrorRatioPost parameter
            /// </summary>
            BitErrorRatioPost = 8,
            /// <summary>
            /// TunerLocked parameter
            /// </summary>
            TunerLocked = 16,
            /// <summary>
            /// DemodulatorLocked parameter
            /// </summary>
            DemodulatorLocked = 32,
            /// <summary>
            /// TransportStreamLocked parameter
            /// </summary>
            TransportStreamLocked = 64,
            /// <summary>
            /// FECLocked parameter
            /// </summary>
            FECLocked = 128,
            /// <summary>
            /// DecoderLocked parameter
            /// </summary>
            DecoderLocked = 256,
            /// <summary>
            /// ALL parameters are set
            /// </summary>
            ALL = 511
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

        private double _rssi;
        /// <summary>
        /// Received Carrier Level aka RSSI, in dBm
        /// </summary>
        [DataMember(IsRequired = false)]
        public double ReceivedCarrierLevel
        {
            get
            {
                return _rssi;
            }
            set
            {
                _rssi = value;
            }
        }

        private double _snr;
        /// <summary>
        /// Signal to Noise Ratio, in dB
        /// </summary>
        [DataMember(IsRequired = false)]
        public double SignalToNoiseRatio
        {
            get
            {
                return _snr;
            }
            set
            {
                _snr = value;
            }
        }

        private double _berPre;
        /// <summary>
        /// Bit Error Ratio, before Forward Error Correction has been applied -- # Errors Detected / # Bits Received
        /// </summary>
        [DataMember(IsRequired = false)]
        public double BitErrorRatioPre
        {
            get
            {
                return _berPre;
            }
            set
            {
                _berPre = value;
            }
        }

        private double _berPost;
        /// <summary>
        /// Bit Error Ratio, after Foreward Error Correction has been applied -- # Errors Not Corrected / # Bits Received
        /// </summary>
        [DataMember(IsRequired = false)]
        public double BitErrorRatioPost
        {
            get
            {
                return _berPost;
            }
            set
            {
                _berPost = value;
            }
        }

        private bool _tunerLocked;
        /// <summary>
        /// If known, this means the tuner has locked the carrier frequency
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool TunerLocked
        {
            get
            {
                return _tunerLocked;
            }
            set
            {
                _tunerLocked = value;
            }
        }

        private bool _demodLocked;
        /// <summary>
        /// On digital systems, means the demodulator has locked
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool DemodulatorLocked
        {
            get
            {
                return _demodLocked;
            }
            set
            {
                _demodLocked = value;
            }
        }

        private bool _tsLock;
        /// <summary>
        /// On digital systems, indicates if the MPEG2 TS is locked
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool TransportStreamLocked
        {
            get
            {
                return _tsLock;
            }
            set
            {
                _tsLock = value;
            }
        }

        private bool _fecLock;
        /// <summary>
        /// On digital systems, indicates if the Forward Error Correction is locked
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool FECLocked
        {
            get
            {
                return _fecLock;
            }
            set
            {
                _fecLock = value;
            }
        }

        private bool _decLock;
        /// <summary>
        /// On digital system, indicates if the Decoder is locked.
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool DecoderLocked
        {
            get
            {
                return _decLock;
            }
            set
            {
                _decLock = value;
            }
        }


    }
}
