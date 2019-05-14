using System;
//using System.Configuration;
using System.Diagnostics;
//using System.Collections.Generic;
using System.IO;
using System.IO.Ports;

using FutureConcepts.Media;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.CameraControls
{
    /// <summary>
    /// A class for mirroring the behavior of the <see cref="T:Visca"/> plugin, but in a more testable way
    /// </summary>
    public class ViscaTest1: ICameraControlPlugin
    {
        private ICameraControl _cameraControl;
        /// <summary>
        /// owning camera control
        /// </summary>
        public ICameraControl CameraControl
        {
            get
            {
                return _cameraControl;
            }
            set
            {
                _cameraControl = value;
            }
        }

        private int _currentSpeed = 40;
        /// <summary>
        /// Speed at which to move the camera
        /// </summary>
        public int CurrentSpeed
        {
            get
            {
                return _currentSpeed;
            }
            set
            {
                _currentSpeed = value;
            }
        }

        #region Wiper Timer Stuff

        private System.Timers.Timer _wiperTimer = null;

        private void WiperTimer_Elapsed(object sender, EventArgs e)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x22, 0xff }); // Cycle Wiper
        }
        /// <summary>
        /// Interval between lens wipes, in milliseconds
        /// </summary>
        public double WiperInterval
        {
            set
            {
                if (value != _wiperTimer.Interval)
                {
                    _wiperTimer.Interval = value;
                }
            }
            get
            {
                return _wiperTimer.Interval;
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose()
        {
            if (_wiperTimer != null)
            {
                _wiperTimer.Dispose();
                _wiperTimer = null;
            }
        }

        #endregion

        #region ICameraControlPlugin
        /// <summary>
        /// Instantiates a new instance of this plugin
        /// </summary>
        /// <param name="config">configuration information</param>
        /// <param name="cameraControl">owning camera control</param>
        public ViscaTest1(CameraControlInfo config, ICameraControl cameraControl)
        {
            CameraControl = cameraControl;
            Debug.WriteLine("CameraControls.ViscaTest1.Initialize");
        }

        #endregion

        #region ICameraControlCommon

        /// <summary>
        /// Initializes the camera
        /// </summary>
        public void Initialize()
        {
            AutoFocusOn();
        }

        /// <summary>
        /// Move the camera to the specified pan and tilt positions
        /// </summary>
        /// <param name="panAngleFloat">pan angle, [0,360)</param>
        /// <param name="tiltAngleFloat">tilt angle [-90,90]</param>
        public void PanTiltAbsolute(double panAngleFloat, double tiltAngleFloat)
        {
            byte[] s = { 0x01, 0x06, 0x02, 0x28, 0x28, 0x00, 0x08, 0x07, 0x08, 0x00, 0x08, 0x07, 0x08 };
            byte[] panAngleCoded = ConvertAngleToVisca(panAngleFloat);
            byte[] tiltAngleCoded = ConvertAngleToVisca(tiltAngleFloat + 37.0);
            s[3] = (byte)CurrentSpeed;
            s[4] = (byte)CurrentSpeed;
            s[5] = panAngleCoded[0];
            s[6] = panAngleCoded[1];
            s[7] = panAngleCoded[2];
            s[8] = panAngleCoded[3];
            s[9] = tiltAngleCoded[0];
            s[10] = tiltAngleCoded[1];
            s[11] = tiltAngleCoded[2];
            s[12] = tiltAngleCoded[3];
            SendAndReceive(s);

            _cameraControl.CurrentPanAngle = panAngleFloat;
            _cameraControl.CurrentTiltAngle = tiltAngleFloat;
        }

        /// <summary>
        /// Adjust zoom to specified magnification level
        /// </summary>
        /// <param name="zoomPositionFloat">magnification level</param>
        public void ZoomAbsolute(double zoomPositionFloat)
        {
            int zoomPosition = Convert.ToInt32((zoomPositionFloat - 1) * 189);
            byte[] s = { 0x01, 0x04, 0x47, 0, 0, 0, 0 };
            s[3] = (byte)((zoomPosition & 0xf000) >> 12);
            s[4] = (byte)((zoomPosition & 0xf00) >> 8);
            s[5] = (byte)((zoomPosition & 0xf0) >> 4);
            s[6] = (byte)(zoomPosition & 0xf);
            SendAndReceive(s);

            _cameraControl.CurrentZoomPosition = zoomPositionFloat;
        }

        /// <summary>
        /// Enable or disable the infrared capture filter
        /// </summary>
        /// <param name="enabled">true to enable, false to disable</param>
        public void SetInfrared(bool enabled)
        {
            if (enabled)
            {
                SendAndReceive(new byte[] { 0x01, 0x04, 0x01, 0x02 });
            }
            else
            {
                SendAndReceive(new byte[] { 0x01, 0x04, 0x01, 0x03 });
            }
        }

        /// <summary>
        /// Enable or disable the hardware image stabilizer
        /// </summary>
        /// <param name="enabled">true to enable, false to disable</param>
        public void SetStabilizer(bool enabled)
        {
            if (enabled)
            {
                SendAndReceive(new byte[] { 0x01, 0x04, 0x34, 0x02 });
            }
            else
            {
                SendAndReceive(new byte[] { 0x01, 0x04, 0x34, 0x03 });
            }
        }

        /// <summary>
        /// Enable or disable the lens wiper
        /// </summary>
        /// <param name="enabled">true to enable, false to disable</param>
        public void SetWiper(bool enabled)
        {
            if (enabled)
            {
                if (_wiperTimer == null)
                {
                    _wiperTimer = new System.Timers.Timer();
                    _wiperTimer.Interval = 5000;
                    _wiperTimer.Elapsed += new System.Timers.ElapsedEventHandler(WiperTimer_Elapsed);
                }
                _wiperTimer.Enabled = true;
            }
            else
            {
                if (_wiperTimer != null)
                {
                    _wiperTimer.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Enable or disable the IR emitter
        /// </summary>
        /// <param name="enabled">true to enable, false to disable</param>
        public void SetEmitter(bool enabled)
        {
            if (enabled)
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x14, 0x02 });
            }
            else
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x14, 0x03 });
            }
        }

        /// <summary>
        /// Veritcally Invert the image
        /// </summary>
        /// <param name="isInverted">true to invert, false for normal</param>
        public void SetOrientation(bool isInverted)
        {
            if (isInverted)
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x17, 0x03 });
            }
            else
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x17, 0x02 });
            }
        }

        /// <summary>
        /// Query the camera for the current pan, tilt, and zoom values
        /// </summary>
        public void PanTiltZoomPositionInquire()
        {
            byte[] responseBuffer = {0x00, 0x00, 0x00, 0x08, 0x07, 0x08, 0x00, 0x08, 0x07, 0x08};
            if (responseBuffer.Length > 9)
            {
                double panAngleFloat = Convert.ToDouble((responseBuffer[2] << 12) | (responseBuffer[3] << 8) | (responseBuffer[4] << 4) | responseBuffer[5]);
                panAngleFloat = panAngleFloat / 16;
                CameraControl.CurrentPanAngle = panAngleFloat;
                double tiltAngleFloat = Convert.ToDouble( (responseBuffer[6] << 12) | (responseBuffer[7] << 8) | (responseBuffer[8] << 4) | responseBuffer[9]);
                tiltAngleFloat = (tiltAngleFloat / 16) - 37;
                CameraControl.CurrentTiltAngle = tiltAngleFloat;
            }
            ZoomPositionInq();
        }

        #endregion

        #region Support Methods

        private void ZoomPositionInq()
        {
            byte[] responseBuffer = {0x00, 0x00, 0x00, 0x08, 0x07, 0x08 };
            if (responseBuffer.Length > 5)
            {
                int zoomPosition = (responseBuffer[2] << 12) | (responseBuffer[3] << 8) | (responseBuffer[4] << 4) | responseBuffer[4];
                CameraControl.CurrentZoomPosition = (zoomPosition / 189) + 1;
            }
            else
            {
                CameraControl.CurrentZoomPosition = 1;
            }
        }

        private void SendAndReceive(byte[] s)
        {
            Debug.WriteLine("SendAndReceive:");
            string str = "";
            for (int i = 0; i < s.Length; i++)
            {
                str += s[i].ToString("X");
                str += " ";
            }
            Debug.WriteLine(str);
        }

        static private byte[] ConvertAngleToVisca(double floatAngle)
        {
            byte[] s = new byte[4];
            int angle = Convert.ToInt32(floatAngle * 16);
            s[0] = (byte)((angle & 0xf000) >> 12);
            s[1] = (byte)((angle & 0xf00) >> 8);
            s[2] = (byte)((angle & 0xf0) >> 4);
            s[3] = (byte)(angle & 0xf);
            return s;
        }

        private void AutoFocusOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x38, 0x02 });
        }

        #endregion
    }
}
