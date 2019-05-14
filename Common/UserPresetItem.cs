using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Base class for user-defined presets
    /// kdixon 02/09/2009
    /// </summary>
    /// <remarks>
    /// For any derived classes that need to be serialized, we must add an XmlInclude and KnownType attribute
    /// </remarks>
    [Serializable]
    [XmlInclude(typeof(CameraPositionPreset))]
    [XmlInclude(typeof(MicrowaveFrequencyPreset))]
    [XmlInclude(typeof(CameraIndexedPreset))]
    [XmlInclude(typeof(MicrowaveTuningPreset))]
    [KnownType(typeof(CameraPositionPreset))]
    [KnownType(typeof(MicrowaveFrequencyPreset))]
    [KnownType(typeof(CameraIndexedPreset))]
    [KnownType(typeof(MicrowaveTuningPreset))]
    public class UserPresetItem
    {
        private Guid _id = Guid.NewGuid();
        /// <summary>
        /// A globally-unique identifier for this item
        /// </summary>
        [XmlAttribute]
        public Guid ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private string _name;
        /// <summary>
        /// A friendly name for users to view
        /// </summary>
        [XmlElement]
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

        /// <summary>
        /// Returns the friendly name of the UserPresetItem
        /// </summary>
        /// <returns>Returns the friendly name of the UserPresetItem</returns>
        public override string ToString()
        {
            return (_name != null) ? _name : base.ToString();
        }
    }
}
