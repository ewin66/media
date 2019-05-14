using System;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client.CameraControlClients.Protocol
{
    /// <summary>
    /// This class overrides some of the parameters of the WCC 261 class.
    /// The WCA 261 is fundamentally the same camera as the WCC, but just has some slightly different behavior
    /// kdixon 05/04/2009
    /// </summary>
    public class WonwooWCA261 : WonwooWCC261
    {
        /// <summary>
        /// Creates an instance of this plugin
        /// </summary>
        /// <param name="config">configuration information</param>
        /// <param name="cameraControl">owning camera control</param>
        public WonwooWCA261(ICameraControlClient client, Transport.ITransport transport) : base(client, transport) { }

        /// <summary>
        /// The speed byte that will acheive the fasest panning
        /// </summary>
        protected override byte PanSpeedByte
        {
            get
            {
                return 0x3F;
            }
        }

        /// <summary>
        /// zoom speed factor, used to calculate amount of time needed per magnification step
        /// </summary>
        protected override double ZoomSpeedFactor
        {
            get
            {
                return 4.0;
            }
        }

        /// <summary>
        /// Calculates the amount of time required (in ms) to acheive the given pan angle (distance).
        /// </summary>
        /// <remarks>
        /// The algorithim uses a linear regression model, divided at crucial slope-change points, which
        /// compenstates for the camera's (de)/(ac)celeration and top rotational speeds.
        /// The WCA261 has "endless" pan, so the pan angle is not limited.
        /// </remarks>
        /// <param name="panAngle">number of degrees to turn</param>
        /// <returns>a time in milliseconds that the camera should be told to rotate for</returns>
        protected override int GetPanTime(double panAngle)
        {
            double absPanAngle = Math.Abs(panAngle);

            if (absPanAngle >= 16)
            {
                return (int)(20.1903367 * absPanAngle + 150.84041);
            }
            else if (absPanAngle >= 4)
            {
                return (int)(19.0322581 * absPanAngle + 170.0645161);
            }
            else if (absPanAngle >= 2)
            {
                return (int)(29.1304348 * absPanAngle + 126.826087);
            }
            else if (absPanAngle >= 0.2)
            {
                return (int)(72.1428571 * absPanAngle + 47.5714286);
            }
            else
            {
                return (int)(310.0 * absPanAngle);
            }
        }

        /// <summary>
        /// Calculates the amount of time required to travel the given number of degrees of tilt
        /// </summary>
        /// <param name="tiltAngle">number of degrees to travel</param>
        /// <returns>time in milliseconds that the camera should tilt for</returns>
        protected override int GetTiltTime(double tiltAngle)
        {
            double absTiltAngle = Math.Abs(tiltAngle);

            if (absTiltAngle >= 8)
            {
                return (int)(27.5384615 * absTiltAngle + 140.9538462);
            }
            else if (absTiltAngle >= 4.6)
            {
                return (int)(20.3225806 * absTiltAngle + 196.5161290);
            }
            else if (absTiltAngle >= 2.6)
            {
                return (int)(25.2380952 * absTiltAngle + 169.9523810);
            }
            else if (absTiltAngle >= 1.3)
            {
                return (int)(51.5384615 * absTiltAngle + 111.0);
            }
            else if (absTiltAngle >= 0.5)
            {
                return (int)(85.0 * absTiltAngle + 67.5);
            }
            else
            {
                return (int)(220.0 * absTiltAngle);
            }
        }
    }
}
