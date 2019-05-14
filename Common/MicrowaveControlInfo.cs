using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// This class describes the information neccesary to utilize a Microwave Reciever
    /// kdixon 02/05/2009
    /// </summary>
    [Serializable]
    [DataContract]
    public class MicrowaveControlInfo
    {
        /// <summary>
        /// Creates a blank MicrowaveControlInfo
        /// </summary>
        public MicrowaveControlInfo() { }

        /// <summary>
        /// Clones a MicrowaveControlInfo
        /// </summary>
        /// <param name="clone">object to clone</param>
        public MicrowaveControlInfo(MicrowaveControlInfo clone)
        {
            this.Address = clone.Address;
            this.ReceiverType = clone.ReceiverType;
            this.RelinquishTimer = clone.RelinquishTimer;
            this.BlockDownConverterFrequency = clone.BlockDownConverterFrequency;
        }

        private MicrowaveReceiverType _receiver;
        /// <summary>
        /// The type of Microwave Receiver it is
        /// </summary>
        [XmlElement]
        [DefaultValue(MicrowaveReceiverType.PMR_AR100)]
        [DataMember(Name = "_receiver")]
        public MicrowaveReceiverType ReceiverType
        {
            get
            {
                return _receiver;
            }
            set
            {
                _receiver = value;
            }
        }

        private string _address;
        /// <summary>
        /// The address to be used to connect to the microwave receiver.
        /// If in the format hostOrIP:portNumber, then the addres points to a networked device.
        /// If in the format nameOfComPort@baudRate, then the address points to a local serial port.
        /// </summary>
        [XmlElement]
        [DataMember(Name = "_address")]
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        private int _relinquishTimer = 30000;
        /// <summary>
        /// If this amount of time elapses between commands, the microwave control will be released.
        /// </summary>
        [XmlElement, DefaultValue(30000)]
        [DataMember(Name = "_relinquishTimer")]
        public int RelinquishTimer
        {
            get
            {
                return _relinquishTimer;
            }
            set
            {
                _relinquishTimer = value;
            }
        }

        private UInt64 _bdcFreq = 0;
        /// <summary>
        /// The oscillator frequency of the block down converter, in Hz.
        /// 0 if not used
        /// </summary>
        [XmlElement]
        [DefaultValue(0)]
        [DataMember]
        public UInt64 BlockDownConverterFrequency
        {
            get
            {
                return _bdcFreq;
            }
            set
            {
                _bdcFreq = value;
            }
        }



        #region Convience Properties (non-serialized)

        /// <summary>
        /// True if Address points to a networked device.
        /// False if Address points to a serial device.
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool UsesTCP
        {
            get
            {
                return (Address != null) ? Address.Contains(":") : false;
            }
        }

        /// <summary>
        /// Gets the address of the Serial/COM port to use. Returns string.Empty if configured for Network addressing.
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public string ComPort
        {
            get
            {
                if (!string.IsNullOrEmpty(Address))
                {
                    if (!Address.Contains(":"))
                    {
                        string[] s = Address.Split('@');
                        if (s.Length >= 1)
                        {
                            return s[0];
                        }
                    }
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TCP address to connect to for network mode.
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public string TCPAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(Address))
                {
                    if (!Address.Contains("@"))
                    {
                        string[] s = Address.Split(':');
                        if (s.Length >= 1)
                        {
                            return s[0];
                        }
                    }
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the TCP port number to connect to for network mode.
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public int TCPPort
        {
            get
            {
                if (!string.IsNullOrEmpty(Address))
                {
                    string[] s = Address.Split(':');
                    if (s.Length == 2)
                    {
                        int port;
                        if (Int32.TryParse(s[1], out port))
                        {
                            return port;
                        }
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets the Baud Rate to use when in serial/direct mode.
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public int BaudRate
        {
            get
            {
                if (!string.IsNullOrEmpty(Address))
                {
                    string[] s = Address.Split('@');
                    if (s.Length == 2)
                    {
                        int baud;
                        if (Int32.TryParse(s[1], out baud))
                        {
                            return baud;
                        }
                    }
                }

                return -1;
            }
        }

        #endregion
    }
}
