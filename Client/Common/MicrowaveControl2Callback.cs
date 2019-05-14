using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.Contract;
using System.Diagnostics;

namespace FutureConcepts.Media.Client
{
    internal class MicrowaveControl2Callback : IMicrowaveControl2Callback
    {
        private MicrowaveControl2 parent;

        public MicrowaveControl2Callback(MicrowaveControl2 parent)
        {
            this.parent = parent;
        }

        #region IMicrowaveControl2Callback Members

        public void CapabilitiesChanged(MicrowaveCapabilities capabilities)
        {
            Debug.WriteLine("  MicrowaveControl2Callback.CapabilitiesChanged");
            parent.Capabilities = capabilities;
        }

        public void TuningChanged(MicrowaveTuning tuning)
        {
            Debug.WriteLine("  MicrowaveControl2Callback.FrequencyChanged");
            parent.CurrentTuning = tuning;
        }

        public void LinkQualityChanged(MicrowaveLinkQuality linkQuality)
        {
            Debug.WriteLine("  MicrowaveControl2Callback.LinkQualityChanged");
            parent.LinkQuality = linkQuality;
        }

        public void SavedPresetsChanged(UserPresetStore presets)
        {
            parent.FrequencyPresets = presets;
        }

        #endregion

        #region IChannelScanCallback Members

        int lastProgress = -1;

        public void ChannelScanProgressUpdate(int progress)
        {
            if (lastProgress == -1)
            {
                parent.FireChannelScanStarted();
            }
            parent.FireChannelScanProgressUpdate(progress);
        }

        public void ChannelScanComplete(ChannelScanCompleteEventArgs e)
        {
            parent.FireChannelScanComplete(e);
        }

        #endregion
    }
}
