using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using FutureConcepts.Media.CameraControls;
using FutureConcepts.Tools;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace FutureConcepts.Media.Tools.CameraControlTester
{
    public partial class PelcoDTryCommand : Form
    {
        public PelcoDTryCommand()
        {
            InitializeComponent();
        }

        private SerialPort _serialPort;
        /// <summary>
        /// Current serial port
        /// </summary>
        private SerialPort SerialPort
        {
            get { return _serialPort; }
            set { _serialPort = value; }
        }

        public string SerialPortName { get; set; }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                byte c1, c2, d1, d2;
                c1 = byte.Parse(cmd1.Text, NumberStyles.HexNumber);
                c2 = byte.Parse(cmd2.Text, NumberStyles.HexNumber);
                d1 = byte.Parse(data1.Text, NumberStyles.HexNumber);
                d2 = byte.Parse(data2.Text, NumberStyles.HexNumber);

                byte[] packetOut, responseIn;
                SendCommand(c1, c2, d1, d2, out packetOut);
                ShowPortDialog(Dir.Out, packetOut);
              //  ReceiveResponse(out responseIn);
              //  ShowPortDialog(Dir.In, responseIn);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "SendCommand failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private enum Dir { In, Out };

        private delegate void ShowPortDialogDelegate(Dir d, byte[] b);

        private void ShowPortDialog(Dir dir, byte[] speak)
        {
            if(this.InvokeRequired)
            {
                Delegate d = new ShowPortDialogDelegate(ShowPortDialog);
                this.Invoke(d, new object[]{dir, speak});
                return;
            }
            tbReceived.Text += ArrayToString(((dir == Dir.In) ? " <<  " : "  >> "), speak) + Environment.NewLine;
            tbReceived.Select(tbReceived.Text.Length, 0);
            tbReceived.ScrollToCaret();
        }

        private void btnCmd1PP_Click(object sender, EventArgs e)
        {
            byte b = byte.Parse(cmd1.Text, NumberStyles.HexNumber);
            cmd1.Text = (b + 1).ToString("X2");
            btnSend_Click(this, new EventArgs());
        }

        private void btnCmd2PP_Click(object sender, EventArgs e)
        {
            cmd2.Text = "00";
            byte b;
            do
            {
                Thread.Sleep(500);
                b = byte.Parse(cmd2.Text, NumberStyles.HexNumber);
                cmd2.Text = (b + 1).ToString("X2");
                btnSend_Click(this, new EventArgs());
            } while (b < 255);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            byte[] trash;
            SendCommand(0x00, 0x00, 0x00, 0x00, out trash);
            ShowPortDialog(Dir.Out, trash);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbReceived.Text = "";
        }

        private void FieldTextChanged(object sender, EventArgs e)
        {
            try
            {
                chk.Text = CalcChecksum(byte.Parse(addr.Text, NumberStyles.HexNumber),
                                        byte.Parse(cmd1.Text, NumberStyles.HexNumber),
                                        byte.Parse(cmd2.Text, NumberStyles.HexNumber),
                                        byte.Parse(data1.Text, NumberStyles.HexNumber),
                                        byte.Parse(data2.Text, NumberStyles.HexNumber)).ToString("X2");
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #region Pelco-D methods

        private byte DeviceAddress
        {
            get
            {
                return byte.Parse(addr.Text, NumberStyles.HexNumber);
            }
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
        private void SendCommand(byte cmnd1, byte cmnd2, byte data1, byte data2, out byte[] packet)
        {

          //  SerialPort.DiscardInBuffer();

            packet = new byte[] { 0xFF,
                                  DeviceAddress,
                                  cmnd1,
                                  cmnd2,
                                  data1,
                                  data2,
                                  CalcChecksum(DeviceAddress, cmnd1, cmnd2, data1, data2) };

            SerialPort.Write(packet, 0, packet.Length);

            //   ArrayToDebug("SendCommand: ", packet);


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
        private void ReceiveResponse(out byte[] response)
        {
            List<byte> resBuilder = new List<byte>();

            byte cur;
            do
            {
                cur = (byte)SerialPort.ReadByte();
                resBuilder.Add(cur);
            } while (SerialPort.BytesToRead > 0);
            

            response = resBuilder.ToArray();
        }

        protected string ArrayToString(string prefix, byte[] stuff)
        {
            string output = prefix;
            foreach (byte b in stuff)
            {
                output += b.ToString("X2") + "  ";
            }
            return output;
        }

        #endregion

        private void PelcoDTryCommand_Load(object sender, EventArgs e)
        {
            try
            {
                string[] parts = this.SerialPortName.Split(':');
                if(parts.Length > 1)
                {
                    this.SerialPortName = parts[0];
                    this.addr.Text = byte.Parse(parts[1], NumberStyles.HexNumber).ToString("X2");
                }

                this.SerialPort = new SerialPort(this.SerialPortName, 2400, Parity.None, 8, StopBits.One);
                this.SerialPort.DiscardNull = false;
                //this.SerialPort.DtrEnable = true;
                this.SerialPort.ReadTimeout = 10000;
                this.SerialPort.Open();
                this.SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);


                FieldTextChanged(this, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Can't open serial port!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] response;
            ReceiveResponse(out response);
            ShowPortDialog(Dir.In, response);
        }

        private void PelcoDTryCommand_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.SerialPort != null)
                {
                    this.SerialPort.Close();
                    this.SerialPort.Dispose();
                    this.SerialPort = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Can't close serial port!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
