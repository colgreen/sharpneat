// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.EvolutionAlgorithm;

/// <summary>
/// Population statistics.
/// </summary>
public class PopulationStatistics
{
    #region Genome Fitness Stats

    /// <summary>
    /// Index of the best genome in the population's genome list.
    /// </summary>
    public int BestGenomeIndex { get; set; }

    /// <summary>
    /// FitnessInfo for the current best genome.
    /// </summary>
    public FitnessInfo BestFitness { get; set; }

    /// <summary>
    /// Mean of the genome primary fitness for the current population.
    /// </summary>
    public double MeanFitness { get; set; }

    /// <summary>
    /// A trailing history of best fitness scores at each of the previous N generations.
    /// This object can also provide a mean over the historical fitness scores, thus providing
    /// a moving average value.
    /// </summary>
    public CircularBufferWithStats BestFitnessHistory { get; set; } = new CircularBufferWithStats(100);

    #endregion

    #region Genome Complexity Stats

    /// <summary>
    /// Complexity of the current best genome.
    /// </summary>
    public double BestComplexity { get; set; }

    /// <summary>
    /// Mean genome complexity for the current population.
    /// </summary>
    public double MeanComplexity { get; set; }

    /// <summary>
    /// Max genome complexity for the current population.
    /// </summary>
    public double MaxComplexity { get; set; }

    /// <summary>
    /// A trailing history of mean genome complexity at each of the previous N generations.
    /// This object can also provide a mean over the historical values, thus providing
    /// a moving average value.
    /// </summary>
    public CircularBufferWithStats MeanComplexityHistory { get; set; } = new CircularBufferWithStats(100);

    #endregion
}
