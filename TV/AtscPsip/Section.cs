using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.TV.AtscPsip
{
    /// <summary>
    /// Represents an MPEG2 Section
    /// </summary>
    class Section
    {
        public Section() { }

        /// <summary>
        /// Marshals in a Section struct
        /// </summary>
        /// <param name="ptr">pointer to the Section struct</param>
        public Section(IntPtr ptr)
        {
            int byteOffset = 0;

            this.TableId = Marshal.ReadByte(ptr, byteOffset);
            byteOffset += 1;

            ushort word = (ushort)Marshal.ReadInt16(ptr, byteOffset);
            byteOffset += 2;

            this.SectionSyntaxIndicator = (((word >> 15) & 0x01) == 0x01);

            this.PrivateIndicator = (((word >> 14) & 0x01) == 0x01);

            this.SectionLength = (ushort)(word & 0x0FFF);


            this.Data = new byte[this.SectionLength];
            for (int i = 0; i < this.Data.Length; i++, byteOffset++)
            {
                this.Data[i] = Marshal.ReadByte(ptr, byteOffset);
            }
        }

        /// <summary>
        /// Identifier for this table
        /// </summary>
        public byte TableId { get; set;}

        /// <summary>
        /// If this is true, then this probably represents a LongSection
        /// </summary>
        public bool SectionSyntaxIndicator { get; set; }

        public bool PrivateIndicator { get; set; }

        /// <summary>
        /// the number of bytes following this field.
        /// </summary>
        public UInt16 SectionLength { get; set; }

        public byte[] Data { get; set; }

        /// <summary>
        /// The total ByteOffset for the fields before the data array
        /// </summary>
        public static int DataByteOffset
        {
            get
            {
                return 3;
            }
        }
    }
}
