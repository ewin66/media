using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FutureConcepts.Media.Network.Contract
{
    /// <summary>
    /// Service Contract for the ServerConfigService
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IServerConfig
    {
        /// <summary>
        /// Retreives a complete description of the server
        /// </summary>
        /// <returns>a ServerInfo object</returns>
        [OperationContract]
        ServerInfo GetServerInfo();

        /// <summary>
        /// Retreieves only specific information about the server. Used to conserve bandwidth
        /// </summary>
        /// <param name="requestedServerInfoParams">A bitmask defining which parameters of <see cref="ServerInfo"/> should be sent. <seealso cref="T:RequestedServerInfoParams"/></param>
        /// <param name="requestedStreamSourceInfoParams">A bitmask defining which parameters of <see cref="StreamSourceInfo"/> objects should be sent. <seealso cref="RequestedStreamSourceInfoParams"/></param>
        /// <returns>A partially filled out ServerInfo object. Non-requested fields will be null or their default value.</returns>
        [OperationContract]
        ServerInfo GetServerInfoSpecific(int requestedServerInfoParams, int requestedStreamSourceInfoParams);

        /// <summary>
        /// Updates the <see cref="T:StreamSources"/> on the server.
        /// </summary>
        /// <param name="streamSources">new stream sources definition.</param>
        [OperationContract]
        void PutStreamSources(StreamSources streamSources);

        /// <summary>
        /// Overwrites or adds a new <see cref="T:ProfileGroup"/> to the server.
        /// </summary>
        /// <param name="profileGroup">new or updated <see cref="ProfileGroup"/></param>
        [OperationContract]
        void PutProfileGroup(ProfileGroup profileGroup);
    }
}
