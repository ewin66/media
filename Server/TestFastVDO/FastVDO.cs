using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
using System.Threading;

using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Server.TestFastVDO
{
    public class FastVDO : BaseGraph
    {
        private IntPtr _deviceContext;

        private IBaseFilter _byteStreamFilter;

        public FastVDO()
        {
            SetParameters();
            _captureFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "FastVDO-SmartCapture QBoxSplitter");
            _byteStreamFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "H.264 Byte Stream Transform");
            ConnectFilters(_captureFilter, "Video", _byteStreamFilter, "XForm In");
            FilterGraphTools.RenderPin(_graphBuilder, _byteStreamFilter, "XForm Out");
        }

        public override void Dispose(bool disposing)
        {
            if (_captureFilter != null)
            {
                Marshal.ReleaseComObject(_captureFilter);
                _captureFilter = null;
            }
            if (_byteStreamFilter != null)
            {
                Marshal.ReleaseComObject(_byteStreamFilter);
                _byteStreamFilter = null;
            }
            base.Dispose(disposing);
        }

        public void SetParameters()
        {
            Console.WriteLine("SetParameters:");
            _deviceContext = NativeMethods.SCOpen(1);
            if (_deviceContext == IntPtr.Zero)
            {
                throw new Exception("bad device context");
            }
            if (NativeMethods.SCSetMediaType(_deviceContext, 1) != 1)
            {
                throw new Exception("SCSetMediaType failed");
            }
            Thread.Sleep(1200);
            int videoBitRate = 128;
            Console.WriteLine("SCSetVideoBitRate({0})", videoBitRate);
            if (NativeMethods.SCSetVideoBitRate(_deviceContext, videoBitRate) != 1)
            {
                throw new Exception("SCSetVideoBitRate failed");
            }
            Thread.Sleep(1200);
            int videoFrameRate = 30;
         //   int videoFrameRate = 3;
            Console.WriteLine("SCSetVideoFrameRate({0})", videoFrameRate);
            if (NativeMethods.SCSetVideoFrameRate(_deviceContext, videoFrameRate) != 1)
            {
                throw new Exception("SCSetVideoFrameRate failed");
            }
            Thread.Sleep(1200);
            int videoKeyFrameRate = 90;
          //  int videoKeyFrameRate = 9;
            Console.WriteLine("SCSetVideoGOP({0})", videoKeyFrameRate);
            if (NativeMethods.SCSetVideoGOP(_deviceContext, videoKeyFrameRate) != 1)
            {
                throw new Exception("SCSetVideoGOP failed");
            }
            Thread.Sleep(1200);
            int videoWidth = 320;
            int videoHeight = 240;
            Console.WriteLine("SCSetVideoFrameSize({0},{1})", videoWidth, videoHeight);
            if (NativeMethods.SCSetVideoFrameSize(_deviceContext, videoWidth, videoHeight) != 1)
            {
                throw new Exception("SCSetVideoFrameSize failed");
            }
            Thread.Sleep(1200);
            try
            {
                NativeMethods.SCClose(_deviceContext);
            }
            catch { }
            try
            {
                NativeMethods.SCClose(_deviceContext);
            }
            catch { }
        }

        internal class NativeMethods
        {
            public NativeMethods() { }

            [DllImport("SmartCaptureSDK.dll",EntryPoint="?SCOpen@@YAPAHH@Z")]
            public static extern IntPtr SCOpen(int deviceNumber);

            [DllImport("SmartCaptureSDK.dll",EntryPoint="?SCClose@@YAHPAH@Z")]
            public static extern void SCClose(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCClose@@YAXPAH@Z")]
            public static extern void SCClose2(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll",EntryPoint="?SCSetMediaType@@YAHPAHW4EncodingMediaType@@@Z")]
            public static extern int SCSetMediaType(IntPtr deviceContext, int AVSSelect);

            [DllImport("SmartCaptureSDK.dll",EntryPoint="?SCSetVideoBitRate@@YAHPAHI@Z")]
            public static extern int SCSetVideoBitRate(IntPtr deviceContext, int kbps);

            [DllImport("SmartCaptureSDK.dll",EntryPoint="?SCSetVideoFrameSize@@YAHPAHII@Z")]
            public static extern int SCSetVideoFrameSize(IntPtr deviceContext, int width, int height);

            [DllImport("SmartCaptureSDK.dll", EntryPoint="?SCSetVideoFrameRate@@YAHPAHI@Z")]
            public static extern int SCSetVideoFrameRate(IntPtr deviceContext, int fps);

            [DllImport("SmartCaptureSDK.dll", EntryPoint="?SCSetVideoGOP@@YAHPAHI@Z")]
            public static extern int SCSetVideoGOP(IntPtr deviceContext, int GOP);

            [DllImport("SmartCaptureSDK.dll", EntryPoint="?SCStartRecord@@YAHPAH@Z")]
            public static extern int SCStartRecord(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCStop@@YAHPAH@Z")]
            public static extern int SCStop(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCGetAccessUnit@@YAHPAHPAEAAI2@Z")]
            public static extern int SCGetAccessUnit(IntPtr deviceContext, IntPtr buffer, out int type, out int timestamp);
        }
    }
}
