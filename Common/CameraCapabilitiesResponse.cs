using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// A response to an inquiry describing the capabilites a camera has.
    /// </summary>
    [Serializable()]
    [Obsolete("Use CameraCapabilitiesAndLimits", true)]
    public class CameraCapabilitiesResponse
    {
        private DateTime _responseTime = DateTime.Now;
        /// <summary>
        /// The time at which the response was generated
        /// </summary>
        public DateTime ResponseTime
        {
            get
            {
                return _responseTime;
            }
            set
            {
                _responseTime = value;
            }
        }

        private string _sourceName;
        /// <summary>
        /// The SourceName <seealso cref="StreamSourceInfo"/> the camera is associated with.
        /// </summary>
        public string SourceName
        {
            get
            {
                return _sourceName;
            }
            set
            {
                _sourceName = value;
            }
        }

        private bool _hasCameraControl = false;
        /// <summary>
        /// True if this client has acquired camera control
        /// </summary>
        public bool HasCameraControl
        {
            get
            {
                return _hasCameraControl;
            }
            set
            {
                _hasCameraControl = value;
            }
        }

        private PTZType _ptzType;
        /// <summary>
        /// The particular type of PTZ camera
        /// </summary>
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

        private bool _isAvailable = false;
        /// <summary>
        /// True if the camera is available for use.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
            set
            {
                _isAvailable = value;
            }
        }

        private string _message;
        /// <summary>
        /// Any message associated with the response.
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        private CameraCapabilitiesAndLimits _capabilities;
        /// <summary>
        /// Specific capabilities the camera has.
        /// </summary>
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
