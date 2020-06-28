/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2020 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    /// <summary>
    /// Strategy for selecting a sub-set of items from a superset.
    /// </summary>
    public interface ISubsetSelectionStrategy
    {
        /// <summary>
        /// Select a subset of items from a superset of a given size.
        /// </summary>
        /// <param name="supersetCount">The size of the superset to select from.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>An array of indexes that are the selected items.</returns>
        int[] SelectSubset(int supersetCount, IRandomSource rng);
    }
}
