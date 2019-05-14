using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media
{
    /// <summary>
    /// This class is used to address a device's inputs
    /// </summary>
    [Serializable]
    public class DeviceAddress
    {
        private int _channel;
        /// <summary>
        /// Usage is defined by context, but generally you may use this to address a particular channel, DSP, or other logical block of inputs.
        /// </summary>
        [XmlAttribute, DefaultValue(Int32.MinValue)]
        public int Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value;
            }
        }

        private int _input;
        /// <summary>
        /// Usage is defined by context, but generally indicates a specific input on a "<see cref="P:Channel"/>".
        /// </summary>
        [XmlAttribute, DefaultValue(Int32.MinValue)]
        public int Input
        {
            get
            {
                return _input;
            }
            set
            {
                _input = value;
            }
        }
    }
}
