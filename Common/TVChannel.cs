/*
 *  TVChannel.cs
 *  Contains TV.Channel
 *  Kevin Dixon
 *  10/09/2008
 */

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FutureConcepts.Media.TV
{
    /// <summary>
    /// Represents a TV Channel.
    /// </summary>
    /// <remarks>
    /// Set any of the fields to -1 to indicate that they are unknown or irrelevant.
    /// </remarks>
    [Serializable]
    public class Channel : INotifyPropertyChanged, IComparable<Channel>
    {
        /// <summary>
        /// Creates a "blank" un-tunable channel
        /// </summary>
        public Channel()
        {
            CarrierFrequency = -1;
            PhysicalChannel = -1;
            MajorChannel = -1;
            MinorChannel = -1;
        }

        /// <summary>
        /// Creates a channel that refers to a physical TV Channel.
        /// </summary>
        /// <param name="physical">physical channel</param>
        public Channel(int physical)
        {
            CarrierFrequency = -1;
            PhysicalChannel = physical;
            MajorChannel = -1;
            MinorChannel = -1;
        }

        /// <summary>
        /// Creates a channel that refers to a physical channel, that carries the given major/minor combination.
        /// </summary>
        /// <param name="physical">physical channel</param>
        /// <param name="major">major channel number (may be virtual)</param>
        /// <param name="minor">minor channel number (may be virtual)</param>
        public Channel(int physical, int major, int minor)
        {
            CarrierFrequency = -1;
            PhysicalChannel = physical;
            MajorChannel = major;
            MinorChannel = minor;
        }

        /// <summary>
        /// Creates a channel that refers to a specific carrier frequency, indicates the physical channel, as well as a major/minor combination.
        /// </summary>
        /// <param name="carrierFrequency">The carrier frequency, in KHz</param>
        /// <param name="physical">physical channel</param>
        /// <param name="major">major channel number (may be virtual)</param>
        /// <param name="minor">minor channel number (may be virtual)</param>
        public Channel(int carrierFrequency, int physical, int major, int minor)
        {
            CarrierFrequency = carrierFrequency;
            PhysicalChannel = physical;
            MajorChannel = major;
            MinorChannel = minor;
        }

        private int _carrierFreq;
        /// <summary>
        /// The Carrier Frequency for this channel, in KHz
        /// </summary>
        [XmlAttribute, DefaultValue(-1)]
        public int CarrierFrequency
        {
            get
            {
                return _carrierFreq;
            }
            set
            {
                if (_carrierFreq != value)
                {
                    _carrierFreq = value;
                    NotifyPropertyChanged("CarrierFrequency");
                }
            }
        }



        private int _physicalChannel = -1;
        /// <summary>
        /// The Physical Channel. This is the channel that is mapped directly to a Carrier Frequency either by NTSC or ATSC standards
        /// </summary>
        [XmlAttribute, DefaultValue(-1)]
        public int PhysicalChannel
        {
            get
            {
                return _physicalChannel;
            }
            set
            {
                if (_physicalChannel != value)
                {
                    _physicalChannel = value;
                    NotifyPropertyChanged("PhysicalChannel");
                }
            }
        }

        private int _majorChannel = -1;
        /// <summary>
        /// The primary logical identifier for a station. For analog TV, PhysicalChannel and MajorChannel should be the same.
        /// </summary>
        [XmlAttribute, DefaultValue(-1)]
        public int MajorChannel
        {
            get
            {
                return _majorChannel;
            }
            set
            {
                if (_majorChannel != value)
                {
                    _majorChannel = value;
                    NotifyPropertyChanged("MajorChannel");
                }
            }
        }

        private int _minorChannel = -1;
        /// <summary>
        /// This sub-channel of the MajorChannel. For analog TV, this should be 0.
        /// </summary>
        [XmlAttribute, DefaultValue(-1)]
        public int MinorChannel
        {
            get
            {
                return _minorChannel;
            }
            set
            {
                if (_minorChannel != value)
                {
                    _minorChannel = value;
                    NotifyPropertyChanged("MinorChannel");
                }
            }
        }

        private string _callsign;
        /// <summary>
        /// The callsign for the station
        /// </summary>
        [XmlAttribute, DefaultValue("")]
        public string Callsign
        {
            get
            {
                return _callsign;
            }
            set
            {
                _callsign = value;
            }
        }

        /// <summary>
        /// Returns a string representing this channel.
        /// </summary>
        /// <remarks>
        /// If both <see cref="P:MajorChannel"/> and <see cref="P:MinorChannel"/> are defined, it will return a string in the form "X.X".
        /// If only <see cref="P:MajorChannel"/> is defined, the string will simply be "X".
        /// If <see cref="P:MajorChannel"/> and <see cref="P:PhysicalChannel"/> are the same, and <see cref="P:MinorChannel"/> is 0 or -1, ti will return the physical channel in the format "X".
        /// If only the <see cref="P:PhysicalChannel"/> is defined, then that will be shown, in the form "X".
        /// If nothing else, the <see cref="P:PhysicalChannel"/>, <see cref="P:MajorChannel"/>, and <see cref="P:MinorChannel"/> will all be listed, in the format "X X X".
        /// </remarks>
        public override string ToString()
        {
            if ((MajorChannel > -1) && (MinorChannel > -1))
            {
                return MajorChannel.ToString() + "." + MinorChannel.ToString();
            }
            else if ((PhysicalChannel < 0) && (MajorChannel > -1))
            {
                return MajorChannel.ToString();
            }
            else if ((PhysicalChannel == MajorChannel) && (MinorChannel <= 0))
            {
                return PhysicalChannel.ToString();
            }
            else if ((MajorChannel < 0) && (PhysicalChannel > -1))
            {
                return PhysicalChannel.ToString();
            }
            else
            {
                return PhysicalChannel.ToString() + " " + MajorChannel.ToString() + "." + MinorChannel.ToString();
            }
        }

        /// <summary>
        /// Returns a string with all of the parameters shown
        /// </summary>
        /// <remarks>Shown in the format "<see cref="P:PhysicalChannel"/> <see cref="P:MajorChannel"/> <see cref="P:MinorChannel"/> <see cref="P:CarrierFrequency"/> <see cref="P:Callsign"/>"</remarks>
        public string ToDebugString()
        {
            return PhysicalChannel.ToString() + " " + MajorChannel.ToString() + "." + MinorChannel.ToString() + " " + CarrierFrequency.ToString() + " " + Callsign;
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property has changed in value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Determines if two Channel objects represent the same exact physical and logical channel.
        /// </summary>
        /// <param name="obj">Channel object to compare</param>
        /// <returns>true if they both represent the same exact physical and logical channel. false otherwise</returns>
        public override bool Equals(object obj)
        {
            Channel rhs = obj as Channel;
            if (rhs == null)
            {
                return false;
            }

            return ((this.CarrierFrequency == rhs.CarrierFrequency) &&
                    (this.PhysicalChannel == rhs.PhysicalChannel) &&
                    (this.MajorChannel == rhs.MajorChannel) &&
                    (this.MinorChannel == rhs.MinorChannel));
        }

        /// <summary>
        /// Calculates a hash code for this object
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() +
                  (this.PhysicalChannel ^ this.CarrierFrequency ^ this.MajorChannel ^ this.MinorChannel);
        }

        /// <summary>
        /// Compares "logical channel".
        /// </summary>
        /// <remarks>
        /// If PhysicalChannel is set, and MajorChannel is not, it is assumed to be an analog channel, so that will be the compared value.
        /// If MajorChannel is set (> 0) it will be used first.
        /// Additionally, the MinorChannel will be further used, if MajorChannel is set, for comparison.
        /// </remarks>
        /// <param name="other">channel to compare to this one</param>
        /// <returns></returns>
        public int CompareTo(Channel other)
        {
            if (other == null)
            {
                return 1;
            }

            //determine this "primary channel", if it is the Physical Channel or Major Channel
            int thisPrimaryChannel = this.PhysicalChannel;
            if (this.MajorChannel > 0)
            {
                thisPrimaryChannel = this.MajorChannel;
            }
            //determine other "primary channel"
            int otherPrimaryChannel = other.PhysicalChannel;
            if (other.MajorChannel > 0)
            {
                otherPrimaryChannel = other.MajorChannel;
            }


            if (thisPrimaryChannel == otherPrimaryChannel)
            {
                //if this has no MinorChannel, and other does have one...
                if ((this.MinorChannel < 0) && (other.MinorChannel > -1))
                {
                    return -1;
                }
                //if this does have a MinorChannel, and other does not...
                else if ((this.MinorChannel > -1) && (other.MinorChannel < 0))
                {
                    return 1;
                }
                else
                {
                    return this.MinorChannel - other.MinorChannel;
                }

            }
            else
            {
                return thisPrimaryChannel - otherPrimaryChannel;
            }
        }

        #endregion
    }
}
