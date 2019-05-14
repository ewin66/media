using System;
using GMFBridgeLib;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// GMFBridge Source graph
    /// </summary>
    public interface ISourceSubGraph
    {
        /// <summary>
        /// Fetches the controller that this graph created
        /// </summary>
        IGMFBridgeController Controller
        {
            get;
        }

        /// <summary>
        /// The GMF Bridge Sink filter 
        /// </summary>
        IBaseFilter Output
        {
            get;
        }
    }
}
