using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    /// <summary>
    /// Analog/WDM implementation for AVerMedia A307 card
    /// </summary>
    class AverA307Analog : AnalogTuner
    {
        public AverA307Analog(Config.Graph sourceConfig, Control hostControl) : base(sourceConfig, hostControl)
        {
        }

        public override void Render()
        {
            if (_tuner == null)
                throw new ArgumentNullException("_tuner", "No Tuner Device present");


            ConnectFilters(_tuner, "Analog Video", _crossbar2, "0: Video Tuner In", true);
            ConnectFilters(_tuner, "Analog Audio", _tvaudio, "TVAudio In", true);
            ConnectFilters(_tvaudio, "TVAudio Out", _crossbar2, "3: Audio Tuner In", true);

            ConnectFilters(_crossbar2, "0: Video Decoder Out", _captureFilter, "Analog Video In", true);
            ConnectFilters(_crossbar2, "1: Audio Decoder Out", _captureFilter, "Analog Audio In", true);
            ConnectFilters(_captureFilter, "Capture",  _videoRender, "VMR Input0", true);

            AddAudioRenderer();

            ConnectToAudio(_captureFilter, "Audio Out", true);

            ConfigTV();

            RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Video_VideoDecoder, DirectShowLib.PhysicalConnectorType.Video_Tuner);
            RouteCrossbarPins(DirectShowLib.PhysicalConnectorType.Audio_AudioDecoder, DirectShowLib.PhysicalConnectorType.Audio_Tuner);

            this.SaveGraphFile();
            CaptureRequiresNewGraph = false;

            base.Render();
        }

        public override void RenderCapture(string fileName)
        {
            throw new NotSupportedException();
        }
    }
}
