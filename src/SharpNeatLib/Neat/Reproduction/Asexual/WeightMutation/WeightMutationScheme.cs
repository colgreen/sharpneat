using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.Genome;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    public class WeightMutationScheme<T> 
        where T : struct
    {
        DiscreteDistribution _strategySelectionDist;
        IConnectionArrayMutationStrategy<T>[] _mutationStrategyArr;
        IRandomSource _rng;

        #region Constructor

        public WeightMutationScheme(
            double[] strategyProbabilityArr,
            IConnectionArrayMutationStrategy<T>[] mutationStrategyArr)
        {
            _strategySelectionDist = new DiscreteDistribution(strategyProbabilityArr);
            _mutationStrategyArr = mutationStrategyArr;
            _rng = RandomSourceFactory.Create();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mutate the connection gene array weights based on a stochastically chosen IConnectionArrayMutationStrategy.
        /// </summary>
        /// <param name="connArr">The connection gene array to apply weight mutation to.</param>
        public void MutateWeights(ConnectionGene<T>[] connArr)
        {
            // Select a mutation strategy, and apply it to the array of connection genes.
            int strategyIdx = _strategySelectionDist.Sample();
            var strategy = _mutationStrategyArr[strategyIdx];
            strategy.Invoke(connArr);
        }

        #endregion
    }
}
