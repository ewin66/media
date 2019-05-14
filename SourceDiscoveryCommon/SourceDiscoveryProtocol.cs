using System;

namespace FutureConcepts.Media.SourceDiscoveryCommon
{
    [Serializable()]
    public enum SourceDiscoveryProtocol
    {
        MasterSourceDiscovery,
        WowzaSourceDiscovery,
        LocalSourceDiscovery,
        NetMgrSourceDiscovery
    }
}
