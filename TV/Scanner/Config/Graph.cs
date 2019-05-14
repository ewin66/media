using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FutureConcepts.Media.TV.Scanner.Config
{
    public enum GraphType
    {
        WinTV,
        Network,
        WinTVATSC,
        AverA307Analog,
        AverATSC,
        AverA317Analog,
        AverA323Analog,
        ATSCTuner
    }

    public enum FilterType
    {
        Capture,
        Tuner,
        TVAudio,
        Crossbar,
        VideoEncoder,
        ColorConverter,
        Writer,
        VideoDecoder,
        AudioDecoder,
        AudioVolume,
    }

    /// <summary>
    /// Represents info needed to build a graph
    /// </summary>
    public class Graph
    {
        private TVSource _source;
        /// <summary>
        /// The type of TV Source the graph renders
        /// </summary>
        [XmlAttribute]
        public TVSource Type
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        private GraphType _type;
        /// <summary>
        /// The class that handles the graph building
        /// </summary>
        [XmlElement]
        public GraphType Class
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private List<Filter> _filters;
        /// <summary>
        /// List of filters to use in building the graph
        /// </summary>
        [XmlElement]
        public List<Filter> Filter
        {
            get
            {
                return _filters;
            }
            set
            {
                _filters = value;
            }
        }

        /// <summary>
        /// Locates the Filter info by type
        /// </summary>
        /// <param name="name">type to find</param>
        /// <returns>the filter, or returns null if not found</returns>
        [XmlIgnore]
        public Filter this[FilterType type]
        {
            get
            {
                foreach (Filter f in this.Filter)
                {
                    if (f.Type == type)
                    {
                        return f;
                    }
                }
                //return new Filter();
                return null;
            }
        }

        /// <summary>
        /// Returns a filter that specifies the given codec
        /// </summary>
        /// <param name="videoCodec">a Video Codec to find a filter for</param>
        /// <param name="type">The type of Filter, such as VideoDecoder, Encoder, etc</param>
        /// <returns>The first filter encountered that deals with the given video codec. Returns null if not found.</returns>
        public Filter GetFilterForCodec(VideoCodecType videoCodec, FilterType type)
        {
            foreach (Filter f in this.Filter)
            {
                if ((f.VideoCodec == videoCodec) && (f.Type == type))
                {
                    return f;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a filter that specifies the given codec
        /// </summary>
        /// <param name="audioCodec">an Audio Codec to find a filter for</param>
        /// <param name="type">The type of Filter, such as AudioDecoder, Encoder, etc</param>
        /// <returns>The first filter encountered that deals with the given audio codec. Returns null if not found.</returns>
        public Filter GetFilterForCodec(AudioCodecType audioCodec, FilterType type)
        {
            foreach (Filter f in this.Filter)
            {
                if ((f.AudioCodec == audioCodec) && (f.Type == type))
                {
                    return f;
                }
            }
            return null;
        }

        private VBR _bitrate = new VBR();
        /// <summary>
        /// Specify the min and max bitrates. Set them equal to use a constant bitrate
        /// </summary>
        [XmlElement]
        public VBR VideoBitrate
        {
            get
            {
                return _bitrate;
            }
            set
            {
                _bitrate = value;
            }
        }

        /// <summary>
        /// True if the max and min bitrates are not equal
        /// </summary>
        [XmlIgnore]
        public bool VideoIsVBR
        {
            get
            {
                return (VideoBitrate.MaxBitRate != VideoBitrate.MinBitRate);
            }
        }
    }
}
