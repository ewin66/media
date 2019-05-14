using System;
using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Interface that defines methods that can be invoked on clients of the ICameraControlService.
    /// </summary>
    public interface ICameraControlCallback
    {
        /// <summary>
        /// Raised if the Infrared mode of the camera has changed 
        /// </summary>
        /// <param name="enabled">true if enabled, false if not</param>
        [OperationContract(IsOneWay = true)]
        void InfraredChanged(bool enabled);

        /// <summary>
        /// Raised if the Stabilizer mode has been changed
        /// </summary>
        /// <param name="enabled">true if enabled, false if not</param>
        [OperationContract(IsOneWay = true)]
        void StabilizerChanged(bool enabled);

        /// <summary>
        /// Raised if the Wipe mode has been changed
        /// </summary>
        /// <param name="enabled">true if enabled, false if not</param>
        [OperationContract(IsOneWay = true)]
        void WiperChanged(bool enabled);

        /// <summary>
        /// Raised if the Infrared Emitter has been enabled or not
        /// </summary>
        /// <param name="enabled">true if enabled, false if not</param>
        [OperationContract(IsOneWay = true)]
        void EmitterChanged(bool enabled);

        /// <summary>
        /// Raised if the Orientation of the camera has changed
        /// </summary>
        /// <param name="invertedEnabled">true if inverted verticall, false if normal</param>
        [OperationContract(IsOneWay = true)]
        void OrientationChanged(bool invertedEnabled);

        /// <summary>
        /// Raised if the Pan Angle has changed
        /// </summary>
        /// <param name="panAngle">new pan angle of the camera in the range [0,360)</param>
        [OperationContract(IsOneWay = true)]
        void PanAngleChanged(double panAngle);

        /// <summary>
        /// Raised if the Tilt Angle has changed
        /// </summary>
        /// <param name="tiltAngle">new tilt angle of the camera in the range [-90,90]</param>
        [OperationContract(IsOneWay = true)]
        void TiltAngleChanged(double tiltAngle);

        /// <summary>
        /// Raised if the Zoom Position has changed
        /// </summary>
        /// <param name="zoomPosition">the magnification value of the camera in the range [1,double.MaxValue]</param>
        [OperationContract(IsOneWay = true)]
        void ZoomPositionChanged(double zoomPosition);

        /// <summary>
        /// Raised if the collection of saved positions the server knows about has changed.
        /// </summary>
        /// <param name="namedCameraPositions">The new list of known camera preset positions</param>
        [OperationContract(IsOneWay = true)]
        void SavedPositionsChanged(UserPresetStore namedCameraPositions);

        /// <summary>
        /// Raised when the server announces a status or error message related to the camera's operation or current state.
        /// </summary>
        /// <param name="message">Message, intended to be displayed to the user</param>
        [OperationContract(IsOneWay = true)]
        void StatusMessageChanged(string message);
    }
}
