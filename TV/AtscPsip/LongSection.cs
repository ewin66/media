using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.TV.AtscPsip
{
    /// <summary>
    /// Represents an MPEG2 Long Section
    /// </summary>
    class LongSection : Section
    {
        public LongSection() { }

        /// <summary>
        /// Marshals a LongSection from a pointer.
        /// </summary>
        /// <param name="ptr">pointer to the LongSection structure</param>
        public LongSection(IntPtr ptr)
            : base(ptr)
        {
            if (!this.SectionSyntaxIndicator)
            {
                throw new ArgumentException("The structure at the given pointer is not a LongSection! It is probably just a Section", "ptr");
            }

            int byteOffset = Section.DataByteOffset;

            this.TransportStreamId = (ushort)Marshal.ReadInt16(ptr, byteOffset);
            byteOffset += 2;

            byte mpegHeaderVersionBits = Marshal.ReadByte(ptr, byteOffset);
            byteOffset++;

            //2 bits, reserved

            this.VersionNumber = (byte)((mpegHeaderVersionBits >> 1) & 0x1F);

            this.CurrentNextIndicator = (mpegHeaderVersionBits & 0x01) == 0x01;

            this.SectionNumber = Marshal.ReadByte(ptr, byteOffset);
            byteOffset++;

            this.LastSectionNumber = Marshal.ReadByte(ptr, byteOffset);
            byteOffset++;


            this.Data = new byte[this.SectionLength - 5];
            for (int i = 0; i < this.Data.Length; i++, byteOffset++)
            {
                this.Data[i] = Marshal.ReadByte(ptr, byteOffset);
            }
        }

        /// <summary>
        /// Copies the TableId and HeaderW fields, but does not copy the Data array
        /// </summary>
        /// <param name="s">simple Section to copy from</param>
        public LongSection(Section s)
        {
            this.TableId = s.TableId;
        }

        /// <summary>
        /// Transport Stream Identifier. Unique between instances of the same program
        /// (e.g. same video being broadcast from diffent towers, under the same virtual channel)
        /// </summary>
        /// <remarks>16 bits</remarks>
        public UInt16 TransportStreamId { get; set; }

        //2 bits, reserved

        /// <summary>
        /// The version number of this table/section
        /// </summary>
        /// <remarks>5 bits</remarks>
        public byte VersionNumber { get; set; }

        /// <summary>
        /// If true, then this is the current table. If False, then this table *will* become applicable
        /// </summary>
        /// <remarks>1 bit</remarks>
        public bool CurrentNextIndicator { get; set; }

        /// <summary>
        /// Section number between all sections for this table. Starts with 0
        /// </summary>
        /// <remarks>8 bits</remarks>
        public byte SectionNumber { get; set; }

        /// <summary>
        /// The section with the highest number for this table.
        /// </summary>
        /// <remarks>8 bits</remarks>
        public byte LastSectionNumber { get; set; }

        public new static int DataByteOffset
        {
            get
            {
                return 8;
            }
        }
    }
}
