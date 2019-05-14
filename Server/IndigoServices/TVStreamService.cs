using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.TV;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.IndigoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class TVStreamService : CommonStreamService, ITVStream, IDisposable
    {
        private ITVStreamCallback _clientCallback;
        /// <summary>
        /// The client callback associated with this service instance
        /// </summary>
        public ITVStreamCallback ClientCallback
        {
            get
            {
                return _clientCallback;
            }
            private set
            {
                _clientCallback = value;
            }
        }

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

        #region IStream

        public override SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest)
        {
            this.ClientCallback = OperationContext.Current.GetCallbackChannel<ITVStreamCallback>();
            return base.OpenGraph(clientConnectRequest);
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

        #region ITVStream

        public void SetTVMode(TVMode tvMode)
        {
            try
            {
                TVGraph.TVMode = tvMode;
            }
            catch (Exception exception)
            {
                AppLogger.Message(String.Format("SetTVMode failed because {0}", exception.Message));
                throw exception;
            }
        }

        public TVMode GetTVMode()
        {
            try
            {
                return TVGraph.TVMode;
            }
            catch (Exception exception)
            {
                AppLogger.Message(String.Format("GetTVMode failed because {0}", exception.Message));
                throw exception;
            }
        }

        public void SetChannel(Channel channel)
        {
            try
            {
                if (channel != null)
                {
                    TVGraph.Channel = channel;
                }
            }
            catch (Exception exception)
            {
                AppLogger.Message(String.Format("SetChannel failed because {0}", exception.Message));
                AppLogger.Dump(exception);
                throw;
            }
        }

        public void ChannelUp()
        {
            try
            {
                TVGraph.ChannelUp();
            }
            catch (Exception exception)
            {
                AppLogger.Message(String.Format("ChannelUp failed because {0}", exception.Message));
            }
        }

        public void ChannelDown()
        {
            try
            {
                TVGraph.ChannelDown();
            }
            catch (Exception exception)
            {
                AppLogger.Message(String.Format("ChannelDown failed because {0}", exception.Message));
            }
        }

        public void StartChannelScan()
        {
            TVGraph.StartChannelScan();
        }

        public void CancelChannelScan()
        {
            
        }

        #endregion
    }
}
