// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Speciation;

/// <summary>
/// Species statistics.
/// </summary>
public class SpeciesStats
{
    // Real/Continuous Stats
    /// <summary>
    /// The mean/average fitness of all genomes in the species.
    /// </summary>
    public double MeanFitness;

    /// <summary>
    /// The species target size. This is the real value from the target size calculation; this is 'resolved' to integer
    /// value <see cref="TargetSizeInt"/> to give the final/actual target size in the next generation.
    /// </summary>
    public double TargetSizeReal;

    // Integer Stats
    /// <summary>
    /// The species target size.
    /// </summary>
    public int TargetSizeInt;

    /// <summary>
    /// The number of fittest genomes in the species to be kept in the species in the next generation.
    /// Thus the number of slots for new offspring is <see cref="TargetSizeInt"/> - <see cref="EliteSizeInt"/>.
    /// </summary>
    public int EliteSizeInt;

    /// <summary>
    /// The number of offspring genomes to produce for this species.
    /// This value is simply <see cref="TargetSizeInt"/> - <see cref="EliteSizeInt"/>.
    /// This count is broken down further into <see cref="OffspringAsexualCount"/> and <see cref="OffspringRecombinationCount"/>,
    /// i.e. <see cref="OffspringCount"/> = <see cref="OffspringAsexualCount"/> + <see cref="OffspringRecombinationCount"/>.
    /// </summary>
    public int OffspringCount;

    /// <summary>
    /// The number of offspring to produce through asexual reproduction.
    /// </summary>
    public int OffspringAsexualCount;

    /// <summary>
    /// The number of offspring to produce through recombination reproduction.
    /// </summary>
    public int OffspringRecombinationCount;

    // Selection data.
    /// <summary>
    /// The number of fittest genomes in the species to be selected from for selecting parents for recombination and asexual reproduction.
    /// </summary>
    public int SelectionSizeInt;
}
