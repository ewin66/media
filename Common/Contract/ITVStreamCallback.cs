using System;
using System.ServiceModel;
using FutureConcepts.Media.TV;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// A contract that should be used to report channel scan progress to clients
    /// kdixon
    /// </summary>
    public interface ITVStreamCallback
    {
        /// <summary>
        /// Raised when the channel has changed
        /// </summary>
        /// <param name="newChannel">the newly tuned channel</param>
        [OperationContract(IsOneWay = true)]
        void ChannelChanged(Channel newChannel);

        /// <summary>
        /// Raised when the progress of a channel scan has been updated
        /// </summary>
        /// <param name="progress">progress, as a percentage, [0,100]</param>
        [OperationContract(IsOneWay = true)]
        void ChannelScanProgressUpdate(int progress);

        /// <summary>
        /// Raised when a channel scan has completed.
        /// </summary>
        /// <param name="e">information concerning the outcome of the channel scan</param>
        [OperationContract(IsOneWay = true)]
        void ChannelScanComplete(ChannelScanCompleteEventArgs e);

        /// <summary>
        /// Raised when the <see cref="T:TVMode"/> has changed.
        /// </summary>
        /// <param name="newTVMode">the new <see cref="T:TVMode"/></param>
        [OperationContract(IsOneWay = true)]
        void TVModeChanged(TVMode newTVMode);
    }
}
