using Redzen.Numerics;
using SharpNeat.Neat.Genome;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpNeat.Neat.Reproduction.Asexual
{


    internal class AddCyclicConnectionMutation
    {
        NeatPopulation _pop;
        IRandomSource _rng;

        #region Constructor

        public AddCyclicConnectionMutation(NeatPopulation pop, IRandomSource rng)
        {
            _pop = pop;
            _rng = rng;
        }

        #endregion

        

        #region Public Methods

        public NeatGenome CreateChild(NeatGenome parent)
        { 
            // Notes.
            // Two nodes are selected at random, a source node and a target node. If no connection exists from the source to 
            // the target then a new connection is made between those two nodes.
            //



            // We attempt to find a pair of neurons with no connection between them in one or both directions. We disallow multiple
            // connections between the same two neurons going in the same direction, but we *do* allow connections going 
            // in opposite directions (one connection each way). We also allow a neuron to have a single recurrent connection, 
            // that is, a connection that has the same neuron as its source and target neuron.

            // ENHANCEMENT: Test connection 'density' and use alternative connection selection method if above some threshold.

            // Because input/output neurons are fixed (cannot be added to or deleted) and always present (any domain that 
            // doesn't require input/outputs is a bit nonsensical) we always have candidate pairs of neurons to consider
            // adding connections to, but if all neurons are already fully interconnected then we should handle this case
            // where there are no possible neuron pairs to add a connection to. To handle this we use a simple strategy
            // of testing the suitability of randomly selected pairs and after some number of failed tests we bail out
            // of the routine and perform weight mutation as a last resort - so that we did at least some form of mutation on 
            // the genome.

            // TODO: Try to improve chance of finding a candidate connection to make.



            HashSet<uint> nodeIdSet = parent.NodeIdSet;
            int nodeCount = nodeIdSet.Count;

            var metaNeatGenome = _pop.MetaNeatGenome;

            int inputCount = metaNeatGenome.InputNodeCount;
            int hiddenOutputCount = nodeCount - _pop.MetaNeatGenome.InputNodeCount;
            int inputHiddenCount = nodeCount - _pop.MetaNeatGenome.OutputNodeCount;


            for(int attempts=0; attempts<5; attempts++)
            {
                // Select candidate source and target nodes.
                // Source node can by any node. Target node is any node except input nodes.
                int srcNodeIdx = _rng.Next(nodeCount);
                int tgtNeuronIdx = inputCount + _rng.Next(hiddenOutputCount);

                // Test if this connection already exists.
                NeuronGene sourceNeuron = _neuronGeneList[srcNodeIdx];            
                NeuronGene targetNeuron = _neuronGeneList[tgtNeuronIdx];
                if(sourceNeuron.TargetNeurons.Contains(targetNeuron.Id)) 
                {   // Try again.
                    continue;
                }
                return Mutate_AddConnection_CreateConnection(sourceNeuron, targetNeuron);
            }
            

            // No valid connection to create was found. 
            // Indicate failure.
            return null;
        }

        #endregion
    }
}
