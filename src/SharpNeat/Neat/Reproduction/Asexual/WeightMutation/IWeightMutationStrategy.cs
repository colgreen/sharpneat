// This file is part of SharpNEAT; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

/// <summary>
/// A connection weight mutation strategy.
/// </summary>
/// <typeparam name="TWeight">Connection weight data type.</typeparam>
public interface IWeightMutationStrategy<TWeight>
    where TWeight : unmanaged
{
    /// <summary>
    /// Invoke the strategy.
    /// </summary>
    /// <param name="weightArr">The connection weight array to apply mutations to.</param>
    /// <param name="rng">Random source.</param>
    void Invoke(TWeight[] weightArr, IRandomSource rng);
}
