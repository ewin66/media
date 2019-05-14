using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ComponentModel;
using FutureConcepts.Media.TV;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Used to control a TV Stream session
    /// </summary>
    public class TVStreamControl : IDisposable, IChannelScanProvider, INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a TV Stream Control, and connects to the specified server
        /// </summary>
        /// <param name="serverAddress">IP or hostname of the server to connect to</param>
        public TVStreamControl(string serverAddress)
        {
            ChannelScanProgress = -1;

            IPHostEntry entry = AddressLookup.GetHostEntry(serverAddress);
            _serverAddress = entry.AddressList[0].ToString();

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, true);

            int timeout;
            //get SendTimeout from app.config
            if (Int32.TryParse(ConfigurationManager.AppSettings["GraphControlClientSendTimeout"], out timeout))
            {
                if (timeout > 0)
                {
                    binding.SendTimeout = TimeSpan.FromSeconds(timeout);
                }
            }

            //get CloseTimeout from app.config
            if (Int32.TryParse(ConfigurationManager.AppSettings["GraphControlClientCloseTimeout"], out timeout))
            {
                if (timeout > 0)
                {
                    binding.CloseTimeout = TimeSpan.FromSeconds(timeout);
                }
            }

            TVStreamControlCallback callback = new TVStreamControlCallback(this);

            _proxyFactory = new DuplexChannelFactory<ITVStream>(callback, binding, @"net.tcp://" + ServerAddress + @":8099/TVStreamService");
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

        #region Client/Server Business

        /// <summary>
        /// Raised when this client has been closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Clean up this object, and close the sesion
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

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

        private System.Timers.Timer _serverKeepAliveTimer;

        private SessionDescription _sessionDescription;

        /// <summary>
        /// Gets the current <see cref="T:SessionDescription"/>.
        /// </summary>
        public SessionDescription SessionDescription
        {
            get
            {
                return _sessionDescription;
            }
            private set
            {
                _sessionDescription = value;
                if (_sessionDescription.TVSessionInfo != null)
                {
                    this.Channel = _sessionDescription.TVSessionInfo.Channel;
                }
            }
        }

        private string _serverAddress = null;
        /// <summary>
        /// Gets the hostname or IP of the connected server
        /// </summary>
        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
        }

        private ClientConnectRequest _connectRequest;
        /// <summary>
        /// Gets or sets the associated <see cref="T:ClientConnectRequest"/>
        /// </summary>
        private ClientConnectRequest ClientConnectRequest
        {
            get
            {
                return _connectRequest;
            }
            set
            {
                _connectRequest = value;
            }
        }

        private ChannelFactory<ITVStream> _proxyFactory;
        private ITVStream _proxy;

        /// <summary>
        /// Calls <see cref="M:ITVStream.OpenGraph"/> on the server.
        /// </summary>
        /// <param name="clientConnectRequest">the <see cref="T:ClientConnectRequest"/> to use, indicating the target SourceName</param>
        /// <seealso cref="T:ITVStream"/>
        /// <returns>A <see cref="T:SessionDescription"/> describing the session. Can be accessed via <see cref="P:SessionDescription"/></returns>
        public SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest)
        {
            ClientConnectRequest = clientConnectRequest;
            SessionDescription = _proxy.OpenGraph(ClientConnectRequest);
            try
            {
                TVMode = SessionDescription.TVSessionInfo.TVMode;
                Channel = SessionDescription.TVSessionInfo.Channel;
            }
            catch { }
            return _sessionDescription;
        }

        /// <summary>
        /// Calls <see cref="M:ITVStream.Reconnect"/> on the server
        /// </summary>
        /// <param name="clientConnectRequest">the <see cref="T:ClientConnectRequest"/> to use, indicating the target SourceName</param>
        /// <seealso cref="T:ITVStream"/>
        /// <returns>A <see cref="T:SessionDescription"/> describing the session. Can be accessed via <see cref="P:SessionDescription"/></returns>
        public SessionDescription Reconnect(ClientConnectRequest clientConnectRequest)
        {
            ClientConnectRequest = clientConnectRequest;
            SessionDescription = _proxy.Reconnect(ClientConnectRequest);
            try
            {
                TVMode = SessionDescription.TVSessionInfo.TVMode;
                Channel = SessionDescription.TVSessionInfo.Channel;
            }
            catch { }
            return _sessionDescription;
        }

        /// <summary>
        /// Sets the profile on the server.
        /// </summary>
        /// <param name="newProfile">the new profile to set</param>
        /// <seealso cref="T:ITVStream"/>
        /// <returns>the updated session description</returns>
        public SessionDescription SetProfile(Profile newProfile)
        {
            SessionDescription sd = _proxy.SetProfile(newProfile);
            if (sd != null)
            {
                SessionDescription = sd;
            }
            return sd;
        }

        /// <summary>
        /// Do not call this
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

        #endregion

        #region ITVStream Wrapper

        /// <summary>
        /// Gets the last-reported TVMode from the server
        /// </summary>
        public TVMode TVMode { get; internal set; }

        /// <summary>
        /// Sets the TVMode on the server
        /// </summary>
        /// <param name="mode">TVMode to set to</param>
        public void SetTVMode(TVMode mode)
        {
            if ((_proxy != null) && (mode != this.TVMode))
            {
                _proxy.SetTVMode(mode);
            }
        }

        /// <summary>
        /// Sets the channel on the remote server
        /// </summary>
        /// <param name="ch">channel to set to</param>
        public void SetChannel(Channel ch)
        {
            if (_proxy != null)
            {
                _proxy.SetChannel(ch);
            }
        }

        /// <summary>
        /// Tell the server to go up one channel
        /// </summary>
        public void ChannelUp()
        {
            if (_proxy != null)
            {
                _proxy.ChannelUp();
            }
        }

        /// <summary>
        /// Tell the server to go down one channel
        /// </summary>
        public void ChannelDown()
        {
            if (_proxy != null)
            {
                _proxy.ChannelDown();
            }
        }

        #region IChannelScanProvider Members

        /// <summary>
        /// Begin a channel scan on the server
        /// </summary>
        public void StartChannelScan()
        {
            if (_proxy != null)
            {
                _proxy.StartChannelScan();
            }
        }

        /// <summary>
        /// Cancel any channel scan currently in progress on the server
        /// </summary>
        public void CancelChannelScan()
        {
            if (_proxy != null)
            {
                _proxy.CancelChannelScan();
            }
        }

        /// <summary>
        /// Raised when the server starts a channel scan
        /// </summary>
        public event EventHandler ChannelScanStarted;

        /// <summary>
        /// Raised when the server reports progress on a channel scan
        /// </summary>
        public event EventHandler ChannelScanProgressUpdate;

        /// <summary>
        /// Raised when the channel scan has completed.
        /// </summary>
        public event EventHandler<ChannelScanCompleteEventArgs> ChannelScanComplete;

        /// <summary>
        /// the last reported percentage of server-side channel scan completion.
        /// </summary>
        public int ChannelScanProgress { get; set; }

        internal void FireChannelScanProgressUpdated(int progress)
        {
            if (this.ChannelScanProgress < 0)
            {
                if (ChannelScanStarted != null)
                {
                    ChannelScanStarted.Invoke(this, new EventArgs());
                }
            }

            this.ChannelScanProgress = progress;

            if (ChannelScanProgressUpdate != null)
            {
                ChannelScanProgressUpdate.Invoke(this, new EventArgs());
            }
        }

        internal void FireChannelScanCompleted(ChannelScanCompleteEventArgs e)
        {
            this.ChannelScanProgress = -1;
            if (ChannelScanComplete != null)
            {
                ChannelScanComplete.Invoke(this, e);
            }
        }

        #endregion

        private Channel _channel;
        /// <summary>
        /// Gets the last reported channel from the server
        /// </summary>
        public Channel Channel
        {
            get
            {
                if (this.TVMode != TVMode.Broadcast)
                {
                    return null;
                }
                else
                {
                    return _channel;
                }
            }
            internal set
            {
                _channel = value;
            }
        }

        /// <summary>
        /// Raised when the server indicates the Channel or TVMode has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void FirePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion
    }
}
