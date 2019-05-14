using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Thrown when a Server Source (or other media source) is mal-configured.
    /// </summary>
    public class SourceConfigException : Exception
    {
        /// <summary>
        /// Creates an instance with a generic error message
        /// </summary>
        public SourceConfigException()
            : base("Invalid Source Configuration")
        {
        }

        /// <summary>
        /// Creates an instance with a specific error message
        /// </summary>
        /// <param name="msg">message describing what the issue is</param>
        public SourceConfigException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Creates an instance with a specicif error message and related Exception.
        /// </summary>
        /// <param name="msg">message describing what the issue is</param>
        /// <param name="innerException">exception related to this problem</param>
        public SourceConfigException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }
    }
}
