using System.Collections.Generic;

namespace FutureConcepts.Media.TV
{
    /// <summary>
    /// This is a common interface that should be implemented by any class that provides virtual channel information
    /// Kevin Dixon
    /// 01/30/2009
    /// </summary>
    public interface IVirtualChannelProvider
    {
        /// <summary>
        /// Retreive the list of virtual channels from this provider.
        /// </summary>
        /// <returns>A list of 0 or more channels</returns>
        List<Channel> GetVirtualChannels();
    }
}
