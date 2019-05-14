using System;
using System.Collections.Generic;
using System.Text;
using GMFBridgeLib;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Represents a sub graph that receives data
    /// </summary>
    /// <author>kdixon 02/23/2009</author>
    public interface ISinkSubGraph
    {
        /// <summary>
        /// Attach a source to this sink
        /// </summary>
        /// <param name="source"></param>
        void SetSource(ISourceSubGraph source);

        /// <summary>
        /// Retreives the GMF Sink Filter that is the input to this subgraph
        /// </summary>
        IBaseFilter Input
        {
            get;
        }
    }
}
