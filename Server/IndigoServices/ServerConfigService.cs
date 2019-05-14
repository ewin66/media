using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;
using FutureConcepts.Media.Contract;
using System.ServiceModel.Web;
using System.Text;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.IndigoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ServerConfigService : IServerConfig
    {
        private static int revisionNumber;
        /// <summary>
        /// Gets or sets the Revision Number that this server will advertise
        /// </summary>
        public static int RevisionNumber
        {
            get
            {
                return ServerConfigService.revisionNumber;
            }
            internal set
            {
                ServerConfigService.revisionNumber = value;
            }
        }

        static ServerConfigService()
        {
            //initialize the revision number to a random value, so that a restart of the server will make a different number
            Random r = new Random();
            revisionNumber = r.Next(Int32.MaxValue);
        }

        public ServerInfo GetServerInfo()
        {
            AppLogger.Message("ServerConfigService.GetServerInfo");

            ServerInfo serverInfo = new ServerInfo();
            serverInfo.ServerName = Dns.GetHostName();
            serverInfo.ServerAddress = OperationContext.Current.IncomingMessageHeaders.To.Host;
            serverInfo.VersionInfo = Assembly.GetAssembly(typeof(MediaServer)).ToString();
            serverInfo.StreamSources = StreamSources.LoadFromFile();
            serverInfo.ProfileGroups = ProfileGroups.LoadFromFile();
//            serverInfo.OriginServers = QueryOriginServers.OriginServers;
            serverInfo.OriginServers = null;
            serverInfo.RevisionNumber = ServerConfigService.RevisionNumber;

            return serverInfo;
        }

        public ServerInfo GetServerInfoSpecific(int serverParams, int sourceParams)
        {
            AppLogger.Message("ServerConfigService.GetServerInfoSpecific");
            ServerInfo output = new ServerInfo();

            if ((serverParams & RequestedServerInfoParams.RevisionNumber) == RequestedServerInfoParams.RevisionNumber)
            {
                output.RevisionNumber = ServerConfigService.RevisionNumber;
            }
            if (serverParams == RequestedServerInfoParams.RevisionNumber)   //quick optimization to avoid wasting time on "pings"
            {
                return output;
            }

            if ((serverParams & RequestedServerInfoParams.ServerName) == RequestedServerInfoParams.ServerName)
            {
                output.ServerName = Dns.GetHostName();
            }

            if ((serverParams & RequestedServerInfoParams.ServerAddress) == RequestedServerInfoParams.ServerAddress)
            {
                output.ServerAddress = OperationContext.Current.IncomingMessageHeaders.To.Host;
            }

            if ((serverParams & RequestedServerInfoParams.VersionInfo) == RequestedServerInfoParams.VersionInfo)
            {
                output.VersionInfo = Assembly.GetAssembly(typeof(MediaServer)).ToString();
            }

            if ((serverParams & RequestedServerInfoParams.StreamSources) == RequestedServerInfoParams.StreamSources)
            {
                StreamSources temp;
                CopyStreamSourceInfo(StreamSources.LoadFromFile(), sourceParams, out temp);
                output.StreamSources = temp;
            }

            if ((serverParams & RequestedServerInfoParams.ProfileGroups) == RequestedServerInfoParams.ProfileGroups)
            {
                output.ProfileGroups = ProfileGroups.LoadFromFile();
            }

//            if ((serverParams & RequestedServerInfoParams.OriginServers) == RequestedServerInfoParams.OriginServers)
//            {
//                output.OriginServers = new List<ServerInfo>();
//                if (QueryOriginServers.OriginServers != null)
//                {
//                    foreach (ServerInfo cur in QueryOriginServers.OriginServers)
//                    {
//                        ServerInfo copy;
//                        CopyServerInfo(cur, serverParams, sourceParams, out copy);
//                        output.OriginServers.Add(copy);
//                    }
//                }
//            }

            return output;
        }

        #region Copy*Info Methods

        /// <summary>
        /// This method strips server info *not* requested from an already fully populated server info object.
        /// </summary>
        /// <param name="cur">the ServerInfo to clean</param>
        /// <param name="serverParams">the server params to keep</param>
        /// <param name="sourceParams">the source params to keep</param>
        private void CopyServerInfo(ServerInfo input, int serverParams, int sourceParams, out ServerInfo output)
        {
            output = new ServerInfo();

            if ((serverParams & RequestedServerInfoParams.RevisionNumber) == RequestedServerInfoParams.RevisionNumber)
            {
                output.RevisionNumber = input.RevisionNumber;
            }

            if ((serverParams & RequestedServerInfoParams.ServerName) == RequestedServerInfoParams.ServerName)
            {
                output.ServerName = input.ServerName;
            }

            if ((serverParams & RequestedServerInfoParams.ServerAddress) == RequestedServerInfoParams.ServerAddress)
            {
                output.ServerAddress = input.ServerAddress;
            }

            if ((serverParams & RequestedServerInfoParams.VersionInfo) == RequestedServerInfoParams.VersionInfo)
            {
                output.VersionInfo = input.VersionInfo;
            }

            if ((serverParams & RequestedServerInfoParams.StreamSources) == RequestedServerInfoParams.StreamSources)
            {
                StreamSources temp;
                CopyStreamSourceInfo(input.StreamSources, sourceParams, out temp);
                output.StreamSources = temp;
            }

            if ((serverParams & RequestedServerInfoParams.ProfileGroups) == RequestedServerInfoParams.ProfileGroups)
            {
                output.ProfileGroups = input.ProfileGroups;
            }

            if ((serverParams & RequestedServerInfoParams.OriginServers) == RequestedServerInfoParams.OriginServers)
            {
                output.OriginServers = new List<ServerInfo>();
                foreach (ServerInfo cur in input.OriginServers)
                {
                    ServerInfo copy;
                    CopyServerInfo(cur, serverParams, sourceParams, out copy);
                    output.OriginServers.Add(copy);
                }
            }
        }

        /// <summary>
        /// This method strips out info *not* requested from the given requested stream source info params
        /// </summary>
        /// <param name="streamSources">collection of stream sources to cleanse</param>
        /// <param name="sourceParams">the source parameters to keep</param>
        private void CopyStreamSourceInfo(StreamSources input, int sourceParams, out StreamSources output)
        {
            output = new StreamSources();
            
            if(input.Count < 1)
            {
                return;
            }

            //the numeric/primitive values used below are the ones set as the DefaultValue() in the attributes.
            //when this value is used, then no XML is generated

            foreach (StreamSourceInfo inSource in input.Items)
            {
                StreamSourceInfo outSource = new StreamSourceInfo();

                if ((sourceParams & RequestedStreamSourceInfoParams.CameraControl) == RequestedStreamSourceInfoParams.CameraControl)
                {
                    outSource.CameraControl = inSource.CameraControl;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.Description) == RequestedStreamSourceInfoParams.Description)
                {
                    outSource.Description = inSource.Description;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.DeviceAddress) == RequestedStreamSourceInfoParams.DeviceAddress)
                {
                    outSource.DeviceAddress = inSource.DeviceAddress;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.LiveSource) == RequestedStreamSourceInfoParams.LiveSource)
                {
                    outSource.LiveSource = inSource.LiveSource;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.LogicalGroupSourceNames) == RequestedStreamSourceInfoParams.LogicalGroupSourceNames)
                {
                    outSource.LogicalGroupSourceNames = inSource.LogicalGroupSourceNames;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.MaxClients) == RequestedStreamSourceInfoParams.MaxClients)
                {
                    outSource.MaxClients = inSource.MaxClients;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.MaxQueueDuration) == RequestedStreamSourceInfoParams.MaxQueueDuration)
                {
                    outSource.MaxQueueDuration = inSource.MaxQueueDuration;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.MaxRecordingChunkMinutes) == RequestedStreamSourceInfoParams.MaxRecordingChunkMinutes)
                {
                    outSource.MaxRecordingChunkMinutes = inSource.MaxRecordingChunkMinutes;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.MicrowaveControl) == RequestedStreamSourceInfoParams.MicrowaveControl)
                {
                    if (inSource.MicrowaveControl != null)
                    {
                        outSource.MicrowaveControl = new MicrowaveControlInfo(inSource.MicrowaveControl);
                        //HACK only return the well-known data
                        outSource.MicrowaveControl.BlockDownConverterFrequency = 0;
                        outSource.MicrowaveControl.ReceiverType = MicrowaveReceiverType.PMR_AR100;
                    }
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.ProfileGroupNames) == RequestedStreamSourceInfoParams.ProfileGroupNames)
                {
                    outSource.ProfileGroupNames = inSource.ProfileGroupNames;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.URLs) == RequestedStreamSourceInfoParams.URLs)
                {
//                    outSource.ClientURL = inSource.ClientURL;
//                    outSource.DirectURL = inSource.DirectURL;
                    outSource.SinkAddress = inSource.SinkAddress;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.SourceName) == RequestedStreamSourceInfoParams.SourceName)
                {
                    outSource.SourceName = inSource.SourceName;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.SourceType) == RequestedStreamSourceInfoParams.SourceType)
                {
                    outSource.SourceType = inSource.SourceType;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.SyncToleranceMilliseconds) == RequestedStreamSourceInfoParams.SyncToleranceMilliseconds)
                {
                    outSource.SyncToleranceMilliseconds = inSource.SyncToleranceMilliseconds;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.TVTuner) == RequestedStreamSourceInfoParams.TVTuner)
                {
                    outSource.TVTuner = inSource.TVTuner;
                }
                if ((sourceParams & RequestedStreamSourceInfoParams.Hidden) == RequestedStreamSourceInfoParams.Hidden)
                {
                    outSource.Hidden = inSource.Hidden;
                }

                output.Add(outSource);
            }
        }

        #endregion

        public void PutStreamSources(StreamSources streamSources)
        {
            StreamSources.SaveToFile(streamSources);
        }

        public void PutProfileGroup(ProfileGroup profileGroup)
        {
            profileGroup.SaveToFile(profileGroup.Name);
        }

        public Message Resource(Message input)
        {
            ServerInfo serverInfo = GetServerInfo();
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServerInfo));
            xmlSerializer.Serialize(memoryStream, serverInfo);
            memoryStream.Position = 0;
            return Message.CreateMessage(MessageVersion.None, "*", XmlReader.Create(memoryStream));
        }
    }
}
