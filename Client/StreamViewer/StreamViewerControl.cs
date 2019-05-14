using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using FutureConcepts.Media.CommonControls;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.Client.InBandData;
using FutureConcepts.Media;
using FutureConcepts.Tools;
using FutureConcepts.Tools.Utilities;

using FutureConcepts.Media.Client.StreamViewer.DeviceControl;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// Possible states that a StreamViewer can be in. See "SVD Documentation" article on the wiki.
    /// </summary>
    public enum StreamState
    {
        /// <summary>
        /// At rest.
        /// </summary>
        Available,
        /// <summary>
        /// Establishing connection to server
        /// </summary>
        Connecting,
        /// <summary>
        /// Connection established -- buffering data stream
        /// </summary>
        Buffering,
        /// <summary>
        /// Live/Streaming
        /// </summary>
        Playing,
        /// <summary>
        /// Disconnecting
        /// </summary>
        Stopping,
        /// <summary>
        /// Resources released
        /// </summary>
        Stopped,
        /// <summary>
        /// Frame capture occuring
        /// </summary>
        Snapshot,
        /// <summary>
        /// Preparing to begin recording
        /// </summary>
        BeginRecording,
        /// <summary>
        /// Live/Streaming, recording to disk
        /// </summary>
        Recording
    }

    /// <summary>
    /// Encapsulates all of the functionality to connect to, render, record and frame grab a video stream, as a user control
    /// </summary>
    public partial class StreamViewerControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        private IDeviceControl _deviceControl;
        private Graphs.BaseGraph _graph;

        public Boolean ProfileGroupSelectorEnabled
        {
            get
            {
                Boolean result = false;
                if (_deviceControl != null)
                {
                    result = _deviceControl.IsProfileGroupSelectorEnabled();
                }
                return result;
            }
        }

        /// <summary>
        /// Hold this mutex during connect or disconnect operations to ensure only one is occuring at once
        /// </summary>
        private readonly object connectMutex = new object();
     
        private BackgroundWorker connectWorker;
        private BackgroundWorker disconnectWorker;

        public delegate void SetCurrentConnectionDelegate(ConnectionChainDescriptor desc);

        /// <summary>
        /// Instantiates a new StreamViewerControl
        /// </summary>
        public StreamViewerControl()
        {
            InitializeComponent();

            //app.config can control whether or not the recording/snapshot buttons are available
            bool recordingEnabled = true;
            if (bool.TryParse(ConfigurationManager.AppSettings["StreamViewerRecordingEnabled"], out recordingEnabled))
            {
                this.RecordingEnabled = recordingEnabled;
            }
            else
            {
                this.RecordingEnabled = true;
            }

            State = StreamState.Available;
            LocationChanged += new EventHandler(LocationChanged_Handler);

            connectWorker = new BackgroundWorker();
            connectWorker.WorkerSupportsCancellation = true;
            connectWorker.DoWork += new DoWorkEventHandler(connectWorker_DoWork);
            connectWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(connectWorker_RunWorkerCompleted);

            disconnectWorker = new BackgroundWorker();
            disconnectWorker.DoWork += new DoWorkEventHandler(disconnectWorker_DoWork);
            disconnectWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(disconnectWorker_RunWorkerCompleted);
        }

        private void StreamViewerControl_Load(object sender, EventArgs e)
        {
            DragDropManager.RegisterTarget(this.P_Video, P_Video_DragDrop);
        }

        void IDisposable.Dispose()
        {
            CloseLocalGraph();
            CloseServerGraph();

            connectWorker.Dispose();

            disconnectWorker.Dispose();
        }

        private void Log(String msg)
        {
            Debug.WriteLine(String.Format("{0} StreamViewerControl: {1}", Name, msg));
        }

        #region Properties

        private bool _active = false;
        /// <summary>
        /// Set to true to indicate this SVC has focus
        /// </summary>
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (value != _active)
                {
                    _active = value;
                    if (value == true)
                    {
                        this.BackColor = Color.Yellow;
                    }
                    else
                    {
                        this.BackColor = Color.DimGray;
                    }
                    NotifyPropertyChanged("Active");
                }
            }
        }

        private void P_Video_Click(object sender, EventArgs e)
        {
            Active = true;
        }

        private int _reconnectTime = 2000;
        private int _reconnectCount = 0;

        private bool _firstConnect = true;
        /// <summary>
        /// This property is maintained to remember what behavior to use when contacting the server
        /// </summary>
        private bool FirstConnect
        {
            get
            {
                return _firstConnect;
            }
            set
            {
                _firstConnect = value;
                if (_firstConnect)
                {
                    _reconnectTime = 2000;
                    _reconnectCount = 0;
                }
            }
        }

        private ConnectionChainDescriptor _pendingConnection;

        /// <summary>
        /// Non-null if the user has requested a connection that could not be fulfilled immediately
        /// </summary>
        public ConnectionChainDescriptor PendingConnection
        {
            get
            {
                return _pendingConnection;
            }
            set
            {
                _pendingConnection = value;
                NotifyPropertyChanged("PendingConnection");
            }
        }

        private ConnectionChainDescriptor _connectionChain;
        /// <summary>
        /// The current connection
        /// </summary>
        public ConnectionChainDescriptor CurrentConnection
        {
            get
            {
                return _connectionChain;
            }
            private set
            {
                if (value != _connectionChain)
                {
                    _connectionChain = value;

                    //hide control buttons
                    ControlState[SourceControlTypes.Microwave] = SourceControlTypeState.Unavailable;
                    ControlState[SourceControlTypes.PTZ] = SourceControlTypeState.Unavailable;

                    if (_connectionChain != null)
                    {
                        string desc = _connectionChain.GetDescription();
                        L_StreamDescription.Text = desc;
                        tt.SetToolTip(L_StreamDescription, desc);

                        //determine if this SVC has PTZ control available
                        if (_connectionChain.Source.CameraControl != null)
                        {
                            ControlState[SourceControlTypes.PTZ] = SourceControlTypeState.Inactive;
                            ControlState[SourceControlTypes.PTZ] = SourceControlTypeState.Disabled;
                        }

                        //determine if this SVC has microwave control available
                        if(_connectionChain.Source.MicrowaveControl != null)
                        {
                            ControlState[SourceControlTypes.Microwave] = SourceControlTypeState.Inactive;
                            ControlState[SourceControlTypes.Microwave] = SourceControlTypeState.Disabled;
                        }
                    }
                    else
                    {
                        L_StreamDescription.Text = "---";
                        tt.SetToolTip(L_StreamDescription, "");
                    }
                }

            }
        }

        /// <summary>
        /// Gets the currently associated StreamSourceInfo
        /// </summary>
        public StreamSourceInfo SourceInfo
        {
            get
            {
                if (this.CurrentConnection != null)
                {
                    return this.CurrentConnection.Source;
                }
                return null;
            }
        }

        private int _avgBitRate = -1;
        /// <summary>
        /// Gets or sets the Average Bit Rate
        /// </summary>
        public int AverageBitRate
        {
            get
            {
                return _avgBitRate;
            }
            private set
            {
                if (value != _avgBitRate)
                {
                    _avgBitRate = value;
                    NotifyPropertyChanged("AverageBitRate");
                }
            }
        }

        private int _latency = -1;
        /// <summary>
        /// Gets or sets the last determined Latency
        /// </summary>
        public int Latency
        {
            get
            {
                return _latency;
            }
            private set
            {
                if (_latency != value)
                {
                    _latency = value;
                    NotifyPropertyChanged("Latency");
                }
            }
        }

        /// <summary>
        /// This event is raised for all of the public properties
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires the PropertyChanged event
        /// </summary>
        /// <param name="property">name of the property that changed</param>
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Hold this mutex while a state transition is occuring to prevent concurrent modification of state
        /// </summary>
        private readonly object stateAlterMutex = new object();

        private StreamState _state;
        /// <summary>
        /// Gets or Sets the current State of this stream
        /// </summary>
        /// <remarks>
        /// Setting the State will cause a state transition to be performed.
        /// </remarks>
        public StreamState State
        {
            get
            {
                return _state;
            }
            private set
            {
                lock (stateAlterMutex)
                {
                    if (value != _state)
                    {
                        try
                        {
                            DoStateTransition(value);
                        }
                        catch (Exception exc)
                        {
                            ErrorLogger.DumpToDebug(exc);
                        }
                    }
                }
            }
        }

        private string _progressMessage;
        /// <summary>
        /// Get the current progress message for this control
        /// </summary>
        public string ProgressMessage
        {
            get
            {
                return _progressMessage;
            }
            private set
            {
                _progressMessage = value;
                NotifyPropertyChanged("ProgressMessage");
            }
        }

        /// <summary>
        /// Retreives the IP address for the currently connected origin server.
        /// </summary>
        public string ServerAddress
        {
            get
            {
                if (this.CurrentConnection != null)
                {
                    if (this.CurrentConnection.HostServer != null)
                    {
                        return this.CurrentConnection.HostServer.ServerAddress;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Retreives the SessionDescription if connected. Returns null otherwise.
        /// </summary>
        public SessionDescription SessionDescription
        {
            get
            {
                if (_deviceControl != null)
                {
                    return _deviceControl.GetSessionDescription();
                }
                else
                {
                    return null;
                }
            }
        }

        private List<string> _users = new List<string>();
        /// <summary>
        /// The last known list of users also connected to this stream
        /// </summary>
        public List<string> Users
        {
            get
            {
                return _users;
            }
            private set
            {
                _users = value;
                NotifyPropertyChanged("Users");
            }
        }

        private string _snapshotPath = @"C:\AntaresXData\Snapshots\";
        /// <summary>
        /// The path where snapshots are stored
        /// </summary>
        public string SnapshotPath
        {
            set
            {
                _snapshotPath = value;
            }
            get
            {
                return _snapshotPath;
            }
        }

        private string _snapshotSoundFileName = @"c:\windows\media\ding.wav";
        /// <summary>
        /// Path to the sound played when a snapshot is taken
        /// </summary>
        public string SnapshotSoundFileName
        {
            set
            {
                _snapshotSoundFileName = value;
            }
            get
            {
                return _snapshotSoundFileName;
            }
        }

        private string _recordingsPath = @"C:\AntaresXData\Recordings\";
        /// <summary>
        /// The path where recordings are stored
        /// </summary>
        public string RecordingsPath
        {
            get
            {
                return _recordingsPath;
            }
            set
            {
                _recordingsPath = value;
            }
        }

        private bool _recordingEnabled = true;
        /// <summary>
        /// Enables/Disables Recordings and Snapshots
        /// </summary>
        public bool RecordingEnabled
        {
            get
            {
                return _recordingEnabled;
            }
            set
            {
                _recordingEnabled = value;
                B_Record.Visible = value;
                B_Snapshot.Visible = value;
            }
        }



        #endregion

        #region State Changes

        private void DoStateTransition(StreamState targetState)
        {
            Log(">>>> " + this.Name + " State change from " + _state + " to " + targetState);

            switch (_state)
            {
                case StreamState.Available:
                    switch (targetState)
                    {
                        case StreamState.Connecting:
                            EnableBuffering();
                            break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.Connecting:
                    switch (targetState)
                    {
                        case StreamState.Buffering:
                            EnableBuffering();
                            break;
                  //      case StreamState.Available:
                   //         EnableStopped();
                    //        break;
                        case StreamState.Stopping: // Cancelled
                            EnableStopped();
                            break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.Buffering:
                    switch (targetState)
                    {
                        case StreamState.Stopping:
                            EnableStopped();
                            break;
                        case StreamState.Playing:
                            EnablePlaying();
                            break;
                      //  case StreamState.Available:
                      //      EnableStopped();
                      //      break;
                        case StreamState.Connecting:
                            break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.Playing:
                    switch (targetState)
                    {
                        case StreamState.Buffering:
                            EnableBuffering();
                            break;
                        case StreamState.Connecting:
                            EnableBuffering();
                            break;
                        case StreamState.BeginRecording:
                            EnableRecording(true);
                            break;
                        case StreamState.Stopping:
                            EnableStopped();
                            break;
                        case StreamState.Snapshot:
                            break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.BeginRecording:
                    switch (targetState)
                    {
                        case StreamState.Recording:
                            //B_Record.Enabled = true;
                            break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.Recording:
                    switch (targetState)
                    {
                        case StreamState.Playing:
                            EnableRecording(false);   //TODO this may be risky
                            EnablePlaying();
                            break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.Stopping:
                    switch (targetState)
                    {
                        case StreamState.Stopped:
                            EnableStopped();
                            break;
                        //case StreamState.Snapshot:
                        //    break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.Stopped:
                    switch (targetState)
                    {
                        case StreamState.Available:
                            EnableStopped();
                            break;
                        case StreamState.Connecting:
                            EnableBuffering();
                            break;
                      //  case StreamState.Buffering:
                      //      EnableBuffering();
                      //      break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                case StreamState.Snapshot:
                    switch (targetState)
                    {
                        case StreamState.Playing:
                            break;
                        default:
                            IllegalStateTransition(targetState);
                            break;
                    }
                    break;
                default:
                    Log("Attempted state transition at State " + State);
                    break;
            }

            ShowBusyIcon(targetState);

            _state = targetState;
            NotifyPropertyChanged("State");
        }

        /// <summary>
        /// Shows or hides the Busy Icon for a given target state that the SVC will be put into
        /// </summary>
        /// <param name="targetState">State the SVC is transitioning to</param>
        private void ShowBusyIcon(StreamState targetState)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<StreamState>(ShowBusyIcon), targetState);
                return;
            }

            //sets the visibility of the busy indicator
            switch (targetState)
            {
                case StreamState.Connecting:
                case StreamState.Buffering:
                case StreamState.Stopping:
                case StreamState.Snapshot:
                case StreamState.BeginRecording:
                    tt.SetToolTip(iconBusy, targetState.ToString());
                    iconBusy.Visible = true;
                    break;
                case StreamState.Available:
                case StreamState.Playing:
                case StreamState.Stopped:
                case StreamState.Recording:
                default:
                    tt.SetToolTip(iconBusy, null);
                    iconBusy.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// Tries to fix the SVC when an illegal state transition occurs
        /// </summary>
        /// <param name="targetState">state that is illegal to enter from current state</param>
        private void IllegalStateTransition(StreamState targetState)
        {
            // How and why are we here?
            Log(">>>> " + this.Name + ": Illegal state transition from " + State.ToString() + " to " + targetState.ToString());
            ErrorLogger.WriteToEventLog(this.Name + ": Illegal state transition from " + State.ToString() + " to " + targetState.ToString(), EventLogEntryType.Warning);

            if (!disconnectWorker.IsBusy)
            {
                lock (connectMutex)
                {
                    lock (stateAlterMutex)
                    {
                        _state = StreamState.Stopping;
                    }
                    disconnectWorker.RunWorkerAsync();
                }
            }
        }

        #endregion

        #region Profile Handling

        private Profile _currentProfile;
        /// <summary>
        /// Gets or sets the current profile
        /// </summary>
        public Profile CurrentProfile
        {
            get
            {
                return _currentProfile;
            }
            set
            {
                if (value != _currentProfile)
                {
                    _currentProfile = value;
                    NotifyPropertyChanged("CurrentProfile");
                }
            }
        }

        private Profile _customProfile;
        /// <summary>
        /// Gets or sets the custom profile
        /// </summary>
        public Profile CustomProfile
        {
            get
            {
                return _customProfile;
            }
            set
            {
                if (value != _customProfile)
                {
                    _customProfile = value;
                    NotifyPropertyChanged("CustomProfile");
                }
            }
        }

        /// <summary>
        /// Sets the profile on the server.
        /// This method should be called on a separate thread to avoid blocking the GUI
        /// </summary>
        /// <param name="newProfile">new profile to set on the server</param>
        public void SetProfileOnServer(Profile newProfile)
        {
            lock (connectMutex)
            {
                try
                {
                    if (_deviceControl != null)
                    {
                        //if SetProfile returns a new session description, we must reconnect
                        _deviceControl.SetProfile(newProfile);
                        if (true)
//                        if (_deviceControl.SetProfile(newProfile) != null)
                        {
                            CurrentProfile = newProfile;
                            CloseLocalGraph();
                            StartLocalGraph();
                        }
                    }
                }
                //if an exception occurs during this process, then we almost certainly have lost connection the server
                //see Mantis #1073      kdixon 04/08/09
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                    this.GraphControlClient_Faulted(this, new EventArgs()); //invoke the Faulted method to reconnect
                }
            }
        }

        #endregion

        #region UI Controls

        /// <summary>
        /// Sets the enabled state of the controls when in the Recording state.
        /// Thread Safe.
        /// </summary>
        /// <param name="startingRecording">If true, starting recording, if false, stopping recording</param>
        private void EnableRecording(bool startingRecording)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(EnableRecording), new object[] { startingRecording });
                return;
            }

            B_StopFull.Enabled = false;
            B_Snapshot.Enabled = false;
            B_Record.Enabled = true;
            B_SingleView.Enabled = true;
            B_FullScreen.Enabled = true;

            ControlState.SetAllTo(SourceControlTypeState.Enabled);

            if (startingRecording)
            {
                B_Record.ImageIndex = 1;
                tt.SetToolTip(B_Record, "Stop recording the stream");
            }
            else
            {
                B_Record.ImageIndex = 0;
                tt.SetToolTip(B_Record, "Record the current stream to a local file");
            }
        }

        /// <summary>
        /// Sets the enabled state of the controls when in the Playing state.
        /// Thread Safe.
        /// </summary>
        private void EnablePlaying()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(EnablePlaying));
                return;
            }

            B_StopFull.Enabled = true;
            B_Snapshot.Enabled = true;
            B_Record.Enabled = true;
            B_Record.ImageIndex = 0;
            B_SingleView.Enabled = true;
            B_FullScreen.Enabled = true;

            ControlState.SetAllTo(SourceControlTypeState.Enabled);
        }

        /// <summary>
        /// Sets the enabled state of the controls when not in the Playing state (Stopped, or other intermittant).
        /// Thread Safe.
        /// </summary>
        private void EnableStopped()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(EnableStopped));
                return;
            }

            B_StopFull.Enabled = false;
            B_Snapshot.Enabled = false;
            B_Record.Enabled = false;
            B_Record.ImageIndex = 0;
            B_SingleView.Enabled = true;
            B_FullScreen.Enabled = false;

            ControlState.SetAllTo(SourceControlTypeState.Disabled);

            if (this.FullScreen)
            {
                FireFullScreenClicked();
            }
        }

        /// <summary>
        /// Sets the enabled state of the controls when in the Buffering mode.
        /// Thread Safe.
        /// </summary>
        private void EnableBuffering()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(EnableBuffering));
                return;
            }

            EnableStopped();
            B_StopFull.Enabled = true;
        }

        /// <summary>
        /// Returns a snapshot of the current stream, scaled to 160x120
        /// </summary>
        /// <returns>the image, or null if an error occurrs</returns>
        public Image GetSnapshot()
        {
            try
            {
                Snapshot ss = _graph.CreateSnapshot();
                return ss.GetImageScaled(new Size(160, 120));
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets an acceptable filename for a recording or snapshot
        /// </summary>
        /// <returns>a filename with no extension</returns>
        private string GetRecordingFilename()
        {
            StringBuilder fname = new StringBuilder();
            fname.Append(DateTime.Now.ToString("HHmmss") + "_");
            if (CurrentConnection.OriginServer != null)
            {
                fname.Append(this.CurrentConnection.OriginServer.ServerName);
                fname.Append(" - ");
            }
            fname.Append(this.CurrentConnection.Source.Description);
            foreach (char i in Path.GetInvalidFileNameChars())
            {
                fname = fname.Replace(i, ' ');
            }
            return fname.ToString();
        }

        private void B_Snapshot_Click(object sender, EventArgs e)
        {
            try
            {
                if (_graph == null)
                {
                    return;
                }

                string path = SnapshotPath + @"\" + DateTime.Now.ToString("MMddyy") + @"\";
                State = StreamState.Snapshot;
                Snapshot ss = _graph.CreateSnapshot();
                Image img = ss.Image;
                if (img == null)
                {
                    return;
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fname = GetRecordingFilename() + ".jpg";
                img.Save(path + fname, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Dispose();
                img = null;

                ss.Dispose();
                try
                {
                    using (SoundPlayer player = new SoundPlayer(SnapshotSoundFileName))
                    {
                        player.Play();
                    }
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
            finally
            {
                State = StreamState.Playing;
            }
        }

        private void B_Record_Click(object sender, EventArgs e)
        {
            if (State == StreamState.Playing)
            {
                try
                {
                    string dateNow = DateTime.Now.ToString("MMddyy");
                    string filename = GetRecordingFilename() + ".mpg";

                    State = StreamState.BeginRecording;
                    string dirPath = RecordingsPath + dateNow + @"\";
                    Directory.CreateDirectory(dirPath);
                    _graph.RecordingFileName = dirPath + filename;
                    State = StreamState.Recording;
                }
                catch (Exception ex)
                {
                    Log("Failed to start recording!");
                    ErrorLogger.DumpToDebug(ex);
                    if (_graph != null)
                    {
                        _graph.RecordingFileName = null;
                    }
                    if (State == StreamState.BeginRecording)
                    {
                        State = StreamState.Recording;
                    }
                    State = StreamState.Playing;
                }
            }
            else if (State == StreamState.Recording)
            {
                _graph.RecordingFileName = null;
                State = StreamState.Playing;
            }
        }

        #region [Single-View / 4-way]

        /// <summary>
        /// Fired when the FullView button is clicked.
        /// </summary>
        public event EventHandler SingleViewClicked;

        private void B_SingleView_Click(object sender, EventArgs e)
        {
            FireSingleViewClicked();
        }

        private void FireSingleViewClicked()
        {
            if (SingleViewClicked != null)
            {
                SingleViewClicked(this, new EventArgs());
            }
        }

        private bool _inSingleView;
        /// <summary>
        /// Returns true if this SVC is in Single-View mode or not
        /// </summary>
        public bool SingleView
        {
            get
            {
                return _inSingleView;
            }
            set
            {
                _inSingleView = value;
                B_SingleView.ImageIndex = _inSingleView ? 1 : 0;
                tt.Hide(B_SingleView);
                tt.SetToolTip(B_SingleView, null);
                if (_inSingleView)
                {
                    tt.SetToolTip(B_SingleView, "Switch to 4-way view.");
                }
                else
                {
                    tt.SetToolTip(B_SingleView, "Switch this stream to single view. Any other streams will be stopped.");
                }
            }
        }

        #endregion

        #region [Full-Screen]

        /// <summary>
        /// This event is fired when the Full Screen button is clicked. The <c>sender</c> will be the StreamViewer
        /// that wants to be displayed full screen
        /// </summary>
        public event EventHandler FullScreenClicked;

        /// <summary>
        /// Called when the full screen button is clicked.
        /// </summary>
        private void B_FullScreen_Click(object sender, EventArgs e)
        {
            FireFullScreenClicked();
        }

        /// <summary>
        /// Call this method to alert listeners that this SVC needs to change its full-screen state,
        /// the target state being defined in the FullScreen property.
        /// </summary>
        private void FireFullScreenClicked()
        {
            if (FullScreenClicked != null)
            {
                FullScreenClicked(this, new EventArgs());
            }
        }

        private bool _inFullScreen;
        /// <summary>
        /// True if this streamviewer is in full screen mode, false if not.
        /// </summary>
        public bool FullScreen
        {
            get
            {
                return _inFullScreen;
            }
            set
            {
                _inFullScreen = value;
                B_FullScreen.ImageIndex = _inFullScreen ? 1 : 0;
                tt.Hide(B_FullScreen);
                tt.SetToolTip(B_FullScreen, null);
                if (_inFullScreen)
                {
                    tt.SetToolTip(B_FullScreen, "Leave full-screen mode.");
                    B_SingleView.Enabled = false;
                }
                else
                {
                    tt.SetToolTip(B_FullScreen, "Expand this stream to full-screen.");
                    B_SingleView.Enabled = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// Prompts the user to disconnect after a certain amount of time.
        /// </summary>
        private void tmrStreamTimeLimit_Tick(object sender, EventArgs e)
        {
            tmrStreamTimeLimit.Enabled = false;
            FCTimedYesNoMsgBox msgBox = new FCTimedYesNoMsgBox();
            DialogResult result = msgBox.ShowDialog("Stream Time Limit Reached", "Do you want to continue viewing?", new Size(220, 150));
            if (result == DialogResult.No || result == DialogResult.Cancel)
            {
                Stop();
                if (result == DialogResult.Cancel)
                {
                    FCMessageBox.Show("Stream Time Limit Expired", "You have exceeded the time limit for this stream and have been disconnected.");
                }
            }
            else
            {
                tmrStreamTimeLimit.Enabled = true;
            }
        }

        #endregion

        #region [Add-On Source Controls]

        /// <summary>
        /// Raised when user requests to begin controlling a peripheral -- e.g. ptz camera or microwave rx
        /// </summary>
        /// <seealso cref="FutureConcepts.Media.Client.StreamViewer.StreamViewerControl.StopControl"/>
        [Category("Action"), Description("Raised when control should be established for this viewer.")]
        public event EventHandler<SourceControlTypeEventArgs> StartControl;

        /// <summary>
        /// Raised when the user is done controlling a peripheral.
        /// </summary>
        /// <seealso cref="FutureConcepts.Media.Client.StreamViewer.StreamViewerControl.StartControl"/>
        [Category("Action"), Description("Raised when control should be stopped for this viewer.")]
        public event EventHandler<SourceControlTypeEventArgs> StopControl;

        /// <summary>
        /// Raises the StartControl event
        /// </summary>
        /// <param name="type">type of control to start</param>
        private void FireStartControl(SourceControlTypes type)
        {
            if (StartControl != null)
            {
                iconBusy.Visible = true;
                tt.SetToolTip(iconBusy, "Starting " + type.ToString() + " control");
                StartControl.Invoke(this, new SourceControlTypeEventArgs(type));
            }
        }

        /// <summary>
        /// Raises the StopControl event
        /// </summary>
        /// <param name="type">type of control to stop</param>
        private void FireStopControl(SourceControlTypes type)
        {
            if (StopControl != null)
            {
                iconBusy.Visible = true;
                tt.SetToolTip(iconBusy, "Stopping " + type.ToString() + " control");
                StopControl.Invoke(this, new SourceControlTypeEventArgs(type));
            }
        }

        private SourceControlTypeStateTracker _controlEnabled;
        /// <summary>
        /// Get or set the enabled state of each source control type
        /// </summary>
        public SourceControlTypeStateTracker ControlState
        {
            get
            {
                if (_controlEnabled == null)
                {
                    _controlEnabled = new SourceControlTypeStateTracker();
                    _controlEnabled.StateChanged += new EventHandler<SourceControlTypeEventArgs>(SourceControlType_StateChanged);
                }
                return _controlEnabled;
            }
        }

        private void SourceControlType_StateChanged(object sender, SourceControlTypeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<SourceControlTypeEventArgs>(SourceControlType_StateChanged), new object[] { sender, e });
                return;
            }

            iconBusy.Visible = false;
            tt.SetToolTip(iconBusy, null);

            switch (e.ControlType)
            {
                case SourceControlTypes.PTZ:

                    switch (ControlState[e.ControlType])
                    {
                        case SourceControlTypeState.Active:
                            btnStartStopPTZ.ImageIndex = 1;
                            btnStartStopPTZ.Visible = true;
                            btnStartStopPTZ.Enabled = true;
                            tt.SetToolTip(btnStartStopPTZ, "Stop controlling this Pan/Tilt/Zoom camera.");
                            break;
                        case SourceControlTypeState.Inactive:
                            btnStartStopPTZ.ImageIndex = 0;
                            btnStartStopPTZ.Visible = true;
                            btnStartStopPTZ.Enabled = true;
                            tt.SetToolTip(btnStartStopPTZ, "Control this Pan/Tilt/Zoom camera.");
                            break;
                        case SourceControlTypeState.Enabled:
                            btnStartStopPTZ.Enabled = true;
                            break;
                        case SourceControlTypeState.Disabled:
                            btnStartStopPTZ.Enabled = false;
                            break;
                        case SourceControlTypeState.Unavailable:
                            btnStartStopPTZ.Visible = false;
                            break;
                    }

                    if (PTZOverlayControl != null)
                    {
                        PTZOverlayControl.Enabled = ControlState[e.ControlType] == SourceControlTypeState.Active;
                    }
                    break;

                case SourceControlTypes.Microwave:
                    switch (ControlState[e.ControlType])
                    {
                        case SourceControlTypeState.Active:
                            btnStartStopMicrowaveControl.Visible = true;
                            btnStartStopMicrowaveControl.Enabled = true;
                            btnStartStopMicrowaveControl.ImageIndex = 1;
                            tt.SetToolTip(btnStartStopMicrowaveControl, "Stop controlling the attached Microwave Receiver.");
                            break;
                        case SourceControlTypeState.Inactive:
                            btnStartStopMicrowaveControl.Visible = true;
                            btnStartStopMicrowaveControl.Enabled = true;
                            btnStartStopMicrowaveControl.ImageIndex = 0;
                            tt.SetToolTip(btnStartStopMicrowaveControl, "Control the attached Microwave Receiver.");
                            break;
                        case SourceControlTypeState.Enabled:
                            btnStartStopMicrowaveControl.Enabled = true;
                            break;
                        case SourceControlTypeState.Disabled:
                            btnStartStopMicrowaveControl.Enabled = false;
                            break;
                        case SourceControlTypeState.Unavailable:
                            btnStartStopMicrowaveControl.Visible = false;
                            break;
                    }
                    break;
            }
        }


        #region PTZ Eventing and Overlay

        private void btnStartStopPTZ_Click(object sender, EventArgs e)
        {
            if (ControlState[SourceControlTypes.PTZ] == SourceControlTypeState.Active)
            {
                FireStopControl(SourceControlTypes.PTZ);
            }
            else if (ControlState[SourceControlTypes.PTZ] == SourceControlTypeState.Inactive)
            {
                btnStartStopPTZ.ImageIndex = 1;
                ControlState[SourceControlTypes.PTZ] = SourceControlTypeState.Disabled;
                FireStartControl(SourceControlTypes.PTZ);
            }
        }

        private IPTZOverlayControl _ptzOverlayControl;
        /// <summary>
        /// Allows the setting of a PTZ Overlay Control.
        /// </summary>
        /// <remarks>
        /// Detaches and Attaches Mouse Event handlers from the viewing screen to the PTZ Overlay Control
        /// </remarks>
        public IPTZOverlayControl PTZOverlayControl
        {
            get
            {
                return _ptzOverlayControl;
            }
            set
            {
                if (_ptzOverlayControl != null)
                {
                    P_Video.MouseClick -= new MouseEventHandler(_ptzOverlayControl.OnMouseClick);
                    P_Video.MouseDown -= new MouseEventHandler(_ptzOverlayControl.OnMouseDown);
                    P_Video.MouseUp -= new MouseEventHandler(_ptzOverlayControl.OnMouseUp);
                    P_Video.MouseMove -= new MouseEventHandler(_ptzOverlayControl.OnMouseMove);
                    P_Video.Cursor = Cursors.Default;
                }
                _ptzOverlayControl = value;
                if (value != null)
                {
                    P_Video.MouseClick += new MouseEventHandler(value.OnMouseClick);
                    P_Video.MouseDown += new MouseEventHandler(value.OnMouseDown);
                    P_Video.MouseUp += new MouseEventHandler(value.OnMouseUp);
                    P_Video.MouseMove += new MouseEventHandler(value.OnMouseMove);
                }
            }
        }
        
        #endregion

        #region Microwave Control

        private void btnStartStopMicrowaveControl_Click(object sender, EventArgs e)
        {
            if (ControlState[SourceControlTypes.Microwave] == SourceControlTypeState.Active)
            {
                FireStopControl(SourceControlTypes.Microwave);
            }
            else if (ControlState[SourceControlTypes.Microwave] == SourceControlTypeState.Inactive)
            {
                btnStartStopMicrowaveControl.ImageIndex = 1;
                ControlState[SourceControlTypes.Microwave] = SourceControlTypeState.Disabled;
                FireStartControl(SourceControlTypes.Microwave);
            }
        }

        #endregion

        #endregion

        #region Drag/Drop Methods

        private void P_Video_DragDrop(object sender, DataDroppedEventArgs e)
        {
            if (!(e.Data is ConnectionChainDescriptor))
            {
                return;
            }

            try
            {
                switch (State)
                {
                    case StreamState.Connecting:
                    case StreamState.Buffering:
                    case StreamState.Playing:
                        FirstConnect = true;
                        PendingConnection = e.Data as ConnectionChainDescriptor;
                        Stop();
                        break;
                    case StreamState.Recording:
                        if (DialogResult.Yes == FCYesNoMsgBox.ShowDialog("Stop recording?", "Starting a new stream will stop the recording in progress. Do you wish to continue?", this))
                        {
                            if (_graph != null)
                            {
                                _graph.RecordingFileName = null;
                                State = StreamState.Playing;
                            }
                            FirstConnect = true;
                            PendingConnection = e.Data as ConnectionChainDescriptor;
                            Stop();
                        }
                        break;
                    case StreamState.Stopped:
                    case StreamState.Available:
                        FirstConnect = true;
                        PendingConnection = e.Data as ConnectionChainDescriptor;
                        ExecutePendingConnection();
                        break;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        /// <summary>
        /// CurrentConnection
        /// </summary>
        public void SetCurrentConnection(ConnectionChainDescriptor connection)
        {
            if (InvokeRequired)
            {
                Invoke(new SetCurrentConnectionDelegate(SetCurrentConnection), connection);
                return;
            }
            if (connection != null)
            {
                FirstConnect = true;
                CurrentConnection = connection;
            }
        }

        /// <summary>
        /// If the PendingConnection property is non-null, then it will start a connection
        /// </summary>
        private void ExecutePendingConnection()
        {
            if (PendingConnection == null)
            {
                return;
            }

            Active = true;

            this.Cursor = Cursors.WaitCursor;

            this.CurrentConnection = PendingConnection;
            PendingConnection = null;
            ClientConnectRequest = null;    //clear this property so it gets regenerated correctly
            Run();

            this.Cursor = Cursors.Default;
        }

        #endregion

        #region Connection Establishment and Control

        private ClientConnectRequest _connectRequest;
        /// <summary>
        /// Stores the ClientConnectRequest. If the getter is called and it is null, then a new one is created.
        /// </summary>
        private ClientConnectRequest ClientConnectRequest
        {
            get
            {
                if (_connectRequest == null)
                {
                    _connectRequest = new ClientConnectRequest(this.CurrentConnection.GetSourceName());
                }
                return _connectRequest;
            }
            set
            {
                _connectRequest = value;
            }
        }

        /// <summary>
        /// Begins the connection initiation and the video
        /// </summary>
        public void Run()
        {
            if (!connectWorker.IsBusy)
            {
                connectWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Background thread that does the initial connection to the server.
        /// </summary>
        private void connectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (connectMutex)
            {
                Thread.CurrentThread.Name = this.Name + " connectWorker";

                Log(">>>> " + this.Name + " connectWorker_DoWork");

                bool success = false;
                IDeviceControl deviceControl = null;
                
                for (int i = 1; !success; i++)
                {
                    try
                    {
                        Boolean setTimeoutTimer = false;
                        deviceControl = DeviceControlFactory.Create(SourceInfo, ServerAddress);

                        CheckConnectCancelled();

                        State = StreamState.Connecting;
                        Log(String.Format("Connect attempt {0}", i));
                        ProgressMessage = "Connecting";

                        if ( (SourceInfo.SourceType == SourceType.RTSP || SourceInfo.SourceType == SourceType.RTSP_Elecard) == false)
                        {
                            //attempt ping to measure latency and determine connectivity
                            Ping ping = new Ping();
                            PingReply pingReply = ping.Send(ServerAddress);
                            if (pingReply.Status != IPStatus.Success)
                            {
                                throw new PingException(pingReply.Status.ToString());
                            }

                            this.Latency = (int)pingReply.RoundtripTime;
                            CheckConnectCancelled();
                        }

                        if (FirstConnect)
                        {
                            FirstConnect = false;
                            deviceControl.Open(this.ClientConnectRequest);
                        }
                        else
                        {
                            deviceControl.Reconnect(this.ClientConnectRequest);
                        }

                        CheckConnectCancelled();

                        //OK, connection was succussful...
                        _deviceControl = deviceControl;
                        _deviceControl.Closed += new EventHandler(GraphControlClient_Faulted);

                        SessionDescription sessionDescription = _deviceControl.GetSessionDescription();
                        if (sessionDescription != null)
                        {
                            if (sessionDescription.StreamTimeLimit != 0)
                            {
                                tmrStreamTimeLimit.Interval = sessionDescription.StreamTimeLimit;
                                setTimeoutTimer = true;
                            }
                            //negotiate profile
                            if (sessionDescription.CurrentProfileIsCustom)
                            {
                                Profile customProfile = new Profile();
                                customProfile.Name = sessionDescription.CurrentProfileName;
                                CurrentProfile = customProfile;
                                CustomProfile = customProfile;
                            }
                            else
                            {
                                string[] fqpnParts = Profile.GetProfileNameParts(sessionDescription.CurrentProfileName);
                                foreach (ProfileGroup profileGroup in sessionDescription.ProfileGroups.Items)
                                {
                                    if (profileGroup.Name == fqpnParts[0])
                                    {
                                        CurrentProfile = profileGroup[fqpnParts[1]];
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new ServerGraphException("no session description");
                        }
                        CheckConnectCancelled();

                        //create the DirectShow graph
                        StartLocalGraph();

                        CheckConnectCancelled();

                        //indicate success and result
                        if (setTimeoutTimer)
                        {
                            tmrStreamTimeLimit.Enabled = true;
                        }
                        success = true;
                        e.Result = null;
                    }
                    catch (Exception exc)
                    {
                        ErrorLogger.DumpToDebug(exc);
                        e.Result = exc;

                        CloseLocalGraph();

                        _deviceControl = null;
                        if (deviceControl != null)
                        {
                            deviceControl.Dispose();
                        }

                        if ((exc is OperationCanceledException) ||
                            (exc.FaultExceptionDetailIsType(typeof(SourceHasMaxClientsException))) ||
                            (PendingConnection != null))
                        {
                            return;
                        }
                        else
                        {
                            // Reconnect logic here
                            // e.Result = new OperationCanceledException("Maximum allowed reconnect attempts reached");
                            // return;
                            // when algorithm logic cannot try again

                            Log("Reconnect attempt");
                            _reconnectCount++;
                            if (_reconnectCount > 5)
                            {
                                _reconnectTime = 8000;
                                ProgressMessage = "Waiting...";
                                for (int j = 0; (j < _reconnectTime) && connectWorker.CancellationPending == false; j++)
                                {
                                    Thread.Sleep(1000);
                                    j += 1000;
                                }
                            }
                            else
                            {
                                _reconnectTime = 2000;
                                ProgressMessage = "Waiting..";
                                for (int j = 0; (j < _reconnectTime) && connectWorker.CancellationPending == false; j++)
                                {
                                    Thread.Sleep(1000);
                                    j += 1000;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs any neccesary actions after the server connection has been established.
        /// </summary>
        /// <remarks>
        /// If the connection was dropped by the server, the user is notified.
        /// If the connection was successful, then it proceeds to connect and run the directshow graph.
        /// </remarks>
        private void connectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RunWorkerCompletedEventHandler(connectWorker_RunWorkerCompleted), new object[] { sender, e });
                return;
            }

            Log(">>>> " + this.Name + " connectWorker_RunWorkerCompleted");

            // if this connection ended in error
            //   but it is not in the process of being stopped, or is not yet stopped
            // then execute the disconnectWorker
            if ((e.Result is Exception) && (!disconnectWorker.IsBusy) && (this.State != StreamState.Stopped))
            {
                State = StreamState.Stopping;
                disconnectWorker.RunWorkerAsync();

                Exception exc = e.Result as Exception;
                if (exc.FaultExceptionDetailIsType(typeof(SourceHasMaxClientsException)))
                {
                    FCMessageBox.Show("Connect Aborted", exc.Message, this);
                }
            }
        }

        /// <summary>
        /// Does the work of disconnecting the SVC
        /// </summary>
        private void disconnectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (connectMutex)
            {
                Thread.CurrentThread.Name = this.Name + " disconnectWorker";
                Log(">>>> " + this.Name + " disconnectWorker_DoWork");

                try
                {
                    ClearConnectionInfo();

                    if (_graph != null)
                    {
                        _graph.Stop();
                    }
                    CloseLocalGraph();
                    CloseServerGraph();
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                    e.Result = ex;
                }
                finally
                {
                    State = StreamState.Stopped;
                }
            }
        }

        /// <summary>
        /// Finishes up the stop
        /// </summary>
        private void disconnectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new RunWorkerCompletedEventHandler(disconnectWorker_RunWorkerCompleted), new object[] { sender, e });
                return;
            }

            Log(">>>> " + this.Name + " disconnectWorker_RunWorkerCompleted");

            State = StreamState.Available;
            AverageBitRate = 0;
            Users.Clear();
            NotifyPropertyChanged("Users");
            P_Video.Refresh();

            ExecutePendingConnection();
        }

        /// <summary>
        /// Throws an exception to quit the currently-being-attempted connection if a cancellation is pending.
        /// </summary>
        private void CheckConnectCancelled()
        {
            if (connectWorker.CancellationPending)
            {
                throw new OperationCanceledException("Cancelled Connect to server");
            }
        }

        /// <summary>
        /// Stops the stream viewer.
        /// </summary>
        /// <remarks>
        /// Depending on the state, it may use a combination of approaches to stop any processes that are in progress
        /// </remarks>
        public void Stop()
        {
            if (((State == StreamState.Connecting) || (State == StreamState.Buffering)) && (connectWorker.IsBusy))
            {
                State = StreamState.Stopping;
                connectWorker.CancelAsync();
            }
            else if ((State == StreamState.Connecting) || (State == StreamState.Buffering) || (State == StreamState.Playing))
            {
                State = StreamState.Stopping;
                if (!disconnectWorker.IsBusy)
                {
                    disconnectWorker.RunWorkerAsync();
                }
            }
            else
            {
                Log(this.Name + ": Stop called when State is " + State.ToString());
            }
        }

        private void B_StopFull_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Clears the source info. Thread Safe.
        /// </summary>
        private void ClearConnectionInfo()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(ClearConnectionInfo));
                return;
            }

            CurrentConnection = null;
            ClientConnectRequest = null;
        }

        private void GraphControlClient_Faulted(object sender, EventArgs e)
        {
            Log("GraphControlClient_Faulted when state was " + State.ToString());

            //move to a good state for stopping
            if (State == StreamState.Recording)
            {
                _graph.RecordingFileName = null;
                State = StreamState.Playing;
            }

            if ((State == StreamState.Playing) || (State == StreamState.Buffering))
            {
                PendingConnection = CurrentConnection;
                Stop();
            }
        }

        #endregion

        #region DirectShow Graph Management

        /// <summary>
        /// Instatiates the DirectShow graph
        /// </summary>
        private void StartLocalGraph()
        {
            //if another thread had requested us to stop
            if ((this.State == StreamState.Stopping) || (this.State == StreamState.Stopped))
            {
                return;
            }

            if (string.IsNullOrEmpty(SessionDescription.SinkURL))
            {
                throw new Exception("No ClientURL provided...Server not ready?");
            }

            State = StreamState.Buffering;
            try
            {
                Debug.Assert(_graph == null);
                Graphs.GraphFactory graphFactory = new Graphs.GraphFactory();
                _graph = graphFactory.CreateGraph(SourceInfo.SourceType, _deviceControl.GetSessionDescription().SinkURL);
                _graph.HasAudio = SourceInfo.HasAudio;
                _graph.StreamName = Name;
                _graph.Complete += new EventHandler(Graph_Complete);
                _graph.PropertyChanged += new PropertyChangedEventHandler(Graph_PropertyChanged);
                _graph.TelemetryUpdate += new EventHandler<XmlTelemetryEventArgs>(Graph_TelemetryUpdate);
                _graph.Setup();
                _graph.Run();

                //if someone else snagged the state out from under us (e.g. Stopping),
                //we don't want to do an illegal transition
                if (State == StreamState.Buffering) 
                {
                    State = StreamState.Playing;
                }
            }
            catch (Exception e)
            {
                // creation of the local DirectShow graph failed
                CloseLocalGraph();

                throw;
            }
        }

        /// <summary>
        /// Disposes the DirectShow graph
        /// </summary>
        private void CloseLocalGraph()
        {
            if (_graph != null)
            {
                try
                {
                    _graph.Dispose();
                }
                catch (Exception exc)
                {
                    ErrorLogger.DumpToDebug(exc);
                }
                finally
                {
                    _graph = null;
                }
            }
        }

        /// <summary>
        /// Disposes the local graph, then notifies the server of the disconnect.
        /// </summary>
        private void CloseAllContextBackground()
        {
            CloseLocalGraph();
            CloseServerGraph();
        }

        /// <summary>
        /// Issues a disconnect to the server
        /// </summary>
        private void CloseServerGraph()
        {
            if (_deviceControl != null)
            {
                try
                {
                    _deviceControl.Closed -= new EventHandler(GraphControlClient_Faulted);
                    _deviceControl.Dispose();
                    _deviceControl = null;
                }
                catch (Exception exc)
                {
                    ErrorLogger.DumpToDebug(exc);
                }
            }
        }

        private void Graph_Complete(object sender, EventArgs e)
        {
            if (State == StreamState.Playing)
            {
                CloseLocalGraph();
                this.BeginInvoke(new Action(Run));
            }
            else
            {
                Stop();
            }
        }

        private void Graph_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new PropertyChangedEventHandler(Graph_PropertyChanged), new object[] { sender, e });
                return;
            }

            try
            {
                if (_graph == null)
                {
                    return;
                }

                if (e.PropertyName == "AvgBitRate")
                {
                    AverageBitRate = _graph.AvgBitRate / 1024;
                }
                else if (e.PropertyName == "VideoRender")
                {
                    _graph.SetNotifyWindow(this, Win32.WM_USER + 1);
                    _graph.AttachControl(P_Video);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void Graph_TelemetryUpdate(object sender, XmlTelemetryEventArgs e)
        {
            XmlReader xml = e.XmlReader;
            if (xml.Read())
            {
                if (xml.Read())
                {
                    if (xml.Name.Equals("Profile") && xml.IsStartElement())
                    {
                        try
                        {
                            CurrentProfile = Parser.ReadProfile(xml);
                        }
                        catch (Exception exc)
                        {
                            ErrorLogger.DumpToDebug(exc);
                        }
                    }
                    else if (xml.Name.Equals("Clients") && xml.IsStartElement())
                    {
                        Users = Parser.ReadClientList(xml);
                    }
                }
            }
            xml.Close();
        }

        private void LocationChanged_Handler(object sender, EventArgs e)
        {
            Log("LocationChanged");
        }

        #endregion

        /// <summary>
        /// Override the control's window process to intercept messages that the DirectShow graph
        /// needs to know about.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_USER + 1) // it's a IMediaEvent
            {
         //       if ((_graph != null) && (State == StreamState.Playing))
                if (_graph != null)
                {
                    try
                    {
                        _graph.NotifyMediaEvent();
                    }
                    catch (Exception exc)
                    {
                        ErrorLogger.DumpToDebug(exc);
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}
