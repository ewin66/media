using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media.TV.Scanner.Config
{
    /// <summary>
    /// This class represents an entry for one DirectShow filter used in a TV graph.
    /// Kevin Dixon  12/08/2008
    /// </summary>
    [Serializable()]
    public class Filter
    {
        private FilterType _filterType;
        /// <summary>
        /// The type of filter this is.
        /// </summary>
        [XmlAttribute]
        public FilterType Type
        {
            get
            {
                return _filterType;
            }
            set
            {
                _filterType = value;
            }
        }

        private string _name;
        /// <summary>
        /// The name of the filter to use.
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private VideoCodecType _videoCodecType = VideoCodecType.Undefined;
        /// <summary>
        /// Indicates that this filter deals in a specific video codec
        /// </summary>
        [XmlAttribute, DefaultValue(VideoCodecType.Undefined)]
        public VideoCodecType VideoCodec
        {
            get
            {
                return _videoCodecType;
            }
            set
            {
                _videoCodecType = value;
            }
        }

        private AudioCodecType _audioCodecType = AudioCodecType.Undefined;
        /// <summary>
        /// Indicates that this filter deals in a specific audio codec
        /// </summary>
        [XmlAttribute, DefaultValue(AudioCodecType.Undefined)]
        public AudioCodecType AudioCodec
        {
            get
            {
                return _audioCodecType;
            }
            set
            {
                _audioCodecType = value;
            }
        }

        private List<string> _inPin;
        /// <summary>
        /// The name of the input pin to connect to
        /// </summary>
        [XmlElement]
        public List<string> InPin
        {
            get
            {
                return _inPin;
            }
            set
            {
                _inPin = value;
            }
        }

        private List<string> _outPin;
        /// <summary>
        /// The name of the output pin to connect from
        /// </summary>
        [XmlElement]
        public List<string> OutPin
        {
            get
            {
                return _outPin;
            }
            set
            {
                _outPin = value;
            }
        }
    }
}
