using System;
using System.Diagnostics;
using System.Timers;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.DirectShowLib.Framework;
using FutureConcepts.Tools;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// A base graph that implements seeking functionality.
    /// </summary>
    /// <author>kdixon 11/20/2009</author>
    public abstract class SeekingGraph : BaseDSGraph
    {
        private IMediaSeeking _mediaSeeking;
        private IMediaPosition _mediaPosition;
        private IMediaEventEx _mediaEvent;

        private Timer _mediaSeekTimer;

        /// <summary>
        /// One second = 10000000 units of DirectShow reference time
        /// </summary>
        public const long DSReferenceSecond = 10000000;

        private const int MediaSeekTimerInterval = 150;

        /// <summary>
        /// Creates a new instance of the Seeking Graph
        /// </summary>
        public SeekingGraph() : base()
        {
            _mediaSeeking = (IMediaSeeking)graph;
            _mediaPosition = (IMediaPosition)graph;
            _mediaEvent = (IMediaEventEx)graph;

            _mediaSeekTimer = new Timer();
            _mediaSeekTimer.Interval = MediaSeekTimerInterval;
            _mediaSeekTimer.Elapsed += new ElapsedEventHandler(NotifyPlaybackPositionChanged);
        }

        /// <summary>
        /// Releases COM objects associated with seeking
        /// </summary>
        public override void Dispose()
        {
            lock (graphStateLock)
            {
                _mediaSeekTimer.Stop();
                _mediaSeekTimer.Dispose();
                _mediaSeekTimer = null;
                Release(_mediaSeeking);
                Release(_mediaPosition);
                Release(_mediaEvent);
                base.Dispose();
            }
        }

        /// <summary>
        /// Causes the graph to transition to the Running state
        /// </summary>
        public override void Run()
        {
            base.Run();
            _mediaSeekTimer.Start();
        }

        /// <summary>
        /// Causese the graph to transition to the Paused state
        /// </summary>
        public override void Pause()
        {
            base.Pause();
            _mediaSeekTimer.Start();
        }

        /// <summary>
        /// Causes the graph to transition to the Stopped state
        /// </summary>
        public override void Stop()
        {
            _mediaSeekTimer.Stop();
            base.Stop();
            this.Position = 0;
        }

        /// <summary>
        /// Causes the graph to Stop when ready and transition to the Stopped state
        /// </summary>
        public override void StopWhenReady()
        {
            _mediaSeekTimer.Stop();
            base.StopWhenReady();
        }

        /// <summary>
        /// Wait X milliseconds for the graph to receive the EC_COMPLETE
        /// </summary>
        /// <param name="msTimeout">number of milliseconds to timeout before returning, or -1 for INFINITE</param>
        /// <returns>
        /// Complete - Operation completed.
        /// ErrorAbort - Error. Playback cannot continue.
        /// UserAbort - User terminated the operation. 
        /// Unbuilt - Cannot execute this method
        /// 0 - graph is still running
        /// </returns>
        public EventCode WaitForCompletion(int msTimeout)
        {
            if (_mediaEvent != null)
            {
                EventCode e;
                int hr = _mediaEvent.WaitForCompletion(msTimeout, out e);
                if (DsError.Succeeded(hr))
                {
                    return e;
                }
                else
                {
                    return 0;
                }
            }
            return EventCode.Unbuilt;
        }

        /// <summary>
        /// Waits for the specified event.
        /// </summary>
        /// <param name="eventCode">event code to wait for</param>
        /// <param name="msTimeout">timeout</param>
        /// <returns>True if the event was received. False if timed out, or different event received</returns>
        public bool WaitForEvent(EventCode eventCode, int msTimeout)
        {
            EventCode waste1;
            int waste2;
            return WaitForEvent(eventCode, msTimeout, out waste1, out waste2);
        }

        /// <summary>
        /// Waits for the specified event.
        /// </summary>
        /// <param name="desiredEventCode">event code to wait for</param>
        /// <param name="msTimeout">timeout</param>
        /// <param name="actuallyReceivedEventCode">The event actually received, or "0" if no event was received</param>
        /// <param name="param">1st parameter associated with event</param>
        /// <returns>True if the desired event was received. False if timed out, or different event received</returns>
        public bool WaitForEvent(EventCode desiredEventCode, int msTimeout, out EventCode actuallyReceivedEventCode, out int param)
        {
            bool restartTimer = false;
            if(_mediaSeekTimer.Enabled)
            {
                _mediaSeekTimer.Stop();
                restartTimer = true;
            }

            int hr = 0;
            IntPtr lparam1, lparam2;

            hr = _mediaEvent.GetEvent(out actuallyReceivedEventCode, out lparam1, out lparam2, msTimeout);
            if (DsError.Succeeded(hr))
            {
                Debug.WriteLine("WaitForEvent got " + actuallyReceivedEventCode.ToString() + " 0x" + lparam1.ToString("X") + " 0x" + lparam2.ToString("X"));
                param = lparam1.ToInt32();
                _mediaEvent.FreeEventParams(actuallyReceivedEventCode, lparam1, lparam2);
            }
            else
            {
                actuallyReceivedEventCode = 0;
                param = 0;
            }

            if (restartTimer)
            {
                _mediaSeekTimer.Start();
            }

            return DsError.Succeeded(hr) && (actuallyReceivedEventCode == desiredEventCode);
        }

        #region Seeking Support

        private void NotifyPlaybackPositionChanged(object sender, ElapsedEventArgs e)
        {
            if (_mediaSeekTimer == null)
            {
                return;
            }
            try
            {
                _mediaSeekTimer.Stop();
                NotifyPropertyChanged("Position");

                //pause the transport if it reaches the end
                EventCode eventCode;
                IntPtr lparam1, lparam2;
                while (_mediaEvent.GetEvent(out eventCode, out lparam1, out lparam2, 10) == 0)
                {
                    Debug.WriteLine("DirectShow event " + eventCode.ToString() + " 0x" + lparam1.ToString("X") + " 0x" + lparam2.ToString("X"));
                    OnDirectShowEvent(eventCode, lparam1, lparam2);
                    if (eventCode == EventCode.Complete)
                    {
                        OnGraphComplete();
                    }
                    _mediaEvent.FreeEventParams(eventCode, lparam1, lparam2);
                }

                _mediaSeekTimer.Start();
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Override this method to handle directshow events
        /// </summary>
        /// <param name="eventCode">Event that occurred</param>
        /// <param name="lparam1">param 1</param>
        /// <param name="lparam2">param 2</param>
        protected virtual void OnDirectShowEvent(EventCode eventCode, IntPtr lparam1, IntPtr lparam2)
        {
            //we don't do anything, but our subclasses might
        }

        /// <summary>
        /// An overrideable method that will be fired when the graph generates the EC_COMPLETE event
        /// </summary>
        protected virtual void OnGraphComplete()
        {
            Pause();
        }

        private bool _seek_canSeek;
        private bool _pos_canSeek;
        private bool _seek_canGetDuration;
        private bool _pos_canGetDuration;
        private bool _seek_canGetCurrentPos;
        private bool _pos_canGetCurrentPos;

        /// <summary>
        /// Get/Set the current playback position in DSReferenceTime
        /// </summary>
        public virtual long Position
        {
            get
            {
                try
                {
                    //prefer IMediaSeeking interface
                    if (_seek_canGetCurrentPos)
                    {
                        long pos;
                        int hr = _mediaSeeking.GetCurrentPosition(out pos);
                        DsError.ThrowExceptionForHR(hr);
                        return pos;
                    }
                    else if (_pos_canGetCurrentPos)  //fall back on IMediaPosition
                    {
                        double pos;
                        int hr = _mediaPosition.get_CurrentPosition(out pos);
                        DsError.ThrowExceptionForHR(hr);
                        return (long)(pos * DSReferenceSecond);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                }

                return -1;
            }
            set
            {
                int hr;
                if (_seek_canSeek)
                {
                    hr = _mediaSeeking.SetPositions(value, AMSeekingSeekingFlags.AbsolutePositioning,
                                                    null, AMSeekingSeekingFlags.NoPositioning);
                }
                else if (_pos_canSeek)
                {
                    hr = _mediaPosition.put_CurrentPosition((double)(value / DSReferenceSecond));
                }
                else
                {
                    return;
                }
                NotifyPropertyChanged("Position");
            }
        }

        /// <summary>
        /// Returns a textual representation of the current Position of the graph in time.
        /// </summary>
        public string PositionString
        {
            get
            {
                return GetTimeString(this.Position / DSReferenceSecond);
            }
        }

        /// <summary>
        /// Returns a string representation of a given time, in the format m:ss, or h:mm:ss if neccesary
        /// </summary>
        /// <param name="timeInSeconds">the time to represent, in seconds</param>
        /// <returns></returns>
        private string GetTimeString(long timeInSeconds)
        {
            string ret;

            //do seconds
            long secs = timeInSeconds % 60;

            //do hours
            long hours = timeInSeconds / 3600;
            timeInSeconds -= (hours * 3600);

            //do minutes
            long mins = timeInSeconds / 60;

            if (hours > 0)
                ret = hours.ToString() + ":" + mins.ToString().PadLeft(2, '0') + ":" + secs.ToString().PadLeft(2, '0');
            else
                ret = mins.ToString() + ":" + secs.ToString().PadLeft(2, '0');

            return ret;
        }

        /// <summary>
        /// Retreives the Duration of the media in DSReferenceTime
        /// </summary>
        public virtual long Duration
        {
            get
            {
                try
                {
                    //prefer IMediaSeeking interface
                    if (_seek_canGetDuration)
                    {
                        long duration;
                        int hr = _mediaSeeking.GetDuration(out duration);
                        DsError.ThrowExceptionForHR(hr);
                        return duration;
                    }
                    else if (_pos_canGetDuration)   //fall back on IMediaPosition
                    {
                        double duration;
                        int hr = _mediaPosition.get_Duration(out duration);
                        DsError.ThrowExceptionForHR(hr);
                        return (long)(duration * DSReferenceSecond);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                }

                return -1;
            }
        }

        /// <summary>
        /// Retrieves a textual representation of the duration of the video
        /// </summary>
        public string DurationString
        {
            get
            {
                return GetTimeString(this.Duration / DSReferenceSecond);
            }
        }

        /// <summary>
        /// Returns true if setting the Position property will have an effect
        /// </summary>
        public bool SupportsSeeking
        {
            get
            {
                return (_seek_canSeek || _pos_canSeek) && (_seek_canGetCurrentPos || _pos_canGetCurrentPos);
            }
        }

        /// <summary>
        /// Returns true if the Position property will return valid data
        /// </summary>
        public bool SupportsPosition
        {
            get
            {
                return _seek_canGetCurrentPos || _pos_canGetCurrentPos;
            }
        }

        /// <summary>
        /// Returns true if the Duration property will return valid data
        /// </summary>
        public bool SupportsDuration
        {
            get
            {
                return _seek_canGetDuration || _pos_canGetDuration;
            }
        }

        /// <summary>
        /// Queries the current video source for its capabilities regarding seeking and time info.
        /// The graph should be fully constructed for accurate information
        /// </summary>
        protected void QuerySeekingCapabilities()
        {
            try
            {
                _mediaSeeking.SetTimeFormat(TimeFormat.MediaTime);
                //get capabilities from the graph, and see what it supports that interests us
                AMSeekingSeekingCapabilities caps;
                int r = _mediaSeeking.GetCapabilities(out caps);
                long lTest = 0;
                double dblTest = 0;
                if (r != 0)
                {
                    _seek_canGetCurrentPos = false;
                    _seek_canSeek = false;
                    _seek_canGetDuration = false;
                }
                else    //if we were able to read the capabilities, then determine if the capability works, both by checking the
                // advertisement, and actually trying it out.
                {
                    _seek_canSeek = ((caps & AMSeekingSeekingCapabilities.CanSeekAbsolute) == AMSeekingSeekingCapabilities.CanSeekAbsolute) &&
                                     (_mediaSeeking.SetPositions(0, AMSeekingSeekingFlags.AbsolutePositioning,
                                                                 null, AMSeekingSeekingFlags.NoPositioning) == 0);

                    _seek_canGetDuration = ((caps & AMSeekingSeekingCapabilities.CanGetDuration) == AMSeekingSeekingCapabilities.CanGetDuration) &&
                                            (_mediaSeeking.GetDuration(out lTest) == 0);

                    _seek_canGetCurrentPos = ((caps & AMSeekingSeekingCapabilities.CanGetCurrentPos) == AMSeekingSeekingCapabilities.CanGetCurrentPos) &&
                                              (_mediaSeeking.GetCurrentPosition(out lTest) == 0);
                }

                //check capabilities for the IMediaPosition interface
                _pos_canSeek = (_mediaPosition.put_CurrentPosition(0) == 0);
                _pos_canGetDuration = (_mediaPosition.get_Duration(out dblTest) == 0);
                _pos_canGetCurrentPos = (_mediaPosition.get_CurrentPosition(out dblTest) == 0);
            }
            catch (Exception)
            {
                _seek_canSeek = false;
                _pos_canSeek = false;
            }
        }

        #endregion
    }
}
