using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// A generic interface for classes that provide some mechanism of scanning for "channels"
    /// </summary>
    public interface IChannelScanProvider
    {
        /// <summary>
        /// Begins a channel scan.
        /// </summary>
        void StartChannelScan();

        /// <summary>
        /// Cancels any channel scan in progress
        /// </summary>
        void CancelChannelScan();

        /// <summary>
        /// Raised when a channel scan has started
        /// </summary>
        event EventHandler ChannelScanStarted;

        /// <summary>
        /// Raised when there has been a change in the progress of the scan
        /// </summary>
        event EventHandler ChannelScanProgressUpdate;

        /// <summary>
        /// Raised when a channel scan is complete.
        /// </summary>
        event EventHandler<ChannelScanCompleteEventArgs> ChannelScanComplete;

        /// <summary>
        /// Retreives the current progress of a channel scan.
        /// </summary>
        int ChannelScanProgress { get; }
    }

    /// <summary>
    /// Describes the outcome of a Channel Scan once it has been completed.
    /// </summary>
    [Serializable]
    public class ChannelScanCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ChannelScanCompleteEventArgs()
        {
            this.ChannelsFound = 0;
            this.Cancelled = false;
        }

        /// <summary>
        /// Creates a new instance, noting the number of channels found.
        /// </summary>
        /// <param name="channelsFound">Number of channels found.</param>
        public ChannelScanCompleteEventArgs(int channelsFound)
        {
            this.ChannelsFound = channelsFound;
            this.Cancelled = false;
        }

        /// <summary>
        /// Creates a new instance, in which the scan was cancelled.
        /// </summary>
        /// <param name="cancelled">True if the scan was cancelled, false if not.</param>
        public ChannelScanCompleteEventArgs(bool cancelled)
        {
            this.Cancelled = cancelled;
            this.ChannelsFound = 0;
        }

        private bool _cancelled;
        /// <summary>
        /// True if the channel scan is complete because it was cancelled.
        /// </summary>
        public bool Cancelled
        {
            get
            {
                return _cancelled;
            }
            set
            {
                _cancelled = value;
            }
        }

        private int _channelsFound;
        /// <summary>
        /// The number of channels found
        /// </summary>
        public int ChannelsFound
        {
            get
            {
                return _channelsFound;
            }
            set
            {
                _channelsFound = value;
            }
        }
    }
}
