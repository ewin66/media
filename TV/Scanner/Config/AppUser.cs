using System;
using System.Xml.Serialization;
using FutureConcepts.SystemTools.Settings;

namespace FutureConcepts.Media.TV.Scanner.Config
{
    /// <summary>
    /// Contains configuration for Application and User
    /// kdixon 02/02/2009
    /// </summary>
    public class AppUser : GenericSettings
    {
        /// <summary>
        /// The application's name, which will be its subfolder in the Settings folder
        /// </summary>
        private static readonly string appName = @"TV Scanner\";

        /// <summary>
        /// These settings are edited by the application on close
        /// </summary>
        [XmlIgnore]
        public override bool StaticSettings
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Fetches the TV Scanner settings storage directory
        /// </summary>
        [XmlIgnore]
        public static string TVScannerSettingsRoot
        {
            get
            {
                return GenericSettings.AntaresXDataPath + @"Settings\" + appName;
            }
        }

        [XmlIgnore]
        public static string TVScannerOutputRoot
        {
            get
            {
                return GenericSettings.AntaresXDataPath + @"Output\" + appName;
            }
        }

        private static AppUser _me;
        /// <summary>
        /// Static singleton of these settings which should be used at all times.
        /// </summary>
        [XmlIgnore]
        public static AppUser Current
        {
            get
            {
                if (_me == null)
                {
                    _me = new AppUser();
                }
                return _me;
            }
        }

        /// <summary>
        /// Call the default constructor
        /// </summary>
        public AppUser() : base() { }

        public override void SetDefaults()
        {
            TunerDeviceName = "";
            ServerAddress = "";
            ServerSourceName = "";
            TVSource = TVSource.LocalDigital;
            TVMode = TVMode.Broadcast;
            Mute = false;
            Volume = 0;
            RemoteType = "";
            SnapshotMaximum = 20;
            SnapshotInterval = 3;
            ChannelIndex = 0;
            TunerInputType = DirectShowLib.TunerInputType.Antenna;
            SatelliteConnector = DirectShowLib.PhysicalConnectorType.Video_Composite;
        }

        public override string GetFileName()
        {
            return "TVScanner.fcs";
        }

        public override string GetSectionName()
        {
            return "ApplicationUser";
        }

        /// <summary>
        /// The name of the tuner device to use.
        /// </summary>
        public string TunerDeviceName { get; set; }

        /// <summary>
        /// Last used server address. Should only be defined if the TVSource is Network
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// Last used server SourceName
        /// </summary>
        public string ServerSourceName { get; set; }

        /// <summary>
        /// Last used TV Source
        /// </summary>
        public TVSource TVSource { get; set; }

        /// <summary>
        /// Last used TV Mode
        /// </summary>
        public TVMode TVMode { get; set; }

        /// <summary>
        /// Last Mute state
        /// </summary>
        public bool Mute { get; set; }

        /// <summary>
        /// Last Volume level
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Remote control type identifier
        /// </summary>
        public string RemoteType { get; set; }

        /// <summary>
        /// Maximum number of snapshots per auto-snap-session
        /// </summary>
        public int SnapshotMaximum { get; set; }

        /// <summary>
        /// Interval of seconds between each snapshot in auto-snap mode
        /// </summary>
        public int SnapshotInterval { get; set; }

        /// <summary>
        /// the last-tuned Channel Index
        /// </summary>
        public int ChannelIndex { get; set; }

        /// <summary>
        /// The Tuner Input Type
        /// </summary>
        public DirectShowLib.TunerInputType TunerInputType { get; set; }

        /// <summary>
        /// The physical connector type of the Satellite input
        /// </summary>
        public DirectShowLib.PhysicalConnectorType SatelliteConnector { get; set; }
    }
}
