using System;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.Server.Graphs;
using FutureConcepts.Tools;
using FutureConcepts.Media.Client;

namespace FutureConcepts.Media.Server.IndigoServices
{
    public class CommonStreamService
    {
        /// <summary>
        /// Use this string to locate the Restreamer source configuration
        /// </summary>
        public static readonly string RestreamerSourceName = "restreamer";

        private ClientConnectRequest _clientRequest;
        protected ClientConnectRequest ClientConnectRequest
        {
            get
            {
                return _clientRequest;
            }
            set
            {
                _clientRequest = value;
            }
        }

        private OpenGraphRequest _openGraphRequest;

        protected OpenGraphRequest OpenGraphRequest
        {
            get
            {
                if (_openGraphRequest == null)
                {
                    _openGraphRequest = new OpenGraphRequest();
                }
                return _openGraphRequest;
            }
            set
            {
                _openGraphRequest = value;
            }
        }



        private Profile _currentProfile;
        protected Profile CurrentProfile
        {
            get { return _currentProfile; }
            set { _currentProfile = value; }
        }

        protected BaseGraph _graph;

        public StreamingAndRecording RecordGraph
        {
            get
            {
                return _graph as StreamingAndRecording;
            }
        }

        public StreamingTVGraph TVGraph
        {
            get
            {
                if (_graph != null)
                {
                    StreamingTVGraph tvGraph = _graph as StreamingTVGraph;
                    if (tvGraph != null)
                    {
                        return tvGraph;
                    }
                    else
                    {
                        throw new Exception("This graph is not a StreamingTVGraph.");
                    }
                }
                else
                {
                    throw new Exception("A graph has not been opened yet.");
                }
            }
        }

        private int _keepAlives = 0;

#if USE_GLOBAL_GRAPHSERVICELOCK
        private static object _graphServiceLock = new object();
#endif

        public string UserName
        {
            get
            {
                return ClientConnectRequest.UserName;
            }
        }

        public virtual void Dispose()
        {
            if (_graph != null)
            {
              //  try
                {
                    _graph.RemoveClient(this);
                    _graph = null;
                }
             //   catch (Exception exception)
             //   {
             //       AppLogger.Message("RemoveClient failed because " + exception.Message);
             //   }
            }
        }

        #region IStream

        public virtual SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest)
        {
            AppLogger.Message(clientConnectRequest.UserName + " OpenGraph(" + clientConnectRequest.SourceName + ")");
#if USE_GLOBAL_GRAPHSERVICELOCK
            lock (_graphServiceLock)
            {
#endif
                this.ClientConnectRequest = clientConnectRequest;
                this.ClientConnectRequest.InterfaceAddress = GetIncomingInterfaceAddress();
                this.OpenGraphRequest = new OpenGraphRequest(this.ClientConnectRequest);
                this.OpenGraphRequest.ClientReconnecting = false;
                _graph = OpenGraph();
                _graph.AddClient(this);
                SessionDescription sd = _graph.FillOutSessionDescription(this.OpenGraphRequest);
                return sd;
#if USE_GLOBAL_GRAPHSERVICELOCK
            }
#endif
        }

        public virtual SessionDescription Reconnect(ClientConnectRequest clientConnectRequest)
        {
            AppLogger.Message(clientConnectRequest.UserName + " Reconnect(" + clientConnectRequest.SourceName + ")");
#if USE_GLOBAL_GRAPHSERVICELOCK
            lock (_graphServiceLock)
            {
#endif
                this.ClientConnectRequest = clientConnectRequest;
                this.ClientConnectRequest.InterfaceAddress = GetIncomingInterfaceAddress();
                this.OpenGraphRequest.Update(this.ClientConnectRequest);
                this.OpenGraphRequest.ClientReconnecting = true;
                _graph = OpenGraph();
                _graph.AddClient(this);
                SessionDescription sd = _graph.FillOutSessionDescription(this.OpenGraphRequest);
                return sd;
#if USE_GLOBAL_GRAPHSERVICELOCK
            }
#endif
        }

        /// <summary>
        /// Returns the interface address which the current request came into
        /// </summary>
        protected IPAddress GetIncomingInterfaceAddress()
        {
            return AddressLookup.GetHostAddress(OperationContext.Current.IncomingMessageHeaders.To.Host);
        }

        public virtual SessionDescription SetProfile(Profile newProfile)
        {
            AppLogger.Message("CommonStreamService.SetProfile " + newProfile.Name);
#if USE_GLOBAL_GRAPHSERVICELOCK
            lock (_graphServiceLock)
            {
#endif
                if (_graph != null)
                {
                    try
                    {
                        if (newProfile.Name.ToLowerInvariant().Contains("custom"))
                        {
                            CurrentProfile = newProfile;
                        }
                        else
                        {
                            CurrentProfile = _graph.FindProfile(newProfile.Name);
                        }
                        if (CurrentProfile != null)
                        {
                            _graph.ChangeProfile(CurrentProfile);
                        }
                        return null;
                    }
                    catch (ServerGraphRebuildException exc)
                    {
                        AppLogger.Dump(exc);
                        AppLogger.Message("CommonStreamService.SetProfile() caught ServerGraphRebuildException--opening a new graph");
                        _graph.Stop();
                        _graph.State = ServerGraphState.Aborted;
                        _graph.RemoveClient(this);
                        // Save newProfile in the dictionary
                        GraphManager.RemoveAndDisposeGraph(_graph);
                        this.OpenGraphRequest.Profile = CurrentProfile;
                        _graph = OpenGraph();
                        _graph.AddClient(this);
                        SessionDescription sd = _graph.FillOutSessionDescription(this.OpenGraphRequest);
                        return sd;
                    }
                }
                else
                {
                    throw new ServiceHasNoGraphException();
                }
#if USE_GLOBAL_GRAPHSERVICELOCK
            }
#endif
        }

        public virtual void KeepAlive()
        {
            if (_graph != null)
            {
                if ((_graph.State == ServerGraphState.Aborted) || (_graph.State == ServerGraphState.Rebuild))
                {
                    throw new ServiceHasNoGraphException("the server graph has been aborted");
                }
            }
            else
            {
                throw new ServiceHasNoGraphException();
            }
            _keepAlives++;
        }

        #endregion

        #region Support

        protected BaseGraph OpenGraph()
        {
            AppLogger.Message(String.Format("OpenGraph {0}", ClientConnectRequest.SourceName));

            //retreives the correct source config based on the OpenGraphRequest
            StreamSourceInfo sourceConfig = GetSourceConfig();

            //handles logical groups
            if (sourceConfig.SourceType == SourceType.LogicalGroup)
            {
                StreamSources sources = StreamSources.LoadFromFile();
                foreach (string physicalName in sourceConfig.LogicalGroupSourceNames)
                {
                    StreamSourceInfo physicalSourceConfig = sources.FindSource(physicalName);
                    BaseGraph graph = GraphManager.TryFindGraph(physicalName);
                    if (graph == null)
                    {
                        return OpenPhysicalGraph(physicalSourceConfig);
                    }
                    else
                    {
                        if (graph.NumberOfClients < physicalSourceConfig.MaxClients)
                        {
                            return OpenPhysicalGraph(physicalSourceConfig);
                        }
                    }
                }
                throw new Exception("no sources available for " + ClientConnectRequest.SourceName);
            }
            else
            {
                try
                {
                    return OpenPhysicalGraph(sourceConfig);
                }
                catch (SourceHasMaxClientsException e)
                {
                    throw e;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    throw new Exception("Source " + ClientConnectRequest.SourceName + " is not currently available", exc);
                }
            }
        }

        /// <summary>
        /// This method uses the OpenGraphRequest to configure the StreamSourceInfo which is then used to
        /// actually open the physical graph.
        /// </summary>
        /// <returns>Returns the StreamSourceInfo neccesary to open the physical graph</returns>
        protected virtual StreamSourceInfo GetSourceConfig()
        {
            StreamSourceInfo sourceConfig;
            StreamSources sources = StreamSources.LoadFromFile();
            if (ClientConnectRequest.SourceName == null)
            {
                throw new ArgumentNullException("ClientConnectRequest.SourceName", "ClientConnectRequest.SourceName cannot be null!");
            }
//            if (ClientConnectRequest.SourceName.Contains(":"))
//            {
//                string[] parts = ClientConnectRequest.SourceName.Split(new char[] { ':' }, 2);
//                string originServerAddress = parts[0];
//                string sourceName = parts[1];
//                AppLogger.Message("OpenGraph is for OriginServer " + originServerAddress);
//                StreamSourceInfo originServerSourceInfo = QueryOriginServers.FindSource(ClientConnectRequest.SourceName);
//                AppLogger.Message("Found originServerSourceInfo; Description = " + originServerSourceInfo.Description);

//                sourceConfig = new StreamSourceInfo(sources.FindSource(CommonStreamService.RestreamerSourceName));
//                sourceConfig.SourceType = SourceType.LTRestreamer;
//                sourceConfig.SourceName = ClientConnectRequest.SourceName;
//                sourceConfig.Description = originServerSourceInfo.Description;
//                sourceConfig.CameraControl = null;  //removes the CameraControl from restreamed streams
//            }
//            else
            {
                sourceConfig = sources.FindSource(ClientConnectRequest.SourceName);
            }

            return sourceConfig;
        }

        private BaseGraph OpenPhysicalGraph(StreamSourceInfo sourceConfig)
        {
            BaseGraph returnedGraph = null;
            AppLogger.Message(String.Format("OpenPhysicalGraph {0}", sourceConfig.SourceName));
            // CheckSecurity(sourceConfig, openGraphRequest);
            try
            {
                if (GraphManager.ShutdownInProgress)
                {
                    throw new Exception("OpenGraph is disabled--the media server is shutting down.");
                }
                try
                {
                    lock (GraphManager.GraphMap)
                    {
                        AppLogger.Message("   Try to fetch existing graph");
                        returnedGraph = GraphManager.GraphMap[sourceConfig.SourceName];
                    }
                }
                catch
                {
                    lock (GraphManager.GraphMap)
                    {
                        AppLogger.Message("   Constructing new graph...");
                        returnedGraph = BaseGraph.CreateInstance(sourceConfig, this.OpenGraphRequest);
                        GraphManager.GraphMap[sourceConfig.SourceName] = returnedGraph;
                    }
                }
            }
            catch (Exception exception)
            {
                AppLogger.Message(String.Format("Unable to OpenGraph {0}", exception.Message));
                throw new Exception("Unable to OpenGraph because " + exception.Message, exception);
            }
            if (returnedGraph.NumberOfClients >= sourceConfig.MaxClients)
            {
                string message = "Connect aborted - the maximum number of clients has been reached." + Environment.NewLine;
                message += "Source was " + sourceConfig.SourceName + Environment.NewLine;
                message += "Current connected users:" + Environment.NewLine;
                foreach (string username in returnedGraph.ClientList)
                {
                    message += username + Environment.NewLine;
                }
                throw new SourceHasMaxClientsException(message);
            }
            return returnedGraph;
        }

        private void CheckSecurity(StreamSourceInfo sourceConfig)
        {
            // Enable this code when authentication/authorization is needed
            ServiceSecurityContext sc = ServiceSecurityContext.Current;
            if (sc != null)
            {
                AppLogger.Message("We have a security context");
                IIdentity id1 = sc.PrimaryIdentity;
                if (id1 != null)
                {
                    AppLogger.Message(String.Format("Primary identity {0}", id1.Name));
                }
                IIdentity id2 = sc.WindowsIdentity;
                if (id2 != null)
                {
                    AppLogger.Message(String.Format("Windows identity {0}", id2.Name));
                }
                AuthorizationContext ctx = sc.AuthorizationContext;
                if (ctx != null)
                {
                    foreach (ClaimSet claimSet in ctx.ClaimSets)
                    {
                        foreach (Claim claim in claimSet)
                        {
                            AppLogger.Message("ClaimType: " + claim.ClaimType);
                            AppLogger.Message("Resource: " + claim.Resource);
                            AppLogger.Message("Right: " + claim.Right);
                        }
                    }
                }
                else
                {
                    AppLogger.Message("No auth context");
                }
            }
        }

        #endregion
    }
}
