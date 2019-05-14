using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;

//using FutureConcepts.SystemTools.Settings.AntaresX.AntaresXSettings;

using FutureConcepts.Media;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.SourceDiscoveryCommon;

namespace FutureConcepts.Media.Client.LocalSourceDiscovery
{
    /// <summary>
    /// </summary>
    public class ConfigFileReader
    {
        public ConfigFileReader()
        {
            // Add listener to send all of the debug output to the console
            Debug.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
        }

        /// <summary>
        /// Connects to given address and parses xml to extract all IP addresses
        /// of all nodes advertising given service type
        /// </summary>
        /// <param name="type">Type of service to retrieve.</param>
        /// <param name="address">IP address to get xml from - Note that the file /xml/services is added onto this IP.</param>
        /// <param name="exc">If an exception was thrown while retrieving services, it is returned here.</param>
        /// <returns>list of Advertised Service Objects</returns>
        public static List<SourceDiscoveryGroup> ReadFile(String path)
        {
            List<SourceDiscoveryGroup> result = new List<SourceDiscoveryGroup>();
            XDocument doc = XDocument.Load(path);
            IEnumerable<XElement> groups = doc.Element("SourceGroups").Elements("SourceGroup");
            foreach (XElement group in groups)
            {
                SourceDiscoveryGroup sourceGroup = new SourceDiscoveryGroup();
                ServerInfo serverInfo = new ServerInfo();
                serverInfo.ServerName = "Local";
                serverInfo.ServerAddress = "127.0.0.1";
                sourceGroup.ServerInfo = serverInfo;
                sourceGroup.Version = Int32.Parse(group.Attribute("Version").Value);
                sourceGroup.Name = group.Attribute("Name").Value;
                IEnumerable<XElement> sources = group.Elements("StreamSourceInfo");
                foreach (XElement xmlSource in sources)
                {
                    StreamSourceInfo source = new StreamSourceInfo();
                    source.SourceName = xmlSource.Attribute("Name").Value;
                    XAttribute sourceTypeAttribute = xmlSource.Attribute("SourceType");
                    if (sourceTypeAttribute != null)
                    {
                        source.SourceType = (SourceType)Enum.Parse(typeof(SourceType), sourceTypeAttribute.Value);
                    }
                    else
                    {
                        source.SourceType = SourceType.RTSP;
                    }
                    source.Description = xmlSource.Attribute("Description").Value;
                    if (xmlSource.Attribute("SinkAddress") != null)
                    {
                        source.SinkAddress = xmlSource.Attribute("SinkAddress").Value;
                    }
                    else if (xmlSource.Attribute("ClientURL") != null)
                    {
                        source.SinkAddress = xmlSource.Attribute("ClientURL").Value;
                    }
                    if (source.SinkAddress.StartsWith(@"rtsp://"))
                    {
                        if ((xmlSource.Attribute("SourceType") != null) && xmlSource.Attribute("SourceType").Value == "RTSP_Elecard")
                        {
                            source.SourceType = SourceType.RTSP_Elecard;
                        }
                        else
                        {
                            source.SourceType = SourceType.RTSP;
                        }
                    }
                    if (xmlSource.Element("CameraControl") != null)
                    {
                        XElement xmlCameraControl = xmlSource.Element("CameraControl");
                        CameraControlInfo info = new CameraControlInfo();
                        info.PTZType = (PTZType)Enum.Parse(typeof(PTZType), xmlCameraControl.Attribute("PTZType").Value);
                        XElement xmlCaps = xmlCameraControl.Element("Capabilities");
                        if (xmlCaps != null)
                        {
                            info.Capabilities = CameraCapabilitiesAndLimits.CreateFromXml(xmlCaps);
                        }
                        XElement xmlAddress = xmlCameraControl.Element("Address");
                        if (xmlAddress != null)
                        {
                            info.Address = xmlAddress.Value;
                        }
                        source.CameraControl = info;
                    }
                    sourceGroup.Sources.Add(source);
                }
                result.Add(sourceGroup);
            }
            return result;
        }
    }
}