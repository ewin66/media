using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;

using FutureConcepts.Media;
using FutureConcepts.Media.Client;
using FutureConcepts.SystemTools.Networks.AntaresXNetworkServices;
using System.Net.NetworkInformation;

namespace FutureConcepts.Media.SVD.QueryVideoServices
{
    /// <summary>
    /// Awesome methods for querying video services
    /// </summary>
    public static class Engine
    {
        #region Public API

        static Engine()
        {
            NetmanagerAddress = NetControl.NETMANAGER_ADDRESS;
        }

        public static string NetmanagerAddress { get; set; }

        /// <summary>
        /// Fetches the list of all advertised services (video servers and restreamers)
        /// </summary>
        /// <returns>a list of zero or more items</returns>
        public static List<AdvertisedServiceInfo> GetAllAdvertisedServices()
        {
            List<AdvertisedServiceInfo> servers = NetControl.GetAllAdvertisedServices(2);
            servers.AddRange(NetControl.GetAllAdvertisedServices(3));
            return servers;
        }

        /// <summary>
        /// Prints out the info for all advertised services
        /// </summary>
        public static void PrintAdvertisedServices()
        {
            List<AdvertisedServiceInfo> servers = GetAllAdvertisedServices();

            Console.WriteLine("Advertised Video Services:");
            Console.WriteLine();

            foreach (AdvertisedServiceInfo i in servers)
            {
                Console.WriteLine(String.Format("> {0} {1}", i.Name, i.IP.ToString(), i.Type));
                PrintIndent(3);
                Console.WriteLine(String.Format("type={0} local={1} origin={2}/{3}", i.Type, i.Local, i.Origin.FriendlyName, i.Origin.IP));
            }

            Console.WriteLine("--");
        }

        /// <summary>
        /// Prints out the results of querying a service
        /// </summary>
        /// <param name="info">AdvertisedServiceInfo used to reach the server in question</param>
        public static void PrintQueryService(AdvertisedServiceInfo info)
        {
            Console.WriteLine(String.Format(" > Trying {0} {1}...", info.Name, info.IP.ToString()));

            DoGetService(info.IP.ToString());
        }

        /// <summary>
        /// Prints out the results of querying a service
        /// </summary>
        /// <param name="hostOrIP">hostname or IP address to query</param>
        public static void PrintQueryService(string hostOrIP)
        {
            Console.WriteLine(String.Format(" > Trying {0}...", hostOrIP));

            DoGetService(hostOrIP);
        }

        /// <summary>
        /// Prints out the info for a ServerInfo struct
        /// </summary>
        /// <remarks>overloaded -- forwards to private method</remarks>
        /// <param name="serverInfo">struct to print</param>
        public static void PrintServerInfo(ServerInfo serverInfo)
        {
            PrintServerInfo(serverInfo, 0);
        }

        #endregion

        #region Console Out backend

        /// <summary>
        /// Attempts to retreive the ServerInfo object
        /// </summary>
        /// <param name="hostOrIP">hostname or IP to query</param>
        /// <returns>the object on success, or null if failure</returns>
        private static ServerInfo GetServerInfo(string hostOrIP)
        {
            try
            {
                using (ServerConfig client = new ServerConfig(hostOrIP))
                {
                    return client.GetServerInfo();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("error: {0} {1}", hostOrIP, exc.Message); // FIXME
                return null;
            }
        }

        /// <summary>
        /// Does the actual leg work of querying a service and printing it out
        /// </summary>
        /// <param name="hostOrIP">hostname or ip to look up</param>
        private static void DoGetService(string hostOrIP)
        {
            try
            {
                Ping p = new Ping();
                PingReply r = p.Send(hostOrIP);
                if (r.Status != IPStatus.Success)
                {
                    throw new PingException(r.Status.ToString());
                }
                Console.WriteLine(String.Format(" ... ping roundtrip: {0} ms ...", r.RoundtripTime));
            }
            catch (Exception ex)
            {
                string msg = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                Console.WriteLine(String.Format("error: could not ping {0}: {1}", hostOrIP, msg));
                return;
            }

            PrintServerInfo(GetServerInfo(hostOrIP));
        }

        /// <summary>
        /// prints out a server info
        /// </summary>
        /// <param name="serverInfo">server info struct</param>
        /// <param name="indent">indentation level (in spaces). Used for nesting purposes</param>
        private static void PrintServerInfo(ServerInfo serverInfo, int indent)
        {
            if (serverInfo == null)
            {
                return;
            }

            PrintIndent(indent);
            Console.WriteLine(String.Format("{0} {1}", serverInfo.ServerName, serverInfo.ServerAddress));
            if (!string.IsNullOrEmpty(serverInfo.VersionInfo))
            {
                Console.WriteLine(serverInfo.VersionInfo);
            }
            PrintSources(serverInfo.StreamSources, indent + 2);
            if (serverInfo.OriginServers != null)
            {
                if (serverInfo.OriginServers.Count > 0)
                {
                    Console.WriteLine();
                    PrintIndent(indent);
                    Console.WriteLine(String.Format("Restreaming {0} servers:", serverInfo.OriginServers.Count));
                }
                foreach (ServerInfo i in serverInfo.OriginServers)
                {
                    PrintServerInfo(i, indent + 3);
                }
            }
        }

        /// <summary>
        /// Prints out all sources
        /// </summary>
        /// <param name="streamSources">stream sources to print out</param>
        /// <param name="indent">indentation level (in spaces), used for nesting purposes</param>
        private static void PrintSources(StreamSources streamSources, int indent)
        {
            if (streamSources.Count < 1)
            {
                PrintIndent(indent);
                Console.WriteLine("-- no sources --");
                return;
            }

            foreach (StreamSourceInfo i in streamSources.Items)
            {
                PrintIndent(indent);
                Console.WriteLine(String.Format("{0} - {1} ({2}){3}", i.SourceName, i.Description, i.SourceType, (i.Hidden ? " [HIDDEN]" : "")));
                if (i.CameraControl != null)
                {
                    PrintIndent(indent + 2);
                    Console.WriteLine(String.Format("Camera control: {0} address={1}", i.CameraControl.PTZType, i.CameraControl.Address));
                }
                if (i.MicrowaveControl != null)
                {
                    PrintIndent(indent + 2);
                    Console.WriteLine(String.Format("Micro RX control: {0} address={1}", i.MicrowaveControl.ReceiverType, i.MicrowaveControl.Address));
                }
            }
        }

        /// <summary>
        /// Prints the indentation for a given depth
        /// </summary>
        /// <param name="depth">number of spaces to print</param>
        private static void PrintIndent(int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                Console.Write(" ");
            }
        }

        #endregion
    }
}
