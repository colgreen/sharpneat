/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 *
 * Copyright 2004-2022 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

/// <summary>
/// A connection weight mutation strategy.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public interface IWeightMutationStrategy<T> where T : struct
{
    /// <summary>
    /// Invoke the strategy.
    /// </summary>
    /// <param name="weightArr">The connection weight array to apply mutations to.</param>
    /// <param name="rng">Random source.</param>
    void Invoke(T[] weightArr, IRandomSource rng);
}
