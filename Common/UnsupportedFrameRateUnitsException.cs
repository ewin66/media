using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Thrown if a certain VideoFrameRateUnits value cannot be used
    /// </summary>
    public class UnsupportedFrameRateUnitsException : Exception
    {
        /// <summary>
        /// Creates a new instance regarding a specific <see cref="T:VideoFrameRateUnits"/> value.
        /// </summary>
        /// <param name="units">the unsupported <see cref="T:VideoFrameRateUnits"/> value</param>
        public UnsupportedFrameRateUnitsException(VideoFrameRateUnits units)
            : base("An unsupported FrameRateUnits '" + units.ToString() + "' was specified")
        {
            _units = units;
        }

        private VideoFrameRateUnits _units;
        /// <summary>
        /// The unsupported <see cref="T:VideoFrameRateUnits"/> value.
        /// </summary>
        public VideoFrameRateUnits FrameRateUnits
        {
            get
            {
                return _units;
            }
        }
    }
}
