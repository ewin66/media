using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Describes a media source 
    /// </summary>
    [Serializable]
    public class StreamSourceInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public StreamSourceInfo()
        {
        }

        /// <summary>
        /// Creates a shallow copy of another StreamSourceInfo
        /// </summary>
        /// <param name="rhs">item to copy</param>
        public StreamSourceInfo(StreamSourceInfo rhs)
        {
            _sourceName = rhs.SourceName;
            _sourceType = rhs.SourceType;
            _liveSource = rhs.LiveSource;
            _description = rhs.Description;
            _maxRecordingChunkMinutes = rhs.MaxRecordingChunkMinutes;
            _syncToleranceMilliseconds = rhs.SyncToleranceMilliseconds;
            _maxQueueDuration = rhs.MaxQueueDuration;
            _maxClients = rhs.MaxClients;
            _sinkAddress = rhs.SinkAddress;
            if (rhs.LogicalGroupSourceNames != null)
            {
                _logicalGroupSourceNames = new List<string>();
                foreach (string logicalGroupSourceName in rhs.LogicalGroupSourceNames)
                {
                    _logicalGroupSourceNames.Add(logicalGroupSourceName);
                }
            }
            if (rhs.ProfileGroupNames != null)
            {
                _profileGroupNames = new List<string>();
                foreach (string profileGroupName in rhs.ProfileGroupNames)
                {
                    _profileGroupNames.Add(profileGroupName);
                }
            }
            _deviceAddress = rhs.DeviceAddress;
            _cameraControl = rhs.CameraControl;
            _microwave = rhs.MicrowaveControl;
            _tvTuner = rhs.TVTuner;
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

        private string _sourceName;
        /// <summary>
        /// A unique name identifying this source
        /// </summary>
        [XmlElement]
        public string SourceName
        {
            get
            {
                return _sourceName;
            }
            set
            {
                if (_sourceName != value)
                {
                    _sourceName = value;
                    NotifyPropertyChanged("SourceName");
                }
            }
        }
        private SourceType _sourceType;
        /// <summary>
        /// The type of source this is (defines the specific graph on the server)
        /// </summary>
        [XmlElement]
        public SourceType SourceType
        {
            get
            {
                return _sourceType;
            }
            set
            {
                if (_sourceType != value)
                {
                    _sourceType = value;
                    NotifyPropertyChanged("SourceType");
                }
            }
        }

        private bool _hidden = false;
        /// <summary>
        /// If set to true, client applications should ignore this source
        /// </summary>
//        [DataMember]
        [XmlElement, DefaultValue(false)]
        public bool Hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                if (_hidden != value)
                {
                    _hidden = value;
                    NotifyPropertyChanged("Hidden");
                }
            }
        }

        private bool _liveSource = true;
        /// <summary>
        /// True if this is a live source. Please reference the wiki
        /// </summary>
        [XmlElement, DefaultValue(true)]
        public bool LiveSource
        {
            get
            {
                return _liveSource;
            }
            set
            {
                if (_liveSource != value)
                {
                    _liveSource = value;
                    NotifyPropertyChanged("LiveSource");
                }
            }
        }

        private string _description;
        /// <summary>
        /// Description of this source. Displayed to humans.
        /// </summary>
        [XmlElement]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private int _maxRecordingChunkMinutes = 0;
        /// <summary>
        /// Maximum length of time in minutes per recorded chunk
        /// </summary>
        [XmlElement, DefaultValue(0)]
        public int MaxRecordingChunkMinutes
        {
            get
            {
                return _maxRecordingChunkMinutes;
            }
            set
            {
                if (_maxRecordingChunkMinutes != value)
                {
                    _maxRecordingChunkMinutes = value;
                    NotifyPropertyChanged("MaxRecordingChunkMinutes");
                }
            }
        }

        private int _syncToleranceMilliseconds = 3000;
        /// <summary>
        /// Tolerance in milliseconds for video and audio to become desynchronized
        /// </summary>
//        [DataMember]
        [XmlElement, DefaultValue(3000)]
        public int SyncToleranceMilliseconds
        {
            get
            {
                return _syncToleranceMilliseconds;
            }
            set
            {
                if (_syncToleranceMilliseconds != value)
                {
                    _syncToleranceMilliseconds = value;
                    NotifyPropertyChanged("SyncToleranceMilliseconds");
                }
            }
        }

        private double _maxQueueDuration = 0.5;
        /// <summary>
        /// When <see cref="M:LiveSource"/> is false, this is the amount of time, in seconds, to buffer before flushing
        /// </summary>
        [XmlElement, DefaultValue(0.5)]
        public double MaxQueueDuration
        {
            get
            {
                return _maxQueueDuration;
            }
            set
            {
                if (_maxQueueDuration != value)
                {
                    _maxQueueDuration = value;
                    NotifyPropertyChanged("MaxQueueDuration");
                }
            }
        }

        private int _maxClients = 99;
        /// <summary>
        /// Maximum number of allowed clients on this source
        /// </summary>
        [XmlElement, DefaultValue(99)]
        public int MaxClients
        {
            get
            {
                return _maxClients;
            }
            set
            {
                if (_maxClients != value)
                {
                    _maxClients = value;
                    NotifyPropertyChanged("MaxClients");
                }
            }
        }

        private string _sinkAddress;
        /// <summary>
        /// SinkAddress. See the wiki
        /// </summary>
        [XmlElement]
        public string SinkAddress
        {
            get
            {
                return _sinkAddress;
            }
            set
            {
                if (_sinkAddress != value)
                {
                    _sinkAddress = value;
                    NotifyPropertyChanged("SinkAddress");
                }
            }
        }

        private List<string> _logicalGroupSourceNames;
        /// <summary>
        /// This list is used in conjunction with SourceType.LogicalGroup to define
        /// a list of sources that belong to the logical group.
        /// </summary>
        [XmlElement]
        public List<string> LogicalGroupSourceNames
        {
            get
            {
                return _logicalGroupSourceNames;
            }
            set
            {
                if (_logicalGroupSourceNames != value)
                {
                    _logicalGroupSourceNames = value;
                    NotifyPropertyChanged("LogicalGroupSourceNames");
                }
            }
        }

        private List<string> _profileGroupNames;
        /// <summary>
        /// a list of strings pointing to the names of <see cref="T:ProfileGroup"/> that are known the server.
        /// </summary>
        [XmlElement]
        public List<string> ProfileGroupNames
        {
            get
            {
                return _profileGroupNames;
            }
            set
            {
                if (_profileGroupNames != value)
                {
                    _profileGroupNames = value;
                    NotifyPropertyChanged("ProfileGroupNames");
                }
            }
        }

        private DeviceAddress _deviceAddress;
        /// <summary>
        /// Used to define what hardware and input to use
        /// </summary>
        [XmlElement]
        public DeviceAddress DeviceAddress
        {
            get
            {
                return _deviceAddress;
            }
            set
            {
                if (_deviceAddress != value)
                {
                    _deviceAddress = value;
                    NotifyPropertyChanged("DeviceAddress");
                }
            }
        }

        private CameraControlInfo _cameraControl;
        /// <summary>
        /// Defines any information regarding an attached camera (if said camera is controllable)
        /// </summary>
        [XmlElement]
        public CameraControlInfo CameraControl
        {
            get
            {
                return _cameraControl;
            }
            set
            {
                if (_cameraControl != value)
                {
                    _cameraControl = value;
                    NotifyPropertyChanged("CameraControl");
                }
            }
        }

        private TVSourceInfo _tvTuner = null;
        /// <summary>
        /// Defines any additional information regarding the TV tuner (if this is a TV source)
        /// </summary>
        [XmlElement]
        public TVSourceInfo TVTuner
        {
            get
            {
                return _tvTuner;
            }
            set
            {
                if (_tvTuner != value)
                {
                    _tvTuner = value;
                    NotifyPropertyChanged("TVTuner");
                }
            }
        }

        private MicrowaveControlInfo _microwave;
        /// <summary>
        /// Defines how to control the microwave receiver, if this source is a microwave RX that is controllable.
        /// </summary>
        [XmlElement]
        public MicrowaveControlInfo MicrowaveControl
        {
            get
            {
                return _microwave;
            }
            set
            {
                if (_microwave != value)
                {
                    _microwave = value;
                    NotifyPropertyChanged("MicrowaveControl");
                }
            }
        }

        private Boolean _hasAudio = false;

        [XmlElement]
        public Boolean HasAudio
        {
            get
            {
                return _hasAudio;
            }
            set
            {
                if (value != _hasAudio)
                {
                    _hasAudio = value;
                    NotifyPropertyChanged("HasAudio");
                }
            }
        }
    }
}
