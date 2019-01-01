using System.Collections.Generic;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// Represents a type that evaluates a single phenome.
    /// </summary>
    /// <typeparam name="TPhenome">Phenome input/output signal data type.</typeparam>
    public interface IPhenomeEvaluator<TPhenome>
    {
        /// <summary>
        /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
        /// </summary>
        /// <remarks>
        /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation 
        /// for the same genome, such as scheme is non-deterministic.
        /// </remarks>
        bool IsDeterministic { get; }

        /// <summary>
        /// Gets a null fitness score, i.e. for genomes that cannot be assigned a fitness score for whatever reason, e.g.
        /// if a genome failed to decode to a viable phenome that could be tested.
        /// </summary>
        FitnessInfo NullFitness { get; }

        /// <summary>
        /// Gets a fitness comparer. 
        /// </summary>
        /// <remarks>
        /// Typically there is a single fitness score whereby a higher score is better, however if there are multiple fitness scores
        /// per genome then we need a more general purpose comparer to determine an ordering on FitnessInfo(s), i.e. to be able to 
        /// determine which is the better FitnessInfo between any two.
        /// </remarks>
        IComparer<FitnessInfo> FitnessComparer { get; }

        /// <summary>
        /// Evaluate a single phenome and return its fitness score or scores.
        /// </summary>
        /// <param name="phenome">The phenome to evaluate.</param>
        /// <returns></returns>
        FitnessInfo Evaluate(TPhenome phenome);

        /// <summary>
        /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
        /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
        /// </summary>
        /// <param name="fitnessInfo">The fitness info object to test.</param>
        /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
        bool TestForStopCondition(FitnessInfo fitnessInfo);
    }
}
