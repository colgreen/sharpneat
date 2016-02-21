/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using System.Collections.Generic;
using SharpNeat.Network;

namespace SharpNeat.Genomes.Neat
{
    /// <summary>
    /// Used for building a list of connection genes. 
    /// 
    /// Connection genes are added one by one to a list and a dictionary of added connection genes is maintained
    /// keyed on ConnectionEndpointsStruct to allow a caller to check if a connection with the same end points
    /// (and potentially a different innovation ID) already exists in the list.
    /// </summary>
    public class ConnectionGeneListBuilder
    {
        readonly ConnectionGeneList _connectionGeneList;
        readonly Dictionary<ConnectionEndpointsStruct,ConnectionGene> _connectionGeneDictionary;
        readonly SortedDictionary<uint,NeuronGene> _neuronDictionary;
        // Note. connection gene innovation IDs always start above zero as they share the ID space with neurons, 
        // which always come first (e.g. bias neuron is always ID 0).
        uint _highestConnectionGeneId = 0; 

        #region Constructor

        /// <summary>
        /// Constructs the builder with the provided capacity. The capacity should be chosen 
        /// to limit the number of memory re-allocations that occur within the contained
        /// connection list dictionary.
        /// </summary>
        public ConnectionGeneListBuilder(int connectionCapacity)
        {
            _connectionGeneList = new ConnectionGeneList(connectionCapacity);
            _connectionGeneDictionary = new Dictionary<ConnectionEndpointsStruct,ConnectionGene>(connectionCapacity);
            // TODO: Determine better initial capacity.
            _neuronDictionary = new SortedDictionary<uint,NeuronGene>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the contained list of connection genes.
        /// </summary>
        public ConnectionGeneList ConnectionGeneList
        {
            get { return _connectionGeneList; }
        }

        /// <summary>
        /// Gets the builder's dictionary of connection genes keyed on ConnectionEndpointsStruct.
        /// </summary>
        public Dictionary<ConnectionEndpointsStruct,ConnectionGene> ConnectionGeneDictionary
        {
            get { return _connectionGeneDictionary; }
        }

        /// <summary>
        /// Gets the builder's dictionary of neuron IDs obtained from contained connection gene endpoints.
        /// </summary>
        public SortedDictionary<uint,NeuronGene> NeuronDictionary
        {
            get { return _neuronDictionary; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a ConnectionGene to the builder, but only if the connection is not already present (as determined by it's neuron ID endpoints).
        /// </summary>
        /// <param name="connectionGene">The connection to add.</param>
        /// <param name="parentGenome">The conenction's parent genome. This is used to obtain NeuronGene(s) for the connection endpoints.</param>
        /// <param name="overwriteExisting">A flag that indicates if this connection should take precedence oevr an existing connection with
        /// the same endpoints.</param>
        public void TryAddGene(ConnectionGene connectionGene, NeatGenome parentGenome, bool overwriteExisting)
        {
            // Check if a matching gene has already been added.
            ConnectionEndpointsStruct connectionKey = new ConnectionEndpointsStruct(connectionGene.SourceNodeId, connectionGene.TargetNodeId);
            
            ConnectionGene existingConnectionGene;
            if(!_connectionGeneDictionary.TryGetValue(connectionKey, out existingConnectionGene))
            {   // Add new connection gene.
                ConnectionGene connectionGeneCopy = new ConnectionGene(connectionGene);
                _connectionGeneDictionary.Add(connectionKey, connectionGeneCopy);

                // Insert connection gene into a list. Use more efficient approach (append to end) if we know the gene belongs at the end.
                if(connectionGeneCopy.InnovationId > _highestConnectionGeneId) {
                    _connectionGeneList.Add(connectionGeneCopy);
                    _highestConnectionGeneId = connectionGeneCopy.InnovationId;
                } else {
                    _connectionGeneList.InsertIntoPosition(connectionGeneCopy);
                }

                // Add neuron genes (if not already added).
                // Source neuron.
                NeuronGene srcNeuronGene;
                if(!_neuronDictionary.TryGetValue(connectionGene.SourceNodeId, out srcNeuronGene))
                {
                    srcNeuronGene = parentGenome.NeuronGeneList.GetNeuronById(connectionGene.SourceNodeId);
                    srcNeuronGene = new NeuronGene(srcNeuronGene, false); // Make a copy.
                    _neuronDictionary.Add(srcNeuronGene.Id, srcNeuronGene);
                }

                // Target neuron.
                NeuronGene tgtNeuronGene;
                if(!_neuronDictionary.TryGetValue(connectionGene.TargetNodeId, out tgtNeuronGene))
                {
                    tgtNeuronGene = parentGenome.NeuronGeneList.GetNeuronById(connectionGene.TargetNodeId);
                    tgtNeuronGene = new NeuronGene(tgtNeuronGene, false); // Make a copy.
                    _neuronDictionary.Add(tgtNeuronGene.Id, tgtNeuronGene);
                }

                // Register connectivity with each neuron.
                srcNeuronGene.TargetNeurons.Add(tgtNeuronGene.Id);
                tgtNeuronGene.SourceNeurons.Add(srcNeuronGene.Id);
            }
            else if(overwriteExisting)
            {   // The genome we are building already has a connection with the same neuron endpoints as the one we are
                // trying to add. It didn't match up during correlation because it has a different innovation number, this
                // is possible because the innovation history buffers throw away old innovations in a FIFO manner in order
                // to prevent them from bloating.

                // Here the 'overwriteExisting' flag is set so the gene we are currently trying to add is probably from the
                // fitter parent, and therefore we want to use its connection weight in place of the existing gene's weight.
                existingConnectionGene.Weight = connectionGene.Weight;
            }
        }

        /// <summary>
        /// Tests if adding the specified connection would cause a cyclic pathway in the network connectivity.
        /// Returns true if the connection would form a cycle.
        /// Note. This same logic is implemented on NeatGenome.IsConnectionCyclic() but against slightly 
        /// different data structures, hence the method is re-implemented here.
        /// </summary>
        public bool IsConnectionCyclic(uint srcNeuronId, uint tgtNeuronId)
        {
            // Quick test. Is connection connecting a neuron to itself.
            if(srcNeuronId == tgtNeuronId) {
                return true;
            }

            // Quick test. If one of the neuron's is not yet registered with the builder then there can be no cyclic connection
            // (the connection is coming-from or going-to a dead end).
            NeuronGene srcNeuron;
            if(!_neuronDictionary.TryGetValue(srcNeuronId, out srcNeuron) || !_neuronDictionary.ContainsKey(tgtNeuronId)) {
                return false;
            }

            // Trace backwards through sourceNeuron's source neurons. If targetNeuron is encountered then it feeds
            // signals into sourceNeuron already and therefore a new connection between sourceNeuron and targetNeuron
            // would create a cycle.

            // Maintain a set of neurons that have been visited. This allows us to avoid unnecessary re-traversal 
            // of the network and detection of cyclic connections.
            HashSet<uint> visitedNeurons = new HashSet<uint>();
            visitedNeurons.Add(srcNeuronId);

            // Search uses an explicitly created stack instead of function recursion, the logic here is that this 
            // may be more efficient through avoidance of multiple function calls (but not sure).
            Stack<uint> workStack = new Stack<uint>();

            // Push source neuron's sources onto the work stack. We could just push the source neuron but we choose
            // to cover that test above to avoid the one extra neuronID lookup that would require.
            foreach(uint neuronId in srcNeuron.SourceNeurons) {
                workStack.Push(neuronId);
            }

            // While there are neurons to check/traverse.
            while(0 != workStack.Count)
            {
                // Pop a neuron to check from the top of the stack, and then check it.
                uint currNeuronId = workStack.Pop();
                if(visitedNeurons.Contains(currNeuronId)) {
                    // Already visited (via a different route).
                    continue;
                }

                if(currNeuronId == tgtNeuronId) {
                    // Target neuron already feeds into the source neuron.
                    return true;
                }

                // Register visit of this node.
                visitedNeurons.Add(currNeuronId);                

                // Push the current neuron's source neurons onto the work stack.
                NeuronGene currNeuron = _neuronDictionary[currNeuronId];
                foreach(uint neuronId in currNeuron.SourceNeurons) {
                    workStack.Push(neuronId);
                }
            }

            // Connection not cyclic.
            return false;
        }

        #endregion
    }
}
