using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation.Selection;

namespace SharpNeat.Neat.Reproduction.Asexual.WeightMutation
{
    /// <summary>
    /// Strategy for mutating the weights on an array of connection genes.
    /// The strategy consists of two sub-strategies, one for selecting which connection genes to mutate,
    /// and the other that applies weight mutation to the selected genes.
    /// </summary>
    /// <typeparam name="T">Connection weight type.</typeparam>
    public class ConnectionArrayMutationStrategy<T> : IConnectionArrayMutationStrategy<T>
        where T : struct
    {
        readonly ISubsetSelectionStrategy _selectionStrategy;
        readonly IWeightMutationStrategy<T> _weightMutationStrategy;

        #region Constructor

        public ConnectionArrayMutationStrategy(
            ISubsetSelectionStrategy selectionStrategy,
            IWeightMutationStrategy<T> weightMutationStrategy)
        {
            _selectionStrategy = selectionStrategy;
            _weightMutationStrategy = weightMutationStrategy;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invoke the strategy.
        /// </summary>
        /// <param name="connArr">Connection gene array.</param>
        public void Invoke(ConnectionGene<T>[] connArr)
        {
            // Select a subset of connection genes to mutate.
            int[] selectedIdxArr = _selectionStrategy.SelectSubset(connArr.Length);

            // Loop over the connection genes to be mutated, and mutate them.
            for(int i=0; i<selectedIdxArr.Length; i++) {
                connArr[selectedIdxArr[i]].Weight = _weightMutationStrategy.Invoke(connArr[selectedIdxArr[i]].Weight);
            }
        }

        #endregion
    }
}
