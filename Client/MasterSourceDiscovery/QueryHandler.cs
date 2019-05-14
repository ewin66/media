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

using FutureConcepts.Tools;
using FutureConcepts.Media.SourceDiscoveryCommon;

namespace FutureConcepts.Media.Client.MasterSourceDiscovery
{
    /// <summary>
    /// The NetControl class provdes static methods for obtaining data from the local NetManager as
    /// well as sending commands to it. Overloaded methods that accept an IP address are provided for testing
    /// </summary>
    public class QueryHandler
    {
        public const int WEB_REQUEST_TIMEOUT = 200000;

        public QueryHandler()
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
        public static List<SourceDiscoveryGroup> Query(String url, String username, String password)
        {
            List<SourceDiscoveryGroup> result = new List<SourceDiscoveryGroup>();
            XDocument doc = XDocument.Load(url);
            IEnumerable<XElement> groups = doc.Element("SourceGroups").Elements("SourceGroup");
            foreach (XElement group in groups)
            {
                SourceDiscoveryGroup sourceGroup = new SourceDiscoveryGroup();
                sourceGroup.Version = Int32.Parse(group.Attribute("Version").Value);
                sourceGroup.Name = group.Attribute("Name").Value;
                IEnumerable<XElement> sources = group.Elements("StreamSourceInfo");
                foreach (XElement xmlSource in sources)
                {
                    try
                    {
                        StreamSourceInfo source = new StreamSourceInfo();
                        source.SourceName = xmlSource.Attribute("SourceName").Value;
                        source.Description = xmlSource.Attribute("Description").Value;
                        if (xmlSource.Attribute("ClientURL") != null)
                        {
                            source.SinkAddress = xmlSource.Attribute("ClientURL").Value;
                        }
                        else if (xmlSource.Attribute("SinkAddress") != null)
                        {
                            source.SinkAddress = xmlSource.Attribute("SinkAddress").Value;
                        }
                        if (xmlSource.Attribute("HasAudio") != null)
                        {
                            if (xmlSource.Attribute("HasAudio").Value == "1")
                            {
                                source.HasAudio = true;
                            }
                        }
                        XElement xmlCameraControl = xmlSource.Element("CameraControl");
                        if (xmlCameraControl != null)
                        {
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
                    catch (Exception e)
                    {
                        ErrorLogger.DumpToDebug(e);
                    }
                }
                result.Add(sourceGroup);
            }
            return result;
        }
    }
}