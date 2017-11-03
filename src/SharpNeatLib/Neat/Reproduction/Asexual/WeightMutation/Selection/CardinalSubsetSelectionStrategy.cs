using System;
using Redzen.Numerics;
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    /// <summary>
    /// Strategy for selecting a sub-set of items from a superset. 
    /// The number of items to select is a fixed number (the selection cardinality), unless the superset is smaller
    /// in which case all items in the superset are selected.
    /// </summary>
    public class CardinalSubsetSelectionStrategy : ISubsetSelectionStrategy
    {
        readonly int _selectionCount;
        readonly IRandomSource _rng;

        #region Constructor

        /// <summary>
        /// Construct with the given selection count (selection cardinality).
        /// </summary>
        /// <param name="selectionCount">The number of items to select.</param>
        public CardinalSubsetSelectionStrategy(int selectionCount)
        {
            _selectionCount = selectionCount;
            _rng = RandomSourceFactory.Create();
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Select a subset of items from a superset of a given size.
        /// </summary>
        /// <param name="supersetCount">The size of the superset to select from.</param>
        /// <returns>An array of indexes that are the selected items.</returns>
        public int[] SelectSubset(int supersetCount)
        {
            int selectionCount = Math.Min(_selectionCount, supersetCount);
            int[] idxArr = new int[selectionCount];
            DiscreteDistributionUtils.SampleUniformWithoutReplacement(supersetCount, idxArr, _rng);            
            return idxArr;
        }

        #endregion
    }
}
