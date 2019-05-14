using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutureConcepts.Media.Contract;
using System.Threading;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Client class for querying the ServerStatusService
    /// </summary>
    public class ServerStatus : BaseClient<IServerStatus>
    {
        /// <summary>
        /// never call this
        /// </summary>
        private ServerStatus() : base(null) { }

        private ServerStatus(string serverHostOrIP)
            : base(serverHostOrIP)
        {
            Connect();
        }

        #region BaseClient Properties

        /// <summary>
        /// Returns the port to connect to
        /// </summary>
        protected override string Port
        {
            get { return "8097"; }
        }

        /// <summary>
        /// Returns the endpoint to connect to
        /// </summary>
        protected override string Endpoint
        {
            get { return "ServerStatusService"; }
        }

        /// <summary>
        /// Disables the KeepAlive
        /// </summary>
        protected override int KeepAliveInterval
        {
            get { return 0; }
        }

        /// <summary>
        /// we do not use a callback
        /// </summary>
        /// <returns>null</returns>
        protected override object CreateCallback()
        {
            return null;
        }

        #endregion

        #region IServerStatus Members

        /// <summary>
        /// Query a server for its overall status
        /// </summary>
        /// <param name="serverHostOrIP">server hostname or IP address</param>
        /// <returns>The status of the server, or null if an error occurred</returns>
        public static ServerStatusProperties Query(string serverHostOrIP)
        {
            return ServerStatus.Query(serverHostOrIP, null);
        }

        /// <summary>
        /// Query a server for a specific source's status
        /// </summary>
        /// <param name="serverHostOrIP">server hostname or IP address.</param>
        /// <param name="sourceName">The source name to get information about. Use the <see cref="T:ServerConfig"/> class to query a server for its configured sources.</param>
        /// <returns>The status of the source, or null if an error occurred.</returns>
        /// <seealso cref="T:ServerConfig"/>
        /// <seealso cref="T:StreamSourceInfo"/>
        public static ServerStatusProperties Query(string serverHostOrIP, string sourceName)
        {
            try
            {
                using (ServerStatus s = new ServerStatus(serverHostOrIP))
                {
                    return s.QueryServer(sourceName);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                return null;
            }
        }

        private ServerStatusProperties QueryServer(string sourceName)
        {
            return Proxy.Query(sourceName);
        }

        #endregion
    }
}
