using System;
using System.Collections.Generic;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Provides a mapping between SinkProtocolType and the URI prefixes associated with them
    /// </summary>
    public static class SinkProtocolMapper
    {
        private static Dictionary<SinkProtocolType, string> _mappings;
        private static Dictionary<SinkProtocolType, string> Mappings
        {
            get
            {
                if (_mappings == null)
                {
                    _mappings = new Dictionary<SinkProtocolType, string>();
                    _mappings.Add(SinkProtocolType.HTTP, "http://");
                    _mappings.Add(SinkProtocolType.LTSF_DGRAM, "ltsf:dgram://");
                    _mappings.Add(SinkProtocolType.LTSF_TCP, "ltsf://");
                    _mappings.Add(SinkProtocolType.RTP, "rtp://");
                    _mappings.Add(SinkProtocolType.RTSP, "rtsp://");
                }
                return _mappings;
            }
        }

        /// <summary>
        /// Gets the uri prefix in format "uri://" for the specified protocol
        /// </summary>
        /// <param name="protocol">protocol to get prefix for</param>
        /// <returns>Returns the appropriate prefix string, or ltsf:// if the protocol is unrecognized</returns>
        public static string GetPrefix(SinkProtocolType protocol)
        {
            if (!Mappings.ContainsKey(protocol))
            {
                return "ltsf://";
            }
            return Mappings[protocol];
        }

        /// <summary>
        /// Gets the appropriate protocol type identifier 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static SinkProtocolType GetProtocol(string uri)
        {
            int pos = uri.IndexOf("://");
            if (pos > -1)
            {
                string prefix = uri.Substring(0, pos + 3).ToLowerInvariant();
                foreach (KeyValuePair<SinkProtocolType, string> kvp in Mappings)
                {
                    if (kvp.Value.Equals(prefix))
                    {
                        return kvp.Key;
                    }
                }
            }
            return SinkProtocolType.Default;
        }
    }
}
