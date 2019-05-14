using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.IO;

namespace FutureConcepts.Media.TV.Scanner.Config
{
    [Serializable()]
    public class SourcesConfig
    {
        private List<Source> _sources = new List<Source>();
        /// <summary>
        /// All of the sources in this config
        /// </summary>
        [XmlElement]
        public List<Source> Source
        {
            get
            {
                return _sources;
            }
            set
            {
                _sources = value;
            }
        }

        /// <summary>
        /// Fetches a source by name
        /// </summary>
        /// <param name="name">name of the source</param>
        /// <returns>returns the Source object</returns>
        /// <exception cref="FutureConcepts.Media.SourceConfigException">Thrown if the requested name was not found</exception>
        public Source this[string name]
        {
            get
            {
                foreach (Source s in Source)
                {
                    if (name.Equals(s.Name))
                    {
                        return s;
                    }
                }
                throw new SourceConfigException("The source '" + name + "' is not a configured source.");
            }
        }

        /// <summary>
        /// Determines if this config has a source by a given name
        /// </summary>
        /// <param name="name">name to lookup</param>
        /// <returns>true if source is in config, false it not</returns>
        public bool HasSource(string name)
        {
            foreach (Source s in Source)
            {
                if (name.Equals(s.Name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Loads the SourcesConfig from the default file
        /// </summary>
        /// <returns>the default SourcesConfig</returns>
        public static SourcesConfig LoadFromFile()
        {
            return LoadFromFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\app_data\DeviceConfig.xml");
        }

        /// <summary>
        /// Loads the SourcesConfig from the specified file.
        /// </summary>
        /// <param name="path">path to load from</param>
        /// <returns>the SourcesConfig for this path, or throws an exception</returns>
        public static SourcesConfig LoadFromFile(string path)
        {
            XmlTextReader xr = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SourcesConfig));
                xr = new XmlTextReader(path);
                SourcesConfig config = (SourcesConfig)serializer.Deserialize(xr);
                return config;
            }
            finally
            {
                if (xr != null)
                {
                    xr.Close();
                }
            }
        }

        public void SaveToFile(string path)
        {
            XmlTextWriter xw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SourcesConfig));
                xw = new XmlTextWriter(path, null);
                serializer.Serialize(xw, this);
            }
            finally
            {
                if (xw != null)
                {
                    xw.Close();
                }
            }
        }
    }
}
