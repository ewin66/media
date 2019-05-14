using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;
using System.Threading;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Provides an interface to the ServerConfigService
    /// </summary>
    /// <author>kdixon 02/10/2010</author>
    public class ServerConfig : BaseClient<IServerConfig>, IServerConfig
    {
        //never call this
        private ServerConfig() : base(null) { }

        /// <summary>
        /// Creates a new instance of the class, pointing to a specific server
        /// </summary>
        /// <param name="serverHostOrIP">hostname or IP address</param>
        public ServerConfig(string serverHostOrIP)
            : base(serverHostOrIP)
        {
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw ex;
            }
        }

        #region BaseClient implementation

        /// <summary>
        /// ServerConfig service port
        /// </summary>
        protected override string Port
        {
            get
            {
                return "8099";
            }
        }

        /// <summary>
        /// Fetches the appropriate endpoint name
        /// </summary>
        protected override string Endpoint
        {
            get
            {
                return "ServerConfig";
            }
        }

        /// <summary>
        /// Disable keep alives
        /// </summary>
        protected override int KeepAliveInterval
        {
            get { return -1; }
        }

        /// <summary>
        /// There is no callback
        /// </summary>
        /// <returns>null</returns>
        protected override object CreateCallback()
        {
            return null;
        }

        #endregion

        #region IServerConfig Members

        /// <summary>
        /// Fetches all server configuration information for this server.
        /// </summary>
        /// <returns>a fully populated ServerInfo struct</returns>
        public ServerInfo GetServerInfo()
        {
            return Proxy.GetServerInfo();
        }

        /// <summary>
        /// Request that only specific server information be returned.
        /// </summary>
        /// <param name="requestedServerInfoParams">
        /// Bitfield. All ServerInfo will be filtered by this.
        /// See <see cref="T:FutureConcepts.Media.Contract.RequestedServerInfoParams">RequestedServerInfoParams</see>
        /// for valid values
        /// </param>
        /// <param name="requestedStreamSourceInfoParams">
        /// Bitfield. All StreamSourceInfo will be filtered by this.
        /// See <see cref="T:FutureConcepts.Media.Contract.RequestedStreamSourceInfoParams">RequestedStreamSourceInfoParams</see>
        /// for valid values.
        /// </param>
        /// <returns>the ServerInfo for this server.</returns>
        public ServerInfo GetServerInfoSpecific(int requestedServerInfoParams, int requestedStreamSourceInfoParams)
        {
            return Proxy.GetServerInfoSpecific(requestedServerInfoParams, requestedStreamSourceInfoParams);
        }

        /// <summary>
        /// Updates the StreamSources on the server
        /// </summary>
        /// <param name="streamSources"><see cref="T:StreamSources">StreamSources</see> object to use on the server</param>
        public void PutStreamSources(StreamSources streamSources)
        {
            Proxy.PutStreamSources(streamSources);
        }

        /// <summary>
        /// Adds or overwrites a <see cref="T:ProfileGroup">ProfileGroup</see> on the server
        /// </summary>
        /// <param name="profileGroup">the new or updated <see cref="T:ProfileGroup">ProfileGroup</see></param>
        public void PutProfileGroup(ProfileGroup profileGroup)
        {
            Proxy.PutProfileGroup(profileGroup);
        }

        #endregion
    }
}
