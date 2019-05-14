using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class WinTV500: WinTVAnalogBase
    {
        public WinTV500(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            _captureDevice = FindDevice(FilterCategory.AMKSCapture, @"Hauppauge WinTV PVR PCI II Capture");
            _tunerDevice = FindDevice(FilterCategory.AMKSTVTuner, @"Hauppauge WinTV PVR PCI II TvTuner");
            _audioDevice = FindDevice(FilterCategory.AMKSTVAudio, @"Hauppauge WinTV PVR PCI II TvAudio");
            _crossbarDevice = FindDevice(FilterCategory.AMKSCrossbar, @"Hauppauge WinTV PVR PCI II Crossbar");
            _encoderDevice = FindDevice(FilterCategory.WDMStreamingEncoderDevices, @"Hauppauge WinTV PVR PCI II Encoder");
            CreateWinTVGraph();
        }
    }
}
