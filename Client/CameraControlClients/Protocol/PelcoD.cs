using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client.CameraControlClients.Protocol
{
    /// <summary>
    /// Camera control plugin implementing the ICameraControlClient
    /// </summary>
    public abstract class PelcoD : IProtocol
    {
        /// <summary>
        /// Synchronization word used at the start of all Pelco D commands
        /// </summary>
        private static readonly byte _syncWord = 0xFF;
        /// <summary>
        /// Default device address, if none is specified
        /// </summary>
        private static readonly byte _defaultDeviceAddress = 0x01;

        protected ICameraControlClient _client;
        private Transport.ITransport _transport;

        /// <summary>
        /// The camera device address
        /// </summary>
        private byte _deviceAddress = _defaultDeviceAddress;

        /// <summary>
        /// Creates a new instance of the PelcoD control class
        /// </summary>
        /// <param name="cameraControl">parent camera control class that will own this plugin</param>
        public PelcoD(ICameraControlClient client, Transport.ITransport transport)
        {
            Debug.WriteLine("PelcoD constructor");
            _client = client;
            _transport = transport;
        }

        public abstract bool HasAbsoluteControls
        {
            get;
        }

        public abstract bool HasPan
        {
            get;
        }

        public abstract bool HasTilt
        {
            get;
        }

        public abstract bool HasZoom
        {
            get;
        }

        public abstract bool HasDigitalZoom
        {
            get;
        }

        public abstract bool HasEmitter
        {
            get;
        }

        public abstract bool HasStabilizer
        {
            get;
        }

        public abstract bool HasInfrared
        {
            get;
        }

        public abstract bool HasInverter
        {
            get;
        }

        public abstract bool HasWiper
        {
            get;
        }

        public abstract bool HasFocus
        {
            get;
        }

        public abstract double PanLimitStart
        {
            get;
        }

        public abstract double PanLimitAngle
        {
            get;
        }

        public abstract double PanOffset
        {
            get;
        }

        public abstract double TiltMaxAngle
        {
            get;
        }

        public abstract double TiltMinAngle
        {
            get;
        }

        public abstract double ZoomMaxLevel
        {
            get;
        }

        public abstract double ZoomMinLevel
        {
            get;
        }

        public abstract double FieldOfView
        {
            get;
        }

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

        public void Initialize()
        {
            _transport.Open();
        }

        public void Close()
        {
            _transport.Close();
        }

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
        protected void SendCommand(byte cmnd1, byte cmnd2, byte data1, byte data2)
        {
            byte[] packet = new byte[] { _syncWord,
                                            _deviceAddress,
                                            cmnd1,
                                            cmnd2,
                                            data1,
                                            data2,
                                            CalcChecksum(_deviceAddress, cmnd1, cmnd2, data1, data2) };

            _transport.Write(packet, 0, packet.Length);
        }

        /// <summary>
        /// Sends a Pelco D command
        /// </summary>
        /// <param name="cmnd1">command 1 byte</param>
        /// <param name="cmnd2">command 2 byte</param>
        /// <param name="data">16 bits of data</param>
        protected void SendCommand(byte cmnd1, byte cmnd2, UInt16 data)
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
        private byte CalcChecksum(byte deviceAddress, byte cmnd1, byte cmnd2, byte data1, byte data2)
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

            if ((!waitForResponse) && (_transport.BytesToRead == 0))
            {
                response = resBuilder.ToArray();
                Debug.WriteLine("ReceiveResponse (NoResponse)");
                return ResponseTypes.NoResponse;
            }
            if (waitForResponse)
            {
                byte cur;
                do
                {
                    cur = (byte)_transport.ReadByte();
                } while (cur != _syncWord);
                resBuilder.Add(cur);
            }
            
            while (_transport.BytesToRead > 0)
            {
                resBuilder.Add((byte)_transport.ReadByte());
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

        /// <summary>
        /// Returns the Least Significant Byte
        /// </summary>
        /// <param name="word">a 16 bit word</param>
        /// <returns>returns the Least Significant Byte of a 16-bit word</returns>
        private byte LoByte(UInt16 word)
        {
            return (byte)(word & 0xFF);
        }

        /// <summary>
        /// Returns the Most Significant Byte
        /// </summary>
        /// <param name="word">a 16 bit word</param>
        /// <returns>returns the Most Significant Byte of a 16-bit word</returns>
        private byte HiByte(UInt16 word)
        {
            return (byte)((word >> 8) & 0xFF);
        }

        /// <summary>
        /// Creates a word from two bytes
        /// </summary>
        /// <param name="hi">most significant byte</param>
        /// <param name="lo">least significant byte</param>
        /// <returns>the UInt16 equivelent of a MSbyte and LSbyte</returns>
        private UInt16 Word(byte hi, byte lo)
        {
            return (UInt16)((hi << 8) + lo);
        }

        /// <summary>
        /// Dumps an array of bytes to the debug console
        /// </summary>
        /// <param name="prefix">string to prefix the array with</param>
        /// <param name="stuff">the bytes to dump</param>
        private void ArrayToDebug(string prefix, byte[] stuff)
        {
            string output = prefix + " { ";
            foreach (byte b in stuff)
            {
                output += "0x" + b.ToString("X") + "  ";
            }
            Debug.WriteLine(output + "}");
        }

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
        }

        /// <summary>
        /// Issues the command to zoom to an absolute position
        /// </summary>
        /// <param name="zoomPosition">magnification level to zoom to</param>
        public virtual void ZoomAbsolute(double zoomPosition)
        {
            UInt16 value = (UInt16)(zoomPosition * 100);
            SendCommand(0x00, 0x5F, value);
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
                _client.CurrentPanAngle = (Word(response[4], response[5]) / 100.0);
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
                _client.CurrentTiltAngle = angle;
            }

            //query zoom
            SendCommand(0x00, 0x61, 0x00, 0x00);
            if (ReceiveResponse(out response, true) == ResponseTypes.ExtendedResponse)
            {
                _client.CurrentZoomPosition = (Word(response[4], response[5]) / 100.0);
            }
            else    //maybe this model doesn't support zoom, or an error occurred
            {
                _client.CurrentZoomPosition = 1.0;
            }
        }

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

        /// <summary>
        /// Disposes this class, closing the serial port
        /// </summary>
        public virtual void Dispose()
        {
            if (_transport != null)
            {
                _transport.Close();
                _transport.Dispose();
                _transport = null;
            }
        }
    }
}
