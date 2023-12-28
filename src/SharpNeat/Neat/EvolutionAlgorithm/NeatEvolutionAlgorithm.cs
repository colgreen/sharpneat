// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Redzen.Sorting;
using SharpNeat.Neat.ComplexityRegulation;
using SharpNeat.Neat.Reproduction.Asexual;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;
using SharpNeat.Neat.Reproduction.Recombination;

namespace SharpNeat.Neat.EvolutionAlgorithm;

/// <summary>
/// The NEAT  evolution algorithm.
/// </summary>
/// <typeparam name="TScalar">Neural net connection weight and signal data type.</typeparam>
public class NeatEvolutionAlgorithm<TScalar> : IEvolutionAlgorithm
    where TScalar : struct, IBinaryFloatingPointIeee754<TScalar>
{
    NeatEvolutionAlgorithmSettings _eaSettingsCurrent;
    readonly NeatEvolutionAlgorithmSettings _eaSettingsComplexifying;
    readonly NeatEvolutionAlgorithmSettings _eaSettingsSimplifying;
    readonly IGenomeListEvaluator<NeatGenome<TScalar>> _evaluator;
    readonly ISpeciationStrategy<NeatGenome<TScalar>,TScalar> _speciationStrategy;
    readonly NeatPopulation<TScalar> _pop;
    readonly IComplexityRegulationStrategy _complexityRegulationStrategy;
    readonly IRandomSource _rng;
    readonly IComparer<NeatGenome<TScalar>> _genomeComparerDescending;

    readonly Int32Sequence _generationSeq;
    readonly NeatAsexualReproduction<TScalar> _asexualReproduction;
    readonly NeatRecombination<TScalar> _recombination;

    readonly OffspringBuilder<TScalar> _offspringBuilder;
    readonly NeatEvolutionAlgorithmStatistics _eaStats = new();

    // Fields used to calculate the evaluations per second statistic, on each successive update.
    static readonly TimeSpan __oneSec = TimeSpan.FromSeconds(1);
    ulong _evalCountPrev;
    DateTime _evalCountPrevSampleTime = DateTime.MinValue;

    #region Constructors

    /// <summary>
    /// Construct a new instance.
    /// </summary>
    /// <param name="eaSettings">NEAT evolution algorithm settings.</param>
    /// <param name="evaluator">An evaluator of lists of genomes.</param>
    /// <param name="speciationStrategy">Speciation strategy.</param>
    /// <param name="population">An initial population of genomes.</param>
    /// <param name="complexityRegulationStrategy">Complexity regulation strategy.</param>
    /// <param name="asexualReproductionSettings">Asexual reproduction settings.</param>
    /// <param name="recombinationSettings">Recombination reproduction settings.</param>
    /// <param name="weightMutationScheme">Connection weight mutation scheme.</param>
    /// <param name="rng">Random source (optional).</param>
    public NeatEvolutionAlgorithm(
        NeatEvolutionAlgorithmSettings eaSettings,
        IGenomeListEvaluator<NeatGenome<TScalar>> evaluator,
        ISpeciationStrategy<NeatGenome<TScalar>,TScalar> speciationStrategy,
        NeatPopulation<TScalar> population,
        IComplexityRegulationStrategy complexityRegulationStrategy,
        NeatAsexualReproductionSettings asexualReproductionSettings,
        NeatRecombinationSettings recombinationSettings,
        WeightMutationScheme<TScalar> weightMutationScheme,
        IRandomSource? rng = null)
    {
        // Perform some basic validation of the provided settings objects.
        ArgumentNullException.ThrowIfNull(eaSettings);
        ArgumentNullException.ThrowIfNull(asexualReproductionSettings);
        ArgumentNullException.ThrowIfNull(recombinationSettings);
        eaSettings.Validate();
        asexualReproductionSettings.Validate();
        recombinationSettings.Validate();

        // Store instance fields, and null check as we go.
        _eaSettingsCurrent = eaSettings;
        _eaSettingsComplexifying = eaSettings;
        _eaSettingsSimplifying = eaSettings.CreateSimplifyingSettings();

        _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        _speciationStrategy = speciationStrategy ?? throw new ArgumentNullException(nameof(speciationStrategy));
        _pop = population ?? throw new ArgumentNullException(nameof(population));
        _complexityRegulationStrategy = complexityRegulationStrategy ?? throw new ArgumentNullException(nameof(complexityRegulationStrategy));
        _rng = rng ?? RandomDefaults.CreateRandomSource();
        _genomeComparerDescending = new GenomeComparerDescending(evaluator.FitnessComparer);

        if(eaSettings.SpeciesCount > population.TargetSize)
            throw new ArgumentException("Species count is higher than the population size.");

        _generationSeq = new Int32Sequence();

        _asexualReproduction = new NeatAsexualReproduction<TScalar>(
            _pop.MetaNeatGenome, _pop.GenomeBuilder,
            _pop.GenomeIdSeq, population.InnovationIdSeq, _generationSeq,
            _pop.AddedNodeBuffer, asexualReproductionSettings, weightMutationScheme);

        _recombination = new NeatRecombination<TScalar>(
            _pop.MetaNeatGenome, _pop.GenomeBuilder,
            _pop.GenomeIdSeq, _generationSeq,
            recombinationSettings);

        _offspringBuilder = new OffspringBuilder<TScalar>(
            _asexualReproduction,
            _recombination,
            eaSettings.InterspeciesMatingProportion,
            evaluator.FitnessComparer);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the <see cref="NeatPopulation{T}"/>.
    /// </summary>
    public NeatPopulation<TScalar> Population => _pop;

    /// <summary>
    /// Gets the current evolution algorithm statistics.
    /// </summary>
    public EvolutionAlgorithmStatistics Stats => _eaStats;

    /// <summary>
    /// Gets the current complexity regulation mode.
    /// </summary>
    public ComplexityRegulationMode ComplexityRegulationMode
    {
        get => _complexityRegulationStrategy.CurrentMode;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialise the evolutionary algorithm.
    /// </summary>
    public void Initialise()
    {
        // Evaluate each genome in the new population.
        _evaluator.Evaluate(_pop.GenomeList);

        // Initialise species.
        _pop.InitialiseSpecies(
            _speciationStrategy,
            _eaSettingsCurrent.SpeciesCount,
            _genomeComparerDescending,
            _rng);

        // Update population and evolution algorithm statistics.
        UpdateStats(evaluationCountDelta: (ulong)_pop.GenomeList.Count, 0, 0, 0);
    }

    /// <summary>
    /// Perform one generation of the evolution algorithm.
    /// </summary>
    public void PerformOneGeneration()
    {
        if(_pop.SpeciesArray is null)
            throw new InvalidOperationException("Algorithm is not initialised.");

        // Create offspring.
        List<NeatGenome<TScalar>> offspringList = _offspringBuilder.CreateOffspring(
            _pop.SpeciesArray, _rng,
            out int offspringAsexualCount,
            out int offspringRecombinationCount,
            out int offspringInterspeciesCount);

        // Trim population back to elite genomes only.
        // Note. Returns a flag that indicates if there is at least one empty species following trimming.
        TrimSpeciesBackToElite(out bool emptySpeciesFlag);

        // Rebuild _pop.GenomeList. It will now contain just the elite genomes from each species.
        RebuildGenomeList();

        // Append offspring genomes to the elite genomes in _pop.GenomeList. We do this before calling the
        // _genomeListEvaluator.Evaluate() because some evaluation schemes re-evaluate the elite genomes
        // (otherwise we could just evaluate offspringList).
        _pop.GenomeList.AddRange(offspringList);

        // Genome evaluation.
        DoGenomeEvaluation(offspringList, out ulong evaluationCount);

        // Integrate offspring into the species.
        IntegrateOffspringIntoSpecies(offspringList, emptySpeciesFlag);

        // Update statistics.
        UpdateStats(evaluationCount, offspringAsexualCount, offspringRecombinationCount, offspringInterspeciesCount);

        // Complexity regulation.
        UpdateComplexityRegulationMode();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loops through species, and for each trims the species genomeList back to the number of elite
    /// genomes defined for that species.
    /// </summary>
    /// <param name="emptySpeciesFlag">Returns true if there is at least one empty species following trimming.</param>
    /// <remarks>
    /// The genomes in species.GenomeList are ordered best to worst, thus genomes are removed from the end of the lists.
    /// </remarks>
    private void TrimSpeciesBackToElite(out bool emptySpeciesFlag)
    {
        emptySpeciesFlag = false;
        int speciesCount = _pop.SpeciesArray!.Length;

        for(int i=0; i < speciesCount; i++)
        {
            Species<TScalar> species = _pop.SpeciesArray[i];
            int eliteSizeInt = species.Stats.EliteSizeInt;
            int removeCount = species.GenomeList.Count - eliteSizeInt;
            species.GenomeList.RemoveRange(eliteSizeInt, removeCount);

            if(eliteSizeInt == 0)
                emptySpeciesFlag = true;
        }
    }

    /// <summary>
    /// Rebuild _genomeList from genomes held within the species.
    /// </summary>
    private void RebuildGenomeList()
    {
        var genomeList = _pop.GenomeList;

        genomeList.Clear();
        foreach(var species in _pop.SpeciesArray!)
        {
            genomeList.AddRange(species.GenomeList);
        }
    }

    /// <summary>
    /// Perform the genome evaluation stage of the evolution algorithm.
    /// </summary>
    /// <param name="offspringList">The list of offspring genomes to evaluate.</param>
    /// <param name="evaluationCount">Returns the number of evaluations that were performed.</param>
    private void DoGenomeEvaluation(
        List<NeatGenome<TScalar>> offspringList,
        out ulong evaluationCount)
    {
        // TODO: Review this. We don't necessarily want to re-evaluate genomes even if the evaluation scheme is non-deterministic.

        // If the evaluation scheme is deterministic then we only need to evaluate genome that have not been evaluated yet, i.e.
        // the offspring genomes; the elite genomes that remain in the population from the previous generation have already been
        // evaluated and assigned a fitness score, so we can avoid the effort of re-evaluating those. If however the evaluation scheme
        // is non-deterministic, then we must re-evaluate all genomes in the population, both old and new genome.
        if(_evaluator.IsDeterministic)
        {
            _evaluator.Evaluate(offspringList);
            evaluationCount = (ulong)offspringList.Count;
        }
        else
        {
            _evaluator.Evaluate(_pop.GenomeList);
            evaluationCount = (ulong)_pop.GenomeList.Count;
        }

        // Note. In future _evaluator may return an evaluation count, as it may apply a strategy that determines
        // which genomes to evaluate. For now we just evaluate all genomes in the chosen genome list
        // (offspringList or _pop.GenomeList) and return the length of that chosen list.
    }

    private void IntegrateOffspringIntoSpecies(List<NeatGenome<TScalar>> offspringList, bool emptySpeciesFlag)
    {
        // Handle special case of one or more species having a zero target size (dead species).
        if(emptySpeciesFlag)
        {
            // There are one or more terminated species. Therefore we need to fully re-speciate all genomes
            // to evenly divide them between the required number of species.

            // Clear all genomes from species (note. we still have all genomes in _pop.GenomeList).
            _pop.ClearAllSpecies();

            // Re-initialise the species.
            _pop.InitialiseSpecies(
                _speciationStrategy,
                _eaSettingsCurrent.SpeciesCount,
                _genomeComparerDescending,
                _rng);
        }
        else
        {
            // Integrate offspring into the existing species.
            _speciationStrategy.SpeciateAdd(offspringList, _pop.SpeciesArray!, _rng);

            // Sort the genomes in each species by primary fitness, highest fitness first.
            // We use an unstable sort; this ensures that the order of equally fit genomes is randomized, which in turn
            // randomizes which genomes are in the subset if elite genomes that are preserved for the next generation,
            // i.e. when many genomes have equally high fitness.
            foreach(var species in _pop.SpeciesArray!)
            {
                SortUtils.SortUnstable(
                    CollectionsMarshal.AsSpan(species.GenomeList),
                    _genomeComparerDescending,
                    _rng);
            }
        }

        Debug.Assert(!_pop.ContainsEmptySpecies(), "Speciation resulted in one or more empty species.");
    }

    private void UpdateStats(
        ulong evaluationCountDelta,
        int offspringAsexualCount,
        int offspringRecombinationCount,
        int offspringInterspeciesCount)
    {
        // Record the point in clock time we entered this method.
        _eaStats.SampleTime = DateTime.UtcNow;

        // Update population statistics.
        _pop.UpdateStats(_evaluator.FitnessComparer, _rng);

        // Store the current generation number, and increment.
        _eaStats.Generation = _generationSeq.Peek;
        _generationSeq.Next();

        // Test if the evaluator is signalling that the best fitness is good enough to stop the evolution algorithm.
        _eaStats.StopConditionSatisfied = _evaluator.TestForStopCondition(_pop.Stats.BestFitness);

        // Update total number of evaluations performed.
        _eaStats.TotalEvaluationCount += evaluationCountDelta;

        // Calculate/update evaluations per second stat.
        // Skip this calc for the first call here, as we need to two successive calls to calc evaluation per sec.
        if(_evalCountPrevSampleTime == DateTime.MinValue)
        {
            // Record the count and sample time ready for the next call to this subroutine.
            _evalCountPrev = _eaStats.TotalEvaluationCount;
            _evalCountPrevSampleTime = _eaStats.SampleTime;
        }
        else
        {
            // Calc elapsed time since the previous update to this state. If it is less than one second ago then skip the update,
            // as the timespan may be very short, thus giving an unrepresentative evaluations per second value.
            TimeSpan elapsed = _eaStats.SampleTime - _evalCountPrevSampleTime;
            if(elapsed > __oneSec)
            {
                double countDelta = _eaStats.TotalEvaluationCount - _evalCountPrev;
                _eaStats.EvaluationsPerSec = (countDelta * 1e7) / elapsed.Ticks;

                // Record the count and sample time ready for the next call to this subroutine.
                _evalCountPrev = _eaStats.TotalEvaluationCount;
                _evalCountPrevSampleTime = _eaStats.SampleTime;
            }
        }

        // Update total number of offspring genomes produced.
        _eaStats.TotalOffspringCount += (ulong)(offspringAsexualCount + offspringRecombinationCount);
        _eaStats.TotalOffspringAsexualCount += offspringAsexualCount;
        _eaStats.TotalOffspringRecombinationCount += offspringRecombinationCount;
        _eaStats.TotalOffspringInterspeciesCount += offspringInterspeciesCount;

        // Update species allocation sizes.
        SpeciesAllocationCalcs<TScalar>.UpdateSpeciesAllocationSizes(_pop, _eaSettingsCurrent, _rng);
    }

    private void UpdateComplexityRegulationMode()
    {
        // Update complexity regulation mode.
        ComplexityRegulationMode modePrev = _complexityRegulationStrategy.CurrentMode;
        ComplexityRegulationMode mode = _complexityRegulationStrategy.UpdateMode(_eaStats, _pop.Stats);

        // If the mode has not changed then do nothing
        if(modePrev == mode)
            return;

        // Notify all objects that need to be notified of the change in mode.
        _asexualReproduction.NotifyComplexityRegulationMode(mode);

        _eaSettingsCurrent = mode switch
        {
            ComplexityRegulationMode.Complexifying => _eaSettingsComplexifying,
            ComplexityRegulationMode.Simplifying => _eaSettingsSimplifying,
            _ => throw new ArgumentException("Unexpected complexity regulation mode."),
        };
    }

    #endregion
}
