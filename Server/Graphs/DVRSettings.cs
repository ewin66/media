using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FutureConcepts.Media.Server.Graphs
{
    [DataContract]
    [Serializable]
    public class DVRSettings : IExtensibleDataObject
    {
        [NonSerialized]
        private ExtensionDataObject _extensionDataObject;

        public virtual ExtensionDataObject ExtensionData
        {
            get
            {
                return _extensionDataObject;
            }
            set
            {
                _extensionDataObject = value;
            }
        }

        private String _rootFolder;

        [DataMember]
        public String RootFolder
        {
            get
            {
                return _rootFolder;
            }
            set
            {
                _rootFolder = value;
            }
        }

        private int _numberOfFiles = 5;

        [DataMember]
        public int NumberOfFiles
        {
            get
            {
                return _numberOfFiles;
            }
            set
            {
                _numberOfFiles = value;
            }
        }

        private double _fileSize = 80000;

        [DataMember]
        public double FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
            }
        }

        /// <summary>
        /// Fetches the file name where this collection should be persisted.
        /// </summary>
        public static string PersistFileName
        {
            get
            {
                return Server.PathMapper.Config("DVR.xml");
            }
        }
        /// <summary>
        /// Loads the StreamSources for the application, prefering the memory cached copy if present.
        /// </summary>
        /// <returns>the StreamSources for the application</returns>
        public static DVRSettings LoadFromFile()
        {
            return LoadFromFile(PersistFileName);
        }

        /// <summary>
        /// Loads DVRSettings from a specified file.
        /// </summary>
        /// <param name="fileName">The filename to load. If you are trying to load multiple distinct files, set bypassCache to true.</param>
        /// Set to true to always read from the disk and return a distinct copy
        /// Set to false to use the cached copy if available.
        /// </param>
        /// <returns>the RTSPServer from the indicated file</returns>
        public static DVRSettings LoadFromFile(string fileName)
        {
            DVRSettings result = null;
            FileStream file = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DVRSettings));
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                result = (DVRSettings)xmlSerializer.Deserialize(file);
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format("DVRSettings was not loaded -- the configuration file {0} could not be loaded", PersistFileName));
                Debug.WriteLine(e.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
            }
            return result;
        }
    }
}
