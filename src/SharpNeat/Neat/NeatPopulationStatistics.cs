// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Neat;

/// <summary>
/// NeatPopulation statistics.
/// </summary>
public class NeatPopulationStatistics : PopulationStatistics
{
    #region Auto Properties [NeatPopulation Statistics]

    /// <summary>
    /// Index of the species that the best genome is within.
    /// </summary>
    public int BestGenomeSpeciesIdx { get; set; }

    /// <summary>
    /// Sum of species fitness means.
    /// </summary>
    public double SumSpeciesMeanFitness { get; set; }

    /// <summary>
    /// The average (mean) fitness calculated over all species' best/champ genomes.
    /// </summary>
    public double AverageSpeciesBestFitness { get; set; }

    #endregion
}
