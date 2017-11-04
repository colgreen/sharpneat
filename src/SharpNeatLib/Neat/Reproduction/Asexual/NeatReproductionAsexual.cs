using System;
using Redzen.Numerics;
using Redzen.Random;
using SharpNeat.Neat.Genome;
using SharpNeat.Neat.Reproduction.Asexual.WeightMutation;

namespace SharpNeat.Neat.Reproduction.Asexual
{
    /// <summary>
    /// Creation of offspring given a single parent (asexual reproduction).
    /// </summary>
    public class NeatReproductionAsexual<T> where T : struct
    {
        #region Instance Fields

        NeatPopulation<T> _pop;
        NeatReproductionAsexualSettings _settings;
        WeightMutationScheme<T> _weightMutationScheme;

        #endregion

        #region Constructor

        public NeatReproductionAsexual(
            NeatPopulation<T> pop,
            NeatReproductionAsexualSettings settings,
            WeightMutationScheme<T> weightMutationScheme)
        {
            _pop = pop;
            _settings = settings;
            _weightMutationScheme = weightMutationScheme;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asexual reproduction.
        /// </summary>
        /// <param name="parent1">Parent genome.</param>
        public NeatGenome<T> CreateChildGenome(NeatGenome<T> parent)
        {
            // Get a discrete distribution over the set of possible mutation types.
            DiscreteDistribution mutationTypeDist = GetMutationTypeDistribution(parent);

            // Keep trying until a child genome is created.
            for(;;)
            {
                NeatGenome<T> childGenome = Create(parent, ref mutationTypeDist);
                if(null != childGenome) {
                    return childGenome;
                }
            }
        }
        
        #endregion

        #region Private Methods [Create Subroutines]

        public NeatGenome<T> Create(NeatGenome<T> parent, ref DiscreteDistribution mutationTypeDist)
        {
            // Determine the type of mutation to attempt.
            MutationType mutationTypeId = (MutationType)mutationTypeDist.Sample();

            // Attempt to create a child genome using the selected mutation type.
            NeatGenome<T> childGenome = null;

            switch(mutationTypeId)
            {
                // Note. These subroutines will return null if they cannot produce a child genome, 
                // e.g. 'delete connection' will not succeed if there is only one connection.
                case MutationType.ConnectionWeight: 
                    childGenome = Create_WeightMutation(parent);
                    break;
                case MutationType.AddNode: 
                    // FIXME: Reinstate.
                    //childGenome = _addNodeMutation.CreateChild(parent);
                    break;
                case MutationType.AddConnection:
                    childGenome = Create_AddConnectionMutation(parent);
                    break;
                case MutationType.DeleteConnection:
                    childGenome = Create_DeleteConnectionMutation(parent);
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

        #region Private Methods [CreateChild_WeightMutation]

        private NeatGenome<T> Create_WeightMutation(NeatGenome<T> parent)
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

        #region Private Methods [CreateChild_AddConnectionMutation]

        private NeatGenome<T> Create_AddConnectionMutation(NeatGenome<T> parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods [CreateChild_DeleteConnectionMutation]

        private NeatGenome<T> Create_DeleteConnectionMutation(NeatGenome<T> parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods 

        private DiscreteDistribution GetMutationTypeDistribution(NeatGenome<T> parent)
        {
            // If there is only one connection then avoid destructive mutations to avoid the 
            // creation of genomes with no connections.
            DiscreteDistribution dist = (parent.ConnectionGeneArray.Length < 2) ?
                  _settings.MutationTypeDistributionNonDestructive
                : _settings.MutationTypeDistribution;

            return dist;
        }

        #endregion
    }
}
