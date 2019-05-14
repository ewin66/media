using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;

using Antares.Video.Config;
using Antares.Video.ServerGraph;

using DirectShowLib;

namespace Antares.Video
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class GraphService : IStream, IDisposable
    {
        private OpenGraphRequest _openGraphRequest;

        private BaseGraph _graph;

        private int _keepAlives = 0;

        private static object _graphServiceLock = new object();

        public void Dispose()
        {
            if (_graph != null)
            {
                try
                {
                    _graph.RemoveClient(_openGraphRequest);
                    _graph = null;
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("RemoveClient failed because " + exception.Message);
                }
            }
        }

        public SessionDescription OpenGraph(OpenGraphRequest openGraphRequest)
        {
            _openGraphRequest = openGraphRequest;
            _graph = GraphManager.Singleton.OpenGraph(openGraphRequest);
            return _graph.SessionDescription;
        }

        public void SetProfile(string profileName)
        {
            if (_graph != null)
            {
                _graph.ProfileName = profileName;
                _graph.ResetParameters();
                _graph.SendProfileUpdate();

            }
        }

        public void SetTVMode(TVMode tvMode, bool connectAudio)
        {
            try
            {
                TVGraph.SetTVMode(tvMode, connectAudio);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(String.Format("SetTVMode failed because {0}", exception.Message));
                throw new Exception("SetTVMode failed because " + exception.Message);
            }
        }

        public void SetChannel(int channelNum)
        {
            try
            {
                TVGraph.SetChannel(channelNum);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(String.Format("SetChannel failed because {0}", exception.Message));
                throw new Exception("SetChannel failed because " + exception.Message);
            }
        }

        public bool IsSignalPresent()
        {
            AMTunerSignalStrength signalStrength = TVGraph.GetSignalStrength();
            return signalStrength == AMTunerSignalStrength.SignalPresent;
        }

        private StreamingTVGraph TVGraph
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

        public void KeepAlive()
        {
            if (true)
            {
                // this is the first time the user has sent a keep-alive
                // we need to send all the stream related info
                if (_graph != null)
                {
                    _graph.SendProfileUpdate();
                    _graph.SendClientsUpdate();
                }
            }
            System.Threading.Interlocked.Increment(ref _keepAlives);
        }
    }
}
