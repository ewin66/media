using System;
using System.ComponentModel;
using System.ServiceModel;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Legacy class for PTZ control when using MediaServer camera control services
    /// Provides an interface to use the CameraControlService. See <see cref="T:ICameraControl"/>
    /// </summary>
    public class LegacyCameraControlClient : BasePeripheralControl<ICameraControl>, ICameraControlClient
    {
        #region Local Definitions

        private ClientConnectRequest _clientRequest;

        /// <summary>
        /// Creates an instance pointing to a specific server
        /// </summary>
        /// <param name="serverHostOrIP">hostname or IP address of the server</param>
        public LegacyCameraControlClient(string serverHostOrIP) : base(serverHostOrIP) { }

        /// <summary>
        /// Removes all the event listeners. Calls close afterwards
        /// </summary>
        public override void Dispose()
        {
            PropertyChanged = null;
            base.Dispose();
        }

        #endregion

        #region ICameraControlProperties

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

        #region SavedCameraPositions

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

        #region Saved Positions

        /// <summary>
        /// Indicates to the server that the current camera position should be saved as a preset.
        /// </summary>
        /// <returns>The server generates an object to refer to the preset and returns it. Null if an error occurred</returns>
        public UserPresetItem SavePreset()
        {
            if (ProxyReady)
            {
                return Proxy.SavePreset();
            }
            return null;
        }

        /// <summary>
        /// Tell the server to restore a specific preset
        /// </summary>
        /// <param name="id">preset identifier</param>
        public void RestorePreset(Guid id)
        {
            if (ProxyReady)
            {
                Proxy.RestorePreset(id);
            }
        }

        /// <summary>
        /// Updates a preset on the server. The <see cref="P:UserPresetItem.ID"/> must be the same as an existing preset for an update to occur.
        /// </summary>
        /// <param name="item">updated preset</param>
        /// <returns>true on success, false if either no update was made, or an error occurred</returns>
        public bool UpdatePreset(UserPresetItem item)
        {
            if (ProxyReady)
            {
                return Proxy.UpdatePreset(item);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a preset on the server
        /// </summary>
        /// <param name="id">identifier of the preset to delete</param>
        /// <returns>true on success, false otherwise</returns>
        public bool DeletePreset(Guid id)
        {
            if (ProxyReady)
            {
                return Proxy.DeletePreset(id);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes all presets on the server
        /// </summary>
        public void DeleteAllPresets()
        {
            if (ProxyReady)
            {
                Proxy.DeleteAllPresets();
            }
        }

        #endregion

        #endregion

        #region BasePeripheralControl

        /// <summary>
        /// Instantiates the CameraControlCallback
        /// </summary>
        /// <returns>a new instance of <see cref="T:CameraControlCallback"/></returns>
        protected override object CreateCallback()
        {
            return new CameraControlCallback(this);
        }

        /// <summary>
        /// Returns the string representation of the port number on the server to connect to
        /// </summary>
        protected override string Port
        {
            get { return "8081"; }
        }

        /// <summary>
        /// Returns the end point of the Camera Control Service
        /// </summary>
        protected override string Endpoint
        {
            get { return "CameraControlService"; }
        }

        /// <summary>
        /// Returns the desired keep-alive interval
        /// </summary>
        protected override int KeepAliveInterval
        {
            get { return 15000; }
        }

        #endregion

        #region ICameraControl

        /// <summary>
        /// Call this method to start a session.
        /// </summary>
        /// <param name="request">source name to control</param>
        public void Open(ClientConnectRequest request)
        {
            _clientRequest = request;
            try
            {
                Connect();
                Proxy.Open(request);
                Initialize();
            }
            catch (Exception exc)
            {
                if (Factory != null)
                {
                    if (Factory.State == CommunicationState.Opened)
                    {
                        this.Abort();
                    }
                }
                throw new Exception("The camera control is currently unavailable.", exc);
            }
        }

        /// <summary>
        /// Initializes the camera control plug in on the server
        /// </summary>
        public void Initialize()
        {
            if (ProxyReady)
            {
                Proxy.Initialize();
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
            if (ProxyReady)
            {
                Proxy.PanTiltAbsolute(panAngle, tiltAngle);
            }
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
            if (ProxyReady)
            {
                Proxy.ZoomAbsolute(zoomPosition);
            }
        }

        /// <summary>
        /// Enable or disable Infrared capture mode. Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if in infrared capture mode</param>
        public void SetInfrared(bool enabled)
        {
            if (ProxyReady)
            {
                Proxy.SetInfrared(enabled);
            }
        }

        /// <summary>
        /// Enable or disable the auto-image-stabilization.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if stabilization is enabled</param>
        public void SetStabilizer(bool enabled)
        {
            if (ProxyReady)
            {
                Proxy.SetStabilizer(enabled);
            }
        }

        /// <summary>
        /// Enable or disable the lens wiper.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if the wiper is activated</param>
        public void SetWiper(bool enabled)
        {
            if (ProxyReady)
            {
                Proxy.SetWiper(enabled);
            }
        }

        /// <summary>
        /// Enable or disable the Infrared Emitter.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if the infrared emitter is active</param>
        public void SetEmitter(bool enabled)
        {
            if (ProxyReady)
            {
                Proxy.SetEmitter(enabled);
            }
        }

        /// <summary>
        /// Enable or disable the vertical inversion of the image.  Not supported by all cameras.
        /// </summary>
        /// <param name="inverted">true if the camera is inverting the image</param>
        public void SetOrientation(bool inverted)
        {
            if (ProxyReady)
            {
                Proxy.SetOrientation(inverted);
            }
        }

        /// <summary>
        /// Inquire from the camera it's current pan, tilt, zoom information.  Not fully supported by all cameras.
        /// </summary>
        public void PanTiltZoomPositionInquire()
        {
            if (ProxyReady)
            {
                Proxy.PanTiltZoomPositionInquire();
            }
        }

        /// <summary>
        /// do not call
        /// </summary>
        public void KeepAlive()
        {
            throw new NotSupportedException("Do not call KeepAlive directly. BasePeripheralControl handles this.");
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Raised when a property's value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }
        #endregion
    }
}
