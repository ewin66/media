using System;
using System.ComponentModel;
using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Callback used for clients of the IRecord service
    /// </summary>
    public interface IRecordCallback
    {
        /// <summary>
        /// Called when a recording session has began
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Begin();

        /// <summary>
        /// Called when recording has started
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Start();

        /// <summary>
        /// Called when a chunk has completed
        /// </summary>
        /// <param name="time">time when the chunk completed</param>
        /// <param name="chunkNumber">chunk's number</param>
        /// <param name="chunkLength">chunk's length</param>
        [OperationContract(IsOneWay = true)]
        void ChunkComplete(DateTime time, int chunkNumber, int chunkLength);

        /// <summary>
        /// Called when recording has stopped
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Stop();

        /// <summary>
        /// Called when a recording session has ended
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void End();
    }

    /// <summary>
    /// ServiceContract for Recording WCF service
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IRecordCallback))]
    public interface IRecord : IStream
    {
        /// <summary>
        /// Begin a recording session
        /// </summary>
        [OperationContract]
        void BeginSession();

        /// <summary>
        /// Start recording
        /// </summary>
        [OperationContract()]
        void StartRecording();

        /// <summary>
        /// Stop recording
        /// </summary>
        [OperationContract()]
        void StopRecording();

        /// <summary>
        /// End a recording session
        /// </summary>
        [OperationContract()]
        void EndSession();
    }
}
