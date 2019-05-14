using System.ServiceModel;

namespace FutureConcepts.Media.Network.Contract
{
    /// <summary>
    /// Represents a contract that implements a KeepAlive method
    /// </summary>
    [ServiceContract]
    public interface IKeepAlive
    {
        /// <summary>
        /// Call to keep the connection active.
        /// </summary>
        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void KeepAlive();
    }
}
