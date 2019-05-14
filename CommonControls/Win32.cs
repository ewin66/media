using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FutureConcepts.Media.CommonControls
{
    public class Win32
    {
        #region // Constants //

        #region SW - Show Window enum

        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWNA = 8;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;

        #endregion

        public const int SC_CLOSE = 0xF060;
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_CHILD = 5;
        public const int HT_CAPTION = 0x2;
        public const uint GW_OWNER = 4;
        public const int GWL_STYLE = -16;
        public const int WM_SETREDRAW = 11; 

        #region WM - Window Messages

        public const int WM_QUERYENDSESSION = 0x0011;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_ENDSESSION = 0x0016;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_CLOSE = 16;
        public const int WS_VISIBLE = 0x10000000;
        public const int WM_USER = 0x400;
        public const int WM_CL = WM_USER + 215;
        public const int WM_PRGOPEN = WM_USER + 216;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WS_BORDER = 0x00800000;
        public const int WS_SIZEBOX = 0x00040000;
        public const int WM_DEVICECHANGE = 0x0219;
        public const int WM_POWERBROADCAST = 0x218;

        #endregion

        public const long MONITOR_DEFAULTTONULL = 0;    //If the monitor is not found, return 0
        public const long MONITOR_DEFAULTTOPRIMARY = 1; //If the monitor is not found, return the primary monitor
        public const long MONITOR_DEFAULTTONEAREST = 2; //If the monitor is not found, return the nearest monitor

        /// <summary>
        /// Use with EnumDisplaySettings -- retrive the current settings
        /// </summary>
        public const int ENUM_CURRENT_SETTINGS = -1;
        /// <summary>
        /// Use with EnumDisplaySettings -- retreive the settings stored in the registry
        /// </summary>
        public const int ENUM_REGISTRY_SETTINGS = -2;

        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;    // use passed dwFileAttribute
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1; // 'Small icon
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;


        [Flags]
        public enum EXECUTION_STATE : uint
        {
            Failure = 0x0,
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            // Legacy flag, should not be used.
            // ES_USER_PRESENT   = 0x00000004,
            ES_CONTINUOUS = 0x80000000,
        }

        #endregion

        #region Winerror.h

        public const int ERROR_SUCCESS = 0;
        public const int NO_ERROR = 0;

        public const int ERROR_NOT_READY = 21;

        #endregion

        #region CopyFileEx

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName,
                                             CopyProgressRoutine lpProgressRoutine,
                                             IntPtr lpData, ref Int32 pbCancel,
                                             CopyFileFlags dwCopyFlags);

        public delegate CopyProgressResult CopyProgressRoutine(long TotalFileSize,
                                                                long TotalBytesTransferred,
                                                                long StreamSize,
                                                                long StreamBytesTransferred,
                                                                uint dwStreamNumber,
                                                                CopyProgressCallbackReason dwCallbackReason,
                                                                IntPtr hSourceFile,
                                                                IntPtr hDestinationFile,
                                                                IntPtr lpData);

        [Flags]
        public enum CopyFileFlags : uint
        {
            COPY_FILE_FAIL_IF_EXISTS = 0x00000001,
            COPY_FILE_RESTARTABLE = 0x00000002,
            COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x00000004,
            COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008,
            COPY_FILE_COPY_SYMLINK = 0x00000800 //NT 6.0+
        }

        public enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        public enum CopyProgressCallbackReason : uint
        {
            CALLBACK_CHUNK_FINISHED = 0x00000000,
            CALLBACK_STREAM_SWITCH = 0x00000001
        }

        #endregion

        #region GetSystemInfo / GetNativeSystemInfo

        /// <summary>
        /// Retrieves information about the current system to an application running under WOW64.
        /// If the function is called from a 64-bit application, it is equivalent to the GetSystemInfo function.
        /// </summary>
        /// <param name="lpSystemInfo">A pointer to a SYSTEM_INFO structure that receives the information.</param>
        [DllImport("kernel32.dll")]
        public static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

        /// <summary>
        /// Retrieves information about the current system.
        /// </summary>
        /// <param name="lpSystemInfo">A pointer to a SYSTEM_INFO structure that receives the information.</param>
        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public _PROCESSOR_INFO_UNION uProcessorInfo;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            public uint dwOemId;
            [FieldOffset(0)]
            public ushort wProcessorArchitecture;
            [FieldOffset(2)]
            public ushort wReserved;

            /// <summary>
            /// Gets the Processor's Architecture.
            /// </summary>
            /// <returns>The type of processor architecture represented here</returns>
            public ProcessorArchitecture ToProcessorArchitecture()
            {
                try
                {
                    ProcessorArchitecture arch = (ProcessorArchitecture)wProcessorArchitecture;
                    return arch;
                }
                catch
                {
                    return ProcessorArchitecture.Unknown;
                }
            }
        }

        /// <summary>
        /// Represents a processor architecture.
        /// </summary>
        public enum ProcessorArchitecture
        {
            /// <summary>
            /// PROCESSOR_ARCHITECTURE_INTEL
            /// </summary>
            x86         = 0,
            /// <summary>
            /// Intel Itanium Processor Family (IPF)
            /// </summary>
            IA64        = 6,
            /// <summary>
            /// x64 (AMD or Intel)
            /// </summary>
            x64         = 9,
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown     = 0xFFFF
        }

        #endregion

        #region user32.dll FormatMessage

        /// <summary>
        /// The formatting options, and how to interpret the lpSource parameter. The low-order byte of dwFlags specifies how the function handles line breaks in the output buffer. The low-order byte can also specify the maximum width of a formatted output line. 
        /// </summary>
        [Flags]
        public enum FormatMessageFlags : int
        {
            /// <summary>
            /// The function allocates a buffer large enough to hold the formatted message, and places a pointer to the allocated buffer at the address specified by lpBuffer. The lpBuffer parameter is a pointer to an LPTSTR; you must cast the pointer to an LPTSTR (for example, (LPTSTR)&lpBuffer). The nSize parameter specifies the minimum number of TCHARs to allocate for an output message buffer. The caller should use the LocalFree function to free the buffer when it is no longer needed.
            /// </summary>
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,

            /// <summary>
            /// The Arguments parameter is not a va_list structure, but is a pointer to an array of values that represent the arguments. This flag cannot be used with 64-bit integer values. If you are using a 64-bit integer, you must use the va_list structure.
            /// </summary>
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000,

            /// <summary>
            /// The lpSource parameter is a module handle containing the message-table resource(s) to search. If this lpSource handle is NULL, the current process's application image file will be searched. This flag cannot be used with FORMAT_MESSAGE_FROM_STRING. If the module has no message table resource, the function fails with ERROR_RESOURCE_TYPE_NOT_FOUND.
            /// </summary>
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,

            /// <summary>
            /// The lpSource parameter is a pointer to a null-terminated string that contains a message definition. The message definition may contain insert sequences, just as the message text in a message table resource may. This flag cannot be used with FORMAT_MESSAGE_FROM_HMODULE or FORMAT_MESSAGE_FROM_SYSTEM.
            /// </summary>
            FORMAT_MESSAGE_FROM_STRING = 0x00000400,

            /// <summary>
            /// The function should search the system message-table resource(s) for the requested message. If this flag is specified with FORMAT_MESSAGE_FROM_HMODULE, the function searches the system message table if the message is not found in the module specified by lpSource. This flag cannot be used with FORMAT_MESSAGE_FROM_STRING. If this flag is specified, an application can pass the result of the GetLastError function to retrieve the message text for a system-defined error.
            /// </summary>
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,

            /// <summary>
            /// Insert sequences in the message definition are to be ignored and passed through to the output buffer unchanged. This flag is useful for fetching a message for later formatting. If this flag is set, the Arguments parameter is ignored.
            /// </summary>
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,

            /// <summary>
            /// There are no output line width restrictions. The function stores line breaks that are in the message definition text into the output buffer.
            /// </summary>
            FORMAT_MESSAGE_NO_MAX_WIDTH = 0,

            /// <summary>
            /// The function ignores regular line breaks in the message definition text. The function stores hard-coded line breaks in the message definition text into the output buffer. The function generates no new line breaks.
            /// </summary>
            FORMAT_MESSAGE_MAX_WIDTH_MASK = 0x000000FF
        }

        /// <summary>
        /// The FormatMessage function formats a message string. The function requires a message definition as input. The message definition can come from a buffer passed into the function. It can come from a message table resource in an already-loaded module. Or the caller can ask the function to search the system's message table resource(s) for the message definition. The function finds the message definition in a message table resource based on a message identifier and a language identifier. The function copies the formatted message text to an output buffer, processing any embedded insert sequences if requested.
        /// </summary>
        /// <param name="dwFlags">Specifies aspects of the formatting process and how to interpret the lpSource parameter. The low-order byte of dwFlags specifies how the function handles line breaks in the output buffer. The low-order byte can also specify the maximum width of a formatted output line.</param>
        /// <param name="lpSource">Specifies the location of the message definition. The type of this parameter depends upon the settings in the dwFlags parameter.</param>
        /// <param name="dwMessageId">Specifies the message identifier for the requested message. This parameter is ignored if dwFlags includes FORMAT_MESSAGE_FROM_STRING.</param>
        /// <param name="dwLanguageId">Specifies the language identifier for the requested message. This parameter is ignored if dwFlags includes FORMAT_MESSAGE_FROM_STRING.</param>
        /// <param name="lpBuffer">Pointer to a buffer for the formatted (and null-terminated) message. If dwFlags includes FORMAT_MESSAGE_ALLOCATE_BUFFER, the function allocates a buffer using the LocalAlloc function, and places the pointer to the buffer at the address specified in lpBuffer.</param>
        /// <param name="nSize">If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter specifies the maximum number of TCHARs that can be stored in the output buffer. If FORMAT_MESSAGE_ALLOCATE_BUFFER is set, this parameter specifies the minimum number of TCHARs to allocate for an output buffer. For ANSI text, this is the number of bytes; for Unicode text, this is the number of characters.</param>
        /// <param name="Arguments">Pointer to an array of values that are used as insert values in the formatted message. A %1 in the format string indicates the first value in the Arguments array; a %2 indicates the second argument; and so on.</param>
        /// <returns>If the function succeeds, the return value is the number of TCHARs stored in the output buffer, excluding the terminating null character.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("user32.dll", EntryPoint = "FormatMessageA", CharSet = CharSet.Ansi)]
        public static extern int FormatMessage(FormatMessageFlags dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int Arguments);

        #endregion

        /// <summary>
        /// Enables an application to inform the system that it is in use, thereby preventing
        /// the system from entering sleep or turning off the display while the application is running.
        /// </summary>
        /// <param name="esFlags">The thread's execution requirements.</param>
        /// <returns>
        /// If the function succeeds, the return value is the previous thread execution state.
        /// If the function fails, the return value is EXECUTION_STATE.Failure.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        #region Display Devices

        /// <summary>
        /// Enumerates the display devices on the system
        /// </summary>
        /// <param name="lpDevice">The Device name, as obtained from the Forms.Screen class. Or null to just use the iDevNum</param>
        /// <param name="iDevNum">device number/index. 0 based. Ex: 3 devices would be 0,1, and 2.</param>
        /// <param name="lpDisplayDevice">(output) the passed in object will be populated</param>
        /// <param name="dwFlags">Set to 0x01 for "EDD_GET_DEVICE_INTERFACE_NAME"</param>
        /// <returns>true if the call succeeded</returns>
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        /// <summary>
        /// Enumerates a display's settings.
        /// </summary>
        /// <param name="deviceName">The device name, as found in a DISPLAY_DEVICE structure</param>
        /// <param name="modeNum">one of ENUM_CURRENT_SETTINGS or ENUM_REGISTRY_SETTINGS</param>
        /// <param name="devMode">the output device mode information</param>
        /// <returns>non-zero on success</returns>
        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        #endregion

        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatus(
            out MemoryStatus stat
        );

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags
         );

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        #region Window Management

        public enum ScrollBarType : int
        {
            /// <summary>
            /// A window's Horizontal scrollbar
            /// </summary>
            SB_HORZ = 0,
            /// <summary>
            /// A window's Vertical scrollbar
            /// </summary>
            SB_VERT = 1,
            /// <summary>
            /// An actual scroll bar control. HWND must be a handle to a scroll bar control
            /// </summary>
            SB_CTL = 2,
            /// <summary>
            /// A windows Horizontal and Vertical scrollbars
            /// </summary>
            SB_BOTH = 3
        }

        /// <summary>
        /// The ShowScrollBar function shows or hides the specified scroll bar. 
        /// </summary>
        /// <param name="hWnd">Handle to a scroll bar control or a window with a standard scroll bar, depending on the value of the wBar parameter. </param>
        /// <param name="wBar">Specifies the scroll bar(s) to be shown or hidden. This parameter can be one of the following values. </param>
        /// <param name="bShow">Specifies whether the scroll bar is shown or hidden. If this parameter is TRUE, the scroll bar is shown; otherwise, it is hidden. </param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowScrollBar(IntPtr hWnd, ScrollBarType wBar, bool bShow);

        /// <summary>
        /// The RegisterWindowMessage function defines a new window message that is guaranteed to be unique throughout the system.
        /// The message value can be used when sending or posting messages.
        /// </summary>
        /// <param name="lpString">Pointer to a null-terminated string that specifies the message to be registered.</param>
        /// <returns>
        /// If the message is successfully registered, the return value is a message identifier in the range 0xC000 through 0xFFFF.
        /// If the function fails, the return value is zero. To get extended error information, call  GetLastError.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        /// <summary>
        /// Callback prototype for EnumWindows function.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <param name="lParam">Optional parameter.</param>
        /// <returns></returns>
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(
            IntPtr hWnd,
            StringBuilder title,
            int size
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int EnumWindows(
            EnumWindowsProc lpEnumFunc,
            IntPtr lParam
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(
            IntPtr hWnd,
            out int process
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(
            IntPtr hWnd
        );

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(
            IntPtr hWnd,
            int nCmdShow
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsIconic(
            IntPtr hwnd
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool IsZoomed(
            IntPtr hwnd
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto, EntryPoint = "IsWindowEnabled")]
        public static extern bool IsWindowEnabled(
            IntPtr hWnd
        );

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(
            string lpClassName, // class name 
            string lpWindowName // window name 
        );

        [DllImport("user32.dll")]
        public static extern int SendMessage(
            IntPtr hWnd, // handle to destination window 
            uint Msg, // message 
            int wParam, // first message parameter 
            int lParam // second message parameter 
        );

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Unicode, EntryPoint = "SetWindowLongW")]
        public static extern long SetWindowLong(
            IntPtr hWnd,
            int nIndex,
            long styles
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int SetForegroundWindow(
            IntPtr hWnd
        );
        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto, EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(
            IntPtr hWnd,
            int x,
            int y,
            int width,
            int height,
            bool bRepaint
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto, EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(
            IntPtr hWnd,
            ref RECT rect
        );

        [DllImport("user32.dll")]
        public static extern bool PostMessage(
            IntPtr hWnd,
            int wMsg,
            int wParam,
            int lParam
        );

        [DllImport("user32.dll")]
        public static extern bool PostMessage(
            IntPtr hWnd,
            int wMsg,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(
            IntPtr hWnd,
            uint wCmd
        );
        [DllImport("user32.dll")]
        public static extern int GetClassName(
            IntPtr hWnd,
            string lpClassName,
            int nMaxCount
        );
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(
            IntPtr hWnd
        );
        [DllImport("user32.dll")]
        public static extern int GetWindowText(
            IntPtr hWnd,
            string lpString,
            int cch
        );
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(
            IntPtr hWnd,
            ref WINDOWPLACEMENT lpwndpl
        );
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(
            IntPtr hWnd,
            int nIndex
        );
        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(
            IntPtr hWnd,
            int nCmdShow
        );

        [DllImport("user32.dll")]
        public static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(
            IntPtr hWnd
            );

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(
            IntPtr hWnd,
            long dwFlag
            );

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(
            POINT p
        );

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(
            IntPtr hWnd
        );

        /// <summary>
        /// Retreives the handle to the top most window at the specified point in the monitor
        /// </summary>
        /// <param name="hWnd">The handle to the window to comapare the top window to</param>
        /// <param name="p">The point at which you want to check</param>
        /// <returns>True if the window handle is the top window handle at that point, false otherwise</returns>
        public static bool IsTopWindowAtPoint(IntPtr hWnd, POINT p)
        {
            IntPtr parentHandle = IntPtr.Zero;
           
            // Get window handle at point p
            IntPtr handle = Win32.WindowFromPoint(p);

            // Get Parent handle
            do
            {
                parentHandle = Win32.GetParent(handle);
                if (!parentHandle.Equals(IntPtr.Zero))
                    handle = parentHandle;

            } while (!parentHandle.Equals(IntPtr.Zero));

            return handle.Equals(hWnd);
        }

        #endregion

        #region // Structs //

        #region Monitor/Display Device Query Stuff

        /// <summary>
        /// initialize the "cb" member to sizeof
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;

            public override string ToString()
            {
                return DeviceString;
            }
        }

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x16,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        /// <summary>
        /// initialize dmSize to the sizeof
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
            public string dmDeviceName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public DevModeFlags dmFields;

            public POINTAPI dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmCollate;

            // CCHDEVICENAME = 32 = 0x50 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
            public string dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningHeight;

        }

        [Flags()]
        public enum DevModeFlags : int
        {
            Orientation = 0x1,
            PaperSize = 0x2,
            PaperLength = 0x4,
            PaperWidth = 0x8,
            Scale = 0x10,
            Position = 0x20,
            NUP = 0x40,
            DisplayOrientation = 0x80,
            Copies = 0x100,
            DefaultSource = 0x200,
            PrintQuality = 0x400,
            Color = 0x800,
            Duplex = 0x1000,
            YResolution = 0x2000,
            TTOption = 0x4000,
            Collate = 0x8000,
            FormName = 0x10000,
            LogPixels = 0x20000,
            BitsPerPixel = 0x40000,
            PelsWidth = 0x80000,
            PelsHeight = 0x100000,
            DisplayFlags = 0x200000,
            DisplayFrequency = 0x400000,
            ICMMethod = 0x800000,
            ICMIntent = 0x1000000,
            MediaType = 0x2000000,
            DitherType = 0x4000000,
            PanningWidth = 0x8000000,
            PanningHeight = 0x10000000,
            DisplayFixedOutput = 0x20000000
        }

        #endregion

        #region WM_DEVICECHANGE-related

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public uint dbcv_size;
            public DeviceType dbcv_devicetype;
            public uint dbcv_reserved;
            public uint dbcv_unitmask;
            public VolumeFlags dbcv_flags;
        }

        public enum DeviceType : uint
        {
            OEM = 0x00000000,           //DBT_DEVTYP_OEM
            DeviceNode = 0x00000001,    //DBT_DEVTYP_DEVNODE
            Volume = 0x00000002,        //DBT_DEVTYP_VOLUME
            Port = 0x00000003,          //DBT_DEVTYP_PORT
            Net = 0x00000004            //DBT_DEVTYP_NET
        }

        public enum VolumeFlags : ushort
        {
            USB = 0x0000,             //DBTF_MEDIA
            Media = 0x0001,             //DBTF_MEDIA
            Net = 0x0002                //DBTF_NET
        }

        public enum DeviceEventBroadcast : ushort
        {
            DBT_DEVICEARRIVAL = 0x8000,           //DBT_DEVICEARRIVAL
            DBT_DEVICEQUERYREMOVE = 0x8001,       //DBT_DEVICEQUERYREMOVE
            DBT_DEVICEQUERYREMOVEFAILED = 0x8002, //DBT_DEVICEQUERYREMOVEFAILED
            DBT_DEVICEREMOVEPENDING = 0x8003,     //DBT_DEVICEREMOVEPENDING
            DBT_DEVICEREMOVECOMPLETE = 0x8004,    //DBT_DEVICEREMOVECOMPLETE
            DBT_SPECIFIC = 0x8005,          //DBT_DEVICEREMOVECOMPLETE
            DBT_CUSTOMEVENT = 0x8006             //DBT_CUSTOMEVENT
        }

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public long x;
            public long y;
        }

        /// <summary>
        /// Rectangle information for Win32 API calls.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINTAPI ptMinPosition;
            public POINTAPI ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryStatus
        {
            public uint Length;
            public uint MemoryLoad;
            public uint TotalPhysical;
            public uint AvailablePhysical;
            public uint TotalPageFile;
            public uint AvailablePageFile;
            public uint TotalVirtual;
            public uint AvailableVirtual;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };
        #endregion

        #region kernel32.dll LoadLibrary and Friends

        /// <summary>
        /// The LoadLibrary function maps the specified executable module into the address space of the calling process
        /// </summary>
        /// <param name="lpLibFileName">Pointer to a null-terminated string that names the executable module (either a .dll or .exe file). The name specified is the file name of the module and is not related to the name stored in the library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.
        /// <returns>If the function succeeds, the return value is a handle to the module.If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA", CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        /// <summary>
        /// The FreeLibrary function decrements the reference count of the loaded dynamic-link library (DLL). When the reference count reaches zero, the module is unmapped from the address space of the calling process and the handle is no longer valid.
        /// </summary>
        /// <param name="hLibModule">Handle to the loaded DLL module. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Ansi)]
        public static extern int FreeLibrary(IntPtr hLibModule);

        /// <summary>
        /// The GetProcAddress function retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">Handle to the DLL module that contains the function or variable. The LoadLibrary or GetModuleHandle function returns this handle.</param>
        /// <param name="lpProcName">Pointer to a null-terminated string containing the function or variable name, or the function's ordinal value. If this parameter is an ordinal value, it must be in the low-order word; the high-order word must be zero.</param>
        /// <returns>If the function succeeds, the return value is the address of the exported function or variable.<br></br><br>If the function fails, the return value is NULL. To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        #endregion

        #region InitiateSystemShutdownEx

        /// <summary>
        /// Initiates a shutdown and optional restart of the specified computer, and optionally records the reason for the shutdown.
        /// </summary>
        /// <param name="lpMachineName">The network name of the computer to be shut down. If lpMachineName  is NULL or an empty string, the function shuts down the local computer.</param>
        /// <param name="lpMessage">The message to be displayed in the shutdown dialog box. This parameter can be NULL if no message is required. </param>
        /// <param name="dwTimeout">
        /// The length of time that the shutdown dialog box should be displayed, in seconds.
        /// If dwTimeout is not zero, InitiateSystemShutdownEx displays a dialog box on the specified computer.
        /// If dwTimeout is zero, the computer shuts down without displaying the dialog box, and the shutdown cannot be stopped by AbortSystemShutdown.
        /// </param>
        /// <param name="bForceAppsClosed">If this parameter is TRUE, applications with unsaved changes are to be forcibly closed. If this parameter is FALSE, the system displays a dialog box instructing the user to close the applications.</param>
        /// <param name="bRebootAfterShutdown">If this parameter is TRUE, the computer is to restart immediately after shutting down. If this parameter is FALSE, the system flushes all caches to disk and safely powers down the system.</param>
        /// <param name="dwReason">The reason for initiating the shutdown. This parameter must be one of the system shutdown reason codes.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InitiateSystemShutdownEx(
            string lpMachineName,
            string lpMessage,
            uint dwTimeout,
            bool bForceAppsClosed,
            bool bRebootAfterShutdown,
            ShutdownReason dwReason);

        #endregion

        #region ExitWindowsEx

        /// <summary>
        /// The ExitWindowsEx function either logs off the current user, shuts down the system, or shuts down and restarts the system. It sends the WM_QUERYENDSESSION message to all applications to determine if they can be terminated.
        /// </summary>
        /// <param name="uFlags">Specifies the type of shutdown.</param>
        /// <param name="dwReserved">This parameter is ignored.</param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Ansi)]
        public static extern int ExitWindowsEx(ExitWindowsExFlags uFlags, ShutdownReason dwReason);

        [Flags]
        public enum ShutdownReason : int
        {
            /// <summary>
            /// The shutdown was unplanned
            /// </summary>
            Unplanned = 0,

            /// <summary>
            /// Application issue.
            /// </summary>
            SHTDN_REASON_MAJOR_APPLICATION = 0x00040000,

            /// <summary>
            /// Hardware issue.
            /// </summary>
            SHTDN_REASON_MAJOR_HARDWARE = 0x00010000,

            /// <summary>
            /// The InitiateSystemShutdown function was used instead of InitiateSystemShutdownEx.
            /// </summary>
            SHTDN_REASON_MAJOR_LEGACY_API = 0x00070000,

            /// <summary>
            /// Operating system issue.
            /// </summary>
            SHTDN_REASON_MAJOR_OPERATINGSYSTEM = 0x00020000,

            /// <summary>
            /// Other issue.
            /// </summary>
            SHTDN_REASON_MAJOR_OTHER = 0x00000000,

            /// <summary>
            /// Power failure.
            /// </summary>
            SHTDN_REASON_MAJOR_POWER = 0x00060000,

            /// <summary>
            /// Software issue.
            /// </summary>
            SHTDN_REASON_MAJOR_SOFTWARE = 0x00030000,

            /// <summary>
            /// System failure.
            /// </summary>
            SHTDN_REASON_MAJOR_SYSTEM = 0x00050000,

            /// <summary>
            /// Blue screen crash event.
            /// </summary>
            SHTDN_REASON_MINOR_BLUESCREEN = 0x0000000F,

            /// <summary>
            /// Unplugged.
            /// </summary>
            SHTDN_REASON_MINOR_CORDUNPLUGGED = 0x0000000b,

            /// <summary>
            /// Disk.
            /// </summary>
            SHTDN_REASON_MINOR_DISK = 0x00000007,

            /// <summary>
            /// Environment.
            /// </summary>
            SHTDN_REASON_MINOR_ENVIRONMENT = 0x0000000c,

            /// <summary>
            /// Driver.
            /// </summary>
            SHTDN_REASON_MINOR_HARDWARE_DRIVER = 0x0000000d,

            /// <summary>
            /// Hot fix.
            /// </summary>
            SHTDN_REASON_MINOR_HOTFIX = 0x00000011,

            /// <summary>
            /// Hot fix uninstallation.
            /// </summary>
            SHTDN_REASON_MINOR_HOTFIX_UNINSTALL = 0x00000017,

            /// <summary>
            /// Unresponsive.
            /// </summary>
            SHTDN_REASON_MINOR_HUNG = 0x00000005,

            /// <summary>
            /// Installation.
            /// </summary>
            SHTDN_REASON_MINOR_INSTALLATION = 0x00000002,

            /// <summary>
            /// Maintenance.
            /// </summary>
            SHTDN_REASON_MINOR_MAINTENANCE = 0x00000001,

            /// <summary>
            /// MMC issue.
            /// </summary>
            SHTDN_REASON_MINOR_MMC = 0x00000019,

            /// <summary>
            /// Network connectivity.
            /// </summary>
            SHTDN_REASON_MINOR_NETWORK_CONNECTIVITY = 0x00000014,

            /// <summary>
            /// Network card.
            /// </summary>
            SHTDN_REASON_MINOR_NETWORKCARD = 0x00000009,

            /// <summary>
            /// Other issue.
            /// </summary>
            SHTDN_REASON_MINOR_OTHER = 0x00000000,

            /// <summary>
            /// Other driver event.
            /// </summary>
            SHTDN_REASON_MINOR_OTHERDRIVER = 0x0000000e,

            /// <summary>
            /// Power supply.
            /// </summary>
            SHTDN_REASON_MINOR_POWER_SUPPLY = 0x0000000a,

            /// <summary>
            /// Processor.
            /// </summary>
            SHTDN_REASON_MINOR_PROCESSOR = 0x00000008,

            /// <summary>
            /// Reconfigure.
            /// </summary>
            SHTDN_REASON_MINOR_RECONFIG = 0x00000004,

            /// <summary>
            /// Security issue.
            /// </summary>
            SHTDN_REASON_MINOR_SECURITY = 0x00000013,

            /// <summary>
            /// Security patch.
            /// </summary>
            SHTDN_REASON_MINOR_SECURITYFIX = 0x00000012,

            /// <summary>
            /// Security patch uninstallation.
            /// </summary>
            SHTDN_REASON_MINOR_SECURITYFIX_UNINSTALL = 0x00000018,

            /// <summary>
            /// Service pack.
            /// </summary>
            SHTDN_REASON_MINOR_SERVICEPACK = 0x00000010,

            /// <summary>
            /// Service pack uninstallation.
            /// </summary>
            SHTDN_REASON_MINOR_SERVICEPACK_UNINSTALL = 0x00000016,

            /// <summary>
            /// Terminal Services.
            /// </summary>
            SHTDN_REASON_MINOR_TERMSRV = 0x00000020,

            /// <summary>
            /// Unstable.
            /// </summary>
            SHTDN_REASON_MINOR_UNSTABLE = 0x00000006,

            /// <summary>
            /// Upgrade.
            /// </summary>
            SHTDN_REASON_MINOR_UPGRADE = 0x00000003,

            /// <summary>
            /// WMI issue.
            /// </summary>
            SHTDN_REASON_MINOR_WMI = 0x00000015,

            /// <summary>
            /// The reason code is defined by the user. For more information, see Defining a Custom Reason Code. If this flag is not present, the reason code is defined by the system.
            /// </summary>
            SHTDN_REASON_FLAG_USER_DEFINED = 0x40000000,

            /// <summary>
            /// The shutdown was planned. The system generates a System State Data (SSD) file. This file contains system state information such as the processes, threads, memory usage, and configuration.
            /// </summary>
            SHTDN_REASON_FLAG_PLANNED = unchecked((int)0x80000000)
        }

        [Flags]
        public enum ExitWindowsExFlags : int
        {
            /// <summary>
            /// Shuts down all processes running in the logon session of the process that called the ExitWindowsEx function.
            /// Then it logs the user off. This flag can be used only by processes running in an interactive user's logon session.
            /// </summary>
            EWX_LOGOFF = 0,

            /// <summary>
            /// Shuts down the system and turns off the power. The system must support the power-off feature.
            /// The calling process must have the SE_SHUTDOWN_NAME privilege. For more information, see the following Remarks section.
            /// </summary>
            EWX_POWEROFF = 0x00000008,

            /// <summary>
            /// Shuts down the system and then restarts the system. The calling process must have the SE_SHUTDOWN_NAME privilege.
            /// For more information, see the following Remarks section.
            /// </summary>
            EWX_REBOOT = 0x00000002,

            /// <summary>
            /// Shuts down the system and then restarts it, as well as any applications that have been registered for restart
            /// using the RegisterApplicationRestart function.
            /// These application receive the WM_QUERYENDSESSION message with lParam set to the ENDSESSION_CLOSEAPP value.
            /// For more information, see Guidelines for Applications.
            /// </summary>
            EWX_RESTARTAPPS = 0x00000040,

            /// <summary>
            /// Shuts down the system to a point at which it is safe to turn off the power.
            /// All file buffers have been flushed to disk, and all running processes have stopped.
            /// The calling process must have the SE_SHUTDOWN_NAME privilege.
            /// For more information, see the following Remarks section.
            /// Specifying this flag will not turn off the power even if the system supports the power-off feature.
            /// You must specify EWX_POWEROFF to do this.
            ///     Windows XP with SP1:  If the system supports the power-off feature, specifying this flag turns off the power.
            /// </summary>
            EWX_SHUTDOWN = 0x00000001,

            /// <summary>
            /// This flag has no effect if terminal services is enabled.
            /// Otherwise, the system does not send the WM_QUERYENDSESSION message.
            /// This can cause applications to lose data. Therefore, you should only use this flag in an emergency.
            /// </summary>
            EWX_FORCE = 0x00000004,

            /// <summary>
            /// Forces processes to terminate if they do not respond to the WM_QUERYENDSESSION or WM_ENDSESSION
            /// message within the timeout interval. For more information, see the Remarks.
            /// </summary>
            EWX_FORCEIFHUNG = 0x00000010
        }

        #endregion

        #region advapi32.dll Security Tokens, etc

        /// <summary>
        /// The OpenProcessToken function opens the access token associated with a process
        /// </summary>
        /// <param name="ProcessHandle">Handle to the process whose access token is opened</param>
        /// <param name="DesiredAccess">Specifies an access mask that specifies the requested types of access to the access token. These requested access types are compared with the token's discretionary access-control list (DACL) to determine which accesses are granted or denied.</param>
        /// <param name="TokenHandle">Pointer to a handle identifying the newly-opened access token when the function returns.</param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", CharSet = CharSet.Ansi)]
        public static extern int OpenProcessToken(IntPtr ProcessHandle, AccessToken DesiredAccess, ref IntPtr TokenHandle);

        /// <summary>
        /// The LookupPrivilegeValue function retrieves the locally unique identifier (LUID) used on a specified system to locally represent the specified privilege name.
        /// </summary>
        /// <param name="lpSystemName">Pointer to a null-terminated string specifying the name of the system on which the privilege name is looked up. If a null string is specified, the function attempts to find the privilege name on the local system.</param>
        /// <param name="lpName">Pointer to a null-terminated string that specifies the name of the privilege, as defined in the Winnt.h header file. For example, this parameter could specify the constant SE_SECURITY_NAME, or its corresponding string, "SeSecurityPrivilege".</param>
        /// <param name="lpLuid">Pointer to a variable that receives the locally unique identifier by which the privilege is known on the system, specified by the lpSystemName parameter.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br></br><br>If the function fails, the return value is zero. To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueA", CharSet = CharSet.Ansi)]
        public static extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);

        /// <summary>
        /// The AdjustTokenPrivileges function enables or disables privileges in the specified access token. Enabling or disabling privileges in an access token requires TOKEN_ADJUST_PRIVILEGES access.
        /// </summary>
        /// <param name="TokenHandle">Handle to the access token that contains the privileges to be modified. The handle must have TOKEN_ADJUST_PRIVILEGES access to the token. If the PreviousState parameter is not NULL, the handle must also have TOKEN_QUERY access.</param>
        /// <param name="DisableAllPrivileges">Specifies whether the function disables all of the token's privileges. If this value is TRUE, the function disables all privileges and ignores the NewState parameter. If it is FALSE, the function modifies privileges based on the information pointed to by the NewState parameter.</param>
        /// <param name="NewState">Pointer to a TOKEN_PRIVILEGES structure that specifies an array of privileges and their attributes. If the DisableAllPrivileges parameter is FALSE, AdjustTokenPrivileges enables or disables these privileges for the token. If you set the SE_PRIVILEGE_ENABLED attribute for a privilege, the function enables that privilege; otherwise, it disables the privilege. If DisableAllPrivileges is TRUE, the function ignores this parameter.</param>
        /// <param name="BufferLength">Specifies the size, in bytes, of the buffer pointed to by the PreviousState parameter. This parameter can be zero if the PreviousState parameter is NULL.</param>
        /// <param name="PreviousState">Pointer to a buffer that the function fills with a TOKEN_PRIVILEGES structure that contains the previous state of any privileges that the function modifies. This parameter can be NULL.</param>
        /// <param name="ReturnLength">Pointer to a variable that receives the required size, in bytes, of the buffer pointed to by the PreviousState parameter. This parameter can be NULL if PreviousState is NULL.</param>
        /// <returns>If the function succeeds, the return value is nonzero. To determine whether the function adjusted all of the specified privileges, call Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", CharSet = CharSet.Ansi)]
        public static extern int AdjustTokenPrivileges(IntPtr TokenHandle, int DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, ref TOKEN_PRIVILEGES PreviousState, ref int ReturnLength);

        [Flags]
        public enum AccessToken : int
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            STANDARD_RIGHTS_READ = 0x00020000,
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY),
            TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
                                TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
                                TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
                                TOKEN_ADJUST_SESSIONID)
        }

        /// <summary>
        ///  The TOKEN_PRIVILEGES structure contains information about a set of privileges for an access token
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TOKEN_PRIVILEGES
        {
            /// <summary>
            ///  Specifies the number of entries in the Privileges array
            /// </summary>
            public int PrivilegeCount;
            /// <summary>
            ///  Specifies an array of LUID_AND_ATTRIBUTES structures. Each structure contains the LUID and attributes of a privilege
            /// </summary>
            public LUID_AND_ATTRIBUTES Privileges;
        }

        /// <summary>
        ///  The LUID_AND_ATTRIBUTES structure represents a locally unique identifier (LUID) and its attributes
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LUID_AND_ATTRIBUTES
        {
            /// <summary>
            ///  Specifies an LUID value
            /// </summary>
            public LUID pLuid;
            /// <summary>
            /// Specifies attributes of the LUID. This value contains up to 32 one-bit flags
            /// Its meaning is dependent on the definition and use of the LUID
            /// </summary>
            public LUIDAttributes Attributes;
        }

        [Flags]
        public enum LUIDAttributes : int
        {
            SE_PRIVILEGE_ENABLED = 0x2
        }

        /// <summary>
        ///  An LUID is a 64-bit value guaranteed to be unique only on the system on which it was generated
        /// </summary>
        // The uniqueness of a locally unique identifier (LUID) is guaranteed only until the system is restarted
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LUID
        {
            /// <summary>
            ///  The low order part of the 64 bit value
            /// </summary>
            public int LowPart;
            /// <summary>
            ///  The high order part of the 64 bit value
            /// </summary>
            public int HighPart;
        }

        #endregion
    }
}
