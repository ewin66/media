using System;
using System.Diagnostics;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;
using System.Threading;

namespace FutureConcepts.Media.CameraControls
{
    /// <summary>
    /// A test camera control plugin, used to test the ICameraControl and ICameraControlPlugin interfaces
    /// </summary>
    public class Test1: ICameraControlPlugin
    {
        /// <summary>
        /// Disposes the class
        /// </summary>
        public void Dispose()
        {
        }

        private ICameraControl _cameraControl;

        /// <summary>
        /// Creates a new instance of the test plugin
        /// </summary>
        /// <param name="config">configuration</param>
        /// <param name="cameraControl">owning parent camera control</param>
        public Test1(CameraControlInfo config, ICameraControl cameraControl)
        {
            try
            {
                _cameraControl = cameraControl;
                Debug.WriteLine("CameraControls.Test1.Load");
                Thread.Sleep(1000); // some fake delay to emulate start/stop of SerialPort
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
            }
        }

        /// <summary>
        /// Initializes the camera control to some default test values
        /// </summary>
        public void Initialize()
        {
            _cameraControl.CurrentPanAngle = 50.456;
            _cameraControl.CurrentTiltAngle = 30.123;
            _cameraControl.CurrentZoomPosition = 3.0;
        }

        /// <summary>
        /// Turns on or off the infrared capture filter
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        public void SetInfrared(bool enabled)
        {
            Debug.WriteLine(String.Format("CameraControls.Test1.SetInfrared({0})", enabled.ToString() ));
        }

        /// <summary>
        /// Turns on or off the stabilizer
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        public void SetStabilizer(bool enabled)
        {
            Debug.WriteLine(String.Format("CameraControls.Test1.SetStabilizer({0})", enabled.ToString()));
        }

        /// <summary>
        /// Turns on or off the wiper
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        public void SetWiper(bool enabled)
        {
            Debug.WriteLine(String.Format("CameraControls.Test1.SetWiper({0})", enabled.ToString()));
        }

        /// <summary>
        /// Turns on or off the IR emitter
        /// </summary>
        /// <param name="enabled">true for enabled, false to disable</param>
        public void SetEmitter(bool enabled)
        {
            Debug.WriteLine(String.Format("CameraControls.Test1.SetEmitter({0})", enabled.ToString() ));
        }

        /// <summary>
        /// Turns on or off the image inversion
        /// </summary>
        /// <param name="inverted">true for enabled, false to disable</param>
        public void SetOrientation(bool inverted)
        {
            Debug.WriteLine(String.Format("CameraControls.Test1.SetOrientation({0})", inverted.ToString()));
        }

        /// <summary>
        /// Updates the Pan and Tilt angles
        /// </summary>
        /// <param name="panAngle">pan angle</param>
        /// <param name="tiltAngle">tilt angle</param>
        public void PanTiltAbsolute(double panAngle, double tiltAngle)
        {
            Debug.WriteLine(String.Format("CameraControls.Test1.PanTiltAbsolute({0},{1})", panAngle.ToString(), tiltAngle.ToString()));

            _cameraControl.CurrentPanAngle = panAngle;
            _cameraControl.CurrentTiltAngle = tiltAngle;
        }

        /// <summary>
        /// Updates the zoom position
        /// </summary>
        /// <param name="zoomPositionFloat">zoom position</param>
        public void ZoomAbsolute(double zoomPositionFloat)
        {
            Debug.WriteLine(String.Format("CameraControls.Test1.ZoomDirect {0}", zoomPositionFloat.ToString()));

            _cameraControl.CurrentZoomPosition = zoomPositionFloat;
        }

        /// <summary>
        /// Updates the current information on the parent camera control
        /// </summary>
        public void PanTiltZoomPositionInquire()
        {
            Debug.WriteLine("CameraControls.Test1.PanTiltZoomPositionInquire");
            _cameraControl.CurrentPanAngle = _cameraControl.CurrentPanAngle;
            _cameraControl.CurrentTiltAngle = _cameraControl.CurrentTiltAngle;
            _cameraControl.CurrentZoomPosition = _cameraControl.CurrentZoomPosition;
        }
    }
}
