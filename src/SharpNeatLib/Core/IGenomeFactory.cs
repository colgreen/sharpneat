using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Core
{
    public interface IGenomeFactory<TGenome>
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

        /// <summary>
        /// Creates a list of genomes spawned from a seed genome. Spawning uses asexual reproduction.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="seedGenome">The seed genome.</param>
        List<TGenome> CreateGenomeList(int length, TGenome seedGenome);

        /// <summary>
        /// Creates a list of genomes spawned from a list of seed genomes. Spawning uses asexual reproduction and
        /// typically we repeatedly loop over (and spawn from) the seed genomes until we have the required number
        /// of spawned genomes.
        /// </summary>
        /// <param name="length">The number of genomes to create.</param>
        /// <param name="seedGenomeList">A list of seed genomes.</param>
        List<TGenome> CreateGenomeList(int length, List<TGenome> seedGenomeList);
    }
}
