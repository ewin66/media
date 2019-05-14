using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Supported SourceTypes on the Media Server
    /// </summary>
    [Serializable()]
    public enum GraphType
    {
        /// <summary>
        /// Bouncing Ball, for testing
        /// </summary>
        BouncingBall,
        /// <summary>
        /// Bouncing Ball, recording, for testing
        /// </summary>
        BouncingBallDVR,
        /// <summary>
        /// Stream from a pre-recorded file
        /// </summary>
        File,
        /// <summary>
        /// Mango
        /// </summary>
        MX,
        /// <summary>
        /// Mango with motion detection
        /// </summary>
        MXMotion,
        /// <summary>
        /// LEADTOOLS restreamer
        /// </summary>
        LTRestreamer,
        /// <summary>
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
        /// FastVDO Smart Capture
        /// </summary>
        FVSC,
        /// <summary>
        /// LEADTOOLS to Windows Media (WebRestreamer)
        /// </summary>
        LT2WM,
        /// <summary>
        /// Mango with recording
        /// </summary>
        MangoDVR,
        /// <summary>
        /// Special, resolved at run time. See the wiki
        /// </summary>
        LogicalGroup,
        /// <summary>
        /// Indicates the source is a restreamer. Actual type (<see cref="SourceType.LTRestreamer"/>, <see cref="SourceType.LT2WM"/>) is resolved at run time
        /// </summary>
        Restreamer,
        /// <summary>
        /// Sensoray Model 314 Frame Grabber
        /// </summary>
        SRM314
    }
}
