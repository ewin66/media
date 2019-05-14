using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client.StreamViewer.DeviceControl
{
    public class ManagedDeviceControl : IDeviceControl
    {
        private GraphControl _client;

        public event EventHandler Closed;

        public ManagedDeviceControl(StreamSourceInfo streamSourceInfo, String serverAddress)
        {
            //Open WCF connection for session
            _client = new GraphControl(serverAddress);
            _client.Closed += new EventHandler(GraphControl_Closed);
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Closed -= new EventHandler(GraphControl_Closed);
                _client.Dispose();
                _client = null;
            }
        }

        public void Open(ClientConnectRequest clientConnectRequest)
        {
            if (_client != null)
            {
                _client.OpenGraph(clientConnectRequest);
            }
        }

        public void Reconnect(ClientConnectRequest clientConnectRequest)
        {
            if (_client != null)
            {
                _client.Reconnect(clientConnectRequest);
            }
        }

        public Boolean IsProfileGroupSelectorEnabled()
        {
            return false;
        }

        public SessionDescription SetProfile(Profile profile)
        {
            SessionDescription result = null;
            if (_client != null)
            {
                result = _client.SetProfile(profile);
            }
            return result;
        }

        public SessionDescription GetSessionDescription()
        {
            if (_client != null)
            {
                return _client.SessionDescription;
            }
            else
            {
                return null;
            }
        }

        private void GraphControl_Closed(Object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(sender, e);
            }
        }
    }
}
