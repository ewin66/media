using System;
using System.Collections.Generic;
using System.Text;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Represnets a sub graph that is intermediate (has an input and an output)
    /// </summary>
    /// <author>kdixon 02/23/2009</author>
    public interface IIntermediateSubGraph : ISourceSubGraph, ISinkSubGraph
    {
    }
}
