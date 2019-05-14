using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class WinTV418: WinTVAnalogBase
    {
        public WinTV418(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            _captureDevice = FindDevice(FilterCategory.AMKSCapture, @"Hauppauge WinTV 418 Video Capture");
            _tunerDevice = FindDevice(FilterCategory.AMKSTVTuner, @"Hauppauge WinTV 418 Tuner");
            _audioDevice = FindDevice(FilterCategory.AMKSTVAudio, @"Hauppauge WinTV 418 TvAudio");
            _crossbarDevice = FindDevice(FilterCategory.AMKSCrossbar, @"Hauppauge WinTV 418 Crossbar");
            _encoderDevice = FindDevice(FilterCategory.WDMStreamingEncoderDevices, @"Hauppauge WinTV 418 Encoder");
            CreateWinTVGraph();
        }
    }
}
