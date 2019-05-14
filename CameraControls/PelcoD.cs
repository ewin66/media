using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.CameraControls
{
    /// <summary>
    /// Camera control plugin implementing the Pelco D protocol
    /// </summary>
    public class PelcoD : ICameraControlPlugin
    {
        /// <summary>
        /// Synchronization word used at the start of all Pelco D commands
        /// </summary>
        protected static readonly byte _syncWord = 0xFF;
        /// <summary>
        /// Default device address, if none is specified
        /// </summary>
        protected static readonly byte _defaultDeviceAddress = 0x01;
        /// <summary>
        /// Pelco D protocol specifies all devices must support operation at 2400bps -- other speeds by negotiation
        /// </summary>
        protected static readonly int _baudRate = 2400;

        /// <summary>
        /// The camera device address
        /// </summary>
        protected byte _deviceAddress;
        /// <summary>
        /// Configuration
        /// </summary>
        protected CameraControlInfo _config;
        /// <summary>
        /// SerialPort used for communication
        /// </summary>
        protected SerialPort _serialPort = null;
        /// <summary>
        /// Hold this lock while performing operations on the serial port
        /// </summary>
        protected readonly object _serialPortLock = new object();
        /// <summary>
        /// Camera Control interface that our plug in is working with
        /// </summary>
        protected ICameraControl _cameraControl;

        /// <summary>
        /// restrictions, capabilities, and limits
        /// </summary>
        protected CameraCapabilitiesAndLimits _capabilities;

        #region ICameraControlPlugin Members

        /// <summary>
        /// Creates a new instance of the PelcoD control class
        /// </summary>
        /// <param name="config">configuration for the camera and the method of communication</param>
        /// <param name="cameraControl">parent camera control class that will own this plugin</param>
        public PelcoD(CameraControlInfo config, ICameraControl cameraControl)
        {
            Debug.WriteLine("CameraControls.PelcoD.Load");

            _cameraControl = cameraControl;

            try
            {
                _config = config;
                string portName = ParsePortAndAddress(_config.Address);
                _serialPort = new SerialPort(portName, _baudRate, Parity.None, 8, StopBits.One);
                _serialPort.Open();
                _serialPort.DiscardNull = false;
                _serialPort.DtrEnable = true;
                _serialPort.ReadTimeout = 10000;

                Debug.WriteLine("CameraControls.PelcoD.Open opened = " + _serialPort.PortName + " @ " + _baudRate + " baud");
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
                throw new Exception("Unable to open serial port", exc);
            }

            _capabilities = _config.Capabilities;
        }

        /// <summary>
        /// Initializes the plug in for use
        /// </summary>
        /// <remarks>
        /// Pelco D protocol requires no initialization
        /// </remarks>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// Parses the desired port and device address from a configuration's Address string
        /// </summary>
        /// <param name="portAndAddress">Config.CameraControl.Address</param>
        /// <returns>returns the name of the COM port to connect to</returns>
        protected string ParsePortAndAddress(string portAndAddress)
        {
            string[] addressInfo = portAndAddress.Split(':');
            if (addressInfo.Length > 2)
            {
                throw new ArgumentException("More than one ':' character was found in the address string!", "cameraControlConfig.Address");
            }
            else if (addressInfo.Length == 2)
            {
                //parse the device address from the 2nd half of the address string
                if (!byte.TryParse(addressInfo[1], out _deviceAddress))
                {
                    _deviceAddress = _defaultDeviceAddress;
                    Debug.WriteLine("CameraControls.PelcoD: Could not determine device address from string \"" + addressInfo[1] + "\".");
                }
            }
            else
            {
                _deviceAddress = _defaultDeviceAddress;
                Debug.WriteLine("CameraControls.PelcoD: No device address specified.");
            }

            Debug.WriteLine("CameraControls.PelcoD: Using device address: " + _deviceAddress.ToString());
            return addressInfo[0];
        }

        #endregion

        #region Device Specifc Methods

        /// <summary>
        /// The various types of responses available in the Pelco-D protocol. Each item's value is the number of bytes in the response
        /// </summary>
        protected enum ResponseTypes
        { 
            /// <summary>
            /// An unrecognized response
            /// </summary>
            Unknown = -1,
            /// <summary>
            /// No response
            /// </summary>
            NoResponse = 0,
            /// <summary>
            /// General Response
            /// </summary>
            GeneralResponse = 4,
            /// <summary>
            /// Extended Response
            /// </summary>
            ExtendedResponse = 7,
            /// <summary>
            /// Query Response
            /// </summary>
            QueryResponse = 18
        };

        /// <summary>
        /// Sends a Pelco-D packet across the com port
        /// </summary>
        /// <remarks>
        /// properly addresses the packet, and calculates checksum.
        /// </remarks>
        /// <param name="cmnd1">command 1 byte</param>
        /// <param name="cmnd2">command 2 byte</param>
        /// <param name="data1">data 1 byte</param>
        /// <param name="data2">data 2 byte</param>
        protected virtual void SendCommand(byte cmnd1, byte cmnd2, byte data1, byte data2)
        {
            lock (_serialPortLock)
            {
                _serialPort.DiscardInBuffer();

                byte[] packet = new byte[] { _syncWord,
                                             _deviceAddress,
                                             cmnd1,
                                             cmnd2,
                                             data1,
                                             data2,
                                             CalcChecksum(_deviceAddress, cmnd1, cmnd2, data1, data2) };

                _serialPort.Write(packet, 0, packet.Length);

             //   ArrayToDebug("SendCommand: ", packet);

            }
        }

        /// <summary>
        /// Sends a Pelco D command
        /// </summary>
        /// <param name="cmnd1">command 1 byte</param>
        /// <param name="cmnd2">command 2 byte</param>
        /// <param name="data">16 bits of data</param>
        protected virtual void SendCommand(byte cmnd1, byte cmnd2, UInt16 data)
        {
            SendCommand(cmnd1, cmnd2, HiByte(data), LoByte(data));
        }

        /// <summary>
        /// Cacluates the checksum for the given parameters
        /// </summary>
        /// <param name="deviceAddress">device address</param>
        /// <param name="cmnd1">command 1 byte</param>
        /// <param name="cmnd2">command 2 byte</param>
        /// <param name="data1">data 1 byte</param>
        /// <param name="data2">data 2 byte</param>
        /// <returns>the checksum for these data</returns>
        protected byte CalcChecksum(byte deviceAddress, byte cmnd1, byte cmnd2, byte data1, byte data2)
        {
            return (byte)((deviceAddress + cmnd1 + cmnd2 + data1 + data2) % 256);
        }

        /// <summary>
        /// Receives a response from the input buffer
        /// </summary>
        /// <param name="response">the output response buffer</param>
        /// <param name="waitForResponse">
        /// set to true if you wish to wait until a response is seen on the serial port
        /// set to false if you expect a response is already there
        /// </param>
        /// <returns></returns>
        protected virtual ResponseTypes ReceiveResponse(out byte[] response, bool waitForResponse)
        {
            List<byte> resBuilder = new List<byte>();

            if ((!waitForResponse) && (_serialPort.BytesToRead == 0))
            {
                response = resBuilder.ToArray();
                Debug.WriteLine("ReceiveResponse (NoResponse)");
                return ResponseTypes.NoResponse;
            }

            lock (_serialPortLock)
            {
                if (waitForResponse)
                {
                    byte cur;
                    do
                    {
                        cur = (byte)_serialPort.ReadByte();
                    } while (cur != _syncWord);
                    resBuilder.Add(cur);
                }
            
                while (_serialPort.BytesToRead > 0)
                {
                    resBuilder.Add((byte)_serialPort.ReadByte());
                }

                response = resBuilder.ToArray();
                ResponseTypes type;
                try
                {
                    type = (ResponseTypes)resBuilder.Count;
                }
                catch (Exception)
                {
                    type = ResponseTypes.Unknown;
                }

                ArrayToDebug("ReceiveResponse (" + type.ToString() + "): ", response);

                return type;
            }          
        }

        /// <summary>
        /// Returns the Least Significant Byte
        /// </summary>
        /// <param name="word">a 16 bit word</param>
        /// <returns>returns the Least Significant Byte of a 16-bit word</returns>
        protected byte LoByte(UInt16 word)
        {
            return (byte)(word & 0xFF);

        }

        /// <summary>
        /// Returns the Most Significant Byte
        /// </summary>
        /// <param name="word">a 16 bit word</param>
        /// <returns>returns the Most Significant Byte of a 16-bit word</returns>
        protected byte HiByte(UInt16 word)
        {
            return (byte)((word >> 8) & 0xFF);
        }

        /// <summary>
        /// Creates a word from two bytes
        /// </summary>
        /// <param name="hi">most significant byte</param>
        /// <param name="lo">least significant byte</param>
        /// <returns>the UInt16 equivelent of a MSbyte and LSbyte</returns>
        protected UInt16 Word(byte hi, byte lo)
        {
            return (UInt16)((hi << 8) + lo);
        }

        /// <summary>
        /// Dumps an array of bytes to the debug console
        /// </summary>
        /// <param name="prefix">string to prefix the array with</param>
        /// <param name="stuff">the bytes to dump</param>
        protected void ArrayToDebug(string prefix, byte[] stuff)
        {
            string output = prefix + " { ";
            foreach (byte b in stuff)
            {
                output += "0x" + b.ToString("X") + "  ";
            }
            Debug.WriteLine(output + "}");
        }

        #endregion

        #region ICameraControl Members

        /// <summary>
        /// Issues the command to pan the camera to an absolute position
        /// </summary>
        /// <param name="panAngle">an angle in the range [0, +360)</param>
        /// <param name="tiltAngle">an angle in the range [-90, +90]</param>
        public virtual void PanTiltAbsolute(double panAngle, double tiltAngle)
        {
            if ((panAngle >= 0) && (panAngle < 360))
            {
                UInt16 value = (UInt16)(panAngle * 100);
                SendCommand(0x00, 0x4B, value);
                _cameraControl.CurrentPanAngle = panAngle;
            }
            else
            {
                throw new ArgumentException("Argument out of range for protocol.", "panAngle");
            }


            if ((tiltAngle < 0) && (tiltAngle >= -90))
            {
                tiltAngle *= -1;
            }
            else if ((tiltAngle >= 0) && (tiltAngle <= 90))
            {
                tiltAngle = 360 - tiltAngle;
            }
            else
            {
                throw new ArgumentException("Argument out of range for protocol.", "tiltAngle");
            }
            UInt16 value2 = (UInt16)(tiltAngle * 100);
            SendCommand(0x00, 0x4D, value2);
            _cameraControl.CurrentTiltAngle = tiltAngle;
        }

        /// <summary>
        /// Issues the command to zoom to an absolute position
        /// </summary>
        /// <param name="zoomPosition">magnification level to zoom to</param>
        public virtual void ZoomAbsolute(double zoomPosition)
        {
            UInt16 value = (UInt16)(zoomPosition * 100);
            SendCommand(0x00, 0x5F, value);
            _cameraControl.CurrentZoomPosition = zoomPosition;
        }

        /// <summary>
        /// Turns on or off the infrared capture filter
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        /// <exception cref="System.NotImplementedException">Pelco D protocol has no standardized mechanism for this feature.</exception>
        public virtual void SetInfrared(bool enabled)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Turns on or off image stabilization
        /// </summary>
        /// <param name="enabled"></param>
        /// <exception cref="System.NotImplementedException">Pelco D protocol has no standardized mechanism for this feature.</exception>
        public virtual void SetStabilizer(bool enabled)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Turns or or off the wiper
        /// </summary>
        /// <param name="enabled"></param>
        /// <exception cref="System.NotImplementedException">Pelco D protocol has no standardized mechanism for this feature.</exception>
        public virtual void SetWiper(bool enabled)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Turns on or off the infrared emitter
        /// </summary>
        /// <param name="enabled"></param>
        /// <exception cref="System.NotImplementedException">Pelco D protocol has no standardized mechanism for this feature.</exception>
        public virtual void SetEmitter(bool enabled)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Turns on or off vertical image inversion
        /// </summary>
        /// <param name="inverted"></param>
        /// <exception cref="System.NotImplementedException">Pelco D protocol has no standardized mechanism for this feature.</exception>
        public virtual void SetOrientation(bool inverted)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the camera for its current pan, tilt, and zoom data
        /// </summary>
        public virtual void PanTiltZoomPositionInquire()
        {
            byte[] response;
            //query pan
            SendCommand(0x00, 0x51, 0x00, 0x00);
            if (ReceiveResponse(out response, true) == ResponseTypes.ExtendedResponse)
            {
                _cameraControl.CurrentPanAngle = (Word(response[4], response[5]) / 100.0);
            }

            //query tilt
            SendCommand(0x00, 0x53, 0x00, 0x00);
            if (ReceiveResponse(out response, true) == ResponseTypes.ExtendedResponse)
            {
                double angle = Word(response[4], response[5]) / 100.0;
                if ((angle > 0) && (angle <= 90))
                {
                    angle *= -1;
                }
                else if ((angle <= 360) && (angle >= 270))
                {
                    angle = 360 - angle;
                }
                _cameraControl.CurrentTiltAngle = angle;
            }

            //query zoom
            SendCommand(0x00, 0x61, 0x00, 0x00);
            if (ReceiveResponse(out response, true) == ResponseTypes.ExtendedResponse)
            {
                _cameraControl.CurrentZoomPosition = (Word(response[4], response[5]) / 100.0);
            }
          //  else    //maybe this model doesn't support zoom, or an error occurred
          //  {
          //      _cameraControl.CurrentZoomPosition = 1.0;
          //  }
        }

        #endregion

        #region Position Preset Support

        /// <summary>
        /// Returns the lowest valid preset index
        /// </summary>
        protected virtual int LowestPresetIndex { get { return 0x01; } }

        /// <summary>
        /// Returns the highest valid preset index
        /// </summary>
        protected virtual int HighestPresetIndex { get { return 0xFF; } }

        /// <summary>
        /// Sets the current position of the camera to the given preset ID
        /// </summary>
        /// <param name="id">preset's ID</param>
        protected virtual void SetPreset(byte id)
        {
            SendCommand(0x00, 0x03, 0x00, id);
        }

        /// <summary>
        /// Clears the given preset on the camera
        /// </summary>
        /// <param name="id">preset's ID</param>
        protected virtual void ClearPreset(byte id)
        {
            SendCommand(0x00, 0x05, 0x00, id);
        }

        /// <summary>
        /// Restores the camera to the given preset
        /// </summary>
        /// <param name="id">preset's ID</param>
        protected virtual void CallPreset(byte id)
        {
            SendCommand(0x00, 0x07, 0x00, id);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes this class, closing the serial port
        /// </summary>
        public virtual void Dispose()
        {
            if (_serialPort != null)
            {
                //SetEmitter(false);
                Debug.WriteLine("Closing " + _serialPort.PortName);
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        #endregion
    }
}
