using System;
using System.Collections.Generic;

namespace FutureConcepts.Media.Client.StreamViewer
{
    /// <summary>
    /// This is a specialized collection for managing the state of any source controls associated with a StreamViewerControl
    /// </summary>
    /// <remarks>
    /// kdixon ~07/15/2009
    /// </remarks>
    public class SourceControlTypeStateTracker
    {
        private Dictionary<SourceControlTypes, SourceControlTypeState> dictionary;

        internal SourceControlTypeStateTracker()
        {
            dictionary = new Dictionary<SourceControlTypes, SourceControlTypeState>();

            foreach (string name in Enum.GetNames(typeof(SourceControlTypes)))
            {
                dictionary.Add((SourceControlTypes)Enum.Parse(typeof(SourceControlTypes), name), SourceControlTypeState.Unavailable);
            }
        }

        /// <summary>
        /// Raised when a state has changed
        /// </summary>
        public event EventHandler<SourceControlTypeEventArgs> StateChanged;

        /// <summary>
        /// Access the state information for a given source control type
        /// </summary>
        /// <param name="type">type of source control to look up</param>
        /// <returns>the state of this control</returns>
        public SourceControlTypeState this[SourceControlTypes type]
        {
            get
            {
                return dictionary[type];
            }
            set
            {
                //the only way to leave the Unavailable state is to go to Inactive
                if ((dictionary[type] == SourceControlTypeState.Unavailable) && (value != SourceControlTypeState.Inactive))
                {
                    return;
                }

                //...if we have a control that is not "Unavailable", and we "Enabled" it, we really mean "Inactive"
                if (value == SourceControlTypeState.Enabled)
                {
                    //...also if you are Enabling something, but the state is already Active, it is not changed.
                    if (dictionary[type] == SourceControlTypeState.Active)
                    {
                        return;
                    }
                    else
                    {
                        value = SourceControlTypeState.Inactive;
                    }
                }

                dictionary[type] = value;
                FireStateChanged(type);
            }
        }

        private void FireStateChanged(SourceControlTypes type)
        {
            if (StateChanged != null)
            {
                StateChanged(this, new SourceControlTypeEventArgs(type));
            }
        }

        /// <summary>
        /// sets all control types to a given state
        /// </summary>
        /// <param name="state">target state</param>
        public void SetAllTo(SourceControlTypeState state)
        {
            foreach (string type in Enum.GetNames(typeof(SourceControlTypes)))
            {
                this[(SourceControlTypes)Enum.Parse(typeof(SourceControlTypes), type)] = state;
            }
        }
    }
}
