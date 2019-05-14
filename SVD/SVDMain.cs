using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.CommonControls;
using FutureConcepts.Media.Client.StreamViewer;
using FutureConcepts.Media.SVD.Controls;
using FutureConcepts.Tools;

using FutureConcepts.Settings;

namespace FutureConcepts.Media.SVD
{
    public partial class SVDMain : MainForm
    {
        private int myLeft = 0, myTop = 0;

        /// <summary>
        /// a multi-dimensional array to show where the SVCs lay in the TableLayoutPanel
        /// </summary>
        private readonly StreamViewerControl[][] Streams;

        public SVDMain(string[] args)
        {
            InitializeComponent();

            DragDropManager.Register(this);

            //assign positions to the streams
            Streams = new StreamViewerControl[2][];
            Streams[0] = new StreamViewerControl[2];
            Streams[1] = new StreamViewerControl[2];
            Streams[0][0] = StreamA;
            Streams[1][0] = StreamB;
            Streams[0][1] = StreamC;
            Streams[1][1] = StreamD;

            this.HasControlBox = FutureConcepts.Settings.SystemInfo.DoCheck();
            
            if (args.Length >= 2)
            {
                int.TryParse(args[0], out myLeft);
                int.TryParse(args[1], out myTop);
            }
        }

        private StreamViewerControl FindViewerMatching(StreamSourceInfo streamSourceInfo)
        {
            StreamViewerControl result = null;
            List<StreamViewerControl> list = new List<StreamViewerControl>();
            list.Add(StreamA);
            list.Add(StreamB);
            list.Add(StreamC);
            list.Add(StreamD);
            foreach (StreamViewerControl streamViewerControl in list)
            {
                if ((streamViewerControl.SourceInfo != null) && (streamViewerControl.SourceInfo.SinkAddress != null))
                {
                    if (streamViewerControl.SourceInfo.SinkAddress == streamSourceInfo.SinkAddress)
                    {
                        result = streamViewerControl;
                        break;
                    }
                }
            }
            return result;
        }

        private StreamViewerControl _activeStreamViewer;
        /// <summary>
        /// This property should only be set by Stream_PropertyChanged. To set an SVC active, set its Active property
        /// </summary>
        protected StreamViewerControl ActiveStreamViewer
        {
            get
            {
                return _activeStreamViewer;
            }
            set
            {
                if ((_activeStreamViewer != null) && (_activeStreamViewer != value))
                {
                    _activeStreamViewer.Active = false;
                }

                if (CurrentPalette != null)
                {
                    if (CurrentPalette.StreamViewerControl != value)
                    {
                        CurrentPalette = null;
                    }
                }

                _activeStreamViewer = value;
                telemetryViewer.Viewer = value;
                profileGroupSelector.StreamViewer = value;
                //show the profile group selector if there is an active stream that is also playing
                if (value != null)
                {
//                    Boolean isVisible = (value.Active && (value.State == StreamState.Playing) && value.ProfileGroupSelectorEnabled);
                    Boolean isVisible = true;
                    profileGroupSelector.Visible = isVisible;
                    profileGroupSelector.Enabled = profileGroupSelector.Visible;
                }
            }
        }

        private LoadingScreenManager _loadingScreen = null;
        /// <summary>
        /// Provides an easy to use wrapper for the loading screen manager.
        /// Set to "null" to close the loading screen.
        /// </summary>
        private LoadingScreenManager LoadingScreen
        {
            get
            {
                if (_loadingScreen == null)
                {
                    _loadingScreen = new LoadingScreenManager(this.Location);
                }
                return _loadingScreen;
            }
            set
            {
                if (value == null)
                {
                    _loadingScreen.DestroyLoadingScreen();
                    _loadingScreen = null;
                }
            }
        }

        private void SVDMain_Load(object sender, EventArgs e)
        {
            if (myLeft == -1279)
            {
                myLeft = -1280;
                TestUI ui = new TestUI();
                ui.ShowDialog(this);
            }

            this.Location = new Point(myLeft, myTop);
            Rectangle r = Screen.GetBounds(this);
            this.Size = new Size(r.Width, r.Height);

            MediaApplicationSettings settings = new MediaApplicationSettings();
            MediaApplicationSettings defaults = new MediaApplicationSettings();
            defaults.SetDefaults();
            if ((string.IsNullOrEmpty(settings.UserName)) || settings.UserName.Equals(defaults.UserName))
            {
                InitialSettingsDialog dialog = new InitialSettingsDialog();
                dialog.TopMost = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    settings.UserName = dialog.UserName;
                    settings.SaveSettings();
                }
                else
                {
                    this.Close();
                    return;
                }
            }

            LoadingScreen.Title = "Streaming Video Desktop";
            LoadingScreen.Text = "Loading Favorites...";
            favoriteStreams.Initialize();

            LoadingScreen.Text = "Configuring Viewers...";
            ConfigureViewer(StreamA, settings);
            ConfigureViewer(StreamB, settings);
            ConfigureViewer(StreamC, settings);
            ConfigureViewer(StreamD, settings);

            LoadingScreen.Text = "Initializing Handlers...";

            Microsoft.Win32.SystemEvents.SessionEnding += new Microsoft.Win32.SessionEndingEventHandler(SystemEvents_SessionEnding);

            LoadingScreen = null;
        }

        private void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            Debug.WriteLine("SystemEvents_SessionEnding");
            SVDMain_FormClosing(this, new FormClosingEventArgs(CloseReason.WindowsShutDown, false));
        }

        /// <summary>
        /// Configures a StreamViewerControl, just the way we like it (tm)
        /// </summary>
        /// <param name="control">SVC to configure</param>
        /// <param name="settings">settings to apply to the SVCs</param>
        private void ConfigureViewer(StreamViewerControl control, MediaApplicationSettings settings)
        {
            control.RecordingsPath = settings.Recordings;
            control.SnapshotPath = settings.Snapshots;
            control.SnapshotSoundFileName = @"app_data\sounds\snap.wav";

            control.PropertyChanged += new PropertyChangedEventHandler(Stream_PropertyChanged);
            control.StartControl += new EventHandler<SourceControlTypeEventArgs>(StreamViewer_StartControl);
            control.StopControl += new EventHandler<SourceControlTypeEventArgs>(StreamViewer_StopControl);
            control.FullScreenClicked += new EventHandler(Stream_FullScreenClicked);
            control.SingleViewClicked += new EventHandler(Stream_ViewLayoutChanged);
        }

        private void SVDMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //override the "X" button if a stream viewer is in full screen -- avoids unexpected (to the user) app quits
            if (ActiveStreamViewer != null)
            {
                if ((e.CloseReason == CloseReason.UserClosing) && (ActiveStreamViewer.FullScreen))
                {
                    Stream_FullScreenClicked(ActiveStreamViewer, new EventArgs());
                    e.Cancel = true;
                    return;
                }
            }
            
            CurrentPalette = null;

            Debug.WriteLine("SVDMain_FormClosing " + e.CloseReason);

            favoriteStreams.Shutdown();

            Microsoft.Win32.SystemEvents.SessionEnding -= new Microsoft.Win32.SessionEndingEventHandler(SystemEvents_SessionEnding);

            StopAllStreams();
        }

        private void Stream_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new PropertyChangedEventHandler(Stream_PropertyChanged), new object[] { sender, e });
                return;
            }

            StreamViewerControl sv = sender as StreamViewerControl;
            if (e.PropertyName == "Active")
            {
                if (sv.Active)
                {
                    ActiveStreamViewer = sv;
                }
                else if (sv.FullScreen)
                {
                    Stream_FullScreenClicked(sv, new EventArgs());
                }
                return;
            }
            if (e.PropertyName == "PendingConnection")
            {
                ConnectionChainDescriptor goingTo = sv.PendingConnection;
                if (goingTo != null)
                {
                    StreamViewerControl existingViewer = this.FindViewerMatching(goingTo.Source);
                    if (existingViewer != null)
                    {
                        StringBuilder message = new StringBuilder("You are already watching that camera.");
                        if (existingViewer.State == StreamState.Recording)
                        {
                            message.AppendLine("Also, it's also being recorded.");
                        }
                        message.AppendLine("");
                        message.AppendLine("Moving that camera to this viewer will stop the");
                        message.AppendLine("existing viewer (and recording if applicable)");
                        message.AppendLine("");
                        message.AppendLine("Are you sure you want to move the camera to this viewer?");
                        if (DialogResult.Yes == FCYesNoMsgBox.ShowDialog("Move camera here", message.ToString(), this))
                        {
                            existingViewer.Stop();
                        }
                        else
                        {
                            throw new OperationCanceledException();
                        }
                    }
                }
            }
            if (sv == ActiveStreamViewer)
            {
                switch (e.PropertyName)
                {
                    case "State":
                        profileGroupSelector.Visible = false;

                        switch (sv.State)
                        {
                            case StreamState.Connecting:
                            case StreamState.Buffering:
                            case StreamState.Stopping:
                                if (CurrentPalette != null)
                                {
                                    CurrentPalette = null;
                                }
                                break;
                            case StreamState.Playing:
//                                if (sv.ProfileGroupSelectorEnabled)
                                if (true)
                                {
                                    profileGroupSelector.Enabled = true;
                                    profileGroupSelector.Visible = true;
                                }
                                profileGroupSelector.StreamViewer = ActiveStreamViewer;
                                break;
                        }
                        break;
                }
            }
        }

        #region Palette Management

        private IControlPalette _currentPalette;
        /// <summary>
        /// Gets control palette currently in use. Null if no palette is open.
        /// Setting the palette Stops the previous palette.
        /// </summary>
        private IControlPalette CurrentPalette
        {
            get
            {
                return _currentPalette;
            }
            set
            {
                try
                {
                    if (_currentPalette != null)
                    {
                        //disable...
                        _currentPalette.StreamViewerControl.ControlState[_currentPalette.ControlType] = SourceControlTypeState.Inactive;
                        _currentPalette.Closed -= new EventHandler<SourceControlTypeEventArgs>(StreamViewer_StopControl);

                        //then stop
                        _currentPalette.Stop();
                    }

                    this.SuspendLayout();

                    _currentPalette = value;

                    if (_currentPalette != null)
                    {
                        if (!(_currentPalette is Control))
                        {
                            throw new InvalidOperationException("A palette must implement Control!");
                        }

                        int areaWidth = this.Width / 2;
                        int controlWidth = ((Control)_currentPalette).Width / 2;
                        int x = areaWidth - controlWidth;
                        if (x < 0)
                        {
                            x = 0;
                        }
                        ((Control)_currentPalette).Location = new Point(x, 0);

                        panelPalettes.Controls.Clear();
                        panelPalettes.Controls.Add(_currentPalette as Control);
                        ((Control)_currentPalette).Visible = true;
                        panelPalettes.Visible = true;
                    }
                    else
                    {
                        panelPalettes.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    panelPalettes.Visible = false;
                    throw ex;
                }
                finally
                {
                    this.ResumeLayout();
                }
            }
        }

        private BackgroundWorker _paletteWorker;
        /// <summary>
        /// This BackgroundWorker is for starting palettes up
        /// </summary>
        private BackgroundWorker PaletteWorker
        {
            get
            {
                if (_paletteWorker == null)
                {
                    _paletteWorker = new BackgroundWorker();
                    _paletteWorker.DoWork += new DoWorkEventHandler(paletteWorker_DoOpenPalette);
                    _paletteWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(paletteWorker_RunWorkerCompleted);
                }
                return _paletteWorker;
            }
        }

        void paletteWorker_DoOpenPalette(object sender, DoWorkEventArgs e)
        {
            try
            {
                IControlPalette p = e.Argument as IControlPalette;
                if(p == null)
                {
                    throw new ArgumentException("Argument to worker must be an IControlPalette");
                }

                p.Start();

                e.Result = p;
            }
            catch(Exception ex)
            {
                e.Result = new object[] { ex, e.Argument };
            }
        }

        void paletteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RunWorkerCompletedEventHandler(paletteWorker_RunWorkerCompleted), sender, e);
                return;
            }

            try
            {
                if (e.Error != null)
                {
                    throw e.Error;
                }

                IControlPalette p = e.Result as IControlPalette;
                if (p != null)
                {
                    CurrentPalette = p;
                    if (p.StreamViewerControl.FullScreen)
                    {
                        Stream_FullScreenClicked(p.StreamViewerControl, new EventArgs());
                    }
                    p.StreamViewerControl.Active = true;
                    p.Closed += new EventHandler<SourceControlTypeEventArgs>(StreamViewer_StopControl);

                    p.StreamViewerControl.ControlState[p.ControlType] = SourceControlTypeState.Active;
                }
                else
                {
                    //try to unwind the error, but if it gets crazy, just throw it out
                    object[] errorResult = (object[])e.Result;
                    if (errorResult != null)
                    {
                        if (errorResult.Length == 2)
                        {
                            Exception error = (Exception)errorResult[0];
                            p = (IControlPalette)errorResult[1];

                            p.StreamViewerControl.ControlState[p.ControlType] = SourceControlTypeState.Inactive;

                            string msg = error.Message;
                            //get fault specific message if available
                            string fcMsg = (error.InnerException != null) ? error.InnerException.GetFaultInnerDetailMessage() : "";
                            if (!string.IsNullOrEmpty(fcMsg))
                            {
                                msg += Environment.NewLine + Environment.NewLine + fcMsg;
                            }

                            msg += Environment.NewLine + Environment.NewLine + "Message from:" + Environment.NewLine +
                                   p.StreamViewerControl.CurrentConnection.GetDescription();

                            FCMessageBox.Show("Cannot access " + p.ControlType.ToString() + " control", msg, this);
                            ErrorLogger.DumpToDebug(error);
                        }
                    }
                }
            }
            catch (Exception ex)    //something terrible has happened...
            {
                FCMessageBox.Show("Cannot access control", ex.Message, this);
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void StreamViewer_StartControl(object sender, SourceControlTypeEventArgs e)
        {
            StreamViewerControl svc = sender as StreamViewerControl;
            if (svc == null)
            {
                return;
            }

            IControlPalette p = null;

            switch (e.ControlType)
            {
                case SourceControlTypes.PTZ:
                    p = new PTZPalette();
                    break;
                case SourceControlTypes.Microwave:
                    p = new MicrowavePalette();
                    break;
                default:
                    return;
            }

            svc.Active = true;
            p.StreamViewerControl = svc;

            if (!PaletteWorker.IsBusy)
            {
                PaletteWorker.RunWorkerAsync(p);
            }
            else
            {
                FCMessageBox.Show("Please wait", "Another peripheral control is in the process of starting." + Environment.NewLine + "Please wait for it to finish.", this);
                svc.ControlState[e.ControlType] = SourceControlTypeState.Inactive;
            }
        }

        private void StreamViewer_StopControl(object sender, SourceControlTypeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<SourceControlTypeEventArgs>(StreamViewer_StopControl), new object[] { sender, e });
                return;
            }

            if (sender is StreamViewerControl)
            {
                ((StreamViewerControl)sender).ControlState[e.ControlType] = SourceControlTypeState.Inactive;
            }
            else if (sender is IControlPalette)
            {
                ((IControlPalette)sender).StreamViewerControl.ControlState[e.ControlType] = SourceControlTypeState.Inactive;
            }
            else
            {
                return;
            }            
            CurrentPalette = null;
        }

        #endregion

        private void StopAllStreams()
        {
            StreamA.Stop();
            StreamB.Stop();
            StreamC.Stop();
            StreamD.Stop();
        }

        //these member variables are used to resize StreamViewerControls
        private Point _originalLocation;
        private Size _originalSize;
        private DockStyle _originalDock;
        private Control _originalParent;

        /// <summary>
        /// Handles the resizing of a StreamViewer.StreamViewer to fill the entire SVD window
        /// </summary>
        /// <param name="sender">the StreamViewerControl to change state</param>
        /// <param name="e">don't care</param>
        private void Stream_FullScreenClicked(object sender, EventArgs e)
        {
            StreamViewerControl sViewer = sender as StreamViewerControl;
            if (sViewer == null)
                return;

            if (sViewer.FullScreen)
            {
                //restore to original placement

                //suspend layout
                panel6.Parent.SuspendLayout();
                _originalParent.SuspendLayout();

                //DO layout
                sViewer.Dock = _originalDock;
                panel6.Dock = DockStyle.Fill;
                sViewer.Parent = _originalParent;
                sViewer.Location = _originalLocation;
                sViewer.Size = _originalSize;
                sViewer.FullScreen = false;

                //resume layout
                _originalParent.ResumeLayout();
                panel6.Parent.ResumeLayout();

            }
            else
            {
                //expand to fill form
                //save original data
                _originalLocation = sViewer.Location;
                _originalSize = sViewer.Size;
                _originalDock = sViewer.Dock;
                _originalParent = sViewer.Parent;

                //suspend layout
                panel6.Parent.SuspendLayout();
                _originalParent.SuspendLayout();

                //DO layout

                panel6.Dock = DockStyle.Top;
                panel6.Height = 0;

                sViewer.Parent = panel6.Parent;

                sViewer.Location = new Point(0, 0);
                sViewer.Dock = DockStyle.Fill;
                sViewer.FullScreen = true;

                //resume layout
                panel6.Parent.ResumeLayout();
                _originalParent.ResumeLayout();

                //this is neccesary so we know what stream is full-screen
                sViewer.Active = true;
            }
        }

        /// <summary>
        /// This method handles changing between 4-way and Single
        /// </summary>
        private void Stream_ViewLayoutChanged(object sender, EventArgs e)
        {
            StreamViewerControl svc = (StreamViewerControl)sender;

            panel4Way.SuspendLayout();

            svc.SingleView = !svc.SingleView;

            for (int x = 0; x < Streams.Length; x++)
            {
                for (int y = 0; y < Streams[x].Length; y++)
                {
                    if (!svc.SingleView)
                    {
                        Streams[x][y].Parent.Visible = true;
                        panel4Way.ColumnStyles[x].Width = 50.0f;
                        panel4Way.RowStyles[y].Height = 50.0f;
                    }
                    else
                    {
                        Streams[x][y].Parent.Visible = Streams[x][y].SingleView;
                        if (Streams[x][y].SingleView)
                        {
                            panel4Way.ColumnStyles[x].Width = 100.0f;
                            panel4Way.RowStyles[y].Height = 100.0f;
                        }
                        else
                        {
                            Streams[x][y].Stop();

                            if (panel4Way.ColumnStyles[x].Width < 100.0f)
                            {
                                panel4Way.ColumnStyles[x].Width = 0.0f;
                            }
                            if (panel4Way.RowStyles[y].Height < 100.0f)
                            {
                                panel4Way.RowStyles[y].Height = 0.0f;
                            }
                        }
                    }
                }
            }

            panel4Way.ResumeLayout();

            svc.Active = true;
        }

        /// <summary>
        /// Implements pressing ESC to leave full screen
        /// </summary>
        private void SVDMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (ActiveStreamViewer != null)
            {
                if ((e.KeyCode == Keys.Escape) && (ActiveStreamViewer.FullScreen))
                {
                    Stream_FullScreenClicked(ActiveStreamViewer, new EventArgs());
                    e.Handled = true;
                }
            }
        }
    }
}
