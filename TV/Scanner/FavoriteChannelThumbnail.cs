using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.TV.Scanner
{
    public partial class FavoriteChannelThumbnail : UserControl
    {
        public FavoriteChannelThumbnail()
        {
            InitializeComponent();
        }

        public event EventHandler SetClick;
        public event EventHandler SnapshotClick;
        public event EventHandler DeleteClick;

        /// <summary>
        /// Sets or Gets the channel associated with the thumbnail.
        /// </summary>
        private Channel _channel;
        public Channel Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value;
                if (!string.IsNullOrEmpty(_channel.Callsign))
                {
                    gbChannelThumbnail.Text = _channel.ToString() + " (" + _channel.Callsign + ")";
                }
                else
                {
                    gbChannelThumbnail.Text = "Channel " + _channel.ToString();
                }

                tt.SetToolTip(pbSnapshot, null);
                tt.SetToolTip(pbSnapshot, "Switch to " + gbChannelThumbnail.Text);
            }
        }

        /// <summary>
        /// Sets or Gets the image associated with the thumbnail. When setting, auto re-sizes the image.
        /// </summary>
        public Image Image
        {
            set
            {
                if (value != null)
                {
                    Bitmap thumb = new Bitmap(pbSnapshot.Width, pbSnapshot.Height);
                    Graphics g = Graphics.FromImage(thumb);
                    g.DrawImage(value, 0, 0, pbSnapshot.Width, pbSnapshot.Height);
                    pbSnapshot.Image = thumb;
                }
                else
                {
                    pbSnapshot.Image = null;
                }
            }
            get
            {
                return pbSnapshot.Image;
            }
        }

        public void Clear()
        {
            pbSnapshot.Image = null;
            Channel = new Channel();
        }

        public bool DeleteButtonEnabled
        {
            get { return btnDelete.Enabled; }
            set { btnDelete.Enabled = value; }
        }

        public bool DeleteButtonVisible
        {
            get { return btnDelete.Visible; }
            set { btnDelete.Visible = value; }
        }

        public new bool Enabled
        {
            get
            {
                return (btnDelete.Enabled && pbSnapshot.Enabled);
            }
            set
            {
                btnDelete.Enabled = value;
                pbSnapshot.Enabled = value;
            }
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (SetClick != null)
            {
                SetClick(this, new EventArgs());
            }
        }

        private void pbSnapshot_Click(object sender, EventArgs e)
        {
            if (SnapshotClick != null)
            {
                SnapshotClick(this, new EventArgs());
            }
        }

        /// <summary>
        /// Sets or Gets the Highlighted state of the control
        /// </summary>
        private bool highlighted = false;
        public bool Highlighted
        {
            get { return highlighted; }
            set
            {
                highlighted = value;
                if (highlighted)
                {
                    this.BackColor = Color.Yellow;
                }
                else
                {
                    this.BackColor = Color.Black;    
                }
            }
        }

        /// <summary>
        /// Saves the currently set Image to the given path.
        /// </summary>
        /// <param name="path">
        /// Directory path to save into, not ending in "\".
        /// It will be saved with the file name this.Channel.ToString()
        /// </param>
        public void SaveThumbnailImage(string path)
        {
            try
            {
                if (pbSnapshot.Image != null)
                {
                    pbSnapshot.Image.Save(path + "\\" + this.Channel.ToString(), ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Loads the thumb for the currently set Channel.
        /// </summary>
        /// <param name="path">directory to load from, not ending in "\"</param>
        public void LoadThumbnailImage(string path)
        {
            LoadThumbnailImageFromFile(path + "\\" + this.Channel.ToString());
        }

        /// <summary>
        /// Loads the specified image and sets it as the thumbnail
        /// </summary>
        /// <param name="filename">file to load from</param>
        public void LoadThumbnailImageFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                Image img = Image.FromStream(fs);
                fs.Close();
                fs.Dispose();
                this.Image = img;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (DeleteClick != null)
            {
                DeleteClick.Invoke(this, new EventArgs());
            }
        }
    }
}
