// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.EvolutionAlgorithm;

namespace SharpNeat.Evaluation;

/// <summary>
/// An implementation of <see cref="IGenomeListEvaluator{TGenome}"/> that evaluates genomes in parallel on multiple CPU threads.
/// </summary>
/// <typeparam name="TGenome">The genome type that is decoded.</typeparam>
/// <typeparam name="TPhenome">The phenome type that is decoded to and then evaluated.</typeparam>
/// <remarks>
/// Genome decoding to a phenome is performed by a <see cref="IGenomeDecoder{TGenome, TPhenome}"/>.
/// Phenome fitness evaluation is performed by a <see cref="IPhenomeEvaluator{TPhenome}"/>.
///
/// This class is for use with a stateless (and therefore thread safe) phenome evaluator, i.e. one phenome evaluator is created
/// and the is used concurrently by multiple threads.
/// </remarks>
public class ParallelGenomeListEvaluatorStateless<TGenome,TPhenome> : IGenomeListEvaluator<TGenome>
    where TGenome : IGenome
    where TPhenome : class, IDisposable
{
    #region Instance Fields

    readonly IGenomeDecoder<TGenome,TPhenome> _genomeDecoder;
    readonly IPhenomeEvaluationScheme<TPhenome> _phenomeEvaluationScheme;
    readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
    readonly ParallelOptions _parallelOptions;

    #endregion

    #region Constructor

    /// <summary>
    /// Construct with the provided genome decoder and phenome evaluator.
    /// </summary>
    /// <param name="genomeDecoder">Genome decoder.</param>
    /// <param name="phenomeEvaluationScheme">Phenome evaluation scheme.</param>
    /// <param name="degreeOfParallelism">The desired degree of parallelism.</param>
    public ParallelGenomeListEvaluatorStateless(
        IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
        IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme,
        int degreeOfParallelism)
    {
        // This class can only accept an evaluation scheme that uses a stateless evaluator.
        if(phenomeEvaluationScheme.EvaluatorsHaveState) throw new ArgumentException("Evaluator must not have state.", nameof(phenomeEvaluationScheme));

        // Reject degreeOfParallelism values less than 2. -1 should have been resolved to an actual number by the time
        // this constructor is invoked, and 1 is nonsensical for a parallel evaluator.
        if(degreeOfParallelism < 2) throw new ArgumentException("Must be 2 or above.", nameof(degreeOfParallelism));

        _genomeDecoder = genomeDecoder;
        _phenomeEvaluationScheme = phenomeEvaluationScheme;
        _phenomeEvaluator = phenomeEvaluationScheme.CreateEvaluator();
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = degreeOfParallelism
        };
    }

    #endregion

    #region IGenomeListEvaluator

    /// <summary>
    /// Indicates if the evaluation scheme is deterministic, i.e. will always return the same fitness score for a given genome.
    /// </summary>
    /// <remarks>
    /// An evaluation scheme that has some random/stochastic characteristics may give a different fitness score at each invocation
    /// for the same genome, such a scheme is non-deterministic.
    /// </remarks>
    public bool IsDeterministic => _phenomeEvaluationScheme.IsDeterministic;

    /// <summary>
    /// The evaluation scheme's fitness comparer.
    /// </summary>
    /// <remarks>
    /// Typically there is a single fitness score and a higher score is considered better/fitter. However, if there are multiple
    /// fitness values assigned to a genome (e.g. where multiple measures of fitness are in use) then we need a task specific
    /// comparer to determine the relative fitness between two instances of <see cref="FitnessInfo"/>.
    /// </remarks>
    public IComparer<FitnessInfo> FitnessComparer => _phenomeEvaluationScheme.FitnessComparer;

    /// <summary>
    /// Evaluates a list of genomes, assigning fitness info to each.
    /// </summary>
    /// <param name="genomeList">The list of genomes to evaluate.</param>
    public void Evaluate(IList<TGenome> genomeList)
    {
        // Decode and evaluate genomes in parallel.
        Parallel.ForEach(
            genomeList,
            _parallelOptions,
            (genome) =>
            {
                using TPhenome phenome = _genomeDecoder.Decode(genome);
                if(phenome is null)
                {   // Non-viable genome.
                    genome.FitnessInfo = _phenomeEvaluationScheme.NullFitness;
                }
                else
                {
                    genome.FitnessInfo = _phenomeEvaluator.Evaluate(phenome);
                }
            });
    }

    /// <summary>
    /// Accepts a <see cref="FitnessInfo"/>, which is intended to be from the fittest genome in the population, and returns a boolean
    /// that indicates if the evolution algorithm can stop, i.e. because the fitness is the best that can be achieved (or good enough).
    /// </summary>
    /// <param name="fitnessInfo">The fitness info object to test.</param>
    /// <returns>Returns true if the fitness is good enough to signal the evolution algorithm to stop.</returns>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return _phenomeEvaluationScheme.TestForStopCondition(fitnessInfo);
    }

    #endregion
}
