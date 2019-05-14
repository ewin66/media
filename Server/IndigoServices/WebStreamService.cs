using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

using FutureConcepts.Media.Server.Graphs;

namespace FutureConcepts.Media.Server.IndigoServices
{
    /// <summary>
    /// This is the WebStreamService. It spins off graphs from a single point
    /// </summary>
    /// <author>kdixon / darnold</author>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WebStreamService : CommonStreamService, IWebStream, IDisposable
    {
        private static object _webStreamServiceLock = new object();

        public static readonly string WebRestreamerSourceName = "webrestreamer";

        public Stream OpenGraph(string sourceName)
        {
            try
            {
                lock (_webStreamServiceLock)
                {
                    string incominguser = GenerateUserName();
                    AppLogger.Message(" > WebStreamService: client connected: " + incominguser);
                    this.ClientConnectRequest = new ClientConnectRequest(sourceName, incominguser);
                    this.ClientConnectRequest.InterfaceAddress = GetIncomingInterfaceAddress();
                    this.OpenGraphRequest = new OpenGraphRequest(this.ClientConnectRequest);
                    //TODO since this is a singleton, this instance member is always equal to the last graph created...
                    _graph = OpenGraph();
                    if (_graph.NumberOfClients <= 0)
                    {
                        _graph.AddClient(this);
                    }

                    string asx = GetASXDescriptor(sourceName, _graph.ClientURL);
                    WebOperationContext.Current.OutgoingResponse.ContentType = "video/x-ms-asf";
                    MemoryStream magic = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(asx));
                    return magic;
                }
            }
            catch (SourceConfigException ex)
            {
                AppLogger.Dump(ex);
                AppLogger.Message("Returning HTTP error 404");
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                return new MemoryStream();
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
                AppLogger.Message("Returning HTTP error 500");
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return new MemoryStream();
            }
        }

        public void StartGraph(string sourceName)
        {
            GraphManager.StartGraph(sourceName);
        }

        public void StopGraph(string sourceName)
        {
            GraphManager.StopGraph(sourceName);
        }

        //TODO make this be read from a file or something
        private const string _clientAccessPolicy = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                    "<access-policy>" +
                                        "<cross-domain-access>" +
                                            "<policy>" +
                                                "<allow-from>" +
                                                    "<domain uri=\"*\"/>" +
                                                "</allow-from>" +
                                                "<grant-to>" +
                                                    "<resource path=\"/\" include-subpaths=\"true\"/>" +
                                                "</grant-to>" +
                                            "</policy>" +
                                        "</cross-domain-access>" +
                                    "</access-policy>";

        /// <summary>
        /// /clientaccesspolicy.xml service endpoint. Retreives the client access policy for Silverlight clients
        /// </summary>
        /// <returns>the client access policy</returns>
        public Stream ClientAccessPolicy()
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";

            MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(_clientAccessPolicy));
            return m;
        }

        private static XmlSerializer _serverInfoSerializer;
        private static XmlSerializer ServerInfoSerializer
        {
            get
            {
                if (_serverInfoSerializer == null)
                {
                    _serverInfoSerializer = new XmlSerializer(typeof(ServerInfo));
                }
                return _serverInfoSerializer;
            }
        }

        /// <summary>
        /// /ServerInfo service endpoint. Forwards requests to ServerConfigService.GetServerInfoSpecific
        /// </summary>
        /// <param name="serverParamsString">string representation of RequestedServerInfoParams, or null to not use</param>
        /// <param name="sourceParamsString">string representation of RequestedStreamSourceInfoParams, or null to not use</param>
        /// <returns>XML serialized version of ServerInfo returned by GetServerInfoSpecific</returns>
        public Stream ServerInfo(string serverParamsString, string sourceParamsString)
        {

            int serverParams = RequestedServerInfoParams.All;
            int sourceParams = RequestedStreamSourceInfoParams.All;
            int temp;
            if (Int32.TryParse(serverParamsString, out temp))
            {
                serverParams = temp;
            }
            if (Int32.TryParse(sourceParamsString, out temp))
            {
                sourceParams = temp;
            }

            ServerInfo serverInfo = (new ServerConfigService()).GetServerInfoSpecific(serverParams, sourceParams);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
            MemoryStream memoryStream = new MemoryStream();
            ServerInfoSerializer.Serialize(memoryStream, serverInfo);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private string GenerateUserName()
        {
            string username = string.Empty;

            RemoteEndpointMessageProperty prop = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (prop != null)
            {
                username = prop.Address + " / ";
            }
            return username + WebOperationContext.Current.IncomingRequest.UserAgent;
        }

        /// <summary>
        /// Overrides the normal GetSourceConfig to redefine it as a web restreamer source
        /// </summary>
        /// <returns></returns>
        protected override StreamSourceInfo GetSourceConfig()
        {
//            if (ClientConnectRequest.SourceName.Contains(":"))
//            {
//                StreamSourceInfo sourceConfig;
//                StreamSources sources = StreamSources.LoadFromFile();

//                string[] parts = ClientConnectRequest.SourceName.Split(new char[] { ':' }, 2);
//                string originServerAddress = parts[0];
//                string sourceName = parts[1];
//                StreamSourceInfo originServerSourceInfo = QueryOriginServers.FindSource(ClientConnectRequest.SourceName);

//                sourceConfig = new StreamSourceInfo(sources.FindSource(WebStreamService.WebRestreamerSourceName));
//                sourceConfig.SourceType = SourceType.LT2WM;
//                sourceConfig.SourceName = ClientConnectRequest.SourceName;
//                sourceConfig.Description = originServerSourceInfo.Description;
//
//                return sourceConfig;
//            }
//            else
            {
                return base.GetSourceConfig();
            }
        }

        private string GetASXDescriptor(string sourceName, string sinkURL)
        {
            StringWriter textWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(textWriter);

            // <asx version="3.0">
            xmlWriter.WriteStartElement("asx");
            xmlWriter.WriteAttributeString("version", "3.0");

            // <title>...</title>
            xmlWriter.WriteStartElement("title");
            xmlWriter.WriteString("Future Concepts Live Stream");
            xmlWriter.WriteEndElement();

            // <entry>
            xmlWriter.WriteStartElement("entry");
            // <title>...</title>
            xmlWriter.WriteStartElement("title");
            xmlWriter.WriteString(sourceName);
            xmlWriter.WriteEndElement();

            // <ref href="..." />
            xmlWriter.WriteStartElement("ref");
            xmlWriter.WriteAttributeString("href", sinkURL);
            xmlWriter.WriteEndElement();

            // </entry>
            xmlWriter.WriteEndElement();
            // </asx>
            xmlWriter.WriteEndElement();

            xmlWriter.Flush();
            string xmlResult = textWriter.ToString();
            xmlWriter.Close();
            textWriter.Close();
            textWriter.Dispose();

            return xmlResult;
        }

        public override void Dispose()
        {
            AppLogger.Message("WebStreamService got Disposed");
            base.Dispose();
        }
    }
}
