// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Evaluation;

/// <summary>
/// An implementation of <see cref="IGenomeBatchEvaluator{TGenome}"/> that evaluates genomes in parallel on multiple CPU threads.
/// </summary>
/// <typeparam name="TGenome">The genome type that is decoded.</typeparam>
/// <typeparam name="TPhenome">The phenome type that is decoded to and then evaluated.</typeparam>
/// <remarks>
/// Genome decoding to a phenome is performed by a <see cref="IGenomeDecoder{TGenome, TPhenome}"/>.
/// Phenome fitness evaluation is performed by a <see cref="IPhenomeEvaluator{TPhenome}"/>.
///
/// This class is for use with a stateful (and therefore thread unsafe) phenome evaluators, i.e., a pool of phenome evaluators
/// is maintained to ensure each CPU core/thread can rent its own phenome evaluator.
/// </remarks>
public class ParallelGenomeBatchEvaluator<TGenome,TPhenome> : IGenomeBatchEvaluator<TGenome>
    where TGenome : IGenome
    where TPhenome : class, IDisposable
{
    readonly IGenomeDecoder<TGenome,TPhenome> _genomeDecoder;
    readonly IPhenomeEvaluationScheme<TPhenome> _phenomeEvaluationScheme;
    readonly ParallelOptions _parallelOptions;
    readonly PhenomeEvaluatorPool<TPhenome> _evaluatorPool;

    #region Constructor

    /// <summary>
    /// Construct with the provided genome decoder and phenome evaluator.
    /// </summary>
    /// <param name="genomeDecoder">Genome decoder.</param>
    /// <param name="phenomeEvaluationScheme">Phenome evaluation scheme.</param>
    /// <param name="degreeOfParallelism">The desired degree of parallelism.</param>
    public ParallelGenomeBatchEvaluator(
        IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
        IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme,
        int degreeOfParallelism)
    {
        // This class should only be used with evaluation schemes that use evaluators with state,
        // otherwise ParallelGenomeBatchEvaluatorStateless should be used.
        if(!phenomeEvaluationScheme.EvaluatorsHaveState) throw new ArgumentException("Evaluation scheme must have state.", nameof(phenomeEvaluationScheme));

        // Reject degreeOfParallelism values less than 2. -1 should have been resolved to an actual number by the time
        // this constructor is invoked, and 1 is nonsensical for a parallel evaluator.
        if(degreeOfParallelism < 2) throw new ArgumentException("Must be 2 or above.", nameof(degreeOfParallelism));

        _genomeDecoder = genomeDecoder;
        _phenomeEvaluationScheme = phenomeEvaluationScheme;
        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = degreeOfParallelism
        };

        // Create a pool of phenome evaluators.
        // Note. the pool is initialised with a number of pre-constructed evaluators that matches
        // degreeOfParallelism. We don't expect the pool to be asked for more than this number of
        // evaluators at any given point in time.
        _evaluatorPool = new PhenomeEvaluatorPool<TPhenome>(
            phenomeEvaluationScheme,
            degreeOfParallelism);
    }

    #endregion

    #region IGenomeBatchEvaluator

    /// <inheritdoc/>
    public bool IsDeterministic => _phenomeEvaluationScheme.IsDeterministic;

    /// <inheritdoc/>
    public IComparer<FitnessInfo> FitnessComparer => _phenomeEvaluationScheme.FitnessComparer;

    /// <inheritdoc/>
    public void Evaluate(IList<TGenome> genomes)
    {
        // Decode and evaluate genomes in parallel.
        // Notes.
        // This overload of Parallel.ForEach accepts a factory function for obtaining an object that represents some state
        // that can re-used within a partition, here we return a phenome evaluator as that partition state object.
        //
        // Here a partition is a group genomes from genomeList that will be evaluated by a single thread, i.e.
        // partitions may be executed in parallel, but genomes within a partition are evaluated sequentially and
        // therefore require only one phenome evaluator between them, we just need to ensure evaluator state is reset between
        // evaluations.
        Parallel.ForEach(
            genomes,
            _parallelOptions,
            () => _evaluatorPool.GetEvaluator(),    // Get a phenome evaluator from the pool to use for the current partition.
            (genome, loopState, evaluator) =>       // Evaluate a single genome.
            {
                using TPhenome phenome = _genomeDecoder.Decode(genome);
                if(phenome is null)
                {   // Non-viable genome.
                    genome.FitnessInfo = _phenomeEvaluationScheme.NullFitness;
                }
                else
                {
                    genome.FitnessInfo = evaluator.Evaluate(phenome);
                }

                // The {evaluator} param for the next call of this anonymous method comes from what we return here,
                // this is useful for struct based partition state, but here we're just passing an object ref around.
                return evaluator;
            },
            (evaluator) => _evaluatorPool.ReleaseEvaluator(evaluator)); // Release this partition's phenome evaluator back into pool.
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return _phenomeEvaluationScheme.TestForStopCondition(fitnessInfo);
    }

    #endregion
}
