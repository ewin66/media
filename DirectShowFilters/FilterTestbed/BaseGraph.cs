using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using FutureConcepts.Media.DirectShowLib;


namespace FutureConcepts.Media.DirectShowFilters.FilterTestbed
{
    public abstract class BaseGraph : IDisposable
    {
        protected IGraphBuilder _graphBuilder;
        protected IMediaControl _mediaControl;
        protected ICaptureGraphBuilder2 _captureGraphBuilder;

        protected BaseGraph()
        {
            int hr;

            //Create the Graph
            _graphBuilder = (IGraphBuilder)new FilterGraph();
            _mediaControl = (IMediaControl)_graphBuilder;
            Debug.Assert(_mediaControl != null);

            //Create the Capture Graph Builder
            _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

            // Attach the filter graph to the capture graph
            hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
            DsError.ThrowExceptionForHR(hr);
        }

        public virtual void Dispose()
        {
            ReleaseComObject(_mediaControl);
            _mediaControl = null;
            ReleaseComObject(_graphBuilder);
            _graphBuilder = null;
            ReleaseComObject(_captureGraphBuilder);
            _captureGraphBuilder = null;
        }

        /// <summary>
        /// Disposes a COM object
        /// </summary>
        /// <remarks>
        /// calls Marshal.ReleaseComObject and then sets the reference to null
        /// </remarks>
        /// <param name="o">COM object to dispose</param>
        protected virtual void ReleaseComObject(object o)
        {
            if (o != null)
            {
                int refcount = Marshal.ReleaseComObject(o);
                if (refcount <= 0)
                {
                    o = null;
                }
            }
        }

        public enum GraphState { Stopped, Paused, Running };

        /// <summary>
        /// Set or Get the state of the graph.
        /// </summary>
        public virtual GraphState State
        {
            get;
            set;
        }

        public virtual void Run()
        {
            int hr = _mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
            State = GraphState.Running;
        }

        public void Pause()
        {
            int hr = _mediaControl.Pause();
            DsError.ThrowExceptionForHR(hr);
            State = GraphState.Paused;
        }

        public void Stop()
        {
            int hr = _mediaControl.Stop();
            DsError.ThrowExceptionForHR(hr);
            State = GraphState.Stopped;
        }

        protected void StopWhenReady()
        {
            int hr = _mediaControl.StopWhenReady();
            DsError.ThrowExceptionForHR(hr);
            State = GraphState.Stopped;
        }
    }
}
