using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FutureConcepts.Media.Network.FCRTPLib;

namespace FutureConcepts.Media.Network.Contract
{
    public interface IRTPStreamCallback
    {
        [OperationContract(IsOneWay = true)]
        void AnnouncePeers(List<RTPEndpoint> peers);

        [OperationContract(IsOneWay = true)]
        void AnnounceProfile(Profile profile);
    }
}
