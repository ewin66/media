using System;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Indicates the state of the DirectShow graph
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The graph is stopped
        /// </summary>
        Stopped,
        /// <summary>
        /// The graph is paused
        /// </summary>
        Paused,
        /// <summary>
        /// The graph is running
        /// </summary>
        Running
    };
}
