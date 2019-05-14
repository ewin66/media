using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.TV.Scanner.Config
{
    /// <summary>
    /// Used to configure a TV Capture Source
    /// Kevin Dixon  12/08/2008
    /// </summary>
    [Serializable()]
    public class Source
    {
        private string _sourceName;
        /// <summary>
        /// The name of the source, as DirectShow discovers it
        /// </summary>
        [XmlElement]
        public string Name
        {
            get
            {
                return _sourceName;
            }
            set
            {
                _sourceName = value;
            }
        }

        public Graph this[TVSource type]
        {
            get
            {
                foreach (Graph g in Graph)
                {
                    if (g.Type == type)
                    {
                        return g;
                    }
                }
                throw new SourceConfigException("No graph for type \"" + type.ToString() + "\" exists for this capture device!");
            }
        }

        public bool HasGraphFor(TVSource type)
        {
            foreach (Graph g in Graph)
            {
                if (g.Type == type)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Graph> _graphs = new List<Graph>();
        /// <summary>
        /// All of the graphs that this source can provide
        /// </summary>
        [XmlElement]
        public List<Graph> Graph
        {
            get
            {
                return _graphs;
            }
            set
            {
                _graphs = value;
            }
        }


    }
}
