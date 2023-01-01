// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Evaluation;

/// <summary>
/// Represents an evaluator of <typeparamref name="TPhenome"/> instances.
/// </summary>
/// <typeparam name="TPhenome">Phenome input/output signal data type.</typeparam>
public interface IPhenomeEvaluator<TPhenome>
{
    /// <summary>
    /// Evaluate a single phenome and return its fitness score or scores.
    /// </summary>
    /// <param name="phenome">The phenome to evaluate.</param>
    /// <returns>An instance of <see cref="FitnessInfo"/> that conveys the phenome's fitness scores/data.</returns>
    FitnessInfo Evaluate(TPhenome phenome);
}
