using System;
using System.Net;
using FutureConcepts.Media.Contract;


namespace FutureConcepts.Media.Server
{
    /// <summary>
    /// This class is used provide parameters neccesary to construct a graph
    /// </summary>
    /// <author>darnold / kdixon</author>
    [Serializable()]
    public class OpenGraphRequest
    {
        /// <summary>
        /// Constructs a blank OpenGraphRequest
        /// </summary>
        public OpenGraphRequest()
        {
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Constructs an OpenGraphRequest based on the parameters indicated in a ClientConnectRequest
        /// </summary>
        /// <param name="clientRequest"></param>
        public OpenGraphRequest(ClientConnectRequest clientRequest)
        {
            this.Update(clientRequest);
        }

        /// <summary>
        /// Allows an OpenGraphRequest to be updated with new client request info, without losing the OGR's other properties
        /// </summary>
        /// <param name="clientRequest">the client request to get updated data from</param>
        public void Update(ClientConnectRequest clientRequest)
        {
            _userName = clientRequest.UserName;
            _id = clientRequest.ID;
            _sourceName = clientRequest.SourceName;
            _interfaceAddress = clientRequest.InterfaceAddress;
        }

        private string _userName;
        /// <summary>
        /// The username to connect with
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        private Guid _id;
        /// <summary>
        /// Unique identifier for this open graph request.
        /// </summary>
        /// <remarks>
        /// This will be the same identifier used by a ClientConnectRequest if the constructor or Update method is used.
        /// </remarks>
        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private string _sourceName;
        /// <summary>
        /// The source name of the graph to open
        /// </summary>
        public string SourceName
        {
            get
            {
                return _sourceName;
            }
            set
            {
                _sourceName = value;
            }
        }

        private Profile _profile;
        /// <summary>
        /// The profile to construct the graph with.
        /// </summary>
        public Profile Profile
        {
            get
            {
                return _profile;
            }
            set
            {
                _profile = value;
            }
        }

        private bool _clientReconnecting = false;
        /// <summary>
        /// Set to true to indicate that the graph is being opened due to a client calling Reconnect,
        /// or false to indicate that the graph is being opened due to client calling OpenGraph
        /// </summary>
        public bool ClientReconnecting
        {
            get
            {
                return _clientReconnecting;
            }
            set
            {
                _clientReconnecting = value;
            }
        }

        private IPAddress _interfaceAddress;
        /// <summary>
        /// The interface address that the SinkURL will be created on.
        /// </summary>
        public IPAddress InterfaceAddress
        {
            get
            {
                return _interfaceAddress;
            }
            set
            {
                _interfaceAddress = value;
            }
        }
    }
}
