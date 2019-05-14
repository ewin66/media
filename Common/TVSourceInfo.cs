using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Describes additional parameters needed to define and configure a TV Server source
    /// </summary>
    [Serializable]
    public class TVSourceInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        private TV.TunerInputType _analogTunerInputType = TV.TunerInputType.Antenna;
        /// <summary>
        /// Indicates the analog tuner input type (RF signal) Antenna, or Cable.
        /// </summary>
        [XmlElement, DefaultValue(TV.TunerInputType.Antenna)]
        public TV.TunerInputType AnalogTunerInputType
        {
            get
            {
                return _analogTunerInputType;
            }
            set
            {
                if (_analogTunerInputType != value)
                {
                    _analogTunerInputType = value;
                    NotifyPropertyChanged("AnalogTunerInputType");
                }
            }
        }

        private TV.PhysicalConnectorType _analogCrossbarVideo = TV.PhysicalConnectorType.Video_Tuner;
        /// <summary>
        /// Indicates how video should be routed in an Analog Crossbar
        /// </summary>
        [XmlElement, DefaultValue(TV.PhysicalConnectorType.Video_Tuner)]
        public TV.PhysicalConnectorType AnalogCrossbarVideo
        {
            get
            {
                return _analogCrossbarVideo;
            }
            set
            {
                if (_analogCrossbarVideo != value)
                {
                    _analogCrossbarVideo = value;
                    NotifyPropertyChanged("AnalogCrossbarVideo");
                }
            }
        }

        private TV.PhysicalConnectorType _analogCrossbarAudio = TV.PhysicalConnectorType.Audio_Tuner;
        /// <summary>
        /// Indicates how audio should be routed in an Analog Crossbar
        /// </summary>
        [XmlElement, DefaultValue(TV.PhysicalConnectorType.Audio_Tuner)]
        public TV.PhysicalConnectorType AnalogCrossbarAudio
        {
            get
            {
                return _analogCrossbarAudio;
            }
            set
            {
                if (_analogCrossbarAudio != value)
                {
                    _analogCrossbarAudio = value;
                    NotifyPropertyChanged("AnalogCrossbarAudio");
                }
            }
        }

        private bool _useHardwareEncoder = false;
        /// <summary>
        /// True if the TV Capture card has a Hardware Encoder that should be utilized.
        /// False if software encoding or decoding will be required by the server.
        /// </summary>
        [XmlElement, DefaultValue(false)]
        public bool UseHardwareEncoder
        {
            get
            {
                return _useHardwareEncoder;
            }
            set
            {
                if (_useHardwareEncoder != value)
                {
                    _useHardwareEncoder = value;
                    NotifyPropertyChanged("UseHardwareEncoder");
                }
            }
        }

        private int _deviceIndex = 0;
        //TODO this should probably be a DeviceAddress object instead of an int
        /// <summary>
        /// On systems with multiple cards, use this index to differentiate between cards (0-based)
        /// </summary>
        [XmlElement]
        public int DeviceIndex
        {
            get
            {
                return _deviceIndex;
            }
            set
            {
                if (_deviceIndex != value)
                {
                    _deviceIndex = value;
                    NotifyPropertyChanged("DeviceIndex");
                }
            }
        }

        private string _regionID;
        /// <summary>
        /// Not used. May be repurposed in the future.
        /// </summary>
        [XmlElement]
        public string RegionID
        {
            get
            {
                return _regionID;
            }
            set
            {
                if (_regionID != value)
                {
                    _regionID = value;
                    NotifyPropertyChanged("RegionID");
                }
            }
        }
    }
}
