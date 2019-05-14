using System;
using System.ServiceModel;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Server.IndigoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class StreamService : CommonStreamService, IStream, IDisposable
    {
        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

        #region IStream

        public override SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest)
        {
            SessionDescription sd = base.OpenGraph(clientConnectRequest);

            Profile newProfile = new Profile();
            newProfile.Name = _graph.DefaultProfileName;
            SetProfile(newProfile);
            sd.CurrentProfileName = newProfile.Name;
            return sd;
        }

        public override SessionDescription Reconnect(ClientConnectRequest clientConnectRequest)
        {
            return base.Reconnect(clientConnectRequest);
        }

        public override SessionDescription SetProfile(Profile newProfile)
        {
            return base.SetProfile(newProfile);
        }

        public override void KeepAlive()
        {
            base.KeepAlive();
        }

        #endregion
    }
}
