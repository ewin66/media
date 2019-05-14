using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// An Exception that indicates the Server Graph needs to be rebuilt to support new settings
    /// </summary>
    public class ServerGraphRebuildException : Exception
    {
        /// <summary>
        /// Creates a ServerGraphRebuildException with a generic message
        /// </summary>
        public ServerGraphRebuildException()
            : base("Server graph needs to be rebuilt to support new settings")
        {
        }

        /// <summary>
        /// Creates a ServerGraphRebuildException with a specific message
        /// </summary>
        /// <param name="msg">Generally should be the reason the graph needs to be reconstructed</param>
        public ServerGraphRebuildException(string msg)
            : base(msg)
        {
        }
    }
}
