using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Defines an encoding profile for a single logical stream of media
    /// </summary>
    [Serializable()]
    public class Profile : ICloneable
    {
        /// <summary>
        /// Breaks up a fully qualified profile name into its two parts
        /// </summary>
        /// <param name="fullyQualifiedProfileName">fully qualified profile name, in the form "group name:profile name"</param>
        /// <returns>an array of size 2. Index 0 = group name, Index 1 = profile name</returns>
        /// <exception cref="System.ArgumentException">Thrown if the path is not correctly qualified.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the supplied path is null or empty.</exception>
        public static string[] GetProfileNameParts(string fullyQualifiedProfileName)
        {
            if (String.IsNullOrEmpty(fullyQualifiedProfileName))
            {
                throw new ArgumentNullException("profilePath", "Profile path cannot be null or empty.");
            }

            string[] parts = fullyQualifiedProfileName.Split(':');
            if (parts.Length == 2)
            {
                return parts;
            }

            throw new ArgumentException("An invalid profilePath was encountered: \"" + fullyQualifiedProfileName + "\"", "fullyQualifiedProfileName");
        }

        private string _name;
        /// <summary>
        /// The name of this profile. Names should be unique across a group.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private SinkProtocolType _sinkProtocol;
        /// <summary>
        /// The SinkProtocol used to transport this stream. <see cref="SinkProtocolType"/>
        /// </summary>
        [XmlElement, DefaultValue(SinkProtocolType.LTSF_TCP)]
        public SinkProtocolType SinkProtocol
        {
            get
            {
                return _sinkProtocol;
            }
            set
            {
                _sinkProtocol = value;
            }
        }

        private VideoSettings _video;
        /// <summary>
        /// The Settings for the video stream. <see langword="null"/> if there is no video stream.
        /// </summary>
        public VideoSettings Video
        {
            get
            {
                return _video;
            }
            set
            {
                _video = value;
            }
        }

        private AudioSettings _audio;
        /// <summary>
        /// The settings for the audio stream.  <see langword="null"/> if there is no audio stream.
        /// </summary>
        public AudioSettings Audio
        {
            get
            {
                return _audio;
            }
            set
            {
                _audio = value;
            }
        }

        /// <summary>
        /// Compares <c>this</c> to another object to see if they are equal
        /// </summary>
        /// <param name="obj">object to compare to.</param>
        /// <returns>returns true if <paramref name="obj">obj</paramref> is a <c>Profile</c> and is equal to <c>this</c></returns>
        public override bool Equals(object obj)
        {
            Profile rhs = obj as Profile;
            if (((object)rhs) == null)
                return false;

            return ((this.Name == rhs.Name) &&
                    (this.Audio == rhs.Audio) &&
//                    (this.SinkProtocol == rhs.SinkProtocol) &&
                    (this.Video == rhs.Video));
        }

        /// <summary>
        /// Returns the hash code for this object
        /// </summary>
        /// <returns>the hash code for this object</returns>
        public override int GetHashCode()
        {
            if (Name != null)
            {
                return this.Name.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// Checks two Profiles for equality
        /// </summary>
        /// <param name="a">left hand argument</param>
        /// <param name="b">right hand argument</param>
        /// <returns>true if a == b, false if a != b</returns>
        public static bool operator ==(Profile a, Profile b)
        {
            if ((((object)a) == null) && (((object)b) == null))
                return true;

            if (((object)a) != null)
                return a.Equals(b);
            return false;
        }

        /// <summary>
        /// Determines if two Profiles are inequal.
        /// </summary>
        /// <returns>true if a != b, false if a == b</returns>
        public static bool operator !=(Profile a, Profile b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Causes this object to clone itself.
        /// </summary>
        /// <returns>returns a clone of this object.</returns>
        public object Clone()
        {
            Profile clone = new Profile();

            clone.Name = (string)this.Name.Clone();
//            clone.SinkProtocol = this.SinkProtocol;

            if (this.Video != null)
            {
                clone.Video = (VideoSettings)this.Video.Clone();
            }
            else
            {
                clone.Video = null;
            }

            if (this.Audio != null)
            {
                clone.Audio = (AudioSettings)this.Audio.Clone();
            }
            else
            {
                clone.Audio = null;
            }

            return clone;
        }
    }
}
