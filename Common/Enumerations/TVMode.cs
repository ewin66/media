using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Logical mode that a TV tuner may be in
    /// </summary>
    [Serializable]
    public enum TVMode
    {
        /// <summary>
        /// Broadcast, over-the-air
        /// </summary>
        Broadcast,
        /// <summary>
        /// Local satellite receiver
        /// </summary>
        Satellite
    }
}
