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
using System.Diagnostics;
using Redzen.Numerics;
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    /// <summary>
    /// Strategy for selecting a sub-set of items from a superset.
    /// The number of items to select is a fixed proportion of the superset size.
    /// </summary>
    public sealed class ProportionSubsetSelectionStrategy : ISubsetSelectionStrategy
    {
        readonly double _selectionProportion;

        #region Constructor

        /// <summary>
        /// Construct with the given selection proportion.
        /// </summary>
        /// <param name="selectionProportion">The proportion of items to select.</param>
        public ProportionSubsetSelectionStrategy(double selectionProportion)
        {
            Debug.Assert(selectionProportion > 0 && selectionProportion <= 1.0);

            _selectionProportion = selectionProportion;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Select a subset of items from a superset of a given size.
        /// </summary>
        /// <param name="supersetCount">The size of the superset to select from.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>An array of indexes that are the selected items.</returns>
        public int[] SelectSubset(int supersetCount, IRandomSource rng)
        {
            // Note. Ideally we'd return a sorted list of indexes to improve performance of the code that consumes them,
            // however, the sampling process inherently produces samples in randomized order, thus the decision of whether
            // to sort or not depends on the cost to the code using the samples. I.e. don't sort here!
            int selectionCount = (int)NumericsUtils.StochasticRound(supersetCount * _selectionProportion, rng);
            int[] idxArr = new int[selectionCount];
            DiscreteDistribution.SampleUniformWithoutReplacement(rng, supersetCount, idxArr);
            return idxArr;
        }

        #endregion
    }
}
