using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// The Types of user-controls available for a Source. <seealso cref="SourceControlTypeState"/>
    /// </summary>
    /// <remarks>
    /// kdixon 02/2009
    /// </remarks>
    public enum SourceControlTypes
    {
        /// <summary>
        /// Pan-Tilt-Zoom camera.
        /// </summary>
        PTZ,
        /// <summary>
        /// Microwave controller.
        /// </summary>
        Microwave
    }

    /// <summary>
    /// The possible states of a Source Control <seealso cref="SourceControlTypes"/>
    /// </summary>
    /// <remarks>
    /// kdixon 07/22/2009
    /// </remarks>
    public enum SourceControlTypeState
    {
        /// <summary>
        /// control is currently in use
        /// </summary>
        Active,
        /// <summary>
        /// control is not currently in use, but is available to the user
        /// </summary>
        Inactive,
        /// <summary>
        /// user is allowed to use control. If the StreamViewerControl *has* such a control, this state is promoted to Inactive
        /// </summary>
        Enabled,
        /// <summary>
        /// user is not allowed to use control
        /// </summary>
        Disabled,
        /// <summary>
        /// no such control exists for the current StreamViewerControl
        /// </summary>
        Unavailable
    }

    /// <summary>
    /// Used to pass information about Palettes and Source Control options to SVD
    /// </summary>
    /// <remarks>
    /// kdixon 02/2009
    /// </remarks>
    public class SourceControlTypeEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance of the SourceControlTypeEventArgs regarding a specific SourceControlType
        /// </summary>
        /// <param name="type">type of control this event is in regard to</param>
        public SourceControlTypeEventArgs(SourceControlTypes type)
        {
            this.ControlType = type;
        }

        /// <summary>
        /// Gets or sets the associated SourceControlType
        /// </summary>
        public SourceControlTypes ControlType { get; set; }
    }
}
