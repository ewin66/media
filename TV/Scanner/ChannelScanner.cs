using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.TV.Scanner
{
    public class ChannelScanner : IDisposable, INotifyPropertyChanged, IChannelScanProvider 
    {
        private TVGraphs.BaseLocalTuner _baseTuner = null;
        private IVirtualChannelProvider _virtualChannelTuner = null;

        private ChannelCollection _channelList = new ChannelCollection();
        private Channel _min;
        private Channel _max;
        private int curChannelCount = 0;
        private int totalChannels;

        private bool _channelScanCancelled = false;

        private System.Windows.Forms.Timer _timer = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public ChannelScanner(TVGraphs.BaseLocalTuner graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph", "graph must not be null!");
            }

            _baseTuner = graph;
            _virtualChannelTuner = _baseTuner as IVirtualChannelProvider;

            _min = graph.MinChannel;
            _max = graph.MaxChannel;
            totalChannels = _max.PhysicalChannel - _min.PhysicalChannel;
            Current = _min;

            _timer = new System.Windows.Forms.Timer();
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Interval = 1000;
        }

        public void Dispose()
        {
            _timer.Dispose();
            this.PropertyChanged = null;
            this.ChannelScanStarted = null;
            this.ChannelScanProgressUpdate = null;
            this.ChannelScanComplete = null;
        }

        private Channel _current;

        public Channel Current
        {
            get
            {
                return _current;
            }
            private set
            {
                if (value != _current)
                {
                    _current = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Current"));
                    }
                }
            }
        }

        public ChannelCollection FoundChannels
        {
            get
            {
                return _channelList;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Enabled = false;
            int str = _baseTuner.GetSignalStrength();
            Debug.WriteLine("Channel " + _current.ToString() + " Str: " + str.ToString());
            if(str != 0)
            {
                Channel actual = _baseTuner.TryChannel;
                if (actual != null)
                {
                    Debug.WriteLine("actually found channel: " + actual.ToDebugString());
                    if (_virtualChannelTuner != null)
                    {
                        List<Channel> virtualChannels = _virtualChannelTuner.GetVirtualChannels();
                        foreach (Channel ch in virtualChannels)
                        {
                            _channelList.Add(ch);
                        }
                    }
                    else
                    {
                        _channelList.Add(actual);
                    }
                    _channelList.DumpToDebug();
                }
            }

            if (Current.Equals(_max))
            {
                Stop();

                _baseTuner.KnownChannels = this.FoundChannels;

                if (_baseTuner.KnownChannels.Count > 0)
                {
                    _baseTuner.Channel = _baseTuner.KnownChannels.Items[0];
                }

                if (ChannelScanComplete != null)
                {
                    ChannelScanComplete.Invoke(this, new ChannelScanCompleteEventArgs(_channelList.Count));
                }
            }
            else
            {
                Current = new Channel(Current.PhysicalChannel + 1);
                _baseTuner.TryChannel = Current;
                curChannelCount++;
                if (ChannelScanProgressUpdate != null)
                {
                    this.ChannelScanProgress = (int)(((double)curChannelCount / (double)totalChannels) * 100.0);
                    ChannelScanProgressUpdate.Invoke(this, new EventArgs());
                }

                if (!_channelScanCancelled)
                {
                    _timer.Enabled = true;
                }
            }
        }

        #region IChannelScanProvider Members

        public void StartChannelScan()
        {
            _channelScanCancelled = false;

            if (ChannelScanStarted != null)
            {
                ChannelScanStarted.Invoke(this, new EventArgs());
            }

            _baseTuner.TryChannel = _current;
            _timer.Enabled = true;
        }

        private void Stop()
        {
            _timer.Enabled = false;
        }

        public void CancelChannelScan()
        {
            _channelScanCancelled = true;
            Stop();
            if (ChannelScanComplete != null)
            {
                ChannelScanComplete.Invoke(this, new ChannelScanCompleteEventArgs(true));
            }
        }

        public event EventHandler ChannelScanStarted;

        public event EventHandler ChannelScanProgressUpdate;

        public event EventHandler<ChannelScanCompleteEventArgs> ChannelScanComplete;

        private int _channelScanProgress = -1;
        public int ChannelScanProgress
        {
            get
            {
                return _channelScanProgress;
            }
            private set
            {
                _channelScanProgress = value;
            }
        }

        #endregion
    }
}
