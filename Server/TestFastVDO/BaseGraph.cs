using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Principal;

using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.Server.TestFastVDO
{
    public class BaseGraph : IDisposable
    {
        protected IGraphBuilder _graphBuilder;
        protected ICaptureGraphBuilder2 _captureGraphBuilder;
        protected IMediaControl _mediaControl;
        protected IBaseFilter _captureFilter;

        public BaseGraph()
        {
            int hr = 0;

            // An exception is thrown if cast fail
            _graphBuilder = (IGraphBuilder)new FilterGraph();
            _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            _mediaControl = (IMediaControl)_graphBuilder;

            // Attach the filter graph to the capture graph
            hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
            DsError.ThrowExceptionForHR(hr);
        }

        public void Dispose()
        {
            Debug.WriteLine("in BaseGraph.Dispose() -- why here?");
            GC.SuppressFinalize(this);
            Dispose(false);
        }

        public virtual void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            if (disposing)
            {
                if (_mediaControl != null)
                {
                    Marshal.ReleaseComObject(_mediaControl);
                    _mediaControl = null;
                }
                if (_graphBuilder != null)
                {
                    Marshal.ReleaseComObject(_graphBuilder);
                    _graphBuilder = null;
                }
                if (_captureGraphBuilder != null)
                {
                    Marshal.ReleaseComObject(_captureGraphBuilder);
                    _captureGraphBuilder = null;
                }
            }
        }

        ~BaseGraph()
        {
            Dispose(false);
        }

        public static BaseGraph CreateInstance()
        {
            return new FastVDO();
        }

        public virtual void Run()
        {
            int hr;

            if (_mediaControl != null)
            {
                hr = _mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public virtual void Stop()
        {
            int hr;

            if (_mediaControl != null)
            {
                hr = _mediaControl.Stop();
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public IBaseFilter AddFilter(IBaseFilter baseFilter, string friendlyName)
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

        public IBaseFilter AddFilterByName(Guid filterCategory, string friendlyName)
        {
            if (_graphBuilder == null)
            {
                throw new Exception("_graphBuilder == null");
            }
            if (filterCategory == null)
            {
                throw new Exception("filterCategory == null");
            }
            if (friendlyName == null)
            {
                throw new Exception("friendlyName == null");
            }
            Console.WriteLine("AddFilterByName {0} {1}", filterCategory.ToString(), friendlyName);
            IBaseFilter filter = FilterGraphTools.AddFilterByName(_graphBuilder, filterCategory, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        public IBaseFilter AddFilterByDevicePath(string devicePath, string friendlyName)
        {
            if (_graphBuilder == null)
            {
                throw new Exception("_graphBuilder is null");
            }
            if (devicePath == null)
            {
                throw new Exception("device path is null");
            }
            if (friendlyName == null)
            {
                throw new Exception("friendlyName is null");
            }
            IBaseFilter filter = FilterGraphTools.AddFilterByDevicePath(_graphBuilder, devicePath, friendlyName);
            if (filter == null)
            {
                throw new Exception("The DirectShow filter " + friendlyName + " not found.");
            }
            return filter;
        }

        public void ConnectFilters(IBaseFilter upFilter, string upPin, IBaseFilter downFilter, string downPin)
        {
            FilterGraphTools.ConnectFilters(_graphBuilder, upFilter, upPin, downFilter, downPin, true);
        }
    }
}
