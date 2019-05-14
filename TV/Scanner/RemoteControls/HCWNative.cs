using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FutureConcepts.Media.TV.Scanner.RemoteControls
{
    public class HCWNative
    {
        /// <summary>
        /// Registers window handle with Hauppauge IR driver
        /// </summary>
        /// <param name="WindowHandle"></param>
        /// <param name="Msg"></param>
        /// <param name="Verbose"></param>
        /// <param name="IRPort"></param>
        /// <returns></returns>
        [DllImport(@"c:\program files\WinTV\irremote.dll")]
        public static extern bool IR_Open(IntPtr WindowHandle, uint Msg, bool Verbose, ushort IRPort);

        /// <summary>
        /// Gets the received key code (new version, works for PVR-150 as well)
        /// </summary>
        /// <param name="RepeatCount"></param>
        /// <param name="RemoteCode"></param>
        /// <param name="KeyCode"></param>
        /// <returns></returns>
        [DllImport(@"c:\program files\WinTV\irremote.dll")]
        public static extern bool IR_GetSystemKeyCode(out int RepeatCount, out int RemoteCode, out int KeyCode);

        /// <summary>
        /// Unregisters window handle from Hauppauge IR driver
        /// </summary>
        /// <param name="WindowHandle"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [DllImport(@"c:\program files\WINTV\irremote.dll")]
        public static extern bool IR_Close(IntPtr WindowHandle, uint Msg);
    }
}
