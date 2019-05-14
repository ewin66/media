using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Defines settings and parameters concerning an audio stream.
    /// </summary>
    [Serializable()]
    public class AudioSettings : MediaSettings, ICloneable
    {
        private AudioCodecType _codecType = AudioCodecType.Undefined;
        /// <summary>
        /// The codec in use.
        /// </summary>
        [XmlElement, DefaultValue(AudioCodecType.Undefined)]
        public AudioCodecType CodecType
        {
            get
            {
                return _codecType;
            }
            set
            {
                _codecType = value;
            }
        }

        /// <summary>
        /// Determines if <c>this</c> is equal to a specifed object.
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>returns true if <c>this</c> is equivelent to <paramref name="obj">obj</paramref>, false if not</returns>
        public override bool Equals(object obj)
        {
            AudioSettings rhs = obj as AudioSettings;
            if (((object)rhs) == null)
                return false;

            return (base.Equals(obj) &&
                    (this.CodecType == rhs.CodecType));
        }

        /// <summary>
        /// Returns the hash code of this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (base.GetHashCode() ^ (int)this.CodecType);
        }

        /// <summary>
        /// Determines the equality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a == b, and false if a != b</returns>
        public static bool operator ==(AudioSettings a, AudioSettings b)
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
        public static bool operator !=(AudioSettings a, AudioSettings b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Creates a copy of this object with the same settings.
        /// </summary>
        /// <returns>a copy of this object with the same settings</returns>
        public object Clone()
        {
            AudioSettings a = new AudioSettings();

            //MediaSettings members
            a.ConstantBitRate = this.ConstantBitRate;
            a.VBR = (this.VBR != null) ? (VBR)this.VBR.Clone() : null;

            //AudioSettings members
            a.CodecType = this.CodecType;

            return a;
        }
    }
}
