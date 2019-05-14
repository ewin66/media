/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace FutureConcepts.Media.DirectShowLib
{
    /// <summary>
    /// A collection of methods to do common DirectShow tasks.
    /// </summary>

    public sealed class FilterGraphTools
    {
        private FilterGraphTools(){}

        /// <summary>
        /// Add a filter to a DirectShow Graph using its CLSID
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="clsid">a valid CLSID. This object must implement IBaseFilter</param>
        /// <param name="name">the name used in the graph (may be null)</param>
        /// <returns>an instance of the filter if the method successfully created it, null if not</returns>
        /// <remarks>
        /// You can use <see cref="IsThisComObjectInstalled">IsThisComObjectInstalled</see> to check is the CLSID is valid before calling this method
        /// </remarks>
        /// <example>This sample shows how to programmatically add a NVIDIA Video decoder filter to a graph
        /// <code>
        /// Guid nvidiaVideoDecClsid = new Guid("71E4616A-DB5E-452B-8CA5-71D9CC7805E9");
        /// 
        /// if (FilterGraphTools.IsThisComObjectInstalled(nvidiaVideoDecClsid))
        /// {
        ///   filter = FilterGraphTools.AddFilterFromClsid(graphBuilder, nvidiaVideoDecClsid, "NVIDIA Video Decoder");
        /// }
        /// else
        /// {
        ///   // use another filter...
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="IsThisComObjectInstalled"/>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if errors occur when the filter is add to the graph</exception>

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static IBaseFilter AddFilterFromClsid(IGraphBuilder graphBuilder, Guid clsid, string name)
        {
            int hr = 0;
            IBaseFilter filter = null;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            try
            {
                Type type = Type.GetTypeFromCLSID(clsid);
                filter = (IBaseFilter) Activator.CreateInstance(type);

                hr = graphBuilder.AddFilter(filter, name);
                DsError.ThrowExceptionForHR(hr);
            }
            catch
            {
                if (filter != null)
                {
                    Marshal.ReleaseComObject(filter);
                    filter = null;
                }
            }

            return filter;
        }

        /// <summary>
        /// Add a filter to a DirectShow Graph using its name
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="deviceCategory">the filter category (see DirectShowLib.FilterCategory)</param>
        /// <param name="friendlyName">the filter name (case-sensitive)</param>
        /// <returns>an instance of the filter if the method successfully created it, null if not</returns>
        /// <example>This sample shows how to programmatically add a NVIDIA Video decoder filter to a graph
        /// <code>
        /// filter = FilterGraphTools.AddFilterByName(graphBuilder, FilterCategory.LegacyAmFilterCategory, "NVIDIA Video Decoder");
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if errors occur when the filter is add to the graph</exception>

        public static IBaseFilter AddFilterByName(IGraphBuilder graphBuilder, Guid deviceCategory, string friendlyName)
        {
            int hr = 0;
            IBaseFilter filter = null;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            DsDevice[] devices = DsDevice.GetDevicesOfCat(deviceCategory);

            for(int i = 0; i < devices.Length; i++)
            {
                if (devices[i].Name != null)
                {
                    if (!devices[i].Name.Equals(friendlyName))
                        continue;

                    hr = (graphBuilder as IFilterGraph2).AddSourceFilterForMoniker(devices[i].Mon, null, friendlyName, out filter);
                    DsError.ThrowExceptionForHR(hr);

                    break;
                }
            }

            return filter;
        }

        public static Boolean FilterExists(Guid deviceCategory, string friendlyName)
        {
            Boolean result = false;

            DsDevice[] devices = DsDevice.GetDevicesOfCat(deviceCategory);

            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].Name != null)
                {
                    if (!devices[i].Name.Equals(friendlyName))
                    {
                        continue;
                    }
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Add a filter to a DirectShow Graph using its Moniker's device path
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="devicePath">a moniker path</param>
        /// <param name="name">the name to use for the filter in the graph</param>
        /// <returns>an instance of the filter if the method successfully creates it, null if not</returns>
        /// <example>This sample shows how to programmatically add a NVIDIA Video decoder filter to a graph
        /// <code>
        /// string devicePath = @"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{71E4616A-DB5E-452B-8CA5-71D9CC7805E9}";
        /// filter = FilterGraphTools.AddFilterByDevicePath(graphBuilder, devicePath, "NVIDIA Video Decoder");
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if errors occur when the filter is add to the graph</exception>

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static IBaseFilter AddFilterByDevicePath(IGraphBuilder graphBuilder, string devicePath, string name)
        {
            int hr = 0;
            IBaseFilter filter = null;
#if USING_NET11
			UCOMIBindCtx bindCtx = null;
			UCOMIMoniker moniker = null;
#else
			IBindCtx bindCtx = null;
			IMoniker moniker = null;
#endif
			int eaten;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            try
            {
                hr = NativeMethods.CreateBindCtx(0, out bindCtx);
                Marshal.ThrowExceptionForHR(hr);

                hr = NativeMethods.MkParseDisplayName(bindCtx, devicePath, out eaten, out moniker);
                Marshal.ThrowExceptionForHR(hr);

                hr = (graphBuilder as IFilterGraph2).AddSourceFilterForMoniker(moniker, bindCtx, name, out filter);
                DsError.ThrowExceptionForHR(hr);
            }
            catch
            {
                // An error occur. Just returning null...
            }
            finally
            {
                if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);
                if (moniker != null) Marshal.ReleaseComObject(moniker);
            }

            return filter;
        }

        /// <summary>
        /// Find a filter in a DirectShow Graph using its name
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="filterName">the filter name to find (case-sensitive)</param>
        /// <returns>an instance of the filter if found, null if not</returns>
        /// <seealso cref="FindFilterByClsid"/>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static IBaseFilter FindFilterByName(IGraphBuilder graphBuilder, string filterName)
        {
            int hr = 0;
            IBaseFilter filter = null;
            IEnumFilters enumFilters = null;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            hr = graphBuilder.EnumFilters(out enumFilters);
            if (hr == 0)
            {
                IBaseFilter[] filters = new IBaseFilter[1];

                while (enumFilters.Next(filters.Length, filters, IntPtr.Zero) == 0)
                {
                    FilterInfo filterInfo;

                    hr = filters[0].QueryFilterInfo(out filterInfo);
                    if (hr == 0)
                    {
                        if (filterInfo.pGraph != null)
                            Marshal.ReleaseComObject(filterInfo.pGraph);

                        if (filterInfo.achName.Equals(filterName))
                        {
                            filter = filters[0];
                            break;
                        }
                    }

                    Marshal.ReleaseComObject(filters[0]);
                }
                Marshal.ReleaseComObject(enumFilters);
            }

            return filter;
        }

        /// <summary>
        /// Find a filter in a DirectShow Graph using its CLSID
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="filterClsid">the CLSID to find</param>
        /// <returns>an instance of the filter if found, null if not</returns>
        /// <seealso cref="FindFilterByName"/>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static IBaseFilter FindFilterByClsid(IGraphBuilder graphBuilder, Guid filterClsid)
        {
            int hr = 0;
            IBaseFilter filter = null;
            IEnumFilters enumFilters = null;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            hr = graphBuilder.EnumFilters(out enumFilters);
            if (hr == 0)
            {
                IBaseFilter[] filters = new IBaseFilter[1];
                while (enumFilters.Next(filters.Length, filters, IntPtr.Zero) == 0)
                {
                    Guid clsid;

                    hr = filters[0].GetClassID(out clsid);

                    if ((hr == 0) && (clsid == filterClsid))
                    {
                        filter = filters[0];
                        break;
                    }

                    Marshal.ReleaseComObject(filters[0]);
                }
                Marshal.ReleaseComObject(enumFilters);
            }

            return filter;
        }

        /// <summary>
        /// Render a filter's pin in a DirectShow Graph
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="source">the filter containing the pin to render</param>
        /// <param name="pinName">the pin name</param>
        /// <returns>true if rendering is a success, false if not</returns>
        /// <example>
        /// <code>
        /// hr = graphBuilder.AddSourceFilter(@"foo.avi", "Source Filter", out filter);
        /// DsError.ThrowExceptionForHR(hr);
        /// 
        /// if (!FilterGraphTools.RenderPin(graphBuilder, filter, "Output"))
        /// {
        ///   // Something went wrong...
        /// }
        /// </code>
        /// </example>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder or source is null</exception>
        /// <remarks>This method assumes that the filter is part of the given graph</remarks>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static bool RenderPin(IGraphBuilder graphBuilder, IBaseFilter source, string pinName)
        {
            int hr = 0;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            if (source == null)
                throw new ArgumentNullException("source");

            IPin pin = DsFindPin.ByName(source, pinName);

            if (pin != null)
            {
                hr = graphBuilder.Render(pin);
                Marshal.ReleaseComObject(pin);

                return (hr >= 0);
            }

            return false;
        }

        /// <summary>
        /// Disconnect all pins on a given filter
        /// </summary>
        /// <param name="filter">the filter on which to disconnect all the pins</param>
        /// <exception cref="System.ArgumentNullException">Thrown if filter is null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if errors occured during the disconnection process</exception>
        /// <remarks>Both input and output pins are disconnected</remarks>

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void DisconnectPins(IBaseFilter filter)
        {
            int hr = 0;

            if (filter == null)
                throw new ArgumentNullException("filter");

            IEnumPins enumPins;
            IPin[] pins = new IPin[1];
            hr = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                while(enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
                {
                    try
                    {
                        hr = pins[0].Disconnect();
                        DsError.ThrowExceptionForHR(hr);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(pins[0]);
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(enumPins);
            }
        }

        /// <summary>
        /// Disconnect pins of all the filters in a DirectShow Graph
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if the method can't enumerate its filters</exception>
        /// <remarks>This method doesn't throw an exception if an error occurs during pin disconnections</remarks>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void DisconnectAllPins(IGraphBuilder graphBuilder)
        {
            int hr = 0;
            IEnumFilters enumFilters;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            hr = graphBuilder.EnumFilters(out enumFilters);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                IBaseFilter[] filters = new IBaseFilter[1];

                while(enumFilters.Next(filters.Length, filters, IntPtr.Zero) == 0)
                {
                    try
                    {
                        DisconnectPins(filters[0]);
                    }
                    catch{}
                    Marshal.ReleaseComObject(filters[0]);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(enumFilters);
            }
        }

        /// <summary>
        /// Remove and release all filters from a DirectShow Graph
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if the method can't enumerate its filters</exception>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void RemoveAllFilters(IGraphBuilder graphBuilder)
        {
            int hr = 0;
            IEnumFilters enumFilters;
            ArrayList filtersArray = new ArrayList();

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            hr = graphBuilder.EnumFilters(out enumFilters);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                IBaseFilter[] filters = new IBaseFilter[1];

                while(enumFilters.Next(filters.Length, filters, IntPtr.Zero) == 0)
                {
                    filtersArray.Add(filters[0]);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(enumFilters);
            }

            foreach(IBaseFilter filter in filtersArray)
            {
                hr = graphBuilder.RemoveFilter(filter);
                Marshal.ReleaseComObject(filter);
            }
        }

        /// <summary>
        /// Save a DirectShow Graph to a GRF file
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="fileName">the file to be saved</param>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if errors occur during the file creation</exception>
        /// <seealso cref="LoadGraphFile"/>
        /// <remarks>This method overwrites any existing file</remarks>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void SaveGraphFile(IGraphBuilder graphBuilder, string fileName)
        {
            int hr = 0;
            IStorage storage = null;
#if USING_NET11
            UCOMIStream stream = null;
#else
            IStream stream = null;
#endif

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            try
            {
                hr = NativeMethods.StgCreateDocfile(
                    fileName, 
                    STGM.Create | STGM.Transacted | STGM.ReadWrite | STGM.ShareExclusive,
                    0,
                    out storage
                    );

                Marshal.ThrowExceptionForHR(hr);

                hr = storage.CreateStream(
                    @"ActiveMovieGraph",
                    STGM.Write | STGM.Create | STGM.ShareExclusive,
                    0,
                    0,
                    out stream
                    );

                Marshal.ThrowExceptionForHR(hr);

                hr = (graphBuilder as IPersistStream).Save(stream, true);
                Marshal.ThrowExceptionForHR(hr);

                hr = storage.Commit(STGC.Default);
                Marshal.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (stream != null)
                    Marshal.ReleaseComObject(stream);
                if (storage != null)
                    Marshal.ReleaseComObject(storage);
            }
        }

        /// <summary>
        /// Load a DirectShow Graph from a file
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="fileName">the file to be loaded</param>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder is null</exception>
        /// <exception cref="System.ArgumentException">Thrown if the given file is not a valid graph file</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if errors occur during loading</exception>
        /// <seealso cref="SaveGraphFile"/>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void LoadGraphFile(IGraphBuilder graphBuilder, string fileName)
        {
            int hr = 0;
            IStorage storage = null;
#if USING_NET11
			UCOMIStream stream = null;
#else
			IStream stream = null;
#endif

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            try
            {
                if (NativeMethods.StgIsStorageFile(fileName) != 0)
                    throw new ArgumentException();

                hr = NativeMethods.StgOpenStorage(
                    fileName,
                    null,
                    STGM.Transacted | STGM.Read | STGM.ShareDenyWrite,
                    IntPtr.Zero,
                    0,
                    out storage
                    );

                Marshal.ThrowExceptionForHR(hr);

                hr = storage.OpenStream(
                    @"ActiveMovieGraph",
                    IntPtr.Zero,
                    STGM.Read | STGM.ShareExclusive,
                    0,
                    out stream
                    );

                Marshal.ThrowExceptionForHR(hr);

                hr = (graphBuilder as IPersistStream).Load(stream);
                Marshal.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (stream != null)
                    Marshal.ReleaseComObject(stream);
                if (storage != null)
                    Marshal.ReleaseComObject(storage);
            }
        }

        /// <summary>
        /// Check if a DirectShow filter can display Property Pages
        /// </summary>
        /// <param name="filter">A DirectShow Filter</param>
        /// <exception cref="System.ArgumentNullException">Thrown if filter is null</exception>
        /// <seealso cref="ShowFilterPropertyPage"/>
        /// <returns>true if the filter has Property Pages, false if not</returns>
        /// <remarks>
        /// This method is intended to be used with <see cref="ShowFilterPropertyPage">ShowFilterPropertyPage</see>
        /// </remarks>
    
        public static bool HasPropertyPages(IBaseFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            return ((filter as ISpecifyPropertyPages) != null);
        }

        /// <summary>
        /// Display Property Pages of a given DirectShow filter
        /// </summary>
        /// <param name="filter">A DirectShow Filter</param>
        /// <param name="parent">A hwnd handle of a window to contain the pages</param>
        /// <exception cref="System.ArgumentNullException">Thrown if filter is null</exception>
        /// <seealso cref="HasPropertyPages"/>
        /// <remarks>
        /// You can check if a filter supports Property Pages with the <see cref="HasPropertyPages">HasPropertyPages</see> method.<br/>
        /// <strong>Warning</strong> : This method is blocking. It only returns when the Property Pages are closed.
        /// </remarks>
        /// <example>This sample shows how to check if a filter supports Property Pages and displays them
        /// <code>
        /// if (FilterGraphTools.HasPropertyPages(myFilter))
        /// {
        ///   FilterGraphTools.ShowFilterPropertyPage(myFilter, myForm.Handle);
        /// }
        /// </code>
        /// </example>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void ShowFilterPropertyPage(IBaseFilter filter, IntPtr parent)
        {
            int hr = 0;
            FilterInfo filterInfo;
            DsCAUUID caGuid;
            object[] objs;

            if (filter == null)
                throw new ArgumentNullException("filter");

            if (HasPropertyPages(filter))
            {
                hr = filter.QueryFilterInfo(out filterInfo);
                DsError.ThrowExceptionForHR(hr);

                if (filterInfo.pGraph != null)
                    Marshal.ReleaseComObject(filterInfo.pGraph);

                hr = (filter as ISpecifyPropertyPages).GetPages(out caGuid);
                DsError.ThrowExceptionForHR(hr);

                try
                {
                    objs = new object[1];
                    objs[0] = filter;

                    NativeMethods.OleCreatePropertyFrame(
                        parent, 0, 0, 
                        filterInfo.achName, 
                        objs.Length, objs, 
                        caGuid.cElems, caGuid.pElems, 
                        0, 0, 
                        IntPtr.Zero
                        );
                }
                finally
                {
                    Marshal.FreeCoTaskMem(caGuid.pElems);
                }
            }
        }

        /// <summary>
        /// Check if a COM Object is available
        /// </summary>
        /// <param name="clsid">The CLSID of this object</param>
        /// <example>This sample shows how to check if the MPEG-2 Demultiplexer filter is available
        /// <code>
        /// if (FilterGraphTools.IsThisComObjectInstalled(typeof(MPEG2Demultiplexer).GUID))
        /// {
        ///   // Use it...
        /// }
        /// </code>
        /// </example>
        /// <returns>true if the object is available, false if not</returns>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static bool IsThisComObjectInstalled(Guid clsid)
        {
            bool retval = false;

            try
            {
                Type type = Type.GetTypeFromCLSID(clsid);
                object o = Activator.CreateInstance(type);
                retval = true;
                Marshal.ReleaseComObject(o);
            }
            catch{}

            return retval;
        }

        /// <summary>
        /// Check if the Video Mixing Renderer 9 Filter is available
        /// <seealso cref="IsThisComObjectInstalled"/>
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="IsThisComObjectInstalled">IsThisComObjectInstalled</see> internally
        /// </remarks>
        /// <returns>true if VMR9 is present, false if not</returns>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static bool IsVMR9Present()
        {
            return IsThisComObjectInstalled(typeof(VideoMixingRenderer9).GUID);
        }

        /// <summary>
        /// Check if the Video Mixing Renderer 7 Filter is available
        /// <seealso cref="IsThisComObjectInstalled"/>
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="IsThisComObjectInstalled">IsThisComObjectInstalled</see> internally
        /// </remarks>
        /// <returns>true if VMR7 is present, false if not</returns>
    
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static bool IsVMR7Present()
        {
            return IsThisComObjectInstalled(typeof(VideoMixingRenderer).GUID);
        }

        /// <summary>
        /// Connect pins from two filters
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="upFilter">the upstream filter</param>
        /// <param name="sourcePinName">the upstream filter pin name</param>
        /// <param name="downFilter">the downstream filter</param>
        /// <param name="destPinName">the downstream filter pin name</param>
        /// <param name="useIntelligentConnect">indicate if the method should use DirectShow's Intelligent Connect</param>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder, upFilter or downFilter are null</exception>
        /// <exception cref="System.ArgumentException">Thrown if pin names are not found in filters</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if pins can't connect</exception>
        /// <remarks>
        /// If useIntelligentConnect is true, this method can add missing filters between the two pins.<br/>
        /// If useIntelligentConnect is false, this method works only if the two media types are compatible.
        /// </remarks>

        public static void ConnectFilters(IGraphBuilder graphBuilder, IBaseFilter upFilter, string sourcePinName, IBaseFilter downFilter, string destPinName, bool useIntelligentConnect)
        {
            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            if (upFilter == null)
                throw new ArgumentNullException("upFilter");

            if (downFilter == null)
                throw new ArgumentNullException("downFilter");

            IPin sourcePin, destPin;

            sourcePin = DsFindPin.ByName(upFilter, sourcePinName);
            if (sourcePin == null)
                throw new ArgumentException("The source filter has no pin called : " + sourcePinName, sourcePinName);

            destPin = DsFindPin.ByName(downFilter, destPinName);
            if (destPin == null)
                throw new ArgumentException("The downstream filter has no pin called : " + destPinName, destPinName);

            try
            {
                ConnectFilters(graphBuilder, sourcePin, destPin, useIntelligentConnect);
            }
            finally
            {
                Marshal.ReleaseComObject(sourcePin);
                Marshal.ReleaseComObject(destPin);
            }
        }

        /// <summary>
        /// Connect pins from two filters
        /// </summary>
        /// <param name="graphBuilder">the IGraphBuilder interface of the graph</param>
        /// <param name="sourcePin">the source (upstream / output) pin</param>
        /// <param name="destPin">the destination (downstream / input) pin</param>
        /// <param name="useIntelligentConnect">indicates if the method should use DirectShow's Intelligent Connect</param>
        /// <exception cref="System.ArgumentNullException">Thrown if graphBuilder, sourcePin or destPin are null</exception>
        /// <exception cref="System.Runtime.InteropServices.COMException">Thrown if pins can't connect</exception>
        /// <remarks>
        /// If useIntelligentConnect is true, this method can add missing filters between the two pins.<br/>
        /// If useIntelligentConnect is false, this method works only if the two media types are compatible.
        /// </remarks>

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        public static void ConnectFilters(IGraphBuilder graphBuilder, IPin sourcePin, IPin destPin, bool useIntelligentConnect)
        {
            int hr = 0;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            if (sourcePin == null)
                throw new ArgumentNullException("sourcePin");

            if (destPin == null)
                throw new ArgumentNullException("destPin");

            if (useIntelligentConnect)
            {
                hr = graphBuilder.Connect(sourcePin, destPin);
                DsError.ThrowExceptionForHR(hr);
            }
            else
            {
                hr = graphBuilder.ConnectDirect(sourcePin, destPin, null);
                DsError.ThrowExceptionForHR(hr);
            }
        }

        public static void DumpMediaType(AMMediaType mediaType)
        {
            Console.Write("        ");

            // majorType

            if (mediaType.majorType == MediaType.Video)
            {
                Console.Write("Video");
            }
            else if (mediaType.majorType == MediaType.Audio)
            {
                Console.Write("Audio");
            }
            else if (mediaType.majorType == MediaType.AnalogAudio)
            {
                Console.Write("AnalogAudio");
            }
            else if (mediaType.majorType == MediaType.AnalogVideo)
            {
                Console.Write("AnalogVideo");
            }
            else if (mediaType.majorType == MediaType.AuxLine21Data)
            {
                Console.Write("AuxLine21Data");
            }
            else if (mediaType.majorType == MediaType.File)
            {
                Console.Write("File");
            }
            else if (mediaType.majorType == MediaType.Interleaved)
            {
                Console.Write("Interleaved");
            }
            else if (mediaType.majorType == MediaType.LMRT)
            {
                Console.Write("LMRT");
            }
            else if (mediaType.majorType == MediaType.Midi)
            {
                Console.Write("Midi");
            }
            else if (mediaType.majorType == MediaType.Mpeg2Sections)
            {
                Console.Write("Mpeg2Sections");
            }
            else if (mediaType.majorType == MediaType.ScriptCommand)
            {
                Console.Write("ScriptCommand");
            }
            else if (mediaType.majorType == MediaType.Stream)
            {
                Console.Write("Stream");
            }
            else if (mediaType.majorType == MediaType.Texts)
            {
                Console.Write("Text");
            }
            else if (mediaType.majorType == MediaType.Timecode)
            {
                Console.Write("Timecode");
            }
            else if (mediaType.majorType == MediaType.URLStream)
            {
                Console.Write("URLStream");
            }
            else if (mediaType.majorType == MediaType.VBI)
            {
                Console.Write("VBI");
            }
            else
            {
                Console.Write(mediaType.majorType.ToString());
            }

            // subtype

            Console.Write(".");

            if (mediaType.subType == MediaSubType.Asf)
            {
                Console.Write("Asf");
            }
            else if (mediaType.subType == MediaSubType.Avi)
            {
                Console.Write("Avi");
            }
            else if (mediaType.subType == MediaSubType.I420)
            {
                Console.Write("I420");
            }
            else if (mediaType.subType == MediaSubType.IYUV)
            {
                Console.Write("IYUV");
            }
            else if (mediaType.subType == MediaSubType.Line21_BytePair)
            {
                Console.Write("Line21_BytePair");
            }
            else if (mediaType.subType == MediaSubType.Line21_GOPPacket)
            {
                Console.Write("Line21_GOPPacket");
            }
            else if (mediaType.subType == MediaSubType.Line21_VBIRawData)
            {
                Console.Write("Line21_VBIRawData");
            }
            else if (mediaType.subType == MediaSubType.MPEG1Audio)
            {
                Console.Write("MPEG1Audio");
            }
            else if (mediaType.subType == MediaSubType.MPEG1AudioPayload)
            {
                Console.Write("MPEG1AudioPayload");
            }
            else if (mediaType.subType == MediaSubType.MPEG1Packet)
            {
                Console.Write("MPEG1Packet");
            }
            else if (mediaType.subType == MediaSubType.MPEG1Payload)
            {
                Console.Write("MPEG1Payload");
            }
            else if (mediaType.subType == MediaSubType.MPEG1System)
            {
                Console.Write("MPEG1System");
            }
            else if (mediaType.subType == MediaSubType.MPEG1SystemStream)
            {
                Console.Write("MPEG1SystemStream");
            }
            else if (mediaType.subType == MediaSubType.MPEG1Video)
            {
                Console.Write("MPEG1Video");
            }
            else if (mediaType.subType == MediaSubType.MPEG1VideoCD)
            {
                Console.Write("MPEG1VideoCD");
            }
            else if (mediaType.subType == MediaSubType.Mpeg2Audio)
            {
                Console.Write("Mpeg2Audio");
            }
            else if (mediaType.subType == MediaSubType.Mpeg2Data)
            {
                Console.Write("Mpeg2Data");
            }
            else if (mediaType.subType == MediaSubType.Mpeg2Program)
            {
                Console.Write("Mpeg2Program");
            }
            else if (mediaType.subType == MediaSubType.Mpeg2Transport)
            {
                Console.Write("Mpeg2Transport");
            }
            else if (mediaType.subType == MediaSubType.Mpeg2TransportStride)
            {
                Console.Write("Mpeg2TransportStride");
            }
            else if (mediaType.subType == MediaSubType.Mpeg2Video)
            {
                Console.Write("Mpeg2Video");
            }
            else if (mediaType.subType == MediaSubType.None)
            {
                Console.Write("None");
            }
            else if (mediaType.subType == MediaSubType.NV12)
            {
                Console.Write("NV12");
            }
            else if (mediaType.subType == MediaSubType.Overlay)
            {
                Console.Write("Overlay");
            }
            else if (mediaType.subType == MediaSubType.PCM)
            {
                Console.Write("PCM");
            }
            else if (mediaType.subType == MediaSubType.PCMAudio_Obsolete)
            {
                Console.Write("PCMAudio_Obsolete");
            }
            else if (mediaType.subType == MediaSubType.PLUM)
            {
                Console.Write("PLUM");
            }
            else if (mediaType.subType == MediaSubType.QTJpeg)
            {
                Console.Write("QTJpeg");
            }
            else if (mediaType.subType == MediaSubType.QTMovie)
            {
                Console.Write("QTMovie");
            }
            else if (mediaType.subType == MediaSubType.QTRle)
            {
                Console.Write("QTRle");
            }
            else if (mediaType.subType == MediaSubType.QTRpza)
            {
                Console.Write("QTRpza");
            }
            else if (mediaType.subType == MediaSubType.QTSmc)
            {
                Console.Write("QTSmc");
            }
            else if (mediaType.subType == MediaSubType.RAW_SPORT)
            {
                Console.Write("RAW_SPORT");
            }
            else if (mediaType.subType == MediaSubType.RGB1)
            {
                Console.Write("RGB1");
            }
            else if (mediaType.subType == MediaSubType.RGB16_D3D_DX7_RT)
            {
                Console.Write("RGB16_D3D_DX7_RT");
            }
            else if (mediaType.subType == MediaSubType.RGB16_D3D_DX9_RT)
            {
                Console.Write("RGB16_D3D_DX9_RT");
            }
            else if (mediaType.subType == MediaSubType.RGB24)
            {
                Console.Write("RGB24");
            }
            else if (mediaType.subType == MediaSubType.RGB32)
            {
                Console.Write("RGB32");
            }
            else if (mediaType.subType == MediaSubType.RGB32_D3D_DX7_RT)
            {
                Console.Write("RGB32_D3D_DX7_RT");
            }
            else if (mediaType.subType == MediaSubType.RGB32_D3D_DX9_RT)
            {
                Console.Write("RGB32_D3D_DX9_RT");
            }
            else if (mediaType.subType == MediaSubType.RGB4)
            {
                Console.Write("RGB4");
            }
            else if (mediaType.subType == MediaSubType.RGB555)
            {
                Console.Write("RGB555");
            }
            else if (mediaType.subType == MediaSubType.RGB565)
            {
                Console.Write("RGB565");
            }
            else if (mediaType.subType == MediaSubType.RGB8)
            {
                Console.Write("RGB8");
            }
            else if (mediaType.subType == MediaSubType.S340)
            {
                Console.Write("S340");
            }
            else if (mediaType.subType == MediaSubType.S342)
            {
                Console.Write("S342");
            }
            else if (mediaType.subType == MediaSubType.SPDIF_TAG_241h)
            {
                Console.Write("SPDIF_TAG_241h");
            }
            else if (mediaType.subType == MediaSubType.TELETEXT)
            {
                Console.Write("TELETEXT");
            }
            else if (mediaType.subType == MediaSubType.TVMJ)
            {
                Console.Write("TVMJ");
            }
            else if (mediaType.subType == MediaSubType.UYVY)
            {
                Console.Write("UYVY");
            }
            else if (mediaType.subType == MediaSubType.VideoImage)
            {
                Console.Write("VideoImage");
            }
            else if (mediaType.subType == MediaSubType.VPS)
            {
                Console.Write("VPS");
            }
            else if (mediaType.subType == MediaSubType.VPVBI)
            {
                Console.Write("VPVBI");
            }
            else if (mediaType.subType == MediaSubType.VPVideo)
            {
                Console.Write("VPVideo");
            }
            else if (mediaType.subType == MediaSubType.WAKE)
            {
                Console.Write("WAKE");
            }
            else if (mediaType.subType == MediaSubType.WAVE)
            {
                Console.Write("WAVE");
            }
            else if (mediaType.subType == MediaSubType.WebStream)
            {
                Console.Write("WebStream");
            }
            else if (mediaType.subType == MediaSubType.WSS)
            {
                Console.Write("WSS");
            }
            else if (mediaType.subType == MediaSubType.Y211)
            {
                Console.Write("Y211");
            }
            else if (mediaType.subType == MediaSubType.Y411)
            {
                Console.Write("Y411");
            }
            else if (mediaType.subType == MediaSubType.Y41P)
            {
                Console.Write("Y41P");
            }
            else if (mediaType.subType == MediaSubType.YUY2)
            {
                Console.Write("YUY2");
            }
            else if (mediaType.subType == MediaSubType.YUYV)
            {
                Console.Write("YUYV");
            }
            else if (mediaType.subType == MediaSubType.YV12)
            {
                Console.Write("YV12");
            }
            else if (mediaType.subType == MediaSubType.YVU9)
            {
                Console.Write("YVU9");
            }
            else if (mediaType.subType == MediaSubType.YVYU)
            {
                Console.Write("YVYU");
            }
            else
            {
                Console.Write(mediaType.subType.ToString());
            }
            Console.WriteLine("");
        }

        public static void DumpPin(IPin pin)
        {
            int hr;

            PinInfo pinInfo;
            hr = pin.QueryPinInfo(out pinInfo);
            Console.Write("  ");
            switch (pinInfo.dir)
            {
                case PinDirection.Input:
                    Console.Write("In");
                    break;
                case PinDirection.Output:
                    Console.Write("Out");
                    break;
            }
            Console.WriteLine(" - " + pinInfo.name);
#if COMMENT
            IAMStreamConfig streamConfig = pin as IAMStreamConfig;
            if (streamConfig != null)
            {
                int nCaps;
                int iSize;
                streamConfig.GetNumberOfCapabilities(out nCaps, out iSize);
                Console.WriteLine("pin capabilities ({0}):", nCaps);
                for (int i = 0; i < nCaps; i++)
                {
                    AMMediaType mediaType = new AMMediaType();
                    hr = streamConfig.GetStreamCaps(i, out mediaType, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                    DumpMediaType(mediaType);
                }
            }
#endif
            IEnumMediaTypes enumMediaTypes;
            hr = pin.EnumMediaTypes(out enumMediaTypes);
            DsError.ThrowExceptionForHR(hr);

            AMMediaType[] mediaTypes = new AMMediaType[1];

            while (enumMediaTypes.Next(1, mediaTypes, IntPtr.Zero) == 0)
            {
                DumpMediaType(mediaTypes[0]);
            }
        }

        public static void DumpFilter(IBaseFilter filter)
        {
            int hr;

            FilterInfo filterInfo;
            hr = filter.QueryFilterInfo(out filterInfo);
            DsError.ThrowExceptionForHR(hr);
            Console.WriteLine(filterInfo.achName);
            IEnumPins enumPins;
            IPin[] pins = new IPin[1];

            hr = filter.EnumPins(out enumPins);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                while (enumPins.Next(pins.Length, pins, IntPtr.Zero) == 0)
                {
                    try
                    {
                        DumpPin(pins[0]);
                        DsError.ThrowExceptionForHR(hr);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(pins[0]);
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(enumPins);
            }
        }

        public static void DumpGraph(IGraphBuilder graphBuilder)
        {
            int hr = 0;
            IEnumFilters enumFilters;

            if (graphBuilder == null)
                throw new ArgumentNullException("graphBuilder");

            hr = graphBuilder.EnumFilters(out enumFilters);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                IBaseFilter[] filters = new IBaseFilter[1];

                while (enumFilters.Next(filters.Length, filters, IntPtr.Zero) == 0)
                {
                    try
                    {
                        DumpFilter(filters[0]);
                    }
                    catch { }
                    Marshal.ReleaseComObject(filters[0]);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(enumFilters);
            }
        }

        public static IBaseFilter AddH264Decoder(IGraphBuilder graphBuilder)
        {
            if (FilterExists(FilterCategory.LegacyAmFilterCategory, "Microsoft DTV-DVD Video Decoder"))
            {
                return AddFilterByName(graphBuilder, FilterCategory.LegacyAmFilterCategory, "Microsoft DTV-DVD Video Decoder");
            }
            else if (FilterExists(FilterCategory.LegacyAmFilterCategory, "Elecard AVC Video Decoder"))
            {
                return AddFilterByName(graphBuilder, FilterCategory.LegacyAmFilterCategory, "Elecard AVC Video Decoder");
            }
            else return AddFilterByName(graphBuilder, FilterCategory.LegacyAmFilterCategory, "LEAD H264 Decoder (3.0)");
        }
    }

    #region Unmanaged Code declarations

    [Flags]
    internal enum STGM
    {
        Read = 0x00000000,
        Write = 0x00000001,
        ReadWrite = 0x00000002,
        ShareDenyNone = 0x00000040,
        ShareDenyRead = 0x00000030,
        ShareDenyWrite = 0x00000020,
        ShareExclusive = 0x00000010,
        Priority = 0x00040000,
        Create = 0x00001000,
        Convert = 0x00020000,
        FailIfThere = 0x00000000,
        Direct = 0x00000000,
        Transacted = 0x00010000,
        NoScratch = 0x00100000,
        NoSnapShot = 0x00200000,
        Simple = 0x08000000,
        DirectSWMR = 0x00400000,
        DeleteOnRelease = 0x04000000,
    }

    [Flags]
    internal enum STGC
    {
        Default        = 0,
        Overwrite      = 1,
        OnlyIfCurrent  = 2,
        DangerouslyCommitMerelyToDiskCache = 4,
        Consolidate    = 8
    }

    [Guid("0000000b-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IStorage
    {
        [PreserveSig]
        int CreateStream(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [In] STGM grfMode,
            [In] int reserved1,
            [In] int reserved2,
#if USING_NET11
			[Out] out UCOMIStream ppstm
#else
			[Out] out IStream ppstm
#endif
            );

        [PreserveSig]
        int OpenStream(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [In] IntPtr reserved1,
            [In] STGM grfMode,
            [In] int reserved2,
#if USING_NET11
			[Out] out UCOMIStream ppstm
#else
			[Out] out IStream ppstm
#endif
			);

        [PreserveSig]
        int CreateStorage(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [In] STGM grfMode,
            [In] int reserved1,
            [In] int reserved2,
            [Out] out IStorage ppstg
            );

        [PreserveSig]
        int OpenStorage(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [In] IStorage pstgPriority,
            [In] STGM grfMode,
            [In] int snbExclude,
            [In] int reserved,
            [Out] out IStorage ppstg
            );

        [PreserveSig]
        int CopyTo(
            [In] int ciidExclude,
            [In] Guid[] rgiidExclude,
            [In] string[] snbExclude,
            [In] IStorage pstgDest
            );

        [PreserveSig]
        int MoveElementTo(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [In] IStorage pstgDest,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName,
            [In] STGM grfFlags
            );

        [PreserveSig]
        int Commit([In] STGC grfCommitFlags);

        [PreserveSig]
        int Revert();

        [PreserveSig]
        int EnumElements(
            [In] int reserved1, 
            [In] IntPtr reserved2, 
            [In] int reserved3, 
            [Out, MarshalAs(UnmanagedType.Interface)] out object ppenum
            );

        [PreserveSig]
        int DestroyElement([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

        [PreserveSig]
        int RenameElement(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsOldName, 
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName
            );

        [PreserveSig]
        int SetElementTimes(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, 
#if USING_NET11
			[In] FILETIME pctime,
			[In] FILETIME patime,
			[In] FILETIME pmtime
#else
			[In] System.Runtime.InteropServices.ComTypes.FILETIME pctime,
            [In] System.Runtime.InteropServices.ComTypes.FILETIME patime,
            [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime
#endif
			);

        [PreserveSig]
        int SetClass([In, MarshalAs(UnmanagedType.LPStruct)] Guid clsid);

        [PreserveSig]
        int SetStateBits(
            [In] int grfStateBits, 
            [In] int grfMask
            );

        [PreserveSig]
        int Stat(
#if USING_NET11
			[Out] out STATSTG pStatStg, 
#else
			[Out] out System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, 
#endif
			[In] int grfStatFlag
            );
    }

    internal sealed class NativeMethods
    {
        private NativeMethods(){}

        [DllImport("ole32.dll")]
#if USING_NET11
		public static extern int CreateBindCtx(int reserved, out UCOMIBindCtx ppbc);
#else
		public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
#endif

        [DllImport("ole32.dll")]
#if USING_NET11
		public static extern int MkParseDisplayName(UCOMIBindCtx pcb, [MarshalAs(UnmanagedType.LPWStr)] string szUserName, out int pchEaten, out UCOMIMoniker ppmk);
#else
		public static extern int MkParseDisplayName(IBindCtx pcb, [MarshalAs(UnmanagedType.LPWStr)] string szUserName, out int pchEaten, out IMoniker ppmk);
#endif

        [DllImport("olepro32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern int OleCreatePropertyFrame(
            [In] IntPtr hwndOwner, 
            [In] int x, 
            [In] int y,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszCaption, 
            [In] int cObjects,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.IUnknown)] object[] ppUnk,
            [In] int cPages,	
            [In] IntPtr pPageClsID, 
            [In] int lcid, 
            [In] int dwReserved, 
            [In] IntPtr pvReserved 
            );

        [DllImport("ole32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern int StgCreateDocfile(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [In] STGM grfMode,
            [In] int reserved,
            [Out] out IStorage ppstgOpen
            );

        [DllImport("ole32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern int StgIsStorageFile([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName);

        [DllImport("ole32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern int StgOpenStorage(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
            [In] IStorage pstgPriority,
            [In] STGM grfMode,
            [In] IntPtr snbExclude,
            [In] int reserved,
            [Out] out IStorage ppstgOpen
            );

    }
    #endregion

}
