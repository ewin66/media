using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// This is the interface for the Camera Control Service
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICameraControlCallback))]
    public interface ICameraControl : IPresetProvider, ICameraControlCommon, ICameraControlProperties, IKeepAlive
    {
        /// <summary>
        /// Opens a connection to the server.
        /// </summary>
        /// <param name="request">
        /// a <see cref="T:ClientConnectRequest"/> properly filled out with the server's SourceName which has the camera you want to control
        /// </param>
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        void Open(ClientConnectRequest request);
    }
}
