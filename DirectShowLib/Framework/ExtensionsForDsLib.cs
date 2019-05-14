using System;
using System.Collections.Generic;
using FutureConcepts.Tools;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FutureConcepts.Media.DirectShowLib.Framework;

namespace FutureConcepts.Media.DirectShowLib
{
    /// <summary>
    /// Supplies extension methods that make using DirectShowLib more awesome
    /// </summary>
    /// <author>kdixon 12/09/2009</author>
    public static class ExtensionsForDsLib
    {
        /// <summary>
        /// Gets the filter that owns the specified pin.
        /// </summary>
        /// <param name="pin">pin to get owner of</param>
        /// <returns>Caller is responsible for freeing this filter</returns>
        public static IBaseFilter GetFilter(this IPin pin)
        {
            PinInfo info = default(PinInfo);
            int hr = pin.QueryPinInfo(out info);
            DsError.ThrowExceptionForHR(hr);
            return info.filter;
        }

        /// <summary>
        /// Gets the width and height of a specified media type descriptor. Returns true if successful  
        /// </summary>
        /// <param name="ds">AMMediaType to inspect</param>
        /// <param name="width">width in pixels</param>
        /// <param name="height">height in pixels</param>
        /// <returns>false if the size was not parsed</returns>
        public static bool GetVideoWidthHeight(this AMMediaType ds, out int width, out int height)
        {
            if (ds.formatType == FormatType.VideoInfo)
            {
                VideoInfoHeader vih = new VideoInfoHeader();
                Marshal.PtrToStructure(ds.formatPtr, vih);
                width = vih.BmiHeader.Width;
                height = vih.BmiHeader.Height;
            }
            else if (ds.formatType == FormatType.VideoInfo2)
            {
                VideoInfoHeader2 vih = new VideoInfoHeader2();
                Marshal.PtrToStructure(ds.formatPtr, vih);
                width = vih.BmiHeader.Width;
                height = vih.BmiHeader.Height;
            }
            else if (ds.formatType == FormatType.AnalogVideo)
            {
                AnalogVideoInfo avi = new AnalogVideoInfo();
                Marshal.PtrToStructure(ds.formatPtr, avi);
                width = avi.dwActiveWidth;
                height = avi.dwActiveHeight;
            }
            else if (ds.formatType == FormatType.Mpeg2Video)
            {
                MPEG2VideoInfo mvi = new MPEG2VideoInfo();
                Marshal.PtrToStructure(ds.formatPtr, mvi);
                width = mvi.hdr.BmiHeader.Width;
                height = mvi.hdr.BmiHeader.Height;
            }
            else if (ds.formatType == FormatType.MpegVideo)
            {
                MPEG1VideoInfo mvi = new MPEG1VideoInfo();
                Marshal.PtrToStructure(ds.formatPtr, mvi);
                width = mvi.hdr.BmiHeader.Width;
                height = mvi.hdr.BmiHeader.Height;
            }
            else
            {
                Debug.WriteLine("Unrecognized format type! " + ds.formatType);
                width = 352;
                height = 240;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retreives detailed information about all pins on a given filter.
        /// Caller must release all returned data. See IEnumerable&lt;DetailPinInfo&gt;.Release()
        /// </summary>
        /// <remarks>
        /// If an exception occurs, it frees all objects that may already be enumerated
        /// </remarks>
        /// <param name="filter">filter to get information about</param>
        public static List<DetailPinInfo> EnumPinsDetails(this IBaseFilter filter)
        {
            List<DetailPinInfo> info = new List<DetailPinInfo>();

            int hr;

            IEnumPins enumPins;
            IEnumMediaTypes enumMediaTypes = null;
            IPin[] curPin = new IPin[1];
            PinInfo curPinInfo = new PinInfo();
            AMMediaType[] curMediaType = new AMMediaType[1];

            hr = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(hr);
            try
            {
                IntPtr fetched = Marshal.AllocCoTaskMem(4);
                try
                {
                    while (enumPins.Next(curPin.Length, curPin, fetched) == 0)
                    {
                        if (Marshal.ReadInt32(fetched) == 1)
                        {
                            info.Add(new DetailPinInfo(curPin[0]));
                        }
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(fetched);
                }

                return info;
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                try
                {
                    BaseDSGraph.Release(curPin[0]);
                    if (curMediaType[0] != null)
                    {
                        DsUtils.FreeAMMediaType(curMediaType[0]);
                    }
                    DsUtils.FreePinInfo(curPinInfo);
                    info.Release();
                }
                catch (Exception cleanUpEx)
                {
                    ErrorLogger.DumpToDebug(cleanUpEx);
                }
                throw;
            }
            finally
            {
                BaseDSGraph.Release(enumPins);
                BaseDSGraph.Release(enumMediaTypes);
            }
        }

        #region Release Methods

        /// <summary>
        /// Decrements the COM reference count on the filter
        /// </summary>
        /// <param name="filter">filter to release</param>
        public static void Release(this IBaseFilter filter)
        {
            BaseDSGraph.Release(filter);
        }

        /// <summary>
        /// Releases all references and memory held by a collection of DetailPinInfo
        /// </summary>
        /// <param name="detailPins"></param>
        public static void Release(this IEnumerable<DetailPinInfo> detailPins)
        {
            if (detailPins != null)
            {
                foreach (DetailPinInfo i in detailPins)
                {
                    i.Dispose();
                }
            }
        }

        /// <summary>
        /// Frees reference to this IPin
        /// </summary>
        /// <param name="pin"></param>
        public static void Release(this IPin pin)
        {
            BaseDSGraph.Release(pin);
        }

        /// <summary>
        /// Frees all references to IPin in this enumerable collection.
        /// </summary>
        /// <param name="pins"></param>
        public static void Release(this IEnumerable<IPin> pins)
        {
            if (pins != null)
            {
                foreach (IPin p in pins)
                {
                    BaseDSGraph.Release(p);
                }
            }
        }

        /// <summary>
        /// Frees references held by this PinInfo
        /// </summary>
        /// <param name="info"></param>
        public static void Release(this PinInfo info)
        {
            DsUtils.FreePinInfo(info);
        }

        /// <summary>
        /// Frees all memory found in an enumerable collection of PinInfo
        /// </summary>
        /// <param name="pinInfo"></param>
        public static void Release(this IEnumerable<PinInfo> pinInfo)
        {
            if (pinInfo != null)
            {
                foreach (PinInfo p in pinInfo)
                {
                    DsUtils.FreePinInfo(p);
                }
            }
        }

        /// <summary>
        /// Frees all memory / references held by this AMMediaType
        /// </summary>
        /// <param name="mediaType"></param>
        public static void Release(this AMMediaType mediaType)
        {
            DsUtils.FreeAMMediaType(mediaType);
        }

        /// <summary>
        /// Frees all memory found in an enumerable collection of AMMediaType
        /// </summary>
        /// <param name="mediaTypes"></param>
        public static void Release(this IEnumerable<AMMediaType> mediaTypes)
        {
            if (mediaTypes != null)
            {
                foreach (AMMediaType t in mediaTypes)
                {
                    DsUtils.FreeAMMediaType(t);
                }
            }
        }

        #endregion
    }
}
