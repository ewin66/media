using System;
using System.Runtime.InteropServices;
using System.Text;

/*
/// This file is used for interfaces that were written by Future Concepts
/// Kevin Dixon
/// 01/28/2009
*/

namespace FutureConcepts.Media.DirectShowLib
{
    /// <summary>
    /// IDVRWriterApi, used to control the DVR Writer filter
    /// </summary>
    /// <author>darnold</author>
    [ComImport]
    [Guid("E0FFBD03-1FCA-4dc7-82C7-D859061A41F9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDVRWriterApi
    {
        /// <summary>
        /// Begins recording
        /// </summary>
        /// <returns></returns>
        [PreserveSig]
        int StartRecording();

        /// <summary>
        /// Stops recording
        /// </summary>
        /// <returns></returns>
        [PreserveSig]
        int StopRecording();

        /// <summary>
        /// Returns the current chunk size
        /// </summary>
        /// <param name="lSize">size (probably in bytes)</param>
        /// <returns></returns>
        [PreserveSig]
        int get_CurrentChunkSize([Out] out int lSize);
    }

    /// <summary>
    /// IFCVolumeMute, supports adjusting of volume and mute properties. See FC Volume Filter
    /// </summary>
    [ComImport]
    [Guid("B28F2C9C-8951-45ad-A42B-29A75A47E7E7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFCVolumeMute
    {
        /// <summary>
        /// Sets the gain for the audio level
        /// </summary>
        /// <param name="attenuationInDB">gain in dB</param>
        /// <returns></returns>
        [PreserveSig]
        int set_Volume([In] int attenuationInDB);

        /// <summary>
        /// Retreives the current attenuation/gain
        /// </summary>
        /// <param name="attenuationInDB">currently set gain in dB</param>
        /// <returns></returns>
        [PreserveSig]
        int get_Volume([Out] out int attenuationInDB);

        /// <summary>
        /// sets the mute
        /// </summary>
        /// <param name="muted">true to mute, false to unmute</param>
        /// <returns></returns>
        [PreserveSig]
        int set_Mute([In, MarshalAs(UnmanagedType.Bool)] bool muted);

        /// <summary>
        /// retreives the current mute state
        /// </summary>
        /// <param name="muted">true if muted, false if unmuted</param>
        /// <returns></returns>
        [PreserveSig]
        int get_Mute([Out, MarshalAs(UnmanagedType.Bool)] out bool muted);
    }

    /// <summary>
    /// interface to control the FC Frame Rate Filter
    /// </summary>
    [ComImport]
    [Guid("2EF86EAE-166F-4524-B272-1ED84C929F88")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFCFrameRateAPI
    {
        /// <summary>
        /// sets the input frame rate, in frames per second
        /// </summary>
        /// <param name="inputFPS">the input frame rate, in frames per second</param>
        /// <returns></returns>
        [PreserveSig]
        int set_InputFramerate([In] double inputFPS);

        /// <summary>
        /// retrives the input frame rate, in frames per second
        /// </summary>
        /// <param name="inputFPS">the input frame rate in frames per second</param>
        /// <returns></returns>
        [PreserveSig]
        int get_InputFramerate([Out] out double inputFPS);

        /// <summary>
        /// sets the target frame rate, in frames per second
        /// </summary>
        /// <param name="targetFPS">target frame rate, in frames per second</param>
        /// <returns></returns>
        [PreserveSig]
        int set_TargetFramerate([In] double targetFPS);

        /// <summary>
        /// retreives the target frame rate, in frames per second
        /// </summary>
        /// <param name="targetFPS"></param>
        /// <returns></returns>
        [PreserveSig]
        int get_TargetFramerate([Out] out double targetFPS);
    }

    /// <summary>
    /// Describes the strategy the Time Stamp Adjust filter should use when encountering a time stamping glitch
    /// </summary>
    public enum FCTimeStampStrategy : int
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        Passive = 0,
        /// <summary>
        /// Set flags that indicate a discontinuity occurred
        /// </summary>
        SetDiscontinuity = 1,
        /// <summary>
        /// Change the time stamp to be in line with the previous running average delta
        /// </summary>
        ChangeStamp = 2
    }

    /// <summary>
    /// Describes the amount of logging to perform
    /// </summary>
    public enum FCTimeStampLogging : int
    {
        /// <summary>
        /// Do not log
        /// </summary>
        Off = 0,
        /// <summary>
        /// Log only changes that occur due to the Strategy setting
        /// </summary>
        Changes = 1,
        /// <summary>
        /// Log every sample and changes made
        /// </summary>
        All = 3
    }

    /// <summary>
    /// FC Time Stamp Adjust filter interface
    /// </summary>
    [ComImport]
    [Guid("E7A1FAEF-C25F-4EEE-B26B-72C88B03742C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFCTimeStampAdjust
    {
        /// <summary>
        /// sets the logging level
        /// </summary>
        /// <param name="level">level of logging</param>
        /// <returns></returns>
        [PreserveSig]
        int set_Logging([In] FCTimeStampLogging level);

        /// <summary>
        /// gets the logging level
        /// </summary>
        /// <param name="level">current level of logging</param>
        /// <returns></returns>
        [PreserveSig]
        int get_Logging([Out] out FCTimeStampLogging level);

        /// <summary>
        /// Set a path to log to.
        /// </summary>
        /// <param name="path">When setting a valid path, then the file is created with the CSV header. Set to null to close the file</param>
        [PreserveSig]
        int set_LogPath([In, MarshalAs(UnmanagedType.BStr)] string path);

        /// <summary>
        /// sets the time stamp adjustment strategy
        /// </summary>
        /// <param name="strategy">strategy to use</param>
        /// <returns></returns>
        [PreserveSig]
        int set_Strategy([In] FCTimeStampStrategy strategy);

        /// <summary>
        /// gets the time stamp adjustment strategy
        /// </summary>
        /// <param name="strategy">currently set strategy</param>
        /// <returns></returns>
        [PreserveSig]
        int get_Strategy([Out] out FCTimeStampStrategy strategy);
    }
}
