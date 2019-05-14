using System;
using System.Diagnostics;

using Leadtools.Multimedia.Common;

namespace FutureConcepts.Media.Tools.License
{
    public class LeadTools
    {
        public class LeadToolsLockEventArgs : EventArgs
        {
            private bool _locked;

            public bool Locked
            {
                get
                {
                    return _locked;
                }
                set
                {
                    _locked = value;
                }
            }

            private string _serial;

            public string Serial
            {
                get
                {
                    return _serial;
                }
                set
                {
                    _serial = value;
                }
            }
        }

        public static event EventHandler<LeadToolsLockEventArgs> SerialChanged;

        public static void Unlock()
        {
            UnlockPair("721707-1890253-000", "Zenith");     // Multimedia SDK v17.5 - re: Rumi Azad e-mail
            UnlockPair("981707-7670031-000", "Zenith");     // Video Streaming Module v17.5
            UnlockPair("911640-7790005-000", "Zenith");     // LEAD RTSP Source Filter

            UnlockPair("721706-7450231-000", "Zenith");     //Multimedia SDK v17
            UnlockPair("981706-2620018-000", "Zenith");     //Video Streaming Module v17
            UnlockPair("231400-5300069-000", "Zenith");     //H.264 Decoder v17
         //   UnlockPair("991500-2310000-001", "Zenith");
            UnlockPair("981501-1040002-000", "Zenith");
            UnlockPair("911404-4710000-001", "Zenith");
            UnlockPair("981405-5050006-001", "Zenith"); // Video Streaming Module v15
            UnlockPair("721500-9510056-001", "Zenith"); // Multimedia SDK v15
            UnlockPair("531428-9120003-001", "Zenith"); // Text Overlay
            UnlockPair("591414-9050001-000", "Zenith"); // MJ2K Decoder
            UnlockPair("591413-6050001-000", "Zenith"); // MJ2K Encoder
            UnlockPair("531425-5150003-000", "Zenith"); // Video Motion Detection Filter (2.0)
            UnlockPair("421400-3840015-000", "Zenith"); // MCMP/MJPEG Decoder
        }

        public static void Lock()
        {
            try
            {
                Leadtools.Multimedia.Common.MultimediaSupport.LockModules(Leadtools.Multimedia.Common.LockType.Computer, "Zenith");
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }

        private static void UnlockPair(string serialNumber, string appId)
        {
            Debug.WriteLine("LT Trying to UNLOCK " + serialNumber);
            try
            {
                Leadtools.Multimedia.Common.MultimediaSupport.UnlockModule(serialNumber, Leadtools.Multimedia.Common.LockType.Computer, appId);
                if (SerialChanged != null)
                {
                    LeadToolsLockEventArgs e = new LeadToolsLockEventArgs();
                    e.Locked = false;
                    e.Serial = serialNumber;
                    SerialChanged(null, e);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Failed unlocking serial# " + serialNumber + " appId " + appId);
                Debug.WriteLine("Reason: " + exc.Message);
            }
        }
    }
}
