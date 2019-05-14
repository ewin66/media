using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

using FutureConcepts.Media;
using FutureConcepts.Tools;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.CameraControls
{
    /// <summary>
    /// Implements Sony VISCA protocol, with some special adaptations for the RVision pSEE
    /// </summary>
    public class Visca: ICameraControlPlugin
    {
        private SerialPort _serialPort = null;

        private ICameraControl _cameraControl;
        /// <summary>
        /// reference to the parent ICameraControl
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

        private int _currentSpeed = 50;
        /// <summary>
        /// Default speed
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
        /// The interval in milliseconds between when the lens wiper is cycled
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
        /// Turns off the IR emitter, and closes the serial port
        /// </summary>
        public void Dispose()
        {
            if (_serialPort != null)
            {
                try
                {
                    SetEmitter(false);
                }
                catch (Exception exc)
                {
                    ErrorLogger.DumpToDebug(exc);
                    Debug.WriteLine("Is there a camera connected to " + _serialPort.PortName + "?");
                }
                Debug.WriteLine("Visca.Dispose Closing " + _serialPort.PortName);
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
            if (_wiperTimer != null)
            {
                _wiperTimer.Dispose();
                _wiperTimer = null;
            }
        }

        #endregion

        #region ICameraControlPlugin

        /// <summary>
        /// Instantiates a new instance of the plugin
        /// </summary>
        /// <param name="config">configuration information</param>
        /// <param name="cameraControl">owner camera control</param>
        public Visca(CameraControlInfo config, ICameraControl cameraControl)
        {
            CameraControl = cameraControl;
            Debug.WriteLine("CameraControls.Visca");
            _serialPort = new SerialPort(config.Address, 9600, Parity.None, 8, StopBits.One);
            _serialPort.Open();
            _serialPort.DiscardNull = false;
            _serialPort.DtrEnable = true;
            _serialPort.ReadTimeout = 10000;
            Debug.WriteLine("CameraControls.Visca.Open opened = " + _serialPort.PortName);
            try
            {
                DeviceTypeInquire();
                SetServo(false);
                SetBrake(true);
                SetSlowMode(false);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Is there a camera attached?");
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
                throw exc;
            }
        }

        #endregion

        #region ICameraControlCommon

        /// <summary>
        /// Initializes the camera -- turns on AutoFocus
        /// </summary>
        public void Initialize()
        {
            Debug.WriteLine("Visca.Initialize");
            AutoFocusOn();
        }

        /// <summary>
        /// Moves the camera to the specified pan and tilt angles
        /// </summary>
        /// <param name="panAngleFloat">pan angle, [0,360)</param>
        /// <param name="tiltAngleFloat">tilt angle [-90,90]</param>
        public void PanTiltAbsolute(double panAngleFloat, double tiltAngleFloat)
        {
            Debug.WriteLine(String.Format("Visca.PanTiltAbsolute({0},{1})", panAngleFloat, tiltAngleFloat));
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
        /// Adjusts the zoom to the specified magnification level
        /// </summary>
        /// <param name="zoomPosition">magnification level</param>
        public void ZoomAbsolute(double zoomPosition)
        {
            Debug.WriteLine(String.Format("Visca.ZoomAbsolute(ZoomPosition={0})", zoomPosition));
            int units = ConvertZoomToViscaUnits(zoomPosition);
            byte[] s = { 0x01, 0x04, 0x47, 0, 0, 0, 0 };
            s[3] = (byte)((units & 0xf000) >> 12);
            s[4] = (byte)((units & 0xf00) >> 8);
            s[5] = (byte)((units & 0xf0) >> 4);
            s[6] = (byte)(units & 0xf);
            SendAndReceive(s);

            _cameraControl.CurrentZoomPosition = zoomPosition;
        }

        /// <summary>
        /// Turns on or off the IR capture filter
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        public void SetInfrared(bool enabled)
        {
            Debug.WriteLine(String.Format("Visca.SetInfrared({0})", enabled));
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
        /// Turns on or off the hardware image stabilization
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        public void SetStabilizer(bool enabled)
        {
            Debug.WriteLine(String.Format("Visca.SetStabilizer({0})", enabled));
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
        /// Activates the lens wiper. It will continue to cycle every 5 sec, until this method is called with <paramref name="enabled"/> set to false.
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        public void SetWiper(bool enabled)
        {
            Debug.WriteLine(String.Format("Visca.SetWiper({0})", enabled));
            if (enabled)
            {
                if (_wiperTimer == null)
                {
                    _wiperTimer = new System.Timers.Timer();
                    _wiperTimer.Elapsed += new System.Timers.ElapsedEventHandler(WiperTimer_Elapsed);
                }
                _wiperTimer.Interval = this.WiperInterval;
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
        /// Activates the IR emitter
        /// </summary>
        /// <param name="enabled">true is active, false if inactive</param>
        public void SetEmitter(bool enabled)
        {
            Debug.WriteLine(String.Format("Visca.SetEmitter({0})", enabled));
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
        /// Vertically inverts the image from the camera
        /// </summary>
        /// <param name="isInverted">true to invert, false for normal</param>
        public void SetOrientation(bool isInverted)
        {
            Debug.WriteLine(String.Format("Visca.SetOrientation({0})", isInverted));
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
        /// Queries the camera for the current pan, tilt, and zoom position
        /// </summary>
        public void PanTiltZoomPositionInquire()
        {
            byte[] responseBuffer;
            do
            {
                responseBuffer = SendReceiveWithResponse(new byte[] { 0x09, 0x06, 0x12 });
            } while (responseBuffer == null || (responseBuffer.Length == 1 && responseBuffer[0] == 0xff));

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
            byte[] responseBuffer = SendReceiveWithResponse(new byte[] { 0x09, 0x04, 0x47 });

            if (responseBuffer.Length > 4)
            {
                CameraControl.CurrentZoomPosition = ConvertViscaUnitsToZoom((responseBuffer[2] << 12) | (responseBuffer[3] << 8) | (responseBuffer[4] << 4) | responseBuffer[4]);
            }
            else
            {
                CameraControl.CurrentZoomPosition = 1;
            }
        }

        private void SendAndReceive(byte[] s)
        {
            lock (_serialPort)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.Write(new byte[] { 0x81 }, 0, 1);
                _serialPort.Write(s, 0, s.Length);
                _serialPort.Write(new byte[] { 0xff }, 0, 1);
                int inChar;
                do
                {
                    inChar = _serialPort.ReadByte();
                } while (inChar != 0xff);
            }
        }

        private byte[] SendReceiveWithResponse(byte[] s)
        {
            int index = 0;
            int inChar;
            byte[] receiveBuffer = new byte[512];
            _serialPort.DiscardInBuffer();
            _serialPort.Write(new byte[] { 0x81 }, 0, 1);
            _serialPort.Write(s, 0, s.Length);
            _serialPort.Write(new byte[] { 0xff }, 0, 1);

            do
            {
                if (index >= receiveBuffer.Length)
                {
                    throw new Exception("received response too big");
                }
                inChar = _serialPort.ReadByte();
                receiveBuffer[index++] = (byte)inChar;
            } while (inChar != 0xff);
            byte[] result = new byte[index];
            for (int i = 0; i < index; i++)
            {
                result[i] = receiveBuffer[i];
            }
            return result;
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

        static private int ConvertZoomToViscaUnits(double zoomPosition)
        {
            int digitalUnits = 0;
            if (zoomPosition > 25.0)
            {
                digitalUnits = Convert.ToInt32((zoomPosition - 25.0) * 1024.0);
                zoomPosition = 25.0;
            }
            int opticalUnits = Convert.ToInt32((zoomPosition - 1) * 682.67);
            return (opticalUnits + digitalUnits);
        }

        static private double ConvertViscaUnitsToZoom(int units)
        {
            double digitalZoom = 0;
            if (units > 0x4000)
            {
                digitalZoom = (units - 0x4000) / 1024;
                units = 0x4000;
            }
            double opticalZoom = (units / 682.67) + 1;
            return digitalZoom + opticalZoom;
        }

        private void AutoFocusOn()
        {
            Debug.WriteLine("Visca.AutoFocusOn");
            SendAndReceive(new byte[] { 0x01, 0x04, 0x38, 0x02 });
        }

        private void DeviceTypeInquire()
        {
            byte[] response = SendReceiveWithResponse(new byte[] { 0x09, 0x00, 0x02 });
            try
            {
                Debug.WriteLine("Visca.DeviceTypeInquire results:");
                Debug.WriteLine("Vendor " + response[2].ToString("X") + response[3].ToString("X"));
                Debug.WriteLine("Model " + response[4].ToString("X") + response[5].ToString("X"));
                Debug.WriteLine("ROM Revision " + response[6].ToString("X") + response[7].ToString("X"));
            }
            catch (IndexOutOfRangeException)
            {
                Debug.WriteLine("Warning: incomplete DeviceTypeInquire results!");
            }
        }

        private void SetServo(bool enabled)
        {
            Debug.WriteLine(String.Format("Visca.SetServo({0})", enabled));
            if (enabled)
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x16, 0x01, 0x09, 0x00, 0x08, 0x02 });
            }
            else
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x16, 0x01, 0x09, 0x0F, 0x07, 0x03 });
            }
        }

        private void SetBrake(bool enabled)
        {
            Debug.WriteLine(String.Format("Visca.SetBrake({0})", enabled));
            if (enabled)
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x16, 0x01, 0x09, 0x02, 0x00, 0x02 });
            }
            else
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x16, 0x01, 0x09, 0x0D, 0x0F, 0x03 });
            }
        }

        private void SetSlowMode(bool enabled)
        {
            Debug.WriteLine(String.Format("Visca.SetSlowMode({0})", enabled));
            if (enabled)
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x16, 0x01, 0x09, 0x00, 0x04, 0x02 });
            }
            else
            {
                SendAndReceive(new byte[] { 0x01, 0x06, 0x16, 0x01, 0x09, 0x0F, 0x0B, 0x03 });
            }
        }

        #endregion
    }
}
