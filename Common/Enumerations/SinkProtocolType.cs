using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// The type of transport protocol used to sink a media stream
    /// </summary>
    [Serializable]
    public enum SinkProtocolType
    {
        /// <summary>
        /// Let the server decide
        /// </summary>
        Default = 0,
        /// <summary>
        /// LEADTOOLS Streaming Format TCP
        /// </summary>
        LTSF_TCP,
        /// <summary>
        /// LEADTOOLS Streaming Format UDP
        /// </summary>
        LTSF_DGRAM,
        /// <summary>
        /// HTTP streaming
        /// </summary>
        HTTP,
        /// <summary>
        /// Real-time Transport Protocol
        /// </summary>
        RTP,
        /// <summary>
        /// Real-time Streaming Protocol
        /// </summary>
        RTSP
    }
}
