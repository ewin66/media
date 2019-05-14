using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using FutureConcepts.SystemTools.Networks.AntaresXNetworkServices;
using FutureConcepts.Settings;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace FutureConcepts.Media.Client.NetMgrSourceDiscovery
{
    /// <summary>
    /// This class works to poll Media Servers via AX services
    /// </summary>
    public class QueryMediaServers
    {
        /// <summary>
        /// Used to indicate how QMS knows about a server
        /// </summary>
        public enum DiscoveryMechanism
        {
            /// <summary>
            /// The server is unknown to this <see cref="T:QueryMediaServers"/> instance.
            /// </summary>
            NotDiscovered,
            /// <summary>
            /// The server has been discovered via AXServices
            /// </summary>
            AXServices,
            /// <summary>
            /// The server has beem manually added
            /// </summary>
            Manual,
            /// <summary>
            /// The server has been both manually added and auto-discovered via AXServices
            /// </summary>
            Both
        };

        private readonly object _mediaServerListLock = new object();

        /// <summary>
        /// This string is suffixed to primary keys to indicate they were manually added.
        /// </summary>
        private const string ManualDiscoverySuffix = "_manual";

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public QueryMediaServers(SourceDiscoveryDefinition definition)
        {
            AXServicesPollInterval = definition.PollInterval;
            if (AXServicesPollInterval <= 0)
            {
                AXServicesPollInterval = 15000;
            }
            AXServicesRefreshInterval = definition.RefreshInterval;
            if (AXServicesRefreshInterval <= 0)
            {
                AXServicesRefreshInterval = 15000;
            }
            AcceptAllSourceTypes = false;
            AcceptHiddenSources = false;
            RemoveSelfFromOriginServers = true;
        }

        #region Properties

        /// <summary>
        /// Fetches the "Local" computer's Friendly Name
        /// </summary>
        [XmlIgnore]
        public static string LocalFriendlyName
        {
            get
            {
                List<AdvertisedServiceInfo> services = NetControl.GetLocalAdvertisedServices(-1, AXServicesAddress);
                if (services.Count > 0)
                {
                    if (!string.IsNullOrEmpty(services[0].Origin.FriendlyName))
                    {
                        return services[0].Origin.FriendlyName;
                    }
                }
                
                return Environment.MachineName;
            }
        }

        private static List<string> _localServerAddresses = null;
        private static object _localServerAddressLock = new object();
        /// <summary>
        /// Fetches the list of all IPs that are locally advertised
        /// </summary>
        [XmlIgnore]
        public static List<string> LocalServerAddresses
        {
            get
            {
                lock (_localServerAddressLock)
                {
                    if (_localServerAddresses == null)
                    {
                        _localServerAddresses = new List<string>();
                        try
                        {
                            List<AdvertisedServiceInfo> services = NetControl.GetLocalAdvertisedServices(-1, AXServicesAddress);
                            foreach (AdvertisedServiceInfo i in services)
                            {
                                _localServerAddresses.Add(i.IP.ToString());
                            }
                        }
                        catch
                        {
                            _localServerAddresses = null;
                        }
                    }
                    return _localServerAddresses;
                }
            }
        }

        private Dictionary<string, ServerInfo> _mediaServers = new Dictionary<string, ServerInfo>();
        /// <summary>
        /// The currently known MediaServers, keyed by their address
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, ServerInfo> MediaServers
        {
            get
            {
                lock (_mediaServerListLock)
                {
                    return _mediaServers;
                }
            }
            private set
            {
                _mediaServers = value;
            }
        }

        /// <summary>
        /// The address at which the AX Services server resides.
        /// Set to null to reset to NetControl default.
        /// </summary>
        [XmlIgnore]
        public static string AXServicesAddress
        {
            get
            {
                return NetControl.NETMANAGER_ADDRESS;
            }
        } 

        private int _pollAXServiceInterval = 0;
        /// <summary>
        /// The interval (in ms) to poll the AX Services
        /// </summary>
        [XmlElement]
        public int AXServicesPollInterval
        {
            get
            {
                return _pollAXServiceInterval;
            }
            set
            {
                _pollAXServiceInterval = value;
            }
        }

        private int _refreshAXServiceInterval = 0;
        /// <summary>
        /// The interval (in ms) to check if a refresh is needed on already known servers (and refresh them if neccesary)
        /// </summary>
        [XmlElement]
        public int AXServicesRefreshInterval
        {
            get
            {
                return _refreshAXServiceInterval;
            }
            set
            {
                _refreshAXServiceInterval = value;
            }
        }

        private bool _populateNormal = true;
        /// <summary>
        /// Set to true to populate normal media servers (default true)
        /// </summary>
        [XmlElement]
        public bool PopulateRegularServers
        {
            get
            {
                return _populateNormal;
            }
            set
            {
                _populateNormal = value;
            }
        }

        private bool _populateRestreamer = true;
        /// <summary>
        /// Set to true to populate restreamers (default true)
        /// </summary>
        [XmlElement]
        public bool PopulateRestreamers
        {
            get
            {
                return _populateRestreamer;
            }
            set
            {
                _populateRestreamer = value;
            }
        }

        private bool _useGetServerInfoSpecific = true;
        /// <summary>
        /// Set to true to use GetServerInfoSpecific (default true)
        /// </summary>
        [XmlElement]
        public bool UseGetServerInfoSpecific
        {
            get
            {
                return _useGetServerInfoSpecific;
            }
            set
            {
                _useGetServerInfoSpecific = value;
            }
        }

        private int _requestedServerInfoParams = Contract.RequestedServerInfoParams.All;
        /// <summary>
        /// ServerInfo parameters to get
        /// </summary>
        [XmlElement]
        public int RequestedServerInfoParams
        {
            get
            {
                return _requestedServerInfoParams;
            }
            set
            {
                //we always want to fetch revision number -- NO MATTER WHAT!
                _requestedServerInfoParams = value | Contract.RequestedServerInfoParams.RevisionNumber;
            }
        }

        private int _requestedStreamSourceInfoParams = Contract.RequestedStreamSourceInfoParams.All;
        /// <summary>
        /// StreamSourceInfo params to get
        /// </summary>
        [XmlElement]
        public int RequestedStreamSourceInfoParams
        {
            get
            {
                return _requestedStreamSourceInfoParams;
            }
            set
            {
                _requestedStreamSourceInfoParams = value;
            }
        }

        /// <summary>
        /// Set to true to accept all source types. Set to false to use AcceptedSourceTypes filter.
        /// Default false.
        /// </summary>
        [XmlElement]
        public bool AcceptAllSourceTypes { get; set; }

        private List<SourceType> _sourceTypeFilter = new List<SourceType>();
        /// <summary>
        /// Allowable SourceType to populate per server
        /// </summary>
        [XmlElement]
        public List<SourceType> AcceptedSourceTypes
        {
            get
            {
                return _sourceTypeFilter;
            }
            set
            {
                _sourceTypeFilter = value;
            }
        }

        /// <summary>
        /// Set to true to include sources that are marked as hidden. Default is false.
        /// </summary>
        [XmlElement]
        public bool AcceptHiddenSources { get; set; }

        /// <summary>
        /// Set to false to show Local advertisements in OriginServers lists.
        /// Default is true, which removes Local advertisements from any received OriginServers.
        /// </summary>
        [XmlElement]
        public bool RemoveSelfFromOriginServers { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Fired when a new server comes online
        /// </summary>
        public event EventHandler<ServerEventArgs> ServerNowAvailable;
        /// <summary>
        /// Fired when a known server goes offline
        /// </summary>
        public event EventHandler<ServerEventArgs> ServerNowUnavailable;
        /// <summary>
        /// Fired when a currently known server's information has changed
        /// </summary>
        public event EventHandler<ServerEventArgs> ServerInfoChanged;

        /// <summary>
        /// Fires the ServerNowUnavailable event
        /// </summary>
        /// <param name="serverAddress">address of server that has become unavailable</param>
        private void FireServerNowUnavailable(string serverAddress)
        {
            if (ServerNowUnavailable != null)
            {
                ServerNowUnavailable.Invoke(this, new ServerEventArgs(serverAddress));
            }
        }

        /// <summary>
        /// Fires the ServerNowAvailable event
        /// </summary>
        /// <param name="serverAddress">address of server that is now available</param>
        private void FireServerNowAvailable(string serverAddress)
        {
            if (ServerNowAvailable != null)
            {
                ServerNowAvailable.Invoke(this, new ServerEventArgs(serverAddress));
            }
        }

        /// <summary>
        /// Fires the ServerInfoChanged event
        /// </summary>
        /// <param name="serverAddress"></param>
        private void FireServerInfoChanged(string serverAddress)
        {
            Debug.WriteLine("QueryMediaServers: ServerInfoChanged: " + serverAddress);
            if (ServerInfoChanged != null)
            {
                ServerInfoChanged.Invoke(this, new ServerEventArgs(serverAddress));
            }
        }

        /// <summary>
        /// Contains information regarding an event involving a server.
        /// </summary>
        public class ServerEventArgs : EventArgs
        {
            /// <summary>
            /// Creates a new instance, referring to a server by address
            /// </summary>
            /// <param name="address">server that is being referred to.</param>
            public ServerEventArgs(string address)
            {
                this.ServerAddress = address;
            }

            /// <summary>
            /// Gets or sets the server address associated with this event
            /// </summary>
            public string ServerAddress { get; set; }
        }

        #endregion

        #region Start/Stop

        /// <summary>
        /// Begins polling for Media Servers
        /// </summary>
        public void Start()
        {
            AXPollingThread.Start();

            AXRefreshThread.Start();
        }

        /// <summary>
        /// Stops polling for Media Servers
        /// </summary>
        public void Stop()
        {
            AXPollingThread.Abort();
            AXPollingThread = null;

            AXRefreshThread.Abort();
            AXRefreshThread = null;
        }

        #endregion

        #region AX Polling Thread

        private Thread _thread;
        /// <summary>
        /// The worker thread that does the AX service polling
        /// </summary>
        private Thread AXPollingThread
        {
            get
            {
                if(_thread == null)
                {
                    _thread = new Thread(new ThreadStart(AXPollingBackgroundThread));
                    _thread.Name = "QMS AXS Polling";
                }
                return _thread;
            }
            set
            {
                _thread = value;
            }
        }

        /// <summary>
        /// This is the method the AX Polling Thread executes
        /// </summary>
        private void AXPollingBackgroundThread()
        {
            while (true)
            {
                List<string> currentActiveServers = new List<string>();
                List<AdvertisedServiceInfo> servInfos = GetFilteredAdvertisedServices();
                if (servInfos.Count > 0)
                {
                    foreach (AdvertisedServiceInfo servInfo in servInfos)
                    {
                        string serverAddress = servInfo.IP.ToString();

                        DiscoveryMechanism m = IsServerKnown(serverAddress);
                        if (m == DiscoveryMechanism.Both)
                        {
                            RemoveManualServer(serverAddress);
                        }
                        bool exists = m != DiscoveryMechanism.NotDiscovered;

                        currentActiveServers.Add(serverAddress);

                        if ((exists) && (servInfo.Type == 3))
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(QueryServerInfoChanged), serverAddress);
                        }
                        else if (!exists)
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(QueryNewServer), servInfo);
                        }
                    }

                    List<string> offlineServers = new List<string>();
                    lock (_mediaServerListLock)
                    {
                        foreach (KeyValuePair<string, ServerInfo> kvp in MediaServers)
                        {
                            if ((!currentActiveServers.Contains(kvp.Value.ServerAddress)) &&
                                (!kvp.Key.Contains(ManualDiscoverySuffix)))
                            {
                                offlineServers.Add(kvp.Value.ServerAddress);
                            }
                        }
                        foreach (string address in offlineServers)
                        {
                            MediaServers.Remove(address);
                        }
                    }
                    foreach (string i in offlineServers)
                    {
                        FireServerNowUnavailable(i);
                    }
                }
                Thread.Sleep(this.AXServicesPollInterval);
            }
            Debug.WriteLine("AXPollingBackgroundThread END");
        }

        #endregion

        #region AX Refresh Thread

        private Thread _refreshthread;
        /// <summary>
        /// The worker therad that does the Currently-known Service Refreshing
        /// </summary>
        private Thread AXRefreshThread
        {
            get
            {
                if (_refreshthread == null)
                {
                    _refreshthread = new Thread(new ThreadStart(AXRefreshingBackgroundThread));
                    _refreshthread.Name = "QMS AXS Refresh";
                }
                return _refreshthread;
            }
            set
            {
                _refreshthread = value;
            }
        }

        /// <summary>
        /// This is the method the AX Refreshing Thread executes
        /// </summary>
        private void AXRefreshingBackgroundThread()
        {
            while (true)
            {
                Thread.Sleep(AXServicesRefreshInterval);    //sleep first, since we are started at the same time as the polling thread

                lock (_mediaServerListLock)
                {
                    foreach (KeyValuePair<string, ServerInfo> kvp in MediaServers)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(QueryServerInfoChanged), kvp.Key);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Manually adds a server to the MediaServers list.
        /// It is not tracked by QMS, so you will not receive events pertaining to it,
        /// except for ServerNowAvailable when it is added.
        /// </summary>
        /// <param name="serverAddress">server address to add</param>
        public void AddServerManually(string serverAddress)
        {
            if (IsServerKnown(serverAddress) == DiscoveryMechanism.AXServices)
            {
                throw new Exception("Server " + serverAddress + " has already been discovered.");
            }

            ServerInfo s = FetchServerInfo(serverAddress);
            if (s == null)
            {
                throw new Exception("Could not retreive server info!");
            }

            serverAddress += ManualDiscoverySuffix;

            lock (_mediaServerListLock)
            {
                this.MediaServers.Add(serverAddress, s);
            }

            FireServerNowAvailable(serverAddress);
        }

        /// <summary>
        /// Removes a manually-added server.
        /// </summary>
        /// <param name="serverAddress">IP addres to remove</param>
        public void RemoveManualServer(string serverAddress)
        {
            string key = serverAddress + ManualDiscoverySuffix;
            if (MediaServers.ContainsKey(key))
            {
                lock (_mediaServerListLock)
                {
                    MediaServers.Remove(key);
                }
            }
        }

        /// <summary>
        /// Determines if we have previously populated the given server (by address)
        /// </summary>
        /// <param name="serverAddress">server address to check</param>
        /// <returns>Returns how the given server IP is known</returns>
        private DiscoveryMechanism IsServerKnown(string serverAddress)
        {
            bool ax = MediaServers.ContainsKey(serverAddress);
            bool manual = MediaServers.ContainsKey(serverAddress + ManualDiscoverySuffix);

            if (ax && manual)
            {
                return DiscoveryMechanism.Both;
            }
            else if (ax)
            {
                return DiscoveryMechanism.AXServices;
            }
            else if (manual)
            {
                return DiscoveryMechanism.Manual;
            }

            return DiscoveryMechanism.NotDiscovered;
        }

        /// <summary>
        /// Removes a server from knowledge, and raises the ServerNowUnavailable event
        /// </summary>
        /// <param name="serverKey">server's address/key</param>
        private void RemoveServer(string serverKey)
        {
            bool raiseEvent = false;

            lock (_mediaServerListLock)
            {
                raiseEvent = MediaServers.Remove(serverKey);
            }

            if(raiseEvent)
            {
                FireServerNowUnavailable(serverKey);
            }
        }

        #endregion

        #region Polling/Discovery Methods

        /// <summary>
        /// This method is invoked to query a new/unknown media server
        /// </summary>
        /// <param name="data">data, which must be of type AdvertisedServiceInfo</param>
        private void QueryNewServer(object data)
        {
            Thread.CurrentThread.Name = "QMS Query New Server";

            AdvertisedServiceInfo servInfo = (AdvertisedServiceInfo)data;
            string serverAddress = servInfo.IP.ToString();
            try
            {
                ServerInfo serverInfo = FetchServerInfo(serverAddress);
                if (serverInfo == null)
                {
                    return;
                }

                // The server name originating from the NetManager advertisement overrides the value
                // coming from the MediaServer (which comes from Dns.GetHostName())
                if (servInfo.Name != null)
                {
                    serverInfo.ServerName = servInfo.Name;
                }

                if ((serverInfo.StreamSources.Count > 0) ||
                    (serverInfo.OriginServers != null) &&
                    (IsServerKnown(serverAddress) != DiscoveryMechanism.AXServices))
                {
                    lock (_mediaServerListLock)
                    {
                        this.MediaServers.Add(serverAddress, serverInfo);
                    }
                    FireServerNowAvailable(serverAddress);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Failed QueryNewServer (" + serverAddress + ") - " + exc.Message);
            }
        }

        /// <summary>
        /// Retreives the ServerInfo for a given server address.
        /// </summary>
        /// <param name="serverAddress">
        /// a string containing the server address to connect to
        /// </param>
        /// <returns>If an error occurs, null is returned.</returns>
        private ServerInfo FetchServerInfo(string serverAddress)
        {
            ServerInfo resultServerInfo = new ServerInfo();
            try
            {
                using (ServerConfig client = new ServerConfig(serverAddress))
                {                  

                    ServerInfo serverInfo = null;
                    if (this.UseGetServerInfoSpecific)
                    {
                        serverInfo = client.GetServerInfoSpecific(this.RequestedServerInfoParams, this.RequestedStreamSourceInfoParams);
                    }
                    else
                    {
                        serverInfo = client.GetServerInfo();
                    }

                    resultServerInfo.ServerName = serverInfo.ServerName;
                    resultServerInfo.ServerAddress = serverInfo.ServerAddress;
                    resultServerInfo.VersionInfo = serverInfo.VersionInfo;
                    resultServerInfo.OriginServers = serverInfo.OriginServers;
                    resultServerInfo.RevisionNumber = serverInfo.RevisionNumber;

                    if (((AcceptAllSourceTypes) && (AcceptHiddenSources)) || (serverInfo.StreamSources.Count < 1))
                    {
                        resultServerInfo.StreamSources = serverInfo.StreamSources;
                    }
                    else
                    {
                        resultServerInfo.StreamSources = new StreamSources();
                        
                        foreach (StreamSourceInfo sourceInfo in serverInfo.StreamSources.Items)
                        {
                            //this performs two filtering operations
                            //     1 - exclude source if it is Hidden
                            // AND 2 - add source if it is an accepted type
                            if ((AcceptHiddenSources || !sourceInfo.Hidden) &&
                                (AcceptAllSourceTypes || AcceptedSourceTypes.Contains(sourceInfo.SourceType)))
                            {
                                resultServerInfo.StreamSources.Add(sourceInfo);
                            }
                        }
                    }

                    if (RemoveSelfFromOriginServers)
                    {
                        RemoveSelf(resultServerInfo.OriginServers);
                    }


                    return resultServerInfo;
                }

            }
            catch (Exception exc)
            {
                Debug.WriteLine("Failed FetchServerInfo (" + serverAddress + ") " + exc.Message);
                return null;
            }
        }

        /// <summary>
        /// Detects if a list of servers contains a local server address. If it does, it removes it
        /// </summary>
        /// <param name="list">list of serverinfo to inspect</param>
        public static void RemoveSelf(List<ServerInfo> list)
        {
            if (LocalServerAddresses == null)
            {
                return;
            }
            if (list != null)
            {
                List<ServerInfo> toDrop = new List<ServerInfo>();
                foreach (ServerInfo i in list)
                {
                    if (LocalServerAddresses.Contains(i.ServerAddress))
                    {
                        toDrop.Add(i);
                    }
                    RemoveSelf(i.OriginServers);
                }
                foreach (ServerInfo i in toDrop)
                {
                    list.Remove(i);
                }
            }
        }

        /// <summary>
        /// Fetches the Advertised Service Info
        /// </summary>
        /// <returns>a list of 0 or more AdvertisedServiceInfo</returns>
        private List<AdvertisedServiceInfo> GetFilteredAdvertisedServices()
        {
            try
            {
                List<AdvertisedServiceInfo> servInfos = new List<AdvertisedServiceInfo>();

                //get normal Media Servers
                if (this.PopulateRegularServers)
                {
                    servInfos.AddRange(NetControl.GetAllAdvertisedServices(2, AXServicesAddress));
                }
                //and get all Restreamer Media Servers
                if (this.PopulateRestreamers)
                {
                    servInfos.AddRange(NetControl.GetAllAdvertisedServices(3, AXServicesAddress));
                }

                return servInfos;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed GetFilteredAdvertisedServices: " + ex.Message);
                return new List<AdvertisedServiceInfo>();
            }
        }

        #endregion

        #region Refresh Methods

        private void QueryServerInfoChanged(object serverKeyObj)
        {
            Thread.CurrentThread.Name = "QMS Query Server Info Changed";
            string serverKey = serverKeyObj as string;
            if (serverKey == null)
            {
                return;
            }

            try
            {
                ServerInfo curInfo = MediaServers[serverKey];

                using (ServerConfig client = new ServerConfig(curInfo.ServerAddress))
                {
                    ServerInfo latest = client.GetServerInfoSpecific(Contract.RequestedServerInfoParams.RevisionNumber, 0);
                    if (latest.RevisionNumber != curInfo.RevisionNumber)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateServerInfo), serverKey);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed QueryServerInfoChanged for " + serverKey + ". Removing...");
                Debug.WriteLine("    " + ex.Message);
                RemoveServer(serverKey);
            }
        }

        /// <summary>
        /// Updates the server info for the specified serverKey
        /// </summary>
        /// <param name="serverKeyObj">string serverKey</param>
        private void UpdateServerInfo(object serverKeyObj)
        {
            string serverKey = serverKeyObj as string;
            if (serverKey == null)
            {
                return;
            }

            if (!MediaServers.ContainsKey(serverKey))
            {
                return;
            }

            ServerInfo original = MediaServers[serverKey];
            ServerInfo updated = FetchServerInfo(original.ServerAddress);
            if (updated != null)
            {
                if (MediaServers.ContainsKey(serverKey))
                {
                    //HACK this preserves the AdvertisedService name
                    updated.ServerName = original.ServerName;

                    MediaServers[serverKey] = updated;
                    FireServerInfoChanged(serverKey);
                }
            }
            else
            {
                Debug.WriteLine("Failed to UpdateServerInfo for " + serverKey + ". Removing...");
                RemoveServer(serverKey);
            }
        }

        #endregion
    }
}
