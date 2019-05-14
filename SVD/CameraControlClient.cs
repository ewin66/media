using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Text;

namespace Antares.Video
{
    class CameraControlClient : Component, INotifyPropertyChanged
    {
        public event EventHandler Opened;
        public event EventHandler Closed;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_factory != null)
                {
                    if (_factory.State == CommunicationState.Opened)
                    {
                        _factory.Close();
                        _factory = null;
                    }
                    _proxy = null;
                }
                if (_keepAliveTimer != null)
                {
                    _keepAliveTimer.Dispose();
                    _keepAliveTimer = null;
                }
            }
        }


        private System.Windows.Forms.Timer _keepAliveTimer = null;

        private string _sourceName;

        public string SourceName
        {
            set
            {
                try
                {
                    _sourceName = value;
                    CameraControl.Open(_sourceName);
                    _keepAliveTimer = new System.Windows.Forms.Timer();
                    _keepAliveTimer.Interval = 5000;
                    _keepAliveTimer.Tick += new EventHandler(KeepAliveTimer_Tick);
                    _keepAliveTimer.Enabled = true;
                    PanTiltPosInq();
                    ZoomPositionInq();
                }
                catch (Exception exc)
                {
                    if (_factory.State == CommunicationState.Opened)
                    {
                        _factory.Abort();
                    }
                    throw new Exception("The camera control is currently unavailable.", exc);
                }
                NotifyPropertyChanged("SourceName");
            }
            get
            {
                return _sourceName;
            }
        }

        private string _serverAddress = null;

        public string ServerAddress
        {
            get
            {
                return _serverAddress;
            }
            set
            {
                _serverAddress = value;
                NotifyPropertyChanged("ServerAddress");
            }
        }

        private ChannelFactory<ICameraControl> _factory = null;
        private ICameraControl _proxy = null;

        public ICameraControl CameraControl
        {
            get
            {
                if (_proxy == null)
                {
                    NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None, true);
                    _factory = new ChannelFactory<ICameraControl>(netTcpBinding, @"net.tcp://" + _serverAddress + @":8081/RoscoService");
                    _factory.Opened += new EventHandler(CameraControlFactory_Opened);
                    _factory.Closed += new EventHandler(CameraControlFactory_Closed);
                    _factory.Faulted += new EventHandler(CameraControlFactory_Closed);
                    _factory.Open();
                    _proxy = _factory.CreateChannel();
                }
                return _proxy;
            }
            set
            {
                if (_proxy != null)
                {
                    _factory.Close();
                    _factory = null;
                    _proxy = null;
                }
            }
        }

        private bool _emitterEnabled = false;

        public bool EmitterEnabled
        {
            set
            {
                if (value != _emitterEnabled)
                {
                    if (_proxy != null)
                    {
                        if (value == true)
                        {
                            _proxy.EmitterOn();
                        }
                        else
                        {
                            _proxy.EmitterOff();
                        }
                    }
                    _emitterEnabled = value;
                    NotifyPropertyChanged("EmitterEnabled");
                }
            }
            get
            {
                return _emitterEnabled;
            }
        }

        private bool _stabilizerEnabled = false;

        public bool StabilizerEnabled
        {
            set
            {
                if (value != _stabilizerEnabled)
                {
                    if (_proxy != null)
                    {
                        if (value == true)
                        {
                            _proxy.StabilizerOn();
                        }
                        else
                        {
                            _proxy.StabilizerOff();
                        }
                    }
                    _stabilizerEnabled = value;
                    NotifyPropertyChanged("StabilizerEnabled");
                }
            }
            get
            {
                return _stabilizerEnabled;
            }
        }

        private bool _infraredEnabled = false;

        public bool InfraredEnabled
        {
            set
            {
                if (value != _stabilizerEnabled)
                {

                    if (_proxy != null)
                    {
                        if (value == true)
                        {
                            _proxy.NearInfraredModeOn();
                        }
                        else
                        {
                            _proxy.NearInfraredModeOff();
                        }
                    }
                    _infraredEnabled = value;
                    NotifyPropertyChanged("InfraredEnabled");
                }
            }
            get
            {
                return _infraredEnabled;
            }
        }

        private bool _invertedEnabled = false;

        public bool InvertedEnabled
        {
            set
            {
                if (value != _invertedEnabled)
                {
                    if (_proxy != null)
                    {
                        if (value == true)
                        {
                            _proxy.OrientationInverted();
                        }
                        else
                        {
                            _proxy.OrientationNotInverted();
                        }
                    }
                    _invertedEnabled = value;
                    NotifyPropertyChanged("InvertedEnabled");
                }
            }
            get
            {
                return _invertedEnabled;
            }
        }

        private System.Timers.Timer _wiperTimer = null;

        public double WiperInterval
        {
            set
            {
                if (value != _wiperTimer.Interval)
                {
                    _wiperTimer.Interval = value;
                    NotifyPropertyChanged("WiperInterval");
                }
            }
            get
            {
                return _wiperTimer.Interval;
            }
        }

        public bool WiperEnabled
        {
            set
            {
                if (_wiperTimer == null)
                {
                    _wiperTimer = new System.Timers.Timer();
                    _wiperTimer.Elapsed += new System.Timers.ElapsedEventHandler(WiperTimer_Elapsed);
                }
                _wiperTimer.Enabled = value;
                NotifyPropertyChanged("WiperEnabled");
            }
            get
            {
                if (_wiperTimer != null)
                {
                    return _wiperTimer.Enabled;
                }
                else
                {
                    return false;
                }
            }
        }

        private void WiperTimer_Elapsed(object sender, EventArgs e)
        {
            if (_proxy != null)
            {
                _proxy.CycleWiper();
            }
        }

        private int _panTiltSpeed = 40;

        public int PanTiltSpeed
        {
            set
            {
                if (value != _panTiltSpeed)
                {
                    _panTiltSpeed = value;
                    NotifyPropertyChanged("PanTiltSpeed");
                }
            }
            get
            {
                return _panTiltSpeed;
            }
        }

        private int _zoomSpeed = 33;

        public int ZoomSpeed
        {
            set
            {
                if (value != _zoomSpeed)
                {
                    _zoomSpeed = value;
                    NotifyPropertyChanged("ZoomSpeed");
                }
            }
            get
            {
                return _zoomSpeed;
            }
        }

        public void ZoomStop()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomStop();
                ZoomPositionInq();
            }
        }

        public void ZoomTeleStandard()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomTeleStandard();
            }
        }

        public void ZoomWideStandard()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomWideStandard();
            }
        }

        public void ZoomTeleVariable()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomTeleVariable(_zoomSpeed);
            }
        }

        public void ZoomWideVariable()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomWideVariable(_zoomSpeed);
            }
        }

        public void ZoomDirect(int zoomPosition)
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomDirect(zoomPosition);
            }
        }

        public void ZoomDigitalZoomOn()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomDigitalZoomOn();
            }
        }

        public void ZoomDigitalZoomOff()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomDigitalZoomOff();
            }
        }

        public void ZoomDirectDigital(int zoomPosition)
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.ZoomDirectDigital(zoomPosition);
            }
        }

        public void FocusStop()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.FocusStop();
            }
        }

        public void FocusFarStandard()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.FocusFarStandard();
            }
        }

        public void FocusNearStandard()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.FocusNearStandard();
            }
        }

        public void FocusAutoFocusOn()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.FocusAutoFocusOn();
            }
        }

        public void FocusManualFocusOn()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.FocusManualFocusOn();
            }
        }

        public void FocusInfinity()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.FocusInfinity();
            }
        }

        public void BrightReset()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.BrightReset();
            }
        }

        public void BrightUp()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.BrightUp();
            }
        }

        public void BrightDown()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.BrightDown();
            }
        }

        public void HomeSet()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.HomeSet();
            }
        }

        public void HomeGoto()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.HomeGoto();
                PanTiltPosInq();
                ZoomPositionInq();
            }
        }

        public void Reset()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.Reset();
            }
        }

        public void PanTiltUp()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltUp(_panTiltSpeed);
            }
        }

        public void PanTiltDown()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltDown(_panTiltSpeed);
            }
        }

        public void PanTiltLeft()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltLeft(_panTiltSpeed);
            }
        }

        public void PanTiltRight()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltRight(_panTiltSpeed);
            }
        }

        public void PanTiltUpLeft()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltUpLeft(_panTiltSpeed);
            }
        }

        public void PanTiltUpRight()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltUpRight(_panTiltSpeed);
            }
        }

        public void PanTiltDownLeft()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltDownLeft(_panTiltSpeed);
            }
        }

        public void PanTiltDownRight()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltDownRight(_panTiltSpeed);
            }
        }

        public void PanTiltStop()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                int panAngle;
                int tiltAngle;
                _proxy.PanTiltStop(out panAngle, out tiltAngle);
                PanAngle = panAngle;
                TiltAngle = tiltAngle;
            }
        }

        public void PanTiltAbsolutePosition(int panAngle, int tiltAngle)
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltAbsolutePosition(panAngle, tiltAngle, _panTiltSpeed);
                PanAngle = panAngle;
                TiltAngle = tiltAngle;
            }
        }

        public void PanTiltRelativePosition(int panAngle, int tiltAngle)
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.PanTiltRelativePosition(panAngle, tiltAngle, _panTiltSpeed);
                PanTiltPosInq();
            }
        }

        private int _panAngle = 0;

        public int PanAngle
        {
            set
            {
                if (value != _panAngle)
                {
                    _panAngle = value;
                    NotifyPropertyChanged("PanAngle");
                }
            }
            get
            {
                return _panAngle;
            }
        }

        private int _tiltAngle = 0;

        public int TiltAngle
        {
            set
            {
                if (value != _tiltAngle)
                {
                    _tiltAngle = value;
                    NotifyPropertyChanged("TiltAngle");
                }
            }
            get
            {
                return _tiltAngle;
            }
        }

        public void PanTiltPosInq()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                int panAngle;
                int tiltAngle;
                _proxy.PanTiltPosInq(out panAngle, out tiltAngle);
                PanAngle = panAngle;
                TiltAngle = tiltAngle;
            }
        }

        private int _zoomPosition = 0;

        public int ZoomPosition
        {
            set
            {
                if (value != _zoomPosition)
                {
                    _zoomPosition = value;
                    NotifyPropertyChanged("ZoomPosition");
                }
            }
            get
            {
                return _zoomPosition;
            }
        }

        public void ZoomPositionInq()
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                ZoomPosition = _proxy.ZoomPositionInq();
            }
        }

        public void SaveCalibrationDataToFile(Dictionary<int, CameraPosition> dataFile)
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.SaveCalibrationDataToFile(dataFile);
            }
        }

        private void KeepAliveTimer_Tick(object sender, EventArgs e)
        {
            if (_factory != null && _factory.State == CommunicationState.Opened)
            {
                _proxy.KeepAlive();
            }
        }

        private void CameraControlFactory_Opened(object sender, EventArgs e)
        {
            if (Opened != null)
            {
                Opened(sender, e);
            }
        }

        private void CameraControlFactory_Closed(object sender, EventArgs e)
        {
            _proxy = null;
            _factory = null;
            if (_keepAliveTimer != null)
            {
                _keepAliveTimer.Enabled = false;
            }
            if (Closed != null)
            {
                Closed(sender, e);
            }
        }
    }
}
