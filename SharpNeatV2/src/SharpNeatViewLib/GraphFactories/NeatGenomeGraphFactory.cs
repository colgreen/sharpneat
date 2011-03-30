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
using SharpNeat.Genomes.Neat;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// A factory that creates IOGraph objects from NeatGenome objects.
    /// </summary>
    public class NeatGenomeGraphFactory
    {
        /// <summary>
        /// Create an IOGraph that represents the structure described by the provided NetGenome.
        /// </summary>
        public IOGraph CreateGraph(NeatGenome genome)
        {
            // Create an IOGraph, allocating storage for the node lists.
            NeuronGeneList nGeneList = genome.NeuronGeneList;
            int nodeCount = genome.NeuronGeneList.Count;
            int inputCount = genome.InputAndBiasNeuronCount;
            int outputCount = genome.OutputNeuronCount;
            int hiddenCount = nodeCount - genome.InputBiasOutputNeuronCount;
            IOGraph ioGraph = new IOGraph(inputCount, outputCount, hiddenCount,
                                          (float)genome.GenomeFactory.NeatGenomeParameters.ConnectionWeightRange);
            
            // We also build a dictionary of nodes keyed by innovation ID. This is used later
            // to assign connections to nodes.
            Dictionary<uint, GraphNode> nodeDict = new Dictionary<uint,GraphNode>(genome.NeuronGeneList.Count);

            // Loop input nodes.
            int idx = 0;
            for(int i=0; i<inputCount; i++, idx++)
            {   // Create node, assign it a tag and add it to the node dictionary and the
                // input node list of the IOGraph.
                uint innovationId = nGeneList[idx].InnovationId;
                GraphNode node = new GraphNode(innovationId.ToString());
                node.AuxData = CreateGraphNodeAuxData(nGeneList[idx]);
                nodeDict.Add(innovationId, node);
                ioGraph.InputNodeList.Add(node);
            }

            // Loop input nodes.
            for(int i=0; i<outputCount; i++, idx++)
            {   // Create node, assign it a tag and add it to the node dictionary and the
                // output node list of the IOGraph.
                uint innovationId = nGeneList[idx].InnovationId;
                GraphNode node = new GraphNode(innovationId.ToString());
                node.AuxData = CreateGraphNodeAuxData(nGeneList[idx]);
                nodeDict.Add(innovationId, node);
                ioGraph.OutputNodeList.Add(node);
            }

            // Loop hidden nodes.
            for(; idx<nodeCount; idx++)
            {   // Create node, assign it a tag and add it to the node dictionary and the
                // hidden node list of the IOGraph.
                uint innovationId = nGeneList[idx].InnovationId;
                GraphNode node = new GraphNode(innovationId.ToString());
                node.AuxData = CreateGraphNodeAuxData(nGeneList[idx]);
                nodeDict.Add(innovationId, node);
                ioGraph.HiddenNodeList.Add(node);
            }

            // Loop connections. Build GraphConnection objects and connect them to their source
            // and target nodes.
            ConnectionGeneList cGeneList = genome.ConnectionGeneList;
            int connCount = cGeneList.Count;            
            for(int i=0; i<connCount; i++)
            {
                // Create connection object and assign it's source and target nodes.
                ConnectionGene cGene = cGeneList[i];
                GraphNode sourceNode = nodeDict[cGene.SourceNodeId];
                GraphNode targetNode = nodeDict[cGene.TargetNodeId];
                GraphConnection conn = new GraphConnection(sourceNode, targetNode, (float)cGene.Weight); 
    
                // Add the conenction to the connection lists on the source and target nodes.
                sourceNode.OutConnectionList.Add(conn);
                targetNode.InConnectionList.Add(conn);
            }

            return ioGraph;
        }

        /// <summary>
        /// Create auxilliary data for the specified NeuronGene.
        /// This version places the neuron activation function ID into element 0 of the aux data array.
        /// </summary>
        protected virtual object[] CreateGraphNodeAuxData(NeuronGene neuronGene)
        {   
            return new object[] {neuronGene.ActivationFnId};
        }
    }
}
