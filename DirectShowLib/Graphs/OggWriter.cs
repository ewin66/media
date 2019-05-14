using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.DirectShowLib.Framework;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.DirectShowLib.Graphs
{
    /// <summary>
    /// Writes any data given to it to C:\temp\oggwriter.ogm in ogg format
    /// </summary>
    /// <author>kdixon 08/2009</author>
    public class OggWriter : BaseDSGraph, ISinkSubGraph
    {
        /// <summary>
        /// Constructs a new OggWriter
        /// </summary>
        public OggWriter() : base()
        {
            oggMux = FilterGraphTools.AddFilterByName(this.graph, FilterCategory.LegacyAmFilterCategory, "LEAD Ogg Multiplexer");
            if (oggMux == null)
            {
                throw new Exception("Could not instantiate OGG Mux!");
            }

            fileWriter = FilterGraphTools.AddFilterByName(this.graph, FilterCategory.LegacyAmFilterCategory, "File writer");
            if (fileWriter == null)
            {
                throw new Exception("File writer could not be instantiated");
            }

            IFileSinkFilter fs = fileWriter as IFileSinkFilter;
            if (fs == null)
            {
                throw new Exception(@"Can't get IFileSinkFilter interface!");
            }

            string datetime = DateTime.Now.ToString("yyMMdd HHmmss");

            int hr = fs.SetFileName(@"C:\temp\" + datetime + " oggwriter.ogm", null);
            DsError.ThrowExceptionForHR(hr);
        }

        private IBaseFilter input = null;
        private IBaseFilter oggMux = null;
        private IBaseFilter fileWriter = null;

        #region ISinkSubGraph Members

        /// <summary>
        /// Sets the source for this graph
        /// </summary>
        /// <param name="source">source sub graph</param>
        public void SetSource(ISourceSubGraph source)
        {
            Debug.WriteLine("OggWriter.SetSource: " + source.GetType().ToString());

            List<DetailPinInfo> pins = null;

            try
            {
                input = (IBaseFilter)source.Controller.InsertSourceFilter(source.Output, this.graph);

                pins = input.EnumPinsDetails();

                int oggStreamCounter = 0;

                foreach (DetailPinInfo i in pins)
                {
                    if (i.Info.dir == PinDirection.Output)
                    {
                        IPin destPin = DsFindPin.ByName(oggMux, "Stream " + oggStreamCounter.ToString());
                        if (destPin != null)
                        {
                            FilterGraphTools.ConnectFilters(this.graph, i.Pin, destPin, false);
                            Release(destPin);
                            oggStreamCounter++;
                        }
                    }
                }

                FilterGraphTools.ConnectFilters(this.graph, oggMux, "Ogg Stream", fileWriter, "in", false);
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
        /// Gets the GMF Sink input
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
