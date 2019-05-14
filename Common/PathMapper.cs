using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FutureConcepts.Settings;

namespace FutureConcepts.Media.Server
{
    /// <summary>
    /// This is the PathMapper class for the Server
    /// </summary>
    /// <author>kdixon 01/26/2011</author>
    public static class PathMapper
    {
        /// <summary>
        /// Returns the location of the given file or directory in the read/write persistant store
        /// </summary>
        /// <param name="fileOrDirName">File or Directory name, with no preceeding slashes</param>
        /// <returns>the fully qualified path of the read/write directory, and specified sub-items</returns>
        public static string AppData(string fileOrDirName)
        {
            return GenericSettings.AntaresXDataPath + @"Output\MediaServer\" + fileOrDirName;
        }

        /// <summary>
        /// Returns the location of the given file or directory in the configuration
        /// </summary>
        /// <param name="fileOrDirName">File or Directory name, with no preceeding slashes</param>
        /// <returns>the fully qualified path of the configuration directory and specified sub-items</returns>
        public static string Config(string fileOrDirName)
        {
            return GenericSettings.AntaresXStaticSettingsPath + @"MediaServer\" + fileOrDirName;
        }

        /// <summary>
        /// Path to the Profiles directory
        /// </summary>
        /// <returns>Path to the Profiles directory</returns>
        public static string ConfigProfiles(string fileOrDirName)
        {
            return Config(@"Profiles\" + fileOrDirName);
        }
    }
}

