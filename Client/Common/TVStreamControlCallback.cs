using System;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.TV;
using System.Diagnostics;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Implements the ITVStreamCallback.
    /// </summary>
    internal class TVStreamControlCallback : ITVStreamCallback
    {
        private TVStreamControl _tvStreamControl;

        /// <summary>
        /// Constructs the callback object
        /// </summary>
        /// <param name="tvStreamControl">TVStreamControl this callback is associated with</param>
        public TVStreamControlCallback(TVStreamControl tvStreamControl)
        {
            _tvStreamControl = tvStreamControl;
        }

        #region ITVStreamCallback Members

        public void ChannelChanged(Channel newChannel)
        {
            _tvStreamControl.Channel = newChannel;
            _tvStreamControl.FirePropertyChanged("Channel");
        }

        /// <summary>
        /// called by the server when the channel scan progress is updated
        /// </summary>
        /// <param name="progress"></param>
        public void ChannelScanProgressUpdate(int progress)
        {
            _tvStreamControl.FireChannelScanProgressUpdated(progress);
        }

        public void ChannelScanComplete(ChannelScanCompleteEventArgs e)
        {
            _tvStreamControl.FireChannelScanCompleted(e);
        }

        public void TVModeChanged(TVMode newMode)
        {
            Debug.WriteLine("Server handed down TVMode: " + newMode.ToString());
            _tvStreamControl.TVMode = newMode;
            _tvStreamControl.FirePropertyChanged("TVMode");
        }

        #endregion
    }
}
