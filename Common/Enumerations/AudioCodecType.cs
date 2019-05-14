using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Supported Audio codecs
    /// </summary>
    [Serializable]
    public enum AudioCodecType
    {
        /// <summary>
        /// Advanced Audio Coding
        /// </summary>
        AAC,
        /// <summary>
        /// Adaptive Multi-Rate
        /// </summary>
        AMR,
        /// <summary>
        /// Dolby Digital / AC3
        /// </summary>
        DolbyDigital,
        /// <summary>
        /// MPEG1 Layer 2
        /// </summary>
        MP2,
        /// <summary>
        /// MPEG1 Layer 3
        /// </summary>
        MP3,
        /// <summary>
        /// Windows Media Audio
        /// </summary>
        WMA,
        /// <summary>
        /// Xiph.org Ogg Vorbis
        /// </summary>
        Vorbis,
        /// <summary>
        /// Undefined / Don't Care
        /// </summary>
        Undefined
    }
}
