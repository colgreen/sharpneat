// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using Redzen;

namespace SharpNeat.Evaluation;

/// <summary>
/// Represents fitness information for a genome.
/// </summary>
public struct FitnessInfo
{
    /// <summary>
    /// Default singleton instance.
    /// </summary>
    public static readonly FitnessInfo DefaultFitnessInfo = new(0.0);

    /// <summary>
    /// Construct with a single fitness score.
    /// </summary>
    /// <param name="fitness">Genome fitness score.</param>
    public FitnessInfo(double fitness)
    {
        if(!DoubleUtils.IsNonNegativeReal(fitness))
            throw new ArgumentOutOfRangeException(nameof(fitness), "Fitness must be non-negative and a real number.");

        PrimaryFitness = fitness;
        AuxFitnessScores = null;
    }

    /// <summary>
    /// Construct with a compound fitness score.
    /// </summary>
    /// <param name="primaryFitness">Primary fitness.</param>
    /// <param name="auxFitnessScores">Auxiliary fitness scores.</param>
    public FitnessInfo(double primaryFitness, double[] auxFitnessScores)
    {
        Debug.Assert(auxFitnessScores.Length > 0);

        if(!DoubleUtils.IsNonNegativeReal(primaryFitness))
            throw new ArgumentOutOfRangeException(nameof(primaryFitness), "Fitness must be non-negative and a real number.");

        PrimaryFitness = primaryFitness;
        AuxFitnessScores = auxFitnessScores;
    }

    /// <summary>
    /// Gets the primary fitness score; for most evaluation schemes this is the one and only fitness score.
    /// </summary>
    public double PrimaryFitness { get; }

    /// <summary>
    /// Gets an array of auxiliary fitness scores.
    /// </summary>
    /// <remarks>
    /// Most problem tasks will yield just a single fitness value via the <see cref="PrimaryFitness"/> property,
    /// and therefore will not use this property.
    /// This is for problem tasks that produce multiple fitness values per evaluation; in those scenarios there
    /// is still a single primary fitness provided by <see cref="PrimaryFitness"/>, but there are also one or more
    /// secondary fitness scores that are for reporting purposes only, i.e., they aren't currently used by the
    /// evolution algorithm.
    /// </remarks>
    public double[]? AuxFitnessScores { get; }
}
