using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FutureConcepts.Media;
using FutureConcepts.Media.DirectShowLib;

using LMDVRSourceLib;

namespace FutureConcepts.Media.Server.Graphs
{
    public class DVRGraphHelper
    {
        /// <summary>
        /// Add the LEAD DVR Source filter to a graph (builder) and
        /// load the fileSource using the associated input graph's
        /// source name (from sourceConfig).  For example, if the output
        /// graph source name was "Vid1-RTSP" then the input graph
        /// source name will have a name of "Vid1".
        /// <DVRSourceName> --> <LeftOfSinkSourceName>
        /// Vid1-RTSP (the output graph source name) --> Vid1 (the input graph source name) 
        /// </summary>
        public static IBaseFilter GetAndConfigureDVRSourceForSink(IGraphBuilder builder, StreamSourceInfo sourceConfig)
        {
            IBaseFilter result = FilterGraphTools.AddFilterByName(builder, FilterCategory.LegacyAmFilterCategory, "LEAD DVR Source");
            if (result == null)
            {
                throw new Exception("The LEADTOOLS DVR Source filter is not registered");
            }
            IFileSourceFilter fileSource = (IFileSourceFilter)result;
            String[] sourceNameParts = sourceConfig.SourceName.Split('-');
            DVRSettings dvrSettings = DVRSettings.LoadFromFile();
            int hr = fileSource.Load(dvrSettings.RootFolder + sourceNameParts[0] + @"/Stream.LBL", null);
            DsError.ThrowExceptionForHR(hr);
            ILMDVRSource sourceControl = (ILMDVRSource)result;
            sourceControl.ResetToDefaultsEx(LMDVRSource_APILEVEL.LMDVRSource_APILEVEL_1);
            return result;
        }
    }
}
