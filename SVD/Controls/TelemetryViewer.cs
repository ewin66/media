using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FutureConcepts.Media.Client;
using FutureConcepts.Media.Client.StreamViewer;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class TelemetryViewer : UserControl
    {
        public TelemetryViewer()
        {
            InitializeComponent();
        }

        private StreamViewerControl _viewer;

        public StreamViewerControl Viewer
        {
            get
            {
                return _viewer;
            }
            set
            {
                if (value != _viewer)
                {
                    if (_viewer != null)
                    {
                        _viewer.PropertyChanged -= new PropertyChangedEventHandler(_viewer_PropertyChanged);
                    }

                    _viewer = value;

                    if (_viewer != null)
                    {
                        _viewer.PropertyChanged += new PropertyChangedEventHandler(_viewer_PropertyChanged);

                        if (((_viewer.State != StreamState.Playing) && 
                             (_viewer.State != StreamState.Available)) ||
                            ((_viewer.State == StreamState.Playing) &&
                             (_viewer.AverageBitRate <= 0)))
                        {
                            Latency = _viewer.Latency;
                        }
                        else
                        {
                            BitRate = _viewer.AverageBitRate;
                        }
                        Users = _viewer.Users;
                        State = _viewer.State;
                        RefreshDateTime();
                    }
                }
            }
        }

        void _viewer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new PropertyChangedEventHandler(_viewer_PropertyChanged), new object[] { sender, e });
                return;
            }

            switch (e.PropertyName)
            {
                case "Users":
                    Users = _viewer.Users;
                    break;
                case "AverageBitRate":
                    BitRate = _viewer.AverageBitRate;
                    break;
                case "Latency":
                    Latency = _viewer.Latency;
                    break;
                case "State":
                    State = _viewer.State;
                    if (_viewer.State == StreamState.Available)
                    {
                        BitRate = 0;
                        Users = null;
                    }
                    break;
                case "ProgressMessage":
                    lblStateValue.Text = _viewer.ProgressMessage;
                    break;
            }
            RefreshDateTime();
        }

        private static readonly string LatencyLabel = "Latency:";
        private static readonly string LatencyFormat = "0 ms";
        private static readonly string BitrateLabel = "Bitrate:";
        private static readonly string BitrateFormat = "0 kbps";

        private int _bitRate;
        /// <summary>
        /// The most recently reported BitRate
        /// </summary>
        public int BitRate
        {
            get
            {
                return _bitRate;
            }
            set
            {
                lblBitrate.Text = BitrateLabel;

                _bitRate = value;
                if (_bitRate <= 0)
                {
                    lblBitrateValue.Text = "-";
                }
                else
                {
                    lblBitrateValue.Text = _bitRate.ToString(BitrateFormat);
                }
                RefreshDateTime();
            }
        }

        private int _latency;
        /// <summary>
        /// The most recently reported Latency
        /// </summary>
        public int Latency
        {
            get
            {
                return _latency;
            }
            set
            {
                lblBitrate.Text = LatencyLabel;

                _latency = value;

                if (_latency < 0)
                {
                    lblBitrateValue.Text = "-";
                }
                else
                {
                    lblBitrateValue.Text = _latency.ToString(LatencyFormat);
                }
                RefreshDateTime();
            }
        }



        private List<string> _users;
        /// <summary>
        /// A list of the currently connected users to this source
        /// </summary>
        public List<string> Users
        {
            get
            {
                return _users;
            }
            set
            {
                tt.SetToolTip(lblUsers, null);
                tt.SetToolTip(lblUsersValue, null);

                _users = value;
                if (_users != null)
                {
                    lblUsersValue.Text = (_users.Count == 0) ? "-" : _users.Count.ToString();
                    string tooltip = "Connected Users:\n";
                    foreach (string user in _users)
                    {
                        tooltip += "  " + user + "\n";
                    }
                    tt.SetToolTip(lblUsersValue, tooltip);
                    tt.SetToolTip(lblUsers, tooltip);
                }
                else
                {
                    lblUsersValue.Text = "-";
                }
                RefreshDateTime();
            }
        }

        private StreamState _streamState;
        /// <summary>
        /// The state of the currently attached stream
        /// </summary>
        public StreamState State
        {
            get
            {
                return _streamState;
            }
            set
            {
                if (value != _streamState)
                {
                    _streamState = value;
                    lblStateValue.Text = value.ToString();
                    tt.RemoveAll();
                    RefreshDateTime();
                }
            }
        }

        /// <summary>
        /// Updates the progress message (e.g. StreamState)
        /// </summary>
        public string ProgressMessage
        {
            set
            {
                lblStateValue.Text = value;
            }
        }
        
        /// <summary>
        /// Used to 
        /// </summary>
        private void RefreshDateTime()
        {
            lblUpdatedValue.Text = DateTime.Now.ToString("T");
            this.Invalidate();
        }
    }
}
