using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Server.IndigoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class RecordService : CommonStreamService, IRecord, IDisposable
    {
        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

        #region IStream

        public override SessionDescription OpenGraph(ClientConnectRequest clientConnectRequest)
        {
            SessionDescription sd = base.OpenGraph(clientConnectRequest);
            _callback = OperationContext.Current.GetCallbackChannel<IRecordCallback>();
            Debug.Assert(_callback != null);
            if (_graph.SourceConfig.MaxRecordingChunkMinutes != 0)
            {
                _chunkTimer = new System.Timers.Timer();
                _chunkTimer.Interval = _graph.SourceConfig.MaxRecordingChunkMinutes * 1000 * 60; // MaxRecordingChunk
                _chunkTimer.Elapsed += new System.Timers.ElapsedEventHandler(ChunkTimer_Elapsed);
                _chunkTimer.Enabled = false;
            }
            return sd;
        }

        public override SessionDescription SetProfile(Profile newProfile)
        {
            return base.SetProfile(newProfile);
        }

        public override void KeepAlive()
        {
            base.KeepAlive();
        }

        #endregion

        #region IRecord

        private IRecordCallback _callback;

        public void BeginSession()
        {
            RecordGraph.BeginSession();
            _callback.Begin();
        }

        public void StartRecording()
        {
            RecordGraph.StartRecording();
            if (_chunkTimer != null)
            {
                _chunkTimer.Enabled = true;
            }
            _callback.Start();
        }

        public void StopRecording()
        {
            RecordGraph.StopRecording();
            if (_chunkTimer != null)
            {
                _chunkTimer.Dispose();
                _chunkTimer = null;
            }
            _callback.Stop();
        }

        public void EndSession()
        {
            RecordGraph.EndSession();
            _callback.End();
        }

        #endregion

        #region ChunkTimer

        private System.Timers.Timer _chunkTimer;

        private void ChunkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RecordGraph.StopRecording();
            _callback.Stop();
            _callback.ChunkComplete(DateTime.Now, RecordGraph.SequenceNumber, RecordGraph.CurrentChunkSize);
            RecordGraph.StartRecording();
            _callback.Start();
        }

        #endregion
    }
}
