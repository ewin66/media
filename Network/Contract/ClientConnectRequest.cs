using System;
using System.Collections.Generic;
using System.Net;

namespace FutureConcepts.Media.Network.Contract
{
    /// <summary>
    /// This class is used by a client to describe the context and goal of the client when connecting to Media Server
    /// kdixon 12/31/2008
    /// </summary>
    [Serializable]
    public class ClientConnectRequest
    {
        /// <summary>
        /// Constructs a connect request for a given source name, using the specified user name
        /// </summary>
        /// <param name="sourceName">source name on the server to connect to</param>
        /// <param name="userName">username to connect with</param>
        /// <param name="reconnect">true if this is a reconnect, false if a first connect</param>
        public ClientConnectRequest(string sourceName, string userName, bool reconnect)
        {
            Construct(sourceName, userName, reconnect);
        }

        /// <summary>
        /// Constructs a connect request for a given source name, using the specified user name
        /// </summary>
        /// <param name="sourceName">source name on the server to connect to</param>
        /// <param name="userName">username to connect with</param>
        public ClientConnectRequest(string sourceName, string userName)
        {
            Construct(sourceName, userName, false);
        }

        /// <summary>
        /// Does the actual construction.
        /// </summary>
        /// <remarks>
        /// C#'s restriction of calling constructors from other constructors is weak.
        /// </remarks>
        private void Construct(string sourceName, string userName, bool reconnect)
        {
            _reconnect = reconnect;
            _openTime = DateTime.Now;
            _machineName = Environment.MachineName;
            _domainName = Environment.UserDomainName;
            _userName = userName;
            _id = Guid.NewGuid();
            _sourceName = sourceName;
        }

        private bool _reconnect;
        /// <summary>
        /// True if this request is a reconnect request rather than a first-connect
        /// </summary>
        public bool Reconnect
        {
            get
            {
                return _reconnect;
            }
            set
            {
                _reconnect = value;
            }
        }



        private DateTime _openTime;
        /// <summary>
        /// Timestamp when the request was generated
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return _openTime;
            }
        }

        private string _machineName;
        /// <summary>
        /// Machine name where this request was generated
        /// </summary>
        public string MachineName
        {
            get
            {
                return _machineName;
            }
        }

        private string _domainName;
        /// <summary>
        /// Domain name for the machine on which this request was generated
        /// </summary>
        public string DomainName
        {
            get
            {
                return _domainName;
            }
        }

        private string _userName;
        /// <summary>
        /// The user name of the client
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
        /// ID of this request
        /// </summary>
        public Guid ID
        {
            get
            {
                return _id;
            }
        }

        private string _sourceName;
        /// <summary>
        /// The Stream Source Name to connect to
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

        private IPAddress _interfaceAddress;
        /// <summary>
        /// For Server use only / The Interface that this request was received on
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
