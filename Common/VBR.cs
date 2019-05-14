using System;
using System.Collections.Generic;
using System.Text;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Contains a minimum and maximum bitrate, for expressing variable bitrate content
    /// </summary>
    [Serializable()]
    public class VBR : ICloneable
    {
#if COMMENT
        public VBR(int min, int max)
        {
            _minBitRate = min;
            _maxBitRate = max;
        }
#endif

        private int _minBitRate;
        /// <summary>
        /// The minimum bitrate that should be used (in Kbps)
        /// </summary>
        public int MinBitRate
        {
            get
            {
                return _minBitRate;
            }
            set
            {
                _minBitRate = value;
            }
        }

        private int _maxBitRate;
        /// <summary>
        /// The maximum bitrate that should be used (in Kbps)
        /// </summary>
        public int MaxBitRate
        {
            get
            {
                return _maxBitRate;
            }
            set
            {
                _maxBitRate = value;
            }
        }

        /// <summary>
        /// Determines if <c>this</c> is equal to the specified object.
        /// </summary>
        /// <remarks>Does not compare Enabled status.</remarks>
        /// <param name="obj">object to compare</param>
        /// <returns>true if this and obj are equal, false if not</returns>
        public override bool Equals(object obj)
        {
            VBR rhs = obj as VBR;
            if (((object)rhs) == null)
                return false;

            return ((this.MaxBitRate == rhs.MaxBitRate) &&
                    (this.MinBitRate == rhs.MinBitRate));
        }

        /// <summary>
        /// Calculates the hash code for the VBR structure
        /// </summary>
        /// <returns>min bit rate XOR'd with the max bit rate</returns>
        public override int GetHashCode()
        {
            return (this.MinBitRate ^ this.MaxBitRate);
        }

        /// <summary>
        /// Determines the equality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a == b, and false if a != b</returns>
        public static bool operator ==(VBR a, VBR b)
        {
            if ((((object)a) == null) && (((object)b) == null))
                return true;

            if (((object)a) != null)
                return a.Equals(b);

            return false;
        }

        /// <summary>
        /// Determines the inequality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a != b, and false if a == b</returns>
        public static bool operator !=(VBR a, VBR b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Creates a clone of this VBR object
        /// </summary>
        /// <returns>a new instance with the same settings</returns>
        public object Clone()
        {
            VBR vbr = new VBR();
            vbr.MaxBitRate = this.MaxBitRate;
            vbr.MinBitRate = this.MinBitRate;

            return vbr;
        }
    }
}