using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Timers;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client.CameraControlClients
{
    /// <summary>
    /// Legacy class for PTZ control when using MediaServer camera control services
    /// Provides an interface to use the CameraControlService. See <see cref="T:ICameraControl"/>
    /// </summary>
    public class StackedCameraControlClient : ICameraControlClient
    {
        #region Local Definitions

        /// <summary>
        /// Raised when the client control is fully opened
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Raised when the client control has been closed or faulted
        /// </summary>
        public event EventHandler Closed;

        private Protocol.IProtocol _protocol;

        public Protocol.IProtocol Protocol
        {
            set
            {
                _protocol = value;
            }
            get
            {
                if (_relinquishTimer != null)
                {
                    _relinquishTimer.Stop();
                    _relinquishTimer.Start();
                }
                return _protocol;
            }
        }

        /// <summary>
        /// Removes all the event listeners. Calls close afterwards
        /// </summary>
        public void Dispose()
        {
            PropertyChanged = null;
        }

        private void Log(String msg)
        {
            Debug.WriteLine(String.Format("CameraControlClient: {0}", msg));
        }

        #endregion

        #region ICameraControlProperties

        private System.Timers.Timer _relinquishTimer;
        private int _relinquishTimeout;

        public int RelinquishTimeout
        {
            get
            {
                return _relinquishTimeout;
            }
            set
            {
                if (_relinquishTimer != null)
                {
                    _relinquishTimer.Stop();
                }
                _relinquishTimeout = value;
                if (value > 0)
                {
                    if (_relinquishTimer == null)
                    {
                        _relinquishTimer = new System.Timers.Timer();
                        _relinquishTimer.Elapsed += new ElapsedEventHandler(relinquishTimer_Elapsed);
                    }
                    _relinquishTimer.Interval = _relinquishTimeout;
                    _relinquishTimer.Start();
                }
            }
        }

        private void relinquishTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Debug.WriteLine("StackedCameraControlClient.reqlinquishTimer_Elapsed");
                _relinquishTimer.Stop();
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in BasePeripheralControl.reqlinquishTimer_Elapsed.");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private bool _infraredEnabled = false;
        /// <summary>
        /// True if the infrared-capture mode of the camera is enabled.  Not supported by all cameras.
        /// </summary>
        public bool InfraredEnabled
        {
            set
            {
                _infraredEnabled = value;
                NotifyPropertyChanged("InfraredEnabled");
            }
            get
            {
                return _infraredEnabled;
            }
        }

        private bool _stabilizerEnabled = false;
        /// <summary>
        /// True if auto-image-stabilization is enabled.  Not supported by all cameras.
        /// </summary>
        public bool StabilizerEnabled
        {
            set
            {
                _stabilizerEnabled = value;
                NotifyPropertyChanged("StabilizerEnabled");
            }
            get
            {
                return _stabilizerEnabled;
            }
        }

        private bool _wiperEnabled = false;
        /// <summary>
        /// True if the lens wiper is active.  Not supported by all cameras.
        /// </summary>
        public bool WiperEnabled
        {
            set
            {
                _wiperEnabled = value;
                NotifyPropertyChanged("WiperEnabled");
            }
            get
            {
                return _wiperEnabled;
            }
        }

        private bool _emitterEnabled = false;
        /// <summary>
        /// True if the infrared emitter is active.  Not supported by all cameras.
        /// </summary>
        public bool EmitterEnabled
        {
            set
            {
                _emitterEnabled = value;
                NotifyPropertyChanged("EmitterEnabled");
            }
            get
            {
                return _emitterEnabled;
            }
        }

        private bool _invertedEnabled = false;
        /// <summary>
        /// True if the camera is vertically inverting the image.  Not supported by all cameras.
        /// </summary>
        public bool InvertedEnabled
        {
            set
            {
                _invertedEnabled = value;
                NotifyPropertyChanged("InvertedEnabled");
            }
            get
            {
                return _invertedEnabled;
            }
        }

        private double _currentPanAngle = 0;
        /// <summary>
        /// Current Pan Angle of the camera, degrees, in the range [0,360)
        /// </summary>
        public double CurrentPanAngle
        {
            set
            {
                _currentPanAngle = value;
                NotifyPropertyChanged("CurrentPanAngle");
            }
            get
            {
                return _currentPanAngle;
            }
        }

        private double _currentTiltAngle = 0;
        /// <summary>
        /// Current Tilt Angle of the camera, degrees, in the range [-90,90]
        /// </summary>
        public double CurrentTiltAngle
        {
            set
            {
                _currentTiltAngle = value;
                NotifyPropertyChanged("CurrentTiltAngle");
            }
            get
            {
                return _currentTiltAngle;
            }
        }

        private double _currentZoomPosition = 0;
        /// <summary>
        /// Current Zoom position of the camera, magnification, in the range [1,Infinity)
        /// </summary>
        public double CurrentZoomPosition
        {
            set
            {
                _currentZoomPosition = value;
                NotifyPropertyChanged("CurrentZoomPosition");
            }
            get
            {
                return _currentZoomPosition;
            }
        }

        private string _message = "";
        /// <summary>
        /// A status or error message from the server.
        /// </summary>
        public string StatusMessage
        {
            get
            {
                return _message;
            }
            set
            {
                //set and notify regardless, in case the server says the same thing twice
                _message = value;
                NotifyPropertyChanged("StatusMessage");
            }
        }

        #endregion

        #region Presets

        private UserPresetStore _presetItems;
        /// <summary>
        /// Gets the saved camera positions listed on the server
        /// </summary>
        public UserPresetStore PresetItems
        {
            set
            {
                _presetItems = value;
                NotifyPropertyChanged("PresetItems");
            }
            get
            {
                return _presetItems;
            }
        }

        /// <summary>
        /// Indicates to the server that the current camera position should be saved as a preset.
        /// </summary>
        /// <returns>The server generates an object to refer to the preset and returns it. Null if an error occurred</returns>
        public UserPresetItem SavePreset()
        {
            Log("SavePreset");
            UserPresetItem result = null;
            IPresetProvider presetProvider = Protocol as IPresetProvider;
            if (presetProvider != null)
            {
                result = presetProvider.SavePreset();
            }
            return result;
        }

        /// <summary>
        /// Tell the server to restore a specific preset
        /// </summary>
        /// <param name="id">preset identifier</param>
        public void RestorePreset(Guid id)
        {
            Log(String.Format("RestorePreset {0}", id.ToString()));
            IPresetProvider presetProvider = Protocol as IPresetProvider;
            if (presetProvider != null)
            {
                presetProvider.RestorePreset(id);
            }
        }

        /// <summary>
        /// Updates a preset on the server. The <see cref="P:UserPresetItem.ID"/> must be the same as an existing preset for an update to occur.
        /// </summary>
        /// <param name="item">updated preset</param>
        /// <returns>true on success, false if either no update was made, or an error occurred</returns>
        public bool UpdatePreset(UserPresetItem item)
        {
            Log(String.Format("UpdatePreset {0} {1}", item.ID, item.Name));
            bool result = false;
            IPresetProvider presetProvider = Protocol as IPresetProvider;
            if (presetProvider != null)
            {
                result = presetProvider.UpdatePreset(item);
            }
            return result;
        }

        /// <summary>
        /// Deletes a preset on the server
        /// </summary>
        /// <param name="id">identifier of the preset to delete</param>
        /// <returns>true on success, false otherwise</returns>
        public bool DeletePreset(Guid id)
        {
            Log(String.Format("DeletePreset {0}", id.ToString()));
            bool result = false;
            IPresetProvider presetProvider = Protocol as IPresetProvider;
            if (presetProvider != null)
            {
                result = presetProvider.DeletePreset(id);
            }
            return result;
        }

        /// <summary>
        /// Deletes all presets on the server
        /// </summary>
        public void DeleteAllPresets()
        {
            Log("DeleteAllPresets");
            IPresetProvider presetProvider = Protocol as IPresetProvider;
            if (presetProvider != null)
            {
                presetProvider.DeleteAllPresets();
            }
        }

        #endregion

        #region ICameraControl

        /// <summary>
        /// Call this method to start a session.
        /// </summary>
        /// <param name="request">source name to control</param>
        public void Open(ClientConnectRequest request)
        {
            Log("Open");
            _protocol.Initialize();
            if (Opened != null)
            {
                Opened(this, new EventArgs());
            }
        }

        public void Initialize()
        {
        }

        /// <summary>
        /// Ensures that the Closed event is raised.
        /// </summary>
        public void Close()
        {
            Log("Close");
            _protocol.Close();
            if (_relinquishTimer != null)
            {
                _relinquishTimer.Enabled = false;
                _relinquishTimer.Dispose();
                _relinquishTimer = null;
            }
            if (Closed != null)
            {
                Closed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Change the Pan and Tilt values of the camera
        /// </summary>
        /// <remarks>
        /// Some camera plugins may use these parameters in a relative sense.
        /// </remarks>
        /// <param name="panAngle">
        /// New pan angle. [0,360).
        /// For some camera plugins, this may be a relative change, with negative values meaning
        /// degrees counter-clockwise and positive values meaning degrees clockwise.
        /// </param>
        /// <param name="tiltAngle">
        /// New tilt angle. [-90,90].
        /// For some camera plugins, this may be a relative change, with negative values meaning
        /// degrees down and positive values meaning degrees up.
        /// </param>
        /// <seealso cref="T:ICameraControlCommon"/>
        public void PanTiltAbsolute(double panAngle, double tiltAngle)
        {
            Log(String.Format("PanTiltAbsolute panAngle={0} tiltAngle={1}", panAngle, tiltAngle));
            Protocol.PanTiltAbsolute(panAngle, tiltAngle);
        }

        /// <summary>
        /// Change the Zoom (magnification) value of the camera.
        /// </summary>
        /// <remarks>
        /// Some camera plugins may use the parameter in a relative sense.
        /// </remarks>
        /// <param name="zoomPosition">
        /// The new absolute zoom value, defined as magnification. [1,Infinity).
        /// For some camera plugins, this may be a relative change, with negative values meaning
        /// a decrease in magnification, and positive values meaning an increase in magnification.
        /// </param>
        /// <seealso cref="T:ICameraControlCommon"/>
        public void ZoomAbsolute(double zoomPosition)
        {
            Log(String.Format("ZoomAbsolute {0}", zoomPosition));
            Protocol.ZoomAbsolute(zoomPosition);
        }

        /// <summary>
        /// Enable or disable Infrared capture mode. Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if in infrared capture mode</param>
        public void SetInfrared(bool enabled)
        {
            Log(String.Format("SetInfrared {0}", enabled.ToString()));
            Protocol.SetInfrared(enabled);
        }

        /// <summary>
        /// Enable or disable the auto-image-stabilization.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if stabilization is enabled</param>
        public void SetStabilizer(bool enabled)
        {
            Log(String.Format("SetStabilizer {0}", enabled.ToString()));
            Protocol.SetStabilizer(enabled);
        }

        /// <summary>
        /// Enable or disable the lens wiper.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if the wiper is activated</param>
        public void SetWiper(bool enabled)
        {
            Log(String.Format("SetWiper {0}", enabled.ToString()));
            Protocol.SetWiper(enabled);
        }

        /// <summary>
        /// Enable or disable the Infrared Emitter.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if the infrared emitter is active</param>
        public void SetEmitter(bool enabled)
        {
            Log(String.Format("SetEmitter {0}", enabled.ToString()));
            Protocol.SetEmitter(enabled);
        }

        /// <summary>
        /// Enable or disable the vertical inversion of the image.  Not supported by all cameras.
        /// </summary>
        /// <param name="inverted">true if the camera is inverting the image</param>
        public void SetOrientation(bool inverted)
        {
            Log(String.Format("SetOrientation inverted={0}", inverted.ToString()));
            Protocol.SetOrientation(inverted);
        }

        /// <summary>
        /// Inquire from the camera it's current pan, tilt, zoom information.  Not fully supported by all cameras.
        /// </summary>
        public void PanTiltZoomPositionInquire()
        {
            Log("PanTiltZoomPositionInquire");
            Protocol.PanTiltZoomPositionInquire();
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Raised when a property's value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string str)
        {
            Log(String.Format("NotifyPropertyChanged {0}", str));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }
        #endregion
    }
}
