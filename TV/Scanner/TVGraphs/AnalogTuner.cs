using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

using FutureConcepts.Media.DirectShowLib;

using FutureConcepts.Media.TV.Scanner.Config;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    public abstract class AnalogTuner : BaseLocalTuner
    {
        protected IAMTVTuner _amTvTuner;
        protected IAMCrossbar _amCrossbar;

        protected IBaseFilter _tuner;
        protected IBaseFilter _tvaudio;
        protected IBaseFilter _crossbar2;
        protected IBaseFilter _videoEncoder;
        protected IBaseFilter _audioEncoder;
        protected IBaseFilter _mpeg2Demux;
        protected IBaseFilter _leadYUVColorConverter;
        protected IBaseFilter _frameRateController;

        protected int _deviceIndex; //TODO evaluate if we need this variable

        protected DsDevice _captureDevice;
        protected DsDevice _tunerDevice;
        protected DsDevice _audioDevice;
        protected DsDevice _crossbarDevice;
        protected DsDevice _writerDevice;

        public AnalogTuner(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
            _curChannelIndex = AppUser.Current.ChannelIndex;

            _deviceIndex = 0;
            _captureDevice = FindDevice(FilterCategory.AMKSCapture, Config.FilterType.Capture);
            _tunerDevice = FindDevice(FilterCategory.AMKSTVTuner, Config.FilterType.Tuner);
            _audioDevice = FindDevice(FilterCategory.AMKSTVAudio, Config.FilterType.TVAudio);
            _crossbarDevice = FindDevice(FilterCategory.AMKSCrossbar, Config.FilterType.Crossbar);
            _writerDevice = FindDevice(FilterCategory.LegacyAmFilterCategory, Config.FilterType.Writer);

            if (_tunerDevice != null)
            {
                _tuner = AddFilterByDevicePath(_tunerDevice.DevicePath, _tunerDevice.Name);
            }
            if (_audioDevice != null)
            {
                _tvaudio = AddFilterByDevicePath(_audioDevice.DevicePath, _audioDevice.Name);
            }
            if (_crossbarDevice != null)
            {
                _crossbar2 = AddFilterByDevicePath(_crossbarDevice.DevicePath, _crossbarDevice.Name);
            }
            if (_captureDevice != null)
            {
                _captureFilter = AddFilterByDevicePath(_captureDevice.DevicePath, _captureDevice.Name);
            }

            AddAudioVolumeFilter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ForceReleaseComObject(_tuner);
                _tuner = null;
                ForceReleaseComObject(_tvaudio);
                _tvaudio = null;
                ForceReleaseComObject(_crossbar2);
                _crossbar2 = null;
                ForceReleaseComObject(_videoEncoder);
                _videoEncoder = null;
                ForceReleaseComObject(_audioEncoder);
                _audioEncoder = null;
                ForceReleaseComObject(_mpeg2Demux);
                _mpeg2Demux = null;
                ForceReleaseComObject(_leadYUVColorConverter);
                _leadYUVColorConverter = null;
                ForceReleaseComObject(_frameRateController);
                _frameRateController = null;
                ForceReleaseComObject(_audioVolumeFilter);
                _audioVolumeFilter = null;

                ForceReleaseComObject(_amTvTuner);
                _amTvTuner = null;
                ForceReleaseComObject(_amCrossbar);
                _amCrossbar = null;
            }

            base.Dispose(disposing);
        }

        #region TV Mode (Crossbar / Audio)

        private TVMode _currentTVMode;

        public override TVMode TVMode
        {
            get
            {
                return _currentTVMode;
            }
            set
            {
                _currentTVMode = value;

                //video
                RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Video_VideoDecoder,
                                    (value == TVMode.Broadcast) ? DirectShowLib.PhysicalConnectorType.Video_Tuner :
                                    (DirectShowLib.PhysicalConnectorType)AppUser.Current.SatelliteConnector);
                //audio
                RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Audio_AudioDecoder,
                                    (value == TVMode.Broadcast) ? DirectShowLib.PhysicalConnectorType.Audio_Tuner :
                                    DirectShowLib.PhysicalConnectorType.Audio_Line);
            }
        }

        #endregion

        #region ChannelControl

        private int _curChannelIndex = -1;
        /// <summary>
        /// Gets or sets the current channel index
        /// </summary>
        protected override int CurrentChannelIndex
        {
            get
            {
                if (_curChannelIndex != -1)
                {
                    return _curChannelIndex;
                }
                else
                {
                    return base.CurrentChannelIndex;
                }
            }
        }

        private Channel _channel;
        /// <summary>
        /// Retrieves the current channel of the Tuner, or attempts to change the channel.
        /// Reverts to previous channel if a failure occurs while changing the channel
        /// </summary>
        /// <exception cref="FutureConcepts.Media.TV.Scanner.NoSignalPresentException">Thrown if changing the channel was not successful.</exception>
        /// <remarks>
        /// If the ChannelForceChange property is true, the exception is not thrown, and the channel
        /// is not reverted to upon failure.
        /// </remarks>
        public override Channel Channel
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                _curChannelIndex = -1;  //reset to auto-determine

                //perform the logical/physical mapping for analog.
                if (value.PhysicalChannel == -1)
                {
                    value.PhysicalChannel = value.MajorChannel;
                }
                value.MinorChannel = -1;

                if (_amTvTuner != null)
                {
                    int hr = _amTvTuner.put_Channel(value.PhysicalChannel, 0, 0);
                    //if the channel change was successful
                    if (hr == 0)
                    {
                        _channel = value;
                        KnownChannels.Add(value);
                        NotifyPropertyChanged("Channel");
                    }
                    else    //else if no signal present on new channel, or failure, revert channel
                    {
                        if (ChannelForceChange)
                        {
                            _channel = value;
                            NotifyPropertyChanged("Channel");
                        }
                        else
                        {
                            //we are not forcing change, so on failure, revert to the last-known channel
                            _amTvTuner.put_Channel(_channel.PhysicalChannel, 0, 0);
                            throw new NoSignalPresentException(value);
                        }
                    }
                }
            }
            get
            {
                if (_amTvTuner != null)
                {
                    int channel;
                    AMTunerSubChannel vid, aud;
                    int hr = _amTvTuner.get_Channel(out channel, out vid, out aud);
                    if (hr == 0)
                    {
                        return new Channel(channel);
                    }
                }
                return _channel;
            }
        }

        public override void ChannelUp()
        {
            if (KnownChannels.Count == 0)
            {
                Channel up = new Channel(this.Channel.PhysicalChannel + 1);
                if (up.CompareTo(this.MaxChannel) > 0)
                {
                    up = this.MinChannel;
                }
                this.Channel = up;
            }
            else
            {
                base.ChannelUp();
            }
        }

        public override void ChannelDown()
        {
            if (KnownChannels.Count == 0)
            {
                Channel down = new Channel(this.Channel.PhysicalChannel - 1);
                if (down.CompareTo(this.MinChannel) < 0)
                {
                    down = this.MaxChannel;
                }
                this.Channel = down;
            }
            else
            {
                base.ChannelDown();
            }
        }

        public override Channel MinChannel
        {
            get
            {
                int min;
                int max;
                int hr = _amTvTuner.ChannelMinMax(out min, out max);
                DsError.ThrowExceptionForHR(hr);
                return new Channel(min);
            }
        }

        public override Channel MaxChannel
        {
            get
            {
                int min;
                int max;
                int hr = _amTvTuner.ChannelMinMax(out min, out max);
                DsError.ThrowExceptionForHR(hr);
                return new Channel(max);
            }
        }

        public override Channel TryChannel
        {
            set
            {
                int hr = _amTvTuner.put_Channel(value.PhysicalChannel, 0, 0);
                if (hr == 0)
                {
                    NotifyPropertyChanged("Channel");
                }
            }
            get
            {
                int channel;
                AMTunerSubChannel v, a;
                int hr = _amTvTuner.get_Channel(out channel, out v, out a);
                if (hr == 0)
                {
                    return new Channel(channel);
                }
                else
                {
                    return null;
                }
            }
        }

        public override int GetSignalStrength()
        {
            if (_amTvTuner != null)
            {
                int hr;
                AMTunerSignalStrength signalStrength;
                hr = _amTvTuner.SignalPresent(out signalStrength);
                DsError.ThrowExceptionForHR(hr);
                return (int)signalStrength;
            }
            else
            {
                return (int)AMTunerSignalStrength.HasNoSignalStrength;
            }
        }

        #endregion

        protected void ConfigTV()
        {
            int hr;
            object o;

            hr = _captureGraphBuilder.FindInterface(null, null, _captureFilter, typeof(IAMTVTuner).GUID, out o);
            if (hr >= 0)
            {
                _amTvTuner = (IAMTVTuner)o;
                hr = _amTvTuner.put_InputType(0, AppUser.Current.TunerInputType);
                DsError.ThrowExceptionForHR(hr);
                o = null;
                hr = _captureGraphBuilder.FindInterface(null, null, _captureFilter, typeof(IAMCrossbar).GUID, out o);
                if (hr >= 0)
                {
                    _amCrossbar = (IAMCrossbar)o;
                    o = null;
                }
                else
                {
                    throw new Exception("IAMCrossbar interface not found");
                }
            }
            else
            {
                throw new Exception("IAMTVTuner interface not found");
            }

            hr = _amTvTuner.put_InputType(0, AppUser.Current.TunerInputType);
            DsError.ThrowExceptionForHR(hr);
        }

        private object getStreamConfigSetting(IAMStreamConfig streamConfig, string fieldName)
        {
            object returnValue = null;
            try
            {
                if (streamConfig == null)
                    throw new NotSupportedException();

                IntPtr pmt = IntPtr.Zero;
                AMMediaType mediaType = new AMMediaType();

                try
                {
                    // Get the current format info
                    mediaType.formatType = FormatType.VideoInfo2;
                    int hr = streamConfig.GetFormat(out mediaType);
                    if (hr != 0)
                    {
                        Console.WriteLine("VideoCaptureDevice:getStreamConfigSetting() FAILED to get:{0} (not supported)", fieldName);
                        Marshal.ThrowExceptionForHR(hr);
                    }
                    // The formatPtr member points to different structures
                    // dependingon the formatType
                    object formatStruct;
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() find formattype"); 
                    if (mediaType.formatType == FormatType.WaveEx)
                        formatStruct = new WaveFormatEx();
                    else if (mediaType.formatType == FormatType.VideoInfo)
                        formatStruct = new VideoInfoHeader();
                    else if (mediaType.formatType == FormatType.VideoInfo2)
                        formatStruct = new VideoInfoHeader2();
           //         else if (mediaType.formatType == FormatType.Mpeg2Video)
           //             formatStruct = new MPEG2VideoInfo();
                    else if (mediaType.formatType == FormatType.None)
                    {
                        //Log.Info("VideoCaptureDevice:getStreamConfigSetting() FAILED no format returned");
                        //throw new NotSupportedException("This device does not support a recognized format block.");
                        return null;
                    }
                    else
                    {
                        //Log.Info("VideoCaptureDevice:getStreamConfigSetting() FAILED unknown fmt:{0} {1} {2}", mediaType.formatType, mediaType.majorType, mediaType.subType);
                        //throw new NotSupportedException("This device does not support a recognized format block.");
                        return null;
                    }

                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() get formatptr");
                    // Retrieve the nested structure
                    Marshal.PtrToStructure(mediaType.formatPtr, formatStruct);

                    // Find the required field
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() get field");
                    Type structType = formatStruct.GetType();
                    FieldInfo fieldInfo = structType.GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        //Log.Info("VideoCaptureDevice.getStreamConfigSetting() FAILED to to find member:{0}", fieldName);
                        //throw new NotSupportedException("VideoCaptureDevice:FAILED to find the member '" + fieldName + "' in the format block.");
                        return null;
                    }

                    // Extract the field's current value
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() get value");
                    returnValue = fieldInfo.GetValue(formatStruct);
                    //Log.Info("  VideoCaptureDevice.getStreamConfigSetting() done");	
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pmt);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("  VideoCaptureDevice.getStreamConfigSetting() FAILED ");
            }
            return (returnValue);
        }

        private object setStreamConfigSetting(IAMStreamConfig streamConfig, string fieldName, object newValue)
        {
            try
            {
                object returnValue = null;
                IntPtr pmt = IntPtr.Zero;
                AMMediaType mediaType = new AMMediaType();

                try
                {
                    // Get the current format info
                    int hr = streamConfig.GetFormat(out mediaType);
                    if (hr != 0)
                    {
                        Console.WriteLine("  VideoCaptureDevice:setStreamConfigSetting() FAILED to set:{0} (getformat) hr:{1}", fieldName, hr);
                        return null;//Marshal.ThrowExceptionForHR(hr);
                    }
                    //Log.Info("  VideoCaptureDevice:setStreamConfigSetting() get formattype");
                    // The formatPtr member points to different structures
                    // dependingon the formatType
                    object formatStruct;
                    if (mediaType.formatType == FormatType.WaveEx)
                        formatStruct = new WaveFormatEx();
                    else if (mediaType.formatType == FormatType.VideoInfo)
                        formatStruct = new VideoInfoHeader();
                    else if (mediaType.formatType == FormatType.VideoInfo2)
                        formatStruct = new VideoInfoHeader2();
           //         else if (mediaType.formatType == FormatType.Mpeg2Video)
           //             formatStruct = new MPEG2VideoInfo();
                    else if (mediaType.formatType == FormatType.None)
                    {
                        Console.WriteLine("  VideoCaptureDevice:setStreamConfigSetting() FAILED no format returned");
                        return null;// throw new NotSupportedException("This device does not support a recognized format block.");
                    }
                    else
                    {
                        Console.WriteLine("  VideoCaptureDevice:setStreamConfigSetting() FAILED unknown fmt");
                        return null;//throw new NotSupportedException("This device does not support a recognized format block.");
                    }
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() get formatptr");
                    // Retrieve the nested structure
                    Marshal.PtrToStructure(mediaType.formatPtr, formatStruct);

                    // Find the required field
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() get field");
                    Type structType = formatStruct.GetType();
                    FieldInfo fieldInfo = structType.GetField(fieldName);
                    if (fieldInfo == null)
                    {
                        Console.WriteLine("  VideoCaptureDevice:setStreamConfigSetting() FAILED to to find member:{0}", fieldName);
                        throw new NotSupportedException("FAILED to find the member '" + fieldName + "' in the format block.");
                    }
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() set value");
                    // Update the value of the field
                    fieldInfo.SetValue(formatStruct, newValue);

                    // PtrToStructure copies the data so we need to copy it back
                    Marshal.StructureToPtr(formatStruct, mediaType.formatPtr, false);

                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() set format");
                    // Save the changes
                    hr = streamConfig.SetFormat(mediaType);
                    if (hr != 0)
                    {
                        Console.WriteLine("  VideoCaptureDevice:setStreamConfigSetting() FAILED to set:{0} {1}", fieldName, hr);
                        return null;//Marshal.ThrowExceptionForHR(hr);
                    }
                    //else Log.Info("  VideoCaptureDevice.setStreamConfigSetting() set:{0}",fieldName);
                    //Log.Info("  VideoCaptureDevice.setStreamConfigSetting() done");
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pmt);
                }
                return (returnValue);
            }
            catch (Exception)
            {
                Console.WriteLine("  VideoCaptureDevice.:setStreamConfigSetting() FAILED ");
            }
            return null;
        }

        private Size FrameSize
        {
            get
            {
                if (_interfaceStreamConfigVideoCapture != null)
                {
                    try
                    {
                        BitmapInfoHeader bmiHeader;
                        object obj = getStreamConfigSetting(_interfaceStreamConfigVideoCapture, "BmiHeader");
                        if (obj != null)
                        {
                            bmiHeader = (BitmapInfoHeader)obj;
                            return new Size(bmiHeader.Width, bmiHeader.Height);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return new Size(720, 576);
            }
            set
            {
                if (value.Width > 0 && value.Height > 0)
                {
                    if (_interfaceStreamConfigVideoCapture != null)
                    {
                        try
                        {
                            BitmapInfoHeader bmiHeader;
                            object obj = getStreamConfigSetting(_interfaceStreamConfigVideoCapture, "BmiHeader");
                            if (obj != null)
                            {
                                bmiHeader = (BitmapInfoHeader)obj;
                                Console.WriteLine("VideoCaptureDevice:change capture Framesize :{0}x{1} ->{2}x{3}", bmiHeader.Width, bmiHeader.Height, FrameSize.Width, FrameSize.Height);
                                bmiHeader.Width = value.Width;
                                bmiHeader.Height = value.Height;
                                setStreamConfigSetting(_interfaceStreamConfigVideoCapture, "BmiHeader", bmiHeader);
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("VideoCaptureDevice:FAILED:could not set capture  Framesize to {0}x{1}!", FrameSize.Width, FrameSize.Height);
                        }
                    }
                }
            }
        }

        private double FrameRate
        {
            get
            {
                if (_interfaceStreamConfigVideoCapture != null)
                {
                    try
                    {
                        object obj = getStreamConfigSetting(_interfaceStreamConfigVideoCapture, "AvgFrameRate");
                        if (obj != null)
                        {
                            long frameRate = (long)(10000000d / (double)obj);
                            return frameRate;
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("SteamingTVGraph.FrameRate exception: {0}", exc.Message);
                    }
                }
                return 0;
            }
            set
            {
                if (value >= 1d && value <= 30d)
                {
                    if (_interfaceStreamConfigVideoCapture != null)
                    {
                        try
                        {
                            Console.WriteLine("SWGraph:capture FrameRate set to {0}", value);
                            long avgTimePerFrame = (long)(10000000d / value);
                            setStreamConfigSetting(_interfaceStreamConfigVideoCapture, "AvgTimePerFrame", avgTimePerFrame);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("StreamingTVGraph.FrameRate threw exception: {0}", exc.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows other software to display the crossbar filter settings
        /// </summary>
        /// <remarks>
        /// Do not de-allocate this filter!!
        /// </remarks>
        public IBaseFilter CrossbarFilter
        {
            get { return _crossbar2; }
            set { _crossbar2 = value; }
        }


        private IAMStreamConfig _interfaceStreamConfigVideoCapture = null;
/*
        public void SetupCaptureFormat(IBaseFilter filter)
        {
            Console.WriteLine("VideoCaptureDevice:get Video stream control interface (IAMStreamConfig)");
            object o;
            int hr = _captureGraphBuilder.FindInterface(PinCategory.Preview, null, (IBaseFilter)filter, typeof(IAMStreamConfig).GUID, out o);
            if (hr == 0)
            {
                _interfaceStreamConfigVideoCapture = o as IAMStreamConfig;
                if (_interfaceStreamConfigVideoCapture != null)
                {
                    Console.WriteLine("FrameRate before set {0}", FrameRate);
                    FrameRate = 15d;
                    //FrameSize = new Size(720, 576);
                   // Size size = FrameSize;
                   // if (size.Width != 720 || size.Height != 576)
                  //  {
                      //  FrameSize = new Size(640, 480);
                        //   FrameSize = new Size(352, 240);
                   // }
                }
            }
            else
            {
                Console.WriteLine("Failed to find Preview interface on filter");
            }
            Console.WriteLine("FrameRate after set {0}", FrameRate);
            return;
        }
*/

        /// <summary>
        /// Finds the specified crossbar pin
        /// </summary>
        /// <param name="PhysicalType">physical type of connector to locate</param>
        /// <param name="bInput">true if this is an input, or false for an output</param>
        /// <returns>the index of the desired pin, or -1 if not found.</returns>
        protected int FindCrossbarPin(DirectShowLib.PhysicalConnectorType PhysicalType, bool bInput)
        {
            int hr;

            int cOut, cIn;
            hr = _amCrossbar.get_PinCounts(out cOut, out cIn);
            DsError.ThrowExceptionForHR(hr);
            // Enumerate pins and look for a matching pin.
            int count = (bInput ? cIn : cOut);
            for (int i = 0; i < count; i++)
            {
                int iRelated = 0;
                DirectShowLib.PhysicalConnectorType ThisPhysicalType = 0;
                hr = _amCrossbar.get_CrossbarPinInfo(bInput, i, out iRelated, out ThisPhysicalType);
                DsError.ThrowExceptionForHR(hr);
                if (ThisPhysicalType == PhysicalType)
                {
                    return i;
                }
            }
            return (-1);
        }

        /// <summary>
        /// Performs the specified routing
        /// </summary>
        /// <param name="outputConnectorType">the output connector type to find and route the input to</param>
        /// <param name="inputConnectorType">the input connector type to find and route to the output</param>
        protected void RouteCrossbarPins(DirectShowLib.PhysicalConnectorType outputConnectorType, DirectShowLib.PhysicalConnectorType inputConnectorType)
        {
            int hr;

            if (_amCrossbar != null)
            {
                int i;
                int j;
                i = FindCrossbarPin(outputConnectorType, false);
                if (i != -1)
                {
                    j = FindCrossbarPin(inputConnectorType, true);
                    if (j != -1)
                    {
                        hr = _amCrossbar.Route(i, j);
                        DsError.ThrowExceptionForHR(hr);
                    }
                }
            }
        }

        protected DsDevice FindDevice(Guid filterCategory, Config.FilterType filterType)
        {
            List<DsDevice> deviceList = new List<DsDevice>();

            if (GraphConfig[filterType] == null)
            {
                throw new ConfigurationErrorsException("No filter defined for type " + filterType);
            }
            string friendlyName = GraphConfig[filterType].Name;

            foreach (DsDevice device in DsDevice.GetDevicesOfCat(filterCategory))
            {
                if (device.Name != null)
                {
                    if (device.Name == friendlyName)
                    {
                        deviceList.Add(device);
                    }
                }
            }
            if (deviceList.Count > _deviceIndex)
            {
                return deviceList[_deviceIndex];
            }
            else
            {
                throw new ConfigurationErrorsException("Instance " + _deviceIndex + " of " + friendlyName + " not found.");
            }
        }
    }
}
