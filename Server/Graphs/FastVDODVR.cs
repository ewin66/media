using System;
using System.Runtime.InteropServices;
using System.Threading;
using FutureConcepts.Media;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.Graphs
{
    /// <summary>
    /// Provides support for FastVDO SmartCapture
    /// </summary>
    /// <author>
    /// </author>
    public class FastVDODVR : BaseDVRSinkGraph
    {
        private IBaseFilter _transcoder;

        private IntPtr _deviceContext = IntPtr.Zero;

        public FastVDODVR(StreamSourceInfo streamSourceInfo, OpenGraphRequest openGraphRequest)
            : base(streamSourceInfo, openGraphRequest)
        {
            //maximum number of retries before blowing away graph/connection
            int MaxRetries = 1;
            for (int retry = 1; retry <= MaxRetries; retry++)
            {
                try
                {
                    InitializeSmartCapture(CurrentProfile);
                    InitializeSink();
                    _captureFilter = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "FastVDO-SmartCapture QBoxSplitter");
                    _transcoder = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "H.264 Byte Stream Transform");
                    AppLogger.Message("Attempting to connect capture filter (Video) to transform (XForm In)");
                    ConnectFilters(_captureFilter, "Video", _transcoder, "XForm In");
                    AppLogger.Message("Attempting to connect transform (XForm Out) to mux (Input 01)");
                    ConnectFilterToMux(_transcoder, "XForm Out", "Input 01");
                    ConnectMuxToSink();
                    break;  //this worked, so stop retrying
                }
                catch (Exception exc)
                {
                    ErrorLogger.DumpToDebug(exc);
                    if (_deviceContext != IntPtr.Zero)
                    {
                        try
                        {
                            AppLogger.Message("Resetting FVDO device");
                            NativeMethods.SCReset(_deviceContext);
                            Thread.Sleep(2000);
                        }
                        catch (Exception exc2)
                        {
                            ErrorLogger.DumpToDebug(exc2);
                        }
                    }
                    //on the last retry, give up
                    if (retry == MaxRetries)
                    {
                        this.Dispose();
                        throw;
                    }
                    else
                    {
                        CloseSmartCapture();
                    }
                }
            }
        }

        public override void Dispose(bool disposing)
        {
            CloseSmartCapture();
            if (_captureFilter != null)
            {
                Marshal.ReleaseComObject(_captureFilter);
                Marshal.ReleaseComObject(_captureFilter);
                _captureFilter = null;
            }
            if (_transcoder != null)
            {
                Marshal.ReleaseComObject(_transcoder);
                Marshal.ReleaseComObject(_transcoder);
                _transcoder = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Stops the device and Closes the device context
        /// </summary>
        private void CloseSmartCapture()
        {
            if (_deviceContext != IntPtr.Zero)
            {
                NativeMethods.SCStop(_deviceContext);
                Thread.Sleep(200);
                try
                {
                    NativeMethods.SCClose(_deviceContext);
                }
                catch { }
                try
                {
                    NativeMethods.SCClose2(_deviceContext);
                }
                catch { }
                _deviceContext = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Fetches device context 
        /// </summary>
        private void InitializeSmartCapture(Profile profile)
        {
            AppLogger.Message("PushFastVDO.InitializeSmartCapture Begin");
            _deviceContext = NativeMethods.SCOpen(1);
            if (_deviceContext == IntPtr.Zero)
            {
                throw new Exception("bad device context");
            }
            Thread.Sleep(5000);
            AppLogger.Message("Resetting FVDO device");
            NativeMethods.SCReset(_deviceContext);
            Thread.Sleep(5000);
            AppLogger.Message(@"SmartCaptureSDK.SCStop()");
            NativeMethods.SCStop(_deviceContext); // per FastVDO e-mail dated 3/24/2008
            Thread.Sleep(5000);
            if (NativeMethods.SCSetMediaType(_deviceContext, 1) != 1)
            {
                throw new Exception("SCSetMediaType failed");
            }
            Thread.Sleep(1000);
            AppLogger.Message("PushFastVDO.InitializeSmartCapture END");
            ChangeProfile(profile);
        }

        /// <summary>
        /// Changes the profile for the graph
        /// </summary>
        /// <param name="newProfile">target profile</param>
        /// <exception cref="FutureConcepts.Media.ServerGraphRebuildException">
        /// Thrown if the server graph must be rebuilt to implement a change
        /// </exception>
        public override void ChangeProfile(Profile profile)
        {
            AppLogger.Message("PushFastVDO.ChangeProfile Begin");
            if (_deviceContext == IntPtr.Zero)
            {
                throw new ArgumentNullException("DeviceContext");
            }
            if (profile.Video == null)
            {
                AppLogger.Message("PushFastVDO.ChangeProfile called but newProfile.Video==null");
                return;
            }
            if (profile.Video.FrameRateUnits != VideoFrameRateUnits.FramesPerSecond)
            {
                throw new UnsupportedFrameRateUnitsException(profile.Video.FrameRateUnits);
            }
            if (profile.Video.FrameRate == 1)
            {
                throw new ArgumentOutOfRangeException("Profile.Video.FrameRate", "1FPS is not supported by this device!");
            }
            if (profile.Video.CodecType != VideoCodecType.H264)
            {
                throw new ArgumentOutOfRangeException("Profile.Video.CodecType", "only H.264 is supported!");
            }
            SetImageSize(profile.Video.ImageSize);
            if (NativeMethods.SCSetVideoFrameRate(_deviceContext, profile.Video.FrameRate) != 1)
            {
                throw new Exception("PushFastVDO.ChangeProfile SCSetVideoFrameRate failed");
            }
            AppLogger.Message(String.Format("FrameRate {0}", profile.Video.FrameRate));
            Thread.Sleep(300);
            if (NativeMethods.SCSetVideoGOP(_deviceContext, profile.Video.KeyFrameRate) != 1)
            {
                throw new Exception("PushFastVDO.ChangeProfile SCSetVideoGOP failed");
            }
            AppLogger.Message(String.Format("KeyFrameRate {0}", profile.Video.KeyFrameRate));
            Thread.Sleep(300);
            if (NativeMethods.SCSetVideoBitRate(_deviceContext, profile.Video.ConstantBitRate) != 1)
            {
                throw new Exception("PushFastVDO.ChangeProfile SCSetVideoBitRate failed");
            }
            AppLogger.Message(String.Format("VideoBitRate {0}", profile.Video.ConstantBitRate));
        }

        /// <summary>
        /// Configures the SmartCapture device to output the given image size.
        /// </summary>
        /// <param name="size">Supports CIF and 4CIF</param>
        private void SetImageSize(VideoImageSize size)
        {
            if (_deviceContext == IntPtr.Zero)
            {
                throw new ArgumentNullException("DeviceContext");
            }

            int width;
            int height;

            switch (size)
            {
                case VideoImageSize.CIF:
                    width = 352;
                    height = 240;
                    AppLogger.Message("ImageSize CIF");
                    break;
                case VideoImageSize.FourCIF:
                    width = 704;
                    height = 480;
                    AppLogger.Message("ImageSize FourCIF");
                    break;
                default:
                    throw new ArgumentException(size.ToString(), "ImageSize");
            }
            if (NativeMethods.SCSetVideoFrameSize(_deviceContext, width, height) != 1)
            {
                throw new Exception("SCSetVideoFrameSize failed");
            }
            Thread.Sleep(300);
        }

        private class NativeMethods
        {
            public NativeMethods() { }

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCOpen@@YAPAHH@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr SCOpen(int deviceNumber);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCClose@@YAHPAH@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SCClose(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCClose@@YAXPAH@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SCClose2(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCSetMediaType@@YAHPAHW4EncodingMediaType@@@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCSetMediaType(IntPtr deviceContext, int AVSSelect);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCSetVideoBitRate@@YAHPAHI@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCSetVideoBitRate(IntPtr deviceContext, int kbps);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCSetVideoFrameSize@@YAHPAHII@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCSetVideoFrameSize(IntPtr deviceContext, int width, int height);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCSetVideoFrameRate@@YAHPAHI@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCSetVideoFrameRate(IntPtr deviceContext, int fps);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCSetVideoGOP@@YAHPAHI@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCSetVideoGOP(IntPtr deviceContext, int GOP);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCStartRecord@@YAHPAH@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCStartRecord(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCStop@@YAHPAH@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCStop(IntPtr deviceContext);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCGetAccessUnit@@YAHPAHPAEAAI2@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCGetAccessUnit(IntPtr deviceContext, IntPtr buffer, out int type, out int timestamp);

            [DllImport("SmartCaptureSDK.dll", EntryPoint = "?SCReset@@YAXPAH@Z", CallingConvention = CallingConvention.Cdecl)]
            public static extern int SCReset(IntPtr deviceContext);
        }
    }
}
