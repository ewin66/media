using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using FutureConcepts.Media.DirectShowLib.Framework;

namespace FutureConcepts.Media.Server.Graphs
{
    public enum ServerGraphState
    {
        Running,
        Stopped,
        Aborted,
        Rebuild
    }

    public class BaseGraph : IDisposable
    {
        protected IGraphBuilder _graphBuilder;
        protected ICaptureGraphBuilder2 _captureGraphBuilder;
        protected IMediaControl _mediaControl;
        protected IBaseFilter _captureFilter;
#if DEBUG_ROTENTRY
        private DsROTEntry _rot = null;
#endif

        /// <summary>
        /// Hold this lock when changing the state of the graph
        /// Calling, Run, Stop, Dispose, etc
        /// </summary>
        protected object _graphStateLock = new object();

        private List<IndigoServices.CommonStreamService> _clients = new List<IndigoServices.CommonStreamService>();
        /// <summary>
        /// The current clients of this graph.
        /// </summary>
        protected List<IndigoServices.CommonStreamService> Clients
        {
            get
            {
                return _clients;
            }
        }

        /// <summary>
        /// Use this object to lock the Clients collection
        /// </summary>
        protected object _clientsCollectionLock = new object();

        private ServerGraphState _state = ServerGraphState.Stopped;
        /// <summary>
        /// The current state of the graph
        /// </summary>
        public ServerGraphState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                AppLogger.Message(String.Format("{0} is now {1}.", SourceConfig.SourceName, value.ToString()));
            }
        }

        private StreamSourceInfo _sourceConfig;

        public StreamSourceInfo SourceConfig
        {
            get
            {
                return _sourceConfig;
            }
            set
            {
                _sourceConfig = value;
            }
        }

        private OpenGraphRequest _openGraphRequest;

        public OpenGraphRequest OpenGraphRequest
        {
            get
            {
                return _openGraphRequest;
            }
            set
            {
                _openGraphRequest = value;
            }
        }

        private Profile _currentProfile;

        public Profile CurrentProfile
        {
            get
            {
                return _currentProfile;
            }
            set
            {
                if (_currentProfile != null)
                {
                    AppLogger.Message(String.Format("BaseGraph.CurrentProfile changing from {0} to {1}", _currentProfile.Name, value.Name));
                }
                else
                {
                    AppLogger.Message(String.Format("BaseGraph.CurrentProfile initially set to {0}", value.Name));
                }
                Profile oldProfile = _currentProfile;
                _currentProfile = value;
            }
        }

        private string _defaultProfileName;

        public string DefaultProfileName
        {
            get
            {
                return _defaultProfileName;
            }
            set
            {
                _defaultProfileName = value;
            }
        }

        private ProfileGroups _profileGroups;

        public ProfileGroups ProfileGroups
        {
            get
            {
                return _profileGroups;
            }
            set
            {
                _profileGroups = value;
            }
        }

        private SinkProtocolType _sinkProtocolType;
        /// <summary>
        /// The sink protocol type (i.e. MPEG2-TS, LTSF, etc.) 
        /// </summary>
        public SinkProtocolType SinkProtocolType
        {
            get
            {
                return _sinkProtocolType;
            }
            set
            {
                _sinkProtocolType = value;
            }
        }

        private string _clientURL;
        /// <summary>
        /// The fully expanded Client URL that the graph was initially opened with.
        /// </summary>
        public string ClientURL
        {
            get
            {
                return _clientURL;
            }
            set
            {
                _clientURL = value;
            }
        }

        private string _baseClientURL;
        /// <summary>
        /// The Sink URL that does not have the $InterfaceAddress macro expanded.
        /// </summary>
        public string BaseClientURL
        {
            get
            {
                return _baseClientURL;
            }
            set
            {
                _baseClientURL = value;
            }
        }

        public BaseGraph(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest)
        {
            int hr = 0;

            SourceConfig = sourceConfig;
            OpenGraphRequest = openGraphRequest;

            // An exception is thrown if cast fail
            _graphBuilder = (IGraphBuilder)new FilterGraph();
            _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            _mediaControl = (IMediaControl)_graphBuilder;

            // Attach the filter graph to the capture graph
            hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
            DsError.ThrowExceptionForHR(hr);

#if DEBUG_ROTENTRY
            _rot = new DsROTEntry(_graphBuilder);
#endif

            LoadProfileGroups();

            //the first listed ProfileGroup is the default ProfileGroup
            if (ProfileGroups.Items.Count == 0)
            {
                throw new SourceConfigException("No Profile Groups loaded!");
            }
            ProfileGroup profileGroup = ProfileGroups.Items[0];
            DefaultProfileName = profileGroup.Name + ":" + profileGroup.DefaultProfileName;

            if (OpenGraphRequest.Profile != null)
            {
                CurrentProfile = OpenGraphRequest.Profile;
            }
            else
            {
                CurrentProfile = FindProfile(DefaultProfileName);
            }
        }

        /// <summary>
        /// When this symbol is in a URL, it should be expanded to contain the interface address that a client
        /// request arrived on
        /// </summary>
        public static readonly string InterfaceAddressMacro = @"$InterfaceAddress";

        /// <summary>
        /// When this symbol is in a URL, accompanied by a range in parenthesis e.g. $PortRange(0-20), then the first
        /// available port in that range will replace the symbol.
        /// </summary>
        public static readonly string PortRangeMacro = @"$PortRange";

        /// <summary>
        /// Expands the InterfaceAddressMacro.
        /// </summary>
        /// <param name="url">url that contains the InterfaceAddressMacro</param>
        /// <param name="interfaceAddress">interface address to use</param>
        /// <returns>the string with the ItnerfaceAddressMacro symbol expanded</returns>
        public static string ExpandInterfaceAddressMacro(string url, IPAddress interfaceAddress)
        {
            if (url == null)
            {
                return null;
            }
            else
            {
                return url.Replace(InterfaceAddressMacro, interfaceAddress.ToString());
            }
        }

        /// <summary>
        /// Generates the list of ports currently allocated to both active and resident graphs
        /// </summary>
        /// <returns>the list of port numbers selected for currently existing SinkURLs</returns>
        protected static List<int> GetAllocatedPorts()
        {
            List<int> ports = new List<int>();
            int p;
            lock (GraphManager.GraphMap)
            {
                foreach (KeyValuePair<string, BaseGraph> g in GraphManager.GraphMap)
                {
                    if (TryParsePort(g.Value.ClientURL, out p))
                    {
                        ports.Add(p);
                    }
                }
            }
            return ports;
        }

        /// <summary>
        /// Returns the port listed in a SinkURL
        /// </summary>
        /// <param name="sinkURL">sink URL to parse</param>
        /// <param name="p">port number</param>
        /// <returns></returns>
        private static bool TryParsePort(string sinkURL, out int portNumber)
        {
            if (sinkURL == null)
            {
                portNumber = 0;
                return false;
            }
            string[] parts = sinkURL.Split(':');
            return Int32.TryParse(parts[parts.Length - 1], out portNumber);
        }

        /// <summary>
        /// Populates the SinkURL and BaseSinkURL properties.
        /// If the $PortRange macro is defined, a port is selected, and the macro is expanded.
        /// </summary>
        /// <remarks>
        /// does not expand $InterfaceAddress for the BaseSinkURL property
        /// </remarks>
        /// <param name="configSinkAddress">the sink address for this source, from the configuration file</param>
        /// <param name="interfaceAddress">The interface address that the graph is/will be opened on</param>
        /// <param name="protocol">protocol that will be used</param>
        protected void BuildClientURL(SinkProtocolType protocol)
        {
            this.SinkProtocolType = protocol;
            if (SourceConfig.SinkAddress == null)
            {
                if (SourceConfig.SinkAddress != null)
                {
                    ClientURL = SourceConfig.SinkAddress;
                }
                else
                {
                    throw new Exception("No ClientURL or SinkAddress defined for Source " + SourceConfig.SourceName);
                }
            }
            else
            {
                ClientURL = SourceConfig.SinkAddress;
            }
            BaseClientURL = ClientURL;

            if (ClientURL.Contains(InterfaceAddressMacro))
            {
                ClientURL = ExpandInterfaceAddressMacro(ClientURL, OpenGraphRequest.InterfaceAddress);
            }
            if (ClientURL.Contains(PortRangeMacro))
            {
                List<int> allocedPorts = GetAllocatedPorts();

                int pos = ClientURL.LastIndexOf(PortRangeMacro);
                string prefix = ClientURL.Substring(0, pos);
                string suffix = ClientURL.Substring(pos);
                int rangeNumbersPos = suffix.LastIndexOf(@"(") + 1;
                if (rangeNumbersPos > 0)
                {
                    int rangeNumbersLength = suffix.Length - rangeNumbersPos - 1;
                    string rangeNumbers = suffix.Substring(rangeNumbersPos, rangeNumbersLength);
                    string[] parts = rangeNumbers.Split(new char[] { '-' });
                    int portLow = Convert.ToInt32(parts[0]);
                    int portHigh = Convert.ToInt32(parts[1]);
                    int testPort = 0;
                    for (testPort = portLow; testPort <= portHigh; testPort++)
                    {
                        if ((!allocedPorts.Contains(testPort)) && IsPortAvailable(protocol, OpenGraphRequest.InterfaceAddress, testPort))
                        {
                            break;
                        }
                    }
                    if (testPort <= portHigh)
                    {
                        AppLogger.Message(String.Format("PortRange specified ({0},{1}) -- first available port was {2}", portLow, portHigh, testPort));
                        ClientURL = prefix + testPort.ToString();

                        BaseClientURL = BaseClientURL.Substring(0, BaseClientURL.LastIndexOf(@"$PortRange")) + testPort.ToString();
                    }
                    else
                    {
                        throw new Exception(String.Format("No ports for protocol \"{0}\" available between {1} and {2}!", protocol, portLow, portHigh));
                    }
                }
            }
            AppLogger.Message(String.Format("   BaseClientURL = {0}   ClientURL = {1}", BaseClientURL, ClientURL));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(false);
        }

        public virtual void Dispose(bool disposing)
        {
            lock (_graphStateLock)
            {
                GC.SuppressFinalize(this);
                if (disposing)
                {
                    FilterGraphTools.RemoveAllFilters(_graphBuilder);
                    if (_graphBuilder != null)
                    {
                        int refCount;
                        do
                        {
                            refCount = Marshal.ReleaseComObject(_graphBuilder);
                        } while (refCount > 0);
                        _graphBuilder = null;
                        _mediaControl = null;
                    }
                    if (_captureGraphBuilder != null)
                    {
                        int refCount;
                        do
                        {
                            refCount = Marshal.ReleaseComObject(_captureGraphBuilder);
                        } while (refCount > 0);
                        _captureGraphBuilder = null;
                    }
                    if (_abortTimer != null)
                    {
                        _abortTimer.Elapsed -= new System.Timers.ElapsedEventHandler(AbortTimer_Elapsed);
                        _abortTimer.Dispose();
                        _abortTimer = null;
                    }
                }
            }
        }

        ~BaseGraph()
        {
            Dispose(false);
        }

        /// <summary>
        /// Returns true if the key UseLTRestreamer2 is set to true in app.config
        /// </summary>
        private static bool UseLTRestreamer2
        {
            get
            {
                bool temp;
                if (bool.TryParse(ConfigurationManager.AppSettings["UseLTRestreamer2"], out temp))
                {
                    return temp;
                }
                return false;
            }
        }

        /// <summary>
        /// Factory method to instantiate graphs
        /// </summary>
        /// <param name="sourceConfig">source configuration. the SourceType here specifies the type of graph to instantiate</param>
        /// <param name="openGraphRequest">parameters regarding the creation of the graph</param>
        /// <returns>a new instance of the graph requested</returns>
        public static BaseGraph CreateInstance(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest)
        {
            BaseGraph graph;

            switch (sourceConfig.SourceType)
            {
                case SourceType.WinTV150:
                    graph = new WinTV150(sourceConfig, openGraphRequest);
                    break;
                case SourceType.WinTV418:
                    graph = new WinTV418(sourceConfig, openGraphRequest);
                    break;
                case SourceType.WinTV418ATSC:
                    graph = new WinTV418ATSC(sourceConfig, openGraphRequest);
                    break;
                case SourceType.WinTV500:
                    graph = new WinTV500(sourceConfig, openGraphRequest);
                    break;
                case SourceType.BouncingBallDVR:
                    graph = new BouncingBallDVR(sourceConfig, openGraphRequest);
                    break;
                case SourceType.CaptureSourceDVR:
                    graph = new CaptureSourceDVR(sourceConfig, openGraphRequest);
                    break;
                case SourceType.DVRtoMPEG2TS:
                    graph = new DVRtoMPEG2TS(sourceConfig, openGraphRequest);
                    break;
                case SourceType.DVRLeadNetSink:
                    graph = new DVRLeadNetSink(sourceConfig, openGraphRequest);
                    break;
                case SourceType.DVRtoDVR:
                    graph = new DVRtoDVR(sourceConfig, openGraphRequest);
                    break;
                case SourceType.ScreenCapture:
                    graph = new ScreenCapture(sourceConfig, openGraphRequest);
                    break;
                case SourceType.FastVDODVR:
                    graph = new FastVDODVR(sourceConfig, openGraphRequest);
                    break;
                case SourceType.MangoDVR:
                    graph = new MangoDVR(sourceConfig, openGraphRequest);
                    break;
#if SUPPORT_SRM314
                case SourceType.SRM314:
                    graph = new SRM314(sourceConfig, openGraphRequest);
                    break;
#endif
                default:
                    throw new ConfigurationErrorsException(sourceConfig.SourceName + " has invalid SourceType.");
            }
            return graph;
        }

        #region Client Management

        public virtual void AddClient(IndigoServices.CommonStreamService client)
        {
            bool runGraph = false;
            lock (_clientsCollectionLock)
            {
                _clients.Add(client);
                runGraph = _clients.Count == 1;
#if SHOW_ADDCLIENTS
                AppLogger.Message(" > " + this.SourceConfig.SourceName + " AddClient(" + client.UserName + ") -> total: " + _clients.Count);
#endif
            }
            if (runGraph)
            {
                Run();
            }
        }

        public virtual void RemoveClient(IndigoServices.CommonStreamService client)
        {
            bool stopGraph = false;
            lock (_clientsCollectionLock)
            {
                _clients.Remove(client);
                stopGraph = _clients.Count == 0;
#if SHOW_ADDCLIENTS
                AppLogger.Message(" > " + this.SourceConfig.SourceName + " RemoveClient(" + client.UserName + ") -> total: " + _clients.Count);
#endif
            }
            if (stopGraph)
            {
                Stop();
            }
        }

        /// <summary>
        /// Removes all of the currently attached clients
        /// </summary>
        public virtual void RemoveAllClients()
        {
            while (_clients.Count > 0)
            {
                RemoveClient(_clients[0]);
            }
        }

        /// <summary>
        /// Returns the number of clients attached to this graph
        /// </summary>
        public virtual int NumberOfClients
        {
            get
            {
                lock (_clientsCollectionLock)
                {
                    return _clients.Count;
                }
            }
        }

        /// <summary>
        /// Fetches the list of clients attached to this graph
        /// </summary>
        public virtual List<string> ClientList
        {
            get
            {
                lock (_clientsCollectionLock)
                {
                    List<string> list = new List<string>();
                    foreach (IStream client in _clients)
                    {
                        if (client is IndigoServices.CommonStreamService)
                        {
                            IndigoServices.CommonStreamService streamService = client as IndigoServices.CommonStreamService;
                            list.Add(streamService.UserName);
                        }
                        else
                        {
                            list.Add("unknown");
                        }
                    }
                    return list;
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns a session description for a given open graph request
        /// </summary>
        /// <param name="openGraphRequest">Open graph request info</param>
        /// <returns>the SessionDescription that the client will be needing to complete the connection</returns>
        public virtual SessionDescription FillOutSessionDescription(OpenGraphRequest openGraphRequest)
        {
            SessionDescription sd = new SessionDescription();

            sd.SourceName = SourceConfig.SourceName;
            sd.ProfileGroups = ProfileGroups;
            sd.CurrentProfileName = CurrentProfile.Name;
            sd.SinkURL = ExpandInterfaceAddressMacro(BaseClientURL, openGraphRequest.InterfaceAddress);
            AppLogger.Message("Returning SinkURL " + sd.SinkURL);
            Debug.WriteLine("Returning SinkURL " + sd.SinkURL);
            sd.StreamTimeLimit = Convert.ToInt32(ConfigurationManager.AppSettings["StreamTimeLimit"]);

            return sd;
        }

        #region Profile Management

        /// <summary>
        /// Takes care of loading the profile groups from disk. 
        /// </summary>
        protected virtual void LoadProfileGroups()
        {
            if (SourceConfig.ProfileGroupNames != null)
            {
                ProfileGroups = new ProfileGroups();
                foreach (string profileGroupName in SourceConfig.ProfileGroupNames)
                {
                    ProfileGroup profileGroup = ProfileGroup.LoadFromFile(profileGroupName);
                    ProfileGroups.Items.Add(profileGroup);
                    foreach (Profile profile in profileGroup.Items)
                    {
                        profile.Name = profileGroup.Name + ":" + profile.Name;
                    }
                }
            }
        }

        private Dictionary<string, Profile> _profileDict;

        public Profile FindProfile(string fullyQualifiedProfileName)
        {
            if (_profileDict == null)
            {
                _profileDict = new Dictionary<string, Profile>();
                foreach (ProfileGroup profileGroup in ProfileGroups.Items)
                {
                    foreach (Profile profile in profileGroup.Items)
                    {
                        _profileDict.Add(profile.Name, profile);
                    }
                }
            }

            return _profileDict[fullyQualifiedProfileName];
        }

        public virtual void ChangeProfile(Profile newProfile)
        {
            CurrentProfile = newProfile;
        }

        #endregion

        #region Graph Transport Run/Stop/Abort

        public FilterState GraphState
        {
            get
            {
                if (_mediaControl != null)
                {
                    FilterState filterState;
                    int hr = _mediaControl.GetState(2000, out filterState);
                    DsError.ThrowExceptionForHR(hr);
                    return filterState;
                }
                else
                {
                    return FilterState.Stopped;
                }
            }
        }

        public virtual void Run()
        {
            lock (_graphStateLock)
            {
                try
                {
                    int hr;

                    if (_mediaControl != null)
                    {
                        StartAbortTimer(Thread.CurrentThread, ServerGraphState.Running);

                        hr = _mediaControl.Run();

                        CancelAbortTimer();

                        DsError.ThrowExceptionForHR(hr);
                        State = ServerGraphState.Running;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public virtual void Stop()
        {
            lock (_graphStateLock)
            {
                int hr;

                if (_mediaControl != null)
                {
                    StartAbortTimer(Thread.CurrentThread, ServerGraphState.Stopped);

                    hr = _mediaControl.Stop();

                    CancelAbortTimer();

                    DsError.ThrowExceptionForHR(hr);
                    State = ServerGraphState.Stopped;
                }
            }
        }

        public void Abort()
        {
            lock (_graphStateLock)
            {
                if (_graphBuilder != null)
                {
                    _graphBuilder.Abort();
                }
            }
        }

        #endregion

        #region Hung-Graph Abortion Logic

        /// <summary>
        /// The watchdog timer
        /// </summary>
        private System.Timers.Timer _abortTimer;
        /// <summary>
        /// The thread that is trying to execute the call that hung
        /// </summary>
        private System.Threading.Thread _abortThread;
        /// <summary>
        /// The state the graph was trying to enter
        /// </summary>
        private ServerGraphState _abortState;

        /// <summary>
        /// Starts the abort timer, in case the state transition you are about to do hangs
        /// </summary>
        /// <param name="threadToKill">thread that is going to be doing the work</param>
        /// <param name="targetState">graph's target state</param>
        private void StartAbortTimer(Thread threadToKill, ServerGraphState targetState)
        {
            _abortThread = threadToKill;
            _abortState = targetState;
            if (_abortTimer == null)
            {
                _abortTimer = new System.Timers.Timer(30000);
                _abortTimer.Enabled = false;
                _abortTimer.Elapsed += new System.Timers.ElapsedEventHandler(AbortTimer_Elapsed);
            }
            _abortTimer.Enabled = true;
        }

        /// <summary>
        /// Cancels any pending abort timer
        /// </summary>
        private void CancelAbortTimer()
        {
            if (_abortTimer != null)
            {
                _abortTimer.Enabled = false;
                _abortTimer.Elapsed -= new System.Timers.ElapsedEventHandler(AbortTimer_Elapsed);
                _abortTimer.Dispose();
                _abortTimer = null;
            }
            _abortThread = null;
        }

        private void AbortTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _abortTimer.Enabled = false;
            string hangMessage = String.Format("Graph {0} is hung trying to enter {1} state", SourceConfig.SourceName, _abortState);
            AppLogger.Message(hangMessage);
            ErrorLogger.WriteToEventLog(hangMessage, EventLogEntryType.Error);

#if SUICIDE_ON_DSHANG
            MediaServer.CommitSuicide();
#endif

            //Calling Abort on the DS graph seems to also hang... so this is commented out

            //AppLogger.Message(String.Format("trying to abort {0}", SourceConfig.SourceName));
            //int hr = _graphBuilder.Abort();
            //DsError.ThrowExceptionForHR(hr);

            //State = ServerGraphState.Aborted;
            //GraphManager.RemoveGraph(SourceConfig.SourceName);
            //_abortThread.Abort();
            
            //Dispose(true);
        }

        #endregion

        #region DirectShow Utility Methods

        protected IBaseFilter AddFilter(IBaseFilter baseFilter, string friendlyName)
        {
            int hr;

            hr = _graphBuilder.AddFilter(baseFilter, friendlyName);
            if (hr != 0)
            {
                string errorText = DsError.GetErrorText(hr);
                throw new Exception("The DirectShow filter " + friendlyName + " could not be added to graph.", new Exception(errorText));
            }
            return baseFilter;
        }

        protected IBaseFilter AddFilterByName(Guid filterCategory, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByName(_graphBuilder, filterCategory, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        protected IBaseFilter AddFilterByDevicePath(string devicePath, string friendlyName)
        {
            IBaseFilter filter = FilterGraphTools.AddFilterByDevicePath(_graphBuilder, devicePath, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        protected void ConnectFilters(IBaseFilter upFilter, string upPin, IBaseFilter downFilter, string downPin)
        {
            FilterGraphTools.ConnectFilters(_graphBuilder, upFilter, upPin, downFilter, downPin, false);
        }

        protected void ConnectFilters(IBaseFilter upFilter, string upPin, IBaseFilter downFilter, string downPin, bool useIntelligentConnect)
        {
            FilterGraphTools.ConnectFilters(_graphBuilder, upFilter, upPin, downFilter, downPin, useIntelligentConnect);
        }

        protected void RenderPin(IBaseFilter upFilter, string upPin)
        {
            if (FilterGraphTools.RenderPin(_graphBuilder, upFilter, upPin) == false)
            {
                throw new Exception("Unable to render " + upPin);
            }
        }

        public void SaveGraphFile(string filename)
        {
            FilterGraphTools.SaveGraphFile(_graphBuilder, filename);
        }

        /// <summary>
        /// Decrements the reference count of an object.
        /// If the reference count becomes 0, it is set to null
        /// </summary>
        /// <param name="o">object to release</param>
        protected void ReleaseComObject(object o)
        {
            if (o != null)
            {
                if (Marshal.ReleaseComObject(o) <= 0)
                {
                    o = null;
                }
            }
        }

        /// <summary>
        /// Decrements the reference count to 0, and sets to null
        /// </summary>
        /// <param name="o">object to forcibly release</param>
        protected void ForceReleaseComObject(object o)
        {
            int refCount;
            if (o != null)
            {
                do
                {
                    refCount = Marshal.ReleaseComObject(o);
                } while (refCount > 0);
                o = null;
            }
        }

        #endregion

        /// <summary>
        /// Determines if a given port is available
        /// </summary>
        /// <param name="sinkProtocolType">sink protocol type</param>
        /// <param name="interfaceAddress">interface address to test with</param>
        /// <param name="port">port number to check</param>
        /// <returns>True if available, false if could not bind.</returns>
        private bool IsPortAvailable(SinkProtocolType sinkProtocolType, IPAddress interfaceAddress, int port)
        {
            bool available = false;
            Socket testSocket = null;
            try
            {
                if ((sinkProtocolType == SinkProtocolType.LTSF_DGRAM) ||
                    (sinkProtocolType == SinkProtocolType.RTP))
                {
                    testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                else if ((sinkProtocolType == SinkProtocolType.LTSF_TCP) ||
                         (sinkProtocolType == SinkProtocolType.HTTP) ||
                         (sinkProtocolType == SinkProtocolType.Default))
                {
                    testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                EndPoint myEndPoint = new IPEndPoint(interfaceAddress, port);
                testSocket.Bind(myEndPoint);
                available = true;
            }
            catch (Exception exc)
            {
                AppLogger.Message(exc.Message);
            }
            finally
            {
                if (testSocket != null)
                {
                    testSocket.Close();
                    testSocket = null;
                }
            }
            return available;
        }
    }
}
