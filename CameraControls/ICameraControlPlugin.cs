using System;
using System.Collections.Generic;
using System.Text;
//using System.ServiceModel;

using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.CameraControls
{
    /// <summary>
    /// Interface to be implemented by all camera control plugins
    /// </summary>
    public interface ICameraControlPlugin : ICameraControlCommon, IDisposable
    {
    }
}
