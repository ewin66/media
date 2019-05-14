using System;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Basic requisuit
    /// </summary>
    public interface IGraphControl
    {
        /// <summary>
        /// Runs the graph
        /// </summary>
        void Run();

        /// <summary>
        /// Stops the graph
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops the graph once buffers have been queued
        /// </summary>
        void StopWhenReady();

        /// <summary>
        /// Pauses the graph
        /// </summary>
        void Pause();

        /// <summary>
        /// Aborts the graph
        /// </summary>
        void Abort();

        /// <summary>
        /// Gets the State of the DirectShow graph
        /// </summary>
        State State
        {
            get;
            set;
        }
    }
}
