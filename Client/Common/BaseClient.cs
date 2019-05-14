using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Timers;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client
{
    //TODO this is being superceeded by FutureConcepts.Tools.WCF
    /// <summary>
    /// This class provides base common functionality for WCF clients
    /// </summary>
    /// <remarks>
    /// Factored out from BasePeripheralControl.
    /// </remarks>
    /// <author>kdixon 09/24/2009</author>
    /// <typeparam name="TContract">The WCF ServiceContract to use</typeparam>
    [Obsolete("Use FutureConcepts.Tools.WCF")]
    public abstract class BaseClient<TContract> : IDisposable
    {
        /// <param name="serverHostOrIP">server hostname or IP that is hosting the service</param>
        public BaseClient(string serverHostOrIP)
        {
            IPHostEntry e = AddressLookup.GetHostEntry(serverHostOrIP);
            this.ServerAddress = e.AddressList[0].ToString();
        }

        /// <summary>
        /// Current instance of the Channel Factory
        /// </summary>
        protected ChannelFactory<TContract> Factory { get; set; }

        private TContract _proxy = default(TContract);
        /// <summary>
        /// Current instance of the proxy
        /// </summary>
        protected virtual TContract Proxy
        {
            get
            {
                return _proxy;
            }
            set
            {
                _proxy = value;
            }
        }

        /// <summary>
        /// Returns the ICommunicationObject interface for the proxy
        /// </summary>
        private ICommunicationObject ProxyInterface
        {
            get
            {
                return Proxy as ICommunicationObject;
            }
        }

        /// <summary>
        /// Returns true if the proxy and factory are ready to be used and in a good state
        /// </summary>
        protected bool ProxyReady
        {
            get
            {
                return ((Factory != null) && (Factory.State == CommunicationState.Opened) &&
                        (_proxy != null) && (((ICommunicationObject)_proxy).State == CommunicationState.Opened));
            }
        }

        /// <summary>
        /// Returns the protocol prefix to use. This is everything before the "://" in the URL.
        /// </summary>
        protected string ProtocolPrefix { get; set; }

        /// <summary>
        /// Returns the ServerAddress to use. Auto-populated by base constructor
        /// </summary>
        protected string ServerAddress { get; set; }

        /// <summary>
        /// Returns the port number to use
        /// </summary>
        protected abstract string Port { get; }

        /// <summary>
        /// Name of the end point, comes after the final "/" in the URL
        /// </summary>
        protected abstract string Endpoint { get; }

        /// <summary>
        /// The amount of time in milliseconds between keep-alives. Set to less than 0 to disable.
        /// </summary>
        protected abstract int KeepAliveInterval { get; }

        /// <summary>
        /// Called when the callback class should be instantiated
        /// </summary>
        /// <returns>Return the instance of the callback class. Return null if the contract does not use a callback</returns>
        protected abstract object CreateCallback();

        /// <summary>
        /// The keep alive timer.
        /// </summary>
        private Timer keepAliveTimer;

        /// <summary>
        /// Returns the address that should be connected to. Default implementation combines the properties
        /// ProtocolPrefix, ServerAddress, Port, and Endpoint to generate it
        /// </summary>
        /// <returns>the address to connect to in string form</returns>
        protected virtual string CreateAddress()
        {
            return ProtocolPrefix + "://" + ServerAddress + ":" + Port + "/" + Endpoint;
        }

        /// <summary>
        /// Called when the binding should be instantiated. Also, set the ProtocolPrefix property.
        /// </summary>
        /// <returns>the binding to use</returns>
        protected virtual Binding CreateBinding()
        {
            ProtocolPrefix = "net.tcp";
            NetTcpBinding b = new NetTcpBinding(SecurityMode.None, true);
            b.CloseTimeout = TimeSpan.FromSeconds(5);
            return b;
        }

        /// <summary>
        /// Call this method when any sub-class is ready to create and open the factory and create a channel
        /// </summary>
        protected virtual void Connect()
        {
            object callback = CreateCallback();
            if (callback == null)
            {
                Factory = new ChannelFactory<TContract>(CreateBinding(), CreateAddress());
            }
            else
            {
                Factory = new DuplexChannelFactory<TContract>(callback, CreateBinding(), CreateAddress());
            }
            Factory.Opened += new EventHandler(Factory_Opened);
            Factory.Closed += new EventHandler(Factory_Closed);
            Factory.Faulted += new EventHandler(Factory_Faulted);
            Factory.Open();

            Proxy = Factory.CreateChannel();
            ProxyInterface.Faulted += new EventHandler(Proxy_Faulted);
            ProxyInterface.Closed += new EventHandler(Proxy_Faulted);
            

            if (KeepAliveInterval > 0)
            {
                keepAliveTimer = new System.Timers.Timer();
                keepAliveTimer.Interval = KeepAliveInterval;
                keepAliveTimer.Elapsed += new ElapsedEventHandler(keepAliveTimer_Elapsed);
                keepAliveTimer.Start();
            }
        }

        private void keepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                keepAliveTimer.Stop();
                keepAliveTimer.Interval = KeepAliveInterval; //update keep alive interval
                this.OnKeepAlive();
                keepAliveTimer.Start();
            }
            catch (Exception ex)
            {
                if (keepAliveTimer != null)
                {
                    keepAliveTimer.Stop();
                }
                ErrorLogger.DumpToDebug(ex);
                this.Close(); //clean up and raise events to clients of the class
            }
        }

        private void Proxy_Faulted(object sender, EventArgs e)
        {
            OnProxyFaulted();
            Close();
        }

        private void Factory_Opened(object sender, EventArgs e)
        {
            OnFactoryOpened();
            FireOpened();
        }

        /// <summary>
        /// To be overriden. Called before the Opened event is fired
        /// </summary>
        protected virtual void OnFactoryOpened()
        {
            //do nothing
        }

        private void Factory_Faulted(object sender, EventArgs e)
        {
            OnFactoryFaulted();
            Close();
        }

        /// <summary>
        /// To be overridden. Called when the proxy faults, and before the Closed event is fired
        /// </summary>
        protected virtual void OnProxyFaulted()
        {
            //do nothing
        }

        /// <summary>
        /// To be overriden. Called when the factory faults, and before the Closed event is fired
        /// </summary>
        protected virtual void OnFactoryFaulted()
        {
            //do nothing
        }

        private void Factory_Closed(object sender, EventArgs e)
        {
            FireClosed();
        }

        /// <summary>
        /// Raised when the client control is fully opened
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Raised when the client control has been closed or faulted
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Closes the factory or aborts it as needed. Ensures that the Closed event is raised.
        /// </summary>
        public virtual void Close()
        {
            if (keepAliveTimer != null)
            {
                keepAliveTimer.Enabled = false;
                keepAliveTimer.Dispose();
                keepAliveTimer = null;
            }

            //close Proxy / Channel
            if (_proxy != null)
            {
                ProxyInterface.Faulted -= new EventHandler(Proxy_Faulted);
                ProxyInterface.Closed -= new EventHandler(Proxy_Faulted);

                try
                {
                    if (ProxyInterface.State == CommunicationState.Opened)
                    {
                        ProxyInterface.Close();
                    }
                    else if ((ProxyInterface.State != CommunicationState.Closing) &&
                             (ProxyInterface.State != CommunicationState.Closed))
                    {
                        ProxyInterface.Abort();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                }

                Proxy = default(TContract);
            }

            //close Factory
            if (Factory != null)
            {
                try
                {
                    if (Factory.State == CommunicationState.Opened)
                    {
                        Factory.Close();
                    }
                    else if ((Factory.State != CommunicationState.Closing) &&
                             (Factory.State != CommunicationState.Closed))
                    {
                        Factory.Abort();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                }

                Factory.Opened -= new EventHandler(Factory_Opened);
                Factory.Closed -= new EventHandler(Factory_Closed);
                Factory.Faulted -= new EventHandler(Factory_Closed);
                Factory = null;
            }
            else
            {
                FireClosed();
            }
        }

        /// <summary>
        /// Raises the Opened event
        /// </summary>
        protected void FireOpened()
        {
            if (Opened != null)
            {
                Opened(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Closed event
        /// </summary>
        protected void FireClosed()
        {
            if (Closed != null)
            {
                Closed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Aborts the proxy factory in the case that it is faulted, or otherwise worthless
        /// </summary>
        public virtual void Abort()
        {
            try
            {
                if (keepAliveTimer != null)
                {
                    keepAliveTimer.Enabled = false;
                    keepAliveTimer.Dispose();
                    keepAliveTimer = null;
                }

                if (Factory != null)
                {
                    Factory.Abort();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Called when the keep-alive should be issued, as defined by KeepAliveInterval.
        /// </summary>
        protected virtual void OnKeepAlive()
        {
            if (ProxyReady)
            {
                if (_proxy is IKeepAlive)
                {
                    ((IKeepAlive)_proxy).KeepAlive();
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Detaches all event listeners and closes the session
        /// </summary>
        public virtual void Dispose()
        {
            Opened = null;
            Closed = null;

            Close();
        }

        #endregion
    }
}
