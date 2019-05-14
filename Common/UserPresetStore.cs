using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Collections;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Generic class for storing User Presets. Any derived classes of UserPresetItem must be listed in the XmlInclude attribute
    /// kdixon 02/09/2009
    /// </summary>
    /// <remarks>
    ///  - Adapted from NamedCameraPositions as written by darnold
    /// Implements IList so that it can be properly serialized by XmlSerializer
    /// </remarks>
    [Serializable]
    [XmlRoot("UserPresetStore")]
    public class UserPresetStore : IList<UserPresetItem>
    {
        /// <summary>
        /// The actual backing store.
        /// </summary>
        private Dictionary<Guid, UserPresetItem> store = new Dictionary<Guid, UserPresetItem>();

        /// <summary>
        /// Adds a new user preset to the collection.
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(UserPresetItem item)
        {
            store.Add(item.ID, item);
        }

        /// <summary>
        /// Removes a specific item from the collection
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>true if successfully removed.</returns>
        public bool Remove(UserPresetItem item)
        {
            return this.Remove(item.ID);
        }

        /// <summary>
        /// Removes a specific item from the collection.
        /// </summary>
        /// <param name="id">The ID of the item to remove</param>
        /// <returns>true if successfully removed</returns>
        public bool Remove(Guid id)
        {
            return store.Remove(id);
        }

        /// <summary>
        /// Determines if this collection contains the given item.
        /// </summary>
        /// <param name="item">item to check for</param>
        /// <returns>true if found in the collection, false if not.</returns>
        public bool Contains(UserPresetItem item)
        {
            return this.Contains(item.ID);
        }

        /// <summary>
        /// Determins if this collection contains the given item.
        /// </summary>
        /// <param name="id">ID of the item to check for</param>
        /// <returns>true if found in the collection, false if not.</returns>
        public bool Contains(Guid id)
        {
            return store.ContainsKey(id);
        }

        /// <summary>
        /// Removes all items from the collection
        /// </summary>
        public void Clear()
        {
            store.Clear();
        }

        /// <summary>
        /// Gets a specific item
        /// </summary>
        /// <param name="id">The item's ID</param>
        /// <returns>the item requested, or null if not found</returns>
        public UserPresetItem this[Guid id]
        {
            get
            {
                if (store.ContainsKey(id))
                {
                    return store[id];
                }
                return null;
            }
        }

        /// <summary>
        /// Copies the contents of this collection to the given array
        /// </summary>
        /// <param name="array">array to copy to</param>
        /// <param name="arrayIndex">array index to start copying to</param>
        public void CopyTo(UserPresetItem[] array, int arrayIndex)
        {
            if ((array.Length - arrayIndex) < this.Count)
            {
                throw new OverflowException("Too many items are in this collection to fit in the supplied array");
            }

            int i = arrayIndex;
            foreach (UserPresetItem item in store.Values)
            {
                array[i] = item;
                i++;
            }
        }

        /// <summary>
        /// Gets the number of items in the collection
        /// </summary>
        public int Count
        {
            get { return store.Count; }
        }

        /// <summary>
        /// True if the collection cannot be altered
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an enumerator for the items.
        /// </summary>
        /// <returns>the enumerator</returns>
        public IEnumerator<UserPresetItem> GetEnumerator()
        {
            return store.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the items.
        /// </summary>
        /// <returns>the enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return store.GetEnumerator();
        }

        #region Not Supported Junk

        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(UserPresetItem item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, UserPresetItem item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public UserPresetItem this[int index]
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}
