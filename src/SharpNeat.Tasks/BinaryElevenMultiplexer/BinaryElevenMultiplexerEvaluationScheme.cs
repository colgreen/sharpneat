// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using SharpNeat.Evaluation;

namespace SharpNeat.Tasks.BinaryElevenMultiplexer;

/// <summary>
/// Evaluation scheme for the Binary 11-Multiplexer task.
/// </summary>
/// <typeparam name="TScalar">Black box input/output data type.</typeparam>
public sealed class BinaryElevenMultiplexerEvaluationScheme<TScalar> : IBlackBoxEvaluationScheme<TScalar>
    where TScalar : unmanaged, IBinaryFloatingPointIeee754<TScalar>
{
    /// <inheritdoc/>
    public int InputCount => 12;

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
    public IPhenomeEvaluator<IBlackBox<TScalar>> CreateEvaluator()
    {
#pragma warning disable CS8603 // Possible null reference return.
        Type scalarType = typeof(TScalar);

        if(scalarType == typeof(double))
        {
            return new BinaryElevenMultiplexerEvaluatorDouble() as IPhenomeEvaluator<IBlackBox<TScalar>>;
        }
        else if(scalarType == typeof(float))
        {
            return new BinaryElevenMultiplexerEvaluatorFloat() as IPhenomeEvaluator<IBlackBox<TScalar>>;
        }
        else
        {
            throw new InvalidOperationException($"Unsupported scalar type '{scalarType}'.");
        }
#pragma warning restore CS8603 // Possible null reference return.
    }

    /// <inheritdoc/>
    public bool TestForStopCondition(FitnessInfo fitnessInfo)
    {
        return (fitnessInfo.PrimaryFitness >= 2048);
    }
}
