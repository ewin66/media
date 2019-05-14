using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Network.FCRTPLib
{
    [Serializable]
    public class RTPStreamDescription
    {
        public RTPStreamDescription()
        {
        }

        public RTPStreamDescription(FCRTPStreamDescriptionStruct raw)
        {
            this.Endpoint = raw.server;
            this.PayloadType = raw.payloadType;

            this.MajorType = raw.dsType.majorType;
            this.SubType = raw.dsType.subType;
            this.FixedSizeSamples = raw.dsType.fixedSizeSamples;
            this.TemporalCompression = raw.dsType.temporalCompression;
            this.SampleSize = raw.dsType.sampleSize;
            this.FormatType = raw.dsType.formatType;
            this.FormatSize = raw.dsType.formatSize;

            Type f = DirectShowLib.FormatType.GetBackingType(this.FormatType);

            object data = Marshal.PtrToStructure(raw.dsType.formatPtr, f);

            if(f == typeof(DVInfo))
            {
                DVInfo = (DVInfo)data;
            }
            else if (f == typeof(MPEG2VideoInfo))
            {
                MPEG2Video = (MPEG2VideoInfo)data;
            }
            else if (f == typeof(MPEG1VideoInfo))
            {
                MPEGVideo = (MPEG1VideoInfo)data;
            }
            else if (f == typeof(VideoInfoHeader))
            {
                VideoInfo = (VideoInfoHeader)data;
            }
            else if (f == typeof(VideoInfoHeader2))
            {
                VideoInfo2 = (VideoInfoHeader2)data;
            }
            else if (f == typeof(WaveFormatEx))
            {
                WaveFormatEx = (WaveFormatEx)data;
            }
            else if (f == typeof(AnalogVideoInfo))
            {
                AnalogVideo = (AnalogVideoInfo)data;
            }
        }

        public FCRTPStreamDescriptionStruct ToStruct()
        {
            FCRTPStreamDescriptionStruct raw = new FCRTPStreamDescriptionStruct();

            raw.server = this.Endpoint;
            raw.payloadType = this.PayloadType;

            raw.dsType = new AMMediaType();
            raw.dsType.majorType = this.MajorType;
            raw.dsType.subType = this.SubType;
            raw.dsType.fixedSizeSamples = this.FixedSizeSamples;
            raw.dsType.temporalCompression = this.TemporalCompression;
            raw.dsType.sampleSize = this.SampleSize;
            raw.dsType.formatType = this.FormatType;
            raw.dsType.formatSize = this.FormatSize;

            Type f = DirectShowLib.FormatType.GetBackingType(this.FormatType);
            object data = null;
            if (f == typeof(DVInfo))
            {
                data = DVInfo;
            }
            else if (f == typeof(MPEG2VideoInfo))
            {
                data = MPEG2Video;
            }
            else if (f == typeof(MPEG1VideoInfo))
            {
                data = MPEGVideo;
            }
            else if (f == typeof(VideoInfoHeader))
            {
                data = VideoInfo;
            }
            else if (f == typeof(VideoInfoHeader2))
            {
                data = VideoInfo2;
            }
            else if (f == typeof(WaveFormatEx))
            {
                data = WaveFormatEx;
            }
            else if (f == typeof(AnalogVideoInfo))
            {
                data = AnalogVideo;
            }

            if ((f != null) && (data != null))
            {
                raw.dsType.formatPtr = Marshal.AllocCoTaskMem(this.FormatSize);
                Marshal.StructureToPtr(data, raw.dsType.formatPtr, false);
            }

            return raw;
        }

        public RTPEndpoint Endpoint { get; set; }

        public byte PayloadType { get; set; }

        #region DirectShow AM Media Type

        public Guid MajorType { get; set; }

        public Guid SubType { get; set; }

        public bool FixedSizeSamples { get; set; }

        public bool TemporalCompression { get; set; }

        public int SampleSize { get; set; }

        public Guid FormatType { get; set; }

        public int FormatSize { get; set; }

        #region Format Pointer, Dereferenced

        public DVInfo DVInfo { get; set; }

        public MPEG2VideoInfo MPEG2Video { get; set; }

        public MPEG1VideoInfo MPEGVideo { get; set; }

        public VideoInfoHeader VideoInfo { get; set; }

        public VideoInfoHeader2 VideoInfo2 { get; set; }

        public WaveFormatEx WaveFormatEx { get; set; }

        public AnalogVideoInfo AnalogVideo { get; set; }

        public byte[] ExtensionData { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// FCRTPStreamDescription, as needed to Marshal from COM
    /// </summary>
    /// <author>kdixon 02/19/2010</author>
    [StructLayout(LayoutKind.Explicit)]
    public struct FCRTPStreamDescriptionStruct
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.U1)]
        public byte payloadType;

        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.Struct)]
        public AMMediaType dsType;

        [FieldOffset(76)]
        [MarshalAs(UnmanagedType.Struct)]
        public RTPEndpoint server;
    }
}
