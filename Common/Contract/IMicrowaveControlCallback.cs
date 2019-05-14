using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Callback interface for clients of the MicrowaveControlService
    /// kdixon 02/09/2009
    /// </summary>
    public interface IMicrowaveControlCallback
    {
        /// <summary>
        /// Raised when the tuned frequency on the server has changed
        /// </summary>
        /// <param name="freq">new frequency, in MHz</param>
        [OperationContract(IsOneWay = true)]
        void FrequencyChanged(int freq);

        /// <summary>
        /// Raised when the signal strength has changed
        /// </summary>
        /// <param name="strength">New strength value in the range [0,100]</param>
        [OperationContract(IsOneWay = true)]
        void SignalStrengthChanged(int strength);

        /// <summary>
        /// Raised when the presets saved on the server have changed
        /// </summary>
        /// <param name="presets">The new collection of presets the server knows about</param>
        [OperationContract(IsOneWay = true)]
        void SavedPresetsChanged(UserPresetStore presets);

        /// <summary>
        /// Raised when the server has updated the amount of progress towards a channel scan
        /// </summary>
        /// <param name="progress">Percentage of progress in the range [0,100]</param>
        [OperationContract(IsOneWay = true)]
        void ChannelScanProgressUpdate(int progress);

        /// <summary>
        /// Raised when a channel scan has completed.
        /// </summary>
        /// <param name="e">Additional information about the channel scan</param>
        [OperationContract(IsOneWay = true)]
        void ChannelScanComplete(ChannelScanCompleteEventArgs e);
    }
}
