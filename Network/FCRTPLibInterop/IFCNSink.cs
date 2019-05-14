using System;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.Network.FCRTPLib
{
    [ComImport]
    [Guid("75C81197-A4DF-46e2-8F17-225EF9CA8AA0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFCNSink
    {
        /// <summary>
        /// Returns the count of connected streams
        /// </summary>
        /// <param name="count">the count</param>
        /// <returns>S_OK</returns>
        [PreserveSig]
        int GetStreamCount([Out] out int count);

        /// <summary>
        /// Sets the port allocation
        /// </summary>
        /// <param name="index">index of stream to set port allocation for</param>
        /// <param name="data">RTP data port</param>
        /// <param name="control">RTCP control port, may supply "0" to auto-select</param>
        /// <returns>S_OK on success</returns>
        [PreserveSig]
        int SetPortAllocation([In] int index, [In, MarshalAs(UnmanagedType.U2)] UInt16 data, [In, MarshalAs(UnmanagedType.U2)] UInt16 control);

        /// <summary>
        /// Gets the actually allocated ports. Returns values of 0 until the RTP session has been established
        /// </summary>
        /// <param name="index">index of stream to get port allocation for</param>
        /// <param name="data">RTP data port</param>
        /// <param name="control">RTCP control port</param>
        /// <returns>S_OK on success, S_FALSE if data is invalid</returns>
        [PreserveSig]
        int GetPortAllocation([In] int index, [Out, MarshalAs(UnmanagedType.U2)] out UInt16 data, [Out, MarshalAs(UnmanagedType.U2)] out UInt16 control);

        /// <summary>
        /// After connecting up the Sink, call this method in order to get all of the description info
        /// </summary>
        /// <param name="index">starting from 0, increment to enumerate the pins.</param>
        /// <param name="description">a description of the connection.</param>
        /// <returns>
        /// S_OK				success
        /// S_FALSE				not connected
        /// VFW_S_NO_MORE_ITEMS	index out of range
        /// </returns>
        [PreserveSig]
        int GetStream([In] int index, [Out] out FCRTPStreamDescriptionStruct description);

        /// <summary>
        /// Adds a destination to a particular stream, or all streams
        /// </summary>
        /// <remarks>
        /// If you receive E_FAIL when adding to index == -1, you should RemoveDestination from all before trying
        /// again, because it is possible that some additions succeeded before failing
        /// </remarks>
        /// <param name="index">index of stream to add destination to, or -1 to add to all</param>
        /// <param name="destination">destination endpoint to add</param>
        /// <returns>returns S_OK if add destination succeeded.
        /// returns E_FAIL if add destination failed.</returns>
        [PreserveSig]
        int AddDestination([In] int index, [In, MarshalAs(UnmanagedType.LPStruct)] RTPEndpoint destination);

        /// <summary>
        /// Removes a destination from a stream, or from all streams
        /// </summary>
        /// <param name="index">index of stream to add destination to, or -1 to add to all</param>
        /// <param name="destination">destination endpoint to remove</param>
        /// <returns>
        /// returns S_OK if removed from destination, or if index is -1.
        /// returns S_FALSE if the endpoint specified was not a destination for the specified index
        /// </returns>
        [PreserveSig]
        int RemoveDestination([In] int index, [In, MarshalAs(UnmanagedType.LPStruct)] RTPEndpoint destination);
    }
}
