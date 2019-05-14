using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FutureConcepts.Media.DirectShowLib.Framework;
using FutureConcepts.Media.Server.Graphs;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server
{
    /// <summary>
    /// Maintains a pool of the graphs that have been created so far in the lifetime of the service
    /// </summary>
    /// <author>darnold circa 2005</author>
    public class GraphManager
    {
        static GraphManager()
        {
            try
            {
                int stashTimeout;
                if (int.TryParse(ConfigurationManager.AppSettings["GraphManagerStashTimeout"], out stashTimeout))
                {
                    _graphStashTimeout = TimeSpan.FromMilliseconds(stashTimeout);
                }
                _graphStashCleanupTimer = new Timer(new TimerCallback(StashCleanup), null, _graphStashTimeout, _graphStashTimeout);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
                ErrorLogger.DumpToEventLog(ex, EventLogEntryType.Error);
            }
        }

        private static Dictionary<string, BaseGraph> _graphMap = new Dictionary<string, BaseGraph>();
        /// <summary>
        /// Current map of SourceName to Graph instance
        /// </summary>
        public static Dictionary<string, BaseGraph> GraphMap
        {
            get
            {
                return _graphMap;
            }
        }

        private static TimeSpan _graphStashTimeout = TimeSpan.FromSeconds(60);

        private static Timer _graphStashCleanupTimer;

        private static Dictionary<string, BaseDSGraph> GraphStash = new Dictionary<string, BaseDSGraph>();
        private static Dictionary<string, DateTime> GraphStashAge = new Dictionary<string, DateTime>();

        public static void Stash(string sourceName, BaseDSGraph graph)
        {
            lock (GraphStash)
            {
                if (GraphStash.ContainsKey(sourceName))
                {
                    throw new Exception("graph already stashed for " + sourceName);
                }

                GraphStash[sourceName] = graph;
                GraphStashAge[sourceName] = DateTime.Now;
            }
        }

        public static BaseDSGraph UnStash(string sourceName)
        {
            lock (GraphStash)
            {
                if (GraphStash.ContainsKey(sourceName))
                {
                    BaseDSGraph graph = GraphStash[sourceName];
                    GraphStash.Remove(sourceName);
                    GraphStashAge.Remove(sourceName);

                    return graph;
                }
                else
                {
                    return null;
                }
            }
        }

        private static void StashCleanup(object state)
        {
            lock (GraphStash)
            {
                try
                {
                    if (GraphStash.Count < 1)
                    {
                        return;
                    }

                    var expired = from i in GraphStash
                                  where
                                      (from age in GraphStashAge
                                       where ((DateTime.Now - age.Value) > _graphStashTimeout)
                                       select age.Key)
                                  .Contains(i.Key)
                                  select i;

                    List<KeyValuePair<string, BaseDSGraph>> cleanupList = expired.ToList();

                    foreach (KeyValuePair<string, BaseDSGraph> g in cleanupList)
                    {
                        AppLogger.Message("StashCleanup: " + g.Key + " expired");
                        GraphStash.Remove(g.Key);
                        GraphStashAge.Remove(g.Key);
                        g.Value.Stop();
                        g.Value.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Dump(ex);
                }
            }
        }

        private static bool _shutdownInProgress = false;
        /// <summary>
        /// Returns true if the Media Server is shutting down.
        /// Setting this property to true causes all incoming connections to be rejected.
        /// </summary>
        public static bool ShutdownInProgress
        {
            get
            {
                return _shutdownInProgress;
            }
            set
            {
                _shutdownInProgress = value;
            }
        }

        public static void StopGraph(string sourceName)
        {
            Debug.WriteLine(String.Format("StopGraph {0}", sourceName));
            if (_graphMap.ContainsKey(sourceName))
            {
                BaseGraph graph = _graphMap[sourceName];
                graph.Stop();
                RemoveAndDisposeGraph(graph);
            }
        }

        public static void StartGraph(string sourceName)
        {
            if (_graphMap.ContainsKey(sourceName) == false)
            {
                StreamSources streamSources = StreamSources.LoadFromFile(false);
                if (streamSources != null)
                {
                    StreamSourceInfo streamSourceInfo = streamSources.FindSource(sourceName);
                    if (streamSourceInfo != null)
                    {
                        OpenGraphRequest openGraphRequest = new OpenGraphRequest();
                        openGraphRequest.Id = Guid.NewGuid();
                        openGraphRequest.SourceName = streamSourceInfo.SourceName;
                        openGraphRequest.UserName = "restreamer";
                        BaseGraph graph = BaseGraph.CreateInstance(streamSourceInfo, openGraphRequest);
                        graph.Run();
                        if (graph != null)
                        {
                            _graphMap.Add(streamSourceInfo.SourceName, graph);
                        }
                    }
                }
            }
        }

        public static void StartPushGraphs()
        {
            try
            {
                StreamSources streamSources = StreamSources.LoadFromFile(false);
                StartSources startSources = StartSources.LoadFromFile(false);
                if ((streamSources != null) && (startSources != null))
                {
                    foreach (StartSource startSource in startSources.Items)
                    {
                        StreamSourceInfo streamSourceInfo = streamSources.FindSource(startSource.SourceName);
                        if (streamSourceInfo != null)
                        {
                            AppLogger.Message(String.Format("Starting source {0}", streamSourceInfo.SourceName));
                            if (streamSourceInfo.DeviceAddress != null)
                            {
                                AppLogger.Message(String.Format("Starting Channel={0} Input={1}", streamSourceInfo.DeviceAddress.Channel, streamSourceInfo.DeviceAddress.Input));
                            }
                            OpenGraphRequest openGraphRequest = new OpenGraphRequest();
                            openGraphRequest.Id = Guid.NewGuid();
                            openGraphRequest.SourceName = streamSourceInfo.SourceName;
                            openGraphRequest.UserName = "restreamer";
                            BaseGraph graph = BaseGraph.CreateInstance(streamSourceInfo, openGraphRequest);
                            graph.Run();
                            if (graph != null)
                            {
                                _graphMap.Add(streamSourceInfo.SourceName, graph);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AppLogger.Dump(e);
            }
        }

        public static void StopAllGraphs()
        {
            List<BaseGraph> graphs = new List<BaseGraph>(_graphMap.Values);
            AppLogger.Message("GraphManager StopAllGraphs");
            foreach (BaseGraph graph in graphs)
            {
                graph.Stop();
            }
            AppLogger.Message("GraphManager StopAllGraphs done");
        }

        public static void AbortAllGraphs()
        {
            List<BaseGraph> graphs = new List<BaseGraph>(_graphMap.Values);
            AppLogger.Message("GraphManager AbortAllGraphs");
            foreach (BaseGraph graph in graphs)
            {
                graph.Abort();
            }
            AppLogger.Message("GraphManager AbortAllGraphs done");
        }

        #region Utility Methods / Accessors

        /// <summary>
        /// Removes a graph from the map, and then invokes dispose on the graph instance
        /// </summary>
        /// <param name="graph">The graph instance to release</param>
        public static void RemoveAndDisposeGraph(BaseGraph graph)
        {
            lock (_graphMap)
            {
                _graphMap.Remove(graph.SourceConfig.SourceName);
            }
            graph.Dispose(true);
        }

        /// <summary>
        /// Attempts to locate a graph in the map, given the source name. Also verifies that the provided
        /// source name has been actually configured in the server's source configuration.
        /// </summary>
        /// <param name="SourceName">source name to retreive</param>
        /// <returns>the graph instance associated with the given source name.</returns>
        /// <exception cref="T:FutureConcepts.Media.SourceConfigException">
        /// Thrown if the given sourcename is not configured in the server's source config
        /// </exception>
        /// <exception cref="T:System.Exception">
        /// Thrown if no such graph has been instantiated; Can also be thrown if the server configuration does not exist.
        /// </exception>
        public static BaseGraph FindGraph(string SourceName)
        {
            StreamSourceInfo source = StreamSources.LoadFromFile().FindSource(SourceName);
            lock (_graphMap)
            {
                BaseGraph graph = _graphMap[source.SourceName];
                if (graph == null)
                {
                    throw new Exception("A graph for source " + SourceName + " does not exist.");
                }
                return graph;
            }
        }

        /// <summary>
        /// Tries to fetch a graph from the map
        /// </summary>
        /// <param name="sourceName">source name to fetch</param>
        /// <returns>an instance of the graph, if such a graph exists, or null if no instance exists</returns>
        public static BaseGraph TryFindGraph(string sourceName)
        {
            lock (_graphMap)
            {
                try
                {
                    return _graphMap[sourceName];
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Removes a graph from the map
        /// </summary>
        /// <param name="sourceName">source name of the graph to remove</param>
        public static void RemoveGraph(string sourceName)
        {
            lock (_graphMap)
            {
                _graphMap.Remove(sourceName);
            }
        }

        #endregion
    }
}
