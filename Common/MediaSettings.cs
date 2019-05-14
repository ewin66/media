using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Parent class of VideoSettings and AudioSettings
    /// </summary>
    [Serializable()]
    public class MediaSettings
    {
        private VBR _vbr;
        /// <summary>
        /// If non-null, then ConstantBitRate should be ignored.
        /// </summary>
        [XmlElement]
        public VBR VBR
        {
            get
            {
                return _vbr;
            }
            set
            {
                _vbr = value;
            }
        }

        private int _constantBitRate;
        /// <summary>
        /// The constant bitrate of this media in Kbps
        /// </summary>
        [XmlElement, DefaultValue(0)]
        public int ConstantBitRate
        {
            get
            {
                return _constantBitRate;
            }
            set
            {
                _constantBitRate = value;
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
            MediaSettings rhs = obj as MediaSettings;
            if (((object)rhs) == null)
                return false;

            return ((this.VBR == rhs.VBR) &&
                    (this.ConstantBitRate == rhs.ConstantBitRate));
        }

        /// <summary>
        /// Returns a hash code for this settings object
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            return (this.ConstantBitRate.GetHashCode());
        }

        /// <summary>
        /// Determines the equality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a == b, and false if a != b</returns>
        public static bool operator ==(MediaSettings a, MediaSettings b)
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
        public static bool operator !=(MediaSettings a, MediaSettings b)
        {
            return !(a == b);
        }
    }
}
