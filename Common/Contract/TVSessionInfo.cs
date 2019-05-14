using System;
using FutureConcepts.Media.TV;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Provides additional information about a server/client session when the <see cref="T:SourceType"/>
    /// is a TV type. <seealso cref="T:SessionDescription"/>
    /// </summary>
    [Serializable()]
    public class TVSessionInfo
    {
        private Channel _channel;
        /// <summary>
        /// The currently tuned channel
        /// </summary>
        public Channel Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value;
            }
        }

        private TVMode _tvMode;
        /// <summary>
        /// The currently selected TV Mode
        /// </summary>
        public TVMode TVMode
        {
            get
            {
                return _tvMode;
            }
            set
            {
                _tvMode = value;
            }
        }

        private bool _channelScanInProgress;
        /// <summary>
        /// If true, then the client should not connect their DirectShow graph until the server
        /// indicates the channel scan is complete.
        /// </summary>
        public bool ChannelScanInProgress
        {
            get
            {
                return _channelScanInProgress;
            }
            set
            {
                _channelScanInProgress = value;
            }
        }
    }
}
