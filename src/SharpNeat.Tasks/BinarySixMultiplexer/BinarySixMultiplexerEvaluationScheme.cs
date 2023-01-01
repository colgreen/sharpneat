// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.BinarySixMultiplexer;

/// <summary>
/// Evaluation scheme for the Binary 6-Multiplexer task.
/// </summary>
public sealed class BinarySixMultiplexerEvaluationScheme : IBlackBoxEvaluationScheme<double>
{
    /// <inheritdoc/>
    public int InputCount => 7;

    /// <inheritdoc/>
    public int OutputCount => 1;

    /// <inheritdoc/>
    public bool IsDeterministic => true;

    /// <inheritdoc/>
    public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

    /// <inheritdoc/>
    public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

    /// <inheritdoc/>
    public bool EvaluatorsHaveState => false;

    /// <inheritdoc/>
    public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
    {
        return new BinarySixMultiplexerEvaluator();
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 1_000);
    }
}
