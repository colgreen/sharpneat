/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2016 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with SharpNEAT; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using SharpNeat.Network;

namespace SharpNeat.View.Graph
{
    /// <summary>
    /// A factory that creates IOGraph objects from INetworkDefinition(s).
    /// </summary>
    public class NetworkGraphFactory
    {
        /// <summary>
        /// Create an IOGraph that represents the structure described by the provided INetworkDefinition.
        /// </summary>
        public IOGraph CreateGraph(INetworkDefinition networkDef)
        {
            // Perform depth analysis of network.
            NetworkDepthInfo depthInfo;
            if(networkDef.IsAcyclic)
            {
                AcyclicNetworkDepthAnalysis depthAnalysis = new AcyclicNetworkDepthAnalysis();
                depthInfo = depthAnalysis.CalculateNodeDepths(networkDef);
            }
            else
            {   
                CyclicNetworkDepthAnalysis depthAnalysis = new CyclicNetworkDepthAnalysis();
                depthInfo = depthAnalysis.CalculateNodeDepths(networkDef);
            }

            // Create an IOGraph, allocating storage for the node lists.
            INodeList nodeList = networkDef.NodeList;
            int nodeCount = nodeList.Count;
            int inputCount = networkDef.InputNodeCount + 1; // + to count bias as an input layer node.
            int outputCount = networkDef.OutputNodeCount;
            int hiddenCount = nodeCount - (inputCount + outputCount);
            IOGraph ioGraph = new IOGraph(inputCount, outputCount, hiddenCount, 0f, depthInfo._networkDepth);
            
            // We also build a dictionary of nodes keyed by innovation ID. This is used later
            // to assign connections to nodes.
            Dictionary<uint, GraphNode> nodeDict = new Dictionary<uint,GraphNode>(nodeCount);

            // Loop input nodes.
            int idx = 0;
            for(int i=0; i<inputCount; i++, idx++)
            {   // Create node, assign it a tag and add it to the node dictionary and the
                // input node list of the IOGraph.
                uint innovationId = nodeList[idx].Id;
                GraphNode node = new GraphNode(innovationId.ToString());
                node.AuxData = CreateGraphNodeAuxData(nodeList[idx]);
                node.Depth = depthInfo._nodeDepthArr[idx];
                nodeDict.Add(innovationId, node);
                ioGraph.InputNodeList.Add(node);
            }

            // Loop output nodes.
            for(int i=0; i<outputCount; i++, idx++)
            {   // Create node, assign it a tag and add it to the node dictionary and the
                // output node list of the IOGraph.
                uint innovationId = nodeList[idx].Id;
                GraphNode node = new GraphNode(innovationId.ToString());
                node.AuxData = CreateGraphNodeAuxData(nodeList[idx]);
                node.Depth = depthInfo._nodeDepthArr[idx];
                nodeDict.Add(innovationId, node);
                ioGraph.OutputNodeList.Add(node);
            }

            // Loop hidden nodes.
            for(; idx<nodeCount; idx++)
            {   // Create node, assign it a tag and add it to the node dictionary and the
                // hidden node list of the IOGraph.
                uint innovationId = nodeList[idx].Id;
                GraphNode node = new GraphNode(innovationId.ToString());
                node.AuxData = CreateGraphNodeAuxData(nodeList[idx]);
                node.Depth = depthInfo._nodeDepthArr[idx];
                nodeDict.Add(innovationId, node);
                ioGraph.HiddenNodeList.Add(node);
            }

            // Loop connections. Build GraphConnection objects and connect them to their source
            // and target nodes.
            double maxAbsWeight = 0.1; // Start at a non-zero value to prevent possibility of a divide by zero occurring.
            IConnectionList connectionList = networkDef.ConnectionList;
            int connCount = connectionList.Count;            
            for(int i=0; i<connCount; i++)
            {
                // Create connection object and assign its source and target nodes.
                INetworkConnection connection = connectionList[i];
                GraphNode sourceNode = nodeDict[connection.SourceNodeId];
                GraphNode targetNode = nodeDict[connection.TargetNodeId];
                GraphConnection conn = new GraphConnection(sourceNode, targetNode, (float)connection.Weight); 
    
                // Add the connection to the connection lists on the source and target nodes.
                sourceNode.OutConnectionList.Add(conn);
                targetNode.InConnectionList.Add(conn);

                // Track weight range over all connections.
                double absWeight = Math.Abs(connection.Weight);
                if(absWeight > maxAbsWeight) {
                    maxAbsWeight = absWeight;
                }
            }

            ioGraph.ConnectionWeightRange = (float)maxAbsWeight;
            return ioGraph;
        }


        /// <summary>
        /// Create auxiliary data for the specified INetworkNode.
        /// This version places the node activation function ID into element 0 of the aux data array.
        /// </summary>
        protected virtual object[] CreateGraphNodeAuxData(INetworkNode node)
        {   
            return new object[] {node.ActivationFnId};
        }
    }
}
