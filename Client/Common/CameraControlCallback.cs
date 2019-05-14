using System;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client
{
    public class CameraControlCallback : ICameraControlCallback
    {
        private ICameraControlClient _ccc;

        public CameraControlCallback(ICameraControlClient ccc)
        {
            _ccc = ccc;
        }

        public void InfraredChanged(bool enabled)
        {
            _ccc.InfraredEnabled = enabled;
        }

        public void StabilizerChanged(bool enabled)
        {
            _ccc.StabilizerEnabled = enabled;
        }

        public void WiperChanged(bool enabled)
        {
            _ccc.WiperEnabled = enabled;
        }

        public void EmitterChanged(bool enabled)
        {
            _ccc.EmitterEnabled = enabled;
        }

        public void OrientationChanged(bool invertedEnabled)
        {
            _ccc.InvertedEnabled = invertedEnabled;
        }

        public void PanAngleChanged(double panAngle)
        {
            _ccc.CurrentPanAngle = panAngle;
        }

        public void TiltAngleChanged(double tiltAngle)
        {
            _ccc.CurrentTiltAngle = tiltAngle;
        }

        public void ZoomPositionChanged(double zoomPosition)
        {
            _ccc.CurrentZoomPosition = zoomPosition;
        }

        public void SavedPositionsChanged(UserPresetStore savedCameraPositions)
        {
            _ccc.PresetItems = savedCameraPositions;
        }

        public void StatusMessageChanged(string message)
        {
            _ccc.StatusMessage = message;
        }
    }
}
