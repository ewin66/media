using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Defines Container codecs
    /// </summary>
    [Serializable]
    public enum ContainerCodecType
    {
        /// <summary>
        /// No container -- indicates an elementary stream
        /// </summary>
        None,
        /// <summary>
        /// MPEG 1/2 Program Stream
        /// </summary>
        MPEG_PS,
        /// <summary>
        /// MPEG2 Transport Stream
        /// </summary>
        MPEG2_TS,
        /// <summary>
        /// MPEG4 Part 12 Container
        /// </summary>
        MP4,
        /// <summary>
        /// 3GPP for 3G devices
        /// </summary>
        ThreeGPP,
        /// <summary>
        /// 3GPP2 for CDMA devices
        /// </summary>
        ThreeGPP2,
        /// <summary>
        /// LEADTOOLS Stream Format
        /// </summary>
        LTStream,
        /// <summary>
        /// Audio/Video Interleave
        /// </summary>
        AVI,
        /// <summary>
        /// Advanced Systems Format (Windows Media)
        /// </summary>
        ASF,
        /// <summary>
        /// Xiph.org OGG Container
        /// </summary>
        OGG,
        /// <summary>
        /// Ogg Media Container, as used by LEADTOOLS and OggDS projects
        /// </summary>
        OGM,
        /// <summary>
        /// QuickTime MOV container
        /// </summary>
        QuickTime,
        /// <summary>
        /// Matroska MKV/MKA container
        /// </summary>
        Matroska,
        /// <summary>
        /// Adobe Flash Container
        /// </summary>
        Flash,
        /// <summary>
        /// Google WebM
        /// </summary>
        WebM,
        /// <summary>
        /// Undefined / Don't Care
        /// </summary>
        Undefined
    }
}
