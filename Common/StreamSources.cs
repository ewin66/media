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
    [Serializable]
    public class StreamSources
    {
        private List<StreamSourceInfo> _items;
        /// <summary>
        /// Gets or sets the list of <see cref="T:StreamSourceInfo"/>
        /// </summary>
        public List<StreamSourceInfo> Items
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
        /// Gets the number of <see cref="T:StreamSourceInfo"/> in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                if (_items != null)
                {
                    return _items.Count;
                }
                else
                {
                    return 0;
                }
            }
            set {}
        }

        /// <summary>
        /// Adds a new <see cref="T:StreamSourceInfo"/> item to the collection.
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(StreamSourceInfo item)
        {
            if (_items == null)
            {
                _items = new List<StreamSourceInfo>();
            }
            _items.Add(item);
        }

        /// <summary>
        /// Locates a specific source in the configuration
        /// </summary>
        /// <param name="sourceName">source name to locate</param>
        /// <returns>returns the <see cref="T:StreamSourceInfo"/> for the given source name</returns>
        /// <exception cref="T:SourceConfigException">Thrown if the requested source is not found</exception>
        public StreamSourceInfo FindSource(string sourceName)
        {
            foreach (StreamSourceInfo sourceInfo in Items)
            {
                if (sourceInfo.SourceName == sourceName)
                {
                    return sourceInfo;
                }
            }
            throw new SourceConfigException(String.Format("Source {0} not found", sourceName));
        }

        /// <summary>
        /// Gets all sources of a given type.
        /// </summary>
        /// <param name="type">The SourceType to get sources for</param>
        /// <returns>
        /// returns a list of <see cref="T:StreamSourceInfo"/> that are of the specified type.
        /// Returns an empty list if none were found.
        /// </returns>
        public List<StreamSourceInfo> FindSources(SourceType type)
        {
            List<StreamSourceInfo> list = new List<StreamSourceInfo>();
            foreach (StreamSourceInfo info in Items)
            {
                if (info.SourceType == type)
                {
                    list.Add(info);
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
                return Server.PathMapper.Config("Sources.xml");
            }
        }

        private static StreamSources _sources = null;

        /// <summary>
        /// Loads the StreamSources for the application, prefering the memory cached copy if present.
        /// </summary>
        /// <returns>the StreamSources for the application</returns>
        public static StreamSources LoadFromFile()
        {
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
        public static StreamSources LoadFromFile(bool bypassCache)
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
        public static StreamSources LoadFromFile(string fileName, bool bypassCache)
        {
            if ((_sources == null) || bypassCache)
            {
                FileStream file = null;
                try
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(StreamSources));
                    file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamSources s = (StreamSources)xmlSerializer.Deserialize(file);

                    if (bypassCache)
                    {
                        return s;
                    }
                    else
                    {
                        _sources = s;
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
            return _sources;
        }

        /// <summary>
        /// Writes a <see cref="StreamSources"/> collection to disk at the <see cref="P:PersistFileName"/> location.
        /// </summary>
        /// <param name="sources">sources to write</param>
        public static void SaveToFile(StreamSources sources)
        {
            FileStream file = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(sources.GetType());
                file = new FileStream(PersistFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                xmlSerializer.Serialize(file, sources);
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
