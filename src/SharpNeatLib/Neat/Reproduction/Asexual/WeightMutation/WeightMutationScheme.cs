using Redzen.Numerics;
using Redzen.Numerics.Distributions;
using Redzen.Random;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    public class WeightMutationScheme<T> 
        where T : struct
    {
        #region Instance Fields

        DiscreteDistribution _strategySelectionDist;
        IWeightMutationStrategy<T>[] _mutationStrategyArr;

        #endregion

        #region Constructor

        public WeightMutationScheme(
            double[] strategyProbabilityArr,
            IWeightMutationStrategy<T>[] mutationStrategyArr)
        {
            _strategySelectionDist = new DiscreteDistribution(strategyProbabilityArr);
            _mutationStrategyArr = mutationStrategyArr;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mutate the connection weights based on a stochastically chosen IWeightMutationStrategy.
        /// </summary>
        /// <param name="weightArr">The connection weight array to apply mutations to.</param>
        public void MutateWeights(T[] weightArr, IRandomSource rng)
        {
            // Select a mutation strategy, and apply it to the array of connection genes.
            int strategyIdx = DiscreteDistribution.Sample(rng, _strategySelectionDist);
            var strategy = _mutationStrategyArr[strategyIdx];
            strategy.Invoke(weightArr, rng);
        }

        #endregion
    }
}
