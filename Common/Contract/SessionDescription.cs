using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Used to describe a server/client session. The server generates these objects and hands them to the client
    /// when a session is successfully established.
    /// </summary>
    /// <seealso cref="T:IStream"/>
    /// <seealso cref="T:ITVStream"/>
    [Serializable()]
    public class SessionDescription
    {
        private string _sourceName;
        /// <summary>
        /// The source name that the client has established a session for. 
        /// </summary>
        /// <seealso cref="T:ServerInfo"/>
        /// <seealso cref="T:StreamSourceInfo"/>
        public string SourceName
        {
            set
            {
                _sourceName = value;
            }
            get
            {
                return _sourceName;
            }
        }

        private string _sinkURL;
        /// <summary>
        /// The URL to connect to where the media stream is being sinked to.
        /// </summary>
        public string SinkURL
        {
            set
            {
                _sinkURL = value;
            }
            get
            {
                return _sinkURL;
            }
        }

        private TVSessionInfo _tvSessionInfo;
        /// <summary>
        /// If non-null, then the session is a TV Source.
        /// </summary>
        public TVSessionInfo TVSessionInfo
        {
            get
            {
                return _tvSessionInfo;
            }
            set
            {
                _tvSessionInfo = value;
            }
        }

        private int _streamTimeLimit;
        /// <summary>
        /// If greater than zero, the client should auto-disconnect after the given time (in milliseconds)
        /// </summary>
        public int StreamTimeLimit
        {
            get
            {
                return _streamTimeLimit;
            }
            set
            {
                _streamTimeLimit = value;
            }
        }

        private ProfileGroups _profileGroups;
        /// <summary>
        /// The ProfileGroups containing the <see cref="T:ProfileGroup"/> and <see cref="T:Profile"/> accepted for this source.
        /// </summary>
        /// <seealso cref="M:IStream.SetProfile"/>
        public ProfileGroups ProfileGroups
        {
            get
            {
                return _profileGroups;
            }
            set
            {
                _profileGroups = value;
            }
        }

        private string _currentProfileName;
        /// <summary>
        /// The currently active current profile name.
        /// </summary>
        /// <remarks>
        /// Check the value of <see cref="P:CurrentProfileIsCustom"/>;
        /// If true, then this refers to a custom profile.
        /// If false, then it refers to a profile that can be found in the <see cref="P:ProfileGroups"/> collection.
        /// </remarks>
        public string CurrentProfileName
        {
            get
            {
                return _currentProfileName;
            }
            set
            {
                _currentProfileName = value;
            }
        }

        /// <summary>
        /// Returns true if the current profile is a custom profile
        /// </summary>
        [XmlIgnore]
        [DataMember(IsRequired = false)]
        public bool CurrentProfileIsCustom
        {
            get
            {
                if (this.CurrentProfileName != null)
                {
                    return this.CurrentProfileName.ToLowerInvariant().Contains("custom");
                }
                return false;
            }
        }

        /// <summary>
        /// Fetches the Current Profile based on the CurrentProfileName and the ProfileGroups properties.
        /// Returns null on error, if the specified profile does not exist in its specified group,
        /// or if a custom profile is in use.
        /// </summary>
        [XmlIgnore]
        [DataMember(IsRequired = false)]
        public Profile CurrentProfile
        {
            get
            {
                try
                {
                    if ((CurrentProfileIsCustom) || (ProfileGroups == null))
                    {
                        return null;
                    }

                    string[] parts = Profile.GetProfileNameParts(this.CurrentProfileName);

                    if (ProfileGroups.Items != null)
                    {
                        if (ProfileGroups.Items.Count > 0)
                        {
                            foreach (ProfileGroup g in ProfileGroups.Items)
                            {
                                if (parts[0].Equals(g.Name))
                                {
                                    return g[parts[1]];
                                }
                            }
                        }
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
