using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Describes a media source 
    /// </summary>
    [DataContract]
    [Serializable]
    public class SourceDiscoveryDefinition : INotifyPropertyChanged, IExtensibleDataObject
    {
        [NonSerialized]
        private ExtensionDataObject _extensionDataObject;

        /// <summary>
        /// Forward-compatible support
        /// </summary>
        public virtual ExtensionDataObject ExtensionData
        {
            get
            {
                return _extensionDataObject;
            }
            set
            {
                _extensionDataObject = value;
            }
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public SourceDiscoveryDefinition()
        {
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        #endregion

        private string _name;
        /// <summary>
        /// A unique name identifying this source
        /// </summary>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private String _type;
        /// <summary>
        /// The type of source this is (defines the specific graph on the server)
        /// </summary>
        [DataMember]
        public String Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        private String _path;

        [DataMember]
        public String Path1
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        private String _url;

        [DataMember]
        public String URL
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        private int _pollInterval;

        [DataMember]
        public int PollInterval
        {
            get
            {
                return _pollInterval;
            }
            set
            {
                _pollInterval = value;
            }
        }

        private int _refreshInterval;

        [DataMember]
        public int RefreshInterval
        {
            get
            {
                return _refreshInterval;
            }
            set
            {
                _refreshInterval = value;
            }
        }
    }
}
