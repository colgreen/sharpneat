// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.CartPole.SinglePole;

/// <summary>
/// Evaluation scheme for the cart and pole balancing task, with a single pole.
/// </summary>
public sealed class CartSinglePoleEvaluationScheme : IBlackBoxEvaluationScheme<double>
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
    public bool EvaluatorsHaveState => true;

    /// <inheritdoc/>
    public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
    {
        return new CartSinglePoleEvaluator();
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 100.0);
    }
}
