using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutureConcepts.Media.Contract;
using System.ServiceModel;
using System.ComponentModel;
using System.Diagnostics;
using FutureConcepts.Media.Server.Graphs;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.IndigoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                     ConcurrencyMode = ConcurrencyMode.Single)]
    public class ServerStatusService : IServerStatus
    {
        public ServerStatusProperties Query(string sourceName)
        {
            AppLogger.Message("ServerStatusService.Query " + (sourceName ?? "<server>"));
            try
            {
                if (!string.IsNullOrEmpty(sourceName))
                {
                    return GenerateStatus(sourceName, GraphManager.TryFindGraph(sourceName));
                }
                else
                {
                    return AggregateStreamInfo();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
                return null;
            }
        }

        private ServerStatusProperties AggregateStreamInfo()
        {
            ServerStatusProperties server = new ServerStatusProperties();
            server.State = ServerStatusState.Unknown;

            //determine count of graphs that have a specific state
            Dictionary<ServerStatusState, int> statusBuckets = new Dictionary<ServerStatusState, int>();

            foreach (BaseGraph g in GraphManager.GraphMap.Values.ToArray<BaseGraph>())
            {
                //grab the state info
                ServerStatusState thisState = GraphStateToStatus(g.State, g.NumberOfClients);
                if (!statusBuckets.ContainsKey(thisState))
                {
                    statusBuckets.Add(thisState, 0);
                }
                statusBuckets[thisState]++;

                //add user count
                server.UserCount += g.NumberOfClients;

                //get bitrate
                if (g is StreamingGraph)
                {
                    server.Bitrate += (((StreamingGraph)g).AverageBitRate * g.NumberOfClients);
                }
            }

            server.Bitrate /= 1024;

            //determine server status
            if (MediaServer.ServerNeedsPowerCycle)
            {
                server.State = ServerStatusState.NeedsPowerCycle;
            }
            else if (GraphManager.GraphMap.Count == 0)
            {
                server.State = ServerStatusState.Idle;
            }
            else
            {
                KeyValuePair<ServerStatusState, int> max = new KeyValuePair<ServerStatusState, int>(ServerStatusState.Unknown, 0);
                foreach (KeyValuePair<ServerStatusState, int> cur in statusBuckets)
                {
                    if (cur.Value > max.Value)
                    {
                        max = cur;
                    }
                }
                server.State = max.Key;
            }

            return server;
        }

        private ServerStatusProperties GenerateStatus(string sourceName, BaseGraph graph)
        {
            ServerStatusProperties p = new ServerStatusProperties();
            p.SourceName = sourceName;
            if (graph == null)
            {
                p.State = ServerStatusState.Idle;
            }
            else
            {
                p.State = GraphStateToStatus(graph.State, graph.NumberOfClients);

                //get current profile name
                p.Profile = graph.CurrentProfile.Name;

                //get users list/count
                p.Users = graph.ClientList;
                p.UserCount = p.Users.Count;

                if (graph is StreamingGraph)
                {
                    p.Bitrate = ((StreamingGraph)graph).AverageBitRate;
                }
            }

            return p;
        }

        private ServerStatusState GraphStateToStatus(ServerGraphState state, int numClients)
        {
            if ((state == ServerGraphState.Running) && (numClients > 0))
            {
                return ServerStatusState.Streaming;
            }
            else if ((state == ServerGraphState.Rebuild) ||
                     (state == ServerGraphState.Aborted))
            {
                return ServerStatusState.Recovering;
            }
            else
            {
                return ServerStatusState.Idle;
            }
        }
    }
}
