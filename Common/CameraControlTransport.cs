using System;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Defines the a type of camera (see <see cref="T:PTZType"/>) and how to connect to it for control purposes.
    /// </summary>
    [Serializable()]
    public class CameraControlTransport
    {
        private CameraControlTransportType _type;

        /// <summary>
        /// Indicates the type of plug in to use when controlling this camera.
        /// </summary>
        [XmlElement]
        public CameraControlTransportType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
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

        public static CameraControlTransport CreateFromXml(XElement xml)
        {
            CameraControlTransport result = new CameraControlTransport();
            result.Type = (CameraControlTransportType)Enum.Parse(typeof(CameraControlTransportType), xml.Attribute("Type").Value);
            result.Address = xml.Attribute("Address").Value;
            return result;
        }
    }
}
