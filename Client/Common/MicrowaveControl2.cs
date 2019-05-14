using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.Contract;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Diagnostics;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// Provides an interface for the MicrowaveControlService2
    /// </summary>
    public class MicrowaveControl2 : BasePeripheralControl<IMicrowaveControl2>, IPresetProvider, IChannelScanProvider, INotifyPropertyChanged, IDisposable
    {
        private ClientConnectRequest clientRequest;

        /// <summary>
        /// Creates a new instance of the client class
        /// </summary>
        /// <param name="serverHostOrIP">server's hostname or IP with which we will be connecting to</param>
        public MicrowaveControl2(string serverHostOrIP) : base(serverHostOrIP) { }

        #region BasePeripheralControl

        /// <summary>
        /// Creates an instance of the <see cref="T:MicrowaveControlCallback"/>
        /// </summary>
        /// <returns>a new instance of <see cref="T:MicrowaveControlCallback"/></returns>
        protected override object CreateCallback()
        {
            return new MicrowaveControl2Callback(this);
        }

        /// <summary>
        /// Returns the port number to use on the server
        /// </summary>
        protected override string Port
        {
            get { return "8082"; }
        }

        /// <summary>
        /// Returns the name of the end point on the server
        /// </summary>
        protected override string Endpoint
        {
            get { return "MicrowaveControlService"; }
        }
        
        /// <summary>
        /// The desired keep-alive interval
        /// </summary>
        protected override int KeepAliveInterval
        {
            get { return 15000; }
        }

        #endregion

        #region IMicrowaveControl

        /// <summary>
        /// Opens a connection to the server and begins the session
        /// </summary>
        /// <param name="request">connection information, configure the SourceName you want to use</param>
        public void Open(ClientConnectRequest request)
        {
            this.clientRequest = request;
            try
            {
                Connect();
                Proxy.Open(clientRequest);

                if (ProxyReady)
                {
                    Proxy.ForceUpdate();
                }
            }
            catch (Exception exc)
            {
                if (Factory != null)
                {
                    if (Factory.State == CommunicationState.Opened)
                    {
                        Factory.Abort();
                    }
                }
                ErrorLogger.DumpToDebug(exc);
                throw new Exception("The microwave control is currently unavailable.", exc);
            }
        }

        /// <summary>
        /// Sets the frequency in MHz on the server
        /// </summary>
        public void SetTuning(MicrowaveTuning tuning)
        {
            if (ProxyReady)
            {
                Proxy.SetTuning(tuning);
            }
        }

        #endregion

        #region Properties

        private UInt64 _startFreq;
        /// <summary>
        /// Get or Set the Scan Sweep Start frequency (in Hz)
        /// </summary>
        public UInt64 ScanStartFrequency
        {
            get
            {
                return _startFreq;
            }
            set
            {
                if (_startFreq != value)
                {
                    _startFreq = value;
                    NotifyPropertyChanged("ScanStartFrequency");
                }
            }
        }

        private UInt64 _endFreq;
        /// <summary>
        /// Get or Set the Scan Sweep End frequency (in Hz)
        /// </summary>
        public UInt64 ScanEndFrequency
        {
            get
            {
                return _endFreq;
            }
            set
            {
                if (_endFreq != value)
                {
                    _endFreq = value;
                    NotifyPropertyChanged("ScanEndFrequency");
                }
            }
        }

        private int _scanThreshold;
        /// <summary>
        /// Get or Set the Scan Minimum Signal Strength Threshold (as a percentage or link quality rating)
        /// </summary>
        public int ScanThreshold
        {
            get
            {
                return _scanThreshold;
            }
            set
            {
                if (_scanThreshold != value)
                {
                    _scanThreshold = value;
                    NotifyPropertyChanged("ScanThreshold");
                }
            }
        }

        private MicrowaveCapabilities _capabilities;
        /// <summary>
        /// The capabilities handed down from the server
        /// </summary>
        public MicrowaveCapabilities Capabilities
        {
            get
            {
                return _capabilities;
            }
            internal set
            {
                if (_capabilities != value)
                {
                    _capabilities = value;
                    NotifyPropertyChanged("Capabilities");
                }
            }
        }

        private MicrowaveTuning _currentTuning  = null;
        /// <summary>
        /// The last reported tuning from the server
        /// </summary>
        public MicrowaveTuning CurrentTuning
        {
            get
            {
                return _currentTuning;
            }
            internal set
            {
                if (_currentTuning != value)
                {
                    _currentTuning = value;
                    NotifyPropertyChanged("CurrentTuning");
                }
            }
        }

        private MicrowaveLinkQuality _linkQuality = null;
        /// <summary>
        /// The last reported link quality data
        /// </summary>
        public MicrowaveLinkQuality LinkQuality
        {
            get
            {
                return _linkQuality;
            }
            internal set
            {
                if (_linkQuality != value)
                {
                    _linkQuality = value;
                    NotifyPropertyChanged("LinkQuality");
                }
            }
        }

        private UserPresetStore _presets = null;
        /// <summary>
        /// The last reported frequency presets
        /// </summary>
        public UserPresetStore FrequencyPresets
        {
            get
            {
                return _presets;
            }
            internal set
            {
                if (_presets != value)
                {
                    _presets = value;
                    NotifyPropertyChanged("FrequencyPresets");
                }
            }
        }

        #endregion

        #region IPresetProvider Members

        /// <summary>
        /// Indicate to the server to save the current options as a preset
        /// </summary>
        /// <returns>the server returns a <see cref="T:UserPresetItem"/> used to reference the new preset</returns>
        public UserPresetItem SavePreset()
        {
            if (ProxyReady)
            {
                return Proxy.SavePreset();
            }
            return null;
        }

        /// <summary>
        /// Restore the specified preset
        /// </summary>
        /// <param name="id">preset identifier to restore</param>
        public void RestorePreset(Guid id)
        {
            if (ProxyReady)
            {
                Proxy.RestorePreset(id);
            }
        }

        /// <summary>
        /// Update information attached to a preset on the server.
        /// The <see cref="P:UserPresetItem.ID"/> field must match an existing item for the update to occur.
        /// </summary>
        /// <param name="updatedItem">item with udpated information</param>
        /// <returns>true on success</returns>
        public bool UpdatePreset(UserPresetItem updatedItem)
        {
            if (ProxyReady)
            {
                return Proxy.UpdatePreset(updatedItem);
            }
            return false;
        }

        /// <summary>
        /// Delete the specified preset on the server
        /// </summary>
        /// <param name="id">preset identifier</param>
        /// <returns>true on success</returns>
        public bool DeletePreset(Guid id)
        {
            if (ProxyReady)
            {
                return Proxy.DeletePreset(id);
            }
            return false;
        }

        /// <summary>
        /// Deletes all presets on the server
        /// </summary>
        public void DeleteAllPresets()
        {
            if (ProxyReady)
            {
                Proxy.DeleteAllPresets();
            }
        }

        #endregion

        #region IChannelScanProvider Members

        /// <summary>
        /// Initiate a scan for frequencies/channels that the microwave receiver can tune.
        /// </summary>
        public void StartChannelScan()
        {
            if (ProxyReady)
            {
                Proxy.StartSweep(this.CurrentTuning, this.ScanStartFrequency, this.ScanEndFrequency, this.ScanThreshold);
            }
        }

        /// <summary>
        /// Cancel any channel scan in progress on the server
        /// </summary>
        public void CancelChannelScan()
        {
            if (ProxyReady)
            {
                Proxy.CancelSweep();
            }
        }

        /// <summary>
        /// Raised when the server has begun scanning for frequencies/channels
        /// </summary>
        public event EventHandler ChannelScanStarted;

        /// <summary>
        /// Raised when the server has made an change in progress for the frequency/channel scan
        /// </summary>
        public event EventHandler ChannelScanProgressUpdate;

        /// <summary>
        /// Raised when the server has completed the frequency/channel scan
        /// </summary>
        public event EventHandler<ChannelScanCompleteEventArgs> ChannelScanComplete;

        /// <summary>
        /// Gets the current channel scan progress. May return -1 if no scan is under way
        /// </summary>
        public int ChannelScanProgress
        {
            get;
            internal set;
        }

        internal void FireChannelScanComplete(ChannelScanCompleteEventArgs e)
        {
            Proxy.ToString();   //touch the Proxy property to avoid RelinquishTimer

            if (ChannelScanComplete != null)
            {
                ChannelScanComplete(this, e);
            }
        }

        internal void FireChannelScanStarted()
        {
            Proxy.ToString();   //touch the Proxy property to avoid RelinquishTimer

            if (ChannelScanStarted != null)
            {
                ChannelScanStarted(this, new EventArgs());
            }
        }

        internal void FireChannelScanProgressUpdate(int progress)
        {
            Proxy.ToString();   //touch the Proxy property to avoid RelinquishTimer

            this.ChannelScanProgress = progress;
            if (ChannelScanProgressUpdate != null)
            {
                ChannelScanProgressUpdate(this, new EventArgs());
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Detaches event handlers and closes the session
        /// </summary>
        public override void Dispose()
        {
            PropertyChanged = null;
            base.Dispose();
        }

        #endregion
    }
}
