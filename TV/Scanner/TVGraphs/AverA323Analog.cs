using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.TV.Scanner.Config;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    /// <summary>
    /// Analog/WDM implementation for AVerMedia A323 card
    /// </summary>
    class AverA323Analog : AnalogTuner
    {
        protected DsDevice _encoderDevice;
        protected IBaseFilter _timeStampAdjustFilter;
        protected IBaseFilter _smartTeeFilter;

        public AverA323Analog(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
            CaptureRequiresNewGraph = true;
        }

        public override void Render()
        {
            if (_tuner == null)
                throw new ArgumentNullException("_tuner", "No Tuner Device present");

            _timeStampAdjustFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "FC Time Stamp Adjust");
            _smartTeeFilter = FilterGraphTools.AddFilterByName(_graphBuilder, FilterCategory.LegacyAmFilterCategory, "Smart Tee");

            ConnectFilters(_tuner, "Analog Video", _crossbar2, "0: Video Tuner In", true);
            ConnectFilters(_tuner, "Analog Audio", _tvaudio, "TVAudio In", true);
            ConnectFilters(_tvaudio, "TVAudio Out", _crossbar2, "3: Audio Tuner In", true);

            ConnectFilters(_crossbar2, "0: Video Decoder Out", _captureFilter, "Analog Video In", true);
            ConnectFilters(_crossbar2, "1: Audio Decoder Out", _captureFilter, "Analog Audio In", true);
//            ConnectFilters(_captureFilter, "Capture",  _videoRender, "VMR Input0", true);
            ConnectFilters(_captureFilter, "Capture", _timeStampAdjustFilter, "Input", true);
            ConnectFilters(_timeStampAdjustFilter, "Output", _smartTeeFilter, "Input", true);
            ConnectFilters(_smartTeeFilter, "Capture", _videoRender, "VMR Input0", true);

            AddAudioRenderer();

            ConnectToAudio(_captureFilter, "Audio", true);

            ConfigTV();

            RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Video_VideoDecoder, DirectShowLib.PhysicalConnectorType.Video_Tuner);
            RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Audio_AudioDecoder, DirectShowLib.PhysicalConnectorType.Audio_Tuner);

            this.SaveGraphFile();
            CaptureRequiresNewGraph = false;

            base.Render();
        }

        public override void RenderCapture(string fileName)
        {
            //AVerMedia Encoder
            //ITU Video--|
            //           |--MPEG2 Program
            //I2S Audio--|

            _encoderDevice = FindDevice(FilterCategory.WDMStreamingEncoderDevices, Config.FilterType.VideoEncoder);

            if (_encoderDevice == null)
            {
                throw new Exception("Encoder device \"" + GraphConfig[FilterType.VideoEncoder].Name + "\" could not be loaded!");
            }
            if (_writerDevice == null)
            {
                throw new Exception("Writer device \"" + GraphConfig[FilterType.Writer].Name + "\" could not be loaded!");
            }

            //get filters from devices
            _videoEncoder = AddFilterByDevicePath(_encoderDevice.DevicePath, _encoderDevice.Name);
            _fileWriter = AddFilterByDevicePath(_writerDevice.DevicePath, _writerDevice.Name);

            ConnectFilters(_captureFilter, "ITU Video", _videoEncoder, "ITU Video", false);
            ConnectFilters(_captureFilter, "I2S Audio", _videoEncoder, "I2S Audio", false);

            ConnectFilters(_videoEncoder, "MPEG2 Program", _fileWriter, "Input", false);

            //configure File Sink / File Writer
            IFileSinkFilter fileSink = (IFileSinkFilter)_fileWriter;
            int hr = fileSink.SetFileName(fileName + ".mpg", _emptyAMMediaType);
            if (hr != 0)
            {
                this.CaptureRequiresNewGraph = true;
            }

            DsError.ThrowExceptionForHR(hr);

            StartDVRWriter(_fileWriter);

            this.SaveGraphFile();
        }
    }
}
