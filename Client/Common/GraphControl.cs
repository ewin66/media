using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Client interface to control a media stream
    /// </summary>
    public class GraphControl : Component, IStream
    {
        /// <summary>
        /// Initializes a connection to the specified server
        /// </summary>
        /// <param name="serverAddress">server hostname or IP address</param>
        public GraphControl(string serverAddress)
        {
            IPHostEntry entry = AddressLookup.GetHostEntry(serverAddress);
            _serverAddress = entry.AddressList[0].ToString();

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, true);

            int sendTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["GraphControlClientSendTimeout"]);
            if (sendTimeout == 0)
            {
                sendTimeout = 20;
            }
            binding.SendTimeout = TimeSpan.FromSeconds(sendTimeout);

            int closeTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["GraphControlClientCloseTimeout"]);
            if (closeTimeout == 0)
            {
                closeTimeout = 5;
            }
            binding.CloseTimeout = TimeSpan.FromSeconds(closeTimeout);
            
            _proxyFactory = new ChannelFactory<IStream>(binding, @"net.tcp://" + ServerAddress + @":8098/StreamService");
            _proxyFactory.Opening += new EventHandler(Proxy_Opening);
            _proxyFactory.Opened += new EventHandler(Proxy_Opened);
            _proxyFactory.Closing += new EventHandler(Proxy_Closing);
            _proxyFactory.Closed += new EventHandler(Proxy_Closed);
            _proxyFactory.Faulted += new EventHandler(Proxy_Faulted);
            _proxyFactory.Open();
            _proxy = _proxyFactory.CreateChannel();

            _serverKeepAliveTimer = new System.Timers.Timer();
            _serverKeepAliveTimer.Interval = 15000;
            _serverKeepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(ServerKeepAliveTimer_Elapsed);
            _serverKeepAliveTimer.Enabled = true;
        }

        /// <summary>
        /// Raised when the connection has been closed
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Aborts the proxy factory in the case that it is faulted, or otherwise worthless
        /// </summary>
        public void Abort()
        {
            try
            {
                if (_proxyFactory != null)
                {
                    _proxyFactory.Abort();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Closes the session, releaseing resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            if (disposing)
            {
                Closed = null;
                if (_proxyFactory != null)
                {
                    try
                    {
                        _proxyFactory.Close();
                    }
                    catch
                    {
                    }
                    _proxyFactory = null;
                    _proxy = null;
                }
                if (_serverKeepAliveTimer != null)
                {
                    _serverKeepAliveTimer.Enabled = false;
                    _serverKeepAliveTimer.Elapsed -= new System.Timers.ElapsedEventHandler(ServerKeepAliveTimer_Elapsed);
                    _serverKeepAliveTimer.Dispose();
                    _serverKeepAliveTimer = null;
                }
            }
        }

        private System.Timers.Timer _serverKeepAliveTimer;

        private SessionDescription _sessionDescription;
        /// <summary>
        /// Gets the associated <see cref="T:SessionDescription"/> for this session
        /// </summary>
        public SessionDescription SessionDescription
        {
            get
            {
                return _sessionDescription;
            }
            set
            {
                _sessionDescription = value;
            }
        }

        private string _serverAddress = null;
        /// <summary>
        /// The server adress connected to
        /// </summary>
        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
        }

        private ClientConnectRequest _clientRequest;
        /// <summary>
        /// The associated <see cref="T:ClientConnectRequest"/>.
        /// </summary>
        private ClientConnectRequest ClientConnectRequest
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

        /// <summary>
        /// The UserName that was supplied to the server
        /// </summary>
        public string UserName
        {
            get
            {
                return _clientRequest.UserName;
            }
        }

        private ChannelFactory<IStream> _proxyFactory;
        private IStream _proxy;

        /// <summary>
        /// Initiates a session to a particular source. If the graph exists, then the profile is reset to its default.
        /// </summary>
        /// <param name="clientConnectRequest">A properly filled-out ClientConnectRequest, indicating which SourceName to connect to.</param>
        /// <returns>A session description for the client.</returns>
        public SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest)
        {
            ClientConnectRequest = clientConnectRequest;
            SessionDescription = _proxy.OpenGraph(ClientConnectRequest);
            return SessionDescription;
        }

        /// <summary>
        /// Reconnects to a session for a particular source. No change in profile will occur.
        /// </summary>
        /// <param name="clientConnectRequest">A properly filled-out ClientConnectRequest, indicating which SourceName to connect to.</param>
        /// <returns>A session description for the client.</returns>
        public SessionDescription Reconnect(ClientConnectRequest clientConnectRequest)
        {
            ClientConnectRequest = clientConnectRequest;
            SessionDescription = _proxy.Reconnect(ClientConnectRequest);
            return SessionDescription;
        }

        /// <summary>
        /// Changes the profile once the session has been established.
        /// </summary>
        /// <param name="newProfile">new profile configuration for the stream</param>
        /// <returns>An updated session description. If null is returned, the client will have to <see cref="M:Reconnect"/></returns>
        public SessionDescription SetProfile(Profile newProfile)
        {
            SessionDescription newSessionDescription = _proxy.SetProfile(newProfile);
            if (newSessionDescription != null)
            {
                SessionDescription = newSessionDescription;
            }
            return newSessionDescription;
        }

        /// <summary>
        /// Do not call
        /// </summary>
        public void KeepAlive()
        {
            throw new NotImplementedException("GraphControlClient.KeepAlive");
        }

        private void ServerKeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Debug.WriteLine("ServerKeepAliveTimer_Elapsed");
            if (_proxy != null)
            {
                try
                {
                    _proxy.KeepAlive();
                }
                catch
                {
                    if (_serverKeepAliveTimer != null)
                    {
                        _serverKeepAliveTimer.Enabled = false;
                    }
                    CloseProxy();
                }
            }
        }

        private void CloseProxy()
        {
            Debug.WriteLine("CloseProxy");
            if (_proxyFactory != null)
            {
                try
                {
                    _proxyFactory.Close();
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
                _proxyFactory = null;
            }
            _proxy = null;
        }

        private void Proxy_Opening(object sender, EventArgs e)
        {
            Debug.WriteLine("Proxy_Opening");
        }

        private void Proxy_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("Proxy_Opened");
        }

        private void Proxy_Closing(object sender, EventArgs e)
        {
            Debug.WriteLine("Proxy_Closing");
            if (Closed != null)
            {
                Debug.WriteLine("Propogating fault up");
                Closed(sender, e);
            }
        }

        private void Proxy_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Proxy_Closed");
        }

        private void Proxy_Faulted(object sender, EventArgs e)
        {
            Debug.WriteLine("Proxy_Faulted");
#if COMMENT
            if (Closed != null)
            {
                Debug.WriteLine("Propogating fault up");
                Closed(sender, e);
            }
#endif
        }
    }
}
