using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;

using FutureConcepts.Settings;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// This class is used by a client to describe the context and goal of the client when connecting to Media Server
    /// kdixon 12/31/2008
    /// </summary>
    [Serializable]
    [DataContract]
    public class ClientConnectRequest
    {
        static ClientConnectRequest()
        {
            try
            {
                Assembly application = Assembly.GetEntryAssembly();
                AssemblyName n = application.GetName();
                DefaultUserAgent = n.Name + "/" + n.Version.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ClientConnectRequest..static_ctor(): Failed to get Entry Assembly information" + Environment.NewLine + ex.ToString());
            }
        }

        private static string _defaultUserAgent = "FutureConcepts.Media.Client";
        /// <summary>
        /// The default string to use for the UserAgent field. If not specified in the constructor, this is used.
        /// </summary>
        [IgnoreDataMember]
        public static string DefaultUserAgent
        {
            get
            {
                return _defaultUserAgent;
            }
            set
            {
                _defaultUserAgent = value;
            }
        }

        /// <summary>
        /// Constructs a connect request for a given source name. Uses the username as specified in MediaApplicationSettings
        /// </summary>
        /// <param name="sourceName">source name on the server to connect to</param>
        public ClientConnectRequest(string sourceName)
        {
            MediaApplicationSettings settings = new MediaApplicationSettings();

            Construct(sourceName, settings.UserName);
        }

        /// <summary>
        /// Constructs a connect request for a given source name, using the specified user name
        /// </summary>
        /// <param name="sourceName">source name on the server to connect to</param>
        /// <param name="userName">username to connect with</param>
        public ClientConnectRequest(string sourceName, string userName)
        {
            Construct(sourceName, userName);
        }

        /// <summary>
        /// Does the actual construction.
        /// </summary>
        /// <remarks>
        /// C#'s restriction of calling constructors from other constructors is weak.
        /// </remarks>
        private void Construct(string sourceName, string userName)
        {
            _openTime = DateTime.Now;
            _machineName = Environment.MachineName;
            _domainName = Environment.UserDomainName;
            _userName = userName;
            _id = Guid.NewGuid();
            _sourceName = sourceName;
            _userAgent = DefaultUserAgent;
        }

        private DateTime _openTime;
        /// <summary>
        /// Timestamp when the request was generated
        /// </summary>
        [DataMember(Name = "_openTime")]
        public DateTime Timestamp
        {
            get
            {
                return _openTime;
            }
            set
            {
                _openTime = value;
            }
        }

        private string _machineName;
        /// <summary>
        /// Machine name where this request was generated
        /// </summary>
        [DataMember(Name = "_machineName")]
        public string MachineName
        {
            get
            {
                return _machineName;
            }
            set
            {
                _machineName = value;
            }
        }

        private string _domainName;
        /// <summary>
        /// Domain name for the machine on which this request was generated
        /// </summary>
        [DataMember(Name = "_domainName")]
        public string DomainName
        {
            get
            {
                return _domainName;
            }
            set
            {
                _domainName = value;
            }
        }

        private string _userName;
        /// <summary>
        /// The user name of the client
        /// </summary>
        [DataMember(Name = "_userName")]
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
        [DataMember(Name = "_id")]
        public Guid ID
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
        /// The Stream Source Name to connect to
        /// </summary>
        [DataMember(Name = "_sourceName")]
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
        [DataMember(Name = "_interfaceAddress")]
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

        private string _userAgent;
        /// <summary>
        /// A string that identifies the application and version making the request
        /// </summary>
        [DataMember(IsRequired = false)]
        public string UserAgent
        {
            get
            {
                return _userAgent;
            }
            set
            {
                _userAgent = value;
            }
        }
    }
}
