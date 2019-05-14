using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Xml;
//using FutureConcepts.SystemTools.Settings.AntaresX.AntaresXSettings;

using FutureConcepts.Media.SourceDiscoveryCommon;
using FutureConcepts.Media;

namespace FutureConcepts.Media.Client.WowzaSourceDiscovery
{
    /// <summary>
    /// The NetControl class provdes static methods for obtaining data from the local NetManager as
    /// well as sending commands to it. Overloaded methods that accept an IP address are provided for testing
    /// </summary>
    public class Wowza
    {
        public const int WEB_REQUEST_TIMEOUT = 200000;

        public Wowza()
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
        public static SourceDiscoveryGroup QueryWowza(String hostAddress, String username, String password)
        {
            SourceDiscoveryGroup result = new SourceDiscoveryGroup();
            string page = "";
            String url = String.Format(@"http://{0}/serverinfo", hostAddress);
            try
            {
                page = GetResponseString(url, username, password);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                return result;
            }
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(page);
            XmlNodeList appNodes = xmldoc.SelectNodes("/WowzaMediaServerPro/VHost/Application");
            foreach (XmlNode appNode in appNodes)
            {
                XmlNode appNameNode = appNode.SelectSingleNode("Name");
                if (appNameNode.InnerText == "live" || appNameNode.InnerText == "axbro")
                {
                    XmlNode appInstanceNode = appNode.SelectSingleNode("ApplicationInstance");
                    if (appInstanceNode != null)
                    {
                        XmlNodeList rtpSessions = appInstanceNode.SelectNodes("RTPSession");
                        foreach (XmlNode rtpSession in rtpSessions)
                        {
                            XmlNode uriNode = rtpSession.SelectSingleNode("URI");
                            String uri = uriNode.InnerText;
                            StreamSourceInfo source = new StreamSourceInfo();
//                            source.ClientURL = uri;
                            source.Description = uri;
                            result.Sources.Add(source);
                        }
                    }
                }
            }
            return result;
        }

        private static string GetResponseString(String url, String username, String password)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Credentials = new NetworkCredential(username, password);
            request.Timeout = WEB_REQUEST_TIMEOUT;
            request.ReadWriteTimeout = WEB_REQUEST_TIMEOUT;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            return sr.ReadToEnd();
        }
    }
}