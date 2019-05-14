using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FutureConcepts.Media.Server;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Defines a list of <see cref="ProfileGroup"/>. <seealso cref="T:ProfileGroup"/> <seealso cref="T:Profile"/>
    /// </summary>
    [Serializable()]
    public class ProfileGroups
    {
        private List<ProfileGroup> _items = new List<ProfileGroup>();

        /// <summary>
        /// Gets or sets the list of <see cref="T:ProfileGroup"/>
        /// </summary>
        public List<ProfileGroup> Items
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

        private static ProfileGroups _cachedProfileGroups;

        private static object _cacheLock = new Object();

        /// <summary>
        /// Deserializes all of the <see cref="T:ProfileGroup"/> files found in the app_data/Profiles/ and puts them into a <see cref="T:ProfileGroups"/>.
        /// </summary>
        /// <returns>a new instance of <see cref="T:ProfileGroups"/></returns>
        public static ProfileGroups LoadFromFile()
        {
            lock (_cacheLock)
            {
                if (_cachedProfileGroups == null)
                {
                    _cachedProfileGroups = new ProfileGroups();
                    string[] files = Directory.GetFiles(PathMapper.ConfigProfiles(""));
                    foreach (string profileFileName in files)
                    {
                        _cachedProfileGroups.Items.Add(ProfileGroup.LoadFromFile(profileFileName));
                    }
                }
            }
            return _cachedProfileGroups;
        }
    }
}
