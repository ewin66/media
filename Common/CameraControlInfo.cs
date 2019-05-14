using System;
using System.Configuration;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Defines the a type of camera (see <see cref="T:PTZType"/>) and how to connect to it for control purposes.
    /// </summary>
    [Serializable()]
    public class CameraControlInfo
    {
        private PTZType _ptzType;
        /// <summary>
        /// Indicates the type of plug in to use when controlling this camera.
        /// </summary>
        [XmlElement]
        public PTZType PTZType
        {
            get
            {
                return _ptzType;
            }
            set
            {
                _ptzType = value;
            }
        }

        private string _address;
        /// <summary>
        /// An address paramter, usually the name of a Serial port.
        /// </summary>
        [XmlElement]
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        private int _relinquishTimer = 30000;
        /// <summary>
        /// If this amount of time elapses between commands, the camera control will be released.
        /// </summary>
        [XmlElement, DefaultValue(30000)]
        public int RelinquishTimer
        {
            get
            {
                return _relinquishTimer;
            }
            set
            {
                _relinquishTimer = value;
            }
        }

        private CameraCapabilitiesAndLimits _capabilities;
        /// <summary>
        /// Details specific additional capabilities of the camera
        /// </summary>
        [XmlElement]
        public CameraCapabilitiesAndLimits Capabilities
        {
            get
            {
                return _capabilities;
            }
            set
            {
                _capabilities = value;
            }
        }
    }
}
