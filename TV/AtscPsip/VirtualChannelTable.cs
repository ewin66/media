using System;
using System.Collections.Generic;
using System.Text;

namespace FutureConcepts.Media.TV.AtscPsip
{
    /// <summary>
    /// Defines a ATSC Virtual Channel Table
    /// </summary>
    public class VirtualChannelTable
    {
        /// <summary>
        /// Creates a Virtual Channel Table from all of its sections
        /// </summary>
        /// <param name="sections">All of the Sections that make up the table</param>
        internal VirtualChannelTable(List<Section> sections)
        {
            Items = new List<Entry>();

            foreach (Section s in sections)
            {
                AddSection(s as LongSection);
            }
        }

        /// <summary>
        /// Adds the data contained in a given section to the virtual channel table.
        /// </summary>
        /// <param name="s">a Virtual Channel Table section</param>
        private void AddSection(LongSection s)
        {
            if (s == null)
            {
                return;
            }

            byte protocol_version = s.Data[0];
            byte num_channels_in_section = s.Data[1];

            for (int i = 2, channel = 0; (i < s.Data.Length) && (channel < num_channels_in_section); channel++)
            {
                Entry cur = new VirtualChannelTable.Entry();
                int bytesUsed;
                cur.ParseFromArray(s.Data, i, out bytesUsed);
                i += bytesUsed;
                this.Items.Add(cur);
            }
        }

        /// <summary>
        /// The type of table this is.
        /// </summary>
        public TableID Type { get; internal set; }

        /// <summary>
        /// The Virtual Channel Table entries
        /// </summary>
        public List<Entry> Items { get; private set; }

        /// <summary>
        /// Represents a Virtual Channel Table Entry
        /// </summary>
        public class Entry : Channel
        {
            #region Methods

            /// <summary>
            /// Populates the structure by parsing data from the specified array.
            /// </summary>
            /// <param name="array">array to parse from</param>
            /// <param name="start">starting index</param>
            /// <param name="usedBytes">the number of bytes parsed for this structure</param>
            public void ParseFromArray(byte[] array, int start, out int usedBytes)
            {
                //the current index in array
                int i = start;

                try
                {
                    this.ShortName = Encoding.BigEndianUnicode.GetString(array, i, 14);
                    i += 14;

                    this.MajorChannel = (int)(((array[i] & 0x0F) << 6) + (array[i + 1] >> 2));
                    this.MinorChannel = (int)(((array[i + 1] & 0x03) << 8) + array[i + 2]);
                    i += 3;

                    this.ModulationMode = (VCTModulationModes)array[i];
                    i += 1;

                    this.CarrierFrequency = (int)Parser.MakeUInt32(array, i);
                    i += 4;

                    this.ChannelTSID = Parser.MakeUInt16(array, i);
                    i += 2;

                    this.ProgramNumber = Parser.MakeUInt16(array, i);
                    i += 2;

                    this.ETMLocation = (VCTETMLocations)((array[i] >> 6) & 0x03);

                    this.AccessControlled = ((array[i] >> 5) & 0x01) == 0x01;

                    this.Hidden = ((array[i] >> 4) & 0x01) == 0x01;

                    this.HideGuide = ((array[i] >> 1) & 0x01) == 0x01;
                    i += 1;

                    this.ServiceType = (VCTServiceTypes)(array[i] & 0x3F);
                    i += 1;

                    this.SourceId = Parser.MakeUInt16(array, i);
                    i += 2;

                    this.DescriptorLength = (UInt16)(Parser.MakeUInt16(array, i) & 0x03FF);
                    i += 2;

                    i += this.DescriptorLength;  //skip Descriptors, since we don't know how to parse them
                }
                catch
                {
                }
                finally
                {
                    usedBytes = i - start;
                }
            }

            #endregion

            /// <remarks>7 * 16 bits</remarks>
            private string _shortname;

            /// <summary>
            /// Channel's short name. 7 characters of Big-Endian UTF-16.
            /// </summary>
            /// <remarks>The set method replaces null characters with string.Empty</remarks>
            public string ShortName
            {
                get
                {
                    return _shortname;
                }
                set
                {
                    _shortname = value.Replace("\0", string.Empty);
                }
            }

            // reserved 1: 4 bits == 1111

            //MajorChannelNumber 10 bits

            //MinorChannelNumber 10 bits

            /// <summary>
            /// Modulation mode
            /// </summary>
            /// <remarks>8 bits</remarks>
            public VCTModulationModes ModulationMode { get; set; }

            /*
             * Carrier Frequency. Deprecated. After January 2010, this will always be 0.
             * 32 bits
             */

            /// <summary>
            /// Channel Transport Stream ID. From 0x0000 to 0xFFFF. 
            /// </summary>
            /// <remarks>16 bits</remarks>
            public UInt16 ChannelTSID { get; set; }

            /// <summary>
            /// Program number. Relates this entry to the PAT and Program Map tables
            /// </summary>
            /// <remarks>16 bits</remarks>
            public UInt16 ProgramNumber { get; set; }

            /// <summary>
            /// Extended Text Mesasge Location. Tells where the Extended Text Message can be found.
            /// </summary>
            /// <remarks>2 bits</remarks>
            public VCTETMLocations ETMLocation { get; set; }

            /// <summary>
            /// True if events associated with this virtual channel are access controlled
            /// </summary>
            /// <remarks>1 bit</remarks>
            public bool AccessControlled { get; set; }

            /// <summary>
            /// If true, this channel should not be accessable.
            /// </summary>
            /// <remarks>1 bit</remarks>
            public bool Hidden { get; set; }

            // reserved 2: 2 bits == 11

            /// <summary>
            /// If true, then this channel should not appear in Guide data. If false, then it can be shown.
            /// If the channel has Hidden = false, then this field should be ignored.
            /// </summary>
            /// <remarks>1 bit</remarks>
            public bool HideGuide { get; set; }

            //reserved 3: 3 bits == 111

            /// <summary>
            /// Indicates what type of programming is on this virtual channel.
            /// </summary>
            /// <remarks>6 bits</remarks>
            public VCTServiceTypes ServiceType { get; set; }

            /// <summary>
            /// Identifies the programming source associated with the virtual channel.
            /// 0x0001 to 0x0FFF shall be unique within the transport stream carrying the VCT.
            /// 0x1000 and up are assigned by the regional ATSC authority.
            /// </summary>
            /// <remarks>16 bits</remarks>
            public UInt16 SourceId { get; set; }

            // reserved 4: 6 bits == 111111

            /// <summary>
            /// Length in bytes of the following descriptor(s)
            /// </summary>
            /// <remarks>10 bits</remarks>
            public UInt16 DescriptorLength { get; set; }

            //TODO There are "DescriptorLength" bytes here, but so far we do not support Descriptors

            /// <summary>
            /// Returns the base class representation of this Channel
            /// </summary>
            public Channel ToChannel()
            {
                Channel c = new Channel(this.CarrierFrequency, this.PhysicalChannel, this.MajorChannel, this.MinorChannel);
                c.Callsign = this.ShortName;
                return c;
            }
        }
    }
}
