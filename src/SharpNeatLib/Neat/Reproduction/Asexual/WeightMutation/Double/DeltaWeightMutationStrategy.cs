using Redzen.Numerics.Distributions;
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
        readonly IStatelessSampler<double> _weightDeltaSampler;

        #region Constructor

        public DeltaWeightMutationStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            IStatelessSampler<double> weightDeltaSampler)
        {
            _selectionStrategy = selectionStrategy;
            _weightDeltaSampler = weightDeltaSampler;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="weightArr">The connection weight array to apply mutations to.</param>
        /// <param name="rng">Random source.</param>
        public void Invoke(double[] weightArr, IRandomSource rng)
        {
            // Select a subset of connection genes to mutate.
            int[] selectedIdxArr = _selectionStrategy.SelectSubset(weightArr.Length, rng);

            // Loop over the connection genes to be mutated, and mutate them.
            for(int i=0; i<selectedIdxArr.Length; i++) {
                weightArr[selectedIdxArr[i]] += _weightDeltaSampler.Sample(rng);
            }
        }

        #endregion

        #region Public Static Methods

        public static DeltaWeightMutationStrategy CreateUniformDeltaStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double weightScale)
        {
            var sampler = UniformDistributionSamplerFactory.CreateStatelessSampler<double>(weightScale, true);
            return new DeltaWeightMutationStrategy(selectionStrategy, sampler);
        }

        // TODO: Consider Laplacian distribution.
        public static DeltaWeightMutationStrategy CreateGaussianDeltaStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            double stdDev)
        {
            var sampler = GaussianDistributionSamplerFactory.CreateStatelessSampler<double>(0, stdDev);
            return new DeltaWeightMutationStrategy(selectionStrategy, sampler);
        }

        #endregion
    }
}
