using System;
using System.Xml;

namespace FutureConcepts.Media.Client.InBandData
{
    /// <summary>
    /// This class is used to distribute XML to various receivers
    /// </summary>
    public class XmlTelemetryEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="xmlReader">associated xml reader</param>
        public XmlTelemetryEventArgs(XmlReader xmlReader)
        {
            this.XmlReader = xmlReader;
        }

        /// <summary>
        /// Gets the attached <see cref="T:XmlReader"/>
        /// </summary>
        public XmlReader XmlReader { get; private set; }
    }
}
