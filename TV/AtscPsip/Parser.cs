using System;
using System.Collections.Generic;

using FutureConcepts.Media.DirectShowLib;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.TV.AtscPsip
{
    /// <summary>
    /// Performs parsing of ATSC PSIP data
    /// </summary>
    public class Parser
    {
        private IMpeg2Data mpeg2Data;

        private static readonly int Timeout = 5000;

        /// <summary>
        /// We can't operate without the things we require
        /// </summary>
        private Parser()
        {
        }

        /// <summary>
        /// Constructs the AtscPsip Parser
        /// </summary>
        /// <remarks>
        /// The associated DirectShow graph should probably be running to get valid data.
        /// </remarks>
        /// <param name="mpeg2DataInterface">the IMpeg2Data interface to the MPEG-2 Sections and Tables filter</param>
        public Parser(IMpeg2Data mpeg2DataInterface)
        {
            if (mpeg2DataInterface == null)
            {
                throw new ArgumentNullException("mpeg2DataInterface");
            }

            mpeg2Data = mpeg2DataInterface;
        }

        /// <summary>
        /// Returns a section list for the entire specified table. Caller must dispose the resulting ISectionList
        /// </summary>
        /// <param name="PID">program ID</param>
        /// <param name="TID">table ID</param>
        /// <returns>returns the section list neccesary to read the whole table</returns>
        private ISectionList GetTableSectionList(short PID, byte TID)
        {
            ISectionList sectionList = null;
            int hr = mpeg2Data.GetTable(PID, TID, null, Parser.Timeout, out sectionList);
            DsError.ThrowExceptionForHR(hr);

            return sectionList;
        }

        /// <summary>
        /// Retreives all of the sections for the specified table.
        /// </summary>
        /// <param name="PID">packet ID</param>
        /// <param name="TID">table ID</param>
        /// <returns>a list of Sections. Or an empty list if an error occurs</returns>
        private List<Section> GetAllSections(short PID, byte TID)
        {
            lock (mpeg2Data)
            {
                ISectionList sectionList = null;
                List<Section> retreivedSections = new List<Section>();
                try
                {
                    sectionList = GetTableSectionList(PID, TID);
                    short sectionCount;
                    sectionList.GetNumberOfSections(out sectionCount);

                    for (short i = 0; i < sectionCount; i++)
                    {
                        try
                        {
                            IntPtr pSection;
                            int sectionSize;
                            int hr = sectionList.GetSectionData(i, out sectionSize, out pSection);
                            DsError.ThrowExceptionForHR(hr);

                            Section section = new Section(pSection);
                            if (section.SectionSyntaxIndicator)
                            {
                                section = new LongSection(pSection);
                            }

                            retreivedSections.Add(section);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failed to retreive section:");
                            ErrorLogger.DumpToDebug(ex);
                        }
                    }

                    return retreivedSections;
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                    return retreivedSections;
                }
                finally
                {
                    if (sectionList != null)
                    {
                        Marshal.ReleaseComObject(sectionList);
                    }
                }
            }
        }

        /// <summary>
        /// Retreives the Virtual Channel Table for the current transport stream
        /// </summary>
        /// <returns>a VirtualChannelTable</returns>
        public VirtualChannelTable GetVirtualChannelTable()
        {
            List<Section> sections = GetAllSections((short)PacketID.PSIP, (byte)TableID.VCTTerrestrial);
            VirtualChannelTable vct = new VirtualChannelTable(sections);
            vct.Type = TableID.VCTTerrestrial;
            return vct;
        }

        /// <summary>
        /// Prints a byte array to the Debug console...obviously for debug purposes
        /// </summary>
        private void PrintByteArrayToDebug(byte[] array)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                str.AppendFormat(array[i].ToString("X") + " ");
                if ((i % 8) == 7)
                {
                    Debug.WriteLine(str.ToString());
                    str = new StringBuilder();
                }
            }
            Debug.WriteLine(str.ToString());
        }

        /// <summary>
        /// Uses 4 bytes to make a 32 bit unsigned integer
        /// </summary>
        /// <param name="array">array to read from</param>
        /// <param name="i">beginning index</param>
        /// <returns>the UInt32 representation</returns>
        internal static UInt32 MakeUInt32(byte[] array, int i)
        {
            return (UInt32)((array[i] << 24) + (array[i + 1] << 16) + (array[i + 2] << 8) + array[i + 3]);
        }

        /// <summary>
        /// Uses 2 bytes to make a 16 bit unsigned integer
        /// </summary>
        /// <param name="array">array to read from</param>
        /// <param name="i">beginning index</param>
        /// <returns>the UInt16 representation</returns>
        internal static UInt16 MakeUInt16(byte[] array, int i)
        {
            return (UInt16)((array[i] << 8) + array[i + 1]);
        }
    }
}
