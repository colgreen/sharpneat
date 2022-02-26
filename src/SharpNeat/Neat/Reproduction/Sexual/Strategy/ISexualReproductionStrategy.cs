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
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Sexual.Strategy;

/// <summary>
/// Represents a NEAT genome sexual reproduction strategy.
/// </summary>
/// <typeparam name="T">Connection weight data type.</typeparam>
public interface ISexualReproductionStrategy<T> where T : struct
{
    /// <summary>
    /// Create a new child genome based on the genetic content of two parent genome.
    /// </summary>
    /// <param name="parent1">Parent 1.</param>
    /// <param name="parent2">Parent 2.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new child genome.</returns>
    NeatGenome<T> CreateGenome(NeatGenome<T> parent1, NeatGenome<T> parent2, IRandomSource rng);
}
