using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Represents a contract that implements a KeepAlive method
    /// </summary>
    /// <remarks>
    /// Used by Client.BasePeripheralControl to identify and call the KeepAlive method
    /// </remarks>
    [ServiceContract]
    public interface IKeepAlive
    {
        /// <summary>
        /// Call to keep the connection active.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void KeepAlive();
    }
}
