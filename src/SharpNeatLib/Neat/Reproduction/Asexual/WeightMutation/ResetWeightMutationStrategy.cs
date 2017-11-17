using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// A connection weight mutation strategy that resets the connection weight.
    /// The new weight can be from either a uniform or gaussian distribution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResetWeightMutationStrategy<T> : IWeightMutationStrategy<T>
        where T : struct
    {
        IContinuousDistribution<T> _dist;

        #region Constructor

        public ResetWeightMutationStrategy(IContinuousDistribution<T> weightDistribution)
        {
            _dist = weightDistribution;
        }

        #endregion

        #region Public Methods

        public T Invoke(T weight)
        {
            return _dist.Sample();
        }

        #endregion

        #region Public Static Methods

        public static ResetWeightMutationStrategy<T> CreateUniformResetStrategy(double weightScale)
        {
            var dist = ContinuousDistributionFactory.CreateUniformDistribution<T>(weightScale, true);
            return new ResetWeightMutationStrategy<T>(dist);
        }

        public static ResetWeightMutationStrategy<T> CreateGaussianResetStrategy(double stdDev)
        {
            var dist = ContinuousDistributionFactory.CreateGaussianDistribution<T>(0, stdDev);
            return new ResetWeightMutationStrategy<T>(dist);
        }

        #endregion
    }
}
