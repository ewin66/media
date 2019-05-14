using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.SystemTools.Settings.AntaresX.AntaresXSettings;

namespace FutureConcepts.Media.TV.Scanner
{
    public class Snapshot : IDisposable
    {
        private IntPtr _dib = IntPtr.Zero;

        /// <summary>
        /// Creates a blank snapshot, with a null image
        /// </summary>
        public Snapshot()
        {
        }

        /// <summary>
        /// Creates a Snapshot from a windowless control
        /// </summary>
        /// <param name="control">control that is being drawn to by the VMR7</param>
        public Snapshot(IVMRWindowlessControl control)
        {
            int hr;

            hr = control.GetCurrentImage(out _dib);
            DsError.ThrowExceptionForHR(hr);
        }
        
        /// <summary>
        /// Creates a Snapshot from a windowless control
        /// </summary>
        /// <param name="control">control that is being drawn to by the VMR9</param>
        public Snapshot(IVMRWindowlessControl9 control)
        {
            int hr;

            hr = control.GetCurrentImage(out _dib);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Releases an un-managed memory
        /// </summary>
        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            if (_dib != IntPtr.Zero)
            {
                NativeMethods.CoTaskMemFree(_dib);
                _dib = IntPtr.Zero;
            }
        }

        private Image _bitmap = null;
        public Image DIBBitmap
        {
            get
            {
                if (_bitmap == null)
                {
                    _bitmap = BitmapFromDIB(_dib);
                    if (_bitmap == null)
                    {
                        return null;
                    }
                }
                if (_bitmap.Width < 640)
                {
                    Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    return _bitmap.GetThumbnailImage(640, 480, myCallback, IntPtr.Zero);
                }
                else
                {
                    return _bitmap;
                }
            }
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        private static Bitmap BitmapFromDIB(IntPtr pDIB)
        {
            // get pointer to DIB pixels
            IntPtr pPix = new IntPtr(pDIB.ToInt32() + Marshal.SizeOf(typeof(BITMAPINFOHEADER)));

            MethodInfo mi = typeof(Bitmap).GetMethod("FromGDIplus", BindingFlags.Static | BindingFlags.NonPublic);

            if (mi == null)
            {
                return null; // (permission problem) 
            }

            IntPtr pBmp = IntPtr.Zero;
            int status = NativeMethods.GdipCreateBitmapFromGdiDib(pDIB, pPix, out pBmp);

            if ((status == 0) && (pBmp != IntPtr.Zero)) // success 
            {
                return (Bitmap)mi.Invoke(null, new object[] { pBmp });
            }
            else
            {
                return null; // failure 
            }
        }

        public void SaveAsFile(string filename)
        {
            ApplicationSettings settings = new ApplicationSettings();
            string dirPath = settings.Snapshots + DateTime.Now.ToString("MMddyy") + @"\";
            Directory.CreateDirectory(dirPath);
            DIBBitmap.Save(dirPath + filename, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        public bool ThumbnailCallback()
        {
            return false;
        }

        internal class NativeMethods
        {
            public NativeMethods() {}
        
            [DllImport("GdiPlus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern int GdipCreateBitmapFromGdiDib(IntPtr pBIH, IntPtr pPix, out IntPtr pBitmap);

            [DllImport("ole32.dll")]
            public static extern void CoTaskMemFree(IntPtr pv);
        }
    }       
}
