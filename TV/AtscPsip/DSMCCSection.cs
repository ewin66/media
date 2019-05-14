using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.TV.AtscPsip
{
    /// <summary>
    /// Represents an MPEG2 Digital Storage Media Command and Control section
    /// </summary>
    /// <remarks>
    /// Not much used I guess
    /// Kevin Dixon
    /// </remarks>
    class DSMCCSection : LongSection
    {
        public DSMCCSection() { }

        /// <summary>
        /// Marshals in a DSM-CC Section
        /// </summary>
        /// <param name="ptr">pointer to the DSMCCSection struct</param>
        public DSMCCSection(IntPtr ptr)
            : base(ptr)
        {
            int byteOffset = LongSection.DataByteOffset;

            this.ProtocolDiscriminator = Marshal.ReadByte(ptr, byteOffset++);

            this.DsmccType = Marshal.ReadByte(ptr, byteOffset++);

            this.MessageId = (ushort)Marshal.ReadInt16(ptr, byteOffset);
            byteOffset += 2;

            this.TransactionId = (ushort)Marshal.ReadInt16(ptr, byteOffset);
            byteOffset += 2;

            this.Reserved = Marshal.ReadByte(ptr, byteOffset++);

            this.AdaptationLength = Marshal.ReadByte(ptr, byteOffset++);

            this.MessageLength = (ushort)Marshal.ReadInt16(ptr, byteOffset);
            byteOffset += 2;


            this.Data = new byte[this.MessageLength + this.AdaptationLength];
            for (int i = 0; i < this.Data.Length; i++, byteOffset++)
            {
                this.Data[i] = Marshal.ReadByte(ptr, byteOffset);
            }
        }

        public byte ProtocolDiscriminator { get; set; }

        public byte DsmccType { get; set; }

        public ushort MessageId { get; set; }

        public ushort TransactionId { get; set; }

        public byte Reserved { get; set; }

        public byte AdaptationLength { get; set; }

        public ushort MessageLength { get; set; }

        public new static int DataByteOffset
        {
            get
            {
                return 18;
            }
        }
    }
}
