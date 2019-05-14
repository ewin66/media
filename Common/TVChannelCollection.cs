using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace FutureConcepts.Media.TV
{
    /// <summary>
    /// Represents a collection of Channels
    /// Supports serializing to/from disk, and is serializable in general.
    /// Kevin Dixon
    /// </summary>
    /// <remarks>
    /// This collection does not allow duplicate logical channels (Major/Minor channel match).
    /// This collection uses BinarySearch to do fast lookups.
    /// This collection keeps the collection ordered at all times.
    /// </remarks>
    [Serializable]
    public class ChannelCollection
    {
        /// <summary>
        /// Adds the channel to the collection. Does nothing if the channel is a logical duplicate. (Major/Minor channel match)
        /// </summary>
        /// <param name="channel">channel to add</param>
        /// <returns>Returns the index where the channel was added. Returns -1 if nothing happened</returns>
        public int Add(Channel channel)
        {
            int insertAt = IndexOf(channel);
            if (insertAt < 0)
            {
                Items.Insert(~insertAt, channel);
                return ~insertAt;
            }

            return -1;
        }

        /// <summary>
        /// Adds all of the channels in an enumerable set
        /// </summary>
        /// <param name="items">an enumerable collection of Channel objects</param>
        public void AddRange(IEnumerable<Channel> items)
        {
            foreach (Channel ch in items)
            {
                this.Add(ch);
            }
        }

        /// <summary>
        /// Returns the index of a given channel, matched by logical channel
        /// </summary>
        /// <param name="match">channel to get the index of</param>
        /// <returns>returns a value >= 0 if found, or less than 0 if not found</returns>
        public int IndexOf(Channel match)
        {
            return Items.BinarySearch(match);
        }

        /// <summary>
        /// Returns true if the given logical channel is in this collection
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool Contains(Channel channel)
        {
            return (IndexOf(channel) > -1);
        }

        /// <summary>
        /// Removes the specified channel from the collection.
        /// </summary>
        /// <param name="channel">channel to remove</param>
        public void Remove(Channel channel)
        {
            int i = IndexOf(channel);
            if (i >= 0)
            {
                Items.RemoveAt(i);
            }
        }

        /// <summary>
        /// Removes all items from the collection
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Gets the number of channels in the collection.
        /// </summary>
        [XmlIgnore]
        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        /// <summary>
        /// Finds the closest logical channel.
        /// </summary>
        /// <param name="mask">Set fields to -1 to not care about those</param>
        /// <returns>Returns the nearest Channel, with the most matching fields. Returns null if no channels are in the collection.</returns>
        public Channel FindClosest(Channel mask)
        {
            if (Items.Count == 0)
            {
                return null;
            }

            return Items[FindClosestIndex(mask)];
        }

        /// <summary>
        /// Finds the index of the closest logical channel
        /// </summary>
        /// <param name="mask">Set fields to -1 to not care about those</param>
        /// <returns>Returns the index of the nearest Channel, with the most matching fields.</returns>
        public int FindClosestIndex(Channel mask)
        {
            int i = IndexOf(mask);
            if (i < 0)
            {
                if ((~i) >= Items.Count)
                {
                    return (Items.Count - 1);
                }

                if (Items[~i].MajorChannel != mask.MajorChannel)
                {
                    if (~i > 0)
                    {
                        if (Items[~i - 1].MajorChannel == mask.MajorChannel)
                        {
                            return (~i - 1);
                        }
                    }
                }
                return (~i);
            }
            else
            {
                return i;
            }
        }

        private List<Channel> _channels;
        /// <summary>
        /// Gets or sets the list if <see cref="T:Channel"/> objects associated with this collection.
        /// </summary>
        [XmlElement]
        public List<Channel> Items
        {
            get
            {
                if (_channels == null)
                {
                    _channels = new List<Channel>();
                }
                return _channels;
            }
            set
            {
                _channels = value;
                if (value != null)
                {
                    _channels.Sort();
                }
            }
        }

        /// <summary>
        /// Writes the whole collection to the Debug trace listener
        /// </summary>
        public void DumpToDebug()
        {
            Debug.WriteLine("ChannelCollection.DumpToDebug: " + this.Items.Count + " items");
            foreach (Channel ch in Items)
            {
                Debug.WriteLine(ch.ToDebugString());
            }
        }

        /// <summary>
        /// Writes the current ChannelCollection to the specified file.
        /// Throws any exceptions to the caller
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        public void SaveToFile(string filename)
        {
            XmlTextWriter w = null;
            try
            {
                w = new XmlTextWriter(filename, null);
                XmlSerializer s = new XmlSerializer(typeof(ChannelCollection));
                s.Serialize(w, this);
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
        /// Deserializes a ChannelCollection from the given file.
        /// Throws any exceptions to the caller
        /// </summary>
        /// <param name="filename">path to file to load from</param>
        /// <returns>returns a properly populated collection, or throws an exception</returns>
        public static ChannelCollection LoadFromFile(string filename)
        {
            FileStream file = null;
            XmlTextReader r = null;
            try
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException("File does not exist.", filename);
                }
                file = new FileStream(filename, FileMode.Open, FileAccess.Read);
                r = new XmlTextReader(file);
                XmlSerializer s = new XmlSerializer(typeof(ChannelCollection));
                ChannelCollection c = (ChannelCollection)s.Deserialize(r);
                return c;
            }
            finally
            {
                if (r != null)
                {
                    r.Close();
                }
                if (file != null)
                {
                    file.Close();
                }
            }  
        }
    }
}
