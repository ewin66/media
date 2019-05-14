using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Extension methods for the Framework
    /// </summary>
    /// <author>kdixon 01/06/2010</author>
    public static class ExtensionsForFramework
    {
        /// <summary>
        /// Connects the GMFBridge between source and destination
        /// </summary>
        /// <param name="source">this / source sub graph</param>
        /// <param name="dest">destination sub graph</param>
        public static void BridgeTo(this ISourceSubGraph source, ISinkSubGraph dest)
        {
            source.Controller.BridgeGraphs(source.Output, (dest != null) ? dest.Input : null);
        }
    }
}
