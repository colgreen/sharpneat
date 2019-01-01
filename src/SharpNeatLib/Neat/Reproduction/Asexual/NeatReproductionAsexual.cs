using System;
using Redzen.Numerics.Distributions;
using Redzen.Random;
using Redzen.Structures;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.Strategy;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

namespace SharpNeat.Neat.Reproduction.Asexual
{
    /// <summary>
    /// Creation of offspring given a single parent (asexual reproduction).
    /// </summary>
    public class NeatReproductionAsexual<T> : IAsexualReproductionStrategy<T>
        where T : struct
    {
        #region Instance Fields

        readonly NeatReproductionAsexualSettings _settings;

        // Asexual reproduction strategies..
        readonly IAsexualReproductionStrategy<T> _mutateWeightsStrategy;
        readonly IAsexualReproductionStrategy<T> _deleteConnectionStrategy;
        readonly IAsexualReproductionStrategy<T> _addConnectionStrategy;
        readonly IAsexualReproductionStrategy<T> _addNodeStrategy;

        #endregion

        #region Constructor

        public NeatReproductionAsexual(
            MetaNeatGenome<T> metaNeatGenome,
            INeatGenomeBuilder<T> genomeBuilder,
            Int32Sequence genomeIdSeq,
            Int32Sequence innovationIdSeq,
            Int32Sequence generationSeq,
            AddedNodeBuffer addedNodeBuffer,
            NeatReproductionAsexualSettings settings,
            WeightMutationScheme<T> weightMutationScheme)
        {
            _settings = settings;

            // Instantiate reproduction strategies.
            _mutateWeightsStrategy = new MutateWeightsStrategy<T>(metaNeatGenome, genomeBuilder, genomeIdSeq, generationSeq, weightMutationScheme);
            _deleteConnectionStrategy = new DeleteConnectionStrategy<T>(metaNeatGenome, genomeBuilder, genomeIdSeq, generationSeq);

            // Add connection mutation; select acyclic/cyclic strategy as appropriate.
            if(metaNeatGenome.IsAcyclic) 
            {
                _addConnectionStrategy = new AddAcyclicConnectionStrategy<T>(
                    metaNeatGenome, genomeBuilder,
                    genomeIdSeq, innovationIdSeq, generationSeq);
            }
            else 
            {
                _addConnectionStrategy = new AddCyclicConnectionStrategy<T>(
                    metaNeatGenome, genomeBuilder,
                    genomeIdSeq, innovationIdSeq, generationSeq);
            }      
            
            _addNodeStrategy = new AddNodeStrategy<T>(metaNeatGenome, genomeBuilder, genomeIdSeq, innovationIdSeq, generationSeq, addedNodeBuffer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new child genome from a given parent genome.
        /// </summary>
        /// <param name="parent">The parent genome.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new child genome.</returns>
        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent, IRandomSource rng)
        {
            // Get a discrete distribution over the set of possible mutation types.
            DiscreteDistribution mutationTypeDist = GetMutationTypeDistribution(parent);

            // Keep trying until a child genome is created.
            for(;;)
            {
                NeatGenome<T> childGenome = Create(parent, rng, ref mutationTypeDist);
                if(null != childGenome) {
                    return childGenome;
                }
            }
        }
        
        #endregion

        #region Private Methods [Create Subroutines]

        private NeatGenome<T> Create(
            NeatGenome<T> parent,
            IRandomSource rng,
            ref DiscreteDistribution mutationTypeDist)
        {
            // Determine the type of mutation to attempt.
            MutationType mutationTypeId = (MutationType)DiscreteDistribution.Sample(rng, mutationTypeDist);

            // Attempt to create a child genome using the selected mutation type.
            NeatGenome<T> childGenome = null;

            switch(mutationTypeId)
            {
                // Note. These subroutines will return null if they cannot produce a child genome, 
                // e.g. 'delete connection' will not succeed if there is only one connection.
                case MutationType.ConnectionWeight: 
                    childGenome = _mutateWeightsStrategy.CreateChildGenome(parent, rng);
                    break;
                case MutationType.AddNode: 
                    // FIXME: Reinstate.
                    childGenome = _addNodeStrategy.CreateChildGenome(parent, rng);
                    break;
                case MutationType.AddConnection:
                    childGenome = _addConnectionStrategy.CreateChildGenome(parent, rng);
                    break;
                case MutationType.DeleteConnection:
                    childGenome = _deleteConnectionStrategy.CreateChildGenome(parent, rng);
                    break;
                default: 
                    throw new Exception($"Unexpected mutationTypeId [{mutationTypeId}].");
            }

            if(null != childGenome) {
                return childGenome;
            }

            // The chosen mutation type was not possible; remove that type from the set of possible types.
            mutationTypeDist = mutationTypeDist.RemoveOutcome((int)mutationTypeId);

            // Sanity test.
            if(0 == mutationTypeDist.Probabilities.Length)
            {   // This shouldn't be possible, hence this is an exceptional circumstance.
                // Note. Connection weight and 'add node' mutations should always be possible, because there should 
                // always be at least one connection.
                throw new Exception("All types of genome mutation failed.");
            }
            return null;
        }

        #endregion

        #region Private Methods 

        private DiscreteDistribution GetMutationTypeDistribution(NeatGenome<T> parent)
        {
            // If there is only one connection then avoid destructive mutations to avoid the 
            // creation of genomes with no connections.
            DiscreteDistribution dist = (parent.ConnectionGenes.Length < 2) ?
                  _settings.MutationTypeDistributionNonDestructive
                : _settings.MutationTypeDistribution;

            return dist;
        }

        #endregion
    }
}
