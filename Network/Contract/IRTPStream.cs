using System.Collections.Generic;
using System.ServiceModel;
using FutureConcepts.Media.Network.FCRTPLib;

namespace FutureConcepts.Media.Network.Contract
{
    [ServiceContract(Namespace="FutureConcepts.Media.Network",
                     SessionMode = SessionMode.Required,
                     CallbackContract = typeof(IRTPStreamCallback))]
    public interface IRTPStream : IKeepAlive
    {
        [OperationContract(IsInitiating = true)]
        List<RTPStreamDescription> Connect(ClientConnectRequest request);

        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void Disconnect();

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void SetProfile(Profile newProfile);
    }
}
