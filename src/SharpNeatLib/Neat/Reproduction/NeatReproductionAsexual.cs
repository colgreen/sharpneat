using Redzen.Numerics;
using SharpNeat.Neat.Genome;
using SharpNeat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Neat.Reproduction
{
    /// <summary>
    /// Creation of offspring given a single parent (asexual reproduction).
    /// </summary>
    public class NeatReproductionAsexual
    {
        NeatReproductionAsexualSettings _settings;
        NeatPopulation _pop;
        IRandomSource _rng;

        #region Constructor

        public NeatReproductionAsexual(NeatReproductionAsexualSettings settings, NeatPopulation pop)
        {
            _settings = settings;
            _pop = pop;
            _rng = RandomFactory.Create();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asexual reproduction.
        /// </summary>
        /// <param name="parent1">Parent genome.</param>
        public NeatGenome CreateChild(NeatGenome parent)
        {
            // Get a discrete distribution over the set of possible mutation types.
            DiscreteDistribution mutationTypeDist = GetMutationTypeDistribution(parent);

            // Keep trying until a child genome is created.
            for(;;)
            {
                NeatGenome childGenome = CreateChildInner(parent, mutationTypeDist);
                if(null != childGenome) {
                    return childGenome;
                }
            }
        }
        
        #endregion

        #region Private Methods [CreateChild Subroutines]

        public NeatGenome CreateChildInner(NeatGenome parent, DiscreteDistribution mutationTypeDist)
        {
            // Determine the type of mutation to attempt.
            int mutationTypeId = DiscreteDistributionUtils.Sample(mutationTypeDist, _rng);

            // Attempt to create a child genome using the selected mutation type.
            NeatGenome childGenome = null;

            switch(mutationTypeId)
            {
                // Note. These subroutines will return null if they cannot produce a child genome, 
                // e.g. 'delete connection' will not succeed if there is only one connection.
                case 0: 
                    childGenome = CreateChild_WeightMutation(parent);
                    break;
                case 1: 
                    childGenome = CreateChild_AddNodeMutation(parent);
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
                mutationTypeDist.RemoveOutcome(mutationTypeId);

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




        private NeatGenome CreateChild_WeightMutation(NeatGenome parent)
        {
            // TODO:
            return null;
        }

        private NeatGenome CreateChild_AddNodeMutation(NeatGenome parent)
        {
            // TODO:
            return null;
        }

        private NeatGenome CreateChild_AddConnectionMutation(NeatGenome parent)
        {
            // TODO:
            return null;
        }

        private NeatGenome CreateChild_DeleteConnectionMutation(NeatGenome parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods 

        private DiscreteDistribution GetMutationTypeDistribution(NeatGenome parent)
        {
            // If there is only one connection then avoid destructive mutations to avoid the 
            // creation of genomes with no connections.
            DiscreteDistribution dist = (parent.ConnectionGeneList.Count < 2) ?
                  _settings.MutationTypeDistributionNonDestructive
                : _settings.MutationTypeDistribution;

            return dist;
        }

        #endregion
    }
}
