// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;
using SharpNeat.Evaluation;

namespace SharpNeat.EvolutionAlgorithm;

/// <summary>
/// A population of genomes.
/// </summary>
/// <typeparam name="TGenome">Genome type.</typeparam>
public abstract class Population<TGenome>
    where TGenome : IGenome
{
    const int __minTargetSize = 2;

    #region Constructors

    /// <summary>
    /// Construct an empty population with the specified target size.
    /// </summary>
    /// <param name="targetSize">Population target size.</param>
    public Population(int targetSize)
    {
        if (targetSize < __minTargetSize) throw new ArgumentOutOfRangeException(nameof(targetSize), $"Minimum target size is {__minTargetSize}");
        TargetSize = targetSize;

        GenomeList = new List<TGenome>();
        Stats = CreatePopulatonStats();
    }

    /// <summary>
    /// Construct a population with the provided genome list and target size.
    /// </summary>
    /// <param name="targetSize">Population target size.</param>
    /// <param name="genomeList">A list of genomes (this allowed to be empty).</param>
    public Population(int targetSize, List<TGenome> genomeList)
    {
        if(targetSize < __minTargetSize) throw new ArgumentOutOfRangeException(nameof(targetSize), $"Minimum target size is {__minTargetSize}");
        TargetSize = targetSize;

        GenomeList = genomeList;
        Stats = CreatePopulatonStats();
    }

    #endregion

    #region Properties

    /// <summary>
    /// The list of genomes that make up the population.
    /// </summary>
    public List<TGenome> GenomeList { get; }

    /// <summary>
    /// Gets the current best genome.
    /// </summary>
    /// <remarks>
    /// Note. If the evolution algorithm has not yet been initialised then this will simply return the genome at index zero in the population.
    /// </remarks>
    public TGenome BestGenome => GenomeList[Stats.BestGenomeIndex];

    /// <summary>
    /// Gets the desired number of genomes in the population.
    /// </summary>
    /// <remarks>
    /// During certain phases of the evolution algorithm the length of <see cref="GenomeList"/> will vary and
    /// therefore it may not match <see cref="TargetSize"/> at any given point in time, thus this
    /// property is the definitive source of the population size.
    /// </remarks>
    public int TargetSize { get; }

    /// <summary>
    /// Population statistics.
    /// </summary>
    public PopulationStatistics Stats { get; }

    #endregion

    #region Public / Protected Methods

    /// <summary>
    /// Update the population statistics.
    /// </summary>
    /// <param name="fitnessComparer">A genome fitness comparer.</param>
    /// <param name="rng">Random source.</param>
    public abstract void UpdateStats(IComparer<FitnessInfo> fitnessComparer, IRandomSource rng);

    /// <summary>
    /// Create a new population statistics object.
    /// </summary>
    /// <returns>A new instance of <see cref="Stats"/>.</returns>
    protected virtual PopulationStatistics CreatePopulatonStats()
    {
        return new PopulationStatistics();
    }

    #endregion
}
