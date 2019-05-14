using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Camera Control Common interface
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICameraControlCallback))]
    public interface ICameraControlCommon
    {
        /// <summary>
        /// Initializes the camera control module on the server
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Initialize();

        /// <summary>
        /// Change the Pan and Tilt values of the camera
        /// </summary>
        /// <remarks>
        /// Some camera plugins may use these parameters in a relative sense.
        /// </remarks>
        /// <param name="panAngle">
        /// New pan angle. [0,360).
        /// For some camera plugins, this may be a relative change, with negative values meaning
        /// degrees counter-clockwise and positive values meaning degrees clockwise.
        /// </param>
        /// <param name="tiltAngle">
        /// New tilt angle. [-90,90].
        /// For some camera plugins, this may be a relative change, with negative values meaning
        /// degrees down and positive values meaning degrees up.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void PanTiltAbsolute(double panAngle, double tiltAngle);

        /// <summary>
        /// Change the Zoom (magnification) value of the camera.
        /// </summary>
        /// <remarks>
        /// Some camera plugins may use the parameter in a relative sense.
        /// </remarks>
        /// <param name="zoomPosition">
        /// The new absolute zoom value, defined as magnification. [1,Infinity).
        /// For some camera plugins, this may be a relative change, with negative values meaning
        /// a decrease in magnification, and positive values meaning an increase in magnification.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void ZoomAbsolute(double zoomPosition);

        // Capabilities

        /// <summary>
        /// Enable or disable Infrared capture mode. Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if in infrared capture mode</param>
        [OperationContract(IsOneWay = true)]
        void SetInfrared(bool enabled);

        /// <summary>
        /// Enable or disable the auto-image-stabilization.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if stabilization is enabled</param>
        [OperationContract(IsOneWay = true)]
        void SetStabilizer(bool enabled);

        /// <summary>
        /// Enable or disable the lens wiper.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if the wiper is activated</param>
        [OperationContract(IsOneWay = true)]
        void SetWiper(bool enabled);

        /// <summary>
        /// Enable or disable the Infrared Emitter.  Not supported by all cameras.
        /// </summary>
        /// <param name="enabled">true if the infrared emitter is active</param>
        [OperationContract(IsOneWay = true)]
        void SetEmitter(bool enabled);

        /// <summary>
        /// Enable or disable the vertical inversion of the image.  Not supported by all cameras.
        /// </summary>
        /// <param name="inverted">true if the camera is inverting the image</param>
        [OperationContract(IsOneWay = true)]
        void SetOrientation(bool inverted);

        // Queries

        /// <summary>
        /// Inquire from the camera it's current pan, tilt, zoom information.  Not fully supported by all cameras.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void PanTiltZoomPositionInquire();
    }
}
