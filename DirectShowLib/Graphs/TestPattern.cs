using System;
using FutureConcepts.Media.DirectShowLib.Framework;
using GMFBridgeLib;
using LMH264EncoderLib;

namespace FutureConcepts.Media.DirectShowLib.Graphs
{
    /// <summary>
    /// A source that is a bouncing ball
    /// </summary>
    /// <author>kdixon 01/06/2010</author>
    public class TestPattern : BaseDSGraph, ISourceSubGraph
    {
        private IBaseFilter bouncingBall;
        
        private IBaseFilter videoEncoder;
        private LMH264Encoder h264Encoder;

        /// <summary>
        /// Creates a test pattern, and creates its own GMFBridgeController
        /// </summary>
        /// <param name="compressVideo">true if the video should be compressed, or false for raw frames</param>
        public TestPattern(bool compressVideo)
        {
            this.Controller = (IGMFBridgeController)new GMFBridgeControllerClass();
            this.Controller.AddStream(1, eFormatType.eAny, 1);
            Construct(compressVideo);
        }

        /// <summary>
        /// Creates a test pattern
        /// </summary>
        /// <param name="controller">GMFBridgeController to use</param>
        /// <param name="compressVideo">true if the video should be compressed, or false for raw frames</param>
        public TestPattern(IGMFBridgeController controller, bool compressVideo)
        {
            this.Controller = controller;
            Construct(compressVideo);
        }

        private void Construct(bool compressVideo)
        {
            this.Output = (IBaseFilter)this.Controller.InsertSinkFilter(this.graph);

            bouncingBall = FilterGraphTools.AddFilterByDevicePath(this.graph, @"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{8D234572-35E6-495D-98AE-9A36856C49C8}", "Bouncing Ball");
            if (compressVideo)
            {
                videoEncoder = FilterGraphTools.AddFilterByName(this.graph, FilterCategory.VideoCompressorCategory, "LEAD H264 Encoder (4.0)");
                h264Encoder = (LMH264Encoder)videoEncoder;
                h264Encoder.EnableRateControl = true;
                h264Encoder.EnableSuperCompression = false;
                h264Encoder.BitRate = 10000;
                h264Encoder.FrameRate = -1;
                h264Encoder.EncodingSpeed = eH264ENCODINGSPEED.H264SPEED_1;
                h264Encoder.EncodingThreads = eH264ENCODINGTHREADS.H264THREAD_AUTO;
                FilterGraphTools.ConnectFilters(this.graph, bouncingBall, "A Bouncing Ball!", videoEncoder, "XForm In", false);
                FilterGraphTools.ConnectFilters(this.graph, videoEncoder, "XForm Out", this.Output, "Input 1", false);
            }
            else
            {
                FilterGraphTools.ConnectFilters(this.graph, bouncingBall, "A Bouncing Ball!", this.Output, "Input 1", false);
            }
        }

        #region ISourceSubGraph Members

        /// <summary>
        /// The GMFBridgeController that is this sink
        /// </summary>
        public IGMFBridgeController Controller
        {
            get;
            private set;
        }
        
        /// <summary>
        /// The GMF Sink that is the output of this graph
        /// </summary>
        public IBaseFilter Output
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Disposes the graph
        /// </summary>
        public override void Dispose()
        {
            this.Stop();
            Release(this.Output);
            Release(videoEncoder);
            Release(bouncingBall);
            base.Dispose();
        }
    }
}
