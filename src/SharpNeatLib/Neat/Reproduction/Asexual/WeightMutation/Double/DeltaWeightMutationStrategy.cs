using Redzen.Random;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Double
{
    /// <summary>
    /// A connection weight mutation strategy that applies a delta to the existing weight.
    /// The delta can be from either a uniform or gaussian distribution.
    /// </summary>
    public class DeltaWeightMutationStrategy: IWeightMutationStrategy<double>
    {
        IContinuousDistribution<double> _dist;

        #region Constructor

        public DeltaWeightMutationStrategy(IContinuousDistribution<double> weightDeltaDistribution)
        {
            _dist = weightDeltaDistribution;
        }

        #endregion

        #region Public Methods

        public double Invoke(double weight)
        {
            return weight + _dist.Sample();
        }

        #endregion

        #region Public Static Methods

        public static DeltaWeightMutationStrategy CreateUniformDeltaStrategy(double weightScale)
        {
            var dist = ContinuousDistributionFactory.CreateUniformDistribution<double>(weightScale, true);
            return new DeltaWeightMutationStrategy(dist);
        }

        public static DeltaWeightMutationStrategy CreateGaussianDeltaStrategy(double stdDev)
        {
            var dist = ContinuousDistributionFactory.CreateGaussianDistribution<double>(0, stdDev);
            return new DeltaWeightMutationStrategy(dist);
        }

        #endregion
    }
}
