using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Used to define specific capabilities and limitations of a camera.
    /// </summary>
    [Serializable()]
    public class CameraCapabilitiesAndLimits
    {
        private bool _hasAbsoluteControls = true;

        /// <summary>
        ///  Does the camera control support presets
        /// </summary>
        [XmlAttribute]
        public bool HasAbsoluteControls
        {
            get
            {
                return _hasAbsoluteControls;
            }
            set
            {
                _hasAbsoluteControls = value;
            }
        }

        private bool _hasPan = false;
        /// <summary>
        /// Ability to pan
        /// </summary>
        [XmlAttribute]
        public bool HasPan
        {
            get
            {
                return _hasPan;
            }
            set
            {
                _hasPan = value;
            }
        }

        private bool _hasTilt = false;
        /// <summary>
        /// Ability to tilt
        /// </summary>
        [XmlAttribute]
        public bool HasTilt
        {
            get
            {
                return _hasTilt;
            }
            set
            {
                _hasTilt = value;
            }
        }

        private bool _hasZoom = false;
        /// <summary>
        /// Ability to zoom
        /// </summary>
        [XmlAttribute]
        public bool HasZoom
        {
            get
            {
                return _hasZoom;
            }
            set
            {
                _hasZoom = value;
            }
        }

        private bool _hasDigitalZoom = false;
        /// <summary>
        /// Camera has digital zoom, either in combination with HasZoom, or not.
        /// </summary>
        [XmlAttribute]
        public bool HasDigitalZoom
        {
            get
            {
                return _hasDigitalZoom;
            }
            set
            {
                _hasDigitalZoom = value;
            }
        }



        private bool _hasEmitter = false;
        /// <summary>
        /// The camera has an IR emitter that can be controlled
        /// </summary>
        [XmlAttribute]
        public bool HasEmitter
        {
            get
            {
                return _hasEmitter;
            }
            set
            {
                _hasEmitter = value;
            }
        }

        private bool _hasStabilizer = false;
        /// <summary>
        /// The camera has a stabilization feature that can be controlled
        /// </summary>
        [XmlAttribute]
        public bool HasStabilizer
        {
            get
            {
                return _hasStabilizer;
            }
            set
            {
                _hasStabilizer = value;
            }
        }

        private bool _hasInfrared = false;
        /// <summary>
        /// The camera has an Infrared or Night Vision mode
        /// </summary>
        [XmlAttribute]
        public bool HasInfrared
        {
            get
            {
                return _hasInfrared;
            }
            set
            {
                _hasInfrared = value;
            }
        }

        private bool _hasInverter = false;
        /// <summary>
        /// The camera can invert its image
        /// </summary>
        [XmlAttribute]
        public bool HasInverter
        {
            get
            {
                return _hasInverter;
            }
            set
            {
                _hasInverter = value;
            }
        }

        private bool _hasWiper = false;
        /// <summary>
        /// The camera has a lens wiper
        /// </summary>
        [XmlAttribute]
        public bool HasWiper
        {
            get
            {
                return _hasWiper;
            }
            set
            {
                _hasWiper = value;
            }
        }

        private bool _hasFocus = false;
        /// <summary>
        /// The camera's focus can be manually controlled
        /// </summary>
        [XmlAttribute]
        public bool HasFocus
        {
            get
            {
                return _hasFocus;
            }
            set
            {
                _hasFocus = value;
            }
        }

        private double _panLimitStart;
        /// <summary>
        /// Degree position where a pan limitation starts
        /// </summary>
        [XmlAttribute]
        public double PanLimitStart
        {
            get
            {
                return _panLimitStart;
            }
            set
            {
                _panLimitStart = value;
            }
        }

        private double _panLimitAngle;
        /// <summary>
        /// Angle of movement that the camera is constrained to, extending clockwise from PanLimitStart
        /// </summary>
        [XmlAttribute]
        public double PanLimitAngle
        {
            get
            {
                return _panLimitAngle;
            }
            set
            {
                _panLimitAngle = value;
            }
        }

        private double _panOffset;
        /// <summary>
        /// The offset (in degrees) to apply to the camera's pan position.
        /// </summary>
        /// <remarks>
        /// Used to align Pan angles to the front of a vehicle, or other natural reference point
        /// </remarks>
        [XmlAttribute]
        public double PanOffset
        {
            get
            {
                return _panOffset;
            }
            set
            {
                _panOffset = value;
            }
        }

        private double _tiltMaxAngle;
        /// <summary>
        /// The upper limit of tilt capability -90 to +90
        /// </summary>
        [XmlAttribute]
        public double TiltMaxAngle
        {
            get
            {
                return _tiltMaxAngle;
            }
            set
            {
                _tiltMaxAngle = value;
            }
        }

        private double _tiltMinAngle;
        /// <summary>
        /// The lower limit of tilt capability -90 to +90.
        /// For cameras that require relative control, this should be negative
        /// </summary>
        [XmlAttribute]
        public double TiltMinAngle
        {
            get
            {
                return _tiltMinAngle;
            }
            set
            {
                _tiltMinAngle = value;
            }
        }

        private double _zoomMaxLevel;
        /// <summary>
        /// The highest zoom level magnification.
        /// </summary>
        [XmlAttribute]
        public double ZoomMaxLevel
        {
            get
            {
                return _zoomMaxLevel;
            }
            set
            {
                _zoomMaxLevel = value;
            }
        }

        private double _zoomMinLevel;
        /// <summary>
        /// The lowest zoom level magnification. Usually 1.
        /// For cameras that require relative control, this should be negative.
        /// </summary>
        [XmlAttribute]
        public double ZoomMinLevel
        {
            get
            {
                return _zoomMinLevel;
            }
            set
            {
                _zoomMinLevel = value;
            }
        }

        private double _fieldOfView;
        /// <summary>
        /// Specifies the Horizontal Field of View for this camera. Specify "0" for unknown.
        /// </summary>
        [XmlAttribute]
        public double FieldOfView
        {
            get
            {
                return _fieldOfView;
            }
            set
            {
                _fieldOfView = value;
            }
        }

        public static CameraCapabilitiesAndLimits CreateFromXml(XElement xml)
        {
            CameraCapabilitiesAndLimits result = new CameraCapabilitiesAndLimits();
            result.HasPan = Boolean.Parse(xml.Attribute("HasPan").Value);
            result.HasTilt = Boolean.Parse(xml.Attribute("HasTilt").Value);
            result.HasZoom = Boolean.Parse(xml.Attribute("HasZoom").Value);
            result.HasDigitalZoom = Boolean.Parse(xml.Attribute("HasDigitalZoom").Value);
            result.HasEmitter = Boolean.Parse(xml.Attribute("HasEmitter").Value);
            result.HasStabilizer = Boolean.Parse(xml.Attribute("HasStabilizer").Value);
            result.HasInfrared = Boolean.Parse(xml.Attribute("HasInfrared").Value);
            result.HasInverter = Boolean.Parse(xml.Attribute("HasInverter").Value);
            result.HasWiper = Boolean.Parse(xml.Attribute("HasWiper").Value);
            result.HasFocus = Boolean.Parse(xml.Attribute("HasFocus").Value);
            result.PanLimitStart = Double.Parse(xml.Attribute("PanLimitStart").Value);
            result.PanLimitAngle = Double.Parse(xml.Attribute("PanLimitAngle").Value);
            result.PanOffset = Double.Parse(xml.Attribute("PanOffset").Value);
            result.TiltMaxAngle = Double.Parse(xml.Attribute("TiltMaxAngle").Value);
            result.TiltMinAngle = Double.Parse(xml.Attribute("TiltMinAngle").Value);
            result.ZoomMaxLevel = Double.Parse(xml.Attribute("ZoomMaxLevel").Value);
            result.ZoomMinLevel = Double.Parse(xml.Attribute("ZoomMinLevel").Value);
            result.FieldOfView = Double.Parse(xml.Attribute("FieldOfView").Value);
            return result;
        }
    }
}
