using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using FutureConcepts.Media;

namespace FutureConcepts.Media.Client.InBandData
{
    /// <summary>
    /// Performs parsing of various types of data that is transmitted in-band
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Read a Profile struct from the XML Reader
        /// </summary>
        /// <param name="xml">XmlReader to parse from</param>
        /// <returns>A Profile struct</returns>
        /// <exception cref="System.Exception">Thrown if an error is encountered parsing Profile. See InnerException for more detail.</exception>
        public static Profile ReadProfile(XmlReader xml)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Profile));
                return (Profile)xmlSerializer.Deserialize(xml);
            }
            catch (Exception exc)
            {
                throw new Exception("Could not parse Profile from supplied XML!", exc);
            }
        }

        /// <summary>
        /// Parses a TV Channel from the XML Reader
        /// </summary>
        /// <param name="xml">XmlReader to parse from</param>
        /// <returns>the TV.Channel struct</returns>
        /// <exception cref="System.Exception">Thrown if an error is encountered parsing the TV Channel. See InnerException for more detail.</exception>
        public static TV.Channel ReadTVChannel(XmlReader xml)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(TV.Channel));
                return (TV.Channel)xmlSerializer.Deserialize(xml);
            }
            catch (Exception exc)
            {
                throw new Exception("Could not parse TV.Channel from supplied XML!", exc);
            }
        }

        /// <summary>
        /// Parses a client list
        /// </summary>
        /// <param name="xml">XmlReader to read from</param>
        /// <returns>a list of usernames representing the clients</returns>
        public static List<string> ReadClientList(XmlReader xml)
        {
            List<string> users = new List<string>();
            while (xml.Read())
            {
                if (xml.Name.Equals("add"))
                {
                    string userName = xml.GetAttribute("UserName");
                    users.Add(userName);
                }
            }
            return users;
        }
    }
}
