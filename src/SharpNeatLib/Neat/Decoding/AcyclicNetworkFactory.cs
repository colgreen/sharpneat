using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Neat.Genome;
using SharpNeat.Network;
using SharpNeat.Network.Analysis;
using SharpNeat.Phenomes.NeuralNets;
using SharpNeat.Phenomes.NeuralNets.Acyclic;

namespace SharpNeat.Neat.Decoding
{
    /// <summary>
    /// Static factory for creating AcyclicNetwork(s) from INetworkDefinition(s).
    /// </summary>
    public static class AcyclicNetworkFactory
    {
        #region Public Static Methods

        public static AcyclicNetwork CreateAcyclicNetwork(INetworkDefinition netDef, bool boundedOutput)
        {
            IActivationFunction<double>[] activationFnArr;
            ConnectionInfo[] connInfoArr;
            LayerInfo[] layerInfoArr;
            int[] outputNeuronIdxArr;
            InternalDecode(netDef, out activationFnArr, out connInfoArr, out layerInfoArr, out outputNeuronIdxArr);

            return new AcyclicNetwork(
                activationFnArr[0].Fn,
                connInfoArr,
                layerInfoArr,
                outputNeuronIdxArr,
                netDef.NodeList.Count,
                netDef.InputNodeCount,
                netDef.OutputNodeCount,
                boundedOutput);
        }

        public static HeterogeneousAcyclicNetwork CreateHeterogeneousAcyclicNetwork(INetworkDefinition netDef, bool boundedOutput)
        {
            IActivationFunction<double>[] activationFnArr;
            ConnectionInfo[] connInfoArr;
            LayerInfo[] layerInfoArr;
            int[] outputNeuronIdxArr;
            InternalDecode(netDef, out activationFnArr, out connInfoArr, out layerInfoArr, out outputNeuronIdxArr);

            // Extract the specific activation function delegate we want from each IActivationFunction.
            var fnArr = new Func<double,double>[activationFnArr.Length];
            for(int i=0; i<activationFnArr.Length; i++) {
                fnArr[i] = activationFnArr[i].Fn;
            }

            return new HeterogeneousAcyclicNetwork(
                fnArr,
                connInfoArr,
                layerInfoArr,
                outputNeuronIdxArr,
                netDef.NodeList.Count,
                netDef.InputNodeCount,
                netDef.OutputNodeCount,
                boundedOutput);
        }

        #endregion

        #region Private Static Methods

        private static void InternalDecode(
            NeatGenome genome,
            out IActivationFunction<double>[] activationFnArr,
            out ConnectionInfo[] connInfoArr,
            out LayerInfo[] layerInfoArr,
            out int[] outputNeuronIdxArr)
        {
            Debug.Assert(!CyclicNetworkTest.IsNetworkCyclic(netDef), "Attempt to decode a cyclic network definition into an AcyclicNetwork.");

            // Determine the depth of each node in the network. 
            // Node depths are used to separate the nodes into depth based layers, these layers can then be
            // used to determine the order in which signals are propagated through the network.
            AcyclicNetworkDepthAnalysis depthAnalysis = new AcyclicNetworkDepthAnalysis();
            NetworkDepthInfo netDepthInfo = depthAnalysis.CalculateNodeDepths(netDef);

            // Construct an array of NodeInfo, ordered by node depth.
            // Create/populate NodeInfo array.
            int[] nodeDepthArr = netDepthInfo._nodeDepthArr;
            INodeList nodeList = netDef.NodeList;
            int nodeCount = nodeList.Count;
            NodeInfo[] nodeInfoByDepth = new NodeInfo[nodeCount];
            for(int i=0; i<nodeCount; i++)
            {
                nodeInfoByDepth[i]._nodeId = nodeList[i].Id;
                nodeInfoByDepth[i]._definitionIdx = i;
                nodeInfoByDepth[i]._nodeDepth = nodeDepthArr[i];
            }

            // Sort NodeInfo array.
            // We use an IComparer here because an anonymous method is not accepted on the method overload that accepts
            // a sort range, which we use to avoid sorting the input nodes. Sort() performs an unstable sort therefore
            // we must restrict the range of the sort to ensure the input node indexes are unchanged. Restricting the
            // sort to the required range is also more efficient (less items to sort).
            int inputCount = netDef.InputNodeCount;
            Array.Sort(nodeInfoByDepth, inputCount, nodeCount-inputCount, NodeDepthComparer.__NodeDepthComparer);

            // Array of live node indexes indexed by their index in the original network definition. This allows us to 
            // locate the position of input and output nodes in their new positions in the live network data structures.
            int[] newIdxByDefinitionIdx = new int[nodeCount];

            // Dictionary of live node indexes keyed by node ID. This allows us to convert the network definition connection
            // endpoints from node IDs to indexes into the live/runtime network data structures.
            Dictionary<uint,int> newIdxById = new Dictionary<uint,int>(nodeCount);

            // Populate both the lookup array and dictionary.
            for(int i=0; i<nodeCount; i++) 
            {
                NodeInfo nodeInfo = nodeInfoByDepth[i];
                newIdxByDefinitionIdx[nodeInfo._definitionIdx] = i;
                newIdxById.Add(nodeInfo._nodeId, i);
            }

            // Make a copy of the sub-range of newIdxByDefinitionIdx that represents the output nodes.
            int outputCount = netDef.OutputNodeCount;
            outputNeuronIdxArr = new int[outputCount];
            // Note. 'inputCount' holds the index of the first output node.
            Array.Copy(newIdxByDefinitionIdx, inputCount, outputNeuronIdxArr, 0, outputCount);

            // Construct activation function array.
            IActivationFunctionLibrary activationFnLibrary = netDef.ActivationFnLibrary;
            activationFnArr = new IActivationFunction<double>[nodeCount];
            for(int i=0; i<nodeCount; i++) 
            {
                int definitionIdx = nodeInfoByDepth[i]._definitionIdx;
                activationFnArr[i] = activationFnLibrary.GetFunction(nodeList[definitionIdx].ActivationFnId);
            }


        //=== Create array of ConnectionInfo(s). 

            // Loop the connections and lookup the node IDs for each connection's end points using newIdxById.
            IConnectionList connectionList = netDef.ConnectionList;
            int connectionCount = connectionList.Count;
            connInfoArr = new ConnectionInfo[connectionCount];

            for(int i=0; i<connectionCount; i++)
            {   
                INetworkConnection conn = connectionList[i];
                connInfoArr[i]._srcNeuronIdx = newIdxById[conn.SourceNodeId];
                connInfoArr[i]._tgtNeuronIdx = newIdxById[conn.TargetNodeId];
                connInfoArr[i]._weight = conn.Weight;
            }

            // Sort connInfoArr by source node index. This allows us to activate the connections in the 
            // order they are present within the network (by depth). We also secondary sort by target index to 
            // improve CPU cache coherency of the data (in order accesses that are as close to each other as possible).
            Array.Sort(connInfoArr, delegate(ConnectionInfo x, ConnectionInfo y)
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
            // The array is in order of depth, from layer zero (inputs nodes) to the last layer 
            // (usually output nodes, but not necessarily if there is a dead end pathway with a high number of hops).
            // Note. There is guaranteed to be at least one connection with a source at a given depth level, this is
            // because for there to be a layer N there must necessarily be a connection from a node in layer N-1 
            // to a node in layer N.
            int netDepth = netDepthInfo._networkDepth;
            layerInfoArr = new LayerInfo[netDepth];

            // Scanning over nodes can start at inputCount instead of zero, because we know that all nodes prior to 
            // that index are at depth zero.
            int nodeIdx = inputCount;
            int connIdx = 0;

            for(int currDepth=0; currDepth < netDepth; currDepth++)
            {
                // Scan for last node at the current depth.
                for(; nodeIdx < nodeCount && nodeInfoByDepth[nodeIdx]._nodeDepth == currDepth; nodeIdx++);
                
                // Scan for last connection at the current depth.
                for(; connIdx < connInfoArr.Length && nodeInfoByDepth[connInfoArr[connIdx]._srcNeuronIdx]._nodeDepth == currDepth; connIdx++);

                // Store node and connection end indexes for the layer.
                layerInfoArr[currDepth]._endNodeIdx = nodeIdx;
                layerInfoArr[currDepth]._endConnectionIdx = connIdx;
            }
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
                // Use fast method of comparison (subtraction) instead of performing multiple tests. 
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
