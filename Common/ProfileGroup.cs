using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using FutureConcepts.Media.Server;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Describes a group of profiles, generally all related.
    /// </summary>
    [Serializable()]
    public class ProfileGroup : ICloneable
    {
        private string _name;
        /// <summary>
        /// Name of this Profile Group
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private string _comment;
        /// <summary>
        /// A comment about the groups purpose. Please limit to less than 40 characters.
        /// </summary>
        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        private string _defaultProfileName;
        /// <summary>
        /// A name referring to a <see cref="T:Profile"/> in the <see cref="M:Items"/> list.
        /// This name will be used to auto-select a profile from this group.
        /// </summary>
        public string DefaultProfileName
        {
            get
            {
                return _defaultProfileName;
            }
            set
            {
                _defaultProfileName = value;
            }
        }

//        private Boolean _customProfileEnabled = true;
        /// <summary>
        /// Is a custom profile enabled for this profile group?
        /// </summary>
//        public Boolean CustomProfileEnabled
//        {
//            get
//            {
//                return _customProfileEnabled;
//            }
//            set
//            {
//                _customProfileEnabled = value;
//            }
//        }

        private List<Profile> _items = new List<Profile>();
        private Dictionary<string, Profile> _dict = null;
        /// <summary>
        /// The list of <see cref="T:Profile"/>s this profile group contains.
        /// </summary>
        public List<Profile> Items
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
        /// Retreives a <see cref="T:Profile"/> by name
        /// </summary>
        /// <remarks>
        /// This accessor is error prone if you alter the <see cref="M:Items"/> collection.
        /// </remarks>
        /// <param name="name">name of the profile to fetch</param>
        /// <returns>The profile requested</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the name does not exist</exception>
        public Profile this[string name]
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<string, Profile>();
                    foreach (Profile profile in _items)
                    {
                        if (profile.Name.Contains(":"))
                        {
                            _dict.Add(profile.Name, profile);
                        }
                        else
                        {
                            _dict.Add(this.Name + ":" + profile.Name, profile);
                        }
                    }
                }
                if (name.Contains(":"))
                {
                    return _dict[name];
                }
                else
                {
                    return _dict[Name + ":" + name];
                }
            }
        }

        /// <summary>
        /// Gets a profile by index
        /// </summary>
        /// <param name="index">index to retrieve</param>
        /// <returns>The requested profile from the <see cref="M:Items"/> collection.</returns>
        public Profile this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        /// <summary>
        /// generates a ProfileGroup from a local file
        /// </summary>
        /// <param name="groupName">
        /// If groupName contains path separator characters, then it is assumed to be a fully qualified file path.
        /// If it does not, then it is interpreted as the name of the profile group to load.
        /// </param>
        /// <returns>returns a ProfileGroup</returns>
        public static ProfileGroup LoadFromFile(string groupName)
        {
            FileStream fileStream = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProfileGroup));
                string filename;
                if (groupName.Contains(Path.DirectorySeparatorChar.ToString()) || groupName.Contains(Path.AltDirectorySeparatorChar.ToString()))
                {
                    filename = groupName;
                }
                else
                {
                    filename = PathMapper.ConfigProfiles(groupName + @".xml");
                }

                fileStream = new FileStream(filename, FileMode.Open);
                ProfileGroup profileGroup = (ProfileGroup)xmlSerializer.Deserialize(fileStream);
                return profileGroup;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Saves <c>this</c> to the specified file.
        /// </summary>
        /// <param name="groupName">
        /// If groupName contains path separator characters, then it is assumed to be a fully qualified file path.
        /// If it does not, then it is interpreted as a group name, and is saved in the application's local profile storage folder, as "groupName.xml"
        /// </param>
        public void SaveToFile(string groupName)
        {
            XmlSerializer x = new XmlSerializer(typeof(ProfileGroup));

            DirectoryInfo dirInfo = Directory.GetParent(Assembly.GetEntryAssembly().Location);
            string filename;
            if (groupName.Contains(Path.DirectorySeparatorChar.ToString()) || groupName.Contains(Path.AltDirectorySeparatorChar.ToString()))
            {
                filename = groupName;
            }
            else
            {
                filename = dirInfo.FullName + @"/app_data/Profiles/" + groupName + @".xml";
            }

            FileStream file = new FileStream(filename, FileMode.Create);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            x.Serialize(file, this, ns);

            file.Close();
            file.Dispose();
        }

        /// <summary>
        /// Does a deep-clone of <c>this</c> and returns it.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            ProfileGroup clone = new ProfileGroup();

            clone.Name = (this.Name != null) ? (string)this.Name.Clone() : null;
            
            clone.DefaultProfileName = (this.DefaultProfileName != null) ? (string)this.DefaultProfileName.Clone() : null;

            clone.Comment = (this.Comment != null) ? (string)this.Comment.Clone() : null;

            foreach (Profile p in this.Items)
            {
                clone.Items.Add((Profile)p.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Checks if this profile group is equivelent to another object
        /// </summary>
        /// <remarks>
        /// Returns true if all properties are true, and each of the <c>Items</c> collections contain equivelent profiles.
        /// Order of the profiles does not matter.
        /// </remarks>
        /// <param name="obj">object to compare against</param>
        /// <returns>true if equal, false if not</returns>
        public override bool Equals(object obj)
        {
            ProfileGroup rhs = obj as ProfileGroup;
            if (((object)rhs) == null)
                return false;

            bool mainMembersEqual = ((this.Name == rhs.Name) &&
                                     (this.DefaultProfileName == rhs.DefaultProfileName) &&
                                     (this.Comment == rhs.Comment));

            if((mainMembersEqual) && (this.Items != null) && (rhs.Items != null))
            {
                if(this.Items.Count != rhs.Items.Count)
                {
                    return false;
                }
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i] != rhs.Items[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for a Profile Group
        /// </summary>
        /// <returns>a hash code for this profile group</returns>
        public override int GetHashCode()
        {
            return (this.Name.GetHashCode());
        }

        /// <summary>
        /// Determines the equality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a == b, and false if a != b</returns>
        public static bool operator ==(ProfileGroup a, ProfileGroup b)
        {
            if ((((object)a) == null) && (((object)b) == null))
                return true;

            if (((object)a) != null)
                return a.Equals(b);

            return false;
        }

        /// <summary>
        /// Determines the inequality of two VideoSettings <c>a</c> and <c>b</c>
        /// </summary>
        /// <returns>returns true if a != b, and false if a == b</returns>
        public static bool operator !=(ProfileGroup a, ProfileGroup b)
        {
            return !(a == b);
        }
    }
}
