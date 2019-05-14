using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media;
using System.Threading;

namespace FutureConcepts.Media.MicrowaveReceiverController
{
    public class Vislink_HDR1000 : MicrowaveReceiver
    {
        static Vislink_HDR1000()
        {
            _hardwareSupportedModes = new List<RFVideoStandard>();
            _hardwareSupportedModes.Add(RFVideoStandard.SCM);
            _hardwareSupportedModes.Add(RFVideoStandard.DVB_T);
            _hardwareSupportedModes.Add(RFVideoStandard.LMST);
            _hardwareSupportedModes.Add(RFVideoStandard.SCM_QCD);
            _hardwareSupportedModes.Add(RFVideoStandard.DVB_ASI);

            MicrowaveCapabilities c = new MicrowaveCapabilities();
            c.FrequencyResolution = 250000;
            c.MinimumFrequency = 4400000000;
            c.MaximumFrequency = 5000000000;

            c.ReceivedCarrierLevelMinimum = -80;
            c.ReceivedCarrierLevelMaximum = 0;
            c.SignalToNoiseRatioMaximum = 40;

            c.SupportedTransports = new List<RFVideoStandard>();
            c.SupportedTransports.Add(RFVideoStandard.DVB_T);

            c.SupportedModulations = new List<RFModulationType>();
            c.SupportedModulations.Add(RFModulationType.QPSK);
            c.SupportedModulations.Add(RFModulationType.QAM16);
            c.SupportedModulations.Add(RFModulationType.QAM32);
            c.SupportedModulations.Add(RFModulationType.QAM64);
            c.SupportedModulations.Add(RFModulationType.Auto);

            c.SupportedGuardIntervals = new List<Interval>();
            c.SupportedGuardIntervals.Add(Interval._1_32);
            c.SupportedGuardIntervals.Add(Interval._1_16);
            c.SupportedGuardIntervals.Add(Interval._1_8);
            c.SupportedGuardIntervals.Add(Interval._1_4);
            c.SupportedGuardIntervals.Add(Interval._1_3);
            c.SupportedGuardIntervals.Add(Interval.Auto);

            c.SupportedBandwidth = new List<ulong>();
            c.SupportedBandwidth.Add(6000000);  // 6MHz
            c.SupportedBandwidth.Add(7000000);  // 7MHz
            c.SupportedBandwidth.Add(8000000);  // 8MHz
            c.SupportedBandwidth.Add(0);        // auto

            c.SupportedForwardErrorCorrection = new List<Interval>();
            c.SupportedForwardErrorCorrection.Add(Interval.Auto);
            c.SupportedForwardErrorCorrection.Add(Interval.Auto);   //documentation says index1 is "RS only" not clear what that means
            c.SupportedForwardErrorCorrection.Add(Interval._1_2);
            c.SupportedForwardErrorCorrection.Add(Interval._2_3);
            c.SupportedForwardErrorCorrection.Add(Interval._3_4);
            c.SupportedForwardErrorCorrection.Add(Interval._5_6);
            c.SupportedForwardErrorCorrection.Add(Interval._7_8);

            c.SupportedEncryption = new EncryptionCapabilities();
            c.SupportedEncryption.SupportedTypesAndKeyLengths = new Dictionary<EncryptionType, List<int>>();
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.None, null);
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.AES_Legacy, new int[] { 128, 256 }.ToList());
            //   c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.AES_Bcrypt, new int[] { 128, 256 }.ToList());

            //the BISS key lengths are derived from http://tech.ebu.ch/docs/tech/tech3292.pdf spec
            //48 bits are needed for the 12-character SW
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.BISS_1, new int[] { 48 }.ToList());
            //128 bits are needed for the 16-character SW
            c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.BISS_E, new int[] { 64 }.ToList());
            //    c.SupportedEncryption.SupportedTypesAndKeyLengths.Add(EncryptionType.DES, new int[] { 56 }.ToList());

            c.AutoTuning = new MicrowaveTuning();
            c.AutoTuning.ChannelBandwidth = 0;
            c.AutoTuning.Encryption = null;
            c.AutoTuning.ForwardErrorCorrection = Interval.Auto;
            c.AutoTuning.Frequency = 0;
            c.AutoTuning.GuardInterval = Interval.Auto;
            c.AutoTuning.Interleaver = false;
            c.AutoTuning.Modulation = RFModulationType.Auto;
            c.AutoTuning.PacketDiversityControl = false;
            c.AutoTuning.SpectralInversion = true;
            c.AutoTuning.TransportMode = RFVideoStandard.DVB_T;

            c.SupportedTuningParameters = (int)(MicrowaveTuning.Parameters.Frequency |
                                                MicrowaveTuning.Parameters.ChannelBandwidth |
                                                MicrowaveTuning.Parameters.ForwardErrorCorrection |
                                                MicrowaveTuning.Parameters.GuardInterval |
                                                MicrowaveTuning.Parameters.Modulation |
                                                MicrowaveTuning.Parameters.TransportMode |
                                                MicrowaveTuning.Parameters.Encryption |
                                                MicrowaveTuning.Parameters.SpectralInversion);
            c.AutoTuningRequirements = (int)MicrowaveTuning.Parameters.Frequency;

            c.SupportedLinkQualityParameters = (int)(MicrowaveLinkQuality.Parameters.BitErrorRatioPost |
                                                     MicrowaveLinkQuality.Parameters.BitErrorRatioPre |
                                                     MicrowaveLinkQuality.Parameters.DecoderLocked |
                                                     MicrowaveLinkQuality.Parameters.DemodulatorLocked |
                                                     MicrowaveLinkQuality.Parameters.FECLocked |
                                                     MicrowaveLinkQuality.Parameters.ReceivedCarrierLevel |
                                                     MicrowaveLinkQuality.Parameters.SignalToNoiseRatio |
                                                     MicrowaveLinkQuality.Parameters.TransportStreamLocked |
                                                     MicrowaveLinkQuality.Parameters.TunerLocked);

            _capabilities = c;

            //terminator list for MicrowaveReceiver API
            ETX = new List<byte[]>();
            ETX.Add(new byte[] { (byte)Terminator.End });
        }

        private int _writeReceiveTimeout;

        private UInt64 _bdcFreq = 0;

        /// <summary>
        /// Sets up an instance with BDC = 5.2GHz
        /// </summary>
        /// <param name="tcpAddress"></param>
        /// <param name="port"></param>
        /// <param name="useTCP"></param>
        /// <param name="comport"></param>
        /// <param name="baud"></param>
        public Vislink_HDR1000(string tcpAddress, int port, bool useTCP, string comport, int baud)
            : this(tcpAddress, port, useTCP, comport, baud, 5200000000)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tcpAddress"></param>
        /// <param name="port"></param>
        /// <param name="useTCP"></param>
        /// <param name="comport"></param>
        /// <param name="baud">9600</param>
        /// <param name="bdcFreq">Block Down Converter oscillator frequency. 0 if BDC not present</param>
        public Vislink_HDR1000(string tcpAddress, int port, bool useTCP, string comport, int baud, UInt64 bdcFreq)
            : base(tcpAddress, port, useTCP, comport, baud)
        {
            ReceiveTimeout = 1000;
            _writeReceiveTimeout = 10000;
            _bdcFreq = bdcFreq;

            InitializeDevice();
        }

        private int _deviceAddress = 1;

        private static MicrowaveCapabilities _capabilities;

        private static List<byte[]> ETX;

        private static List<RFVideoStandard> _hardwareSupportedModes;

        private void InitializeDevice()
        {
            //set NTSC on all outputs
            //PrintDebug("Init vi1");
            //Write(CommandCategory.VideoDecoder, "vi1", 0);
            //PrintDebug("Init vi2");
            //Write(CommandCategory.VideoDecoder, "vi2", 0);
        }

        #region Public API

        public override string GetDeviceInfo()
        {
            string endl = Environment.NewLine;
            string m = "Vislink HDR1000 Info:" + endl;

            try
            {
                m += "Serial Number: " + Read<string>(CommandCategory.General, "ser")  + endl;
                m += "Hardware revision: " + Read<string>(CommandCategory.General, "hdw") + endl;
                m += "BDC M&C version: " + Read<string>(CommandCategory.General, "sof") + endl;
                m += "BDC FPGA version: " + Read<string>(CommandCategory.General, "fpg") + endl;
                m += "BDC Hardware Revision: " + Read<string>(CommandCategory.General, "hdw") + endl;
                m += "HPC Firmware revision: " + Read<string>(CommandCategory.General, "hpv") + endl;
                m += "TCM Firmware version: " + Read<string>(CommandCategory.General, "tcv") + endl;
                m += "TCM Kernel version: " + Read<string>(CommandCategory.General, "tkv") + endl;
            }
            catch (Exception ex)
            {
                m += endl + endl + "An error occurred while retreiving all device info:" + endl;
                m += ex.ToString();
            }

            m += endl + endl;
            m += " raw command syntax:" + endl;
            m += "(r|w)<c><cmd>;[data]" + endl;
            m += "  ^   ^   ^     ^" + endl;
            m += "read/ | command |" + endl;
            m += "write |         data (optional)" + endl;
            m += "     category" + endl;
            m += "device address will be auto determined" + endl;

            return m;
        }

        public override MicrowaveCapabilities GetCapabilities()
        {
            return _capabilities;
        }

        private MicrowaveTuning _lastSetTuning;

        /// <summary>
        /// hold this lock while performing a whole action, like SetTuning, GetLinkQuailty, etc...
        /// </summary>
        private readonly object _stateLock = new object();

        public override void SetTuning(MicrowaveTuning tuning)
        {
            _lastSetTuning = tuning;
            lock (_stateLock)
            {
                try
                {

                    Write(CommandCategory.Tuner, "tsf", FreqProt2Dev(tuning.Frequency));

                    Write(CommandCategory.Demodulator, "opr", GetDemodMode(tuning.TransportMode));

                    //HACK the Vislink device fails when we write anything to d-mod. Waiting For Firmware Fix
                    //Write(CommandCategory.Demodulator, "mod", IndexOf(_capabilities.SupportedModulations, tuning.Modulation));

                    //Write(CommandCategory.Demodulator, "gua", IndexOf(_capabilities.SupportedGuardIntervals, tuning.GuardInterval));

                    if (tuning.TransportMode == RFVideoStandard.DVB_T)
                    {
                        //Write(CommandCategory.Demodulator, "wid", IndexOf(_capabilities.SupportedBandwidth, tuning.ChannelBandwidth));
                    }

                    //Write(CommandCategory.Demodulator, "fec", IndexOf(_capabilities.SupportedForwardErrorCorrection, tuning.ForwardErrorCorrection));

                    Write(CommandCategory.Demodulator, "pol", (tuning.SpectralInversion ? 1 : 0)); //TODO support other than Auto?


                    int type;
                    string key;
                    GetEncryptionType(tuning.Encryption, out type, out key);

                    Write(CommandCategory.VideoDecoder, "scr", type);
                    if (key != null)
                    {
                        Write(CommandCategory.VideoDecoder, "key", key);
                    }
                }
                finally
                {
                    Thread.Sleep(4000);   //give the MRX some chill time to transition to the new tuning
                    DoTuningChangeEvent();
                }
            }
        }
        
        public override MicrowaveTuning GetTuning()
        {
            lock (_stateLock)
            {
                MicrowaveTuning t = new MicrowaveTuning();
                t.ValidParameterData = (int)(MicrowaveTuning.Parameters.Frequency |
                                             MicrowaveTuning.Parameters.TransportMode |
                                             MicrowaveTuning.Parameters.Modulation |
                                             MicrowaveTuning.Parameters.GuardInterval |
                                             MicrowaveTuning.Parameters.ForwardErrorCorrection |
                                             MicrowaveTuning.Parameters.SpectralInversion |
                                             MicrowaveTuning.Parameters.Encryption);

                t.Frequency = FreqDev2Prot(Read<double>(CommandCategory.Tuner, "tsf"));

                t.TransportMode = GetDemodMode(Read<int>(CommandCategory.Demodulator, "opr"));

                int temp = Read<int>(CommandCategory.Demodulator, "mod");
                t.Modulation = _capabilities.SupportedModulations[temp];

                temp = Read<int>(CommandCategory.Demodulator, "gua");
                t.GuardInterval = _capabilities.SupportedGuardIntervals[temp];

                if (t.TransportMode == RFVideoStandard.DVB_T)
                {
                    temp = Read<int>(CommandCategory.Demodulator, "wid");
                    t.ChannelBandwidth = _capabilities.SupportedBandwidth[temp];
                    t.ValidParameterData |= (int)MicrowaveTuning.Parameters.ChannelBandwidth;
                }
                else
                {
                    t.ChannelBandwidth = 0;
                }

                temp = Read<int>(CommandCategory.Demodulator, "fec");
                t.ForwardErrorCorrection = _capabilities.SupportedForwardErrorCorrection[temp];

                temp = Read<int>(CommandCategory.Demodulator, "pol");
                t.SpectralInversion = temp == 1; //TODO do we need 3 states?

                temp = Read<int>(CommandCategory.VideoDecoder, "scr");
                t.Encryption = GetEncryptionType(temp);
                //the HDR1000 does not let us retreive the key (per BISS spec), so grab it from the last-known key
                // if the encryption types are the same.
                if (_lastSetTuning != null)
                {
                    if (_lastSetTuning.Encryption != null)
                    {
                        if ((t.Encryption.Type == _lastSetTuning.Encryption.Type) && (_lastSetTuning.Encryption.DecryptionKey != null))
                        {
                            t.Encryption.DecryptionKey = _lastSetTuning.Encryption.DecryptionKey;
                        }
                    }
                }

                return t;
            }
        }

        /// <summary>
        /// Retreives a full measure of link quality from the receiver
        /// </summary>
        /// <returns>a full measure of link quality from the receiver</returns>
        public override MicrowaveLinkQuality GetLinkQuality()
        {
            lock (_stateLock)
            {
                MicrowaveLinkQuality q = new MicrowaveLinkQuality();

                double snr1 = Read<double>(CommandCategory.Tuner, "sr1");
                double snr2 = Read<double>(CommandCategory.Tuner, "sr2");
                double rs1 = Read<double>(CommandCategory.Tuner, "rs1");
                double rs2 = Read<double>(CommandCategory.Tuner, "rs2");
                //make sure the sign is "-"
                if (rs1 > 0)
                    rs1 *= -1;
                if (rs2 > 0)
                    rs2 *= -1;

                PrintDebug("RSSI1: " + rs1 + "dB SNR1: " + snr1 + "dB RSSI2: " + rs2 + "dB SNR2: " + snr2 + "dB");

                q.SignalToNoiseRatio = Math.Max(snr1, snr2);
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.SignalToNoiseRatio;
                q.ReceivedCarrierLevel = Math.Max(rs1, rs2);
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.ReceivedCarrierLevel;

                q.TunerLocked = 0 == Read<int>(CommandCategory.Tuner, "syl");
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.TunerLocked;

                q.BitErrorRatioPre = Read<double>(CommandCategory.Demodulator, "pre");
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.BitErrorRatioPre;
                q.BitErrorRatioPost = Read<double>(CommandCategory.Demodulator, "pos");
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.BitErrorRatioPost;

                q.DemodulatorLocked = 0 == Read<int>(CommandCategory.Demodulator, "dmd");
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.DemodulatorLocked;
                q.TransportStreamLocked = 0 == Read<int>(CommandCategory.Demodulator, "tsl");
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.TransportStreamLocked;
                q.FECLocked = 0 == Read<int>(CommandCategory.Demodulator, "fel");
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.FECLocked;
                q.DecoderLocked = 0 == Read<int>(CommandCategory.VideoDecoder, "dec");
                q.ValidParameterData |= (int)MicrowaveLinkQuality.Parameters.DecoderLocked;

                return q;
            }
        }

        public override bool TestConnection()
        {
            lock (_stateLock)
            {
                byte[] packet = ConstructPacket(1, RWMode.Read, CommandCategory.General, "add");

                bool success;
                byte[] response = BlockingSend(packet,
                                               out success,
                                               ETX);

                int addr;
                ResponseStatus status;
                byte[] data;
                ParseResponse(response, out addr, out status, out data);
                //HACK apparently read g-add status CommandFailure means it worked
                if (status == ResponseStatus.CommandFailure)
                {
                    _deviceAddress = addr;  //basically the first device to reply will be the one we're dealing with
                    PrintDebug(" Receiver at serial address " + _deviceAddress);

                    this.Connected = true;
                    return true;
                }
                else
                {
                    PrintDebug("status = " + status + " was not expected.");
                }

                this.Connected = false;
                return false;
            }
        }

        internal override string RawCommand(string userinput)
        {
            string reply = string.Empty;

            try
            {
                string[] parts = userinput.Split(new char[] { ';' }, 2);
                RWMode mode = (RWMode)parts[0][0];
                CommandCategory cat = (CommandCategory)parts[0][1];
                string cmd = parts[0].Substring(2, 3);
                reply = mode + " " + cat + "-" + cmd + Environment.NewLine;

                byte[] data = new byte[0];
                if(parts.Length == 2)
                {
                    data = System.Text.Encoding.ASCII.GetBytes(parts[1]);
                }
                byte[] packet = ConstructPacket(_deviceAddress, mode, cat, cmd, data);
                bool success;
                byte[] response = BlockingSend(packet, out success, Vislink_HDR1000.ETX);
                if (response != null)
                {
                    int addr;
                    ResponseStatus status;
                    byte[] respData;
                    ParseResponse(response, out addr, out status, out respData);
                    reply += "Received Bytes: " + ByteArrayToString(response) + Environment.NewLine;
                    reply += " Addr = " + addr + " Status = " + status + " (" + (char)status + ") Data = \"" + ToAsciiChars(respData) + "\"" + Environment.NewLine;
                }
                else
                {
                    reply += "<no response received>" + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                reply += Environment.NewLine + ex.ToString();
            }
            return reply;
        }

        #endregion

        #region Convert Between Serial Protocol and FC Data Structures

        /// <summary>
        /// Converts Frequency in Hz to the string format read by the device, compensating for a Block Down Converter
        /// </summary>
        /// <param name="protocolFrequency">frequency in Hz</param>
        /// <returns>a string for the device to accept</returns>
        private string FreqProt2Dev(UInt64 protocolFrequency)
        {
            UInt64 devFreq = protocolFrequency;
            //the device accepts the REAL frequency, but returns the INTERMEDIATE freq
            //if (_bdcFreq != 0)
            //{
            //    devFreq = _bdcFreq - protocolFrequency;
            //    PrintDebug("Block Down Converter: " + protocolFrequency + " -> " + devFreq);
            //}

            double freq = devFreq / 1000000.0; //scale to fractional MHz
            return freq.ToString("0.000"); //should this be format "0000.000" ?
        }

        /// <summary>
        /// Converts Frequency in Fractional MHz to plain Hz, compensating for a Block Down Converter
        /// </summary>
        /// <param name="deviceFrequency">frequency reported by device</param>
        /// <returns>the tuned frequency in Hz</returns>
        private UInt64 FreqDev2Prot(double deviceFrequency)
        {
            UInt64 dFreqHz = (UInt64)(deviceFrequency * 1000000); //scale from fractional MHz to Hz
            //the device accepts the REAL frequency, but returns the INTERMEDIATE freq
            UInt64 realFreq = dFreqHz;
            if (_bdcFreq != 0)
            {
                realFreq = _bdcFreq - dFreqHz;
                PrintDebug("Un-Block Down Converter: " + dFreqHz + " -> " + realFreq);
            }

            return realFreq;
        }

        /// <summary>
        /// Converts a decryption mode (h-scr) value to EncryptionInfo 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private EncryptionInfo GetEncryptionType(int type)
        {
            EncryptionInfo info = new EncryptionInfo();
            info.DecryptionKey = null;
            info.EncryptionKey = null;
            info.KeyLength = 0;
            switch (type)
            {
                case 0:
                    info.Type = EncryptionType.None;
                    break;
                case 1:
                    info.Type = EncryptionType.AES_Legacy;
                    info.KeyLength = 128;
                    break;
                case 2:
                    info.Type = EncryptionType.AES_Legacy;
                    info.KeyLength = 256;
                    break;
                case 3:
                    info.Type = EncryptionType.AES_Bcrypt;
                    info.KeyLength = 128;
                    break;
                case 4:
                    info.Type = EncryptionType.AES_Bcrypt;
                    info.KeyLength = 256;
                    break;
                case 5:
                    info.Type = EncryptionType.BISS_1;
                    info.KeyLength = _capabilities.SupportedEncryption.SupportedTypesAndKeyLengths[EncryptionType.BISS_1][0];
                    break;
                case 6:
                    info.Type = EncryptionType.BISS_E;
                    info.KeyLength = _capabilities.SupportedEncryption.SupportedTypesAndKeyLengths[EncryptionType.BISS_E][0];
                    break;
                case 7:
                    info.Type = EncryptionType.DES;
                    info.KeyLength = _capabilities.SupportedEncryption.SupportedTypesAndKeyLengths[EncryptionType.DES][0];
                    break;
                default:
                    throw new NotSupportedException("Unknown value from h-scr returned! \"" + type + "\"");
            }
            return info;
        }

        /// <summary>
        /// Converts Encryption Info to the Vislink required format
        /// </summary>
        /// <param name="info">info to convert</param>
        /// <param name="type">integer type indicator for h-scr parameter</param>
        /// <param name="key">32 char string for h-key parameter</param>
        private void GetEncryptionType(EncryptionInfo info, out int type, out string key)
        {
            if (info == null)
            {
                info = new EncryptionInfo() { Type = EncryptionType.None };
            }

            switch (info.Type)
            {
                case EncryptionType.None:
                    type = 0;
                    break;
                case EncryptionType.AES_Legacy:
                    if (info.KeyLength == 128)
                    {
                        type = 1;
                    }
                    else if (info.KeyLength == 256)
                    {
                        type = 2;
                    }
                    else
                    {
                        throw new NotSupportedException("Encryption algorithm " + info.Type + " with a key length of " + info.KeyLength + " bits is not supported!");
                    }
                    break;
                case EncryptionType.AES_Bcrypt:
                    if (info.KeyLength == 128)
                    {
                        type = 3;
                    }
                    else if (info.KeyLength == 256)
                    {
                        type = 4;
                    }
                    else
                    {
                        throw new NotSupportedException("Encryption algorithm " + info.Type + " with a key length of " + info.KeyLength + " bits is not supported!");
                    }
                    break;
                case EncryptionType.BISS_1:
                    type = 5;
                    break;
                case EncryptionType.BISS_E:
                    type = 6;
                    break;
                case EncryptionType.DES:
                    type = 7;
                    break;
                default:
                    throw new NotSupportedException("Encryption algorithim " + info.Type + " is not supported!");
            }

            if (info.Type != EncryptionType.None)
            {
                if (info.DecryptionKey == null)
                {
                    throw new ArgumentNullException("EncryptionInfo.DecryptionKey", "DecryptionKey may not be null");
                }

                if ((info.KeyLength / 4) != info.DecryptionKey.Length)
                {
                    throw new ArgumentOutOfRangeException("KeyLength does not match DecryptionKey length!");
                }

                StringBuilder k = new StringBuilder();
                foreach (byte b in info.DecryptionKey)
                {
                    k.Append(b.ToString("X1"));
                }
                key = k.ToString();
            }
            else
            {
                key = null;
            }
        }

        private int GetDemodMode(RFVideoStandard demod)
        {
            for (int i = 0; i < _hardwareSupportedModes.Count; i++)
            {
                if (_hardwareSupportedModes[i] == demod)
                {
                    return i;
                }
            }
            throw new NotSupportedException("The RFVideoStandard \"" + demod + "\" is not supported by this hardware");
        }

        private RFVideoStandard GetDemodMode(int index)
        {
            if ((index < 0) || (index >= _hardwareSupportedModes.Count))
            {
                throw new NotSupportedException("Unrecognized d-opr value: " + index);
            }

            return _hardwareSupportedModes[index];
        }

        #endregion

        #region Read/Write Helper Methods

        private int _serialRetries = 5;

        private void Write<T>(CommandCategory category, string command, T value)
        {
            //adjust the ReceiveTimeout for write operations, since the device doesn't respond
            // until its done effecting the change
            int temp = this.ReceiveTimeout;
            this.ReceiveTimeout = _writeReceiveTimeout;

            try
            {
                PrintDebug("WRITE " + category + "-" + command);
                byte[] data = null;
                if (typeof(T) == typeof(byte))
                {
                    data = new byte[1];
                    data[0] = (byte)Convert.ChangeType(value, typeof(byte));
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    data = (byte[])Convert.ChangeType(value, typeof(byte[]));
                }
                else
                {
                    string szValue = value.ToString();
                    data = ToAsciiBytes(szValue);
                }

                byte[] packet = ConstructPacket(_deviceAddress, RWMode.Write, category, command, data);
                bool success;
                byte[] response = BlockingSend(packet, out success, ETX);
                if (success)
                {
                    int addr;
                    ResponseStatus status;
                    byte[] respData;
                    ParseResponse(response, out addr, out status, out respData);

                    if (status != ResponseStatus.OK)
                    {
                        throw new Exception("Command " + category + "-" + command + " failed. Receiver responded with failure: " + status);
                    }
                }
                else
                {
                    if (response == null)
                        throw new Exception("Command " + category + "-" + command + " failed. Send failed!");
                    else
                        throw new Exception("Command " + category + "-" + command + " failed. No data received!");
                }

            }
            finally
            {
                //restore Read timeout
                this.ReceiveTimeout = temp;
            }
        }

        /// <summary>
        /// Issuse the specified READ command to the receiver
        /// </summary>
        /// <typeparam name="T">type of data we're trying to retreive</typeparam>
        /// <param name="category">command category</param>
        /// <param name="command">command to issue</param>
        /// <returns>the data we're requesting</returns>
        private T Read<T>(CommandCategory category, string command)
        {
            int tries = 0;
            do
            {
                try
                {
                    PrintDebug("READ " + category + "-" + command);
                    byte[] packet = ConstructPacket(_deviceAddress, RWMode.Read, category, command);
                    bool success;
                    byte[] response = BlockingSend(packet, out success, ETX);
                    if (success)
                    {
                        int addr;
                        ResponseStatus status;
                        byte[] data;
                        ParseResponse(response, out addr, out status, out data);
                        if (data == null)
                        {
                            throw new Exception("Command " + category + "-" + command + " failed. No data parsed! addr=" + addr + " status=" + status.ToString());
                        }


                        string d = ToAsciiChars(data);
                        return (T)Convert.ChangeType(d, typeof(T));
                    }
                    else
                    {
                        if (response == null)
                            throw new Exception("Command " + category + "-" + command + " failed. Send failed!");
                        else
                            throw new Exception("Command " + category + "-" + command + " failed. No data received!");
                    }
                }
                catch (FormatException ex)
                {
                    PrintDebug(ex.ToString());
                }
                catch (TimeoutException ex)
                {
                    PrintDebug(ex.ToString());
                }
            }
            while (tries++ < _serialRetries);

            throw new TimeoutException("Command " + category + "-" + command + " failed. The number of Read attempts was exceeded.");
        }

        private void ParseResponse(byte[] response, out int address, out ResponseStatus status, out byte[] data)
        {
            if (response[0] != (byte)Terminator.Start)
            {
                throw new FormatException("Response did not start with STX");
            }

            string addy = ToAsciiChars(response, 1, 4);
            address = int.Parse(addy);
            status = (ResponseStatus)response[5];

            int i = IndexOf(response, (byte)Terminator.Separator);
            if (i == -1)
            {
                data = null;
            }
            else
            {
                data = new byte[i - 6];
                Array.Copy(response, 6, data, 0, i - 6);
            }
        }

        #endregion

        #region Packetizing Helpers

        /// <summary>
        /// locates the index of the target object
        /// </summary>
        /// <param name="array">array to search</param>
        /// <param name="target">item to find</param>
        /// <returns>the index of target, or -1 if not found</returns>
        private int IndexOf<T>(T[] array, T target)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if ((array[i] == null) && (target == null))
                {
                    return i;
                }
                else if ((array[i] == null) && (target != null))
                {
                    continue;
                }
                else if (array[i].Equals(target))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Wraps the IList.IndexOf method, and throws an exception if the item is not found
        /// </summary>
        /// <typeparam name="T">type of list we're looking in</typeparam>
        /// <param name="array">list to search</param>
        /// <param name="target">target item to find</param>
        /// <returns>an index in the range [0,array.Count), or throws NotSupportedException</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the target is not found</exception>
        private int IndexOf<T>(IList<T> array, T target)
        {
            int i = array.IndexOf(target);
            if (i == -1)
            {
                throw new NotSupportedException((target == null) ? "Couldn't find null in supported list." : "Couldn't find \"" + target.ToString() + "\" in supported list!");
            }
            return i;
        }

        /// <summary>
        /// Returns the 4 byte chunk indicating the device address
        /// </summary>
        /// <param name="device">device address: [0,9999]</param>
        /// <returns>4 bytes for the address field</returns>
        private byte[] Address(int device)
        {
            if ((device < 0) || (device > 9999))
            {
                throw new ArgumentOutOfRangeException("Devices addresses must be in the range [0,9999]");
            }

            return ToAsciiBytes(device.ToString(), 4, 0x30);
        }

        /// <summary>
        /// Converts bytes to characters
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string ToAsciiChars(byte[] bytes)
        {
            return ToAsciiChars(bytes, 0, 0);
        }


        private string ToAsciiChars(byte[] bytes, int offset, int count)
        {
            if (count == 0)
            {
                count = bytes.Length;
                if (offset != 0)
                {
                    throw new IndexOutOfRangeException("You cannot take more bytes than the count if offset != 0");
                }
            }

            ASCIIEncoding e = new ASCIIEncoding();
            char[] ch = e.GetChars(bytes, offset, count);
            return new string(ch);
        }

        /// <summary>
        /// Converts any text to ASCII bytes
        /// </summary>
        /// <param name="text">text to convert</param>
        /// <returns>the bytes needed to represent this text</returns>
        private byte[] ToAsciiBytes(string text)
        {
            ASCIIEncoding e = new ASCIIEncoding();
            return e.GetBytes(text);
        }

        /// <summary>
        /// Converts any text to ASCII bytes, generating the indicated number of bytes
        /// </summary>
        /// <param name="text">text to convert</param>
        /// <param name="targetLength">the length of the array to return</param>
        /// <param name="padding">the byte value to use if padding is needed</param>
        /// <returns>the array holding the data, padded with leading 0-bytes if needed</returns>
        private byte[] ToAsciiBytes(string text, int targetLength, byte padding)
        {
            byte[] output = ToAsciiBytes(text);

            if (output.Length > targetLength)
            {
                throw new ArgumentException("text \"" + text + "\" is too long to fit in \"" + targetLength + "\" bytes!");
            }
            else if (output.Length < targetLength)
            {
                //pad the front of the array with 0-bytes if needed
                byte[] temp = output;
                output = new byte[targetLength];
                for (int o = targetLength - 1, t = temp.Length - 1; o>= 0; t--, o--)
                {
                    if (t >= 0)
                    {
                        output[o] = temp[t];
                    }
                    else
                    {
                        output[0] = padding;
                    }
                }
            }

            return output;

        }

        private byte[] ConstructPacket(int deviceAddress, RWMode readOrWrite, CommandCategory category, string command, params byte[] data)
        {
            List<byte> packet = new List<byte>(14 + data.Length);

            // 0
            packet.Add((byte)Terminator.Start);
            // 1-4
            packet.AddRange(Address(deviceAddress));
            // 5
            packet.Add((byte)readOrWrite);
            // 6
            packet.Add((byte)category);
            // 7-9
            packet.AddRange(ToAsciiBytes(command, 3, 0x00));
            // 10
            packet.Add((byte)Terminator.Separator);

            if (data.Length > 0)
            {
                packet.AddRange(data);
                
            }

            packet.Add((byte)Terminator.Separator);

            packet.Add(CheckSum(packet));

            packet.Add((byte)Terminator.End);

            return packet.ToArray();
        }

        /// <summary>
        /// Calculates a check sum for a packet, skipping the first byte, and assuming that the
        /// packet does not currently contain a check sum or ETX terminator.
        /// </summary>
        /// <param name="packet">packet to generate for</param>
        /// <returns>the check sum</returns>
        private byte CheckSum(List<byte> packet)
        {
            int sum = 0;
            for (int i = 1; i < packet.Count; i++)
            {
                sum += packet[i];
            }
            return (byte)((sum % 256) | 0x80);
        }

        #endregion

        #region Enumerations for Commands

        internal enum Terminator : byte
        {
            Start       = 0x02, // STX
            Separator   = 0x3B, // ';'
            End         = 0x03  // ETX
        }

        internal enum RWMode : byte
        {
            Read    = 0x72,   // 'r'
            Write   = 0x77    // 'w'
        }

        internal enum CommandCategory : byte
        {
            /// <summary>
            /// 'f'
            /// </summary>
            Tuner               = 0x66,
            /// <summary>
            /// 'd'
            /// </summary>
            Demodulator         = 0x64,
            /// <summary>
            /// 'h'
            /// </summary>
            VideoDecoder        = 0x68,
            /// <summary>
            /// 'c'
            /// </summary>
            ControlProcessor    = 0x63,
            /// <summary>
            /// 'n'
            /// </summary>
            Network             = 0x6E,
            /// <summary>
            /// 'g'
            /// </summary>
            General             = 0x67,
            /// <summary>
            /// 'l'
            /// </summary>
            Calibration         = 0x6C
        }
        
        /// <summary>
        /// The values of the status byte (index 5) sent in a reply
        /// </summary>
        internal enum ResponseStatus : byte
        {
            /// <summary>
            /// '1' - All OK
            /// </summary>
            OK              = 0x31,
            /// <summary>
            /// 'D' - Data out of range
            /// </summary>
            DataOutOfRange  = 0x44,
            /// <summary>
            /// 'F' - Format error / wrong data length
            /// </summary>
            FormatError     = 0x46,
            /// <summary>
            /// 'E' - General error, command could not be actioned
            /// </summary>
            CommandFailure  = 0x45
        }

        #endregion

        #region Base Class Overrides
#if VISLINK_RECEIVEUNTIL
        /// <summary>
        /// This is a more-different ReceiveUntil method, optimized for fastness on serial connections.
        /// If more than 1 terminator is specified, or connected via TCP, the base class method is used
        /// </summary>
        /// <param name="terminators">Only allows a single 1-byte terminator</param>
        /// <returns>received bytes, including the terminator</returns>
        protected override byte[] ReceiveUntil(List<byte[]> terminators)
        {
            if ((base._UseTCP) || (terminators.Count != 1))
            {
                return base.ReceiveUntil(terminators);
            }
            else
            {
                serialConnection.ReadTimeout = this.ReceiveTimeout;

                int index = 0;
                int inChar;
                byte[] receiveBuffer = new byte[512];
                do
                {
                    if (index >= receiveBuffer.Length)
                    {
                        throw new Exception("received response too big");
                    }
                    inChar = serialConnection.ReadByte();
                    receiveBuffer[index++] = (byte)inChar;
                }
                while ((byte)inChar != terminators[0][0]);

                byte[] result = new byte[index];
                for (int i = 0; i < index; i++)
                {
                    result[i] = receiveBuffer[i];
                }
                return result;
            }
        }
#endif

        #endregion
    }
}
