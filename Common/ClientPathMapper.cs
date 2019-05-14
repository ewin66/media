using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using FutureConcepts.Settings;

namespace FutureConcepts.Media
{
    /// <summary>
    /// This is the PathMapper class for media client apps
    /// </summary>
    /// <author>darnold 11/20/2013 - from kdixon's server PathMapper</author>
    public static class ClientPathMapper
    {
        /// <summary>
        /// Returns the location of the given file or directory in the configuration
        /// </summary>
        /// <param name="fileOrDirName">File or Directory name, with no preceeding slashes</param>
        /// <returns>the fully qualified path of the configuration directory and specified sub-items</returns>
        public static string SVDConfig(string fileOrDirName)
        {
            String rootPath = GenericSettings.AntaresXStaticSettingsPath + @"SVD\";
            if (Directory.Exists(rootPath) == false)
            {
                Directory.CreateDirectory(rootPath);
            }
            return rootPath + fileOrDirName;
        }
    }
}
