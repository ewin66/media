using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Client interface for the recording client
    /// </summary>
    public class RecordClient : Component, IRecord
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="serverAddress">server hostname or ip address</param>
        /// <param name="recordCallback">the <see cref="T:IRecordCallback"/> you want to use</param>
        public RecordClient(string serverAddress, IRecordCallback recordCallback)
        {
            IPHostEntry entry = AddressLookup.GetHostEntry(serverAddress);
            _serverAddress = entry.AddressList[0].ToString();

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, true);
          //  binding.SendTimeout = TimeSpan.FromSeconds(Convert.ToInt32(ConfigurationManager.AppSettings["GraphControlClientSendTimeout"]));
            _proxyFactory = new DuplexChannelFactory<IRecord>(recordCallback, binding, @"net.tcp://" + ServerAddress + @":8099/RecordService");
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
        /// Disposes this object
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
                    _serverKeepAliveTimer.Dispose();
                    _serverKeepAliveTimer = null;
                }
            }
        }

        private System.Timers.Timer _serverKeepAliveTimer;

        private SessionDescription _sessionDescription;
        /// <summary>
        /// The current <see cref="T:SessionDescription"/>
        /// </summary>
        public SessionDescription SessionDescription
        {
            get
            {
                return _sessionDescription;
            }
        }

        private ProfileGroup _profileGroup;
        /// <summary>
        /// The current profile group
        /// </summary>
        public ProfileGroup ProfileGroup
        {
            get
            {
                return _profileGroup;
            }
            set
            {
                _profileGroup = value;
            }
        }

        private string _serverAddress = null;
        /// <summary>
        /// Gets the server address connected to
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
        /// Gets the associated <see cref="T:ClientConnectRequest"/>
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
        /// Gets the UserName that was supplied to the server
        /// </summary>
        public string UserName
        {
            get
            {
                return _clientRequest.UserName;
            }
        }

        private ChannelFactory<IRecord> _proxyFactory;
        private IRecord _proxy;

        /// <summary>
        /// Opens the graph
        /// </summary>
        /// <param name="clientConnectRequest">connection parameters</param>
        /// <returns><see cref="T:SessionDescription"/></returns>
        public SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest)
        {
            ClientConnectRequest = clientConnectRequest;
            _sessionDescription = _proxy.OpenGraph(ClientConnectRequest);
            return _sessionDescription;
        }

        /// <summary>
        /// Reconnects to the graph
        /// </summary>
        /// <param name="clientConnectRequest">connection parameters</param>
        /// <returns><see cref="T:SessionDescription"/></returns>
        public SessionDescription Reconnect(ClientConnectRequest clientConnectRequest)
        {
            ClientConnectRequest = clientConnectRequest;
            _sessionDescription = _proxy.OpenGraph(ClientConnectRequest);
            return _sessionDescription;
        }

        /// <summary>
        /// Sets the profile on the server
        /// </summary>
        /// <param name="newProfile">new <see cref="T:Profile"/> to use</param>
        /// <returns></returns>
        public SessionDescription SetProfile(Profile newProfile)
        {
            return _proxy.SetProfile(newProfile);
        }

        /// <summary>
        /// do not call
        /// </summary>
        public void KeepAlive()
        {
            throw new NotImplementedException("RecordClient.KeepAlive");
        }

        /// <summary>
        /// Begins a recording session
        /// </summary>
        public void BeginSession()
        {
            if (_proxy != null)
            {
                _proxy.BeginSession();
            }
        }

        /// <summary>
        /// Starts a recording
        /// </summary>
        public void StartRecording()
        {
            if (_proxy != null)
            {
                _proxy.StartRecording();
            }
        }

        /// <summary>
        /// Stops a recording
        /// </summary>
        public void StopRecording()
        {
            if (_proxy != null)
            {
                _proxy.StopRecording();
            }
        }

        /// <summary>
        /// Ends a recording session
        /// </summary>
        public void EndSession()
        {
            if (_proxy != null)
            {
                _proxy.EndSession();
            }
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
