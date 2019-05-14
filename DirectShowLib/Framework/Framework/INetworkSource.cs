using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Possible statuses that a Network Source would have
    /// </summary>
    /// <author>kdixon 02/23/2009</author>
    public enum NetworkSourceStatus
    {
        /// <summary>
        /// The source is doing nothing
        /// </summary>
        Inactive,
        /// <summary>
        /// In the process of establishing a connection
        /// </summary>
        Connecting,
        /// <summary>
        /// The connection is established, and is buffering data
        /// </summary>
        Buffering,
        /// <summary>
        /// The connection is established and data is flowing
        /// </summary>
        Connected,
        /// <summary>
        /// The connection is being torn down
        /// </summary>
        Disconnecting,
        /// <summary>
        /// An error occurred on the connection
        /// </summary>
        Faulted
    };

    /// <summary>
    /// Represents a source subgraph that derives its data from a network connection
    /// </summary>
    /// <author>kdixon 02/23/2009</author>
    public interface INetworkSource : INotifyPropertyChanged, ISourceSubGraph
    {
        /// <summary>
        /// Call to initate a connection
        /// </summary>
        void Connect();

        /// <summary>
        /// Call to terminate a connection
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Fetches any error that may have occurred relating to a change in status
        /// </summary>
        Exception StatusError { get; }

        /// <summary>
        /// Retreives the current status of the network connection
        /// </summary>
        NetworkSourceStatus Status { get; }

        /// <summary>
        /// Current received bitrate, in kbps
        /// </summary>
        int Bitrate { get; }
    }
}
