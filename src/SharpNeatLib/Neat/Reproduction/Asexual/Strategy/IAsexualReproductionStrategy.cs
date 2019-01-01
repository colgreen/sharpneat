/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2019 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using Redzen.Random;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public interface IAsexualReproductionStrategy<T> where T : struct
    {
        /// <summary>
        /// Create a new child genome from a given parent genome.
        /// </summary>
        /// <param name="parent">The parent genome.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new child genome.</returns>
        NeatGenome<T> CreateChildGenome(NeatGenome<T> parent, IRandomSource rng);
    }
}
