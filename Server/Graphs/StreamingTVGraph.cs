using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.TV;
using FutureConcepts.Tools;
using System.IO;

namespace FutureConcepts.Media.Server.Graphs
{
    public abstract class StreamingTVGraph : StreamingGraph
    {
        public StreamingTVGraph(StreamSourceInfo sourceConfig, OpenGraphRequest openGraphRequest)
            : base(sourceConfig, openGraphRequest)
        {
            InitializeNetworkSink();

            this.TVConfig = sourceConfig.TVTuner;

            try
            {
                this.KnownChannels = ChannelCollection.LoadFromFile(GetKnownChannelsStoreFilename());
            }
            catch (Exception ex)
            {
                this.KnownChannels = new ChannelCollection();
                AppLogger.Dump(ex);
            }
        }

        protected TVSourceInfo TVConfig { get; set; }

        private ChannelCollection _knownChannels;
        /// <summary>
        /// The collection of known channels
        /// </summary>
        protected ChannelCollection KnownChannels
        {
            get
            {
                return _knownChannels;
            }
            set
            {
                _knownChannels = value;
            }
        }

        /// <summary>
        /// Returns the file name used to persist the KnownChannels collection
        /// </summary>
        /// <returns>a string of the filename to save/load the KnownChannels from</returns>
        protected string GetKnownChannelsStoreFilename()
        {
            string fileName = this.GetType().ToString();
            fileName = fileName.Substring(fileName.LastIndexOf('.') + 1) + "_KnownChannels.xml";
            return PathMapper.AppData(fileName);
        }

        public override SessionDescription FillOutSessionDescription(OpenGraphRequest openGraphRequest)
        {
            SessionDescription sd = base.FillOutSessionDescription(openGraphRequest);

            sd.TVSessionInfo = new TVSessionInfo();
            sd.TVSessionInfo.Channel = this.Channel;
            sd.TVSessionInfo.TVMode = this.TVMode;
            sd.TVSessionInfo.ChannelScanInProgress = (channelScanWorker != null) ? channelScanWorker.IsBusy : false;

            return sd;
        }

        protected bool ChannelScanInProgress
        {
            get
            {
                if (channelScanWorker != null)
                {
                    return channelScanWorker.IsBusy;
                }
                return false;
            }
        }

        public override void Run()
        {
            if (ChannelScanInProgress)
            {
                return;
            }

            base.Run();
        }

        public override void Stop()
        {
            if ((!ChannelScanInProgress) || (GraphManager.ShutdownInProgress))
            {
                if (ChannelScanInProgress)
                {
                    CancelChannelScan();
                }
                base.Stop();
            }
        }

        /// <summary>
        /// Saves the KnownChannels collection to disk
        /// </summary>
        protected void SaveKnownChannels()
        {
            try
            {
                string file = GetKnownChannelsStoreFilename();
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                this.KnownChannels.SaveToFile(file);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        /// <summary>
        /// Gets and Sets the current channel.
        /// </summary>
        /// <remarks>
        /// The default implementation of set selects the closest known channel.
        /// </remarks>
        public virtual Channel Channel
        {
            get
            {
                Channel ch = GetChannel();

                Channel known = KnownChannels.FindClosest(ch);

                if (known != null)
                {
                    if ((known.MajorChannel == ch.MajorChannel) &&
                        (known.MinorChannel == ch.MinorChannel))
                    {
                        return known;
                    }
                }
                return ch;
            }
            set
            {
                Channel closest = KnownChannels.FindClosest(value);
                if (closest != null)
                {
                    AppLogger.Message("set_Channel: " + value.ToDebugString() + "   using: " + closest.ToDebugString());
                    SetChannel(closest);
                    DispatchUpdate(new Action<ITVStreamCallback>(SendChannelUpdate));
                }
                else
                {
                    AppLogger.Message("set_Channel: " + value.ToDebugString());
                    SetChannel(value);
                    DispatchUpdate(new Action<ITVStreamCallback>(SendChannelUpdate));
                }
            }
        }

        protected void DispatchUpdate(Action<ITVStreamCallback> ClientUpdateMethod)
        {
            try
            {
                if (State == ServerGraphState.Running)
                {
                    lock (_clientsCollectionLock)
                    {
                        foreach (IndigoServices.CommonStreamService client in this.Clients)
                        {
                            IndigoServices.TVStreamService tvClient = client as IndigoServices.TVStreamService;
                            if (tvClient != null)
                            {
                                if (tvClient.ClientCallback != null)
                                {
                                    try
                                    {
                                        ClientUpdateMethod(tvClient.ClientCallback);
                                    }
                                    catch (Exception ex)
                                    {
                                        AppLogger.Dump(ex);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
            }
        }

        /// <summary>
        /// Invoke via DispatchUpdate...sends the ChannelChanged update
        /// </summary>
        /// <param name="client">client to invoke on</param>
        protected virtual void SendChannelUpdate(ITVStreamCallback client)
        {
            Channel current = this.Channel;
            if (current != null)
            {
                client.ChannelChanged(current);
            }
        }

        /// <summary>
        /// Invoke via DispatchUpdate...sends the TVModeChanged update
        /// </summary>
        /// <param name="client"></param>
        protected virtual void SendTVModeUpdate(ITVStreamCallback client)
        {
            client.TVModeChanged(this.TVMode);
        }

        private TVMode _tvMode;
        public virtual TVMode TVMode
        {
            get
            {
                return _tvMode;
            }
            set
            {
                _tvMode = value;
                DispatchUpdate(new Action<ITVStreamCallback>(SendTVModeUpdate));
                if (value == TVMode.Broadcast)
                {
                    DispatchUpdate(new Action<ITVStreamCallback>(SendChannelUpdate));
                }
            }
        }

        /// <summary>
        /// Goes to the next highest known channel.
        /// </summary>
        public virtual void ChannelUp()
        {
            int i = KnownChannels.IndexOf(this.Channel);
            if (i == (KnownChannels.Count - 1))
            {
                this.Channel = KnownChannels.Items[0];
            }
            else if (i >= 0)
            {
                this.Channel = KnownChannels.Items[i + 1];
            }
            else if (KnownChannels.Count > 0)
            {
                this.Channel = KnownChannels.Items[0];
            }
        }

        /// <summary>
        /// Goes to the next lowest known channel
        /// </summary>
        public virtual void ChannelDown()
        {
            int i = KnownChannels.IndexOf(this.Channel);
            if (i == 0)
            {
                this.Channel = KnownChannels.Items[KnownChannels.Count - 1];
            }
            else if(i > 0)
            {
                this.Channel = KnownChannels.Items[i - 1];
            }
            else if (KnownChannels.Count > 0)
            {
                this.Channel = KnownChannels.Items[0];
            }
        }

        #region Channel Scanning

        protected abstract Channel GetChannel();
        protected abstract void SetChannel(Channel channel);
        private ChannelCollection _foundChannels;

        private BackgroundWorker channelScanWorker;

        /// <summary>
        /// Begins the channel scanning operation
        /// </summary>
        public void StartChannelScan()
        {
            AppLogger.Message("StreamingTVGraph.StartChannelScan");
            if (channelScanWorker == null)
            {
                channelScanWorker = new BackgroundWorker();
                channelScanWorker.DoWork += new DoWorkEventHandler(channelScanWorker_DoWork);
                channelScanWorker.ProgressChanged += new ProgressChangedEventHandler(channelScanWorker_ProgressChanged);
                channelScanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(channelScanWorker_RunWorkerCompleted);
                channelScanWorker.WorkerReportsProgress = true;
                channelScanWorker.WorkerSupportsCancellation = true;
            }

            if (!channelScanWorker.IsBusy)
            {
                _foundChannels = new ChannelCollection();
                channelScanWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Cancels the current channels can operation
        /// </summary>
        public void CancelChannelScan()
        {
            AppLogger.Message("StreamingTVGraph.CancelChannelScan");
            if (channelScanWorker != null)
            {
                channelScanWorker.CancelAsync();
            }
        }

        private void channelScanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int numChannels = this.MaxChannel.PhysicalChannel - this.MinChannel.PhysicalChannel;
            IVirtualChannelProvider _virtualChannelTuner = this as IVirtualChannelProvider;

            for (int i = this.MinChannel.PhysicalChannel; i <= this.MaxChannel.PhysicalChannel; i++)
            {
                if (CheckScanCanceled(e))
                {
                    return;
                }

                this.SetChannel(new Channel(i));
                Thread.Sleep(500);

                if (CheckScanCanceled(e))
                {
                    return;
                }

                int str = this.GetSignalStrength();
                AppLogger.Message("Try channel : " + i + " str=" + str);
                if (str != 0)
                {
                    Channel actual = this.GetChannel();
                    if (actual.MajorChannel != -1)
                    {
                        if (_virtualChannelTuner != null)
                        {
                            List<Channel> virtualChannels = _virtualChannelTuner.GetVirtualChannels();
                            foreach (Channel ch in virtualChannels)
                            {
                                if ((ch.MajorChannel == actual.MajorChannel) && (ch.MinorChannel > 0))
                                {
                                    ch.CarrierFrequency = actual.CarrierFrequency;
                                    _foundChannels.Add(ch);
                                }
                            }
                        }
                        else
                        {
                            _foundChannels.Add(actual);
                        }
                    }
                }

                if (CheckScanCanceled(e))
                {
                    return;
                }
                channelScanWorker.ReportProgress((int)(((double)(i - this.MinChannel.PhysicalChannel) / (double)numChannels) * 100));
            }
        }

        /// <summary>
        /// Returns true if the channel scanning has been canceled
        /// </summary>
        /// <param name="e">sets e.Cancel appropriate</param>
        /// <returns>true if canceled, false if not</returns>
        private bool CheckScanCanceled(CancelEventArgs e)
        {
            if (channelScanWorker.CancellationPending)
            {
                e.Cancel = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// When this event is raised, forward the channel scan progress to all clients
        /// </summary>
        private void channelScanWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (_clientsCollectionLock)
            {
                foreach (IndigoServices.CommonStreamService client in this.Clients)
                {
                    IndigoServices.TVStreamService tvClient = client as IndigoServices.TVStreamService;
                    if (tvClient != null)
                    {
                        if (tvClient.ClientCallback != null)
                        {
                            try
                            {
                                tvClient.ClientCallback.ChannelScanProgressUpdate(e.ProgressPercentage);
                            }
                            catch (Exception ex)
                            {
                                AppLogger.Dump(ex);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// When this event is raised, replace our known channel collection, and save it!
        /// </summary>
        private void channelScanWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.KnownChannels = _foundChannels;
                SaveKnownChannels();
                if (this.KnownChannels.Count > 0)
                {
                    this.Channel = this.KnownChannels.Items[0];
                }
            }

            //if everyone disconnected while we were busy scanning, then we'll just stop
            if (this.NumberOfClients == 0)
            {
                this.Stop();
            }
            else
            {
                ChannelScanCompleteEventArgs channelScanInfo = new ChannelScanCompleteEventArgs();
                channelScanInfo.Cancelled = e.Cancelled;
                channelScanInfo.ChannelsFound = this.KnownChannels.Count;

                lock (_clientsCollectionLock)
                {
                    foreach (IndigoServices.CommonStreamService client in this.Clients)
                    {
                        IndigoServices.TVStreamService tvClient = client as IndigoServices.TVStreamService;
                        if (tvClient != null)
                        {
                            if (tvClient.ClientCallback != null)
                            {
                                try
                                {
                                    tvClient.ClientCallback.ChannelScanComplete(channelScanInfo);
                                }
                                catch (Exception ex)
                                {
                                    AppLogger.Dump(ex);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected abstract int GetSignalStrength();

        protected abstract Channel MinChannel { get; }
        protected abstract Channel MaxChannel { get; }

        #endregion
    }
}
