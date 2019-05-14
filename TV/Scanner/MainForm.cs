using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using FutureConcepts.Media.DirectShowLib;

using FutureConcepts.Media.TV.Scanner.Properties;
using FutureConcepts.Media.TV.Scanner.Config;
using FutureConcepts.Media.TV.Scanner.RemoteControls;
using FutureConcepts.Controls.AntaresX.AntaresXForms.LoadScreen;
using FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons;
using FutureConcepts.SystemTools.Settings.AntaresX.AntaresXSettings;
using FutureConcepts.Controls.AntaresX.AntaresXForms;
using FutureConcepts.Tools;
using FutureConcepts.Tools.Utilities;

namespace FutureConcepts.Media.TV.Scanner
{
    public partial class MainForm : FutureConcepts.Controls.AntaresX.AntaresXForms.MainForm
    {
        #region Properties / Members

        private int left = 0;
        private int top = 0;

        private LoadingScreenManager _loadingScreen = null;
        /// <summary>
        /// Maintains a "singleton" loading screen
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
                    if (_loadingScreen != null)
                    {
                        //remove loading screen
                        _loadingScreen.DestroyLoadingScreen();
                        _loadingScreen = null;
                    }
                }
            }
        }

        /// <summary>
        /// The DirectShow graph we are using this time.
        /// </summary>
        private TVGraphs.BaseGraph DSGraph { get; set; }

        private Stack<Channel> _channelStack = new Stack<Channel>();

        private RemoteControls.HCWRemote _hcwRemote;

        private static SourcesConfig _config;
        /// <summary>
        /// Retrevies the SourcesConfig for this application
        /// </summary>
        private static SourcesConfig SourcesConfig
        {
            get
            {
                if (_config == null)
                {
                    _config = SourcesConfig.LoadFromFile();
                }
                return _config;
            }
        }

        /// <summary>
        /// The currently configured capture source
        /// </summary>
        private Source CurrentSource { get; set; }

        /// <summary>
        /// The channel that was in use when a channel scan began
        /// </summary>
        private Channel _holdChannel;
        /// <summary>
        /// holds a reference to the currently in use channel scanner
        /// </summary>
        private ChannelScanner _channelScanner;
        
        private bool _autoSnapping = false;

        private Label DisconnectedLabel;

        /// <summary>
        /// Returns true if the screen occupied by the application is video-accelerated
        /// </summary>
        internal bool ScreenIsAccelerated
        {
            get
            {
                try
                {
                    SystemInfo si = new SystemInfo();
                    if (!string.IsNullOrEmpty(si.VideoAcceleratorDeviceName))
                    {
                        Screen occupied = Screen.FromPoint(this.Location);
                        List<Win32.DISPLAY_DEVICE> list = DisplayDevices.GetMatching(si.VideoAcceleratorDeviceName, true);
                        if (list.Count > 0)
                        {
                            Screen target = DisplayDevices.GetScreenForDevice(list[0]);
                            if (target.Equals(occupied))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                    return false;
                }
            }
        }

        #endregion

        public MainForm(string[] args)
        {
            InitializeComponent();
            if (args.Length == 2)
            {
                left = int.Parse(args[0]);
                top = int.Parse(args[1]);
            }
            
            this.HasControlBox = SystemInfo.DoCheck();

            sourceInputSelector.Enabled = true;

            DisconnectedLabel = new Label();
            DisconnectedLabel.Text = "Disconnected." + Environment.NewLine + "Double-Click Network to reconnect.";
            DisconnectedLabel.TextAlign = ContentAlignment.MiddleCenter;
            DisconnectedLabel.Dock = DockStyle.Fill;
            DisconnectedLabel.Font = new Font(FontFamily.GenericSansSerif, 15);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            if (processes.Length != 1) // more than one process is running.
            {
                Thread.Sleep(1000); // wait 1 second to make sure the app has closed
                processes = Process.GetProcessesByName(current.ProcessName);
                if (processes.Length != 1)
                {
                    FCMessageBox.Show("Program already running!", "The TV Scanner application is already running!");
                    this.Visible = false;
                    this.Close();
                    this.Dispose();
                    return;
                }
            }

            try
            {
                this.Location = new Point(left, top);
                Rectangle r = Screen.GetBounds(this);
                this.Size = new Size(r.Width, r.Height);

                //clear the channel number so nothing looks wonky
                lblCurrentChannel.Text = "-";

                LoadingScreen.Text = "Initializing Remote Control support";
                LoadRemoteControl();

                bool scanForChannels = false;

                LoadingScreen.Text = "Searching for local TV Tuners...";
                //do an initial check for TV Tuner devices.
                VideoInputDeviceList deviceList = new VideoInputDeviceList();

                if ((!AppUser.Current.TunerDeviceName.Equals(VideoInputDeviceList.NetworkDeviceName)) &&
                    (!deviceList.HasDevice(AppUser.Current.TunerDeviceName)) &&
                    (deviceList.Items.Count > 1))
                {
                    FCMessageBox.Show("Local tuner changed!",
                                      "Your local tuner card appears to have changed. TV Scanner will now use this card:" +
                                      Environment.NewLine +
                                      deviceList.Items[0] +
                                      Environment.NewLine + Environment.NewLine +
                                      "If this is unexpected, please contact Future Concepts.", this);

                    AppUser.Current.TunerDeviceName = deviceList.Items[0];
                    AppUser.Current.SaveSettings();
                    scanForChannels = true;
                }
                else if (( ((deviceList.Items.Count == 1) && (deviceList.HasNetworkDevice)) || 
                            (deviceList.Items.Count == 0)) &&
                          !AppUser.Current.TunerDeviceName.Equals(VideoInputDeviceList.NetworkDeviceName))
                {
                    FCMessageBox.Show("No local tuner found!",
                                      "Could not find any compatible TV Tuner card." + 
                                      Environment.NewLine + Environment.NewLine + 
                                      "Please run the configuration tool.", this);
                    this.Close();
                    return;
                }
  
                LoadingScreen.Text = "Loading Source Configuration...";

                sourceInputSelector.Enabled = false;

                CurrentSource = SourcesConfig[AppUser.Current.TunerDeviceName];

                sourceInputSelector.SetSourceAvailable(TVSource.Network, AppUser.Current.ServerAddress != string.Empty);

                if (AppUser.Current.TunerDeviceName.Equals(VideoInputDeviceList.NetworkDeviceName))
                {
                    sourceInputSelector.SetSourceAvailable(TVSource.LocalAnalog, false);
                    sourceInputSelector.SetSourceAvailable(TVSource.LocalDigital, false);
                }
                else
                {
                    sourceInputSelector.SetSourceAvailable(TVSource.LocalAnalog, CurrentSource.HasGraphFor(TVSource.LocalAnalog));
                    sourceInputSelector.SetSourceAvailable(TVSource.LocalDigital, CurrentSource.HasGraphFor(TVSource.LocalDigital));
                }

                if (CurrentSource.Name.Equals(VideoInputDeviceList.NetworkDeviceName))
                {
                    sourceInputSelector.SelectedSource = TVSource.Network;
                }
                else
                {
                    sourceInputSelector.SelectedSource = AppUser.Current.TVSource;
                }

                sourceInputSelector.Enabled = true;

                sourceInputSelector.SelectedInput = AppUser.Current.TVMode;

                LoadingScreen.Text = "Initializing Playback...";

                PrepareToSwitchGraph(sourceInputSelector.SelectedSource, LoadingScreen);

                try
                {
                    CreateDirectShowGraph(null);
                }
                catch (Exception ex)
                {
                    DestroyDirectShowGraph(true);
                    if (sourceInputSelector.SelectedSource == TVSource.Network)
                    {
                        FCMessageBox.Show("Error connecting to Network TV.",
                                          "You may double-click the Network icon to attempt a reconnect once the program is started." +
                                          Environment.NewLine + Environment.NewLine + ex.Message, this);
                    }
                    else
                    {
                        throw;
                    }
                }

                FinishSwitchingGraph(sourceInputSelector.SelectedSource);            

                LoadingScreen.Text = "Initializing Audio...";

                //Set Audio Properties
                trackVolume.Value = AppUser.Current.Volume;
                trackVolume_ValueChanged(this, new EventArgs());
                chkMute.Checked = AppUser.Current.Mute;
                chkMute_CheckedChanged(this, new EventArgs());

                LoadingScreen.Text = "Loading Known Channels...";
                //technically, the channels are already loaded in CreateDirectShowGraph
                if (DSGraph is TVGraphs.BaseLocalTuner)
                {
                    if (((TVGraphs.BaseLocalTuner)DSGraph).KnownChannels.Count <= 0)
                    {
                        scanForChannels = true;
                    }
                }

                //force a channel scan, if required
                if ((scanForChannels) && 
                    (sourceInputSelector.SelectedSource != TVSource.Network) &&
                    (sourceInputSelector.SelectedInput != TVMode.Satellite))
                {
                    LoadingScreen.Text = "Starting Channel Scan...";
                    StartChannelDiscovery();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                FCMessageBox.Show("TV Scanner Encountered Error!", ex.Message);
                this.Close();
                return;
            }
            finally
            {
                LoadingScreen = null;
            }
        }

        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            LoadingScreen.Title = "Closing TV Scanner...";
            LoadingScreen.Text = "Saving Favorite Channels...";
            //save favorites
            favoriteChannels.SaveFavoritesToFile();
            LoadingScreen.Text = "Stop Remote Control...";
            SaveKnownChannels();
            //save favorites
            favoriteChannels.SaveFavoritesToFile();

            if (_hcwRemote != null)
            {
                _hcwRemote.Dispose();
            }

            DestroyDirectShowGraph(true);
            if (sourceInputSelector.SelectedInput != AppUser.Current.TVMode)
            {
                AppUser.Current.TVMode = sourceInputSelector.SelectedInput;
            }
            if (sourceInputSelector.SelectedSource != AppUser.Current.TVSource)
            {
                AppUser.Current.TVSource = sourceInputSelector.SelectedSource;
            }
            if (trackVolume.Value != AppUser.Current.Volume)
            {
                AppUser.Current.Volume = (int)trackVolume.Value;
            }
            if (chkMute.Checked != AppUser.Current.Mute)
            {
                AppUser.Current.Mute = chkMute.Checked;
            }
            AppUser.Current.SaveSettings();
        }

        #region UI Maintainence

        /// <summary>
        /// Enables or disables all of the channel controls
        /// </summary>
        /// <param name="val">true if channel/audio controls should be enabled, false if disabled</param>
        private void SetChannelControlsEnabled(bool val)
        {
            btnChannelUp.Visible = val && (sourceInputSelector.SelectedInput != TVMode.Satellite);
            btnChannelDown.Visible = val && (sourceInputSelector.SelectedInput != TVMode.Satellite);
            channelKeypad.Visible = val && (sourceInputSelector.SelectedInput != TVMode.Satellite);
            btnShowHideFavoriteChannels.Enabled = val;
            favoriteChannels.Enabled = val;

            if ((DSGraph is TVGraphs.BaseLocalTuner) || (DSGraph is IChannelScanProvider))
            {
                btnChannelExplorer.Visible = val && (sourceInputSelector.SelectedInput != TVMode.Satellite);
            }
            else
            {
                btnChannelExplorer.Visible = false;
            }
            
            SetAudioControlsEnabled(val);
        }

        /// <summary>
        /// Sets the controls for satellite mode.
        /// </summary>
        /// <param name="val">true to come out of satellite mode, false to go into satellite mode</param>
        private void SetSatelliteControlsEnabled(bool val)
        {

            btnChannelUp.Visible = val && (DSGraph != null);
            btnChannelDown.Visible = val && (DSGraph != null);
            channelKeypad.Visible = val && (DSGraph != null);
            if ((DSGraph is TVGraphs.BaseLocalTuner) || (DSGraph is IChannelScanProvider))
            {
                btnChannelExplorer.Visible = val;
            }
            else
            {
                btnChannelExplorer.Visible = false;
            }

            panelFavoriteChannelsHeader.Visible = val;
            favoriteChannels.Visible = ShowFavoriteChannels && val;
        }

        /// <summary>
        /// Handles the enabled/disabled state of the audio controls
        /// </summary>
        /// <param name="enable">true - enable the controls, false - disable the controls</param>
        private void SetAudioControlsEnabled(bool enable)
        {
            chkMute.Enabled = enable;
            trackVolume.Enabled = enable && !chkMute.Checked;
            volUp.Enabled = enable && !chkMute.Checked;
            volDown.Enabled = enable && !chkMute.Checked;
        }

        private bool _showFavoriteChannels = true;

        public bool ShowFavoriteChannels
        {
            get
            {
                return _showFavoriteChannels;
            }
            set
            {
                _showFavoriteChannels = value;
                favoriteChannels.Visible = value;

                btnShowHideFavoriteChannels.ImageList = value ? ilHideFavoriteChannels : ilShowFavoriteChannels;
            }
        }

        private void btnShowHideFavoriteChannels_Click(object sender, EventArgs e)
        {
            ShowFavoriteChannels = !ShowFavoriteChannels;
        }

        #endregion

        #region Changing the Channel

        /// <summary>
        /// Displays a channel number that has not been tuned yet
        /// </summary>
        /// <param name="channel">channel that possibly will be tuned</param>
        private void UpdateCurrentChannelTentative(Channel channel)
        {
            lblCurrentChannel.ForeColor = Color.Yellow;
            if (channel == null)
            {
                lblCurrentChannel.Text = "-";
            }
            else
            {
                lblCurrentChannel.Text = channel.ToString();
            }
        }

        /// <summary>
        /// Displays a channel number that has been successfully tuned.
        /// </summary>
        /// <param name="channel">the channel that is currently tuned</param>
        private void UpdateCurrentChannel(Channel channel)
        {
            channelKeypad.Channel = channel;
            favoriteChannels.Channel = channel;

            tt.SetToolTip(lblCurrentChannel, null);

            if (sourceInputSelector.SelectedInput != TVMode.Satellite)
            {
                if (channel != null)
                {
                    lblCurrentChannel.ForeColor = Color.LimeGreen;
                    lblCurrentChannel.Text = channel.ToString();
                    if (!string.IsNullOrEmpty(channel.Callsign))
                    {
                        tt.SetToolTip(lblCurrentChannel, channel.Callsign);
                        ttChannel.Hide(this);
                        ttChannel.Show(channel.Callsign,
                                       this,
                                       panelCenterFrame.Left + 10,
                                       panelCenterFrame.Location.Y + panelCenterFrame.Height,
                                       2000);
                    }
                    else
                    {
                        tt.SetToolTip(lblCurrentChannel, channel.ToString());
                    }
                }
                else
                {
                    lblCurrentChannel.ForeColor = Color.Yellow;
                    lblCurrentChannel.Text = "-";
                }
            }

            if (DSGraph != null)
            {
                if (DSGraph.ChannelForceChange)
                {
                    DSGraph.ChannelForceChange = false;
                }
            }
        }

        /// <summary>
        /// Draws the channel callsign popup
        /// </summary>
        private void ttChannel_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.Graphics.DrawString(e.ToolTipText,
                                  lblCurrentChannel.Font,
                                  Brushes.White,
                                  e.Bounds.X,
                                  e.Bounds.Y);
        }

        /// <summary>
        /// Determines the size required to show the channel callsign popup
        /// </summary>
        private void ttChannel_Popup(object sender, PopupEventArgs e)
        {
            string text = tt.GetToolTip(lblCurrentChannel);
            Graphics g = lblCurrentChannel.CreateGraphics();
            SizeF size = g.MeasureString(text, lblCurrentChannel.Font);
            g.Dispose();
            e.ToolTipSize = new Size((int)size.Width, (int)size.Height);
            e.Cancel = false;
        }

        /// <summary>
        /// To be called before every channel change.
        /// </summary>
        private void BeginChannelChange()
        {
            BeginChannelChange(false);
        }

        /// <summary>
        /// To be called before every channel change.
        /// </summary>
        /// <param name="stopScanning">If true, any favorites scanning in progress will be aborted.</param>
        private void BeginChannelChange(bool stopScanning)
        {
            if (DSGraph == null)
            {
                return;
            }

            Channel current = DSGraph.Channel;

            if (stopScanning)
            {
                favoriteChannels.Scanning = false;
            }

            _channelStack.Push(current);

            if (favoriteChannels.IsFavorite(current))
            {
                Snapshot snap = DSGraph.CreateSnapshot();
                if (snap != null)
                {
                    favoriteChannels.UpdateSnapshot(current, snap);
                    snap.Dispose();
                }
            }
        }

        private void btnChannelUp_Click(object sender, EventArgs e)
        {
            try
            {
                BeginChannelChange();
                DSGraph.ChannelForceChange = true;
                DSGraph.ChannelUp();
                DSGraph.ChannelForceChange = false;
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Exception in btnChannelUp_Click: " + exc.Message);
            }
        }

        private void btnChannelDown_Click(object sender, EventArgs e)
        {
            try
            {
                BeginChannelChange();
                DSGraph.ChannelForceChange = true;
                DSGraph.ChannelDown();
                DSGraph.ChannelForceChange = false;
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Exception in btnChannelDown_Click: " + exc.Message);
            }
        }

        private void channelKeypad_ChannelChanging(object sender, EventArgs e)
        {
            favoriteChannels.Scanning = false;

            UpdateCurrentChannelTentative(channelKeypad.Channel);
        }

        private void channelKeypad_ChannelEntered(object sender, EventArgs e)
        {
            try
            {
                if (DSGraph == null)
                {
                    return;
                }

                BeginChannelChange();

                DSGraph.ChannelForceChange = true;
                DSGraph.Channel = channelKeypad.Channel;
                UpdateCurrentChannel(DSGraph.Channel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in ChannelEntered: " + ex.Message);
            }
        }

        #endregion  

        #region DirectShow Graph

        /// <summary>
        /// Attempts to create a directshow graph to render the current source.
        /// </summary>
        /// <param name="captureFilename">file name to render capture to. or <c>null</c> to not record.</param>
        /// <param name="errorMessage">
        /// if an exception occurs within this method, its message is available through this parameter.
        /// this string will be "" if <c>true</c> is returned, or the exception's message if <c>false</c> is returned.
        /// </param>
        /// <returns><c>true</c> if the graph was created successfully. <c>false</c> if it could not be done.</returns>
        private void CreateDirectShowGraph(string captureFilename)
        {
            if (CurrentSource == null)
            {
                throw new ArgumentNullException("currentSource must not be null!");
            }

            DestroyDirectShowGraph(false);

            panelMainPreview.Controls.Remove(DisconnectedLabel);

            LoadingScreen.Text = "Initializing Playback...";
            DSGraph = TVGraphs.BaseGraph.CreateInstance(CurrentSource[sourceInputSelector.SelectedSource], panelMainPreview);
            if (DSGraph != null)
            {
                LoadingScreen.Text = "Rendering...";
                DSGraph.PropertyChanged += new PropertyChangedEventHandler(DSGraph_PropertyChanged);

                DSGraph.ChannelForceChange = true;

                DSGraph.Render();
               
                trackVolume.Minimum = DSGraph.MinVolume;
                trackVolume.Maximum = DSGraph.MaxVolume;

                if (captureFilename != null)
                {
                    DSGraph.RenderCapture(captureFilename);
                }

                LoadingScreen.Text = "Attaching...";
                IChannelScanProvider scannableGraph = DSGraph as IChannelScanProvider;
                if (scannableGraph != null)
                {
                    scannableGraph.ChannelScanStarted += new EventHandler(ChannelScanStarted);
                    scannableGraph.ChannelScanProgressUpdate += new EventHandler(ChannelScanProgressUpdate);
                    scannableGraph.ChannelScanComplete += new EventHandler<ChannelScanCompleteEventArgs>(ChannelScanComplete);
                }

                if (DSGraph is TVGraphs.LTNetSource)
                {
                    ((TVGraphs.LTNetSource)DSGraph).ConnectionFaulted += new EventHandler(Server_ConnectionFaulted);
                }

                LoadingScreen.Text = "Starting Playback...";

                DSGraph.Mute = chkMute.Checked;
                DSGraph.Volume = (int)trackVolume.Value;

                DSGraph.Run();

                DSGraph.TVMode = sourceInputSelector.SelectedInput;

                DSGraph.ChannelForceChange = false;
            }
        }

        /// <summary>
        /// Disposes any current instance of a directshow graph.
        /// </summary>
        /// <param name="closeLoadingScreen">
        /// If true, the loading screen will be closed after executing.
        /// If this call is part of a series of operations, set to false
        /// </param>
        private void DestroyDirectShowGraph(bool closeLoadingScreen)
        {
            try
            {
                if (DSGraph != null)
                {
                    LoadingScreen.Text = "Stopping Playback...";
                    if (DSGraph is TVGraphs.BaseLocalTuner)
                    {
                        ((TVGraphs.BaseLocalTuner)DSGraph).SaveKnownChannels();
                    }
                    DSGraph.Stop();


                    LoadingScreen.Text = "Release Resources...";
                    DSGraph.PropertyChanged -= new PropertyChangedEventHandler(DSGraph_PropertyChanged);

                    IChannelScanProvider scannableGraph = DSGraph as IChannelScanProvider;
                    if (scannableGraph != null)
                    {
                        scannableGraph.ChannelScanStarted -= new EventHandler(ChannelScanStarted);
                        scannableGraph.ChannelScanProgressUpdate -= new EventHandler(ChannelScanProgressUpdate);
                        scannableGraph.ChannelScanComplete -= new EventHandler<ChannelScanCompleteEventArgs>(ChannelScanComplete);
                    }

                    DSGraph.Dispose();
                    DSGraph = null;

                    panelMainPreview.Controls.Add(DisconnectedLabel);
                    panelMainPreview.Invalidate();
                    
                }
            }
            finally
            {
                if (closeLoadingScreen)
                {
                    LoadingScreen = null;
                }
            }
        }

        /// <summary>
        /// If the current graph has a known channels collection, save it.
        /// </summary>
        private void SaveKnownChannels()
        {
            if (DSGraph is TVGraphs.BaseLocalTuner)
            {
                ((TVGraphs.BaseLocalTuner)DSGraph).SaveKnownChannels();
            }
        }

        private void DSGraph_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DSGraph == null)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                EventHandler<PropertyChangedEventArgs> d = new EventHandler<PropertyChangedEventArgs>(DSGraph_PropertyChanged);
                this.Invoke(d, new object[] { sender, e });
                return;
            }

            switch (e.PropertyName)
            {
                case "Channel":
                    if (sourceInputSelector.SelectedInput == TVMode.Broadcast)
                    {
                        UpdateCurrentChannel(DSGraph.Channel);
                    }
                    else if(sourceInputSelector.SelectedInput == TVMode.Satellite)
                    {
                        lblCurrentChannel.Text = "SAT";
                        lblCurrentChannel.ForeColor = Color.LimeGreen;
                    }
                    break;
                case "TVMode":
                    sourceInputSelector.SelectedInput = DSGraph.TVMode;
                    break;
                case "Message":
                    LoadingScreen.Text = DSGraph.Message;
                    break;
            }
        }

        private void Server_ConnectionFaulted(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Delegate d = new EventHandler(Server_ConnectionFaulted);
                this.Invoke(d, new object[] { sender, e });
                return;
            }

            FCMessageBox.Show("Connection dropped.", "The connection to the remote server was lost." + Environment.NewLine + Environment.NewLine + "Double-click the Network button to try to reconnect.", this);
            DestroyDirectShowGraph(true);
        }

        #endregion

        #region Channel Discovery

        private bool preChannelScanMuteState = false;

        private void btnChannelExplorer_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                IVirtualChannelProvider vcp = this.DSGraph as IVirtualChannelProvider;
                if (vcp != null)
                {
                    List<Channel> curVirts = vcp.GetVirtualChannels();
                    foreach (Channel c in curVirts)
                    {
                        Debug.WriteLine(c.ToDebugString());
                    }
                }
            }
            else
            {
                try
                {
                    StartChannelDiscovery();
                }
                catch (Exception ex)
                {
                    FCMessageBox.Show("Could not scan for channels!", "A channel scan could not be started at this time." + Environment.NewLine + Environment.NewLine + ex.Message, this);
                }
            }
        }

        /// <summary>
        /// Begins the Channel Discovery process
        /// </summary>
        private void StartChannelDiscovery()
        {
            if (DSGraph == null)
            {
                throw new InvalidOperationException("Current tuner is not ready.");
            }

            if (DSGraph is IChannelScanProvider)
            {
                ((IChannelScanProvider)DSGraph).StartChannelScan();
            }
            else if (DSGraph is TVGraphs.BaseLocalTuner)
            {
                _channelScanner = new ChannelScanner(DSGraph as TVGraphs.BaseLocalTuner);
                _channelScanner.PropertyChanged += new PropertyChangedEventHandler(ChannelScanner_PropertyChanged);
                _channelScanner.ChannelScanStarted += new EventHandler(ChannelScanStarted);
                _channelScanner.ChannelScanProgressUpdate += new EventHandler(ChannelScanProgressUpdate);
                _channelScanner.ChannelScanComplete += new EventHandler<ChannelScanCompleteEventArgs>(ChannelScanComplete);
                _channelScanner.StartChannelScan();
            }
            else
            {
                throw new NotSupportedException("Current tuner does not support scanning for channels!");
            }
        }

        /// <summary>
        /// Stops any channel discovery in progress.
        /// </summary>
        private void StopChannelDiscovery()
        {
            IChannelScanProvider provider = (_channelScanner != null) ? _channelScanner : (DSGraph as IChannelScanProvider);
            if (provider != null)
            {
                provider.CancelChannelScan();
            }
            else
            {
                throw new NotSupportedException("Current tuner does not support scanning for channels!");
            }
        }

        private ScanningForm _channelScanDialog;
        /// <summary>
        /// Controls creation and disposing of a ScanningForm for server channel scanning
        /// </summary>
        private ScanningForm ChannelScanDialog
        {
            get
            {
                if (_channelScanDialog == null)
                {
                    _channelScanDialog = new ScanningForm();
                }
                return _channelScanDialog;
            }
            set
            {
                if (value == null)
                {
                    if (_channelScanDialog != null)
                    {
                        _channelScanDialog.Close();
                        _channelScanDialog.StopClicked -= new EventHandler(NetworkGraph_CancelChannelScan);
                        _channelScanDialog.StopClicked -= new EventHandler(LocalGraph_CancelChannelScan);
                        _channelScanDialog.Dispose();
                        _channelScanDialog = null;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when a channel scan is started
        /// </summary>
        /// <param name="sender">
        /// the text of the dialog is determined by the sender.
        /// If the sender is TVGraphs.LTNetSource, then it shows the server scanning dialog.
        /// Else, shows the local scanning dialog
        /// </param>
        private void ChannelScanStarted(object sender, EventArgs e)
        {
            if (sender is TVGraphs.LTNetSource)
            {
                ChannelScanDialog.Message = "The server is scanning for available channels.";
                ChannelScanDialog.StopClicked += new EventHandler(NetworkGraph_CancelChannelScan);
                ChannelScanDialog.StopButtonText = "Disconnect";
            }
            else
            {
                ChannelScanDialog.Message = "Scanning for available channels.";
                ChannelScanDialog.StopClicked += new EventHandler(LocalGraph_CancelChannelScan);
                ChannelScanDialog.StopButtonText = "Cancel";

                Channel temp = DSGraph.Channel;
                _holdChannel = new Channel(-1, temp.PhysicalChannel, temp.MajorChannel, temp.MinorChannel);
            }

            ChannelScanDialog.Show(this);

            //disable everything
            //HACK this is a work-around to prevent preChannelScanMuteState to be bound to the pointer DSGraph.Mute returns
            preChannelScanMuteState = DSGraph.Mute;
            DSGraph.Mute = true;
            SetChannelControlsEnabled(false);
            sourceInputSelector.Enabled = false;
            SetRecordControlsEnabled(false);
            favoriteChannels.Enabled = false;
        }

        /// <summary>
        /// Invoked when the channel scanning interface sends a progress update
        /// </summary>
        private void ChannelScanProgressUpdate(object sender, EventArgs e)
        {
            ChannelScanDialog.Progress = ((IChannelScanProvider)sender).ChannelScanProgress;
        }

        /// <summary>
        /// Invoked when a channel scan has been completed.
        /// Does any house keeping neccesary to get the user back to watching TV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">additional info about the process</param>
        private void ChannelScanComplete(object sender, ChannelScanCompleteEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Delegate d = new EventHandler<ChannelScanCompleteEventArgs>(ChannelScanComplete);
                this.Invoke(d, new object[] { sender, e });
                return;
            }

            ChannelScanDialog = null;

            //re-enable everything
            if (DSGraph != null)
            {
                DSGraph.Mute = preChannelScanMuteState;
            }
            SetChannelControlsEnabled(true);
            sourceInputSelector.Enabled = true;
            SetRecordControlsEnabled(true);
            favoriteChannels.Enabled = true;

            if (_channelScanner != null)
            {
                _channelScanner.Dispose();
                _channelScanner = null;
            }

            if (DSGraph is TVGraphs.BaseLocalTuner)
            {
                TVGraphs.BaseLocalTuner tuner = DSGraph as TVGraphs.BaseLocalTuner;
                favoriteChannels.UpdateFavorites(tuner.KnownChannels);
                if (tuner is TVGraphs.AnalogTuner)
                {
                    ((TVGraphs.AnalogTuner)tuner).ChannelForceChange = true;
                }
                if (tuner.KnownChannels.Contains(_holdChannel))
                {
                    DSGraph.Channel = _holdChannel;
                }
                else if (tuner.KnownChannels.Count > 0)
                {
                    DSGraph.Channel = tuner.KnownChannels.Items[0];
                }
                if (tuner is TVGraphs.AnalogTuner)
                {
                    ((TVGraphs.AnalogTuner)tuner).ChannelForceChange = false;
                }
            }

            UpdateCurrentChannel(DSGraph.Channel);


            btnRecord.Visible = true;

            if ((!e.Cancelled) && (e.ChannelsFound > 0) && (sender is TVGraphs.LTNetSource))
            {
                if (((TVGraphs.LTNetSource)sender).Connected != TVGraphs.LTNetSource.ConnectionState.Full)
                {
                    //HACK there may be a cleaner way to do this
                    sourceInputSelector_SelectedSourceChanged(this, new EventArgs());
                }
            }
            else if ((!e.Cancelled) && (e.ChannelsFound == 0))
            {
                string msg = "No valid channels were found during the scan. Try again later when ";
                if (sender is TVGraphs.LTNetSource)
                {
                    msg += "the server has";
                }
                else
                {
                    msg += "you have";
                }
                msg += " better TV reception.";

                FCMessageBox.Show("No channels found!", msg, this);

                if (sender is TVGraphs.LTNetSource)
                {
                    DestroyDirectShowGraph(true);
                }
            }
        }

        /// <summary>
        /// Invoked by the channel scanning dialog if the user clicks the Stop button during
        /// a network scan.
        /// </summary>
        private void NetworkGraph_CancelChannelScan(object sender, EventArgs e)
        {
            DestroyDirectShowGraph(true);
            LocalGraph_CancelChannelScan(sender, e);
            SetChannelControlsEnabled(false);
            SetRecordControlsEnabled(false);
            favoriteChannels.Enabled = false;
        }

        /// <summary>
        /// Invoked by the channel scanning dialog if the user clicks the Stop button during
        /// a local channel scan
        /// </summary>
        private void LocalGraph_CancelChannelScan(object sender, EventArgs e)
        {
            if (_channelScanner != null)
            {
                _channelScanner.CancelChannelScan();
            }

            ChannelScanDialog = null;

            //re-enable everything
            if (DSGraph != null)
            {
                DSGraph.Mute = preChannelScanMuteState;
            }
            SetChannelControlsEnabled(true);
            sourceInputSelector.Enabled = true;
            SetRecordControlsEnabled(true);
            favoriteChannels.Enabled = true;
        }

        /// <summary>
        /// Updates the tenative channel while the local channel scanner does its thing
        /// </summary>
        private void ChannelScanner_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Current")
            {
                UpdateCurrentChannelTentative(_channelScanner.Current);
            }
        }

        #endregion

        #region Recording Capabilities

        private void btnSnapshot_Click(object sender, EventArgs e)
        {
            try
            {
                if (DSGraph == null)
                {
                    return;
                }

                Snapshot snapshot = DSGraph.CreateSnapshot();
                snapshot.SaveAsFile(DateTime.Now.ToString("HHmmss") + @"_" + DSGraph.Channel + @".jpg");
                snapshot.Dispose();

                try
                {
                    if (!chkMute.Checked)
                    {
                        SoundPlayer player = new SoundPlayer(@"app_data\sounds\snap.wav");
                        player.Play();
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private int _autoSnapCount = 0;
        /// <summary>
        /// The current AutoSnapCount. If it is set to >= the maximum snapshot, it calls StopAutoSnapping
        /// </summary>
        public int AutoSnapCount
        {
            get
            {
                return _autoSnapCount;
            }
            set
            {
                _autoSnapCount = value;
                if (AppUser.Current.SnapshotMaximum == -1)
                {
                    return;
                }
                if (_autoSnapCount >= AppUser.Current.SnapshotMaximum)
                {
                    StopAutoSnapping();
                }
            }
        }

        private void btnAutoSnap_Click(object sender, EventArgs e)
        {
            if (_autoSnapping == false)
            {
                StartAutoSnapping();
            }
            else
            {
                StopAutoSnapping();
            }
        }

        /// <summary>
        /// Begins the Auto-Snapping mode
        /// </summary>
        private void StartAutoSnapping()
        {
            timerAutoSnap.Interval = AppUser.Current.SnapshotInterval * 1000;
            _autoSnapping = true;
            timerAutoSnap.Enabled = true;
            lblAutoSnapping.Visible = true;
            timerAutoSnap_Tick(this, new EventArgs());
        }

        /// <summary>
        /// Stops the Auto-Snapping mode.
        /// </summary>
        private void StopAutoSnapping()
        {
            timerAutoSnap.Enabled = false;
            _autoSnapping = false;
            lblAutoSnapping.Visible = false;
            btnAutoSnap.ImageIndex = 0;
            AutoSnapCount = 0;
        }

        private void timerAutoSnap_Tick(object sender, EventArgs e)
        {
            lblAutoSnapping.Visible = false;
            btnSnapshot_Click(this, new EventArgs());
            lblAutoSnapping.Visible = true;
            AutoSnapCount++;
        }
        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (btnRecord.Tag.ToString() == "Record")
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        /// <summary>
        /// Starts recording the video
        /// </summary>
        private void StartRecording()
        {
            try
            {
                if (DSGraph == null)
                {
                    return;
                }

                btnRecord.Enabled = false;

                //stop favorites scanning
                favoriteChannels.Scanning = false;

                //set blink mode
                Blinky |= BlinkMode.Recording;

                //update button
                btnRecord.ImageList = ilStopRec;
                btnRecord.Tag = "Stop\nRecording";


                if (sourceInputSelector.SelectedInput == TVMode.Broadcast)
                {
                    SetChannelControlsEnabled(false);
                }
                sourceInputSelector.Enabled = false;
                favoriteChannels.Enabled = false;
                SetAudioControlsEnabled(true);

                //construct path for recording
                ApplicationSettings settings = new ApplicationSettings();
                string dirPath = settings.Recordings + DateTime.Now.ToString("MMddyy") + @"\";
                Directory.CreateDirectory(dirPath);
                string filename = DateTime.Now.ToString("HHmmss") + @"_";
                if (sourceInputSelector.SelectedInput == TVMode.Broadcast)
                {
                    filename += DSGraph.Channel;
                }
                else
                {
                    filename += "SAT";
                }

                //create new graph to record with if neccesary, or setup graph to capture
                if (DSGraph.CaptureRequiresNewGraph)
                {
                    CreateDirectShowGraph(dirPath + filename);
                }
                else
                {
                    DSGraph.Stop();
                    DSGraph.RenderCapture(dirPath + filename);
                    DSGraph.Run();
                }

                LoadingScreen = null;
            }
            catch (Exception ex)
            {
                LoadingScreen = null;
                FCMessageBox.Show("Can't start recording!", ex.Message, this);
                ErrorLogger.DumpToDebug(ex);
                StopRecording();
            }
            finally
            {
                btnRecord.Enabled = true;
            }
        }

        /// <summary>
        /// Stops recording the video
        /// </summary>
        private void StopRecording()
        {
            if (DSGraph == null)
            {
                return;
            }

            bool stopCaptureFailed = false;
            Exception stopCaptureException = null;

            try
            {
                btnRecord.Enabled = false;

                //re-enable controls
                sourceInputSelector.Enabled = true;
                if (sourceInputSelector.SelectedInput == TVMode.Broadcast)
                {
                    SetChannelControlsEnabled(true);
                }
                sourceInputSelector.Enabled = true;
                favoriteChannels.Enabled = true;

                //set program state
                btnRecord.Tag = "Record";
                btnRecord.ImageList = ilRecord;
                Blinky = Blinky & ~BlinkMode.Recording;
                lblRecording.Visible = false;

                //stop capture
                DSGraph.StopCapture();
            }
            catch(Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                stopCaptureFailed = true;
                stopCaptureException = ex;
            }

            try
            {
                if ((DSGraph.CaptureRequiresNewGraph) || (stopCaptureFailed))
                {
                    Channel ch = DSGraph.Channel;
                    CreateDirectShowGraph(null);
                    DSGraph.ChannelForceChange = true;
                    DSGraph.Channel = ch;
                    DSGraph.ChannelForceChange = false;
                }
                else
                {
                    DSGraph.Run();
                }

                LoadingScreen = null;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                if (stopCaptureException != null)
                {
                    msg += Environment.NewLine + Environment.NewLine + 
                           "Additionally, an error occurred while stopping recording: " + 
                           Environment.NewLine + stopCaptureException.Message;
                }

                LoadingScreen = null;

                FCMessageBox.Show("Error resuming playback!", msg, this);
            }
            finally
            {
                btnRecord.Enabled = true;
            }
        }

        private void SetRecordControlsEnabled(bool enabled)
        {
            btnRecord.Enabled = enabled;
            btnSnapshot.Enabled = enabled;
            btnAutoSnap.Enabled = enabled;
        }

        #endregion

        #region Audio Control

        private void chkMute_CheckedChanged(object sender, EventArgs e)
        {
            if (DSGraph != null)
            {
                DSGraph.Mute = chkMute.Checked;

                volUp.Enabled = !chkMute.Checked;
                volDown.Enabled = !chkMute.Checked;
                trackVolume.Enabled = !chkMute.Checked;

                if (!chkMute.Checked)
                {
                    DSGraph.Volume = (int)trackVolume.Value;
                }
            }
        }

        /// <summary>
        /// Fired as the user is dragging -- forces the trackbar to snap to integer values and change as they go
        /// </summary>
        private void trackVolume_Scroll(object sender, EventArgs e)
        {
            trackVolume.Value = (int)Math.Round(trackVolume.CurrentValue);
        }

        private void trackVolume_ValueChanged(object sender, EventArgs e)
        {
            if (DSGraph != null)
            {
                DSGraph.Volume = (int)Math.Round(trackVolume.Value);
            }
        }

        private void volUp_Click(object sender, EventArgs e)
        {
            trackVolume.Value += trackVolume.TrackClickStepValue;
        }

        private void volDown_Click(object sender, EventArgs e)
        {
            trackVolume.Value -= trackVolume.TrackClickStepValue;
        }

        #endregion

        #region Source Selection

        private void sourceInputSelector_SelectedSourceChanged(object sender, EventArgs e)
        {
            favoriteChannels.Scanning = false;
            StopAutoSnapping();

            LoadingScreen.Text = "Loading Configuration...";
            PrepareToSwitchGraph(sourceInputSelector.SelectedSource, LoadingScreen);

            try
            {
                CreateDirectShowGraph(null);
                
                if (sourceInputSelector.SelectedSource == TVSource.LocalDigital)
                {
                    sourceInputSelector.SelectedInput = TVMode.Broadcast;
                }
            }
            catch (Exception ex)
            {
                LoadingScreen = null;   //get the loading screen out of the way of the dialog!

                FCMessageBox.Show("Unable to switch sources!", ex.Message, this);

                DestroyDirectShowGraph(false);
            }
            finally
            {
                LoadingScreen.Text = "Finishing switch...";
                FinishSwitchingGraph(sourceInputSelector.SelectedSource);
                LoadingScreen = null;
            }

            try
            {
                if ((DSGraph is TVGraphs.BaseLocalTuner) && (sourceInputSelector.SelectedInput != TVMode.Satellite))
                {
                    if (((TVGraphs.BaseLocalTuner)DSGraph).KnownChannels.Count <= 0)
                    {
                        StartChannelDiscovery();
                    }
                }
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Cannot start channel scan!", "A channel scan is required, but could not be started. Please try again." + Environment.NewLine + Environment.NewLine + ex.Message, this);
            }
        }

        /// <summary>
        /// Called directly before CreateDirectShowGraph if the user requests a switch.
        /// </summary>
        /// <remarks>
        /// This method should put the UI in any required state, and then also select the correct graph. (_currentSource)
        /// </remarks>
        /// <param name="targetSource">The source we are switching to.</param>
        /// <param name="statusDialog">The status dialog. Set the Title appropriately for the targetSource</param>
        private void PrepareToSwitchGraph(TVSource targetSource, LoadingScreenManager statusDialog)
        {
            
            //take the opportunity to save the favorites
            favoriteChannels.SaveFavoritesToFile();

            //save known channels of current graph
            SaveKnownChannels();

            switch (targetSource)
            {
                case TVSource.LocalAnalog:
                    statusDialog.Title = "Switching to Local Analog TV...";
                    CurrentSource = SourcesConfig[AppUser.Current.TunerDeviceName];
                    break;
                case TVSource.LocalDigital:
                    statusDialog.Title = "Switching to Local Digital TV...";
                    CurrentSource = SourcesConfig[AppUser.Current.TunerDeviceName];
                    break;
                case TVSource.Network:
                    statusDialog.Title = "Switching to Network TV...";
                    CurrentSource = SourcesConfig[VideoInputDeviceList.NetworkDeviceName];
                    UpdateCurrentChannel(null);
                    break;
            }
        }

        /// <summary>
        /// Called directly after CreateDirectShowGraph, if the user has requested a switch.
        /// </summary>
        /// <remarks>
        /// Should put the UI in the correct state for the given source.
        /// </remarks>
        /// <param name="targetSource">the source that was switched to.</param>
        private void FinishSwitchingGraph(TVSource targetSource)
        {
            try
            {
                switch (targetSource)
                {
                    case TVSource.LocalAnalog:
                        btnChannelExplorer.Visible = (DSGraph.TVMode != TVMode.Satellite);
                        ShowPerformanceWarning(false);
                        break;
                    case TVSource.LocalDigital:
                        btnChannelExplorer.Visible = true;
                        ShowPerformanceWarning(true);
                        break;
                    case TVSource.Network:
                        ShowPerformanceWarning(true);
                        break;
                }

                favoriteChannels.CurrentTVSource = targetSource;

                bool graphExists = (DSGraph != null);
                btnChannelExplorer.Enabled = graphExists;
                SetChannelControlsEnabled(graphExists);
                panelRecordingControls.Visible = graphExists;
                
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Called when the input type is changed.
        /// </summary>
        private void sourceInputSelector_SelectedInputChanged(object sender, EventArgs e)
        {
            switch (sourceInputSelector.SelectedInput)
            {
                case TVMode.Broadcast:
                    SetSatelliteControlsEnabled(true);
                    break;
                case TVMode.Satellite:

                    //HACK satellite is only supported in Analog/SVideo mode; change source if neccesary
                    if (sourceInputSelector.SelectedSource == TVSource.LocalDigital)
                    {
                        sourceInputSelector.SelectedSource = TVSource.LocalAnalog;
                    }

                    SetSatelliteControlsEnabled(false);
                    break;
            }

            //Update channel display, and tell the DirectShow graph what to do
            if (DSGraph != null)
            {
                if (sourceInputSelector.SelectedInput == TVMode.Broadcast)
                {
                    UpdateCurrentChannel(DSGraph.Channel);
                }
                DSGraph.TVMode = sourceInputSelector.SelectedInput;
            }

            if(sourceInputSelector.SelectedInput == TVMode.Satellite)
            {
                lblCurrentChannel.Text = "SAT";
                lblCurrentChannel.ForeColor = Color.LimeGreen;
            }
        }

        #endregion

        #region UI Performance Warning

        /// <summary>
        /// Shows the performance warning if neccesary
        /// </summary>
        /// <param name="ifNeccesary">true to show if neccesary, false to hide</param>
        private void ShowPerformanceWarning(bool ifNeccesary)
        {
            ttPerformanceWarning.Hide(panelMainPreview);
            if (ifNeccesary)
            {
                if (!this.ScreenIsAccelerated)
                {
                    bool neccesary = false;
                    if (sourceInputSelector.SelectedSource == TVSource.LocalDigital)
                    {
                        neccesary = true;
                    }
                    else if (sourceInputSelector.SelectedSource == TVSource.Network)
                    {
                        if (DSGraph is TVGraphs.LTNetSource)
                        {
                            Profile current = ((TVGraphs.LTNetSource)DSGraph).CurrentProfile;
                            if (current != null)
                            {
                                if (current.Video != null)
                                {
                                    neccesary = current.Video.CodecType == VideoCodecType.MPEG2;
                                }
                            }
                        }
                    }
                    if (neccesary)
                    {
                        ttPerformanceWarning.Show("Watching HDTV on this monitor may adversely affect the performance of the system. Please restart TV Scanner on the preferred monitor.", panelMainPreview, 5, 5, 86400000);
                    }
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ShowPerformanceWarning(true);
            if (DSGraph != null)
            {
                UpdateCurrentChannel(DSGraph.Channel);
            }
        }

        private void panelMainPreview_SizeChanged(object sender, EventArgs e)
        {
            ShowPerformanceWarning(true);
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            ShowPerformanceWarning(true);
        }

        #endregion

        #region UI BlinkMode

        private enum BlinkMode { None = 0, Scan = 1, Recording = 2 };
        /// <summary>
        /// Indicates the current blink mode. BlinkModes are not mutually exclusive, and can be OR'd together
        /// </summary>
        private BlinkMode _sonOfBlinky;
        private BlinkMode Blinky
        {
            get
            {
                return _sonOfBlinky;
            }
            set
            {
                _sonOfBlinky = value;
                tmrBlink.Enabled = (value != BlinkMode.None);
                if (value == BlinkMode.None)
                {
                    lblRecording.Visible = false;
                    lblScanning.Visible = false;
                }
            }
        }

        private void tmrBlink_Tick(object sender, EventArgs e)
        {
            if ((Blinky & BlinkMode.Recording) == BlinkMode.Recording)
            {
                lblRecording.Visible = !lblRecording.Visible;
            }
            if ((Blinky & BlinkMode.Scan) == BlinkMode.Scan)
            {
                lblScanning.Visible = !lblScanning.Visible;
            }
        }

        #endregion

        #region Remote Control

        /// <summary>
        /// Initalizes and loads the remote control, if specified in the settings.
        /// </summary>
        private void LoadRemoteControl()
        {
            if (AppUser.Current.RemoteType == "HCW")
            {
                try
                {
                    Process.Start(@"C:\Program Files\WinTV\IR.EXE", @"/QUIT");
                    _hcwRemote = new RemoteControls.HCWRemote();
                    _hcwRemote.KeyPressed += new EventHandler<RemoteControls.KeyPressedEventArgs>(HCWRemote_KeyPressed);
                    this.Controls.Add(_hcwRemote);
                }
                catch
                {
                    if (_hcwRemote != null)
                    {
                        _hcwRemote.Dispose();
                        _hcwRemote = null;
                    }
                }
            }
        }

        private void HCWRemote_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            NameValueCollection map = ConfigurationManager.GetSection("HCWRemoteControlMap") as NameValueCollection;
            if (map != null)
            {
                string actionName = map[e.PressedKeyName.ToString()];
                if (actionName != null)
                {
                    switch (actionName)
                    {

                        case "SendDigitToChannelKeypad":
                            channelKeypad.SendKey((int)e.PressedKeyName);
                            break;
                        case "MuteAudio":
                            chkMute.Checked = !chkMute.Checked;
                            break;
                        case "VolumeUp":
                            volUp_Click(volUp, new EventArgs());
                            break;
                        case "VolumeDown":
                            volDown_Click(volDown, new EventArgs());
                            break;
                        case "ChannelUp":
                            btnChannelUp_Click(this, e);
                            break;
                        case "ChannelDown":
                            btnChannelDown_Click(this, e);
                            break;
                        case "PreviousChannel":
                            if (_channelStack.Count > 0)
                            {
                                DSGraph.Channel = _channelStack.Pop();
                            }
                            break;
                        case "CloseApp":
                            this.Close();
                            break;
                        case "Record":
                            btnRecord_Click(btnRecord, new EventArgs());
                            break;
                        case "Left":
                        case "Right":
                        case "Up":
                        case "Down":
                            MoveCursor(actionName);
                            break;
                    }
                }
            }
        }

        private void MoveCursor(string whichway)
        {
            switch (whichway)
            {
                case "Left":
                    Cursor.Position = new Point(Cursor.Position.X - 20, Cursor.Position.Y);
                    break;
                case "Right":
                    Cursor.Position = new Point(Cursor.Position.X + 20, Cursor.Position.Y);
                    break;
                case "Up":
                    Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y - 20);
                    break;
                case "Down":
                    Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + 20);
                    break;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Decimal:
                case Keys.OemPeriod:
                case Keys.Subtract:
                case Keys.Space:
                case Keys.Separator:
                case Keys.OemMinus:
                    channelKeypad.SendKey(".");
                    break;
                case Keys.Enter:
                    channelKeypad.SendKey(Environment.NewLine);
                    break;
                case Keys.Up:
                    btnChannelUp_Click(btnChannelUp, new EventArgs());
                    break;
                case Keys.Down:
                    btnChannelDown_Click(btnChannelDown, new EventArgs());
                    break;
                case Keys.Left:
                case Keys.VolumeDown:
                    volDown_Click(volDown, new EventArgs());
                    break;
                case Keys.Right:
                case Keys.VolumeUp:
                    volUp_Click(volUp, new EventArgs());
                    break;
                case Keys.VolumeMute:
                    chkMute.Checked = !chkMute.Checked;
                    chkMute_CheckedChanged(chkMute, new EventArgs());
                    break;
                default:
                    int number = GetNumericKey(e.KeyCode);
                    if (number != -1)
                    {
                        channelKeypad.SendKey(number);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the integer equivelent of a given Key
        /// </summary>
        /// <param name="k">key to get integer equivelent for</param>
        /// <returns>-1 if not a numeric key, else returns 0 - 9</returns>
        private int GetNumericKey(Keys k)
        {
            switch (k)
            {
                case Keys.D0:
                case Keys.NumPad0:
                    return 0;
                case Keys.D1:
                case Keys.NumPad1:
                    return 1;
                case Keys.D2:
                case Keys.NumPad2:
                    return 2;
                case Keys.D3:
                case Keys.NumPad3:
                    return 3;
                case Keys.D4:
                case Keys.NumPad4:
                    return 4;
                case Keys.D5:
                case Keys.NumPad5:
                    return 5;
                case Keys.D6:
                case Keys.NumPad6:
                    return 6;
                case Keys.D7:
                case Keys.NumPad7:
                    return 7;
                case Keys.D8:
                case Keys.NumPad8:
                    return 8;
                case Keys.D9:
                case Keys.NumPad9:
                    return 9;
                default:
                    return -1;
            }
        }

        #endregion

        #region Favorite Channels Handlers

        private void favoriteChannels_AddCurrentAsFavorite(object sender, EventArgs e)
        {
            if (DSGraph == null)
            {
                return;
            }

            Snapshot snap = DSGraph.CreateSnapshot();
            favoriteChannels.Add(DSGraph.Channel, snap);
            if (snap != null)
            {
                snap.Dispose();
            }
        }

        private void favoriteChannels_TuneFavoriteChannel(object sender, Channel channel)
        {
            if (DSGraph == null)
            {
                return;
            }

            BeginChannelChange(false);

            DSGraph.ChannelForceChange = true;

            DSGraph.Channel = channel;
        }

        private void favoriteChannels_ScanningStateChanged(object sender, EventArgs e)
        {
            if (favoriteChannels.Scanning)
            {
                Blinky |= BlinkMode.Scan;
            }
            else
            {
                Blinky = Blinky & ~(BlinkMode.Scan);
            }
        }

        #endregion
    }
}
