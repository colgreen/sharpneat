// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Speciation;

/// <summary>
/// Represents a NEAT species.
/// </summary>
/// <typeparam name="T">Neural net numeric data type.</typeparam>
public class Species<T>
    where T : struct
{
    /// <summary>
    /// Species ID.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Species centroid.
    /// </summary>
    public ConnectionGenes<T> Centroid { get; set; }

    /// <summary>
    /// The genomes that are within the species.
    /// </summary>
    public List<NeatGenome<T>> GenomeList { get; }

    /// <summary>
    /// A working dictionary of genomes keyed by ID.
    /// </summary>
    public Dictionary<int,NeatGenome<T>> GenomeById { get; }

    /// <summary>
    /// Working list of genomes to be added to GenomeById at the end of a k-means iteration.
    /// </summary>
    public List<NeatGenome<T>> PendingAddsList { get; }

    /// <summary>
    /// Working list of genome IDs to remove from GenomeById at the end of a k-means iteration.
    /// </summary>
    public List<int> PendingRemovesList { get; }

    /// <summary>
    /// Species statistics.
    /// </summary>
    public SpeciesStats Stats { get; }

    #region Constructor

    /// <summary>
    /// Construct with the given species ID, centroid and initial capacity.
    /// </summary>
    /// <param name="id">Species ID.</param>
    /// <param name="centroid">Species centroid.</param>
    /// <param name="capacity">Initial capacity for the species genome list.</param>
    public Species(int id, ConnectionGenes<T> centroid, int capacity = 0)
    {
        Id = id;
        Centroid = centroid;
        GenomeList = new List<NeatGenome<T>>(capacity);
        GenomeById = new Dictionary<int,NeatGenome<T>>(capacity);
        PendingAddsList = new List<NeatGenome<T>>(capacity);
        PendingRemovesList = new List<int>(capacity);
        Stats = new SpeciesStats();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Transfer genomes from GenomeList into GenomeById.
    /// </summary>
    public void LoadWorkingDictionary()
    {
        GenomeById.Clear();
        foreach(var genome in GenomeList)
            GenomeById.Add(genome.Id, genome);

        GenomeList.Clear();
    }

    /// <summary>
    /// Transfer genomes from GenomeById into GenomeList.
    /// </summary>
    public void FlushWorkingDictionary()
    {
        GenomeList.Clear();
        GenomeList.AddRange(GenomeById.Values);
        GenomeById.Clear();
    }

    /// <summary>
    /// Complete all pending genome moves for this species.
    /// </summary>
    public void CompletePendingMoves()
    {
        // Remove genomes that are marked for removal.
        foreach(int id in PendingRemovesList)
            GenomeById.Remove(id);

        // Process pending additions.
        foreach(var genome in PendingAddsList)
            GenomeById.Add(genome.Id, genome);

        PendingRemovesList.Clear();
        PendingAddsList.Clear();
    }

    /// <summary>
    /// Calculates the sum total complexity of all genomes within the species.
    /// </summary>
    /// <returns>The sum of <see cref="IGenome.Complexity"/> for all genomes in the species.</returns>
    public double CalcTotalComplexity()
    {
        double total = 0.0;
        foreach(var genome in GenomeList)
            total += genome.Complexity;

        return total;
    }

    /// <summary>
    /// Calculates the mean complexity of genomes within the species.
    /// </summary>
    /// <returns>The arithmetic mean of <see cref="IGenome.Complexity"/> for all genomes in the species.</returns>
    public double CalcMeanComplexity()
    {
        return CalcTotalComplexity() / GenomeList.Count;
    }

    #endregion
}
