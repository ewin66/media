using System;
using System.ServiceModel;
using FutureConcepts.Media.TV;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// ServiceContract for the WCF TVStreamService. Adds a callback contract, and additional methods for television.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ITVStreamCallback))]
    public interface ITVStream : IStream
    {
        /// <summary>
        /// Adjusts the <see cref="T:TVMode"/> of the stream.
        /// </summary>
        /// <param name="tvMode"><see cref="T:TVMode"/> to set</param>
        [OperationContract(IsOneWay = true)]
        void SetTVMode(TVMode tvMode);

        /// <summary>
        /// Changes to an explicit channel.
        /// </summary>
        /// <param name="ch">channel to set</param>
        [OperationContract(IsOneWay = true)]
        void SetChannel(Channel ch);

        /// <summary>
        /// Increments the current channel to the next-known-channel.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ChannelUp();

        /// <summary>
        /// Decrements the current channel to the previous-known-channel
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ChannelDown();

        /// <summary>
        /// Initiate a scan for channels on the server.
        /// </summary>
        [OperationContract]
        void StartChannelScan();

        /// <summary>
        /// Abort any current channel scan on the server.
        /// </summary>
        [OperationContract]
        void CancelChannelScan();
    }
}
