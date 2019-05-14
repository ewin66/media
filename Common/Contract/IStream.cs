using System;
using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Basic WCF contract used for all streaming sessions
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IStream
    {
        /// <summary>
        /// Initiates a session to a particular source. If the graph exists, then the profile is reset to its default.
        /// </summary>
        /// <param name="clientConnectRequest">A properly filled-out ClientConnectRequest, indicating which SourceName to connect to.</param>
        /// <returns>A session description for the client.</returns>
        [OperationContract()]
        SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest);

        /// <summary>
        /// Reconnects to a session for a particular source. No change in profile will occur.
        /// </summary>
        /// <param name="clientConnectRequest">A properly filled-out ClientConnectRequest, indicating which SourceName to connect to.</param>
        /// <returns>A session description for the client.</returns>
        [OperationContract()]
        SessionDescription Reconnect(ClientConnectRequest clientConnectRequest);

        /// <summary>
        /// Changes the profile once the session has been established.
        /// </summary>
        /// <param name="newProfile">new profile configuration for the stream</param>
        /// <returns>An updated session description. If null is returned, the client will have to <see cref="M:Reconnect"/></returns>
        [OperationContract()]
        SessionDescription SetProfile(Profile newProfile);

        /// <summary>
        /// Call periodically to keep the session alive.
        /// </summary>
        [OperationContract()]
        void KeepAlive();
    }
}
