using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;

namespace FutureConcepts.Media.Server.RoscoTest
{
    public class RVisionControl : IDisposable
    {
        private SerialPort _serialPort = null;
        private string _comPort = null;

        public void Dispose()
        {
            if (_serialPort != null)
            {
                if (_comPort != null)
                {
                    Console.WriteLine("Closing {0}", _comPort);
                }
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        public void Open(string comPort)
        {
            Console.WriteLine("Open " + comPort);
            _comPort = comPort;
            _serialPort = new SerialPort(_comPort, 9600, Parity.None, 8, StopBits.One);
            _serialPort.Open();
            _serialPort.DiscardNull = false;
            _serialPort.DtrEnable = true;
            _serialPort.ReadTimeout = 2000;
            Console.WriteLine("Opened = " + _comPort);
        }

        public void PowerOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x00, 0x02 });
        }

        public void PowerOff()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x00, 0x03 });
        }

        public int ZoomStop()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x07, 0x00 });
            return ZoomPositionInq();
        }

        public void ZoomTeleStandard()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x07, 0x02 });
        }

        public void ZoomWideStandard()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x07, 0x03 });
        }

        public void ZoomTeleVariable(int speed)
        {
            byte[] s = { 0x01, 0x04, 0x07, 0x20 };
            s[3] |= (byte)speed;
            SendAndReceive(s);
        }

        public void ZoomWideVariable(int speed)
        {
            byte[] s = { 0x01, 0x04, 0x07, 0x30 };
            s[3] |= (byte)speed;
            SendAndReceive(s);
        }

        public void ZoomDirect(int zoomPosition)
        {
            byte[] s = { 0x01, 0x04, 0x47, 0, 0, 0, 0 };
            s[3] = (byte)((zoomPosition & 0xf000) >> 12);
            s[4] = (byte)((zoomPosition & 0xf00) >> 8);
            s[5] = (byte)((zoomPosition & 0xf0) >> 4);
            s[6] = (byte)(zoomPosition & 0xf);
            Console.WriteLine("zoomPosition: {0}", zoomPosition.ToString());

            SendAndReceive(s);
        }

        public void ZoomDigitalZoomOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x06, 0x02 });
        }

        public void ZoomDigitalZoomOff()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x06, 0x03 });
        }

        public void ZoomDirectDigital(int zoomPosition)
        {
            byte[] s ={ 0x01, 0x04, 0x46, 0x00, 0x00, 0x00, 0x00 };
            s[3] = (byte)((zoomPosition & 0xf000) >> 12);
            s[4] = (byte)((zoomPosition & 0xf00) >> 8);
            s[5] = (byte)((zoomPosition & 0xf0) >> 4);
            s[6] = (byte)(zoomPosition & 0xf);
            Console.WriteLine("zoomPosition: {0}", zoomPosition.ToString());

            SendAndReceive(s);
        }

        public void FocusStop()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x08, 0x00 });
        }

        public void FocusFarStandard()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x08, 0x02 });
        }

        public void FocusNearStandard()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x08, 0x03 });
        }

        public void FocusAutoFocusOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x38, 0x02 });
        }

        public void FocusManualFocusOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x38, 0x03 });
        }

        public void FocusInfinity()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x18, 0x02 });
        }

        public void BrightReset()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x0d, 0x00 });
        }

        public void BrightUp()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x0d, 0x02 });
        }

        public void BrightDown()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x0d, 0x03 });
        }

        public void NearInfraredModeOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x01, 0x02 });
        }

        public void NearInfraredModeOff()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x01, 0x03 });
        }

        public void HomeSet()
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x04, 0x02 });
        }

        public void HomeGoto()
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x04, 0x03 });
        }

        public void Reset()
        {
            SendReceiveWithResponse(new byte[] { 0x01, 0x06, 0x05 });
        }

        public void CycleWiper()
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x22, 0xff });
        }

        public void StabilizerOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x34, 0x02 });
        }

        public void StabilizerOff()
        {
            SendAndReceive(new byte[] { 0x01, 0x04, 0x34, 0x03 });
        }

        public void EmitterOn()
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x14, 0x02 });
        }

        public void EmitterOff()
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x14, 0x03 });
        }

        public void PanTiltUp(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x03, 0x01 });
        }

        public void PanTiltDown(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x03, 0x02 });
        }

        public void PanTiltLeft(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x01, 0x03 });
        }

        public void PanTiltRight(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x02, 0x03 });
        }

        public void PanTiltUpLeft(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x01, 0x01 });
        }

        public void PanTiltUpRight(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x02, 0x01 });
        }

        public void PanTiltDownLeft(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x01, 0x02 });
        }

        public void PanTiltDownRight(int speed)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, (byte)speed, (byte)speed, 0x02, 0x02 });
        }

        public void PanTiltStop(out int panAngle, out int tiltAngle)
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x01, 0, 0, 0x03, 0x03, 0xff });
            PanTiltPosInq(out panAngle, out tiltAngle);
        }

        public void PanTiltAbsolutePosition(int panAngle, int tiltAngle, int speed)
        {
            byte[] s = { 0x01, 0x06, 0x02, 0x28, 0x28, 0x00, 0x08, 0x07, 0x08, 0x00, 0x08, 0x07, 0x08 };
            s[3] = (byte)speed;
            s[4] = (byte)speed;
            s[5] = (byte)((panAngle & 0xf00) >> 8);
            s[6] = (byte)((panAngle & 0xf0) >> 4);
            s[7] = (byte)(panAngle & 0xf);
            s[8] = 0;
            s[9] = (byte)((tiltAngle & 0xf00) >> 8);
            s[10] = (byte)((tiltAngle & 0xf0) >> 4);
            s[11] = (byte)(tiltAngle & 0xf);
            s[12] = 0;
            SendAndReceive(s);
        }

        public void PanTiltPosInq(out int panAngle, out int tiltAngle)
        {
            byte[] responseBuffer;
            do
            {
                responseBuffer = SendReceiveWithResponse(new byte[] { 0x09, 0x06, 0x12 });
            } while (responseBuffer.Length == 1 && responseBuffer[0] == 0xff);

            panAngle = 0xFF;
            tiltAngle = 0xFF;

            if (responseBuffer.Length > 8)
            {
                panAngle = (responseBuffer[2] << 8) | (responseBuffer[3] << 4) | responseBuffer[4];
                tiltAngle = (responseBuffer[6] << 8) | (responseBuffer[7] << 4) | responseBuffer[8];
            }
        }

        public void OrientationInverted()
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x17, 0x03 });
        }

        public void OrientationNotInverted()
        {
            SendAndReceive(new byte[] { 0x01, 0x06, 0x17, 0x02 });
        }

        public int ZoomPositionInq()
        {
            int zoom_pos;
            byte[] responseBuffer = SendReceiveWithResponse(new byte[] { 0x09, 0x04, 0x47 });

            if (responseBuffer.Length > 4)
            {
                zoom_pos = (responseBuffer[2] << 12) | (responseBuffer[3] << 8) | (responseBuffer[4] << 4) | responseBuffer[4];
            }
            else
            {
                zoom_pos = 7;
            }
            return zoom_pos;
        }

        public byte[] GetDeviceInfo()
        {
            return SendReceiveWithResponse(new byte[] { 0x09, 0x00, 0x02 } );
        }

        public byte[] GetRevisionInfo()
        {
            return SendReceiveWithResponse(new byte[] { 0x09, 0x06, 0x1c });
        }

        public byte[] GetSerialNumbers()
        {
            return SendReceiveWithResponse(new byte[] { 0x09, 0x06, 0x26, 0x00 });
        }

        private void SendAndReceive(byte[] s)
        {
            lock (_serialPort)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.Write(new byte[] { 0x81 }, 0, 1);
                _serialPort.Write(s, 0, s.Length);
                _serialPort.Write(new byte[] { 0xff }, 0, 1);
                Console.WriteLine("Getting response");
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
            Console.WriteLine("Getting response");
            try
            {
                do
                {
                    if (index >= receiveBuffer.Length)
                    {
                        Console.WriteLine("received response too big");
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
            catch
            {
                Console.WriteLine("Serial Interface Time out");
                return null;
            }
        }
    }
}
