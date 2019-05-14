using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Client.StreamViewer.DeviceControl
{
    public class HttpGetDeviceControl : IDeviceControl
    {
        public event EventHandler Closed;

        private StreamSourceInfo _sourceInfo;
        private SessionDescription _sessionDescription;

        public HttpGetDeviceControl(StreamSourceInfo streamSourceInfo, String serverAddress)
        {
            _sourceInfo = streamSourceInfo;
            _sessionDescription = new SessionDescription();
//            _sessionDescription.ClientURL = streamSourceInfo.ClientURL;
            Profile profile = new Profile();
            profile.Name = "RTSP:RTSP";
            ProfileGroup profileGroup = new ProfileGroup();
            profileGroup.Name = "RTSP";
            profileGroup.Items.Add(profile);
            ProfileGroups profileGroups = new ProfileGroups();
            profileGroups.Items.Add(profileGroup);
            _sessionDescription.ProfileGroups = profileGroups;
            _sessionDescription.CurrentProfileName = "RTSP:RTSP";
//            _sessionDescription.SinkProtocolType = SinkProtocolType.RTSP;
        }

        public void Dispose()
        {
        }

        public void Open(ClientConnectRequest clientConnectRequest)
        {
            Debug.WriteLine("ClientConnectRequest");
        }

        public void Reconnect(ClientConnectRequest clientConnectRequest)
        {
        }

        public Boolean IsProfileGroupSelectorEnabled()
        {
            return false;
        }

        public SessionDescription SetProfile(Profile profile)
        {
            return null;
        }

        public SessionDescription GetSessionDescription()
        {
            return _sessionDescription;
        }
    }
}
