using System.Collections.Generic;
using SharpNeat.Phenomes;

namespace SharpNeat.Evaluation
{
    /// <summary>
    /// Represents a type that evaluates a single phenome.
    /// </summary>
    /// <typeparam name="T">Phenome input/output signal data type.</typeparam>
    public interface IPhenomeEvaluator<T> where T : struct
    {
        /// <summary>
        /// Evaluate a single phenome and return its fitness score or scores.
        /// </summary>
        /// <param name="phenome">The phenome to evaluate.</param>
        /// <returns></returns>
        FitnessInfo Evaluate(IPhenome<T> phenome);

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
