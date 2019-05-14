using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FutureConcepts.Tools;
using System.ComponentModel;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Base-bones implementation of a DirectShow graph
    /// </summary>
    public abstract class BaseDSGraph : IGraphControl, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The IGraphBuilder
        /// </summary>
        protected IGraphBuilder graph;
        /// <summary>
        /// The ICaptureGraphBuilder2
        /// </summary>
        protected ICaptureGraphBuilder2 captureGraph;
        /// <summary>
        /// The IMediaControl
        /// </summary>
        protected IMediaControl mediaControl;

        /// <summary>
        /// Hold this object when touching objects that are referenced in the constructor or dispose method
        /// </summary>
        protected readonly object graphStateLock = new object();

        /// <summary>
        /// Construts an empty DirectShow graph
        /// </summary>
        public BaseDSGraph()
        {
            int hr;

            // An exception is thrown if cast fail
            graph = (IGraphBuilder)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)graph;

            // Attach the filter graph to the capture graph
            hr = captureGraph.SetFiltergraph(graph);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Runs the graph
        /// </summary>
        public virtual void Run()
        {
#if DEBUG_SHOW_STATECALLS
            Debug.WriteLine(this.GetType().ToString() + ".Run()");
#endif
            lock (graphStateLock)
            {
                if (mediaControl != null)
                {
                    int hr = mediaControl.Run();
                    DsError.ThrowExceptionForHR(hr);
                    this.State = State.Running;
                }
            }
        }

        /// <summary>
        /// Stops the graph
        /// </summary>
        public virtual void Stop()
        {
#if DEBUG_SHOW_STATECALLS
            Debug.WriteLine(this.GetType().ToString() + ".Stop()");
#endif
            lock (graphStateLock)
            {
                if (mediaControl != null)
                {
                    int hr = mediaControl.Stop();
                    DsError.ThrowExceptionForHR(hr);
                    this.State = State.Stopped;
                }
            }
        }

        /// <summary>
        /// Stops the graph once buffers have been queued.
        /// </summary>
        public virtual void StopWhenReady()
        {
#if DEBUG_SHOW_STATECALLS
            Debug.WriteLine(this.GetType().ToString() + ".StopWhenReady()");
#endif
            lock (graphStateLock)
            {
                if (mediaControl != null)
                {
                    int hr = mediaControl.StopWhenReady();
                    DsError.ThrowExceptionForHR(hr);
                    this.State = State.Stopped;
                }
            }
        }

        /// <summary>
        /// Pauses the graph
        /// </summary>
        public virtual void Pause()
        {
#if DEBUG_SHOW_STATECALLS
            Debug.WriteLine(this.GetType().ToString() + ".Pause()");
#endif
            lock (graphStateLock)
            {
                if (mediaControl != null)
                {
                    int hr = mediaControl.Pause();
                    DsError.ThrowExceptionForHR(hr);
                    this.State = State.Paused;
                }
            }
        }

        /// <summary>
        /// Attempts to abort the graph
        /// </summary>
        public virtual void Abort()
        {
#if DEBUG_SHOW_STATECALLS
            Debug.WriteLine(this.GetType().ToString() + ".Abort()");
#endif
            lock (graphStateLock)
            {
                if (graph != null)
                {
                    graph.Abort();
                }
            }
        }

        private State _state = State.Stopped;
        /// <summary>
        /// Gets the State of the DirectShow graph
        /// </summary>
        public virtual State State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

        /// <summary>
        /// Decrements a COM object reference. If reference count is less than or equal to zero, the the object is set to null
        /// </summary>
        /// <param name="o">Object to release</param>
        internal protected static void Release(object o)
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
        /// Decrements the reference count to 0
        /// </summary>
        /// <param name="o">object to release</param>
        internal protected static void ReleaseForce(object o)
        {
            if (o != null)
            {
                int refcount = 0;
                do
                {
                    refcount = Marshal.ReleaseComObject(o);
                }
                while (refcount > 0);
            }
        }

        #region Eventing & Handlers

        /// <summary>
        /// Raised whena property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Use this to raise a property changed event for the specified property
        /// </summary>
        /// <param name="property">property name that changed</param>
        protected void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the graph
        /// </summary>
        public virtual void Dispose()
        {
            PropertyChanged = null;

            lock (graphStateLock)
            {
                Release(mediaControl);
                Release(captureGraph);
                Release(graph);
            }
        }

        #endregion
    }
}
