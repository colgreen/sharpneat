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
                NeatGenome childGenome = CreateChildInner(parent, ref mutationTypeDist);
                if(null != childGenome) {
                    return childGenome;
                }
            }
        }
        
        #endregion

        #region Private Methods [CreateChild Subroutines]

        public NeatGenome CreateChildInner(NeatGenome parent, ref DiscreteDistribution mutationTypeDist)
        {
            // Determine the type of mutation to attempt.
            int mutationTypeId = mutationTypeDist.Sample(_rng);

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

        private NeatGenome CreateChild_WeightMutation(NeatGenome parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods [CreateChild_AddNodeMutation]

        private NeatGenome CreateChild_AddNodeMutation(NeatGenome parent)
        {
            // Notes.
            // Adding a node is achieved by selecting a connection at random, and replaing it with
            // a link made up of two connections with a node in between. I.e. the original connection 
            // is 'split' by a new node.
            // Genomes are guaranteed to always have at least one connection, therefore this type of mutation 
            // will always succeed.

            // Select a connection at random from the parent genome.
            ConnectionGeneList parentGeneList = parent.ConnectionGeneList;
            int replaceIdx = _rng.Next(parentGeneList.Count);
            ConnectionGene connectionToReplace = parentGeneList[replaceIdx];

            // Get IDs for the two new connections and single new node. This call will check the history 
            // buffer (AddedNodeBuffer) for matching structures from previously added nodes.
            AddedNodeInfo addedNodeInfo = CreateChild_AddNodeMutation_GetIDs(parent, connectionToReplace.InnovationId);

            // TODO: Review this. The connection weight allocation scheme here is adopted from SharpNEAT v2.x, but may 
            // not be optimal, or even good.
            // The basic principle (or goal) is that we wish to replace the removed connection with new structure that is 
            // as near to being functionally equivalent as is possible.
            
            // Create two new connection genes.
            var cGeneNew1 = new ConnectionGene(addedNodeInfo.AddedInputConnectionId, 
                                               connectionToReplace.SourceNodeId,
                                               addedNodeInfo.AddedNodeId,
                                               connectionToReplace.Weight);

            var cGeneNew2 = new ConnectionGene(addedNodeInfo.AddedInputConnectionId, 
                                               addedNodeInfo.AddedNodeId,
                                               connectionToReplace.TargetNodeId,
                                               _pop.MetaNeatGenome.ConnectionWeightRange);

            // We have yet to either clone the parent's connection genes, or do the actual connection replacement;
            // we now do both of these things together, and in a way that avoid creating a list and then removing and adding to it,
            // which requires inefficient shuffling of list item elements.

            // Create a sorted list of the innovation IDs that we are adding or removing from the connection gene list.
            // Each ID is paired with a boolean; true => new gene to be added, false => old gene to be removed.
            var actionIdList = new List<Tuple<ConnectionGene,bool>>(3);
            actionIdList.Add(Tuple.Create(connectionToReplace, false));
            actionIdList.Add(Tuple.Create(cGeneNew1, true));
            actionIdList.Add(Tuple.Create(cGeneNew2, true));
            actionIdList.Sort(delegate(Tuple<ConnectionGene,bool> x, Tuple<ConnectionGene,bool> y) { 
                                return x.Item1.InnovationId.CompareTo(y.Item1.InnovationId);
                            });

            // Copy the parent genes to a new gene list.
            ConnectionGeneList newGeneList = new ConnectionGeneList(parentGeneList.Count + 1);
            int parentIdx = 0;

            // TODO: Create unit tests (complex logic).

            // Loop the actionable innovation IDs.
            for(int i=0; i < 3; i++)
            {
                var tuple = actionIdList[i];
                uint actionableId = tuple.Item1.InnovationId;

                // Copy connection genes until we reach the current/next actionable innovation ID.
                for(; parentGeneList[parentIdx].InnovationId < actionableId && parentIdx < parentGeneList.Count; parentIdx++) {
                    newGeneList.Add(new ConnectionGene(parentGeneList[parentIdx]));
                }

                if(tuple.Item2)
                {   // Insert new gene.
                    newGeneList.Add(tuple.Item1);
                }
                else
                {   // Gene to be removed; skip it.
                    parentIdx++;
                }
            }

            // Copy any remaining parent genes.
            for(; parentIdx < parentGeneList.Count; parentIdx++) {
                newGeneList.Add(new ConnectionGene(parentGeneList[parentIdx]));
            }

            uint genomeId = _pop.GenomeIdSeq.Next();
            return new NeatGenome(genomeId, _pop.CurrentGenerationAge, newGeneList);            
        }

        private AddedNodeInfo CreateChild_AddNodeMutation_GetIDs(NeatGenome parent, uint connectionToReplaceId)
        {
            // If the connection being replaced has already been replaced previously (on a different genome)
            // *and* none of the innovation IDs used previously are present on the genome, then we can re-use
            // the IDs from the previous replacement.
            //
            // Note. Some of the innovations IDs may already be present on our genome, because it's possible 
            // to acquire some subset of them via sexual reproduction, or asexual reproduction that copies all of them
            // but a further asexual mutation could delete some of the connections.
            AddedNodeInfo addedNodeInfo;
            bool connectionExists = _pop.AddedNodeBuffer.TryGetValue(connectionToReplaceId, out addedNodeInfo);
            if(connectionExists
                && parent.ConnectionGeneList.BinarySearch(addedNodeInfo.AddedInputConnectionId) < 0
                && parent.ConnectionGeneList.BinarySearch(addedNodeInfo.AddedOutputConnectionId) < 0
                && !parent.NodeIdSet.Contains(addedNodeInfo.AddedNodeId)) 
            {
                return addedNodeInfo;
            }

            // There is either no pre-existing matching structure, or if there some of its genes (identified by 
            // innovation IDs) are already present in the parent genome.
            // As such the new structures (one node and two connections) are allocated new IDs.
            addedNodeInfo = new AddedNodeInfo(_pop.InnovationIdSeq);

            // Finally, we record the newly generated IDs, but only if connectionExists is false. I.e if a
            // set of IDs has already been recorded for connectionToReplaceId, then we leave those IDs in place.
            if(connectionExists) {
                _pop.AddedNodeBuffer.Enqueue(connectionToReplaceId, addedNodeInfo);
            }
            return addedNodeInfo;
        }

        #endregion

        #region Private Methods [CreateChild_AddConnectionMutation]

        private NeatGenome CreateChild_AddConnectionMutation(NeatGenome parent)
        {
            // TODO:
            return null;
        }

        #endregion

        #region Private Methods [CreateChild_DeleteConnectionMutation]

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
