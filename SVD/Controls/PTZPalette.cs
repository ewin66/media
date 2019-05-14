using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using FutureConcepts.Media.CommonControls;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.Client.StreamViewer;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;
using System.Collections.Generic;

namespace FutureConcepts.Media.SVD.Controls
{
    /// <summary>
    /// Encapsulates all PTZ functionality
    /// </summary>
    public partial class PTZPalette : UserControl, IControlPalette
    {
        public PTZPalette()
        {
            this.HandleCreated += new EventHandler(PTZPalette_HandleCreated);
            InitializeComponent();
        }

        #region Fix for Mantis#1518

        void PTZPalette_HandleCreated(object sender, EventArgs e)
        {
            windowHandleCreated = true;
        }

        /// <summary>
        /// is true if the window handle has been created
        /// </summary>
        private bool windowHandleCreated = false;

        /// <summary>
        /// this queue stores any property changes that have come off of the client before the window handle was created
        /// </summary>
        private Queue<string> pendingPropertyChanges = new Queue<string>();

        #endregion

        private StreamViewerControl _svc;
        /// <summary>
        /// The currently bound StreamViewerControl
        /// </summary>
        [Browsable(false)]
        public StreamViewerControl StreamViewerControl
        {
            get
            {
                return _svc;
            }
            set
            {
                if (_svc != null)
                {
                    Stop();
                }

                _svc = value;

                if (value != null)
                {
                    //TODO something?
                }
            }
        }

        /// <summary>
        /// This palette is the PTZ palette
        /// </summary>
        public SourceControlTypes ControlType
        {
            get
            {
                return SourceControlTypes.PTZ;
            }
        }

        private bool ChangingSelfValues { get; set; }

        void SVC_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("State"))
            {
                switch (StreamViewerControl.State)
                {
                  //  case StreamState.Available:
                    case StreamState.Connecting:
                    case StreamState.Buffering:
                    case StreamState.Stopping:
                    case StreamState.Stopped:
                        Stop();
                        break;
                }
            }
        }

        #region Camera Control Client

        private ICameraControlClient client;

        /// <summary>
        /// Connects to the server and opens camera control
        /// </summary>
        public void Start()
        {
            if (StreamViewerControl == null)
            {
                throw new ArgumentNullException("StreamViewerControl must not be null!");
            }

            if (StreamViewerControl.SourceInfo.CameraControl == null)
            {
                throw new ArgumentNullException("StreamViewerControl.SourceInfo.CameraControl may not be null!");
            }

            if (StreamViewerControl.SourceInfo.CameraControl.PTZType == PTZType.Null)
            {
                throw new NotSupportedException("PTZType.Null is not supported!");
            }

//            if(string.IsNullOrEmpty(StreamViewerControl.ServerAddress))
//            {
//                throw new ArgumentException("StreamViewerControl.ServerAddress must not be empty!");
//            }

            //start listening to SVC's property events
            _svc.PropertyChanged += new PropertyChangedEventHandler(SVC_PropertyChanged);

            client = FutureConcepts.Media.Client.CameraControlClients.ClientFactory.Create(StreamViewerControl.ServerAddress, StreamViewerControl.SourceInfo);

            client.Opened += new EventHandler(client_Opened);
            client.Closed += new EventHandler(client_Closed);
            client.PropertyChanged += new PropertyChangedEventHandler(client_PropertyChanged);

            client.Open(new ClientConnectRequest(StreamViewerControl.SourceInfo.SourceName));
        }

        public void Stop()
        {
            try
            {
                if (client != null)
                {
                    try
                    {
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        DoCleanup();
                        throw ex;
                    }
                }
                else
                {
                    DoCleanup();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
            finally
            {
                FireClosed();
            }

        }

        /// <summary>
        /// Performs cleanup of all event handlers and such
        /// </summary>
        private void DoCleanup()
        {
            _svc.PropertyChanged -= new PropertyChangedEventHandler(SVC_PropertyChanged);
            ptzFavoritesControl.SaveThumbnailImages();

            Debug.WriteLine("PTZPalette.DoCleanup()");

            if (client != null)
            {
                client.Opened -= new EventHandler(client_Opened);
                client.Closed -= new EventHandler(client_Closed);
                client.PropertyChanged -= new PropertyChangedEventHandler(client_PropertyChanged);

                client = null;
            }

            if (StreamViewerControl.PTZOverlayControl != null)
            {
                StreamViewerControl.PTZOverlayControl.MoveRequested -= new EventHandler<PTZMoveRequestEventArgs>(PTZOverlayControl_MoveRequested);
                StreamViewerControl.PTZOverlayControl = null;
            }
        }

        /// <summary>
        /// Initializes controls when the camera control has been opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void client_Opened(object sender, EventArgs e)
        {
            try
            {
                client.RelinquishTimeout = StreamViewerControl.SourceInfo.CameraControl.RelinquishTimer;
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                this.Stop();
            }
        }

        /// <summary>
        /// Handles the closure or faulting of the camera control client
        /// </summary>
        private void client_Closed(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new EventHandler(client_Closed), new object[2] { sender, e });
                    return;
                }

                DoCleanup();
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
            finally
            {
                FireClosed();
            }
        }

        private void client_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if no window handle has been created, we should just queue this change instead of executing it
            // because that means that InvokeRequired will not return the proper value, and we will execute
            // the resulting code on a non-GUI thread, causing dead lock
            //  Mantis: 1518
            if (!windowHandleCreated)
            {
                pendingPropertyChanges.Enqueue(e.PropertyName);
                return;
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new PropertyChangedEventHandler(client_PropertyChanged), new object[] { sender, e });
                return;
            }

            ChangingSelfValues = true;
            if (client != null)
            {
                switch (e.PropertyName)
                {
                    case "CurrentPanAngle":
                        ptzControl.PanAngle = client.CurrentPanAngle;
                        break;
                    case "CurrentTiltAngle":
                        ptzControl.TiltAngle = client.CurrentTiltAngle;
                        break;
                    case "CurrentZoomPosition":
                        ptzControl.ZoomLevel = client.CurrentZoomPosition;
                        UpdatePTZOverlay();
                        break;
                    case "EmitterEnabled":
                        SetFCClickButtonEnabled(B_Emitter, client.EmitterEnabled);
                        break;
                    case "StabilizerEnabled":
                        SetFCClickButtonEnabled(B_Stabilizer, client.StabilizerEnabled);
                        break;
                    case "InfraredEnabled":
                        SetFCClickButtonEnabled(B_Infrared, client.InfraredEnabled);
                        break;
                    case "InvertedEnabled":
                        SetFCClickButtonEnabled(B_Invert, client.InvertedEnabled);
                        break;
                    case "WiperEnabled":
                        SetFCClickButtonEnabled(B_Wiper, client.WiperEnabled);
                        break;
                    case "PresetItems":
                        try
                        {
                            Debug.WriteLine("SVDMain camctl PropertyChanged PresetItems");
                            Debug.WriteLine("PresetItems.count = " + client.PresetItems.Count);
                            ptzFavoritesControl.UpdateItems(client.PresetItems);
                        }
                        catch (Exception exc)
                        {
                            ErrorLogger.DumpToDebug(exc);
                        }
                        break;
                    case "StatusMessage":
                        FCMessageBox.Show("Camera Control Notification", client.StatusMessage, this);
                        break;
                }
            }
            ChangingSelfValues = false;
        }

        #endregion

        #region UI State Management

        private void PTZPalette_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                try
                {
                    InitControls(StreamViewerControl.SourceInfo.CameraControl);
                    //execute any property changes that happened before we were visible
                    while (pendingPropertyChanges.Count > 0)
                    {
                        client_PropertyChanged(client, new PropertyChangedEventArgs(pendingPropertyChanges.Dequeue()));
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                    this.Stop();
                }
            }
        }

        private void InitControls(CameraControlInfo cc)
        {
            if(cc == null)
            {
                throw new ArgumentNullException("CameraCapabilitiesAndLimits may not be null!");
            }

            B_Emitter.Visible = cc.Capabilities.HasEmitter;
            B_Infrared.Visible = cc.Capabilities.HasInfrared;
            B_Invert.Visible = cc.Capabilities.HasInverter;
            B_Stabilizer.Visible = cc.Capabilities.HasStabilizer;
            B_Wiper.Visible = cc.Capabilities.HasWiper;

            ptzControl.PanEnabled = cc.Capabilities.HasPan;
            ptzControl.TiltEnabled = cc.Capabilities.HasTilt;
            ptzControl.ZoomEnabled = cc.Capabilities.HasZoom;

            ptzFavoritesControl.Visible = cc.Capabilities.HasAbsoluteControls;

            if (cc.Capabilities.HasEmitter)
            {
                client.EmitterEnabled = false;
                B_Emitter.ImageIndex = 0;
            }
            if (cc.Capabilities.HasInfrared)
            {
                client.InfraredEnabled = false;
                B_Infrared.ImageIndex = 0;
            }
            if (cc.Capabilities.HasInverter)
            {
                client.InvertedEnabled = false;
                B_Invert.ImageIndex = 0;
            }
            if (cc.Capabilities.HasStabilizer)
            {
                client.StabilizerEnabled = true;
                B_Stabilizer.ImageIndex = 1;
            }
            if (cc.Capabilities.HasWiper)
            {
                client.WiperEnabled = false;
                B_Wiper.ImageIndex = 0;
            }

            ArrangePTZOptions();

            ChangingSelfValues = true;

            //set pan angle
            ptzControl.PanAngle = client.CurrentPanAngle;
            ptzControl.PanAngleOffset = cc.Capabilities.PanOffset;

            //set the tilt level
            ptzControl.TiltAngle = client.CurrentTiltAngle;

            ptzControl.TiltAngleMinimum = cc.Capabilities.TiltMinAngle;
            ptzControl.TiltAngleMaximum = cc.Capabilities.TiltMaxAngle;
            ptzControl.TiltAngleCustomTicks = new double[] { ptzControl.TiltAngleMinimum, 0, ptzControl.TiltAngleMaximum };

            //set field of view
            ptzControl.CameraFieldOfView = cc.Capabilities.FieldOfView;

            //set zoom level
            ptzControl.ZoomLevel = client.CurrentZoomPosition;

            ptzControl.ZoomLevelMinimum = cc.Capabilities.ZoomMinLevel;
            ptzControl.ZoomLevelMaximum = cc.Capabilities.ZoomMaxLevel;
            ptzControl.ZoomLevelTickFrequency = 10;

            ChangingSelfValues = false;

            if (ptzControl.GetCurrentFieldOfView() != 0)
            {
                this.StreamViewerControl.PTZOverlayControl = new ClickToCenter();
                this.StreamViewerControl.PTZOverlayControl.FieldOfView = ptzControl.GetCurrentFieldOfView();
                this.StreamViewerControl.PTZOverlayControl.MoveRequested += new EventHandler<PTZMoveRequestEventArgs>(PTZOverlayControl_MoveRequested);
                this.StreamViewerControl.PTZOverlayControl.Enabled = true;
            }
        }

        /// <summary>
        /// Arranges the visible PTZ Options buttons
        /// </summary>
        private void ArrangePTZOptions()
        {
            int x = 5;
            int y = 14;

            int visibleControls = 0;
            foreach (Control ctl in GB_PTZOptions.Controls)
            {
                if (ctl.Visible)
                {
                    ctl.Left = x;
                    ctl.Top = y;
                    x += (ctl.Width + 2);
                    if (x > ctl.Width * 2)
                    {
                        x = 5;
                        y += (ctl.Height + 2);
                    }
                    visibleControls++;
                }
            }

            GB_PTZOptions.Visible = (visibleControls > 0);
            if (visibleControls == 0)
            {
                this.Width = ptzControl.Width + ptzFavoritesControl.Width;
            }
            else
            {
                this.Width = ptzControl.Width + GB_PTZOptions.Width + ptzFavoritesControl.Width;
            }
        }

        private void SetFCClickButtonEnabled(FCClickButton button, bool enabled)
        {
            if (enabled == true)
            {
                button.ImageIndex = 1;
            }
            else
            {
                button.ImageIndex = 0;
            }
        }

        /// <summary>
        /// Forwards any neccesary changes to the current PTZ Overlay Control
        /// </summary>
        private void UpdatePTZOverlay()
        {
            if (StreamViewerControl.PTZOverlayControl != null)
            {
                StreamViewerControl.PTZOverlayControl.FieldOfView = ptzControl.GetCurrentFieldOfView();
            }
        }

        [Category("Action"), Description("This event is raised when the control can do no more work and should be closed.")]
        public event EventHandler<SourceControlTypeEventArgs> Closed;
        private void FireClosed()
        {
            if (Closed != null)
            {
                Closed.Invoke(this, new SourceControlTypeEventArgs(this.ControlType));
            }
        }

        #endregion

        #region Pan Tilt Zoom

        /// <summary>
        /// Called when PanAngleChanged event occurs
        /// </summary>
        private void ptzControl_PanChanged(object sender, System.EventArgs e)
        {
            if ((client == null) || (ChangingSelfValues))
            {
                return;
            }

            try
            {
                client.PanTiltAbsolute(ptzControl.PanAngle, ptzControl.TiltAngle);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                Stop();
            }
        }

        /// <summary>
        /// Called when TiltAngleChanged events occurs
        /// </summary>
        private void ptzControl_TiltChanged(object sender, System.EventArgs e)
        {
            if ((client == null) || (ChangingSelfValues))
            {
                return;
            }

            try
            {
                client.PanTiltAbsolute(ptzControl.PanAngle, ptzControl.TiltAngle);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                Stop();
            }

        }

        /// <summary>
        /// Called when the ZoomLevelChanged event occurs
        /// </summary>
        private void ptzControl_ZoomLevelChanged(object sender, System.EventArgs e)
        {
            UpdatePTZOverlay();

            if ((client == null) || (ChangingSelfValues))
            {
                return;
            }

            try
            {
                client.ZoomAbsolute(ptzControl.ZoomLevel);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                Stop();
            }
        }

        /// <summary>
        /// Dispatches a move request from a PTZ Overlay Control to the camera control client
        /// </summary>
        private void PTZOverlayControl_MoveRequested(object sender, PTZMoveRequestEventArgs e)
        {
            if (client == null)
            {
                return;
            }

            try
            {
                Debug.WriteLine("Got Move Request: " + e.Pan + ", " + e.Tilt + ", " + e.Zoom);

                if (e.IsAbsolute)
                {
                   client.PanTiltAbsolute(e.Pan, e.Tilt);
                    if (e.Zoom != 0)
                    {
                        client.ZoomAbsolute(e.Zoom);
                    }
                }
                else
                {
                    double newPan = FCCircularTrackbar.FitValueToCircle(ptzControl.PanAngle + e.Pan);

                    double newTilt = ptzControl.TiltAngle + e.Tilt;

                    if (newTilt > StreamViewerControl.SourceInfo.CameraControl.Capabilities.TiltMaxAngle)
                    {
                        newTilt = StreamViewerControl.SourceInfo.CameraControl.Capabilities.TiltMaxAngle;
                    }
                    else if (newTilt < StreamViewerControl.SourceInfo.CameraControl.Capabilities.TiltMinAngle)
                    {
                        newTilt = StreamViewerControl.SourceInfo.CameraControl.Capabilities.TiltMinAngle;
                    }

                    client.PanTiltAbsolute(newPan, newTilt);
                    if (e.Zoom != 0)
                    {
                        client.ZoomAbsolute(ptzControl.ZoomLevel + e.Zoom);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                Stop();
            }
        }

        #endregion

        #region Camera Options

        private void Stabilizer_Click(object sender, System.EventArgs e)
        {
            if (client != null)
            {
                client.SetStabilizer(!client.StabilizerEnabled);
            }
        }

        private void Emitter_Click(object sender, System.EventArgs e)
        {
            if (client != null)
            {
                client.SetEmitter(!client.EmitterEnabled);
            }
        }

        private void Infrared_Click(object sender, System.EventArgs e)
        {
            if (client != null)
            {
                client.SetInfrared(!client.InfraredEnabled);
            }
        }

        private void Invert_Click(object sender, System.EventArgs e)
        {
            if (client != null)
            {
                client.SetOrientation(!client.InvertedEnabled);
            }
        }

        private void Wiper_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.SetWiper(!client.WiperEnabled);
            }
        }

        #endregion

        #region PTZ Favorites Handling

        /// <summary>
        /// Called when the Favorites control needs a favorite added to it
        /// </summary>
        private void ptzFavoritesControl_AddFavorite(object sender, EventArgs e)
        {
            try
            {
                if (client != null)
                {
                    UserPresetItem preset = client.SavePreset();

                    if (preset != null)
                    {
                        UserPresetItemView view = new UserPresetItemView();
                        view.Image = StreamViewerControl.GetSnapshot();
                        view.Preset = preset;
                        ptzFavoritesControl.Add(view);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                Stop();
            }
        }

        private void ptzFavoritesControl_PresetsCleared(object sender, EventArgs e)
        {
            try
            {
                if (client != null)
                {
                    client.DeleteAllPresets();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void ptzFavoritesControl_PresetRenamed(object sender, UserPresetEventArgs e)
        {
            try
            {
                if (client != null)
                {
                    client.UpdatePreset(e.Item.Preset);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void ptzFavoritesControl_PresetDeleted(object sender, UserPresetEventArgs e)
        {
            try
            {
                if (client != null)
                {
                    client.DeletePreset(e.Item.Preset.ID);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private UserPresetSnapshotTimer _snapshotTimer = new UserPresetSnapshotTimer();

        /// <summary>
        /// Called when a favorite needs to be restored
        /// </summary>
        /// <param name="sender">the sending object</param>
        /// <param name="restore">the PTZFavorite to restore. It is passed by reference in case any updates need to be done to it.</param>
        private void ptzFavoritesControl_RestorePreset(object sender, UserPresetEventArgs update)
        {
            try
            {
                if (client != null)
                {

                    client.RestorePreset(update.Item.Preset.ID);

                    _snapshotTimer.TakeSnapshot(update.Item, StreamViewerControl);

                    ptzFavoritesControl.Refresh();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #endregion
    }
}
