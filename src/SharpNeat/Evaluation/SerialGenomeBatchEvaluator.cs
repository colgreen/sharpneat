// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Evaluation;

/// <summary>
/// An implementation of <see cref="IGenomeBatchEvaluator{TGenome}"/> that evaluates genomes in series on a single CPU thread.
/// </summary>
/// <typeparam name="TGenome">The genome type that is decoded.</typeparam>
/// <typeparam name="TPhenome">The phenome type that is decoded to and then evaluated.</typeparam>
/// <remarks>
/// Single threaded evaluation can be useful in various scenarios e.g. when debugging.
///
/// Genome decoding is performed by a provided IGenomeDecoder.
/// Phenome evaluation is performed by a provided IPhenomeEvaluator.
/// </remarks>
public class SerialGenomeBatchEvaluator<TGenome,TPhenome> : IGenomeBatchEvaluator<TGenome>
    where TGenome : IGenome
    where TPhenome : IDisposable
{
    readonly IGenomeDecoder<TGenome,TPhenome> _genomeDecoder;
    readonly IPhenomeEvaluationScheme<TPhenome> _phenomeEvaluationScheme;
    readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;

    #region Constructor

    /// <summary>
    /// Construct with the provided genome decoder and phenome evaluator.
    /// </summary>
    /// <param name="genomeDecoder">Genome decoder.</param>
    /// <param name="phenomeEvaluationScheme">Phenome evaluation scheme.</param>
    public SerialGenomeBatchEvaluator(
        IGenomeDecoder<TGenome,TPhenome> genomeDecoder,
        IPhenomeEvaluationScheme<TPhenome> phenomeEvaluationScheme)
    {
        _genomeDecoder = genomeDecoder;
        _phenomeEvaluationScheme = phenomeEvaluationScheme;

        // Note. SerialGenomeBatchEvaluator will only evaluate on one thread therefore only ever requires a single evaluator.
        _phenomeEvaluator = phenomeEvaluationScheme.CreateEvaluator();
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
        // Decode and evaluate each genome in turn.
        foreach(TGenome genome in genomes)
        {
            // TODO: Implement phenome caching (to avoid decode cost when re-evaluating with a non-deterministic evaluation scheme).
            using TPhenome phenome = _genomeDecoder.Decode(genome);
            if(phenome is null)
            {   // Non-viable genome.
                genome.FitnessInfo = _phenomeEvaluationScheme.NullFitness;
            }
            else
            {
                genome.FitnessInfo = _phenomeEvaluator.Evaluate(phenome);
            }
        }
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return _phenomeEvaluationScheme.TestForStopCondition(fitnessInfo);
    }

    #endregion
}
