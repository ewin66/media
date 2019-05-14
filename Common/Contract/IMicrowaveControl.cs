using System;
using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Service contract for the MicrowaveControlService
    /// kdixon 02/09/2009
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMicrowaveControlCallback))]
    public interface IMicrowaveControl : IPresetProvider, IKeepAlive
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
        /// Sets the tuned frequency
        /// </summary>
        /// <param name="freq">frequency in MHz</param>
        [OperationContract(IsOneWay = true)]
        void SetFrequency(int freq);

        /// <summary>
        /// Starts a sweep for frequencies
        /// </summary>
        /// <param name="start">start frequency in MHz</param>
        /// <param name="end">end frequency in MHz</param>
        /// <param name="threshold">minimum percentage of signal strength</param>
        [OperationContract(IsOneWay = true)]
        void StartSweep(int start, int end, int threshold);

        /// <summary>
        /// Cancels an current frequency sweep in progress
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void CancelSweep();
    }
}
