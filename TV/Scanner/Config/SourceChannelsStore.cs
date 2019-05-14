using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace FutureConcepts.Media.TV.Scanner.Config
{
    /// <summary>
    /// This class is used to serialize the contents of the FavoriteChannels control
    /// Kevin Dixon
    /// 10/17/2008
    /// </summary>
    [Serializable()]
    public class SourceChannelsStore
    {
        public SourceChannelsStore()
        {
            this.Source = new List<SourceChannelsStoreItem>();
        }

        public void Add(TVSource type, ChannelCollection favorites)
        {
            Source.Add(new SourceChannelsStoreItem(type, favorites));
        }

        [XmlElement]
        public List<SourceChannelsStoreItem> Source { get; set; }

        /// <summary>
        /// Writes the current data to the given local file.
        /// </summary>
        /// <param name="filename">file to write to</param>
        public void SaveToFile(string filename)
        {
            XmlWriter w = null;
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(SourceChannelsStore));
                w = new XmlTextWriter(filename, null);
                x.Serialize(w, this);
            }
            finally
            {
                if (w != null)
                {
                    w.Close();
                }
            }
        }

        /// <summary>
        /// Loads a FavoriteChannelsStore from a local file
        /// </summary>
        /// <param name="filename">file to read from</param>
        /// <returns>the FavoriteChannelsStore as read from disk</returns>
        public static SourceChannelsStore LoadFromFile(string filename)
        {
            FileStream s = null;
            XmlTextReader r = null;
            try
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException("File does not exist.", filename);
                }

                XmlSerializer x = new XmlSerializer(typeof(SourceChannelsStore));
                s = new FileStream(filename, FileMode.Open, FileAccess.Read);
                r = new XmlTextReader(s);
                SourceChannelsStore store = (SourceChannelsStore)x.Deserialize(r);

                return store;
            }
            finally
            {
                if (r != null)
                {
                    r.Close();
                }
                if (s != null)
                {
                    s.Close();
                }
            }
        }
    }

    /// <summary>
    /// This class represents one pairing of TV Source to a channel collection that are its favorites
    /// </summary>
    [Serializable()]
    public class SourceChannelsStoreItem
    {
        public SourceChannelsStoreItem()
        {
        }

        public SourceChannelsStoreItem(TVSource type, ChannelCollection channels)
        {
            this.Type = type;
            this.Channels = channels;
        }

        [XmlAttribute]
        public TVSource Type { get; set; }

        [XmlElement]
        public ChannelCollection Channels { get; set; }
    }
}
