using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// A connection weight mutation strategy that resets connection weights.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public class ResetWeightMutationStrategy<T> : IWeightMutationStrategy<T>
        where T : struct
    {
        readonly ISubsetSelectionStrategy _selectionStrategy;
        readonly IContinuousDistribution<T> _dist;

        #region Constructor

        public ResetWeightMutationStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            IContinuousDistribution<T> weightDistribution)
        {
            _selectionStrategy = selectionStrategy;
            _dist = weightDistribution;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="weightArr">The connection weight array to apply mutations to.</param>
        public void Invoke(T[] weightArr)
        {
            // Select a subset of connection genes to mutate.
            int[] selectedIdxArr = _selectionStrategy.SelectSubset(weightArr.Length);

            // Loop over the connection genes to be mutated, and mutate them.
            for(int i=0; i<selectedIdxArr.Length; i++) {
                weightArr[selectedIdxArr[i]] = _dist.Sample();
            }
        }

        #endregion

        #region Public Static Methods

        public static ResetWeightMutationStrategy<T> CreateUniformResetStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double weightScale)
        {
            var dist = ContinuousDistributionFactory.CreateUniformDistribution<T>(weightScale, true);
            return new ResetWeightMutationStrategy<T>(selectionStrategy, dist);
        }

        public static ResetWeightMutationStrategy<T> CreateGaussianResetStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double stdDev)
        {
            var dist = ContinuousDistributionFactory.CreateGaussianDistribution<T>(0, stdDev);
            return new ResetWeightMutationStrategy<T>(selectionStrategy, dist);
        }

        #endregion
    }
}
