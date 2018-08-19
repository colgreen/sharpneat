using System;
using System.Collections.Generic;
using Redzen.Structures;

namespace SharpNeat.EvolutionAlgorithm
{
    /// <summary>
    /// A population of genomes.
    /// </summary>
    /// <typeparam name="TGenome">Genome type.</typeparam>
    public class Population<TGenome>    
    {
        #region Auto Properties

        /// <summary>
        /// The list of genomes that make up the population.
        /// </summary>
        public List<TGenome> GenomeList { get; }

        /// <summary>
        /// The number of genomes in the population.
        /// </summary>
        /// <remarks>
        /// This value is set and fixed to be the length of <see cref="GenomeList"/> at construction time.
        /// During certain phases of the evolution algorithm the length of <see cref="GenomeList"/> will vary and therefore it may not match
        /// <see cref="PopulationSize"/> at any given point in time, thus this property is the definitive source of the population size.
        /// </remarks>
        public int PopulationSize { get; }

        /// <summary>
        /// A sequence that provides the current generation number.
        /// </summary>
        public Int32Sequence GenerationSeq { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct the population with the provided list of genomes that make up the initial population.
        /// </summary>
        /// <param name="genomeList"></param>
        public Population(List<TGenome> genomeList)
        {
            this.GenomeList = genomeList ?? throw new ArgumentNullException(nameof(genomeList));
            this.PopulationSize = genomeList.Count;
            this.GenerationSeq = new Int32Sequence();

            if(genomeList.Count == 0) {
                throw new ArgumentException("Empty genome list. The initial population cannot be empty.", nameof(genomeList));
            }
        }

        #endregion
    }
}
