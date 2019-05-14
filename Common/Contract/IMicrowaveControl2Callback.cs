using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Callback interface for clients of the MicrowaveControlService2
    /// </summary>
    /// <author>kdixon 02/01/2011</author>
    public interface IMicrowaveControl2Callback
    {
        /// <summary>
        /// Raised when the capabilities of a microwave receiver are announced or have changed.
        /// </summary>
        /// <param name="capabilities">data regarding the MRX's capabilities</param>
        [OperationContract(IsOneWay = true)]
        void CapabilitiesChanged(MicrowaveCapabilities capabilities);

        /// <summary>
        /// Raised when the any tuning parameter on the server has changed
        /// </summary>
        /// <param name="tuning">tuning settings</param>
        [OperationContract(IsOneWay = true)]
        void TuningChanged(MicrowaveTuning tuning);

        /// <summary>
        /// Raised when the signal strength has changed
        /// </summary>
        /// <param name="linkQuality">the stats for the link quality</param>
        [OperationContract(IsOneWay = true)]
        void LinkQualityChanged(MicrowaveLinkQuality linkQuality);

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
