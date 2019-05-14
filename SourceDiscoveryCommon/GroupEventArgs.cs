using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.SourceDiscoveryCommon
{

    /// <summary>
    /// Contains information regarding an event involving a server.
    /// </summary>
    public class GroupEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance, referring to a source group by name
        /// </summary>
        /// <param name="address">server that is being referred to.</param>
        public GroupEventArgs(SourceDiscoveryGroup group)
        {
            this.Group = group;
        }

        public SourceDiscoveryPlugin Plugin { get; set; }

        /// <summary>
        /// Gets or sets the server address associated with this event
        /// </summary>
        public SourceDiscoveryGroup Group { get; set; }
    }
}
