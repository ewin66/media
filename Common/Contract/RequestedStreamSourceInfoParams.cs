using System;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Used with Media.Client.ServerConfig.GetServerInfoSpecific
    /// Specify a combination of these flags to determine what fields of Media.StreamSourceInfo struct you want filled out.
    /// kdixon 11/20/2008
    /// </summary>
    public static class RequestedStreamSourceInfoParams
    {
        /// <summary>
        /// SourceName
        /// </summary>
        public const int SourceName                 = 1;
        /// <summary>
        /// SourceType
        /// </summary>
        public const int SourceType                 = 2;
        /// <summary>
        /// LiveSource
        /// </summary>
        public const int LiveSource                 = 4;
        /// <summary>
        /// Description
        /// </summary>
        public const int Description                = 8;
        /// <summary>
        /// MaxRecordingChunkMinutes
        /// </summary>
        public const int MaxRecordingChunkMinutes   = 16;
        /// <summary>
        /// SyncToleranceMilliseconds
        /// </summary>
        public const int SyncToleranceMilliseconds  = 32;
        /// <summary>
        /// MaxQueueDuration
        /// </summary>
        public const int MaxQueueDuration           = 64;
        /// <summary>
        /// MaxClients
        /// </summary>
        public const int MaxClients                 = 128;
        /// <summary>
        /// URLs
        /// </summary>
        public const int URLs                       = 256;
        /// <summary>
        /// LogicalGroupSourceNames
        /// </summary>
        public const int LogicalGroupSourceNames    = 512;
        /// <summary>
        /// ProfileGroupNames
        /// </summary>
        public const int ProfileGroupNames          = 1024;
        /// <summary>
        /// DeviceAddress
        /// </summary>
        public const int DeviceAddress              = 2048;
        /// <summary>
        /// CameraControl
        /// </summary>
        public const int CameraControl              = 4096;
        /// <summary>
        /// TVTuner
        /// </summary>
        public const int TVTuner                    = 8192;
        /// <summary>
        /// MicrowaveControl
        /// </summary>
        public const int MicrowaveControl           = 16384;
        /// <summary>
        /// Hidden
        /// </summary>
        public const int Hidden                     = 32768;
        /// <summary>
        /// All fields
        /// </summary>
        public const int All                        = 65535;    //this value is the sum of all other values
    }
}
