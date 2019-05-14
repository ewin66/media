using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Tools;
using FutureConcepts.Tools.Utilities;
//using Leadtools.Multimedia;
using LMNetMuxLib;
using LMNetSnkLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class StreamingGraph : BaseGraph
    {
        protected IBaseFilter _netMux;
        protected IBaseFilter _netSnk;
        private ILMNetMux _lmNetMux;
        private ILMNetSnk _lmNetSnk;

        /// <summary>
        /// If false, then SendClientsUpdate will do nothing
        /// </summary>
        private bool DispatchClientsUpdate = true;
        /// <summary>
        /// If false, then SendProfileUpdate will do nothing
        /// </summary>
        private bool DispatchProfileUpdate = true;

        public StreamingGraph(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest) : base(sourceConfig, openGraphRequest)
        {
            _netMux = AddFilterByName(FilterCategory.LegacyAmFilterCategory, @"LEAD Network Multiplexer (2.0)");
            _lmNetMux = (ILMNetMux)_netMux;
            _lmNetMux.LiveSource = SourceConfig.LiveSource;
            AppLogger.Message(String.Format("LiveSource={0}", LiveSource));
//            _lmNetMux.MaxQueueDuration = SourceConfig.MaxQueueDuration;
//            _lmNetMux.MaxQueueDuration = SourceConfig.MaxQueueDuration;
            AppLogger.Message(String.Format("MaxQueueDuration={0}", _lmNetMux.MaxQueueDuration));
            _netSnk = AddFilterByName(FilterCategory.LegacyAmFilterCategory, "LEAD Network Sink (2.0)");

            //parse the app.config to see if we are disabling either of these.
            bool.TryParse(ConfigurationManager.AppSettings["DispatchClientsUpdate"], out this.DispatchClientsUpdate);
            bool.TryParse(ConfigurationManager.AppSettings["DispatchProfileUpdate"], out this.DispatchProfileUpdate);
            if (!this.DispatchProfileUpdate)
            {
                AppLogger.Message("   DispatchProfileUpdate is disabled.");
            }
            if (!this.DispatchClientsUpdate)
            {
                AppLogger.Message("   DispatchClientsUpdate is disabled.");
            }
            BuildClientURL(SinkProtocolType.LTSF_TCP);
        }

        /// <summary>
        /// This method must be called to initialize the network sink. Make sure the SinkURL is properly populated before calling this method.
        /// </summary>
        protected void InitializeNetworkSink()
        {
            IFileSinkFilter fileSink = (IFileSinkFilter)_netSnk;
            if (fileSink == null)
            {
                throw new Exception("IFileSourceFilter not found on LMNetSink");
            }
            _lmNetSnk = (ILMNetSnk)_netSnk;
            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = DirectShowLib.MediaType.Stream;
            mediaType.subType = MediaSubType.Null;
            int hr = fileSink.SetFileName(ClientURL, mediaType);
            DsError.ThrowExceptionForHR(hr);
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_updateTimer != null)
                {
                    _updateTimer.Enabled = false;
                    _updateTimer.Elapsed -= new System.Timers.ElapsedEventHandler(UpdateTimer_Elapsed);
                }

                if (_netSnk != null)
                {
                    if (_lmNetSnk != null)
                    {
                        _lmNetSnk.CloseAll();
                    }
                    _netSnk = null;
                    _lmNetSnk = null;
                }
                _netMux = null;
            }
            base.Dispose(disposing);
        }

        public bool LiveSource
        {
            get
            {
                return _lmNetMux.LiveSource;
            }
            set
            {
                _lmNetMux.LiveSource = value;
            }
        }

        public int AverageBitRate
        {
            get
            {
                return _lmNetMux.BitRate;
            }
        }

        private System.Timers.Timer _updateTimer;

        private void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FireUpdatesToClients();
        }

        /// <summary>
        /// Call this method to send all updates to clients.
        /// </summary>
        protected virtual void FireUpdatesToClients()
        {
            SendProfileUpdate();
            SendClientsUpdate();
        }

        /// <summary>
        /// Write a message to all clients using the LT Net Mux
        /// </summary>
        /// <param name="msg">message to send</param>
        protected void WriteLTNetMessage(string msg)
        {
            if (_lmNetMux != null)
            {
                _lmNetMux.WriteMessage(msg);
            }
        }

        private void SendProfileUpdate()
        {
            try
            {
                if ((CurrentProfile != null) && (State == ServerGraphState.Running) && (this.DispatchProfileUpdate))
                {
                    string profileXml = XmlToString.Serialize<Profile>(this.CurrentProfile);
                    WriteLTNetMessage(profileXml);
                }
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
            }
        }

        private void SendClientsUpdate()
        {
            if ((State == ServerGraphState.Running) && (this.DispatchClientsUpdate))
            {
                TextWriter textWriter = new StringWriter();
                XmlWriter xmlWriter = new XmlTextWriter(textWriter);
                xmlWriter.WriteStartDocument(true);
                xmlWriter.WriteStartElement("Clients");
                foreach (string userName in ClientList)
                {
                    xmlWriter.WriteStartElement("add");
                    xmlWriter.WriteAttributeString("UserName", userName);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                try
                {
                    WriteLTNetMessage(textWriter.ToString());
                }
                catch (Exception exc)
                {
                    AppLogger.Message("StreamingGraph.SendClientsUpdate exception: " + exc.Message);
                }
                xmlWriter.Close();
                textWriter.Close();
                textWriter.Dispose();
            }
        }

        public override void Run()
        {
            if (_updateTimer == null)
            {
                _updateTimer = new System.Timers.Timer(10000);
                _updateTimer.Enabled = false;
                _updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateTimer_Elapsed);
            }
            _updateTimer.Enabled = true;

            base.Run();

            //send out preliminary updates!
            FireUpdatesToClients();
        }

        public override void Stop()
        {
            if (_updateTimer != null)
            {
                _updateTimer.Enabled = false;
            }
            base.Stop();
        }

        public void ConnectFilterToNetMux(IBaseFilter sourceFilter, string sourcePinName, string muxPinName)
        {
            ConnectFilters(sourceFilter, sourcePinName, _netMux, muxPinName);
        }

        public void ConnectNetMuxTo(IBaseFilter downFilter, string pinName)
        {
            ConnectFilters(_netMux, "Output 01", downFilter, pinName);
        }

        public void ConnectNetMuxToNetSnk()
        {
            ConnectFilters(_netMux, "Output 01", _netSnk, "Input");
        }

        public void ConnectToNetSnk(IBaseFilter upFilter, string pinName)
        {
            ConnectFilters(upFilter, pinName, _netSnk, "Input");
        }
    }
}
