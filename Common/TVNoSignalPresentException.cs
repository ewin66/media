using System;
using System.Text;

namespace FutureConcepts.Media.TV
{
    /// <summary>
    /// Thrown when there is no signal present on a given TV channel or tuning
    /// </summary>
    public class NoSignalPresentException : System.Exception
    {
        /// <summary>
        /// Creates a new NoSignalPresentException
        /// </summary>
        /// <param name="channelNumber">specify the channel number where there is no signal.</param>
        public NoSignalPresentException(int channelNumber) : base(String.Format("No signal was present on channel {0}.", channelNumber)) { }
        /// <summary>
        /// Creates a new NoSignalPresentException
        /// </summary>
        /// <param name="channel">specify the channel where there is no signal.</param>
        public NoSignalPresentException(Channel channel) : base(String.Format("No signal was present on channel {0}.", channel.ToString())) { }
        /// <summary>
        /// Creates a new NoSignalPresentException
        /// </summary>
        /// <param name="msg">specify the message associated with this exception</param>
        public NoSignalPresentException(string msg) : base(msg) { }
        /// <summary>
        /// Creates a new NoSignalPresentException
        /// </summary>
        /// <param name="msg">specify a message assocaited with this exception. Use {0} in string to insert channel number.</param>
        /// <param name="channel">channel with no signal</param>
        public NoSignalPresentException(string msg, Channel channel) : base(String.Format(msg, channel.ToString())) { }

    }
}
