// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.EvolutionAlgorithm;

/// <summary>
/// Represents some universal properties of a genome in SharpNEAT.
/// </summary>
public interface IGenome
{
    /// <summary>
    /// Gets the genome's unique ID. IDs are unique across all genomes created from a single
    /// IGenomeFactory.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The generation that a genome was born/created in. Used to track genome age.
    /// </summary>
    int BirthGeneration { get; }

    /// <summary>
    /// The genome's fitness info.
    /// </summary>
    FitnessInfo FitnessInfo { get; set; }

    /// <summary>
    /// Gets a value that is representative of the genome's complexity.
    /// </summary>
    double Complexity { get; }
}
