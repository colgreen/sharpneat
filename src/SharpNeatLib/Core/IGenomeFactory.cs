using System.Collections.Generic;

namespace SharpNeat.Core
{
    public interface IGenomeFactory<TGenome> 
        where TGenome : IGenome
    {
        /// <summary>
        /// Creates a single randomly initialised genome.
        /// </summary>
        TGenome CreateGenome();

        /// <summary>
        /// Creates a list of randomly initialised genomes.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        List<TGenome> CreateGenomeList(int length);
    }
}
