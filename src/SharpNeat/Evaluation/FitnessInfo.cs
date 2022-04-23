// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using Redzen;

namespace SharpNeat.Evaluation;

/// <summary>
/// Conveys fitness information for a genome.
/// </summary>
public struct FitnessInfo
{
    /// <summary>
    /// Default singleton instance.
    /// </summary>
    public static readonly FitnessInfo DefaultFitnessInfo = new(0.0);

    #region Instance Fields

    readonly double _primaryFitness;

    /// <summary>
    /// An array of auxiliary fitness scores. Most problem tasks will yield just a single fitness value,
    /// here we allow for multiple fitness values per evaluation to allow for multiple objectives, or
    /// secondary fitness scores for reporting only.
    /// </summary>
    readonly double[]? _auxFitnessScores;

    #endregion

    #region Constructors

    /// <summary>
    /// Construct with a single fitness score.
    /// </summary>
    /// <param name="fitness">Genome fitness score.</param>
    public FitnessInfo(double fitness)
    {
        if(!DoubleUtils.IsNonNegativeReal(fitness))
            throw new ArgumentOutOfRangeException(nameof(fitness), "Fitness must be non-negative and a real number.");

        _primaryFitness = fitness;
        _auxFitnessScores = null;
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

        _primaryFitness = primaryFitness;
        _auxFitnessScores = auxFitnessScores;
    }

    #endregion

    #region Properties / Indexer

    /// <summary>
    /// Gets the primary fitness score; for most evaluation schemes this is the one and only fitness score.
    /// </summary>
    public double PrimaryFitness => _primaryFitness;

    /// <summary>
    /// Gets an array of auxiliary fitness scores.
    /// </summary>
    public double[]? AuxFitnessScores => _auxFitnessScores;

    #endregion
}
