using System;
using FutureConcepts.Media.Client.StreamViewer;
using System.ComponentModel;

namespace FutureConcepts.Media.SVD.Controls
{
    public interface IControlPalette
    {
        /// <summary>
        /// Associated StreamViewerControl
        /// </summary>
        StreamViewerControl StreamViewerControl { get; set; }

        /// <summary>
        /// The type of control this palette offers
        /// </summary>
        SourceControlTypes ControlType { get; }

        /// <summary>
        /// Raised when the palette can no longer do any operations and needs to be closed.
        /// </summary>
        [Category("Action"), Description("Raised when the palette can no longer do any operations and needs to be closed.")]
        event EventHandler<SourceControlTypeEventArgs> Closed;

        /// <summary>
        /// The control palette should become active
        /// </summary>
        void Start();

        /// <summary>
        /// The control palette should become dormant
        /// </summary>
        void Stop();
    }
}
