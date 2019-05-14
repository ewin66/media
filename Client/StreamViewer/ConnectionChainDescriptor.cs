using System;
using System.Collections.Generic;
using System.Text;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// Describes a chain of servers to get to a specified source
    /// </summary>
    /// <remarks>
    /// kdixon 05/11/2009
    /// </remarks>
    public class ConnectionChainDescriptor
    {
        /// <summary>
        /// Creates a new, blank connection chain descriptor
        /// </summary>
        public ConnectionChainDescriptor()
        {
            this.ServerChain = new List<ServerInfo>();
        }

        /// <summary>
        /// Creates a 1-hop chain descriptor
        /// </summary>
        /// <param name="server">server endpoint</param>
        /// <param name="source">source on the server to connec to</param>
        public ConnectionChainDescriptor(ServerInfo server, StreamSourceInfo source)
            : this()
        {
            this.ServerChain.Add(server);
            this.Source = source;
        }

        /// <summary>
        /// Creates a new descriptor
        /// </summary>
        /// <param name="chain">chain of N servers to follow. 0th server is the HostServer, and the N-1th server is the OriginServer</param>
        /// <param name="source">source on OriginServer to ultimately connect to</param>
        public ConnectionChainDescriptor(List<ServerInfo> chain, StreamSourceInfo source)
        {
            this.ServerChain = chain;
            this.Source = source;
        }

        /// <summary>
        /// The list of servers to traverse to reach the indicated source. The server at the end of the
        /// list has the source specified. The servers prior to that indicate the hops needed.
        /// </summary>
        public List<ServerInfo> ServerChain { get; set; }

        /// <summary>
        /// Returns the server that actually has the Source
        /// </summary>
        public ServerInfo OriginServer
        {
            get
            {
                if (ServerChain.Count > 0)
                {
                    return ServerChain[ServerChain.Count - 1];
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the server that this client should connect to
        /// </summary>
        public ServerInfo HostServer
        {
            get
            {
                if (ServerChain.Count > 0)
                {
                    return ServerChain[0];
                }
                return null;
            }
        }

        /// <summary>
        /// The source requested.
        /// </summary>
        public StreamSourceInfo Source { get; set; }

        /// <summary>
        /// Returns the address path to provide to the HostServer
        /// </summary>
        /// <returns></returns>
        public string GetSourceName()
        {
            if ((ServerChain.Count == 0) || (Source == null))
            {
                return string.Empty;
            }

            StringBuilder b = new StringBuilder();
            for (int i = 1; i < ServerChain.Count; i++)
            {
                b.Append(ServerChain[i].ServerAddress);
                b.Append(":");
            }
            b.Append(Source.SourceName);

            return b.ToString();
        }

        /// <summary>
        /// Returns a human-readable description of the descriptor
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            try
            {
                StringBuilder desc = new StringBuilder();

                desc.Append(Source.Description);

                if (ServerChain.Count > 0)
                {
                    desc.Append(Environment.NewLine);
                    desc.Append(OriginServer.ServerName);
                    if (ServerChain.Count > 1)
                    {
                        desc.Append(" (via ");
                        desc.Append(HostServer.ServerName);
                        if (ServerChain.Count > 2)
                        {
                            desc.Append(" (and ");
                            desc.Append((ServerChain.Count - 2).ToString());
                            desc.Append(" more)");
                        }
                        desc.Append(")");
                    }
                }
                
                return desc.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
