using Redzen.Numerics;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;
using System;

namespace SharpNeat.Neat.Reproduction.Asexual
{
    /// <summary>
    /// Creation of offspring given a single parent (asexual reproduction).
    /// </summary>
    public class NeatReproductionAsexual
    {
        NeatReproductionAsexualSettings _settings;
        NeatPopulation<double> _pop;
        IRandomSource _rng;

        //AddNodeMutation _addNodeMutation;

        #region Constructor

        public NeatReproductionAsexual(NeatReproductionAsexualSettings settings, NeatPopulation<double> pop)
        {
            _settings = settings;
            _pop = pop;
            _rng = RandomFactory.Create();
            //_addNodeMutation = new AddNodeMutation(pop, _rng);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asexual reproduction.
        /// </summary>
        /// <param name="parent1">Parent genome.</param>
        public NeatGenome<double> CreateChild(NeatGenome<double> parent)
        {
            // Get a discrete distribution over the set of possible mutation types.
            DiscreteDistribution mutationTypeDist = GetMutationTypeDistribution(parent);

            // Keep trying until a child genome is created.
            for(;;)
            {
                NeatGenome<double> childGenome = CreateChildInner(parent, ref mutationTypeDist);
                if(null != childGenome) {
                    return childGenome;
                }
            }
        }
        
        #endregion

        #region Private Methods [CreateChild Subroutines]

        public NeatGenome<double> CreateChildInner(NeatGenome<double> parent, ref DiscreteDistribution mutationTypeDist)
        {
            // Determine the type of mutation to attempt.
            int mutationTypeId = mutationTypeDist.Sample(_rng);

            // Attempt to create a child genome using the selected mutation type.
            NeatGenome<double> childGenome = null;

            switch(mutationTypeId)
            {
                // Note. These subroutines will return null if they cannot produce a child genome, 
                // e.g. 'delete connection' will not succeed if there is only one connection.
                case 0: 
                    childGenome = CreateChild_WeightMutation(parent);
                    break;
                case 1: 
                    // FIXME: Reinstate.
                    //childGenome = _addNodeMutation.CreateChild(parent);
                    break;
                case 2:
                    childGenome = CreateChild_AddConnectionMutation(parent);
                    break;
                case 3:
                    childGenome = CreateChild_DeleteConnectionMutation(parent);
                    break;
                default: 
                    throw new Exception($"Unexpected mutationTypeId [{mutationTypeId}].");
            }

            if(null == childGenome)
            {
                // The chosen mutation type was not possible; remove that type from the set of possible types.
                mutationTypeDist = mutationTypeDist.RemoveOutcome(mutationTypeId);

                // Sanity test.
                if(0 == mutationTypeDist.Probabilities.Length)
                {   // This shouldn't be possible, hence this is an exceptional circumstance.
                    // Note. Connection weight and 'add node' mutations should always be possible, because there should 
                    // always be at least one connection.
                    throw new Exception("All types of genome mutation failed.");
                }
            }
            return childGenome;
        }

        #endregion

        #region Private Methods [CreateChild_WeightMutation]

        private NeatGenome<double> CreateChild_WeightMutation(NeatGenome<double> parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods [CreateChild_AddConnectionMutation]

        private NeatGenome<double> CreateChild_AddConnectionMutation(NeatGenome<double> parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods [CreateChild_DeleteConnectionMutation]

        private NeatGenome<double> CreateChild_DeleteConnectionMutation(NeatGenome<double> parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods 

        private DiscreteDistribution GetMutationTypeDistribution(NeatGenome<double> parent)
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
