using System;
using System.Net;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Utility to perform a reliable host/IP resolve
    /// </summary>
    /// <author>kdixon</author>
    public static class AddressLookup
    {
        /// <summary>
        /// Get the Host entry for a hostname or IP address
        /// </summary>
        /// <param name="hostOrIP">hostname or IP address</param>
        /// <returns>the IPHostEntry for the specfied host or IP</returns>
        public static IPHostEntry GetHostEntry(string hostOrIP)
        {
            if (string.IsNullOrEmpty(hostOrIP))
            {
                throw new ArgumentNullException("hostOrIP");
            }

            IPHostEntry h = new IPHostEntry();
            
            IPAddress address;
            if (IPAddress.TryParse(hostOrIP, out address))
            {
                h.AddressList = new IPAddress[] { address };
                h.HostName = hostOrIP;
                return h;
            }

            return Dns.GetHostEntry(hostOrIP);
        }

        /// <summary>
        /// Returns the first IP address associated with the given hostname or IP
        /// </summary>
        /// <param name="hostOrIP">hostname or IP address</param>
        /// <returns>the first IP address found</returns>
        public static IPAddress GetHostAddress(string hostOrIP)
        {
            return GetHostEntry(hostOrIP).AddressList[0];
        }
    }
}
