using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// PTZ Cameras
    /// </summary>
    [Serializable]
    public enum PTZType
    {
        /// <summary>
        /// Sony VISCA protocol cameras
        /// </summary>
        Visca,
        /// <summary>
        /// Sony VISCA protocol for testing (doesn't output to device)
        /// </summary>
        ViscaTest1,
        /// <summary>
        /// Pelco D protocol cameras
        /// </summary>
        PelcoD,
        /// <summary>
        /// good way to generate a NullReferenceException
        /// </summary>
        Null,
        /// <summary>
        /// Virtual plugin for testing
        /// </summary>
        Test1,
        /// <summary>
        /// Wonwoo WCC-261 camera
        /// </summary>
        WWCC,
        /// <summary>
        /// Wonwoo WCA-261 camera
        /// </summary>
        WWCA,
        /// <summary>
        /// A "Stacked" camera control client
        /// is a framework with a protocol module
        /// on top of a transport module.
        /// The protocol defines the type of
        /// protocol the PTZ speaks (Visca,
        /// PelcoD, etc.) and the transport
        /// defines the type of transport
        /// used to connect to the PTZ.  Example
        /// Transports could be serial_point,
        /// Vivotek tunnel, a IP port->serial
        /// reflector device, or possibly the PTZ
        /// device defines it's own transport layer.  
        /// </summary>
        Stacked
    }
}
