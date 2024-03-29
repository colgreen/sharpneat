﻿// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.EvolutionAlgorithm;

/// <summary>
/// NEAT evolution algorithm settings.
/// </summary>
public sealed record NeatEvolutionAlgorithmSettings
{
    /// <summary>
    /// The species count.
    /// </summary>
    public int SpeciesCount { get; set; } = 10;

    /// <summary>
    /// Elitism proportion.
    /// We sort species genomes by fitness and keep the top N%, the other genomes are
    /// removed to make way for the offspring.
    /// </summary>
    public double ElitismProportion { get; set; } = 0.2;

    /// <summary>
    /// Selection proportion.
    /// We sort species genomes by fitness and select parent genomes for producing offspring from
    /// the top N%. Selection is performed prior to elitism being applied, therefore selecting from more
    /// genomes than will be made elite is possible.
    /// </summary>
    public double SelectionProportion { get; set; } = 0.2;

    /// <summary>
    /// The proportion of offspring to be produced via asexual reproduction (mutation).
    /// </summary>
    public double OffspringAsexualProportion { get; set; } = 0.5;

    /// <summary>
    /// The proportion of offspring to be produced via recombination reproduction.
    /// </summary>
    public double OffspringRecombinationProportion { get; set; } = 0.5;

    /// <summary>
    /// The proportion of recombination reproductions that will use genomes from different species.
    /// </summary>
    public double InterspeciesMatingProportion { get; set; } = 0.01;

    /// <summary>
    /// Length of the history buffer used for calculating the moving average for best fitness, mean fitness and mean complexity.
    /// </summary>
    public int StatisticsMovingAverageHistoryLength { get; set; } = 100;

    /// <summary>
    /// Creates a new settings object based on the current settings object, but modified to be suitable for use when
    /// the evolution algorithm is in simplifying mode.
    /// </summary>
    /// <returns>A new instance of <see cref="NeatEvolutionAlgorithmSettings"/>.</returns>
    public NeatEvolutionAlgorithmSettings CreateSimplifyingSettings()
    {
        NeatEvolutionAlgorithmSettings simplifying = this with
        {
            OffspringAsexualProportion = 1.0,
            OffspringRecombinationProportion = 0.0
        };

        return simplifying;
    }

    /// <summary>
    /// Validate the settings, and throw an exception if not valid.
    /// </summary>
    /// <remarks>
    /// As a 'simple' collection of properties there is no construction time check that can be performed, therefore this method is supplied to
    /// allow consumers of a settings object to validate it before using it.
    /// </remarks>
    public void Validate()
    {
        if (SpeciesCount < 1) throw new InvalidOperationException("SpeciesCount must be >= 1.");
        if (!IsProportion(ElitismProportion)) throw new InvalidOperationException("ElitismProportion must be in the interval [0,1].");
        if (!IsProportion(SelectionProportion)) throw new InvalidOperationException("SelectionProportion must be in the interval [0,1].");
        if (!IsProportion(OffspringAsexualProportion)) throw new InvalidOperationException("OffspringAsexualProportion must be in the interval [0,1].");
        if (!IsProportion(OffspringRecombinationProportion)) throw new InvalidOperationException("OffspringRecombinationProportion must be in the interval [0,1].");
        if (!IsProportion(InterspeciesMatingProportion)) throw new InvalidOperationException("InterspeciesMatingProportion must be in the interval [0,1].");
        if (StatisticsMovingAverageHistoryLength < 1) throw new InvalidOperationException("StatisticsMovingAverageHistoryLength must be >= 1.");
        if (Math.Abs(OffspringAsexualProportion + OffspringRecombinationProportion - 1.0) > 1e-6) throw new InvalidOperationException("OffspringAsexualProportion and OffspringRecombinationProportion must sum to 1.0");

        static bool IsProportion(double p) => p >= 0 && p <= 1.0;
    }
}
