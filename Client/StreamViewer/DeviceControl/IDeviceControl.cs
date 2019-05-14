using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client.StreamViewer.DeviceControl
{
    public interface IDeviceControl : IDisposable
    {
        /// <summary>
        /// Raised when the connection has been closed
        /// </summary>
        event EventHandler Closed;

        void Open(ClientConnectRequest clientConnectRequest);
        void Reconnect(ClientConnectRequest clientConnectRequest);
        SessionDescription SetProfile(Profile profile);
        SessionDescription GetSessionDescription();
        Boolean IsProfileGroupSelectorEnabled();
    }
}
