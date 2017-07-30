using System.Collections.Generic;
using Redzen.Structures;
using SharpNeat.EA;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Utils;

namespace SharpNeat.Neat
{
    public class NeatPopulation<T> : Population<NeatGenome<T>>
        where T : struct
    {
        const int __defaultInnovationHistoryBufferSize = 0x20000; // = 131,072.

        #region Constructor

        public NeatPopulation(
            UInt32Sequence genomeIdSeq,
            UInt32Sequence innovationIdSeq,
            List<NeatGenome<T>> genomeList,
            MetaNeatGenome metaNeatGenome)
        : this(genomeIdSeq, innovationIdSeq, genomeList, metaNeatGenome, __defaultInnovationHistoryBufferSize)
        {}

        public NeatPopulation(
            UInt32Sequence genomeIdSeq,
            UInt32Sequence innovationIdSeq,
            List<NeatGenome<T>> genomeList,
            MetaNeatGenome metaNeatGenome,
            int innovationHistoryBufferSize)
        : base(genomeIdSeq, innovationIdSeq, genomeList)
        {
            this.MetaNeatGenome = metaNeatGenome;
            this.AddedConnectionBuffer = new KeyedCircularBuffer<DirectedConnection,uint>(innovationHistoryBufferSize);
            this.AddedNodeBuffer = new KeyedCircularBuffer<uint,AddedNodeInfo>(innovationHistoryBufferSize);
        }

        #endregion

        #region Properties

        public MetaNeatGenome MetaNeatGenome { get; }

        /// <summary>
        /// A history buffer of added connections. 
        /// Used when adding new connections to check if an identical connection has been added to a genome elsewhere 
        /// in the population. This allows re-use of the same innovation ID for like connections.
        /// </summary>
        public KeyedCircularBuffer<DirectedConnection,uint> AddedConnectionBuffer { get; }

        /// <summary>
        /// A history buffer of added neurons.
        /// Used when adding new neurons to check if an identical neuron has been added to a genome elsewhere in the 
        /// population. This allows re-use of the same innovation ID for like neurons.
        /// </summary>
        public KeyedCircularBuffer<uint,AddedNodeInfo> AddedNodeBuffer { get; }

        #endregion
    }
}
