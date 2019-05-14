using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// This exception indicates that the particular requested Source has no associated DirectShow graph
    /// </summary>
    public class ServiceHasNoGraphException : Exception
    {
        /// <summary>
        /// Creates a new instance with a generic message
        /// </summary>
        public ServiceHasNoGraphException()
            : base("Service has no graph")
        {
        }

        /// <summary>
        /// Creates a new instance with a specific message
        /// </summary>
        /// <param name="msg">message or informative text</param>
        public ServiceHasNoGraphException(string msg)
            : base(msg)
        {
        }
    }
}
