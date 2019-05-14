using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// This are the accepted/valid frame rates.
    /// </summary>
    /// <seealso cref="T:FutureConcepts.Media.VideoFrameRateUnits"/>
    public static class ValidFrameRates
    {
        /// <summary>
        /// The list of frame rates supported at <see cref="F:VideoFrameRateUnits.FramesPerSecond"/>
        /// </summary>
        public static readonly double[] FramesPerSecond = { 1, 3, 5, 10, 15, 30 };
        /// <summary>
        /// The list of frame rates supported at <see cref="F:VideoFrameRateUnits.FramesPerMinute"/>
        /// </summary>
        public static readonly double[] FramesPerMinute = { 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 15, 18, 20, 24, 25, 30, 36, 40, 45, 50, 60 };
    }

    /// <summary>
    /// Describes the parameters of a video stream
    /// </summary>
    [Serializable()]
    public class VideoSettings : MediaSettings, ICloneable
    {
        private VideoCodecType _codecType = VideoCodecType.Undefined;
        
        /// <summary>
        /// The codec used for this stream
        /// </summary>
        [XmlElement, DefaultValue(VideoCodecType.Undefined)]
        public VideoCodecType CodecType
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

        private VideoImageSize _imageSize = VideoImageSize.Undefined;
        /// <summary>
        /// Gets or sets the resolution/image size of this stream. 
        /// </summary>
        [XmlElement, DefaultValue(VideoImageSize.Undefined)]
        public VideoImageSize ImageSize
        {
            get
            {
                return _imageSize;
            }
            set
            {
                _imageSize = value;
            }
        }

        private int _frameRate = -1;
        /// <summary>
        /// Gets or sets the frame rate. Used in conjunction with the <see cref="M:FrameRateUnits"/> field.
        /// </summary>
        [XmlElement, DefaultValue(-1)]
        public int FrameRate
        {
            get
            {
                return _frameRate;
            }
            set
            {
                _frameRate = value;
            }
        }

        private VideoFrameRateUnits _frameRateUnits = VideoFrameRateUnits.Undefined;
        /// <summary>
        /// Gets or sets the frame rate units. Used in conjunction with the <see cref="M:FrameRate"/> field.
        /// </summary>
        [XmlElement, DefaultValue(VideoFrameRateUnits.Undefined)]
        public VideoFrameRateUnits FrameRateUnits
        {
            get
            {
                return _frameRateUnits;
            }
            set
            {
                _frameRateUnits = value;
            }
        }

        private int _keyFrameRate = -1;
        /// <summary>
        /// Gets or sets the Key Frame Rate. Used in conjunction with the <see cref="M:FrameRate"/> field.
        /// </summary>
        [XmlElement, DefaultValue(-1)]
        public int KeyFrameRate
        {
            get
            {
                return _keyFrameRate;
            }
            set
            {
                _keyFrameRate = value;
            }
        }

        private VideoQuality _quality = VideoQuality.Undefined;
        /// <summary>
        /// A qualitative measure of video stream quality.
        /// </summary>
        [XmlElement, DefaultValue(VideoQuality.Undefined)]
        public VideoQuality Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                _quality = value;
            }
        }

        /// <summary>
        /// Determines the equality of <c>this</c> and a specified object.
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if <c>this</c> equals <paramref name="obj">obj</paramref></returns>
        public override bool Equals(object obj)
        {
            VideoSettings rhs = obj as VideoSettings;
            if (((object)rhs) == null)
            {
                return false;
            }
            return (base.Equals(obj) &&
                    (this.CodecType == rhs.CodecType) &&
                    (this.ImageSize == rhs.ImageSize) &&
                    (this.FrameRate == rhs.FrameRate) &&
                    (this.KeyFrameRate == rhs.KeyFrameRate) &&
                    (this.Quality == rhs.Quality) &&
                    (this.FrameRateUnits == rhs.FrameRateUnits));
        }

        /// <summary>
        /// Generates a hash code value for this object
        /// </summary>
        /// <returns>a hash code</returns>
        public override int GetHashCode()
        {
            return (base.GetHashCode() ^ (int)this.ImageSize ^ this.KeyFrameRate ^ this.FrameRate);
        }

        /// <summary>
        /// Determines the equality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a == b, and false if a != b</returns>
        public static bool operator ==(VideoSettings a, VideoSettings b)
        {
            if ((((object)a) == null) && (((object)b) == null))
            {
                return true;
            }
            if (((object)a) != null)
            {
                return a.Equals(b);
            }
            return false;
        }

        /// <summary>
        /// Determines the inequality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a != b, and false if a == b</returns>
        public static bool operator !=(VideoSettings a, VideoSettings b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Creates a clone of this structure
        /// </summary>
        /// <returns>returns a new copy of <c>this</c>, with the same values</returns>
        public object Clone()
        {
            VideoSettings v = new VideoSettings();

            //MediaSettings members
            v.ConstantBitRate = this.ConstantBitRate;
            v.VBR = (this.VBR != null) ? (VBR)this.VBR.Clone() : null;

            //VideoSettings members
            v.CodecType = this.CodecType;
            v.ImageSize = this.ImageSize;
            v.Quality = this.Quality;
            v.FrameRate = this.FrameRate;
            v.KeyFrameRate = this.KeyFrameRate;
            v.FrameRateUnits = this.FrameRateUnits;

            return v;
        }
    }
}
