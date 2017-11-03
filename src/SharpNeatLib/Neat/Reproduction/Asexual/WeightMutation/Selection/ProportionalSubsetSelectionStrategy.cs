using System.Diagnostics;
using Redzen.Numerics;
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection
{
    /// <summary>
    /// Strategy for selecting a sub-set of items from a superset. 
    /// The number of items to select is a fixed proportion of the superset size.
    /// </summary>
    public class ProportionSubsetSelectionStrategy : ISubsetSelectionStrategy
    {
        readonly double _selectionProportion;
        readonly IRandomSource _rng;

        #region Constructor

        /// <summary>
        /// Construct with the given selection proportion.
        /// </summary>
        /// <param name="selectionCount">The proportion of items to select.</param>
        public ProportionSubsetSelectionStrategy(double selectionProportion)
        {
            Debug.Assert(selectionProportion > 0 && selectionProportion <= 1.0);

            _selectionProportion = selectionProportion;
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
            int selectionCount = (int)NumericsUtils.ProbabilisticRound(supersetCount * _selectionProportion, _rng);
            int[] idxArr = new int[selectionCount];
            DiscreteDistributionUtils.SampleUniformWithoutReplacement(supersetCount, idxArr, _rng);
            return idxArr;
        }

        #endregion
    }
}
