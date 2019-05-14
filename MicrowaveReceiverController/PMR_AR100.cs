using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media;

namespace FutureConcepts.Media.MicrowaveReceiverController
{
    public class PMR_AR100 : MicrowaveReceiver
    {
        /// <summary>
        /// All commands sent to and received from this device must start with this code
        /// </summary>
        internal const byte STARTCODE = (byte)'$';
        /// <summary>
        /// All commands sent to and received from this device must end with this code
        /// </summary>
        internal const byte ENDCODE = (byte)'#';
        /// <summary>
        /// This command will return Fnnnn where nnnn is the Frequency it is on.
        /// </summary>
        internal readonly byte[] CHECKFREQ = new byte[] { (byte)'C', (byte)'F' };
        internal readonly byte[] REMOTEMODE = new byte[] { (byte)'M', (byte)'R' };
        internal readonly byte[] SETFREQUENCY = new byte[] { (byte)'F' };
        internal readonly byte[] SETBAND = new byte[] { (byte)'Q' };
        internal readonly byte[] CHECKBAND = new byte[] { (byte)'C', (byte)'Q' };
        internal readonly byte[] CHECKSIGNAL = new byte[] { (byte)'C', (byte)'S', (byte)'%' };

        internal const byte VALID = (byte)'V';
        internal const byte INVALID = (byte)'I';

        /// <summary>
        /// Attaches StartCode and EndCode to given command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private byte[] BuildCommand(byte[] command)
        {
            byte[] toSend = new byte[command.Length + 2];
            toSend[0] = STARTCODE;
            toSend[toSend.Length - 1] = ENDCODE;
            Array.Copy(command, 0, toSend, 1, command.Length);
            return toSend;
        }
        private byte[] BuildCommand(byte[] command, string payload)
        {
            byte[] PayLoad = Encoding.ASCII.GetBytes(payload);
            byte[] toSend = new byte[command.Length + PayLoad.Length + 2];
            toSend[0] = STARTCODE;
            toSend[toSend.Length - 1] = ENDCODE;
            Array.Copy(command, 0, toSend, 1, command.Length);
            Array.Copy(PayLoad, 0, toSend, command.Length + 1, PayLoad.Length);
            return toSend;
        }

        private byte[] ExtractResult(byte[] total)
        {
            if (total.Length > 2)
            {
                if (total[0] == STARTCODE && total[total.Length - 1] == ENDCODE)
                {
                    byte[] result = new byte[total.Length - 2];
                    Array.Copy(total, 1, result, 0, result.Length);
                    return result;
                }
            }
            return total;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tcpaddress"></param>
        /// <param name="port"></param>
        /// <param name="useTCP"></param>
        /// <param name="comport"></param>
        /// <param name="baud">9600 please</param>
        public PMR_AR100(string tcpaddress, int port, bool useTCP, string comport, int baud)
            :base(tcpaddress, port, useTCP, comport, baud)
        {
            EndCodeTerm.Add(new byte[] { ENDCODE });
            ValidInvalidTerms.Add(new byte[] { VALID });
            ValidInvalidTerms.Add(new byte[] { INVALID });
        }

        private List<byte[]> EndCodeTerm = new List<byte[]>();
        private List<byte[]> ValidInvalidTerms = new List<byte[]>();

        public override MicrowaveReceiver.OperatingBand GetBand()
        {
            byte[] toSend = BuildCommand(CHECKBAND);
            bool successfulSend = false;
            OperatingBand band = OperatingBand.NotSet;
            byte[] result = BlockingSend(toSend, out successfulSend, EndCodeTerm);
            if (successfulSend && result[0] == STARTCODE && result[result.Length - 1] == ENDCODE && result[1] == (byte)'Q')
            {
                switch (result[2])
                {
                    case (byte)'L': band = OperatingBand.L; break;
                    case (byte)'S': band = OperatingBand.S; break;
                    case (byte)'C': band = OperatingBand.C; break;
                    case (byte)'X': band = OperatingBand.X; break;
                    default: band = OperatingBand.NotSet; break;
                }
                if (band != OperatingBand.NotSet)
                    Connected = true;
            }
            else
            {
                Connected = false;
                PrintDebug("GetBand failed");
            }
            currentBand = band;
            return band;
        }

        protected int currentFreq = -1;
        protected OperatingBand currentBand = OperatingBand.NotSet;

        /// <summary>
        /// fetches the tuned frequency in MHz
        /// </summary>
        /// <returns></returns>
        internal int GetFrequency()
        {
            byte[] ToSend = BuildCommand(CHECKFREQ);
            bool successfulSend = false;
            byte[] result = BlockingSend(ToSend, out successfulSend, EndCodeTerm);
            if (successfulSend)
            {
                byte[] value = ExtractResult(result);
                if (value[0] == 'F')
                {
                    int freq = -1;
                    if (Int32.TryParse(Encoding.ASCII.GetString(value, 1, value.Length - 1), out freq))
                    {
                        Connected = true;
                        currentFreq = freq;
                        return freq;
                    }
                }
            }
            PrintDebug("GetFrequency failed");
            currentFreq = -1;
            Connected = false;
           // return -1;
            throw new Exception("GetFrequency failed");
        }

        internal override string RawCommand(string userinput)
        {
            try
            {
                byte[] cmd = BuildCommand(System.Text.Encoding.ASCII.GetBytes(userinput));
                bool success;
                byte[] result = BlockingSend(cmd, out success, EndCodeTerm);
                if (result != null)
                {
                    return System.Text.Encoding.ASCII.GetString(result);
                }
                else
                {
                    return "no data received.";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public override MicrowaveTuning GetTuning()
        {
            return new MicrowaveTuning() { FrequencyMHz = (UInt64)this.GetFrequency() };
        }

        public override MicrowaveLinkQuality GetLinkQuality()
        {
            byte[] ToSend = BuildCommand(CHECKSIGNAL);
            bool successfulSend = false;
            byte[] result = BlockingSend(ToSend, out successfulSend, EndCodeTerm);
            if (successfulSend)
            {
                byte[] value = ExtractResult(result);
                if (value[0] == 'S')
                {
                    int strength = 0;
                    byte max = '9' + 1;
                    if (value[1] == max)
                        return new MicrowaveLinkQuality(100);
                    //else if (Int32.TryParse(Encoding.ASCII.GetString(value, 1, value.Length - 2), out strength))
                    //    return strength;
                    else
                    {
                        byte[] str = new byte[value.Length - 2];
                        Array.Copy(value, 1, str, 0, str.Length);
                        string temp = Encoding.ASCII.GetString(str);
                        string temp2 = Encoding.ASCII.GetString(value);
                        try
                        {
                            strength = Int32.Parse(temp);
                            Connected = true;
                        }
                        catch (Exception)
                        {
                            Connected = false;
                            PrintDebug("GetSignalStrength failed: Error Parsing! Actual String: " + temp);
                            //Debug.WriteLine(exc.Message + "\r\n" + exc.StackTrace);
                        }
                        return new MicrowaveLinkQuality(strength);
                    }
                }
            }
            PrintDebug("GetSignalStrength failed");
            Connected = false;
            return new MicrowaveLinkQuality(0);
        }

        public bool SetBand(MicrowaveReceiver.OperatingBand band)
        {
            if (band != OperatingBand.NotSet)
            {
                byte[] ToSend = BuildCommand(SETBAND, System.Enum.GetName(typeof(MicrowaveReceiver.OperatingBand), band));
                bool successfulSend = false;
                byte[] result = BlockingSend(ToSend, out successfulSend, ValidInvalidTerms);
                if (successfulSend && result[0] == VALID)
                {
                    bool temp = GetBand() == band;
                    Connected = temp;
                    if (temp)
                    {
                        PrintDebug("SetBand successful");
                        DoBandChangeEvent();
                    }
                    else
                    {
                        PrintDebug("SetBand sent successfully, but failed to set the Band");
                    }
                    return temp;
                }
            }
            PrintDebug("SetBand failed");
            Connected = false;
            return false;
        }

        public override string GetDeviceInfo()
        {
            return "PMR AR100. Not implemented." + Environment.NewLine + "raw command syntax: leave off the $ and #";
        }

        public override MicrowaveCapabilities GetCapabilities()
        {
            MicrowaveCapabilities c = new MicrowaveCapabilities();
            c.MaximumFrequency = 2500000000;
            c.MinimumFrequency = 1700000000;
            c.FrequencyResolution = 1000000;
            c.ReceivedCarrierLevelMaximum = 100;
            c.SupportedBandwidth = new List<UInt64>();
            c.SupportedBandwidth.Add(6000000);
            c.SupportedEncryption = null;
            c.SupportedForwardErrorCorrection = null;
            c.SupportedGuardIntervals = null;
            c.SupportedModulations = null;
            c.SupportedTransports = new List<Media.RFVideoStandard>();
            c.SupportedTransports.Add(RFVideoStandard.NTSC);
            c.SupportedTransports.Add(RFVideoStandard.PAL);

            c.AutoTuning = new MicrowaveTuning();
            c.AutoTuningRequirements = (int)MicrowaveTuning.Parameters.Frequency;
            c.SupportedTuningParameters = (int)MicrowaveTuning.Parameters.Frequency;
            c.SupportedLinkQualityParameters = (int)MicrowaveLinkQuality.Parameters.ReceivedCarrierLevel;

            return c;
        }

        public override void SetTuning(MicrowaveTuning tuning)
        {
            if (tuning.FrequencyMHz < 0 || tuning.FrequencyMHz > 9999) throw new ArgumentOutOfRangeException("frequency", "Frequency must be between 0 and 9999.");

            SetBandForFreq((int)tuning.FrequencyMHz);
            byte[] ToSend = BuildCommand(SETFREQUENCY, tuning.FrequencyMHz.ToString());
            bool successfulSend = false;
            byte[] result = BlockingSend(ToSend, out successfulSend, ValidInvalidTerms);
            if (successfulSend && result[0] == VALID)
            {
                bool temp = GetFrequency() == (int)tuning.FrequencyMHz;
                Connected = temp;
                if (temp)
                {
                    DoTuningChangeEvent();
                }
                else
                {
                    throw new Exception("SetFrequency sent successfully, but failed to set the frequency");
                }
            }
            Connected = false;
            throw new Exception("SetFrequency failed");
        }

        private bool SetBandForFreq(int freq)
        {
            OperatingBand needed = OperatingBand.NotSet;
            if (freq < 2000)
            {
                needed = OperatingBand.L;
            }
            else if (freq < 3000)
            {
                needed = OperatingBand.S;
            }
            else if (freq < 6000)
            {
                needed = OperatingBand.C;
            }
            else
            {
                needed = OperatingBand.X;
            }
            if (currentBand != needed)
            {
                return SetBand(needed);
            }
            return true;
        }

        public override bool TestConnection()
        {
            bool conn = false;
            byte[] toSend = BuildCommand(REMOTEMODE);
            bool successFulSend = false;
            byte[] result = BlockingSend(toSend, out successFulSend, ValidInvalidTerms);
            if (successFulSend && result[0] == VALID)
            {
                currentBand = GetBand();
                conn = true;
            }
            else
            {
                conn = false;
            }
            PrintDebug("Test Connection " + (conn ? "success" : "failed"));
            Connected = conn;
            return conn;
        }
    }
}