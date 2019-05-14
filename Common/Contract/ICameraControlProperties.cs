namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Properties found on the client Camera Control API, and on the CameraControlPlugin API
    /// </summary>
    public interface ICameraControlProperties
    {
        // Properties

        /// <summary>
        /// True if the infrared-capture mode of the camera is enabled.  Not supported by all cameras.
        /// </summary>
        bool InfraredEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// True if auto-image-stabilization is enabled.  Not supported by all cameras.
        /// </summary>
        bool StabilizerEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// True if the lens wiper is active.  Not supported by all cameras.
        /// </summary>
        bool WiperEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// True if the infrared emitter is active.  Not supported by all cameras.
        /// </summary>
        bool EmitterEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// True if the camera is vertically inverting the image.  Not supported by all cameras.
        /// </summary>
        bool InvertedEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Current Pan Angle of the camera, degrees, in the range [0,360)
        /// </summary>
        double CurrentPanAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Current Tilt Angle of the camera, degrees, in the range [-90,90]
        /// </summary>
        double CurrentTiltAngle
        {
            get;
            set;
        }

        /// <summary>
        /// Current Zoom position of the camera, magnification, in the range [1,Infinity)
        /// </summary>
        double CurrentZoomPosition
        {
            get;
            set;
        }

        /// <summary>
        /// A status or error message from the server.
        /// </summary>
        string StatusMessage
        {
            get;
            set;
        }
    }
}
