using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FutureConcepts.Media.CommonControls;
using FutureConcepts.Media.Client.StreamViewer;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;
using System.Text;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class MicrowavePalette : UserControl, IControlPalette
    {
        public MicrowavePalette()
        {
            this.HandleCreated += new EventHandler(MicrowavePalette_HandleCreated);
            InitializeComponent();
            this.VisibleChanged += new EventHandler(MicrowavePalette_VisibleChanged);

            UpdateBusyStatus(0);
        }

        #region Mantis#1518 fix

        void MicrowavePalette_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                while (pendingPropertyChanges.Count > 0)
                {
                    client_PropertyChanged(client, new PropertyChangedEventArgs(pendingPropertyChanges.Dequeue()));
                }
            }
        }

        private bool windowHandleCreated = false;

        void MicrowavePalette_HandleCreated(object sender, EventArgs e)
        {
            windowHandleCreated = true;
        }

        private Queue<string> pendingPropertyChanges = new Queue<string>();

        #endregion

        #region IControlPalette Members

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

        public SourceControlTypes ControlType
        {
            get { return SourceControlTypes.Microwave; }
        }

        public event EventHandler<SourceControlTypeEventArgs> Closed;

        private Client.MicrowaveControl2 client;

        public void Start()
        {
            if (StreamViewerControl == null)
            {
                throw new ArgumentNullException("StreamViewerControl must not be null!");
            }
            if (StreamViewerControl.SourceInfo == null)
            {
                throw new ArgumentNullException("StreamViewerControl.SourceInfo may not be null!");
            }
            if (StreamViewerControl.SourceInfo.MicrowaveControl == null)
            {
                throw new ArgumentNullException("StreamViewerControl.SourceInfo.CameraControl may not be null!");
            }

            if (string.IsNullOrEmpty(StreamViewerControl.ServerAddress))
            {
                throw new ArgumentException("StreamViewerControl.ServerAddress must not be empty!");
            }

            client = new Client.MicrowaveControl2(StreamViewerControl.ServerAddress);

            client.Closed += new EventHandler(client_Closed);

            client.Open(new ClientConnectRequest(StreamViewerControl.SourceInfo.SourceName));

            client.RelinquishTimeout = StreamViewerControl.SourceInfo.MicrowaveControl.RelinquishTimer;

            client.PropertyChanged += new PropertyChangedEventHandler(client_PropertyChanged);
            client.ChannelScanStarted += new EventHandler(client_ChannelScanStarted);
            client.ChannelScanProgressUpdate += new EventHandler(client_ChannelScanProgressUpdate);
            client.ChannelScanComplete += new EventHandler<ChannelScanCompleteEventArgs>(client_ChannelScanComplete);

            //start listening to SVC's property events
            _svc.PropertyChanged += new PropertyChangedEventHandler(SVC_PropertyChanged);

        }

        void SVC_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("State"))
            {
                switch (StreamViewerControl.State)
                {
                    case StreamState.Connecting:
                    case StreamState.Buffering:
                    case StreamState.Stopping:
                    case StreamState.Stopped:
                        Stop();
                        break;
                }
            }
        }

        [Flags]
        private enum BusyFlags
        {
            None = 0,
            Capabilities = 1,
            Tuning = 2,
            LQ = 4,
            Presets = 8,
            Complete = 15,
            Disable = 16
        };
        private BusyFlags _busyFlag = 0;
        /// <summary>
        /// Helps keep the user informed of what crap we are waiting on from the server
        /// </summary>
        /// <param name="flag">latest received flag. Pass 0 to reset</param>
        private void UpdateBusyStatus(BusyFlags flag)
        {

            if (flag == 0)
            {
                _busyFlag = 0;
            }
            else if (_busyFlag > BusyFlags.Complete)
            {
                return;
            }
            else
            {
                _busyFlag |= flag;
            }

            if (_busyFlag == BusyFlags.Complete)
            {
                busy.Visible = false;
                tuner.Visible = true;
                presets.Visible = true;
                sweep.Visible = false;  // was true;
                _busyFlag = BusyFlags.Disable;
            }
            else
            {
                tuner.Visible = false;
                sweep.Visible = false;
                presets.Visible = false;
                busy.Visible = true;
            }

            StringBuilder stuffWeNeed = new StringBuilder();
            stuffWeNeed.Append("Waiting for ");

            if ((_busyFlag & BusyFlags.Capabilities) != BusyFlags.Capabilities)
            {
                stuffWeNeed.Append("Capabilities");
                if ((int)_busyFlag < 14)
                {
                    stuffWeNeed.Append(", ");
                }
            }
            if ((_busyFlag & BusyFlags.Tuning) != BusyFlags.Tuning)
            {
                stuffWeNeed.Append("Current Tuning");
                if ((int)_busyFlag < 13)
                {
                    stuffWeNeed.Append(", ");
                }
            }
            if ((_busyFlag & BusyFlags.LQ) != BusyFlags.LQ)
            {
                stuffWeNeed.Append("Link Quality");
                if ((int)_busyFlag < 11)
                {
                    stuffWeNeed.Append(", ");
                }
            }
            if ((_busyFlag & BusyFlags.Presets) != BusyFlags.Presets)
            {
                stuffWeNeed.Append("Presets");
            }

            stuffWeNeed.Append("...");

            busy.InfoText = stuffWeNeed.ToString();

        }

        void client_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (!windowHandleCreated)
                {
                    pendingPropertyChanges.Enqueue(e.PropertyName);
                    return;
                }

                if (this.Disposing)
                {
                    return;
                }

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new PropertyChangedEventHandler(client_PropertyChanged), new object[] { sender, e });
                    return;
                }

                switch (e.PropertyName)
                {
                    case "Capabilities":
                        if (client.Capabilities != null)
                        {
                            UpdateBusyStatus(BusyFlags.Capabilities);

                            tuner.Configure(client.Capabilities);

                            sweep.Configure(client.Capabilities);

                            if (client.Capabilities.SupportedEncryption != null)
                            {
                                keyInput.Capabilities = client.Capabilities.SupportedEncryption;
                            }
                        }

                        break;
                    case "CurrentTuning":
                        if (client.CurrentTuning != null)
                        {
                            UpdateBusyStatus(BusyFlags.Tuning);

                            tuner.UpdateTuning(client.CurrentTuning);

                            if (client.CurrentTuning.IsSet(MicrowaveTuning.Parameters.Encryption))
                            {
                                if (client.CurrentTuning.Encryption.Type == EncryptionType.None)
                                {
                                    keyInput.SelectedEncryption = null;
                                }
                                else
                                {
                                    keyInput.SelectedEncryption = client.CurrentTuning.Encryption;
                                }
                                tuner.EncryptionKeyActive = keyInput.SelectedEncryption != null;
                            }
                        }
                        break;
                    case "LinkQuality":
                        if (client.LinkQuality != null)
                        {
                            UpdateBusyStatus(BusyFlags.LQ);

                            tuner.UpdateLinkQuality(client.LinkQuality);
                        }
                        break;
                    case "FrequencyPresets":
                        if (client.FrequencyPresets != null)
                        {
                            UpdateBusyStatus(BusyFlags.Presets);

                            presets.UpdateItems(client.FrequencyPresets);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        void client_Closed(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new EventHandler(client_Closed), new object[] { sender, e });
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

        private void DoCleanup()
        {
            _svc.PropertyChanged -= new PropertyChangedEventHandler(SVC_PropertyChanged);

            if (presets != null)
            {
                presets.SaveThumbnailImages();
            }

            if (client != null)
            {
                client.Closed -= new EventHandler(client_Closed);
                client.PropertyChanged -= new PropertyChangedEventHandler(client_PropertyChanged);
                client.ChannelScanStarted -= new EventHandler(client_ChannelScanStarted);
                client.ChannelScanProgressUpdate -= new EventHandler(client_ChannelScanProgressUpdate);
                client.ChannelScanComplete -= new EventHandler<ChannelScanCompleteEventArgs>(client_ChannelScanComplete);
                client = null;
            }
        }

        private void FireClosed()
        {
            if (Closed != null)
            {
                Closed(this, new SourceControlTypeEventArgs(this.ControlType));
            }
        }

        #endregion

        #region Tuner

        private void tuner_FrequencyChanged(object sender, EventArgs e)
        {
            try
            {
                if (client != null)
                {
                    MicrowaveTuning newTune = new MicrowaveTuning(client.Capabilities.AutoTuning);
                    newTune.Frequency = tuner.Frequency;

                    if (keyInput.SelectedEncryption != null)
                    {
                        newTune.Encryption = keyInput.SelectedEncryption;
                    }
                    client.SetTuning(newTune);
                }
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Error setting frequency!", @"There was an error while setting the receiver's frequency on the server.", this);
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #endregion

        #region Frequency Sweep

        private void sweep_StartSweep(object sender, EventArgs e)
        {
            try
            {
                //TODO sweep frequency
                client.ScanStartFrequency = ((UInt64)sweep.StartFrequency * (UInt64)1000000);
                client.ScanEndFrequency = ((UInt64)sweep.EndFrequency * (UInt64)1000000);
                client.ScanThreshold = sweep.Threshold;
                client.StartChannelScan();
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Error starting frequency sweep!", @"There was an error starting the frequency sweep.", this);
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void sweep_CancelSweep(object sender, EventArgs e)
        {
            try
            {
                client.CancelChannelScan();
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        void client_ChannelScanStarted(object sender, EventArgs e)
        {
            sweep.SetSweepStarted();
        }

        void client_ChannelScanProgressUpdate(object sender, EventArgs e)
        {
            sweep.Progress = client.ChannelScanProgress;
        }

        void client_ChannelScanComplete(object sender, ChannelScanCompleteEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(sweep.SetSweepComplete));
            }
            else
            {
                sweep.SetSweepComplete();
            }
        }

        #endregion

        #region Frequency Presets

        private void presets_AddPreset(object sender, EventArgs e)
        {
            try
            {
                MicrowaveTuningPreset item = client.SavePreset() as MicrowaveTuningPreset;
                if (item != null)
                {
                    UserPresetItemView view = new UserPresetItemView();
                    view.Preset = item;
                    view.Image = StreamViewerControl.GetSnapshot();
                    presets.Add(view);
                }
            }
            catch (Exception ex)
            {
                FCMessageBox.Show(@"Can't add preset!", "There was an error fetching the preset from the server.", this);
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void presets_PresetDeleted(object sender, UserPresetEventArgs e)
        {
            try
            {
                client.DeletePreset(e.Item.Preset.ID);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void presets_PresetRenamed(object sender, UserPresetEventArgs e)
        {
            try
            {
                ((MicrowaveTuningPreset)e.Item.Preset).AutoGenerated = false;
                client.UpdatePreset(e.Item.Preset);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void presets_PresetsCleared(object sender, EventArgs e)
        {
            try
            {
                client.DeleteAllPresets();
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private UserPresetSnapshotTimer snapshotTimer = new UserPresetSnapshotTimer();

        private void presets_RestorePreset(object sender, UserPresetEventArgs e)
        {
            try
            {
                client.RestorePreset(e.Item.Preset.ID);

                UpdateBusyStatus(0);
                UpdateBusyStatus(BusyFlags.Complete & ~BusyFlags.Tuning & ~BusyFlags.LQ);

                snapshotTimer.TakeSnapshot(e.Item, StreamViewerControl);
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Could not set restore preset", "There was an error while attempting to restore the preset.", this);
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #endregion

        private EncryptionInfo _lastEncInfo = null;


        private void tuner_EnterEncryptionKey(object sender, EventArgs e)
        {
            tuner.InputEnabled = false;


            _lastEncInfo = keyInput.SelectedEncryption;

            sweep.Visible = false;
            presets.Visible = false;
            
            tuner.Visible = false;
            keyInput.Visible = true;
            
            
        }

        private void keyInput_DialogClosed(object sender, CancelEventArgs e)
        {
            tuner.InputEnabled = true;

            if (e.Cancel)
            {
                keyInput.SelectedEncryption = _lastEncInfo;
            }

            tuner.EncryptionKeyActive = keyInput.SelectedEncryption != null;

            if (!e.Cancel)
            {
                tuner_FrequencyChanged(this, EventArgs.Empty);
            }

            tuner.Visible = true;
            keyInput.Visible = false;
            sweep.Visible = false;  //was true
            presets.Visible = true;
        }
    }
}
