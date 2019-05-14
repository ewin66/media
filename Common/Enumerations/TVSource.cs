using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Logical source where a TV stream may be coming from
    /// </summary>
    [Serializable]
    public enum TVSource
    {
        /// <summary>
        /// Analog, potentially over-the-air, NTSC
        /// </summary>
        LocalAnalog,
        /// <summary>
        /// Digital, generally over-the-air, ATSC
        /// </summary>
        LocalDigital,
        /// <summary>
        /// Network stream
        /// </summary>
        Network
    }
}
