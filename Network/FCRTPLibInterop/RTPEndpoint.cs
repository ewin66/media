using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.Network.FCRTPLib
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size=16)]
    public struct RTPEndpoint
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.Bool)]
        public bool isMulticast;

        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.Bool)]
        public bool isIPv6;

        [FieldOffset(8)]
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dataPort;

        [FieldOffset(10)]
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 controlPort;

        [FieldOffset(12)]
        [MarshalAs(UnmanagedType.LPStr)]
        public string ipAddress;
    }
}
