
namespace SharpNeat.EA
{
    /// <summary>
    /// A high level strategy that incorporates both the selection and reproduction stages of 
    /// an evolutionary algorithm.
    /// </summary>
    /// <remarks>
    /// Generally an ISelectionReproductionStrategy will operate as follows:
    /// 
    /// 1) Selects genomes from the population.
    /// 2) Creates offspring from those selected genomes and adds the offspring into the population.
    /// 3) Removes enough low fitness genomes from the population to make way for the new offspring,
    /// i.e. to keep the population size constant.
    /// 
    /// Encapsulating all of these stages of the evolutionary algorithm within a single strategy interface
    /// allows for plugging-in of high level evolutionary algorithm strategies.
    /// </remarks>
    /// <typeparam name="TGenome"></typeparam>
    public interface ISelectionReproductionStrategy<TGenome> //where TGenome : IGenome
    {
        /// <summary>
        /// Invoke the strategy.
        /// Accepts a population of genomes, and updates it into a new population.
        /// </summary>
        /// <param name="population">The population to operate upon.</param>
        void Invoke(Population<TGenome> population);
    }
}
