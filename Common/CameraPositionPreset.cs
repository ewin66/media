using System;
using System.Collections.Generic;
using System.Text;

namespace FutureConcepts.Media
{
    /// <summary>
    /// A camera positon that is stored by absolute positioning
    /// </summary>
    [Serializable]
    public class CameraPositionPreset : UserPresetItem
    {
        private static readonly string _panFormatString = "0.#°";
        private static readonly string _tiltFormatString = "0.#°";
        private static readonly string _zoomFormatString = "0.#x";

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public CameraPositionPreset()
        {
        }

        /// <summary>
        /// Creates a new instance refering to a specific set of locations
        /// </summary>
        /// <remarks>
        /// The <see cref="M:Name"/> property is auto generated based on the pan and tilt levels
        /// </remarks>
        /// <param name="panAngle">Pan angle, degrees in range [0,360)</param>
        /// <param name="tiltAngle">Tilt angle, degrees in range [-90,90]</param>
        /// <param name="zoomPosition">Zoom level, magnification [1,Infinity)</param>
        public CameraPositionPreset(double panAngle, double tiltAngle, double zoomPosition)
        {
            _panAngle = panAngle;
            _tiltAngle = tiltAngle;
            _zoomPosition = zoomPosition;
            Name = this.ToString("%p / %t");
        }

        /// <summary>
        /// Creates a named instance associated with a specific location
        /// </summary>
        /// <param name="name">user-friendly name</param>
        /// <param name="panAngle">Pan angle, degrees in range [0,360)</param>
        /// <param name="tiltAngle">Tilt angle, degrees in range [-90,90]</param>
        /// <param name="zoomPosition">Zoom level, magnification [1,Infinity)</param>
        public CameraPositionPreset(string name, double panAngle, double tiltAngle, double zoomPosition)
        {
            Name = name;
            _panAngle = panAngle;
            _tiltAngle = tiltAngle;
            _zoomPosition = zoomPosition;
        }

        private double _panAngle;
        /// <summary>
        /// Pan angle (degrees)
        /// </summary>
        public double PanAngle
        {
            get
            {
                return _panAngle;
            }
            set
            {
                _panAngle = value;
            }
        }

        private double _tiltAngle;
        /// <summary>
        /// Tilt angle (degrees)
        /// </summary>
        public double TiltAngle
        {
            get
            {
                return _tiltAngle;
            }
            set
            {
                _tiltAngle = value;
            }
        }

        private double _zoomPosition;
        /// <summary>
        /// Zoom position (magnification)
        /// </summary>
        public double ZoomPosition
        {
            get
            {
                return _zoomPosition;
            }
            set
            {
                _zoomPosition = value;
            }
        }

        /// <summary>
        /// Generates a string representation.
        /// </summary>
        /// <returns>a string in a default format</returns>
        public override string ToString()
        {
            return this.ToString("P: %p T: %t Z: %z" + Environment.NewLine + "%n");
        }

        /// <summary>
        /// Converts to a string using the given format
        /// </summary>
        /// <param name="format">
        /// use these identifiers:
        /// %n = name
        /// %p = pan
        /// %t = tilt
        /// %z = zoom
        /// </param>
        /// <returns>a string in the desired format</returns>
        public string ToString(string format)
        {
            if (format == null)
            {
                return Name;
            }
            else
            {
                string output = format.Replace("%p", PanAngle.ToString(_panFormatString));
                output = output.Replace("%t", TiltAngle.ToString(_tiltFormatString));
                output = output.Replace("%z", ZoomPosition.ToString(_zoomFormatString));
                output = output.Replace("%n", Name);
                return output;
            }
        }
    }
}
