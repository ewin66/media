using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Interface to be implemented by all camera control plugins
    /// </summary>
    public interface ICameraControlClient : ICameraControlCommon, IPresetProvider, IPresetProviderItems, ICameraControlProperties, INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when the client control is fully opened
        /// </summary>
        event EventHandler Opened;

        /// <summary>
        /// Raised when the client control has been closed or faulted
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Opens the camera control client.
        /// </summary>
        void Open(ClientConnectRequest clientConnectRequest);

        /// <summary>
        /// Closes the factory or aborts it as needed. Ensures that the Closed event is raised.
        /// </summary>
        void Close();

        /// <summary>
        /// How long the PTZ control GUI will remain open during no PTZ activity.
        /// When no activity occurs after RelinquishTimeout, the PTZ GUI control
        /// will automatically close and all PTZ resources allocated by the user
        /// will be released giving other users the ability to control the camera
        /// PTZ.
        /// </summary>
        int RelinquishTimeout { get; set; }
    }
}
