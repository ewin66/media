using System;
using System.Collections.Generic;
using GMFBridgeLib;

namespace FutureConcepts.Media.DirectShowLib.Framework
{
    /// <summary>
    /// Allows 1 source sub graph, "N" intermediate sub graphs, and 1 sink sub graph to be connected in series.
    /// kdixon 02/23/2009
    /// </summary>
    public class ModularGraph : IGraphControl, IDisposable
    {
        /// <summary>
        /// Destroys all underlying graphs
        /// </summary>
        public virtual void Dispose()
        {
            IGraphControl graph = null;
            IDisposable disposeMe = null;

            for (int i = -1; i <= Intermediate.Count; i++)
            {
                graph = GraphAt(i);
                graph.Stop();

                disposeMe = graph as IDisposable;
                if (disposeMe != null)
                {
                    disposeMe.Dispose();
                }
            }
        }

        #region Sub-Graphs

        private ISourceSubGraph _source = null;
        /// <summary>
        /// Gets or sets the Source of this graph
        /// </summary>
        public ISourceSubGraph Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (built)
                {
                    throw new InvalidOperationException(@"The sub-graphs have already been connected!");
                }
                if (!(value is IGraphControl))
                {
                    throw new InvalidCastException("Source sub-graphs must implement IGraphControl!");
                }

                _source = value;
            }
        }

        private List<IIntermediateSubGraph> _intermediate = new List<IIntermediateSubGraph>();
        /// <summary>
        /// Add, arrange, or remove any intermediate sub-graphs
        /// </summary>
        public List<IIntermediateSubGraph> Intermediate
        {
            get
            {
                return _intermediate;
            }
        }

        private ISinkSubGraph _sink = null;
        /// <summary>
        /// Gets or sets the Sink of this graph
        /// </summary>
        public ISinkSubGraph Sink
        {
            get
            {
                return _sink;
            }
            set
            {
                if (built)
                {
                    throw new InvalidOperationException("The sub-graphs have already been connected!");
                }
                if (!(value is IGraphControl))
                {
                    throw new InvalidCastException("Sink sub-graphs must implement IGraphControl!");
                }

                _sink = value;
            }
        }

        #endregion

        #region Helpers

        private bool built = false;

        /// <summary>
        /// Does all preperation and interconnection
        /// </summary>
        private void Build()
        {
            //negotiate all connections and complete building sub-graphs
            for(int i = -1; i < Intermediate.Count; i++)
            {
                ConnectAdjacentGraphsAt(i);
            }

            built = true;
        }

        /// <summary>
        /// Connects two adjacent sub-graphs
        /// </summary>
        /// <param name="i">index of source. Set to -1 for this.Source</param>
        private void ConnectAdjacentGraphsAt(int i)
        {
            ISourceSubGraph source;
            ISinkSubGraph sink;

            if (i > -1)
            {
                source = (ISourceSubGraph)Intermediate[i];
            }
            else
            {
                source = this.Source;
            }

            if ((i + 1) == Intermediate.Count)
            {
                sink = this.Sink;
            }
            else
            {
                sink = Intermediate[i + 1];
            }

            sink.SetSource(source);
        }

        /// <summary>
        /// Returns the Source at a given index
        /// </summary>
        /// <param name="i">-1 through Intermediate.Count - 1</param>
        /// <returns>the requested Source</returns>
        private ISourceSubGraph SourceAt(int i)
        {
            if (i == -1)
            {
                return this.Source;
            }
            else
            {
                return (ISourceSubGraph)Intermediate[i];
            }
        }

        /// <summary>
        /// Returns the Sink at a given index
        /// </summary>
        /// <param name="i">0 through Intermediate.Count</param>
        /// <returns>the requested sink</returns>
        private ISinkSubGraph SinkAt(int i)
        {
            if (i == Intermediate.Count)
            {
                return this.Sink;
            }
            else
            {
                return (ISinkSubGraph)Intermediate[i];
            }
        }

        /// <summary>
        /// Gets the bridge controller at a specified index
        /// </summary>
        /// <param name="i">index of source to bridge from. -1 for this.Source</param>
        /// <returns>the bridge controller at the specified index</returns>
        private IGMFBridgeController BridgeAt(int i)
        {
            return SourceAt(i).Controller;
        }

        /// <summary>
        /// Bridges the data flow between two adjacent graphs
        /// </summary>
        /// <param name="i">index of the source graph</param>
        protected void BridgeGraphsAt(int i)
        {
            BridgeAt(i).BridgeGraphs(SourceAt(i).Output, SinkAt(i + 1).Input);
        }

        /// <summary>
        /// Stops the data flow between two adjacent graphs
        /// </summary>
        /// <param name="i"></param>
        protected void UnbridgeGraphsAt(int i)
        {
            BridgeAt(i).BridgeGraphs(SourceAt(i).Output, null);
        }

        /// <summary>
        /// Fetches the sub-graph at a given index
        /// </summary>
        /// <param name="i">index of the graph, -1 for this.Source, Intermediate.Count for this.Sink</param>
        /// <returns>the graph</returns>
        protected IGraphControl GraphAt(int i)
        {
            if (i == Intermediate.Count)
            {
                return (IGraphControl)Sink;
            }
            else if (i == -1)
            {
                return (IGraphControl)Source;
            }
            else
            {
                return (IGraphControl)Intermediate[i];
            }
        }

        #endregion

        #region IGraphControl Members

        /// <summary>
        /// Runs the modular graph
        /// </summary>
        /// <remarks>Makes sure all sub graphs are connected, and then makes them all run together</remarks>
        public virtual void Run()
        {
            INetworkSource networkSource = this.Source as INetworkSource;
            if (networkSource != null)
            {
                if (networkSource.Status != NetworkSourceStatus.Connected)
                {
                    throw new InvalidOperationException("The graph cannot be run unless the network source is in the Connected state!");
                }
            }

            if (!built)
            {
                Build();
            }

            for (int i = Intermediate.Count; i >= -1; i--)
            {
                if (i < Intermediate.Count)
                {
                    BridgeGraphsAt(i);
                }
                GraphAt(i).Run();
            }

            this.State = State.Running;
        }

        /// <summary>
        /// Stops the graph
        /// </summary>
        public virtual void Stop()
        {
            for (int i = -1; i <= Intermediate.Count; i++)
            {
                if (i < Intermediate.Count)
                {
                    UnbridgeGraphsAt(i);
                }
                GraphAt(i).Stop();
            }
            this.State = State.Stopped;
        }

        /// <summary>
        /// Simply calls Stop
        /// </summary>
        public virtual void StopWhenReady()
        {
            Stop();
        }

        /// <summary>
        /// Pauses the graph
        /// </summary>
        public virtual void Pause()
        {
            for (int i = -1; i <= Intermediate.Count; i++)
            {
                GraphAt(i).Pause();
            }
            this.State = State.Paused;
        }

        /// <summary>
        /// Simply calls Stop
        /// </summary>
        public virtual void Abort()
        {
            Stop();
        }

        /// <summary>
        /// Retreives the State of the graph
        /// </summary>
        public State State
        {
            get;
            set;
        }

        #endregion
    }
}
