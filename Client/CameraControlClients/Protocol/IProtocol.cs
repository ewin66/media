using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client.CameraControlClients.Protocol
{
    /// <summary>
    /// Camera Control Common interface
    /// </summary>
    public interface IProtocol : ICameraControlCommon
    {
        bool HasAbsoluteControls
        {
            get;
        }

        bool HasPan
        {
            get;
        }

        bool HasTilt
        {
            get;
        }

        bool HasZoom
        {
            get;
        }

        bool HasDigitalZoom
        {
            get;
        }

        bool HasEmitter
        {
            get;
        }

        bool HasStabilizer
        {
            get;
        }

        bool HasInfrared
        {
            get;
        }

        bool HasInverter
        {
            get;
        }

        bool HasWiper
        {
            get;
        }

        bool HasFocus
        {
            get;
        }

        double PanLimitStart
        {
            get;
        }

        double PanLimitAngle
        {
            get;
        }

        double PanOffset
        {
            get;
        }

        double TiltMaxAngle
        {
            get;
        }

        double TiltMinAngle
        {
            get;
        }

        double ZoomMaxLevel
        {
            get;
        }

        double ZoomMinLevel
        {
            get;
        }

        double FieldOfView
        {
            get;
        }

        void Close();
    }
}
