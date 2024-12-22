// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Runtime.InteropServices;
using Redzen.Sorting;
using static SharpNeat.Neat.NeatPopulationUtils;

namespace SharpNeat.Neat;

/// <summary>
/// A NEAT population.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public class NeatPopulation<TScalar> : Population<NeatGenome<TScalar>>
    where TScalar : unmanaged
{
    // ENHANCEMENT: Consider increasing buffer capacity, and different capacities for the two different buffers.
    const int __defaultInnovationHistoryBufferSize = 0x20_000; // = 131,072 decimal.

    // A reusable/working list. Stores the index of the species with the fittest genome, or multiple indexes when
    // two or more species are tied at first place.
    private readonly List<int> _fittestSpeciesIndexList = [];

    /// <summary>
    /// NeatGenome metadata.
    /// </summary>
    public MetaNeatGenome<TScalar> MetaNeatGenome { get; }

    /// <summary>
    /// NeatGenome builder.
    /// </summary>
    public INeatGenomeBuilder<TScalar> GenomeBuilder { get; }

    /// <summary>
    /// Genome ID sequence; for obtaining new genome IDs.
    /// </summary>
    public Int32Sequence GenomeIdSeq { get; }

    /// <summary>
    /// Innovation ID sequence; for obtaining new innovation IDs.
    /// </summary>
    public Int32Sequence InnovationIdSeq { get; }

    /// <summary>
    /// A history buffer of added nodes.
    /// Used when adding new nodes to check if an identical node has been added to a genome elsewhere in the population.
    /// This allows re-use of the same innovation ID for like nodes.
    /// </summary>
    public AddedNodeBuffer AddedNodeBuffer { get; }

    /// <summary>
    /// Species array.
    /// </summary>
    public Species<TScalar>[]? SpeciesArray { get; set; }

    /// <summary>
    /// NeatPopulation statistics.
    /// </summary>
    public NeatPopulationStatistics NeatPopulationStats => (NeatPopulationStatistics)Stats;

    #region Constructors

    /// <summary>
    /// Construct a new population with the provided genomes.
    /// </summary>
    /// <param name="metaNeatGenome">NeatGenome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="targetSize">Population target size.</param>
    /// <param name="genomeList">A list of genomes that will make up the population.</param>
    public NeatPopulation(
        MetaNeatGenome<TScalar> metaNeatGenome,
        INeatGenomeBuilder<TScalar> genomeBuilder,
        int targetSize,
        List<NeatGenome<TScalar>> genomeList)
        : base(targetSize, genomeList)
    {
        GetMaxObservedIds(
            genomeList,
            out int maxGenomeId,
            out int maxInnovationId);

        MetaNeatGenome = metaNeatGenome ?? throw new ArgumentNullException(nameof(metaNeatGenome));
        GenomeBuilder = genomeBuilder ?? throw new ArgumentNullException(nameof(genomeBuilder));
        GenomeIdSeq = new Int32Sequence(maxGenomeId + 1);
        InnovationIdSeq = new Int32Sequence(maxInnovationId + 1);
        AddedNodeBuffer = new AddedNodeBuffer(__defaultInnovationHistoryBufferSize);
    }

    /// <summary>
    /// Construct a new population with the provided genomes and accompanying objects.
    /// </summary>
    /// <param name="metaNeatGenome">NeatGenome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="targetSize">Population target size.</param>
    /// <param name="genomeList">A list of genomes that will make up the population.</param>
    /// <param name="genomeIdSeq">Genome ID sequence.</param>
    /// <param name="innovationIdSeq">Innovation ID sequence.</param>
    public NeatPopulation(
        MetaNeatGenome<TScalar> metaNeatGenome,
        INeatGenomeBuilder<TScalar> genomeBuilder,
        int targetSize,
        List<NeatGenome<TScalar>> genomeList,
        Int32Sequence genomeIdSeq,
        Int32Sequence innovationIdSeq)
    : this(metaNeatGenome, genomeBuilder, targetSize, genomeList, genomeIdSeq, innovationIdSeq, __defaultInnovationHistoryBufferSize)
    {
    }

    /// <summary>
    /// Construct a new population with the provided genomes and accompanying objects.
    /// </summary>
    /// <param name="metaNeatGenome">NeatGenome metadata.</param>
    /// <param name="genomeBuilder">NeatGenome builder.</param>
    /// <param name="targetSize">Population target size.</param>
    /// <param name="genomeList">A list of genomes that will make up the population.</param>
    /// <param name="genomeIdSeq">Genome ID sequence.</param>
    /// <param name="innovationIdSeq">Innovation ID sequence.</param>
    /// <param name="addedNodeHistoryBufferSize">The size to allocate for the added node history buffer.</param>
    public NeatPopulation(
        MetaNeatGenome<TScalar> metaNeatGenome,
        INeatGenomeBuilder<TScalar> genomeBuilder,
        int targetSize,
        List<NeatGenome<TScalar>> genomeList,
        Int32Sequence genomeIdSeq,
        Int32Sequence innovationIdSeq,
        int addedNodeHistoryBufferSize)
    : base(targetSize, genomeList)
    {
        MetaNeatGenome = metaNeatGenome ?? throw new ArgumentNullException(nameof(metaNeatGenome));
        GenomeBuilder = genomeBuilder ?? throw new ArgumentNullException(nameof(genomeBuilder));
        GenomeIdSeq = genomeIdSeq ?? throw new ArgumentNullException(nameof(genomeIdSeq));
        InnovationIdSeq = innovationIdSeq ?? throw new ArgumentNullException(nameof(innovationIdSeq));
        AddedNodeBuffer = new AddedNodeBuffer(addedNodeHistoryBufferSize);

        // Assert that the ID sequence objects represent an ID higher than any existing ID used by the genomes.
        Debug.Assert(ValidateIdSequences(genomeList, genomeIdSeq, innovationIdSeq));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialise (or re-initialise) the population species.
    /// </summary>
    /// <param name="speciationStrategy">The speciation strategy to use.</param>
    /// <param name="speciesCount">The required number of species.</param>
    /// <param name="genomeComparerDescending">A genome comparer for sorting by fitness in descending order.</param>
    /// <param name="rng">Random source.</param>
    public void InitialiseSpecies(
        ISpeciationStrategy<NeatGenome<TScalar>,TScalar> speciationStrategy,
        int speciesCount,
        IComparer<NeatGenome<TScalar>> genomeComparerDescending,
        IRandomSource rng)
    {
        // Allocate the genomes to species.
        Species<TScalar>[] speciesArr = speciationStrategy.SpeciateAll(GenomeList, speciesCount, rng);
        if(speciesArr is null || speciesArr.Length != speciesCount)
            throw new InvalidOperationException("Species array is null or has incorrect length.");

        SpeciesArray = speciesArr;

        // Sort the genomes in each species by primary fitness, highest fitness first.
        // We use an unstable sort; this ensures that the order of equally fit genomes is randomized, which in turn
        // randomizes which genomes are in the subset if elite genomes that are preserved for the next generation - if lots
        // of genomes have equally high fitness.
        foreach(var species in speciesArr)
        {
            SortUtils.SortUnstable(
                CollectionsMarshal.AsSpan(species.GenomeList),
                genomeComparerDescending,
                rng);
        }
    }

    /// <inheritdoc/>
    public override void UpdateStats(
        IComparer<FitnessInfo> fitnessComparer,
        IRandomSource rng)
    {
        // Calculate some population-wide stats; these are non-NEAT specific.
        CalcPopulationStats(out double primaryFitnessSum, out double complexitySum, out double maxComplexity);

        // Calculate NEAT specific and species based stats.
        CalcNeatPopulationStats(
            fitnessComparer, rng,
            out double sumMeanFitness,
            out double sumBestFitness,
            out int bestGenomeIdx,
            out int bestGenomeSpeciesIdx);

        // Update PopulationStatistics object.
        PopulationStatistics stats = Stats;

        int genomeCount = GenomeList.Count;
        var bestGenome = GenomeList[bestGenomeIdx];

        // Update fitness stats.
        stats.BestGenomeIndex = bestGenomeIdx;
        stats.BestFitness = bestGenome.FitnessInfo;
        stats.MeanFitness = primaryFitnessSum / genomeCount;
        stats.BestFitnessHistory.Enqueue(bestGenome.FitnessInfo.PrimaryFitness);

        // Update complexity stats.
        stats.BestComplexity = bestGenome.Complexity;
        double meanComplexity = complexitySum / genomeCount;
        stats.MeanComplexity = meanComplexity;
        stats.MeanComplexityHistory.Enqueue(meanComplexity);
        stats.MaxComplexity = maxComplexity;

        // Update NeatPopulationStatistics object.
        NeatPopulationStats.BestGenomeSpeciesIdx = bestGenomeSpeciesIdx;
        NeatPopulationStats.SumSpeciesMeanFitness = sumMeanFitness;
        NeatPopulationStats.AverageSpeciesBestFitness = sumBestFitness / SpeciesArray!.Length;
    }

    /// <summary>
    /// Clear the genome list of all species.
    /// </summary>
    public void ClearAllSpecies()
    {
        foreach(var species in SpeciesArray!)
            species.GenomeList.Clear();
    }

    /// <summary>
    /// Returns true if there is at least one empty species.
    /// </summary>
    /// <returns>True if there is at least one empty species; otherwise false.</returns>
    public bool ContainsEmptySpecies()
    {
        return SpeciesArray!.Any(x => (x.GenomeList.Count == 0));
    }

    #endregion

    #region Protected Methods

    /// <inheritdoc/>
    protected override PopulationStatistics CreatePopulatonStats()
    {
        return new NeatPopulationStatistics();
    }

    #endregion

    #region Private Methods

    private void CalcPopulationStats(
        out double primaryFitnessSum,
        out double complexitySum,
        out double maxComplexity)
    {
        // Calc sum of PrimaryFitness, and sum of Complexity.
        List<NeatGenome<TScalar>> genomeList = GenomeList;
        primaryFitnessSum = 0.0;
        complexitySum = 0.0;
        maxComplexity = 0.0;

        // Loop all genomes.
        foreach(var genome in genomeList)
        {
            primaryFitnessSum += genome.FitnessInfo.PrimaryFitness;
            complexitySum += genome.Complexity;
            if(genome.Complexity > maxComplexity)
                maxComplexity = genome.Complexity;
        }
    }

    private void CalcNeatPopulationStats(
        IComparer<FitnessInfo> fitnessComparer,
        IRandomSource rng,
        out double sumMeanFitness,
        out double sumBestFitness,
        out int bestGenomeIdx,
        out int bestGenomeSpeciesIdx)
    {
        // Loop the species; calculate the each species' mean fitness, and calc a sum over those mean fitnesses.
        sumMeanFitness = 0.0;
        sumBestFitness = 0.0;
        Species<TScalar>[] speciesArr = SpeciesArray!;
        for(int i=0; i < speciesArr.Length; i++)
        {
            Species<TScalar> species = speciesArr[i];

            // Calculate the genome mean fitness for the current species, and store the result.
            double meanFitness = species.GenomeList.Average(x => x.FitnessInfo.PrimaryFitness);
            species.Stats.MeanFitness = meanFitness;

            // Keep a sum of the mean, and best fitness scores.
            sumMeanFitness += meanFitness;
            sumBestFitness += species.GenomeList[0].FitnessInfo.PrimaryFitness;
        }

        // Select a population-wide best genome.
        // Notes.
        // We choose the population champion genome from the subset of genomes made up from the first genome in each species.
        // Genome index zero in each species is the best genome in that species, and the species genomes are sorted with a
        // perfectly unstable sort, therefore if there are multiple genomes in equal first place then the genome at index zero
        // is random within the set of first place (tied) genomes.

        // Ensure this reusable working list is empty/reset.
        _fittestSpeciesIndexList.Clear();

        // Initialise the best fitness to the fitness of the best genome in species zero.
        FitnessInfo bestFitnessInfo = speciesArr[0].GenomeList[0].FitnessInfo;
        _fittestSpeciesIndexList.Add(0);

        // Loop the remaining species.
        for(int i=1; i < speciesArr.Length; i++)
        {
            FitnessInfo speciesBestFitness = speciesArr[i].GenomeList[0].FitnessInfo;
            int comparisonResult = fitnessComparer.Compare(speciesBestFitness, bestFitnessInfo);

            if(comparisonResult > 0)
            {
                // A new best fitness has been found.
                _fittestSpeciesIndexList.Clear();
                _fittestSpeciesIndexList.Add(i);
                bestFitnessInfo = speciesBestFitness;
            }
            else if(comparisonResult == 0)
            {
                // Add species to the list of equally fit fittest candidates.
                _fittestSpeciesIndexList.Add(i);
            }
        }

        // Select one of the candidate species at random.
        if(_fittestSpeciesIndexList.Count == 1)
        {
            // There is only one candidate species; select it.
            bestGenomeSpeciesIdx = _fittestSpeciesIndexList[0];
        }
        else
        {
            // Select one candidate species at random.
            bestGenomeSpeciesIdx = _fittestSpeciesIndexList[rng.Next(_fittestSpeciesIndexList.Count)];
        }

        // Get a reference to the fittest genome.
        var bestGenome = speciesArr[bestGenomeSpeciesIdx].GenomeList[0];

        // Determine the index of the fittest genome in the population-wide genome list;
        // we resort to a scan to do this.
        bestGenomeIdx = -1;
        for(int genomeIdx=0; genomeIdx < GenomeList.Count; genomeIdx++)
        {
            if(GenomeList[genomeIdx] == bestGenome)
            {
                bestGenomeIdx = genomeIdx;
                break;
            }
        }
    }

    #endregion
}
