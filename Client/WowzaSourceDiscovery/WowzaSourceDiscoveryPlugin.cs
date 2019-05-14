using System;
using System.Collections.Generic;
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

namespace FutureConcepts.Media.Client.WowzaSourceDiscovery
{
    /// <summary>
    /// This class works to poll WOWZA Media Servers via HTTP serverinfo (see Wowza.cs)
    /// </summary>
    public class WowzaSourceDiscoveryPlugin : SourceDiscoveryPlugin
    {
       /// <summary>
        /// Creates a new instance
        /// </summary>
        public WowzaSourceDiscoveryPlugin(SourceDiscoveryDefinition sourceDiscoveryDefinition)
        {
            PollInterval = 30000;
            HostAddress = "restreamer.antaresx.net";
            HostPort = 8086;
            Username = "antaresx";
            Password = "shuttle88";
        }

        private int _pollInterval;
        /// <summary>
        /// The interval (in ms) to poll the AX Services
        /// </summary>
        [XmlElement]
        public int PollInterval
        {
            get
            {
                return _pollInterval;
            }
            set
            {
                _pollInterval = value;
            }
        }

        [XmlElement]
        public String HostAddress { get; set; }

        [XmlElement]
        public int HostPort { get; set; }

        [XmlElement]
        public String Username { get; set; }

        [XmlElement]
        public String Password { get; set; }

        private IAsyncResult _asyncContext;
        private Boolean _abort = false;

        private SourceDiscoveryGroup _group;

        /// <summary>
        /// Begins polling for Media Servers
        /// </summary>
        public override void Start()
        {
            AsyncCallback callback = new AsyncCallback(ProcessDnsResult);
            _asyncContext = Dns.BeginGetHostEntry(HostAddress, callback, HostAddress);
        }

        /// <summary>
        /// Stops polling for Media Servers
        /// </summary>
        public override void Stop()
        {
            _abort = true;
        }

        private void ProcessDnsResult(IAsyncResult result)
        {
            String hostName = (String)result.AsyncState;
            IPHostEntry host = Dns.EndGetHostEntry(result);
            foreach (IPAddress ipAddress in host.AddressList)
            {
                try
                {
                    DoWowzaQuery(ipAddress);
                }
                catch (Exception e)
                {
                }

            }
            if (_abort == false)
            {
                Thread.Sleep(this.PollInterval);
                Start();
            }
        }

        private void DoWowzaQuery(IPAddress ipAddress)
        {
            Boolean fireOnline = false;
            Boolean fireChanged = false;
            String hostAddress = ipAddress.ToString()+":"+HostPort;
            SourceDiscoveryGroup group = Wowza.QueryWowza(hostAddress, Username, Password);
            if (_group == null)
            {
                _group = group;
                fireOnline = true;
            }
            else
            {
                if (_group.Sources.Count > 0)
                {
                    foreach (StreamSourceInfo source in _group.Sources)
                    {
                        try
                        {
                            StreamSourceInfo newSourceInfo = group.FindSource(source.SourceName);
                        }
                        catch (Exception e)
                        {
                            _group.Sources.Remove(source);
                            fireChanged = true;
                        }
                    }
                }
                else
                {
                    fireChanged = true;
                }
                foreach (StreamSourceInfo source in group.Sources)
                {
                    try
                    {
                        StreamSourceInfo previousSource = _group.FindSource(source.SourceName);
//                        if (source.ClientURL == previousSource.ClientURL == false)
                        if (true)
                        {
                            _group.Sources.Remove(previousSource);
                            _group.Sources.Add(source);
                            fireChanged = true;
                        }
                    }
                    catch (Exception e)
                    {
                        _group.Sources.Add(source);
                        fireChanged = true;
                    }
                }
            }
            if (fireOnline)
            {
                FireGroupOnline(_group);
            }
            else if (fireChanged)
            {
                FireGroupChanged(_group);
            }
        }
    }
}
