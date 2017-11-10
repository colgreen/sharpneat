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
        // TODO: Consider increasing buffer capacity, and different capacities for the two different buffers.
        const int __defaultInnovationHistoryBufferSize = 0x20000; // = 131,072.

        #region Constructors

        public NeatPopulation(
            MetaNeatGenome<T> metaNeatGenome,
            List<NeatGenome<T>> genomeList)
        : this(metaNeatGenome, genomeList, new Int32Sequence(), new Int32Sequence(), __defaultInnovationHistoryBufferSize, __defaultInnovationHistoryBufferSize)
        {}

        public NeatPopulation(
            MetaNeatGenome<T> metaNeatGenome,
            List<NeatGenome<T>> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq)
        : this(metaNeatGenome, genomeList, genomeIdSeq, innovationIdSeq, __defaultInnovationHistoryBufferSize, __defaultInnovationHistoryBufferSize)
        {}

        public NeatPopulation(
            MetaNeatGenome<T> metaNeatGenome,
            List<NeatGenome<T>> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            int addedConnectionHistoryBufferSize,
            int addedNodeHistoryBufferSize)
        : base(genomeList, genomeIdSeq, innovationIdSeq)
        {
            this.MetaNeatGenome = metaNeatGenome;
            this.AddedConnectionBuffer = new AddedConnectionBuffer(addedConnectionHistoryBufferSize, metaNeatGenome.InputNodeCount, metaNeatGenome.OutputNodeCount);
            this.AddedNodeBuffer = new AddedNodeBuffer(addedNodeHistoryBufferSize);
        }

        #endregion

        #region Properties

        public MetaNeatGenome<T> MetaNeatGenome { get; }

        /// <summary>
        /// A history buffer of added connections. 
        /// Used when adding new connections to check if an identical connection has been added to a genome elsewhere 
        /// in the population. This allows re-use of the same innovation ID for like connections.
        /// </summary>
        public AddedConnectionBuffer AddedConnectionBuffer { get; }

        /// <summary>
        /// A history buffer of added neurons.
        /// Used when adding new nodes to check if an identical nodes has been added to a genome elsewhere in the 
        /// population. This allows re-use of the same innovation ID for like nodes.
        /// </summary>
        public AddedNodeBuffer AddedNodeBuffer { get; }

        #endregion
    }
}
