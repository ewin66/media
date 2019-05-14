using System;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.Network.FCRTPLib
{
    [ComImport]
    [Guid("D76B81EA-746A-41e4-A5FC-2754169E39DA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IFCNSource
    {
        /// <summary>
        /// Once you have received the stream metadata from the server, call this method to
        /// create the connections and pins on the source filter
        /// </summary>
        /// <param name="description">description of the stream</param>
        /// <param name="index">index of the output pin assigned to this stream</param>
        /// <returns></returns>
        [PreserveSig]
	    int AddStream([In] FCRTPStreamDescriptionStruct description, [Out] out int index);

        /// <summary>
        /// Remove a particular stream from the source
        /// </summary>
        /// <param name="index">index of the pin/stream to remove, or -1 to remove all</param>
        /// <returns></returns>
        [PreserveSig]
	    int RemoveStream([In] int index);
    }
}
