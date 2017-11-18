using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.EA;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;

namespace SharpNeat.Neat
{
    public class NeatPopulation<T> : Population<NeatGenome<T>>
        where T : struct
    {
        #region Consts / Statics

        // TODO: Consider increasing buffer capacity, and different capacities for the two different buffers.
        const int __defaultInnovationHistoryBufferSize = 0x20000; // = 131,072.

        #endregion

        #region Auto Properties

        public MetaNeatGenome<T> MetaNeatGenome { get; }

        public Int32Sequence GenomeIdSeq { get; }

        public Int32Sequence InnovationIdSeq { get; }

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

        #region Constructors

        public NeatPopulation(
            MetaNeatGenome<T> metaNeatGenome,
            List<NeatGenome<T>> genomeList) 
            : base(genomeList)
        {
            GetMaxObservedIds(genomeList, out int maxGenomeId, out int maxInnovationId);

            this.MetaNeatGenome = metaNeatGenome;
            this.GenomeIdSeq = new Int32Sequence(maxGenomeId + 1);
            this.InnovationIdSeq = new Int32Sequence(maxInnovationId + 1);
            this.AddedConnectionBuffer = new AddedConnectionBuffer(__defaultInnovationHistoryBufferSize, metaNeatGenome.InputNodeCount, metaNeatGenome.OutputNodeCount);
            this.AddedNodeBuffer = new AddedNodeBuffer(__defaultInnovationHistoryBufferSize);            
        }

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
        : base(genomeList)
        {
            this.MetaNeatGenome = metaNeatGenome;
            this.GenomeIdSeq = genomeIdSeq;
            this.InnovationIdSeq = innovationIdSeq;
            this.AddedConnectionBuffer = new AddedConnectionBuffer(addedConnectionHistoryBufferSize, metaNeatGenome.InputNodeCount, metaNeatGenome.OutputNodeCount);
            this.AddedNodeBuffer = new AddedNodeBuffer(addedNodeHistoryBufferSize);

            // Assert that the ID sequences have a current IDs higher than any existing ID.
            Debug.Assert(ValidateIdSequences(genomeList, genomeIdSeq, innovationIdSeq));
        }

        #endregion

        #region Private Static Methods

        private static bool ValidateIdSequences(
            List<NeatGenome<T>> genomeList,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq)
        {
            GetMaxObservedIds(genomeList, out int maxGenomeId, out int maxInnovationId);

            if(maxGenomeId >= genomeIdSeq.Peek) {
                return false;
            }

            if(maxInnovationId >= innovationIdSeq.Peek) {
                return false;
            }

            return true;
        }

        private static void GetMaxObservedIds(List<NeatGenome<T>> genomeList, out int maxGenomeId, out int maxInnovationId)
        {
            maxGenomeId = 0;
            maxInnovationId = 0;
            
            foreach(var genome in genomeList)
            {
                maxGenomeId = Math.Max(maxGenomeId, genome.Id);

                for(int i=0; i<genome.ConnectionGeneArray.Length; i++)
                {
                    maxInnovationId = Math.Max(maxInnovationId, genome.ConnectionGeneArray[i].Id);
                    maxInnovationId = Math.Max(maxInnovationId, genome.ConnectionGeneArray[i].SourceId);
                    maxInnovationId = Math.Max(maxInnovationId, genome.ConnectionGeneArray[i].TargetId);
                }
            }
        }

        #endregion
    }
}
