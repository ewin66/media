using System;
using System.ServiceModel;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Provides extension methods for Exceptions that simplify dealing with the craziness of WCF's exceptions
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// If the execption is a FaultException with details from the FutureConcepts.Media namespace,
        /// fetches the Message
        /// </summary>
        /// <param name="ex">Exception to examine.</param>
        /// <returns>Returns a non-empty string if the inner execption is from the FutureConcepts.Media namespace</returns>
        public static string GetFaultInnerDetailMessage(this Exception ex)
        {
            FaultException<ExceptionDetail> fe = ex as FaultException<ExceptionDetail>;
            if (fe != null)
            {
                if (fe.Detail.Type.StartsWith("FutureConcepts.Media"))
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Determines if the specified Exception is a FaultException / ExceptionDetail, and if so,
        /// determines if the Detail Type is of the type requested.
        /// </summary>
        /// <param name="ex">exception to examine</param>
        /// <param name="detailType">type to check for</param>
        /// <returns>true if the fault exception detail is of the specified type, false otherwise</returns>
        public static bool FaultExceptionDetailIsType(this Exception ex, Type detailType)
        {
            FaultException<ExceptionDetail> fe = ex as FaultException<ExceptionDetail>;
            if (fe != null)
            {
                if (fe.Detail.Type.Equals(detailType.ToString()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
