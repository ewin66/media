using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.SourceDiscoveryCommon
{
    public abstract class SourceDiscoveryPlugin
    {
        
        #region Events

        /// <summary>
        /// Fired when a new server comes online
        /// </summary>
        public event EventHandler<GroupEventArgs> GroupOnline;
        /// <summary>
        /// Fired when a known server goes offline
        /// </summary>
        public event EventHandler<GroupEventArgs> GroupOffline;
        /// <summary>
        /// Fired when a currently known server's information has changed
        /// </summary>
        public event EventHandler<GroupEventArgs> GroupChanged;

        /// <summary>
        /// Fires the GroupOffline event
        /// </summary>
        /// <param name="serverAddress">address of server that has become unavailable</param>
        protected void FireGroupOffline(SourceDiscoveryGroup group)
        {
            if (GroupOffline != null)
            {
                GroupOffline.Invoke(this, new GroupEventArgs(group));
            }
        }

        /// <summary>
        /// Fires the GroupOnline event
        /// </summary>
        /// <param name="serverAddress">address of server that is now available</param>
        protected void FireGroupOnline(SourceDiscoveryGroup group)
        {
            if (GroupOnline != null)
            {
                GroupOnline.Invoke(this, new GroupEventArgs(group));
            }
        }

        /// <summary>
        /// Fires the ServerInfoChanged event
        /// </summary>
        /// <param name="serverAddress"></param>
        protected void FireGroupChanged(SourceDiscoveryGroup group)
        {
            if (GroupChanged != null)
            {
                GroupChanged.Invoke(this, new GroupEventArgs(group));
            }
        }

        #endregion

        #region Start/Stop

        /// <summary>
        /// Begins polling for sources
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops polling for sources
        /// </summary>
        public abstract void Stop();

        #endregion
    }
}
