using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.Network;

namespace SharpNeat.Genomes.Neat
{
    internal static class NeatGenomeIntegrity
    {
        #region Private Methods [Debug Code / Integrity Checking]

        /// <summary>
        /// Performs an integrity check on the genome's internal data.
        /// Returns true if OK.
        /// </summary>
        public static bool PerformIntegrityCheck(NeatGenome ng)
        {
            // TODO: Loose end.
            //// Check genome class type (can only do this if we have a genome factory).
            //if(null != _genomeFactory && !_genomeFactory.CheckGenomeType(this)) {
            //    Debug.WriteLine($"Invalid genome class type [{this.GetType().Name}]");
            //    return false;
            //}

            // Check neuron genes.
            var neuronGeneList = ng.NeuronGeneList;
            int count = neuronGeneList.Count;
            
            // We will always have at least a bias and an output.
            if(count < 2) {
                Debug.WriteLine($"NeuronGeneList has less than the minimum number of neuron genes [{count}]");
                return false;
            }

            // Check bias neuron.
            if(NodeType.Bias != neuronGeneList[0].NodeType) {
                Debug.WriteLine("Missing bias gene");
                return false;
            }

            if(0u != neuronGeneList[0].InnovationId) {
                Debug.WriteLine($"Bias neuron ID != 0. [{ng.NeuronGeneList[0].InnovationId}]");
                return false;
            }

            // Check input neurons.
            uint prevId = 0u;
            int idx = 1;
            for(int i=0; i<ng.InputNeuronCount; i++, idx++)
            {
                if(NodeType.Input != neuronGeneList[idx].NodeType) {
                    Debug.WriteLine($"Invalid neuron gene type. Expected Input, got [{neuronGeneList[idx].NodeType}]");
                    return false;
                }

                if(neuronGeneList[idx].InnovationId <= prevId) {
                    Debug.WriteLine("Input neuron gene is out of order and/or a duplicate.");
                    return false;
                }

                prevId = neuronGeneList[idx].InnovationId;
            }

            // Check output neurons.
            for(int i=0; i<ng.OutputNeuronCount; i++, idx++)
            {
                if(NodeType.Output != neuronGeneList[idx].NodeType) {
                    Debug.WriteLine($"Invalid neuron gene type. Expected Output, got [{neuronGeneList[idx].NodeType}]");
                    return false;
                }

                if(neuronGeneList[idx].InnovationId <= prevId) {
                    Debug.WriteLine("Output neuron gene is out of order and/or a duplicate.");
                    return false;
                }

                prevId = neuronGeneList[idx].InnovationId;
            }

            // Check hidden neurons.
            // All remaining neurons should be hidden neurons.
            for(; idx<count; idx++)
            {
                if(NodeType.Hidden != neuronGeneList[idx].NodeType) {
                    Debug.WriteLine($"Invalid neuron gene type. Expected Hidden, got [{neuronGeneList[idx].NodeType}]");
                    return false;
                }

                if(neuronGeneList[idx].InnovationId <= prevId) {
                    Debug.WriteLine("Hidden neuron gene is out of order and/or a duplicate.");
                    return false;
                }

                prevId = neuronGeneList[idx].InnovationId;
            }

            // Check connection genes.
            var connectionGeneList = ng.ConnectionGeneList;
            count = connectionGeneList.Count;
            if(0 == count) 
            {   // At least one connection is required. 
                // (A) Connectionless genomes are pointless and 
                // (B) Connections form the basis for defining a genome's position in the encoding space.
                // Without a position speciation will be sub-optimal and may fail (depending on the speciation strategy).
                Debug.WriteLine("Zero connection genes.");
                return false;
            }

            Dictionary<ConnectionEndpointsStruct, object> endpointDict = new Dictionary<ConnectionEndpointsStruct,object>(count);
            
            // Initialise with the first connection's details.
            ConnectionGene connectionGene = connectionGeneList[0];
            prevId = connectionGene.InnovationId;
            endpointDict.Add(new ConnectionEndpointsStruct(connectionGene.SourceNodeId, connectionGene.TargetNodeId), null);

            // Loop over remaining connections.
            for(int i=1; i<count; i++)
            {
                connectionGene = connectionGeneList[i];
                if(connectionGene.InnovationId <= prevId) {
                    Debug.WriteLine("Connection gene is out of order and/or a duplicate.");
                    return false;
                }

                ConnectionEndpointsStruct key = new ConnectionEndpointsStruct(connectionGene.SourceNodeId, connectionGene.TargetNodeId);
                if(endpointDict.ContainsKey(key)) {
                    Debug.WriteLine("Connection gene error. A connection between the specified endpoints already exists.");
                    return false;
                }

                endpointDict.Add(key, null);
                prevId = connectionGene.InnovationId;
            }

            // Check each neuron gene's list of source and target neurons.
            // Init connection info per neuron.
            int nCount = neuronGeneList.Count;
            Dictionary<uint,NeuronConnectionInfo> conInfoByNeuronId = new Dictionary<uint,NeuronConnectionInfo>(count);
            for(int i=0; i<nCount; i++)
            {
                NeuronConnectionInfo conInfo = new NeuronConnectionInfo();
                conInfo._srcNeurons = new HashSet<uint>();
                conInfo._tgtNeurons = new HashSet<uint>();
                conInfoByNeuronId.Add(neuronGeneList[i].InnovationId, conInfo);
            }

            // Compile connectivity info.
            int cCount = connectionGeneList.Count;
            for(int i=0; i<cCount; i++)
            {
                ConnectionGene cGene = connectionGeneList[i];
                conInfoByNeuronId[cGene.SourceNodeId]._tgtNeurons.Add(cGene.TargetNodeId);
                conInfoByNeuronId[cGene.TargetNodeId]._srcNeurons.Add(cGene.SourceNodeId);
            }

            // Compare connectivity info with that recorded in each NeuronGene.
            for(int i=0; i<nCount; i++)
            {
                NeuronGene nGene = neuronGeneList[i];
                NeuronConnectionInfo conInfo = conInfoByNeuronId[nGene.InnovationId];

                // Check source node count.
                if(nGene.SourceNeurons.Count != conInfo._srcNeurons.Count) {
                    Debug.WriteLine("NeuronGene has incorrect number of source neurons recorded.");
                    return false;
                }

                // Check target node count.
                if(nGene.TargetNeurons.Count != conInfo._tgtNeurons.Count) {
                    Debug.WriteLine("NeuronGene has incorrect number of target neurons recorded.");
                    return false;
                }

                // Check that the source node IDs match up.
                foreach(uint srcNeuronId in nGene.SourceNeurons)
                {
                    if(!conInfo._srcNeurons.Contains(srcNeuronId)) {
                        Debug.WriteLine("NeuronGene has incorrect list of source neurons recorded.");
                        return false;
                    }
                }

                // Check that the target node IDs match up.
                foreach(uint tgtNeuronId in nGene.TargetNeurons)
                {
                    if(!conInfo._tgtNeurons.Contains(tgtNeuronId)) {
                        Debug.WriteLine("NeuronGene has incorrect list of target neurons recorded.");
                        return false;
                    }
                }
            }

            // TODO: Loose end.
            //// Check that network is acyclic if we are evolving feed-forward only networks 
            //// (can only do this if we have a genome factory).
            //if(null != _genomeFactory && _genomeFactory.NeatGenomeParameters.FeedforwardOnly)
            //{
            //    if(CyclicNetworkTest.IsNetworkCyclic(this)) {
            //        Debug.WriteLine("Feed-forward only network has one or more cyclic paths.");
            //        return false;
            //    }
            //}

            return true;
        }

        #endregion


        #region Inner Classes

        /// <summary>
        /// Holds sets of source and target neurons for a given neuron.
        /// </summary>
        struct NeuronConnectionInfo
        {
            /// <summary>
            /// Gets a set of IDs for the source neurons that directly connect into a given neuron.
            /// </summary>
            public HashSet<uint> _srcNeurons;
            /// <summary>
            /// Gets a set of IDs for the target neurons that a given neuron directly connects out to.
            /// </summary>
            public HashSet<uint> _tgtNeurons;
        }

        #endregion
    }
}
