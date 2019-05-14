using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Defines the units used when expressing frame rates
    /// </summary>
    [Serializable]
    public enum VideoFrameRateUnits
    {
        /// <summary>
        /// Frames per second
        /// </summary>
        FramesPerSecond,
        /// <summary>
        /// Frames per minute
        /// </summary>
        FramesPerMinute,
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined
    }
}
