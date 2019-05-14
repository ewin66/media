using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Describes information about a particular Media Server
    /// </summary>
    [Serializable]
    public class ServerInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property on this object has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        private string _serverName;
        /// <summary>
        /// Friendly name of this server
        /// </summary>
        public string ServerName
        {
            get
            {
                return _serverName;
            }
            set
            {
                if (_serverName != value)
                {
                    _serverName = value;
                    NotifyPropertyChanged("ServerName");
                }
            }
        }

        private string _serverAddress;
        /// <summary>
        /// IP address of this server, in string format
        /// </summary>
        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
            set
            {
                if (_serverAddress != value)
                {
                    _serverAddress = value;
                    NotifyPropertyChanged("ServerAddress");
                }
            }
        }

        private string _versionInfo;
        /// <summary>
        /// A string detailing the version information of the server
        /// </summary>
        public string VersionInfo
        {
            get
            {
                return _versionInfo;
            }
            set
            {
                if (value != _versionInfo)
                {
                    _versionInfo = value;
                    NotifyPropertyChanged("VersionInfo");
                }
            }
        }

        private ProfileGroups _profileGroups;
        /// <summary>
        /// All Profile Groups configured on this server
        /// </summary>
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

        private StreamSources _streamSources;
        /// <summary>
        /// The configured stream sources for this server
        /// </summary>
        public StreamSources StreamSources
        {
            get
            {
                return _streamSources;
            }
            set
            {
                _streamSources = value;
            }
        }

        private List<ServerInfo> _originServers;
        /// <summary>
        /// If this list is non-null and non-empty, then this server can act as a restreamer to reach these listed servers
        /// </summary>
        public List<ServerInfo> OriginServers
        {
            get
            {
                return _originServers;
            }
            set
            {
                _originServers = value;
            }
        }


        private int _revisionNumber;
        /// <summary>
        /// A number indicating the revision of this structure.
        /// If this number changes since the last time it has been checked, the whole structure must be re-retreived
        /// </summary>
        [DefaultValue(0)]
        public int RevisionNumber
        {
            get
            {
                return _revisionNumber;
            }
            set
            {
                if (_revisionNumber != value)
                {
                    _revisionNumber = value;
                    NotifyPropertyChanged("RevisionNumber");
                }
            }
        }
    }
}
