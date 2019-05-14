using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Possible States for the ServerStatusService to report for either the server
    /// as a whole or a particular source.
    /// </summary>
    public enum ServerStatusState : int
    {
        /// <summary>
        /// The state cannot be determined
        /// </summary>
        Unknown         = 0,
        /// <summary>
        /// The server/source is actively streaming data
        /// </summary>
        Streaming       = 1,
        /// <summary>
        /// The server/source has no clients
        /// </summary>
        Idle            = 2,
        /// <summary>
        /// The server/source is recovering from an error, or reconfiguring its hardware in response to a profile change
        /// </summary>
        Recovering      = 3,
        /// <summary>
        /// The server has detected a hardware fault which requires a power cycle to recover from.
        /// </summary>
        NeedsPowerCycle = 4
    }

    /// <summary>
    /// Reported properties for a server or specific source
    /// </summary>
    /// <author>
    /// kdixon 09/28/2009
    /// </author>
    [Serializable]
    public class ServerStatusProperties
    {
        private string _sourceName = null;
        /// <summary>
        /// The source name that these properties refer to. Null if for server.
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

        private ServerStatusState _state = ServerStatusState.Unknown;
        /// <summary>
        /// The state for this source or server.
        /// </summary>
        public ServerStatusState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        private Int32 _userCount = 0;
        /// <summary>
        /// The number of clients.
        /// </summary>
        public Int32 UserCount
        {
            get
            {
                return _userCount;
            }
            set
            {
                _userCount = value;
            }
        }

        private List<string> _users = null;
        /// <summary>
        /// The list of user names for the clients. If for server, this list will be null or empty.
        /// </summary>
        public List<string> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
            }
        }

        private string _profile = null;
        /// <summary>
        /// The currently selected profile, in format GroupName:ProfileName -- will be null for servers.
        /// </summary>
        public string Profile
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

        private Int32 _bitrate = 0;
        /// <summary>
        /// The average output bitrate, in kbps.
        /// </summary>
        public Int32 Bitrate
        {
            get
            {
                return _bitrate;
            }
            set
            {
                _bitrate = value;
            }
        }
    }
}
