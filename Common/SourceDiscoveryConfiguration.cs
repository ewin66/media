using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Describes a set of <see cref="T:StreamSourceInfo"/>. Generally this structure is used to
    /// collect all <see cref="T:StreamSourceInfo"/> on a single server.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SourceDiscoveryConfiguration : IExtensibleDataObject
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

        private List<SourceDiscoveryDefinition> _items;
        /// <summary>
        /// Gets or sets the list of <see cref="T:StreamSourceInfo"/>
        /// </summary>
        [DataMember]
        public List<SourceDiscoveryDefinition> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        /// <summary>
        /// Adds a new <see cref="T:StreamSourceInfo"/> item to the collection.
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(SourceDiscoveryDefinition item)
        {
            if (_items == null)
            {
                _items = new List<SourceDiscoveryDefinition>();
            }
            _items.Add(item);
        }

        /// <summary>
        /// Locates a specific source in the configuration
        /// </summary>
        /// <param name="sourceName">source name to locate</param>
        /// <returns>returns the <see cref="T:SourceDiscovery"/> for the given source name</returns>
        /// <exception cref="T:SourceConfigException">Thrown if the requested source is not found</exception>
        public SourceDiscoveryDefinition FindSourceByName(string name)
        {
            foreach (SourceDiscoveryDefinition sourceDiscovery in Items)
            {
                if (sourceDiscovery.Name == name)
                {
                    return sourceDiscovery;
                }
            }
            throw new SourceConfigException(String.Format("Source {0} not found", name));
        }

        /// <summary>
        /// Gets all sources of a given type.
        /// </summary>
        /// <param name="type">The SourceType to get sources for</param>
        /// <returns>
        /// returns a list of <see cref="T:StreamSourceInfo"/> that are of the specified type.
        /// Returns an empty list if none were found.
        /// </returns>
        public List<SourceDiscoveryDefinition> FindSourcesByType(String type)
        {
            List<SourceDiscoveryDefinition> list = new List<SourceDiscoveryDefinition>();
            foreach (SourceDiscoveryDefinition sourceDiscovery in Items)
            {
                if (sourceDiscovery.Type == type)
                {
                    list.Add(sourceDiscovery);
                }
            }
            return list;
        }

        /// <summary>
        /// Fetches the file name where this collection should be persisted.
        /// </summary>
        public static string PersistFileName
        {
            get
            {
                return ClientPathMapper.SVDConfig("SourceDiscoveryConfiguration.xml");
            }
        }

        private static SourceDiscoveryConfiguration _configuration = null;

        /// <summary>
        /// Loads the StreamSources for the application, prefering the memory cached copy if present.
        /// </summary>
        /// <returns>the StreamSources for the application</returns>
        public static SourceDiscoveryConfiguration LoadFromFile()
        {
            if (File.Exists(SourceDiscoveryConfiguration.PersistFileName) == false)
            {
                DirectoryInfo dirInfo = Directory.GetParent(Assembly.GetEntryAssembly().Location);
                File.Copy(dirInfo.FullName + @"/app_data/DefaultSourceDiscoveryConfiguration.xml", PersistFileName);
            }
            return LoadFromFile(PersistFileName, false);
        }

        /// <summary>
        /// Loads the StreamSources for the application.
        /// </summary>
        /// <param name="bypassCache">
        /// Set to true to always read from the disk and return a distinct copy
        /// Set to false to use the cached copy if available.
        /// </param>
        /// <returns>the <see cref="T:StreamSources"/> for the application</returns>
        public static SourceDiscoveryConfiguration LoadFromFile(bool bypassCache)
        {
            return LoadFromFile(PersistFileName, bypassCache);
        }

        /// <summary>
        /// Loads StreamSources from a specified file.
        /// </summary>
        /// <param name="fileName">The filename to load. If you are trying to load multiple distinct files, set bypassCache to true.</param>
        /// <param name="bypassCache">
        /// Set to true to always read from the disk and return a distinct copy
        /// Set to false to use the cached copy if available.
        /// </param>
        /// <returns>the StreamSources from the indicated file</returns>
        public static SourceDiscoveryConfiguration LoadFromFile(string fileName, bool bypassCache)
        {
            if ((_configuration == null) || bypassCache)
            {
                FileStream file = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SourceDiscoveryConfiguration));
                    file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    SourceDiscoveryConfiguration s = (SourceDiscoveryConfiguration)serializer.Deserialize(file);
                    if (bypassCache)
                    {
                        return s;
                    }
                    else
                    {
                        _configuration = s;
                    }
                }
                finally
                {
                    if (file != null)
                    {
                        file.Close();
                        file.Dispose();
                    }
                }
            }
            return _configuration;
        }

        private void CreateTestConfig()
        {
            FileStream file = null;
            DataContractSerializer serializer = new DataContractSerializer(typeof(SourceDiscoveryConfiguration),
                new List<Type> { typeof(SourceDiscoveryDefinition) });
            String fileName = @"C:\Temp\test.xml";
            file = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
            SourceDiscoveryDefinition testdef = new SourceDiscoveryDefinition();
            testdef.Name = "Dave";
            testdef.Type = "roogy";
            testdef.URL = "floor";
            testdef.PollInterval = 30000;
            SourceDiscoveryConfiguration testconfig = new SourceDiscoveryConfiguration();
            testconfig.Items = new List<SourceDiscoveryDefinition>();
            testconfig.Items.Add(testdef);
            serializer.WriteObject(file, testconfig);
            file.Close();
        }

        /// <summary>
        /// Writes a <see cref="StreamSources"/> collection to disk at the <see cref="P:PersistFileName"/> location.
        /// </summary>
        /// <param name="sources">sources to write</param>
        public static void SaveToFile(SourceDiscoveryConfiguration configuration)
        {
            FileStream file = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(configuration.GetType());
                file = new FileStream(PersistFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                xmlSerializer.Serialize(file, configuration);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
            }
        }
    }
}
