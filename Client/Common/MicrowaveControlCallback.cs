using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.Contract;
using System.Diagnostics;

namespace FutureConcepts.Media.Client
{
    internal class MicrowaveControlCallback : IMicrowaveControlCallback
    {
        private MicrowaveControl parent;

        public MicrowaveControlCallback(MicrowaveControl parent)
        {
            this.parent = parent;
        }

        #region IMicrowaveControlCallback Members

        public void FrequencyChanged(int freq)
        {
            Debug.WriteLine("  MicrowaveControlCallback.FrequencyChanged " + freq);
            parent.CurrentFrequency = freq;
        }

        public void SignalStrengthChanged(int strength)
        {
            parent.SignalStrength = strength;
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
