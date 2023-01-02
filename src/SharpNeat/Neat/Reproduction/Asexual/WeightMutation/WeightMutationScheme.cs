// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Numerics.Distributions.Double;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

/// <summary>
/// Connection weight mutation scheme.
/// </summary>
/// <typeparam name="T">Neural net numeric data type.</typeparam>
public sealed class WeightMutationScheme<T>
    where T : struct
{
    readonly DiscreteDistribution _strategySelectionDist;
    readonly IWeightMutationStrategy<T>[] _mutationStrategyArr;

    /// <summary>
    /// Construct a new instance with the given strategy arguments.
    /// </summary>
    /// <param name="strategyProbabilityArr">An array of strategy selection probabilities.</param>
    /// <param name="mutationStrategyArr">An array of weight mutation strategies.</param>
    public WeightMutationScheme(
        double[] strategyProbabilityArr,
        IWeightMutationStrategy<T>[] mutationStrategyArr)
    {
        _strategySelectionDist = new DiscreteDistribution(strategyProbabilityArr);
        _mutationStrategyArr = mutationStrategyArr;
    }

    /// <summary>
    /// Mutate the connection weights based on a stochastically chosen <see cref="IWeightMutationStrategy{T}"/>.
    /// </summary>
    /// <param name="weightArr">The connection weight array to apply mutations to.</param>
    /// <param name="rng">Random source.</param>
    public void MutateWeights(T[] weightArr, IRandomSource rng)
    {
        // Select a mutation strategy, and apply it to the array of connection genes.
        int strategyIdx = DiscreteDistribution.Sample(rng, _strategySelectionDist);
        var strategy = _mutationStrategyArr[strategyIdx];
        strategy.Invoke(weightArr, rng);
    }
}
