using System;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Used with Media.Client.ServerConfig.GetServerInfoSpecific
    /// Specify a combination of these flags to determine what fields of Media.ServerInfo struct you want filled out.
    /// kdixon 11/20/2008
    /// </summary>
    public static class RequestedServerInfoParams
    {
        /// <summary>
        /// ServerName field
        /// </summary>
        public const int ServerName     = 1;
        /// <summary>
        /// ServerAddress field
        /// </summary>
        public const int ServerAddress  = 2;
        /// <summary>
        /// VersionInfo field
        /// </summary>
        public const int VersionInfo    = 4;
        /// <summary>
        /// ProfileGroups field
        /// </summary>
        public const int ProfileGroups  = 8;
        /// <summary>
        /// StreamSources field
        /// </summary>
        public const int StreamSources  = 16;
        /// <summary>
        /// OriginServers field
        /// </summary>
        public const int OriginServers  = 32;
        /// <summary>
        /// RevisionNumber field
        /// </summary>
        public const int RevisionNumber = 64;
        /// <summary>
        /// All fields
        /// </summary>
        public const int All            = 127;
    }
}
