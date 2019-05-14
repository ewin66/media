using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FutureConcepts.Media;

namespace FutureConcepts.Media.SourceDiscoveryCommon
{
    public class SourceDiscoveryGroup
    {
        public String Name { get; set; }
        public ServerInfo ServerInfo { get; set; }
        public int Version { get; set; }

        private List<StreamSourceInfo> _sources;

        public List<StreamSourceInfo> Sources
        {
            get
            {
                if (_sources == null)
                {
                    _sources = new List<StreamSourceInfo>();
                }
                return _sources;
            }
        }

        public StreamSourceInfo FindSource(String name)
        {
            foreach (StreamSourceInfo source in _sources)
            {
                if (source.SourceName == name)
                {
                    return source;
                }
            }
            return null;
        }
    }
}
