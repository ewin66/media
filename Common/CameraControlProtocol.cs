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
    public class CameraControlProtocol
    {
        private CameraControlProtocolType _type;
        /// <summary>
        /// Indicates the type of plug in to use when controlling this camera.
        /// </summary>
        [XmlAttribute]
        public CameraControlProtocolType Type
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

        private CameraControlTransport _transport;

        [XmlElement]
        public CameraControlTransport Transport
        {
            get
            {
                return _transport;
            }
            set
            {
                _transport = value;
            }
        }

        public static CameraControlProtocol CreateFromXml(XElement xml)
        {
            CameraControlProtocol result = new CameraControlProtocol();
            result.Type = (CameraControlProtocolType)Enum.Parse(typeof(CameraControlProtocolType), xml.Attribute("Type").Value);
            XElement xmlTransport = xml.Element("Transport");
            if (xmlTransport != null)
            {
                result.Transport = CameraControlTransport.CreateFromXml(xmlTransport);
            }
            return result;
        }
    }
}
