using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using FutureConcepts.Media.Contract;
using System.IO;

namespace FutureConcepts.Media.MicrowaveReceiverController
{
    /// <summary>
    /// Provides all methods required to talk to our current model of Microwave Receiver. 
    /// For an actual implementation, instantiate PMR_AR100. All frequencies are in MHz
    /// </summary>
    public abstract class MicrowaveReceiver : IDisposable
    {
        /// <summary>
        /// Possible Operating bands for the receiver. Band depends on 
        /// frequency.
        /// </summary>
        public enum OperatingBand
        {
            /// <summary>
            /// 1-2GHz
            /// </summary>
            L,
            /// <summary>
            /// 2-4GHz
            /// </summary>
            S,
            /// <summary>
            /// 4-8GHz
            /// </summary>
            C,
            /// <summary>
            /// 8-12GHz
            /// </summary>
            X,
            /// <summary>
            /// not set
            /// </summary>
            NotSet
        }

        protected readonly object _streamLock = new object();

        /// <summary>
        /// The time to wait between polling the link quality
        /// </summary>
        protected int LinkQualityPollInterval = 30000;

        public event EventHandler ReceiverConnectionChange;
        public virtual event EventHandler<ReceiverEventArgs> ReceiverTuningChange;
        public virtual event EventHandler<ReceiverEventArgs> ReceiverBandChange;
        public virtual event EventHandler<ReceiverEventArgs> ReceiverLinkQualityChange;
        public virtual event EventHandler<ReceiverEventArgs> ReceiverErrorOccurred;
      //  protected event EventHandler<DataArgs> DataReceived;
        protected class DataArgs : EventArgs
        {
            public DataArgs(byte[] data) { DataSent = data; }
            public byte[] DataSent { get; set; }
        }

        /// <summary>
        /// Event fired whenever the frequency, Operating Band, or Connected 
        /// status of the receiver changes.
        /// </summary>
        public class ReceiverEventArgs : EventArgs
        {
            /// <summary>
            /// New tuning of the receiver.
            /// </summary>
            public MicrowaveTuning Tuning = null;
            /// <summary>
            /// New Operating Band of the receiver.
            /// </summary>
            public OperatingBand Band = OperatingBand.NotSet;
            /// <summary>
            /// New Link Quality of the receiver
            /// </summary>
            public MicrowaveLinkQuality LinkQuality = null;
            /// <summary>
            /// New Connected status of the receiver.
            /// </summary>
            public bool Connected = false;
            /// <summary>
            /// An error that has occurred
            /// </summary>
            public Exception Error = null;
        }

        #region Connection Management (Serial/TCP)

        protected SerialPort serialConnection;
        protected TcpClient tcpConnection;

        private string _TCPAddress;
        private int _TCPPort;
        protected bool _UseTCP { get; private set; }
        private string _commport;
        private int _baud;

        private void SetUpSerial()
        {
            Close();
            try
            {
                serialConnection = new SerialPort(_commport, _baud, Parity.None, 8, StopBits.One);
                //serialConnection.DataReceived += new SerialDataReceivedEventHandler(serialConnection_DataReceived);

                serialConnection.ReadTimeout = 10000; //HACK test only
                serialConnection.Open();
                Connected = TestConnection();
            }
            catch
            {
                Close();
                throw;
            }
        }

        private void SetUpTCP()
        {
            Close();
            try
            {
                tcpConnection = new TcpClient();
                tcpConnection.Connect(IPAddress, _TCPPort);
                Connected = TestConnection();
            }
            catch
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Closes all underlying connections
        /// </summary>
        public void Close()
        {
            if (serialConnection != null)
            {
                //serialConnection.DataReceived -= new SerialDataReceivedEventHandler(serialConnection_DataReceived);
                serialConnection.Close();
                serialConnection.Dispose();
                serialConnection = null;
            }
            if (tcpConnection != null)
            {
                //tcpConnection.NewTCPDataReceived -= new FCTCPClient.NewTCPDataReceivedHandler(tcpConnection_NewTCPDataReceived);
                tcpConnection.GetStream().Close();
                tcpConnection.Close();
                tcpConnection = null;
            }

            this.Connected = false;
        }

        public void Dispose()
        {
            StopPollingLinkQuality();

            ReceiverConnectionChange = null;
            ReceiverTuningChange = null;
            ReceiverBandChange = null;
            ReceiverLinkQualityChange = null;
            ReceiverErrorOccurred = null;

            Close();
        }

        //void serialConnection_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    _readerWait.Set();
        //    if (DataReceived != null)
        //        DataReceived.Invoke(serialConnection, new DataArgs(Encoding.ASCII.GetBytes(serialConnection.ReadExisting())));
        //}

        //void tcpConnection_NewTCPDataReceived(byte[] data, int readBytes)
        //{
        //    _readerWait.Set();
        //    if (DataReceived != null)
        //        DataReceived.Invoke(tcpConnection, new DataArgs(data));
        //}

        private IPAddress _address;
        public IPAddress IPAddress
        {
            get
            {

                if (_TCPAddress != null && IPAddress.TryParse(_TCPAddress, out _address))
                {
                    return _address;
                }
                else
                {
                    try
                    {
                        if (Dns.GetHostAddresses("serial").Length > 0)
                            _address = Dns.GetHostAddresses("serial")[0];
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc.Message + "\r\n" + exc.StackTrace);
                    }
                }
                return _address;
            }
        }

        private bool _connected;
        /// <summary>
        /// True if the controller has been verified as connected. 
        /// When this value changes, the <see cref="ReceiverConnectionChange"/> 
        /// event will fire.
        /// </summary>
        public bool Connected
        {
            get
            {
                return _connected;
            }
            protected set
            {
                _connected = value;
                if (!value)
                {
                    Debug.WriteLine("Disconnected!");
                }
                if (value != _connected)
                {
                    if (ReceiverConnectionChange != null)
                        this.ReceiverConnectionChange(this, new EventArgs());
                }
            }
        }

        #endregion

        /// <summary>
        /// Pulls the tuning and raises an event
        /// </summary>
        protected void DoTuningChangeEvent()
        {
            ReceiverEventArgs arg = new ReceiverEventArgs();
            arg.Band = OperatingBand.NotSet;
            arg.Tuning = GetTuning();
            arg.Connected = Connected;

            if (ReceiverTuningChange != null)
                ReceiverTuningChange.Invoke(this, arg);
        }

        /// <summary>
        /// pulls the Band, and raises an event
        /// </summary>
        protected void DoBandChangeEvent()
        {
            ReceiverEventArgs arg = new ReceiverEventArgs();
            arg.Band = GetBand();
            arg.Tuning = null;
            arg.Connected = Connected;

            if (ReceiverBandChange != null)
                ReceiverBandChange.Invoke(this, arg);
        }

        /// <summary>
        /// Pulls the Link Quality and raises an event
        /// </summary>
        protected void DoLinkQualityChangeEvent()
        {
            ReceiverEventArgs arg = new ReceiverEventArgs();
            arg.Band = OperatingBand.NotSet;
            arg.Tuning = null;
            arg.LinkQuality = GetLinkQuality();
            arg.Connected = Connected;

            if (ReceiverLinkQualityChange != null)
                ReceiverLinkQualityChange.Invoke(this, arg);
        }

        #region IO

        //protected AutoResetEvent _readerWait = new AutoResetEvent();
        protected readonly object SendMutex = new object();

        /// <summary>
        /// Performs a Send/Receive cycle
        /// </summary>
        /// <param name="toSend">data to send</param>
        /// <param name="success">indicator if the operation was successful or not</param>
        /// <param name="responseTerminators">list of response terminators</param>
        /// <returns>
        /// True if data was returned.
        /// If false and the result is null, then there was a failure during Send.
        /// If false, and the result is an empty array, then there was a failure receiving.
        /// </returns>
        protected byte[] BlockingSend(byte[] toSend, out bool success, List<byte[]> responseTerminators)
        {
            byte[] result = null;
            lock (SendMutex)
            {
                DiscardInBuffer();
                success = Send(toSend);
                if (!success)
                {
                    return result;
                }
                //Debug.WriteLine("MW Rx - Sent: " + Encoding.ASCII.GetString(toSend));
                result = ReceiveUntil(responseTerminators);
                //Debug.WriteLine("MW Rx - Received: " + Encoding.ASCII.GetString(result));
                success = result.Length > 0;
            }
            return result;
        }

        protected void DiscardInBuffer()
        {
            lock (_streamLock)
            {
                if (_UseTCP)
                {
                    if (tcpConnection != null && tcpConnection.Connected)
                    {
                        NetworkStream s = tcpConnection.GetStream();
                        if (s.DataAvailable)
                        {
                            while (-1 != s.ReadByte()) ;
                        }
                    }
                }
                else
                {
                    if (serialConnection != null)
                    {
                        serialConnection.DiscardInBuffer();
                    }
                }
            }
        }

        protected bool Send(byte[] toSend)
        {
            lock (_streamLock)
            {
                if (_UseTCP)
                {
                    if (tcpConnection != null && tcpConnection.Connected)
                    {
                        //tcpConnection.SendBytes(toSend, 0, toSend.Length);
                        tcpConnection.GetStream().Write(toSend, 0, toSend.Length);
                        tcpConnection.GetStream().Flush();
                        return true;
                    }
                }
                else
                {
                    if (serialConnection != null)
                    {
                        serialConnection.Write(toSend, 0, toSend.Length);
                        return true;
                    }
                }
                return false;
            }
        }

        private int _receiveTimeout = 1000;
        /// <summary>
        /// Use this to control the timeout in ReceiveUntil, in milliseconds
        /// </summary>
        protected int ReceiveTimeout
        {
            get
            {
                return _receiveTimeout;
            }
            set
            {
                _receiveTimeout = value;
            }
        }

        private int _receiveSleepChunk = 20;

        protected virtual byte[] ReceiveUntil(List<byte[]> terminators)
        {
            lock (_streamLock)
            {
                int maxSleeps = ReceiveTimeout / 50;

                int totalBytes = 0;
                byte[] buffer = new byte[100];
                int sleepCount = 0;
                for (int i = 0; totalBytes == 0 && i < buffer.Length; i++)
                {
                    //Get a byte if one is available
                    try
                    {
                        if (_UseTCP && tcpConnection.GetStream().DataAvailable)
                        {
                            buffer[i] = (byte)tcpConnection.GetStream().ReadByte();
                        }
                        else if (!_UseTCP && serialConnection.BytesToRead > 0)
                        {
                            buffer[i] = (byte)serialConnection.ReadByte();
                        }
                        else
                        {//Check if this is the end
                            foreach (byte[] terminator in terminators)
                            {
                                //Compare to terminators
                                if (i >= terminator.Length)
                                {
                                    byte[] possibleMatch = new byte[terminator.Length];
                                    Array.Copy(buffer, i - terminator.Length, possibleMatch, 0, terminator.Length);
                                    if (ArrayCompare(possibleMatch, terminator))
                                    {
                                        totalBytes = i;
                                        break;
                                    }
                                }
                            }
                            if (totalBytes != 0)
                                break;
                            //No bytes available, no terminators reached, sleep for a tenth of the total time
                            if (sleepCount > maxSleeps)//We've waited as long as we can - return what we have
                            {
                                totalBytes = i;
                                PrintDebug("ReceiveUntil Timed out! (" + ReceiveTimeout + "ms)");
                                throw new TimeoutException("ReceiveUntil Timed out! (" + ReceiveTimeout + "ms)");
                            }
                            i--;//If no bytes are available, don't move forward a slot.
                            //if (i > 0)//Only sleep if bytes have been received
                            //{
                            Thread.Sleep(50);
                            sleepCount++;
                            //}
                        }
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Received Junk: " + Encoding.ASCII.GetString(buffer));
                        Debug.WriteLine(exc.Message + "\r\n" + exc.StackTrace);
                    }
                }
                //copy down to actual amount of bytes
                byte[] result = new byte[totalBytes];
                Array.Copy(buffer, result, totalBytes);
                return result;
            }
        }

        #endregion

        protected bool ArrayCompare(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length) return false;
            bool itemcompare = true;
            for (int j = 0; j < arr1.Length; j++)
            {
                if (!arr1[j].Equals(arr2[j]))
                {
                    itemcompare = false;
                    break;
                }
            }
            return itemcompare;
        }

        #region Public API

        /// <summary>
        /// Initializes a controller for a Microwave Receiver,
        /// using the passed in settings to communicate with it.
        /// </summary>
        /// <param name="tcpaddress">IP or hostname of serial interface device.</param>
        /// <param name="port">Port on serial device to which receiver is connected.</param>
        /// <param name="useTCP">True to use TCP, otherwise use serial communication.</param>
        /// <param name="comport">COM port to use if communicating serially.</param>
        /// <param name="baud">Baud rate to use if communicating serially.</param>
        public MicrowaveReceiver(string tcpaddress, int port, bool useTCP, string comport, int baud)
        {
            this._TCPAddress = tcpaddress;
            this._TCPPort = port;
            this._commport = comport;
            this._baud = baud;
            this._UseTCP = useTCP;
            if (useTCP)
            {
                SetUpTCP();
            }
            else
            {
                SetUpSerial();
            }
        }

        /// <summary>
        /// Tunes the receiver to the specified frequency, then verifies 
        /// that the receiver thinks it is set to that frequency.
        /// </summary>
        /// <param name="freq">Receiver frequency in MHz</param>
        public abstract void SetTuning(MicrowaveTuning tuning);

        /// <summary>
        /// Allows a receiver to identify its capabilities
        /// </summary>
        /// <returns>the capabilties</returns>
        public abstract MicrowaveCapabilities GetCapabilities();

        /// <summary>
        /// Returns detailed information about the device, such as serial number
        /// or firmware versions. Typically only used by a technician reporting information.
        /// </summary>
        /// <returns>
        /// a string containg information about the device,
        /// and specifying any special syntax for the Raw command
        /// </returns>
        public abstract string GetDeviceInfo();

        /// <summary>
        /// Technician back door for raw device communication
        /// </summary>
        /// <param name="userinput">user input from the command line or some text</param>
        /// <returns>information regarding the status of the command</returns>
        internal abstract string RawCommand(string userinput);

        /// <summary>
        /// Retrieves the frequency the receiver is currently tuned to.
        /// </summary>
        /// <returns>Receiver frequency in MHz</returns>
        public abstract MicrowaveTuning GetTuning();

        /// <summary>
        /// Retrieves the signal strength for the frequency the receiver is
        /// currently tuned to.
        /// </summary>
        /// <returns>Signal strength as a percentage (of 100).</returns>
        public abstract MicrowaveLinkQuality GetLinkQuality();

        /// <summary>
        /// Returns Operating Band of receiver.
        /// </summary>
        /// <returns>Current receiver band of operation.</returns>
        public virtual OperatingBand GetBand()
        {
            MicrowaveTuning t = this.GetTuning();
            if (t.Frequency > 1000000000)
            {
                if (t.Frequency < 2000000000)
                {
                    return OperatingBand.L;
                }
                else if (t.Frequency < 4000000000)
                {
                    return OperatingBand.S;
                }
                else if (t.Frequency < 8000000000)
                {
                    return OperatingBand.C;
                }
                else if (t.Frequency < 12000000000)
                {
                    return OperatingBand.X;
                }
            }

            return OperatingBand.NotSet;
        }


        /// <summary>
        /// Test communication with the receiver, and set the <see cref="Connected"/>
        ///  property to true if successful.
        /// </summary>
        /// <returns>True if able to communicate with the receiver.</returns>
        public abstract bool TestConnection();

        #endregion

        #region Streaming Link Quality

        private long _pollingOn = 0;

        /// <summary>
        /// Spawns a worker thread to stream Link Quality data, 1 parameter at a time
        /// </summary>
        public void StartPollingLinkQuality()
        {
            if (Interlocked.Read(ref _pollingOn) == 1)
            {
                throw new InvalidOperationException("Polling already started");
            }

            Interlocked.Exchange(ref _pollingOn, 1);
            ThreadPool.QueueUserWorkItem(new WaitCallback((arg) =>
            {
                try
                {
                    while (Interlocked.Read(ref _pollingOn) == 1)
                    {
                        Thread.Sleep(LinkQualityPollInterval);
                        OnPollLinkQuality();
                    }
                }
                catch (Exception ex)
                {
                    Interlocked.Exchange(ref _pollingOn, 0);
                    RaiseErrorOccurred(ex);
                }
            }));
        }

        protected void RaiseErrorOccurred(Exception ex)
        {
            if (ReceiverErrorOccurred != null)
            {
                ReceiverErrorOccurred(this, new ReceiverEventArgs() { Error = ex });
            }
        }

        /// <summary>
        /// Stop Streaming Link Quality data
        /// </summary>
        public void StopPollingLinkQuality()
        {
            Interlocked.Exchange(ref _pollingOn, 0);
        }

        /// <summary>
        /// Raise a link quality change event
        /// </summary>
        /// <param name="linkQuality">link quality data to broadcast</param>
        protected void RaiseLinkQualityChanged(MicrowaveLinkQuality linkQuality)
        {
            if (this.ReceiverLinkQualityChange != null)
            {
                this.ReceiverLinkQualityChange(this, new ReceiverEventArgs() { Connected = this.Connected, LinkQuality = linkQuality });
            }
        }

        protected virtual void OnPollLinkQuality()
        {
            try
            {
                RaiseLinkQualityChanged(GetLinkQuality());
            }
            catch (Exception ex)
            {
                RaiseErrorOccurred(ex);
            }
        }

        #endregion

        protected virtual void PrintDebug(string debug)
        {
            Debug.WriteLine(DateTime.Now.ToLongTimeString() + ": MicrowaveReceiver Library - " + debug);
        }

        protected string ByteArrayToString(byte[] array)
        {
            string urmom = string.Empty;
            foreach(byte b in array)
            {
                urmom += b.ToString("X2") + " ";
            }
            return urmom;
        }

        protected virtual void PrintDebug(byte[] array)
        {
            Debug.WriteLine(DateTime.Now.ToLongTimeString() + ": MicrowaveReceiver Library " + ByteArrayToString(array) + "}");
        }
    }
}
