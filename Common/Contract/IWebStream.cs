using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Service Contract for the WebRestreamer service. <see>Media Server</see>
    /// </summary>
    [ServiceContract]
    public interface IWebStream
    {
        /// <summary>
        /// Initializes and begins streaming the graph with the specified source name. <seealso cref="T:IStreamService"/>
        /// </summary>
        /// <remarks>
        /// Utilizes the WebGet feature of .NET 3.5 to map a URI to a method call.
        /// </remarks>
        /// <param name="sourceName">source name on the server</param>
        /// <returns>Returns a stream for the user's web browser to render, typically in the form of an ASX file.</returns>
        [OperationContract, WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "OpenGraph/{sourceName}")]
        Stream OpenGraph(string sourceName);

        /// <summary>
        /// Retreives the server info
        /// </summary>
        /// <returns>the ServerInfo that describes this server</returns>
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "ServerInfo?server={server}&source={source}")]
        Stream ServerInfo(string server, string source);

        /// <summary>
        /// Retreives the client access policy for Silverlight clients
        /// </summary>
        /// <returns>the client access policy</returns>
        [OperationContract, WebGet(BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/clientaccesspolicy.xml")]
        Stream ClientAccessPolicy();
    }
}