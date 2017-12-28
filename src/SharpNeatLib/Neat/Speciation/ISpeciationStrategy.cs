using System.Collections.Generic;

namespace SharpNeat.Neat.Speciation
{
    public interface ISpeciationStrategy<TGenome,TWeight>
        where TWeight : struct
    {
        /// <summary>
        /// Initialise a new set of species based on the provided population of genomes and the 
        /// speciation method in use.
        /// </summary>
        /// <param name="genomeList">The genomes to speciate.</param>
        /// <param name="speciesCount">The number of required species.</param>
        /// <returns>A new array of species.</returns>
        Species<TWeight>[] SpeciateAll(IList<TGenome> genomeList, int speciesCount);

        /// <summary>
        /// Merge new genomes into an existing set of species.
        /// </summary>
        /// <param name="genomeList">A list of genomes that have not yet been assigned a species.</param>
        /// <param name="speciesArr">An array of pre-existing species</param>
        void SpeciateAdd(IList<TGenome> genomeList, Species<TWeight>[] speciesArr);
    }
}
