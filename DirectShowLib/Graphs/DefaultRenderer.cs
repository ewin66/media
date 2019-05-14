using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.DirectShowLib.Framework;
using System.Diagnostics;

namespace FutureConcepts.Media.DirectShowLib.Graphs
{
    /// <summary>
    /// Implents a Sink that renders all pins supplied to it
    /// </summary>
    /// <author>kdixon 02/23/2009</author>
    public class DefaultRenderer : BaseDSGraph, ISinkSubGraph
    {
        private IBaseFilter input;

        /// <summary>
        /// Disposes the renderer
        /// </summary>
        public override void Dispose()
        {
            Release(input);
            base.Dispose();
        }

        #region ISinkSubGraph Members

        /// <summary>
        /// Attaches this graph to a source graph
        /// </summary>
        /// <param name="source">source sub graph to get data from</param>
        public void SetSource(ISourceSubGraph source)
        {
            Debug.WriteLine("DefaultRenderer.SetSource: " + source.GetType().ToString());

            List<DetailPinInfo> pins = null;

            try
            {
                input = (IBaseFilter)source.Controller.InsertSourceFilter(source.Output, this.graph);

                pins = input.EnumPinsDetails();

                foreach (DetailPinInfo i in pins)
                {
                    int hr = this.captureGraph.RenderStream(null, null, i.Pin, null, null);
                    Debug.WriteLine("DefaultRenderer rendered stream; HRESULT = " + hr.ToString("x"));
                }
            }
            finally
            {
                if (pins != null)
                {
                    pins.Release();
                }
            }

        }

        /// <summary>
        /// Fetches the GMF Sink that is the input to this graph
        /// </summary>
        public IBaseFilter Input
        {
            get
            {
                if (input == null)
                {
                    throw new InvalidOperationException("SetSource() must be called before you can retreive the input");
                }
                return input;
            }
        }

        #endregion
    }
}
