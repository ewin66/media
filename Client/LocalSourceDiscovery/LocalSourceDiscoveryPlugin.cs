using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

using FutureConcepts.Media.SourceDiscoveryCommon;

namespace FutureConcepts.Media.Client.LocalSourceDiscovery
{
    public class LocalSourceDiscoveryPlugin : SourceDiscoveryPlugin
    {
        private List<SourceDiscoveryGroup> _groups;
        private SourceDiscoveryDefinition _definition;

        public LocalSourceDiscoveryPlugin(SourceDiscoveryDefinition sourceDiscoveryDefinition)
        {
            _definition = sourceDiscoveryDefinition;
        }

        public override void Start()
        {
            _groups = ConfigFileReader.ReadFile(_definition.Path1);
            if (_groups != null)
            {
                foreach (SourceDiscoveryGroup group in _groups)
                {
                    FireGroupOnline(group);
                }
            }
        }

        public override void Stop()
        {
            Debug.WriteLine("LocalSourceDiscoveryPlugin Stop begin");
            if (_groups != null)
            {
                foreach (SourceDiscoveryGroup group in _groups)
                {
                    FireGroupOffline(group);
                }
                _groups.Clear();
            }
            Debug.WriteLine("LocalSourceDiscoveryPlugin Stop complete");
        }
    }
}
