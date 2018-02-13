using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.EA;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Speciation;
using SharpNeat.Utils;
using static SharpNeat.Neat.NeatPopulationUtils;

namespace SharpNeat.Neat
{
    public class NeatPopulation<T> : Population<NeatGenome<T>>
        where T : struct
    {
        #region Consts / Statics

        // ENHANCEMENT: Consider increasing buffer capacity, and different capacities for the two different buffers.
        const int __defaultInnovationHistoryBufferSize = 0x20000; // = 131,072.

        #endregion

        #region Auto Properties

        /// <summary>
        /// NeatGenome metadata.
        /// </summary>
        public MetaNeatGenome<T> MetaNeatGenome { get; }

        /// <summary>
        /// Genome ID sequence; for obtaining new genome IDs.
        /// </summary>
        public Int32Sequence GenomeIdSeq { get; }

        /// <summary>
        /// Innovation ID sequence; for obtaining new innovation IDs.
        /// </summary>
        public Int32Sequence InnovationIdSeq { get; }

        /// <summary>
        /// A history buffer of added nodes.
        /// Used when adding new nodes to check if an identical node has been added to a genome elsewhere in the population.
        /// This allows re-use of the same innovation ID for like nodes.
        /// </summary>
        public AddedNodeBuffer AddedNodeBuffer { get; }

        /// <summary>
        /// Species array.
        /// </summary>
        public Species<T>[] SpeciesArray { get; set; }

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
            this.AddedNodeBuffer = new AddedNodeBuffer(addedNodeHistoryBufferSize);

            // Assert that the ID sequences have a current IDs higher than any existing ID.
            Debug.Assert(ValidateIdSequences(genomeList, genomeIdSeq, innovationIdSeq));
        }

        #endregion
    }
}
