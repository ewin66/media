using System;
using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Service contract for the MicrowaveControlService2
    /// </summary>
    /// <remarks>, derived from IMicrowaveControl</remarks>
    /// <author>kdixon 02/01/2011</author>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMicrowaveControl2Callback))]
    public interface IMicrowaveControl2 : IPresetProvider, IKeepAlive
    {
        /// <summary>
        /// Call this method to establish a connection
        /// </summary>
        /// <param name="request">A properly filled out <see cref="T:ClientConnectRequest"/> structure, indicating which SourceName to connect to.</param>
        [OperationContract]
        void Open(ClientConnectRequest request);

        /// <summary>
        /// Call this method to receive all of the latest parameters.
        /// Use sparingly
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ForceUpdate();

        /// <summary>
        /// Sets the tuning
        /// </summary>
        /// <param name="tuning">tuning the receiever should adjust to</param>
        [OperationContract(IsOneWay = true)]
        void SetTuning(MicrowaveTuning tuning);

        /// <summary>
        /// Starts a sweep for frequencies
        /// </summary>
        /// <param name="settings">Any specific information that should be tried, or null to accept all auto</param>
        /// <param name="start">start frequency in Hz</param>
        /// <param name="end">end frequency in Hz</param>
        /// <param name="threshold">
        /// for analog systems, a minimum percentage of signal strength,
        /// or a Link Quality rating for digital systems
        /// </param>
        [OperationContract(IsOneWay = true)]
        void StartSweep(MicrowaveTuning settings, UInt64 start, UInt64 end, int threshold);

        /// <summary>
        /// Cancels an current frequency sweep in progress
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void CancelSweep();
    }
}
