using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FutureConcepts.Media.CameraControls;
using FutureConcepts.Tools;
using FutureConcepts.Media.Contract;
using System.Windows.Forms;

namespace FutureConcepts.Media.Tools.CameraControlTester
{
    class DirectCameraControl : ICameraControl, INotifyPropertyChanged
    {
        private ICameraControlPlugin _plugin = null;

        public ICameraControlPlugin Plugin
        {
            get
            {
                return _plugin;
            }
            set
            {
                _plugin = value;
            }
        }
        
        #region ICameraControl Members

        private bool _infrared;
        public bool InfraredEnabled
        {
            get
            {
                return _infrared;
            }
            set
            {
                _infrared = value;
                FirePropertyChanged("InfraredEnabled");
            }
        }

        private bool _stabilizer;
        public bool StabilizerEnabled
        {
            get
            {
                return _stabilizer;
            }
            set
            {
                _stabilizer = value;
                FirePropertyChanged("StabilizerEnabled");
            }
        }

        private bool _wiper;
        public bool WiperEnabled
        {
            get
            {
                return _wiper;
            }
            set
            {
                _wiper = value;
                FirePropertyChanged("WiperEnabled");
            }
        }

        private bool _emitter;
        public bool EmitterEnabled
        {
            get
            {
                return _emitter;
            }
            set
            {
                _emitter = value;
                FirePropertyChanged("EmitterEnabled");
            }
        }

        private bool _inverter;
        public bool InvertedEnabled
        {
            get
            {
                return _inverter;
            }
            set
            {
                _inverter = value;
                FirePropertyChanged("InverterEnabled");
            }
        }

        private double _pan;
        public double CurrentPanAngle
        {
            get
            {
                return _pan;
            }
            set
            {
                _pan = value;
                FirePropertyChanged("CurrentPanAngle");
            }
        }

        private double _tilt;
        public double CurrentTiltAngle
        {
            get
            {
                return _tilt;
            }
            set
            {
                _tilt = value;
                FirePropertyChanged("CurrentTiltAngle");
            }
        }

        private double _zoom;
        public double CurrentZoomPosition
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                FirePropertyChanged("CurrentZoomPosition");
            }
        }

        private string _message;

        public string StatusMessage
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                FirePropertyChanged("StatusMessage");
            }
        }

        /// <summary>
        /// Use Open(Config.CameraControl)
        /// </summary>
        public void Open(ClientConnectRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The last attached CameraConfig (via Open)
        /// </summary>
        public CameraControlInfo CameraConfig
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the plugin
        /// </summary>
        /// <param name="config">configuration to use</param>
        public void Open(CameraControlInfo config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            Plugin = PluginFactory.Create(config, this);
            CameraConfig = config;
        }

        public void Close()
        {
            if (Plugin != null)
            {
                Plugin.Dispose();
            }
        }

        public void SaveCurrentPosition(string name)
        {
            throw new NotImplementedException();
        }

        public void GotoSavedPosition(string name)
        {
            throw new NotImplementedException();
        }

        public bool RenameSavedPosition(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSavedPosition(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllSavedPositions()
        {
            throw new NotImplementedException();
        }

        public void KeepAlive()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICameraControlCommon Members

        public void Initialize()
        {
            Plugin.Initialize();
        }

        public void PanTiltAbsolute(double panAngle, double tiltAngle)
        {
            double oldPanValue = CurrentPanAngle;
            double oldTiltValue = CurrentPanAngle;
            try
            {
                if (((panAngle >= 0) && (panAngle < 360)) &&
                    ((tiltAngle >= CameraConfig.Capabilities.TiltMinAngle) &&
                     (tiltAngle <= CameraConfig.Capabilities.TiltMaxAngle)))
                {
                    Plugin.PanTiltAbsolute(panAngle,tiltAngle);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("panAngle or tiltAngle", "Expected range is [0,360], [" +
                                                          CameraConfig.Capabilities.TiltMinAngle.ToString() + "," +
                                                          CameraConfig.Capabilities.TiltMaxAngle.ToString() + "].");
                }
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
                CurrentPanAngle = oldPanValue;
                CurrentTiltAngle = oldTiltValue;
            }
        }

        public void ZoomAbsolute(double zoomPosition)
        {
            double oldValue = CurrentZoomPosition;
            try
            {
                if ((zoomPosition >= CameraConfig.Capabilities.ZoomMinLevel) && 
                    (zoomPosition <= CameraConfig.Capabilities.ZoomMaxLevel))
                {
                    Plugin.ZoomAbsolute(zoomPosition);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("zoomPosition", "Expected range is [" +
                                                          CameraConfig.Capabilities.ZoomMinLevel.ToString() + "," +
                                                          CameraConfig.Capabilities.ZoomMaxLevel.ToString() + "].");
                }
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
                CurrentZoomPosition = oldValue;
            }
        }

        public void SetInfrared(bool enabled)
        {
            Plugin.SetInfrared(enabled);
        }

        public void SetStabilizer(bool enabled)
        {
            Plugin.SetStabilizer(enabled);
        }

        public void SetWiper(bool enabled)
        {
            Plugin.SetWiper(enabled);
        }

        public void SetEmitter(bool enabled)
        {
            Plugin.SetEmitter(enabled);
        }

        public void SetOrientation(bool inverted)
        {
            Plugin.SetOrientation(inverted);
        }

        public void PanTiltZoomPositionInquire()
        {
            Plugin.PanTiltZoomPositionInquire();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region IPresetProvider Members

        public UserPresetItem SavePreset()
        {
            throw new NotImplementedException();
        }

        public void RestorePreset(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePreset(UserPresetItem updatedItem)
        {
            throw new NotImplementedException();
        }

        public bool DeletePreset(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllPresets()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
