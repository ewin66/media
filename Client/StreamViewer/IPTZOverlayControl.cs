using System;
using System.Windows.Forms;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// A generic interface for PTZ Controls that allow the user to control the PTZ by moving/using the mouse
    /// over the actual picture of the video
    /// </summary>
    public interface IPTZOverlayControl
    {
        /// <summary>
        /// Gets or sets the enabled state. If set to disabled, implementors of this class must not raise events or respond to UI input.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or Sets the horizontal field of view, in degrees.
        /// </summary>
        double FieldOfView { get; set; }

        /// <summary>
        /// Raised when an implementor has generated a move request
        /// </summary>
        event EventHandler<PTZMoveRequestEventArgs> MoveRequested;

        /// <summary>
        /// Invoked when the mouse has been "clicked" on the rendering surface
        /// </summary>
        /// <param name="sender">the Panel that is being rendered on</param>
        /// <param name="e">mouse information associated with the event</param>
        void OnMouseClick(object sender, MouseEventArgs e);

        /// <summary>
        /// Invoked when the mouse is moving over the rendering surface
        /// </summary>
        /// <param name="sender">the Panel that is being rendered on</param>
        /// <param name="e">mouse information associated with the event</param>
        void OnMouseMove(object sender, MouseEventArgs e);

        /// <summary>
        /// Invoked when the mouse has been pressed on the rendering surface
        /// </summary>
        /// <param name="sender">the Panel that is being rendered on</param>
        /// <param name="e">mouse information associated with the event</param>
        void OnMouseDown(object sender, MouseEventArgs e);

        /// <summary>
        /// Invoked when the mouse has been released on the rendering surface
        /// </summary>
        /// <param name="sender">the Panel that is being rendered on</param>
        /// <param name="e">mouse information associated with the event</param>
        void OnMouseUp(object sender, MouseEventArgs e);
    }
}
