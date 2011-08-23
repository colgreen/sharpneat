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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Network;
using SharpNeat.Phenomes.NeuralNets;

namespace SharpNeat.Decoders
{
    /// <summary>
    /// Static factory for creating AcyclicNetwork(s) from INetworkDefinition(s).
    /// </summary>
    public class FastAcyclicNetworkFactory
    {
        #region Public Methods

        /// <summary>
        /// Creates a AcyclicNetwork from an INetworkDefinition.
        /// </summary>
        public static FastAcyclicNetwork CreateFastAcyclicNetwork(INetworkDefinition networkDef)
        {
            Debug.Assert(!CyclicNetworkTest.IsNetworkCyclic(networkDef), "Attempt to decode a cyclic network into a FastAcyclicNetwork.");

            // Determine the depth of each node in the network. 
            // Node depths are used to seperate the nodes into depth based layers, these layers can then be
            // used to determine the order in which signals are propogated through the network.
            AcyclicNetworkDepthAnalysis depthAnalysis = new AcyclicNetworkDepthAnalysis();
            NetworkDepthInfo netDepthInfo = depthAnalysis.CalculateNodeDepths(networkDef);

            // Construct an array of NodeInfo, ordered by node depth.
            // Create/populate NodeInfo array.
            int[] nodeDepthArr = netDepthInfo._nodeDepthArr;
            INodeList nodeList = networkDef.NodeList;
            int nodeCount = nodeList.Count;
            NodeInfo[] nodeInfoByDepth = new NodeInfo[nodeCount];
            for(int i=0; i<nodeCount; i++) {
                nodeInfoByDepth[i]._nodeId = nodeList[i].Id;
                nodeInfoByDepth[i]._definitionIdx = i;
                nodeInfoByDepth[i]._nodeDepth = nodeDepthArr[i];
            }

            // Sort NodeInfo array.
            // We use an IComparer here because an anonymous method is not accepted on the method overload that accepts
            // a sort range, which we use to avoid sorting the input and bias nodes. Sort() performs an unstable sort therefore
            // we must restrict the range of the sort to ensure the input and bias node indexes are unchanged. Restricting the
            // sort to the required range is also more efficient (less items to sort).
            int inputAndBiasCount = networkDef.InputNodeCount + 1;
            Array.Sort(nodeInfoByDepth, inputAndBiasCount, nodeCount-inputAndBiasCount, NodeDepthComparer.__NodeDepthComparer);

            // Array of live node indexes indexed by their index in the original network definition. This allows us to 
            // locate the position of input and output nodes in their new positions in the live network data structures.
            int[] newIdxByDefinitionIdx = new int[nodeCount];
            
            // Dictionary of live node indexes keyed by node ID. This allows us to convert the network definition connection
            // endpoints from referring to node IDs to indexes to node data in the live network data structures.
            Dictionary<uint,int> newIdxById = new Dictionary<uint,int>(nodeCount);

            // Populate both the lookup array and dictionary.
            for(int i=0; i<nodeCount; i++) 
            {
                NodeInfo nodeInfo = nodeInfoByDepth[i];
                newIdxByDefinitionIdx[nodeInfo._definitionIdx] = i;
                newIdxById.Add(nodeInfo._nodeId, i);
            }

            // Make a copy of the sub-range of newIdxByDefinitionIdx that respresents the output nodes.
            int outputCount = networkDef.OutputNodeCount;
            int[] outputNeuronIdxArr = new int[outputCount];
            // Note. 'inputAndBiasCount' holds the index of the first output node.
            Array.Copy(newIdxByDefinitionIdx, inputAndBiasCount, outputNeuronIdxArr, 0, outputCount);

            // Construct arrays with additional 'per node' data/refs (activation functions, activation fn auxiliary data).
            IActivationFunctionLibrary activationFnLibrary = networkDef.ActivationFnLibrary;
            IActivationFunction[] nodeActivationFnArr = new IActivationFunction[nodeCount];
            double[][] nodeAuxArgsArray = new double[nodeCount][];
            for(int i=0; i<nodeCount; i++) 
            {
                int definitionIdx = newIdxByDefinitionIdx[i];
                nodeActivationFnArr[i] = activationFnLibrary.GetFunction(nodeList[definitionIdx].ActivationFnId);
                nodeAuxArgsArray[i] = nodeList[definitionIdx].AuxState;
            }


        //=== Create array of FastConnection(s). 

            // Loop the connections and lookup the node IDs for each connection's end points using newIdxById.
            IConnectionList connectionList = networkDef.ConnectionList;
            int connectionCount = connectionList.Count;
            FastConnection[] fastConnectionArray = new FastConnection[connectionCount];

            for(int i=0; i<connectionCount; i++)
            {   
                INetworkConnection conn = connectionList[i];
                fastConnectionArray[i]._srcNeuronIdx = newIdxById[conn.SourceNodeId];
                fastConnectionArray[i]._tgtNeuronIdx = newIdxById[conn.TargetNodeId];
                fastConnectionArray[i]._weight = conn.Weight;
            }

            // Sort fastConnectionArray by source node index. This allows us to activate the connections in the 
            // order they are present within the network (by depth). We also secondary sort by target index to 
            // improve CPU cache coherency of the data (in order accesses that are as close to each other as possible).
            Array.Sort(fastConnectionArray, delegate(FastConnection x, FastConnection y)
            {   
                if(x._srcNeuronIdx < y._srcNeuronIdx) {
                    return -1;
                }
                if(x._srcNeuronIdx > y._srcNeuronIdx) {
                    return 1;
                }
                // Secondary sort on target index.
                if(x._tgtNeuronIdx < y._tgtNeuronIdx) {
                    return -1;
                }
                if(x._tgtNeuronIdx > y._tgtNeuronIdx) {
                    return 1;
                }
                // Connections are equal (this should not actually happen).
                return 0;
            });

            // Create an array of LayerInfo(s). Each LayerInfo contains the index + 1 of both the last node and last 
            // connection in that layer.
            // The array is in order of depth, from layer zero (bias and inputs nodes) to the last layer 
            // (usually output nodes, but not necessarily if there is a dead end pathway with a high number of hops).
            // Note. There is guaranteed to be at least one connection with a source at a given depth level, this is
            // because for there to be a layer N there must necessarily be a connection from a node in layer N-1 
            // to a node in layer N.
            int netDepth = netDepthInfo._networkDepth;
            LayerInfo[] layerInfoArr = new LayerInfo[netDepth];

            // Scanning over nodes can start at inputAndBiasCount instead of zero, 
            // because we know that all nodes prior to that index are at depth zero.
            int nodeIdx = inputAndBiasCount;
            int connIdx = 0;

            for(int currDepth=0; currDepth < netDepth; currDepth++)
            {
                // Scan for last node at the current depth.
                for(; nodeIdx < nodeCount && nodeInfoByDepth[nodeIdx]._nodeDepth == currDepth; nodeIdx++);
                
                // Scan for last connection at the current depth.
                for(; connIdx < fastConnectionArray.Length && nodeInfoByDepth[fastConnectionArray[connIdx]._srcNeuronIdx]._nodeDepth == currDepth; connIdx++);

                // Store node and connection end indexes for the layer.
                layerInfoArr[currDepth]._endNodeIdx = nodeIdx;
                layerInfoArr[currDepth]._endConnectionIdx = connIdx;
            }

            return new FastAcyclicNetwork(nodeActivationFnArr, nodeAuxArgsArray, fastConnectionArray, layerInfoArr, outputNeuronIdxArr,
                                          nodeCount, networkDef.InputNodeCount, networkDef.OutputNodeCount);
        }

        #endregion

        #region Inner Classes/Structs

        class NodeDepthComparer : IComparer<NodeInfo>
        {
            /// <summary>
            /// Singleton instance.
            /// </summary>
            public static readonly NodeDepthComparer __NodeDepthComparer = new NodeDepthComparer();

            public int Compare(NodeInfo x, NodeInfo y)
            {
                // Use fast method of comparison (subtraction) instead of perfoming multiple tests. 
                // We can do this safely because this delta will always be well within the range on an Int32.
                // (If you have a network with a greater depth range then you have other problems).
                return x._nodeDepth - y._nodeDepth;
            }
        }

        struct NodeInfo
        {
            /// <summary>
            /// The node's ID.
            /// </summary>
            public uint _nodeId;
            /// <summary>
            /// The node's index in the network definition. Input and output nodes are identifiable by this index.
            /// </summary>
            public int _definitionIdx;
            /// <summary>
            /// The node's depth within the network.
            /// </summary>
            public int _nodeDepth;
        }

        #endregion
    }
}
