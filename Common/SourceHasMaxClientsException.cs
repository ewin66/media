using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// An exception thrown if a given source has the maximum number of allowed clients already connected.
    /// </summary>
    public class SourceHasMaxClientsException : Exception
    {
        /// <summary>
        /// Creates a new instance with a message explaining why the source has max clients
        /// </summary>
        /// <param name="message">message to display to end user</param>
        public SourceHasMaxClientsException(string message)
            : base(message)
        {
        }
    }
}
