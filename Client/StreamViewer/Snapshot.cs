using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// Captures and encapsulates a frame from a VMR
    /// </summary>
    public class Snapshot : IDisposable
    {
        private IntPtr _dib = IntPtr.Zero;

        /// <summary>
        /// Create an instance of the current frame, as taken from a VMR7
        /// </summary>
        /// <param name="control">VMR 7 windowless interface</param>
        public Snapshot(IVMRWindowlessControl control)
        {
            int hr;

            hr = control.GetCurrentImage(out _dib);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Create an instance of the current frame, as taken from a VMR9
        /// </summary>
        /// <param name="control">VMR 9 windowless interface</param>
        public Snapshot(IVMRWindowlessControl9 control)
        {
            int hr;

            hr = control.GetCurrentImage(out _dib);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Disposes of the image
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
        /// <summary>
        /// Retreives the Image this snapshot contains. Returns null if an error occurs.
        /// </summary>
        public Image Image
        {
            get
            {
                if (_bitmap == null)
                {
                    _bitmap = BitmapFromDIB(_dib);
                }
                return _bitmap;
            }
        }

        /// <summary>
        /// Returns a scaled version of the image/snapshot.
        /// </summary>
        /// <param name="size">the desired dimensions of the image</param>
        /// <returns>Returns null if error</returns>
        public Image GetImageScaled(Size size)
        {
            System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
            if (this.Image == null)
            {
                return null;
            }
            else
            {
                return this.Image.GetThumbnailImage(size.Width, size.Height, myCallback, IntPtr.Zero);
            }
        }

        private static Bitmap BitmapFromDIB(IntPtr pDIB)
        {
            // get pointer to DIB pixels
            IntPtr pPix = new IntPtr(pDIB.ToInt32() + Marshal.SizeOf(typeof(NativeMethods.BITMAPINFOHEADER)));

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

        private bool ThumbnailCallback()
        {
            return false;
        }

        internal static class NativeMethods
        {       
            [DllImport("GdiPlus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern int GdipCreateBitmapFromGdiDib(IntPtr pBIH, IntPtr pPix, out IntPtr pBitmap);

            [DllImport("ole32.dll")]
            public static extern void CoTaskMemFree(IntPtr pv);

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
        }
    }       
}
