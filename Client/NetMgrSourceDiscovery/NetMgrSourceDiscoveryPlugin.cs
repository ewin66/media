using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using FutureConcepts.Media;
using FutureConcepts.Media.SourceDiscoveryCommon;
using FutureConcepts.Settings;

namespace FutureConcepts.Media.Client.NetMgrSourceDiscovery
{
    /// <summary>
    /// This class uses the legacy NetManager OLSRD video server sources discovery stuff
    /// </summary>
    public class NetMgrSourceDiscoveryPlugin : SourceDiscoveryPlugin
    {
        private QueryMediaServers _queryMediaServers;
        private Dictionary<String, SourceDiscoveryGroup> _groups;

       /// <summary>
        /// Creates a new instance
        /// </summary>
        public NetMgrSourceDiscoveryPlugin(SourceDiscoveryDefinition sourceDiscoveryDefinition)
        {
            _groups = new Dictionary<String, SourceDiscoveryGroup>();
            _queryMediaServers = new QueryMediaServers(sourceDiscoveryDefinition);
            _queryMediaServers.AcceptedSourceTypes.Add(SourceType.BouncingBall);
            _queryMediaServers.AcceptedSourceTypes.Add(SourceType.FVSC);
            _queryMediaServers.AcceptedSourceTypes.Add(SourceType.MX);
            _queryMediaServers.AcceptedSourceTypes.Add(SourceType.SRM314);
            _queryMediaServers.AcceptedSourceTypes.Add(SourceType.RTSP);

            //specify required info
            _queryMediaServers.RequestedServerInfoParams = _queryMediaServers.RequestedServerInfoParams = Contract.RequestedServerInfoParams.OriginServers |
                                                            Contract.RequestedServerInfoParams.ServerName |
                                                            Contract.RequestedServerInfoParams.ServerAddress |
                                                            Contract.RequestedServerInfoParams.StreamSources;

            _queryMediaServers.RequestedStreamSourceInfoParams = Contract.RequestedStreamSourceInfoParams.Description |
                                                                  Contract.RequestedStreamSourceInfoParams.SourceName |
                                                                  Contract.RequestedStreamSourceInfoParams.SourceType |
                                                                  Contract.RequestedStreamSourceInfoParams.CameraControl |
                                                                  Contract.RequestedStreamSourceInfoParams.URLs |
                                                                  Contract.RequestedStreamSourceInfoParams.MicrowaveControl |
                                                                  Contract.RequestedStreamSourceInfoParams.Hidden;
            //bind event handlers
            _queryMediaServers.ServerInfoChanged += new EventHandler<QueryMediaServers.ServerEventArgs>(queryDaemon_ServerInfoChanged);
            _queryMediaServers.ServerNowAvailable += new EventHandler<QueryMediaServers.ServerEventArgs>(queryDaemon_ServerNowAvailable);
            _queryMediaServers.ServerNowUnavailable += new EventHandler<QueryMediaServers.ServerEventArgs>(queryDaemon_ServerNowUnavailable);
        }

        /// <summary>
        /// Begins polling for Media Servers
        /// </summary>
        public override void Start()
        {
            _queryMediaServers.Start();
        }

        /// <summary>
        /// Stops polling for Media Servers
        /// </summary>
        public override void Stop()
        {
            _queryMediaServers.Stop();
            Debug.WriteLine("NetMgrSourceDiscoveryPlugin Stop");
        }

        private void queryDaemon_ServerNowAvailable(object sender, QueryMediaServers.ServerEventArgs e)
        {
            SourceDiscoveryGroup group = new SourceDiscoveryGroup();
            ServerInfo serverInfo = _queryMediaServers.MediaServers[e.ServerAddress];
            group.ServerInfo = serverInfo;
            group.Name = serverInfo.ServerName;
            foreach (StreamSourceInfo streamSourceInfo in serverInfo.StreamSources.Items)
            {
                group.Sources.Add(streamSourceInfo);
            }
            _groups.Add(e.ServerAddress, group);
            FireGroupOnline(group);
        }

        private void queryDaemon_ServerNowUnavailable(object sender, QueryMediaServers.ServerEventArgs e)
        {
            if (_groups.ContainsKey(e.ServerAddress))
            {
                SourceDiscoveryGroup group = _groups[e.ServerAddress];
                FireGroupOffline(group);
                _groups.Remove(e.ServerAddress);
            }
        }

        /// <summary>
        /// Updates a server in the TreeView if its ServerInfo has changed
        /// </summary>
        void queryDaemon_ServerInfoChanged(object sender, QueryMediaServers.ServerEventArgs e)
        {
            if (_groups.ContainsKey(e.ServerAddress))
            {
                SourceDiscoveryGroup group = _groups[e.ServerAddress];
                ServerInfo serverInfo = _queryMediaServers.MediaServers[e.ServerAddress];
                group.Name = serverInfo.ServerName;
                group.Sources.Clear();
                foreach (StreamSourceInfo streamSourceInfo in serverInfo.StreamSources.Items)
                {
                    group.Sources.Add(streamSourceInfo);
                }
                FireGroupChanged(group);
            }
        }
    }
}
