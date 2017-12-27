using System.Collections.Generic;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// Represents a type that evaluates a single phenome.
    /// </summary>
    /// <typeparam name="T">Phenome input/output signal data type.</typeparam>
    public interface IPhenomeEvaluator<TPhenome>
    {
        /// <summary>
        /// Evaluate a single phenome and return its fitness score or scores.
        /// </summary>
        /// <param name="phenome">The phenome to evaluate.</param>
        /// <returns></returns>
        FitnessInfo Evaluate(TPhenome phenome);

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
    }
}
