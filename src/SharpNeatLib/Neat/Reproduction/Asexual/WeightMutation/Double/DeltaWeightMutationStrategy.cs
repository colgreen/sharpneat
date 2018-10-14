using Redzen.Random;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Double
{
    /// <summary>
    /// A connection weight mutation strategy that applies deltas to existing weights.
    /// </summary>
    public class DeltaWeightMutationStrategy: IWeightMutationStrategy<double>
    {
        readonly ISubsetSelectionStrategy _selectionStrategy;
        readonly IContinuousDistribution<double> _dist;

        #region Constructor

        public DeltaWeightMutationStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            IContinuousDistribution<double> weightDeltaDistribution)
        {
            _selectionStrategy = selectionStrategy;
            _dist = weightDeltaDistribution;
        }

        #endregion

        #region Public Methods

        public void Invoke(double[] weightArr)
        {
            // Select a subset of connection genes to mutate.
            int[] selectedIdxArr = _selectionStrategy.SelectSubset(weightArr.Length);

            // Loop over the connection genes to be mutated, and mutate them.
            for(int i=0; i<selectedIdxArr.Length; i++) {
                weightArr[selectedIdxArr[i]] += _dist.Sample();
            }
        }

        #endregion

        #region Public Static Methods

        public static DeltaWeightMutationStrategy CreateUniformDeltaStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double weightScale,
            IRandomSource rng)
        {
            var dist = ContinuousDistributionFactory.CreateUniformDistribution<double>(weightScale, true, rng);
            return new DeltaWeightMutationStrategy(selectionStrategy, dist);
        }

        // TODO: Consider Laplacian distribution.
        public static DeltaWeightMutationStrategy CreateGaussianDeltaStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double stdDev,
            IRandomSource rng)
        {
            var dist = ContinuousDistributionFactory.CreateGaussianDistribution<double>(0, stdDev, rng);
            return new DeltaWeightMutationStrategy(selectionStrategy, dist);
        }

        #endregion
    }
}
