using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Supported SourceTypes on the Media Server
    /// </summary>
    [Serializable()]
    public enum SourceType
    {
        RTSP,
        MPEG2_TS,
        BouncingBall,
        BouncingBallDVR,
        FVSC,
        MX,
        HTTP_GET,
        /// </summary>
        /// <summary>
        /// LEADTOOLS restreamer
        /// </summary>
        LTRestreamer,
        /// <summary>
        /// Generic Capture Device (has a pin named "Capture" that can
        /// be fed into the LEADTOOLS H.264 Encoder filter)
        /// </summary>
        CaptureSourceDVR,
        /// <summary>
        /// Generic Capture Device (has a pin named "Capture" that can
        /// be fed into the LEADTOOLS H.264 Encoder filter)
        /// </summary>
        DVRtoMPEG2TS,
        /// <summary>
        /// DVR to LEAD Network Sink
        /// </summary>
        DVRLeadNetSink,
        /// <summary>
        /// DVR to DVR with transcoding
        /// </summary>
        DVRtoDVR,
        /// Screen Capture stream
        /// </summary>
        ScreenCapture,
        /// <summary>
        /// Analog TV, from a Happuage PVR 150 card
        /// </summary>
        WinTV150,
        /// <summary>
        /// Analog TV from a Happuage HVR 1600 card
        /// </summary>
        WinTV418,
        /// <summary>
        /// Digital/HDTV from a Happuage HVR 1600 card
        /// </summary>
        WinTV418ATSC,
        /// <summary>
        /// Analog TV from a PVR 500 (?)
        /// </summary>
        WinTV500,
        /// <summary>
        /// FastVDO Smart Capture to DVR
        /// </summary>
        FastVDODVR,
        /// <summary>
        /// FastVDO Smart Capture UDP Push to WOWZA
        /// </summary>
        PushFVSC,
        /// <summary>
        /// LEADTOOLS to Windows Media (WebRestreamer)
        /// </summary>
        LT2WM,
        /// <summary>
        /// Mango to DVR Sink
        /// </summary>
        MangoDVR,
        /// <summary>
        /// Mango to LEAD MPEG2 Transport UDP Sink
        /// </summary>
        LogicalGroup,
        /// <summary>
        /// Indicates the source is a restreamer. Actual type (<see cref="SourceType.LTRestreamer"/>, <see cref="SourceType.LT2WM"/>) is resolved at run time
        /// </summary>
        Restreamer,
        /// <summary>
        /// Sensoray Model 314 Frame Grabber
        /// </summary>
        SRM314,
        /// <summary>
        /// RTSP (Real-Time Streaming Protocol -- Elecard filters)
        /// </summary>
        RTSP_Elecard
    }
}
