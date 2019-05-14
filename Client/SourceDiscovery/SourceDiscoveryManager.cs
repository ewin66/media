using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FutureConcepts.Media;
using FutureConcepts.Media.SourceDiscoveryCommon;
using FutureConcepts.Media.Client.LocalSourceDiscovery;
using FutureConcepts.Media.Client.MasterSourceDiscovery;
using FutureConcepts.Media.Client.NetMgrSourceDiscovery;
using FutureConcepts.Media.Client.WowzaSourceDiscovery;

namespace FutureConcepts.Media.Client.SourceDiscovery
{
    public class SourceDiscoveryManager
    {

        private static SourceDiscoveryManager _instance = null;

        private static Object _lock = new Object();

        private Dictionary<String, SourceDiscoveryPlugin> _plugins;

        private SourceDiscoveryManager()
        {
            _plugins = new Dictionary<String, SourceDiscoveryPlugin>();
        }

        public static SourceDiscoveryManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SourceDiscoveryManager();
                    }
                }
                return _instance;
            }
        }

        public SourceDiscoveryPlugin GetPlugin(FutureConcepts.Media.SourceDiscoveryDefinition sourceDiscovery)
        {
            SourceDiscoveryPlugin result = null;
            if (_plugins.ContainsKey(sourceDiscovery.Name) == false)
            {
                switch (sourceDiscovery.Type)
                {
                    case "MasterSourceDiscovery":
                        result = new MasterSourceDiscoveryPlugin(sourceDiscovery);
                        break;
                    case "WowzaSourceDiscovery":
                        result = new WowzaSourceDiscoveryPlugin(sourceDiscovery);
                        break;
                    case "LocalSourceDiscovery":
                        result = new LocalSourceDiscoveryPlugin(sourceDiscovery);
                        break;
                    case "NetMgrSourceDiscovery":
                        result = new NetMgrSourceDiscoveryPlugin(sourceDiscovery);
                        break;
                    default:
                        throw new NotImplementedException(sourceDiscovery.Type);
                }
                _plugins[sourceDiscovery.Name] = result;
            }
            else
            {
                result = _plugins[sourceDiscovery.Name];
            }
            return result;
        }
    }
}
