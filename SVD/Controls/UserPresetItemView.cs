using System;
using System.Drawing;
using System.ComponentModel;
using FutureConcepts.Media.Contract;
using System.Xml.Serialization;

namespace FutureConcepts.Media.SVD.Controls
{
    /// <summary>
    /// This class is the "view" portion of a User Preset Item
    /// </summary>
    public class UserPresetItemView : INotifyPropertyChanged
    {
        private UserPresetItem _item;
        /// <summary>
        /// The Preset data for this item
        /// </summary>
        public UserPresetItem Preset
        {
            get
            {
                return _item;
            }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    NotifyPropertyChanged("Preset");
                }
            }
        }

        private Image _image;
        /// <summary>
        /// The image to display for this item
        /// </summary>
        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    NotifyPropertyChanged("Image");
                }
            }
        }

        public override string ToString()
        {
            return (_item != null) ? _item.ToString() : base.ToString();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion
    }
}
