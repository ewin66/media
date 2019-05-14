using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.SVD.QueryVideoServices
{
    public class ServerWatcher
    {
        private QueryMediaServers qms;

        public ServerWatcher()
        {
            qms = new QueryMediaServers();

            qms.PopulateRegularServers = true;
            qms.PopulateRestreamers = true;
            qms.AcceptAllSourceTypes = true;
            qms.AcceptHiddenSources = true;

            qms.RequestedServerInfoParams = RequestedServerInfoParams.OriginServers |
                                            RequestedServerInfoParams.ServerAddress |
                                            RequestedServerInfoParams.ServerName |
                                            RequestedServerInfoParams.StreamSources |
                                            RequestedServerInfoParams.VersionInfo;
            qms.RequestedStreamSourceInfoParams = RequestedStreamSourceInfoParams.CameraControl |
                                                  RequestedStreamSourceInfoParams.Description |
                                                  RequestedStreamSourceInfoParams.MicrowaveControl |
                                                  RequestedStreamSourceInfoParams.SourceName |
                                                  RequestedStreamSourceInfoParams.SourceType |
                                                  RequestedStreamSourceInfoParams.Hidden;
            
            qms.ServerInfoChanged += new EventHandler<QueryMediaServers.ServerEventArgs>(qms_ServerInfoChanged);
            qms.ServerNowAvailable += new EventHandler<QueryMediaServers.ServerEventArgs>(qms_ServerNowAvailable);
            qms.ServerNowUnavailable += new EventHandler<QueryMediaServers.ServerEventArgs>(qms_ServerNowUnavailable);
        }

        void qms_ServerInfoChanged(object sender, QueryMediaServers.ServerEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine(String.Format("{0} -- ServerInfoChanged --", DateTime.Now.ToString()));
            Engine.PrintServerInfo(qms.MediaServers[e.ServerAddress]);
        }

        void qms_ServerNowAvailable(object sender, QueryMediaServers.ServerEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine(String.Format("{0} -- ServerNowAvailable --", DateTime.Now.ToString()));
            Engine.PrintServerInfo(qms.MediaServers[e.ServerAddress]);
        }

        void qms_ServerNowUnavailable(object sender, QueryMediaServers.ServerEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine(String.Format("{0} -- ServerNowUnavailable --", DateTime.Now.ToString()));
            Console.WriteLine(String.Format("IP Address: {0}", e.ServerAddress));
        }

        /// <summary>
        /// Call this method to Run the server watcher
        /// </summary>
        internal void Run()
        {
            qms.Start();
            Console.WriteLine("Press Escape to stop.");
            Console.WriteLine();
            ConsoleKeyInfo ch = new ConsoleKeyInfo();
            do
            {
                if (Console.KeyAvailable)
                {
                    ch = Console.ReadKey(true);
                }
            } while (ch.Key != ConsoleKey.Escape);
            qms.Stop();
        }
    }
}
