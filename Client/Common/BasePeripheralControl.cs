using System;
using System.Diagnostics;
using System.Timers;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client
{
    /// <summary>
    /// This class provides base common functionality for WCF clients that implement peripheral controls,
    /// such as PTZ control, or Microwave RX control.
    /// </summary>
    /// <remarks>
    /// Adds the concept of Relinquish Timer that gets reset whenever the Proxy property is touched
    /// </remarks>
    /// <author>kdixon 05/14/2009</author>
    /// <typeparam name="TContract">The WCF ServiceContract to use</typeparam>
    public abstract class BasePeripheralControl<TContract> : BaseClient<TContract>
    {
        /// <param name="serverHostOrIP">server hostname or IP that is hosting the service</param>
        public BasePeripheralControl(string serverHostOrIP) : base(serverHostOrIP) { }

        /// <summary>
        /// Current instance of the proxy
        /// </summary>
        protected override TContract Proxy
        {
            get
            {
                if ((base.Proxy != null) && (relinquishTimer != null))
                {
                    relinquishTimer.Stop();
                    relinquishTimer.Start();
                }
                return base.Proxy;
            }
            set
            {
                if (relinquishTimer != null)
                {
                    relinquishTimer.Stop();
                    if (value != null)
                    {
                        relinquishTimer.Start();
                    }
                }
                base.Proxy = value;
            }
        }

        /// <summary>
        /// The relinquish timer
        /// </summary>
        private Timer relinquishTimer;

        private int _relinquishTimeout = 0;
        /// <summary>
        /// The amount of time in milliseconds to relinquish control. Set to less than 0 to disable.
        /// If this time period elapses without any gets/sets on the Proxy property, the channel will
        /// be closed and the Closed event raised.
        /// </summary>
        public int RelinquishTimeout
        {
            get
            {
                return _relinquishTimeout;
            }
            set
            {
                if (relinquishTimer != null)
                {
                    relinquishTimer.Stop();
                }

                _relinquishTimeout = value;

                if (value > 0)
                {
                    if (relinquishTimer == null)
                    {
                        relinquishTimer = new System.Timers.Timer();
                        relinquishTimer.Elapsed += new ElapsedEventHandler(relinquishTimer_Elapsed);
                    }
                    
                    relinquishTimer.Interval = _relinquishTimeout;
                    relinquishTimer.Start();
                }

            }
        }

        private void relinquishTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Debug.WriteLine("BasePeripheralControl.reqlinquishTimer_Elapsed");
                relinquishTimer.Stop();
                this.OnRelinquishTimeout();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in BasePeripheralControl.reqlinquishTimer_Elapsed.");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Closes the factory or aborts it as needed. Ensures that the Closed event is raised.
        /// </summary>
        public override void Close()
        {
            if (relinquishTimer != null)
            {
                relinquishTimer.Enabled = false;
                relinquishTimer.Dispose();
                relinquishTimer = null;
            }

            base.Close();
        }

        /// <summary>
        /// Aborts the proxy factory in the case that it is faulted, or otherwise worthless
        /// </summary>
        public override void Abort()
        {
            if (relinquishTimer != null)
            {
                relinquishTimer.Enabled = false;
                relinquishTimer.Dispose();
                relinquishTimer = null;
            }

            base.Abort();
        }

        /// <summary>
        /// Called when the Relinquish Timeout has elapsed.
        /// </summary>
        protected virtual void OnRelinquishTimeout()
        {
            Close();
        }
    }
}
