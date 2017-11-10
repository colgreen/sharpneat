using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

namespace SharpNeat.Neat.Reproduction.Asexual.Strategy
{
    public class MutateWeightsReproductionStrategy<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        NeatPopulation<T> _pop;
        WeightMutationScheme<T> _weightMutationScheme;

        #region Constructor

        public MutateWeightsReproductionStrategy(
            NeatPopulation<T> pop,
            WeightMutationScheme<T> weightMutationScheme)
        {
            _pop = pop;
            _weightMutationScheme = weightMutationScheme;

        }

        #endregion

        #region Public Methods

        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent)
        {
            // Clone the parent's connection genes.
            var connArr = ConnectionGene<T>.CloneArray(parent.ConnectionGeneArray);

            // Apply mutation to the connection genes.
            _weightMutationScheme.MutateWeights(connArr);

            // Create and return a new genome.
            return new NeatGenome<T>(
                _pop.MetaNeatGenome,
                _pop.GenomeIdSeq.Next(), 
                _pop.CurrentGenerationAge,
                connArr);
        }

        #endregion
    }
}
