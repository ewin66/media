using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client.CameraControlClients.Protocol
{
    /// <summary>
    /// Implements what little support for PelcoD the Wonwoo WCC-261 camera uses
    /// Implemented for RS-485
    /// </summary>
    public class WonwooWCC261 : PelcoD, IPresetProvider
    {
        /// <summary>
        /// Creates an instance of the plugin
        /// </summary>
        /// <param name="config">configuration information</param>
        /// <param name="cameraControl">owning camera control</param>
        public WonwooWCC261(ICameraControlClient client, Transport.ITransport transport)
            : base(client, transport)
        {
            _stopTimer = new Timer( (x) => {
                SendStopCommand(null);
                }, null, Timeout.Infinite, Timeout.Infinite);

            BuildIndexingArray();
        }

        public override bool HasAbsoluteControls
        {
            get
            {
                return false;
            }
        }

        public override bool HasPan
        {
            get
            {
                return true;
            }
        }

        public override bool HasTilt
        {
            get
            {
                return true;
            }
        }

        public override bool HasZoom
        {
            get
            {
                return true;
            }
        }

        public override bool HasDigitalZoom
        {
            get
            {
                return false;
            }
        }

        public override bool HasEmitter
        {
            get
            {
                return false;
            }
        }

        public override bool HasStabilizer
        {
            get
            {
                return false;
            }
        }

        public override bool HasInfrared
        {
            get
            {
                return false;
            }
        }

        public override bool HasInverter
        {
            get
            {
                return false;
            }
        }

        public override bool HasWiper
        {
            get
            {
                return false;
            }
        }

        public override bool HasFocus
        {
            get
            {
                return false;
            }
        }

        public override double PanLimitStart
        {
            get
            {
                return 0;
            }
        }

        public override double PanLimitAngle
        {
            get
            {
                return 0;
            }
        }

        public override double PanOffset
        {
            get
            {
                return 0;
            }
        }

        public override double TiltMaxAngle
        {
            get
            {
                return 45.0;
            }
        }

        public override double TiltMinAngle
        {
            get
            {
                return -45.0;
            }
        }

        public override double ZoomMaxLevel
        {
            get
            {
                return 20.0;
            }
        }

        public override double ZoomMinLevel
        {
            get
            {
                return -20.0;
            }
        }

        public override double FieldOfView
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Disposes the camera control
        /// </summary>
        public override void Dispose()
        {
            //Make sure the camera stops moving before we quit
            SendStopCommand(null);

            base.Dispose();
        }

        private Timer _stopTimer;

        /// <summary>
        /// Returns the pan speed byte to use
        /// </summary>
        protected virtual byte PanSpeedByte
        {
            get { return 0x40; }
        }

        /// <summary>
        /// Returns the tilt speed byte to use
        /// </summary>
        protected virtual byte TiltSpeedByte
        {
            get { return 0x3F; }
        }  

        /// <summary>
        /// Returns the zoom speed factor to use, in units per second
        /// </summary>
        protected virtual double ZoomSpeedFactor
        {
            get { return 3.75; }
        }

        /// <summary>
        /// Receives the response from the camera. In this case, the camera cannot send a response, so this method simply generates a
        /// <see cref="F:ResponseTypes.NoResponse"/>.
        /// </summary>
        /// <param name="response">a 0-length array of bytes</param>
        /// <param name="waitForResponse">not used</param>
        /// <returns><see cref="F:ResponseTypes.NoResponse"/></returns>
        protected override ResponseTypes ReceiveResponse(out byte[] response, bool waitForResponse)
        {
            response = new byte[] {};
            return ResponseTypes.NoResponse;
        }

        #region Capability Support

        /// <summary>
        /// The WCC261, when its firmware is configured correctly, will auto-switch to IR capture mode in low-light settings.
        /// This feature cannot be controlled.
        /// </summary>
        /// <param name="enabled">n/a</param>
        /// <exception cref="System.NotSupportedException">This model of camera does not support this setting</exception>
        public override void SetInfrared(bool enabled)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The WCC261 is always stabilized. This feature cannot be controlled.
        /// </summary>
        /// <param name="enabled">n/a</param>
        /// <exception cref="System.NotSupportedException">This model of camera does not support this setting</exception>
        public override void SetStabilizer(bool enabled)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The WCC261 does not have a lens wiper.
        /// </summary>
        /// <param name="enabled">n/a</param>
        /// <exception cref="System.NotSupportedException">This model of camera does not support this setting</exception>
        public override void SetWiper(bool enabled)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The WCC261 does not have an IR emitter capability
        /// </summary>
        /// <param name="enabled">n/a</param>
        /// <exception cref="System.NotSupportedException">This model of camera does not support this setting</exception>
        public override void SetEmitter(bool enabled)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// The WCC261 can be configured in firmware to invert the image
        /// </summary>
        /// <param name="inverted"></param>
        /// <exception cref="System.NotSupportedException">This model of camera does not support this setting</exception>
        public override void SetOrientation(bool inverted)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Movement Support

        /// <summary>
        /// Executes the Pan and Tilt movements as relative commands, in that order.
        /// </summary>
        /// <param name="panAngle">Amount to pan. Angles between 180 and 360 are interpreted as relative counter-clockwise movement (360 - angle)</param>
        /// <param name="tiltAngle">Amount to tilt.</param>
        public override void PanTiltAbsolute(double panAngle, double tiltAngle)
        {
            //setup left/right panning
            if (panAngle > 180)
            {
                panAngle = panAngle - 360;
            }

            if (panAngle != 0)
            {
                SendCommandSleepStop(0x00, (byte)((panAngle < 0) ? 0x04 : 0x02),
                                     PanSpeedByte, 0x00,
                                     GetPanTime(panAngle));

                //HACK: to force Client.CameraControl to propogate PropertyChanged notices
                _client.CurrentPanAngle = panAngle;
                _client.CurrentPanAngle = 0;
            }

            if (tiltAngle != 0)
            {
                SendCommandSleepStop(0x00, (byte)((tiltAngle < 0) ? 0x10 : 0x08),
                                     0x00, TiltSpeedByte,
                                     GetTiltTime(tiltAngle));

                //HACK: see above
                _client.CurrentTiltAngle = tiltAngle;
                _client.CurrentTiltAngle = 0;
            }
        }

        /// <summary>
        /// Executes the zoom movement as requested
        /// </summary>
        /// <param name="zoomPosition">the increase or decrease in zoom position.</param>
        public override void ZoomAbsolute(double zoomPosition)
        {
            if (zoomPosition != 0)
            {
                SendCommandSleepStop(0x00, (byte)((zoomPosition < 0) ? 0x40 : 0x20),
                                     0x00, 0x00,
                                     GetZoomTime(zoomPosition));

                //HACK: see above
                _client.CurrentZoomPosition = zoomPosition;
                _client.CurrentZoomPosition = 0;
            }
        }

        /// <summary>
        /// Determines the time neccesary to achieve a given change in pan
        /// </summary>
        /// <param name="panAngle">change in pan in degrees, -/+</param>
        /// <returns></returns>
        protected virtual int GetPanTime(double panAngle)
        {
            double absPanAngle = Math.Abs(panAngle);

            if (absPanAngle >= 40.5)
            {
                return (int)(9.85269 * absPanAngle + 186.5303);
            }
            else if (absPanAngle >= 2.9)
            {
                return (int)(9.581 * absPanAngle + 196.9693);
            }
            else
            {
                return (int)(65.8621 * absPanAngle);
            }
        }


        /// <summary>
        /// Determines the time neccesary to acheive the given change in tilt
        /// </summary>
        /// <param name="tiltAngle">change in tilt in degrees -/+</param>
        /// <returns>the time in milliseconds</returns>
        protected virtual int GetTiltTime(double tiltAngle)
        {
            double absTiltAngle = Math.Abs(tiltAngle);

            if (absTiltAngle >= 2.7)
            {
                return (int)(11.738095 * absTiltAngle + 183.3071435);
            }
            else if (absTiltAngle >= 1.5)
            {
                return (int)(30.8333 * absTiltAngle + 131.75);
            }
            else
            {
                return (int)(91.53846 * absTiltAngle + 40.692308);
            }
        }

        /// <summary>
        /// Determines the time neccesary to acheive the given change in zoom
        /// </summary>
        /// <param name="zoomPosition">change in zoom -/+</param>
        /// <returns>the time in milliseconds</returns>
        protected virtual int GetZoomTime(double zoomPosition)
        {
            return (int)(Math.Abs(zoomPosition) / 1.0 / ZoomSpeedFactor * 1000.0);
        }

        #endregion

        /// <summary>
        /// Issues a command, waits the runTime, then issues the Stop command
        /// </summary>
        /// <param name="cmnd1">command 1 byte</param>
        /// <param name="cmnd2">command 2 byte</param>
        /// <param name="data1">data 1 byte</param>
        /// <param name="data2">data 2 byte</param>
        /// <param name="runTime">time in milliseconds to allow the command to run before issueing the stop command</param>
        protected void SendCommandSleepStop(byte cmnd1, byte cmnd2, byte data1, byte data2, int runTime)
        {
            if (PresetUnlockNeeded)
            {
                Debug.WriteLine("Issuing \"Focus Far\" to unlock camera from preset...");
                SendCommand(0x00, 0x80, 0x00, 0x00);
                Thread.Sleep(100);
                SendStopCommand(null);
                PresetUnlockNeeded = false;
            }

            Debug.WriteLine("Running Command for " + runTime.ToString() + " ms.");
            SendCommand(cmnd1, cmnd2, data1, data2);
            _stopTimer.Change(runTime, Timeout.Infinite);
        }

        private void CancelPendingStop()
        {
            _stopTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void SendStopCommand(object state)
        {
            try
            {
                SendCommand(0x00, 0x00, 0x00, 0x00);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #region Camera Hardware Presets

        /// <summary>
        /// the highest usable preset for WCC/A models
        /// </summary>
        protected override int HighestPresetIndex
        {
            get
            {
                return 63;
            }
        }

        /// <summary>
        /// Informs the camera to memorize its current position and save it to the specified ID.
        /// </summary>
        /// <param name="id">ID number for preset</param>
        protected override void SetPreset(byte id)
        {
            CancelPendingStop();
            base.SetPreset(id);
            Thread.Sleep(1000); 
            SendCommandSleepStop(0x00, 0x80, 0x00, 0x00, 100);  //issuing "Focus Far" clears the obnoxious set menu
        }

        /// <summary>
        /// When this property is set to true, then a "Focus Far" command is issued before the next command.
        /// </summary>
        protected bool PresetUnlockNeeded { get; set; }

        /// <summary>
        /// Restores the indicated preset.
        /// </summary>
        /// <param name="id">preset identifier on camera</param>
        protected override void CallPreset(byte id)
        {
            CancelPendingStop();
            base.CallPreset(id);
            PresetUnlockNeeded = true;
        }

        #endregion

        /// <summary>
        /// Reports 0 for all fields. WCC261 cannot be queried.
        /// </summary>
        public override void PanTiltZoomPositionInquire()
        {
            // no can do
        }

        #region IPresetProviderItems Members

        private CameraIndexedPreset[] presetIndexing;

        private UserPresetStore _presetItems = new UserPresetStore();
        /// <summary>
        /// The current collection of presets associated with this camera
        /// </summary>
        public UserPresetStore PresetItems
        {
            get
            {
                return _presetItems;
            }
            set
            {
                _presetItems = value;
                BuildIndexingArray();
            }
        }

        private void BuildIndexingArray()
        {
            presetIndexing = new CameraIndexedPreset[HighestPresetIndex - LowestPresetIndex + 1];
            foreach (UserPresetItem i in PresetItems)
            {
                CameraIndexedPreset cip = i as CameraIndexedPreset;
                if (cip != null)
                {
                    presetIndexing[cip.Index - LowestPresetIndex] = cip;
                }
            }
        }

        #endregion

        #region IPresetProvider Members

        /// <summary>
        /// Saves a new preset at the given position
        /// </summary>
        /// <returns>the preset item for this position</returns>
        public UserPresetItem SavePreset()
        {
            int i = FindAvailableIndex();
            SetPreset((byte)i);
            presetIndexing[i - LowestPresetIndex] = new CameraIndexedPreset(i);
            PresetItems.Add(presetIndexing[i - LowestPresetIndex]);
            return presetIndexing[i - LowestPresetIndex];
        }

        /// <summary>
        /// Finds the lowest available camera index
        /// </summary>
        /// <returns></returns>
        private int FindAvailableIndex()
        {
            for (int i = 0; i < presetIndexing.Length; i++)
            {
                if (presetIndexing[i] == null)
                {
                    return i + LowestPresetIndex;
                }
            }
            throw new IndexOutOfRangeException("There are too many presets! Please delete one before adding more.");
        }

        /// <summary>
        /// Restores the camera to the given preset id
        /// </summary>
        /// <param name="id">id of the preset to restore</param>
        public void RestorePreset(Guid id)
        {
            CameraIndexedPreset i = PresetItems[id] as CameraIndexedPreset;
            if (i != null)
            {
                CallPreset((byte)i.Index);
            }
        }

        /// <summary>
        /// Updates a pre-existing preset with new metadata. If the item does not pre-exist, a new on is created.
        /// </summary>
        /// <param name="newItem">updated item</param>
        /// <returns></returns>
        public bool UpdatePreset(UserPresetItem newItem)
        {
            UserPresetItem oldItem = PresetItems[newItem.ID];
            if (oldItem != null)
            {
                PresetItems.Remove(oldItem);
                if (oldItem is CameraIndexedPreset)
                {
                    presetIndexing[((CameraIndexedPreset)oldItem).Index - LowestPresetIndex] = null;
                }
            }
            else
            {
                Debug.WriteLine("   Warning! No item with ID " + newItem.ID + " pre-existed!");
            }

            PresetItems.Add(newItem);
            if (newItem is CameraIndexedPreset)
            {
                presetIndexing[((CameraIndexedPreset)newItem).Index - LowestPresetIndex] = (CameraIndexedPreset)newItem;
            }
            return true;
        }

        /// <summary>
        /// Deletes the preset with the given identifier
        /// </summary>
        /// <param name="id">preset identifier to delete</param>
        /// <returns>true if the preset was deleted, false if it did not exist</returns>
        public bool DeletePreset(Guid id)
        {
            UserPresetItem item = PresetItems[id];
            CameraIndexedPreset cip = item as CameraIndexedPreset;
            if (cip != null)
            {
                ClearPreset((byte)cip.Index);
                presetIndexing[cip.Index - LowestPresetIndex] = null;
            }
            return PresetItems.Remove(item);
        }

        /// <summary>
        /// Deletes all presets
        /// </summary>
        public void DeleteAllPresets()
        {
            //clear the settings on the hardware
            for (int i = 0; i < presetIndexing.Length; i++)
            {
                if (presetIndexing[i] != null)
                {
                    ClearPreset((byte)(i + LowestPresetIndex));
                }
            }

            //clear the junk
            PresetItems.Clear();
            //rebuild the indexing array 
            BuildIndexingArray();
        }

        #endregion
    }
}
