using System.Collections.Generic;

namespace SharpNeat.Neat
{
    public interface ISpeciationStrategy<TGenome,TWeight>
        where TWeight : struct
    {
        /// <summary>
        /// Create an array of species and allocate a full population of genomes amongst them.
        /// </summary>
        /// <remarks>
        /// Creates the required number of species; assigns each genome in genomeList to one of the 
        /// species and returns the array of species.
        /// </remarks>
        /// <param name="genomeList">The genomes to speciate.</param>
        /// <param name="speciesCount">The number of required species.</param>
        /// <returns>A new array of species.</returns>
        Species<TWeight>[] SpeciateAll(IList<TGenome> genomeList, int speciesCount);

        /// <summary>
        /// Allocate new genomes to pre-existing species.
        /// </summary>
        /// <remarks>
        /// For each new genome in genomeList, determine which of the existing species it belongs within
        /// and add it to that species.
        /// </remarks>
        /// <param name="genomeList">A list of genomes that have not yet been assigned a species.</param>
        /// <param name="speciesArr">An array of pre-existing species</param>
        void SpeciateAdd(IList<TGenome> genomeList, Species<TWeight>[] speciesArr);
    }
}
