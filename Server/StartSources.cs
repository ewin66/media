using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FutureConcepts.Media.Server
{
    [Serializable]
    public class StartSources
    {
        private List<StartSource> _items;
        /// <summary>
        /// Gets or sets the list of <see cref="T:StreamSourceInfo"/>
        /// </summary>
        public List<StartSource> Items
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
        /// Fetches the file name where this collection should be persisted.
        /// </summary>
        public static string PersistFileName
        {
            get
            {
                return Server.PathMapper.Config("StartSources.xml");
            }
        }

        private static StartSources _startSources = null;

        /// <summary>
        /// Loads the StreamSources for the application, prefering the memory cached copy if present.
        /// </summary>
        /// <returns>the StreamSources for the application</returns>
        public static StartSources LoadFromFile()
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
        public static StartSources LoadFromFile(bool bypassCache)
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
        public static StartSources LoadFromFile(string fileName, bool bypassCache)
        {
            if ((_startSources == null) || bypassCache)
            {
                FileStream file = null;
                try
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(StartSources));
                    file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StartSources s = (StartSources)xmlSerializer.Deserialize(file);

                    if (bypassCache)
                    {
                        return s;
                    }
                    else
                    {
                        _startSources = s;
                    }
                }
                catch (Exception e)
                {
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
            }
            return _startSources;
        }

        /// <summary>
        /// Writes a <see cref="StreamSources"/> collection to disk at the <see cref="P:PersistFileName"/> location.
        /// </summary>
        /// <param name="sources">sources to write</param>
        public static void SaveToFile(StartSources startSources)
        {
            FileStream file = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(startSources.GetType());
                file = new FileStream(PersistFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                xmlSerializer.Serialize(file, startSources);
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
