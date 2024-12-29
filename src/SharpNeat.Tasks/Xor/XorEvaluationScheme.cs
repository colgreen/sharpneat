// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.Xor;

/// <summary>
/// Evaluation scheme for the logical XOR task.
/// </summary>
public sealed class XorEvaluationScheme : IBlackBoxEvaluationScheme<double>
{
    /// <inheritdoc/>
    public int InputCount => 3;

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
        return new XorEvaluator<double>();
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 10);
    }
}
