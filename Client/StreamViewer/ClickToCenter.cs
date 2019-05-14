using System;
using System.Windows.Forms;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// Implements a "click to center" PTZ Overlay Control
    /// </summary>
    public class ClickToCenter : IPTZOverlayControl
    {
        #region IPTZOverlayControl Members

        /// <summary>
        /// Get or set the enabled state of this control
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// The Horizontal Field of View
        /// </summary>
        public double FieldOfView
        {
            get;
            set;
        }

        /// <summary>
        /// Raised when a move has been generated
        /// </summary>
        public event EventHandler<PTZMoveRequestEventArgs> MoveRequested;

        /// <summary>
        /// Determines the delta of the mouse cursor from the center of the panel, and translates that into pan/tilt information
        /// </summary>
        /// <param name="sender">panel that the video is rendered on</param>
        /// <param name="e">mouse event info</param>
        public void OnMouseClick(object sender, MouseEventArgs e)
        {
            if ((!this.Enabled) || (MoveRequested == null))
            {
                return;
            }

            Panel p = sender as Panel;
            if (p == null)
            {
                return;
            }

            double vfov = (p.ClientRectangle.Height * this.FieldOfView) / p.ClientRectangle.Width;

            //calculate pan
            double deltaX = e.X - (p.ClientRectangle.Width / 2.0);
            double panDegrees = (deltaX / p.ClientRectangle.Width) * this.FieldOfView;

            //calculate tilt
            double deltaY = ((p.ClientRectangle.Height / 2.0) - e.Y);
            double tiltDegrees = (deltaY / p.ClientRectangle.Height) * vfov;

            MoveRequested(this, new PTZMoveRequestEventArgs(panDegrees, tiltDegrees));
        }

        /// <summary>
        /// Change the cursor to a crosshair when active, for precise clicking!
        /// </summary>
        /// <param name="sender">panel that the video is rendered on</param>
        /// <param name="e">mouse event info</param>
        public void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((sender is Panel) && (this.Enabled))
            {
                ((Panel)sender).Cursor = Cursors.Cross;
            }
        }

        /// <summary>
        /// not used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //do nothing
        }

        /// <summary>
        /// not used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //do nothing
        }

        #endregion
    }
}
