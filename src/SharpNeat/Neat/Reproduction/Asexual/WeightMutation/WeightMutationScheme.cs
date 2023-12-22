// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Numerics.Distributions;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

/// <summary>
/// Connection weight mutation scheme.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
public sealed class WeightMutationScheme<TWeight>
    where TWeight : struct
{
    readonly DiscreteDistribution<double> _strategySelectionDist;
    readonly IWeightMutationStrategy<TWeight>[] _mutationStrategyArr;

    /// <summary>
    /// Construct a new instance with the given strategy arguments.
    /// </summary>
    /// <param name="strategyProbabilityArr">An array of strategy selection probabilities.</param>
    /// <param name="mutationStrategyArr">An array of weight mutation strategies.</param>
    public WeightMutationScheme(
        double[] strategyProbabilityArr,
        IWeightMutationStrategy<TWeight>[] mutationStrategyArr)
    {
        _strategySelectionDist = new DiscreteDistribution<double>(strategyProbabilityArr);
        _mutationStrategyArr = mutationStrategyArr;
    }

    /// <summary>
    /// Mutate the connection weights based on a stochastically chosen <see cref="IWeightMutationStrategy{T}"/>.
    /// </summary>
    /// <param name="weightArr">The connection weight array to apply mutations to.</param>
    /// <param name="rng">Random source.</param>
    public void MutateWeights(TWeight[] weightArr, IRandomSource rng)
    {
        // Select a mutation strategy, and apply it to the array of connection genes.
        int strategyIdx = _strategySelectionDist.Sample(rng);
        var strategy = _mutationStrategyArr[strategyIdx];
        strategy.Invoke(weightArr, rng);
    }
}
