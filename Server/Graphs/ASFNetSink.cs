using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using WindowsMediaLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class ASFNetSink : BaseGraph, IWMStatusCallback
    {
        protected IBaseFilter _asfFilter;
        private IWMWriterNetworkSink _netSink;
        private IWMWriterAdvanced _writerAdvanced;
        private IWMWriter _writer;

        private System.Timers.Timer _eventTimer;

        public ASFNetSink(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest)
            : base(sourceConfig, openGraphRequest)
        {
            _eventTimer = new System.Timers.Timer();
            _eventTimer.Elapsed += new System.Timers.ElapsedEventHandler(_eventTimer_Elapsed);
            _eventTimer.Interval = 100;

            int hr;
            IFileSinkFilter pTmpSink;

            hr = _captureGraphBuilder.SetOutputFileName(MediaSubType.Asf, SourceConfig.SourceName.Replace(':', '_') + @".asf", out _asfFilter, out pTmpSink);
            DsError.ThrowExceptionForHR(hr);

            ConfigureWriterFilter();

            CreateNetworkWriterSink();

            OpenSink();

            AddSinkToWriter();

            _eventTimer.Start();
        }

        /// <summary>
        /// Adds the network sink to the ASF Writer filter.
        /// </summary>
        protected void AddSinkToWriter()
        {
            // Add the network sink
            _writerAdvanced.AddSink(_netSink as IWMWriterSink);
        }

        /// <summary>
        /// Configures the WM ASF Writer filter. (populates member variables as well)
        /// </summary>
        protected void ConfigureWriterFilter()
        {
            IWMProfile profile = GetWMProfile(CurrentProfile);
            WindowsMediaLib.IConfigAsfWriter configAsf = (WindowsMediaLib.IConfigAsfWriter)_asfFilter;
            configAsf.ConfigureFilterUsingProfile(profile);

            //Retreive the IWMWriter* objects from the WM ASF Writer filter.
            _writerAdvanced = GetWriterAdvanced();
            _writer = (IWMWriter)_writerAdvanced;

            //The WM ASF Writer comes with a File-Writer-Sink, which we need to remove.
            RemoveAllSinks(_writerAdvanced);

            //set SourceConfig settings
            _writerAdvanced.SetSyncTolerance(SourceConfig.SyncToleranceMilliseconds);

            _writerAdvanced.SetLiveSource(SourceConfig.LiveSource);
        }

        /// <summary>
        /// Creates the Network Writer Sink (populates member variable)
        /// </summary>
        private void CreateNetworkWriterSink()
        {
            //Create Network Sink
            WMUtils.WMCreateWriterNetworkSink(out _netSink);

            // Configure the network sink

            //register to receive callbacks from the Network Sink
            IWMRegisterCallback iCallback = (IWMRegisterCallback)_netSink;
            iCallback.Advise(this, IntPtr.Zero);

            _netSink.SetNetworkProtocol(NetProtocol.HTTP);

            if ((SourceConfig.MaxClients < 0) || (SourceConfig.MaxClients > 50))
            {
                throw new SourceConfigException("MaxClients must be 0-50 for ASFNetSink!");
            }
            _netSink.SetMaximumClients(SourceConfig.MaxClients);
        }

        protected void OpenSink()
        {
            AppLogger.Message("ASFNetSink -- OpenSink");
            //extract the port number from the SinkURL
            Uri uri = new Uri(this.ClientURL);
            int portNum = uri.Port;
            // Done configuring the network sink, open it
            _netSink.Open(ref portNum);
        }

        protected void CloseSink()
        {
            AppLogger.Message("ASFNetSink -- CloseSink");
            _netSink.Close();
        }

        /// <summary>
        /// Returns the number of clients currently connected. Returns -1 if an error occurs.
        /// </summary>
        /// <remarks>
        /// -1 is used to denote error. Actions such as connection state changes can then rely on
        /// 0 to know if there are actually known to be No clients connected.
        /// </remarks>
        public override int NumberOfClients
        {
            get
            {
                try
                {
                    if (_netSink != null)
                    {
                        IWMClientConnections connections = (IWMClientConnections)_netSink;
                        int num;
                        connections.GetClientCount(out num);
                        return num;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch(Exception)
                {
                    return -1;
                }
            }
        }

        public override void Run()
        {
            _eventTimer.Enabled = true;
            base.Run();
        }

        public override void Stop()
        {
            base.Stop();
            _eventTimer.Stop();
        }

        public override void Dispose(bool disposing)
        {
            AppLogger.Message("ASFNetSink -- Dispose called");

            if (_eventTimer != null)
            {
                _eventTimer.Enabled = false;
            }

            if (_netSink != null)
            {
                try
                {
                    IWMRegisterCallback iCallback = (IWMRegisterCallback)_netSink;
                    iCallback.Unadvise(this, IntPtr.Zero);
                    _netSink.Close();
                }
                catch (Exception ex)
                {
                    AppLogger.Dump(ex);
                }
                finally
                {
                    Marshal.ReleaseComObject(_netSink);
                    _netSink = null;
                }
            }
            if (_writerAdvanced != null)
            {
                RemoveAllSinks(_writerAdvanced);
                Marshal.ReleaseComObject(_writerAdvanced);
                _writerAdvanced = null;
            }
            if (_writer != null)
            {
                Marshal.ReleaseComObject(_writer);
                _writer = null;
            }
            if (_asfFilter != null)
            {
                Marshal.ReleaseComObject(_asfFilter);
                _asfFilter = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Removes all sinks from the Writer filter specified
        /// </summary>
        /// <param name="pWriterAdvanced">the writer to remove all Sinks from.</param>
        private void RemoveAllSinks(IWMWriterAdvanced pWriterAdvanced)
        {
            IWMWriterSink ppSink;
            int iSinkCount;

            pWriterAdvanced.GetSinkCount(out iSinkCount);

            for (int x = iSinkCount - 1; x >= 0; x--)
            {
                pWriterAdvanced.GetSink(x, out ppSink);

                pWriterAdvanced.RemoveSink(ppSink);
            }
        }

        /// <summary>
        /// Retreives the Sink URL, as reported by the given network writer sink
        /// </summary>
        /// <param name="networkSink">sink to analyze</param>
        /// <returns>a string of the URL.</returns>
        private string GetURL(IWMWriterNetworkSink networkSink)
        {
            int iSize = 0;

            // Call the function once to get the size
            networkSink.GetHostURL(null, ref iSize);

            StringBuilder sRet = new StringBuilder(iSize, iSize);
            networkSink.GetHostURL(sRet, ref iSize);

            // Trim off the trailing null
            return sRet.ToString().Substring(0, iSize - 1);
        }

        /// <summary>
        /// Queries the WM ASF Writer filter for the IWMWriterAdvanced interface.
        /// </summary>
        /// <returns>the IWMWriterAdvanced interface from the ASF Writer filter</returns>
        protected IWMWriterAdvanced GetWriterAdvanced()
        {
            int hr;
            IWMWriterAdvanced pWriterAdvanced = null;

            DirectShowLib.IServiceProvider pProvider = _asfFilter as DirectShowLib.IServiceProvider;

            if (pProvider != null)
            {
                object opro;

                hr = pProvider.QueryService(typeof(IWMWriterAdvanced2).GUID, typeof(IWMWriterAdvanced2).GUID, out opro);
                WMError.ThrowExceptionForHR(hr);

                pWriterAdvanced = opro as IWMWriterAdvanced;
                if (pWriterAdvanced == null)
                {
                    throw new Exception("Can't get IWMWriterAdvanced");
                }
            }

            return pWriterAdvanced;
        }

        /// <summary>
        /// Takes a given Profile and converts it to a Windows Media profile.
        /// </summary>
        /// <param name="prof">Profile to convert</param>
        /// <returns>Returns an IWMProfile interface</returns>
        protected IWMProfile GetWMProfile(Profile prof)
        {
            string filename = PathMapper.ConfigProfiles(@"WindowsMediaProfiles\" + prof.Name.Replace(':', '\\') + ".prx");

            IWMProfileManager mgr;
            WindowsMediaLib.WMUtils.WMCreateProfileManager(out mgr);

            FileStream fileStream = new FileStream(filename, FileMode.Open);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);
            fileStream.Close();
            fileStream.Dispose();
            string str = Encoding.Unicode.GetString(bytes);
            IWMProfile profile;
            mgr.LoadProfileByData(str, out profile);
            Marshal.ReleaseComObject(mgr);
            mgr = null;
            return profile;
        }

        public override void ChangeProfile(Profile newProfile)
        {
            //TODO implement changing of profiles, probably via a web interface.
            return;
        }

        /// <summary>
        /// Overloaded to load the WindowsMediaProfiles
        /// </summary>
        protected override void LoadProfileGroups()
        {
            ProfileGroups = new ProfileGroups();

            string wmpRoot = PathMapper.ConfigProfiles(@"WindowsMediaProfiles\");

            DirectoryInfo wmpRootInfo = new DirectoryInfo(wmpRoot);

            DirectoryInfo[] groups = wmpRootInfo.GetDirectories();
            foreach (DirectoryInfo groupInfo in groups)
            {
                ProfileGroup g = new ProfileGroup();
                g.Name = groupInfo.Name;

                foreach (FileInfo item in groupInfo.GetFiles("*.prx"))
                {
                    Profile p = new Profile();
//                    p.SinkProtocol = SinkProtocolType.HTTP;
                    p.Name = g.Name + ":" + Path.GetFileNameWithoutExtension(item.Name);

                    g.Items.Add(p);
                }

                g.DefaultProfileName = (g.Items.Count > 0) ? g.Items[0].Name.Split(':')[1] : null;

                ProfileGroups.Items.Add(g);
            }
        }

       
        private void _eventTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _eventTimer.Enabled = false;
                IMediaEventEx mediaEvent = _graphBuilder as IMediaEventEx;
                if (mediaEvent == null)
                {
                    AppLogger.Message("couldn't get IMediaEventEx from graph builder!");
                    return;
                }
                EventCode eventCode;
                IntPtr lparam1, lparam2;
                while (mediaEvent.GetEvent(out eventCode, out lparam1, out lparam2, 10) == 0)
                {
                    AppLogger.Message("ASFNetSink -- Got Media Event: " + eventCode.ToString() + " 0x" + lparam1.ToString("X") + " 0x" + lparam2.ToString("X"));
                    OnDirectShowEvent(eventCode, lparam1, lparam2);
                    mediaEvent.FreeEventParams(eventCode, lparam1, lparam2);
                }
                _eventTimer.Enabled = true;
            }
            catch(Exception ex)
            {
                AppLogger.Message("ASFNetSink -- Can't get Media Event due to an error");
                AppLogger.Dump(ex);
            }
        }

        /// <summary>
        /// Override this method to receive IMediaEvent events.
        /// </summary>
        /// <remarks>
        /// Default implementation is to stop the graph on event code "Complete"
        /// </remarks>
        /// <param name="eventCode">the Event code</param>
        /// <param name="lparam1">LPARAM 1</param>
        /// <param name="lparam2">LPARAM 2</param>
        protected virtual void OnDirectShowEvent(EventCode eventCode, IntPtr lparam1, IntPtr lparam2)
        {
            switch (eventCode)
            {
                case EventCode.Complete:
                    Stop();
                    break;
            }
        }

        public virtual void OnStatus(Status iStatus, int hr, AttrDataType dwType, IntPtr pValue, IntPtr pvContext)
        {
            AppLogger.Message("ASFNetSink -- OnStatus : " + iStatus.ToString());
        }
    }
}
