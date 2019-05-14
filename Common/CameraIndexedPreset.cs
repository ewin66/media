using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Represents a camera preset that is indexed rather than a store of absolute coordinates.
    /// Typically used for camera hardware that prefers to refer to stored positions by index.
    /// kdixon 05/27/2009
    /// </summary>
    [Serializable]
    public class CameraIndexedPreset : UserPresetItem
    {
        /// <summary>
        /// Creates a CameraIndexedPreset with no associated index, and a default Name
        /// </summary>
        public CameraIndexedPreset()
            : base()
        {
            this.Name = "New Preset";
        }

        /// <summary>
        /// Creates a CameraIndexedPreset with a specific index. The Name is generated from the preset number.
        /// </summary>
        /// <param name="index">index to use.</param>
        public CameraIndexedPreset(int index)
            : base()
        {
            this.Index = index;
            this.Name = "Preset " + index.ToString();
        }

        private int _index;
        /// <summary>
        /// The index of this preset
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }
    }
}
