using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using FutureConcepts.Media.CommonControls;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// Represents a thumbnail/icon to be shown for easy access to a server/source
    /// </summary>
    public partial class Thumbnail : DefaultUserControl
    {
        /// <summary>
        /// Creates a new instance of a thumbnail, not pointing to any server or source
        /// </summary>
        public Thumbnail()
        {
            InitializeComponent();
            SetToolTip();
        }

        private ServerInfo _serverInfo;
        /// <summary>
        /// Gets or sets the server where this thumbnail points to
        /// </summary>
        public ServerInfo ServerInfo
        {
            get
            {
                return _serverInfo;
            }
            set
            {
                _serverInfo = value;
                SetToolTip();
            }
        }

        private StreamSourceInfo _sourceInfo;
        /// <summary>
        /// Gets or sets the source on the <see cref="P:FutureConcepts.Media.Client.StreamViewer.Thumbnail.ServerInfo"/>.
        /// </summary>
        public StreamSourceInfo SourceInfo
        {
            get
            {
                return _sourceInfo;
            }
            set
            {
                _sourceInfo = value;
                SetToolTip();
            }
        }

        /// <summary>
        /// Generates a tool tip describing the server and source
        /// </summary>
        private void SetToolTip()
        {
            if (ServerInfo != null && SourceInfo != null)
            {
                tt.SetToolTip(icon, "Server: " + ServerInfo.ServerName + "\nSource: " + SourceInfo.Description + " (" + SourceInfo.SourceName + ")");
            }
            else
            {
                tt.SetToolTip(icon, "Not attached to a source");
            }
        }

        private string _pictureFilename;
        /// <summary>
        /// The file name of the icon, relative to the app_data\thumbnails folder
        /// </summary>
        public string IconFilename
        {
            get
            {
                return _pictureFilename;
            }
            set
            {
                try
                {
                    _pictureFilename = value;
                    DirectoryInfo di = new DirectoryInfo(Assembly.GetEntryAssembly().Location);
                    icon.Image = new Bitmap(Path.GetDirectoryName(di.FullName) + @"\app_data\thumbnails\" + value);
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                }
            }
        }
/*
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                if (value)
                {
                    IconFilename = IconFilename;
                }
                else
                {
                    icon.Image = GetGrayscale(icon.Image);
                }
            }
        }*/

        /// <summary>
        /// Gets Grayscale image from a given filename
        /// </summary>
        /// <param name="colorImage">a colorized image</param>
        /// <returns>returns the grayscale equivelent</returns>
        private Image GetGrayscale(Image colorImage)
        {
            Bitmap gray = new Bitmap(colorImage);
            for (int y = 0; y < gray.Height; y++)
            {
                for (int x = 0; x < gray.Width; x++)
                {
                    Color c = gray.GetPixel(x, y);
                    gray.SetPixel(x, y, Color.FromArgb((c.R + c.G + c.B) / 3, (c.R + c.G + c.B) / 3, (c.R + c.G + c.B) / 3));
                }
            }
            return gray;
        }

        private void icon_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.ClientRectangle.Contains(e.Location) && (e.Button != MouseButtons.None))
                {
                    icon.Capture = false;
                    if (DragStarted != null)
                    {
                        Debug.WriteLine("Raise DragStarted");
                        DragStarted(this, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raised when the user begins to drag the icon
        /// </summary>
        public event MouseEventHandler DragStarted;
    }
}
