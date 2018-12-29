using System.Collections.Generic;

namespace SharpNeat.Evaluation
{
    public interface IGenomeListEvaluator<TGenome>
    {
        /// <summary>
        /// Evaluates a collection of genomes and assigns fitness info to each.
        /// </summary>
        void Evaluate(ICollection<TGenome> genomeList);

        /// <summary>
        /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
        /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
        /// </summary>
        /// <param name="fitnessInfo">The fitness info object to test.</param>
        /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
        bool TestForStopCondition(FitnessInfo fitnessInfo);

        /// <summary>
        /// Gets a fitness comparer. 
        /// </summary>
        /// <remarks>
        /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
        /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo, i.e. to be able to 
        /// determine which is the better FitenssInfo between any two.
        /// </remarks>
        IComparer<FitnessInfo> FitnessComparer { get; }
    }
}
