using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Supported Video codecs
    /// </summary>
    [Serializable]
    public enum VideoCodecType
    {
        /// <summary>
        /// H.263
        /// </summary>
        H263,
        /// <summary>
        /// H.264 / MPEG4 Part 10 / AVC
        /// </summary>
        H264,
        /// <summary>
        /// MPEG1
        /// </summary>
        MPEG1,
        /// <summary>
        /// MPEG2 (any profile)
        /// </summary>
        MPEG2,
        /// <summary>
        /// MPEG4 Part 2 (any profile)
        /// </summary>
        MPEG4,
        /// <summary>
        /// MPEG4 Part 2 Advanced Simpled Profile
        /// </summary>
        MPEG4_ASP,
        /// <summary>
        /// Motion JPEG
        /// </summary>
        MJPEG,
        /// <summary>
        /// Motion JPEG 2000
        /// </summary>
        MJPEG2000,
        /// <summary>
        /// Windows Media Video
        /// </summary>
        WMV,
        /// <summary>
        /// Xiph.org Ogg Theora
        /// </summary>
        Theora,
        /// <summary>
        /// Google VP8
        /// </summary>
        VP8,
        /// <summary>
        /// Microsoft VC-1
        /// </summary>
        VC1,
        /// <summary>
        /// Undefined / Don't Care
        /// </summary>
        Undefined
    }
}
