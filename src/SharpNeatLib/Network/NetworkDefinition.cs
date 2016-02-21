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
using System.Diagnostics;

namespace SharpNeat.Network
{
    /// <summary>
    /// Concrete implementation of INetworkDefinition.
    /// This class represents network definitions independently of any other requirements, e.g.
    /// a NeatGenome is also an INetworkDefinition but with additional baggage. This class
    /// is useful for representing network definitions, e.g. when creating a network instance from 
    /// a HyperNEAT substrate.
    /// </summary>
    public class NetworkDefinition : INetworkDefinition
    {
        readonly int _inputNodeCount;
        readonly int _outputNodeCount;
        readonly bool _isAcyclic = false;
        readonly IActivationFunctionLibrary _activationFnLib;
        readonly NodeList _nodeList;
        readonly ConnectionList _connectionList;

        // Created in a just-in-time manner and cached for possible re-use.
        NetworkConnectivityData _networkConnectivityData;

        #region Constructors

        /// <summary>
        /// Constructs with the provided input/output node count, activation function library, 
        /// node and connection lists.
        /// </summary>
        public NetworkDefinition(int inputNodeCount, int outputNodeCount,
                                 IActivationFunctionLibrary activationFnLib,
                                 NodeList nodeList, ConnectionList connectionList)
        {
            _inputNodeCount = inputNodeCount;
            _outputNodeCount = outputNodeCount;
            _activationFnLib = activationFnLib;
            _nodeList = nodeList;
            _connectionList = connectionList;
            _isAcyclic = !CyclicNetworkTest.IsNetworkCyclic(this);
        }

        /// <summary>
        /// Constructs with the provided input/output node count, activation function library, 
        /// node and connection lists.
        /// </summary>
        public NetworkDefinition(int inputNodeCount, int outputNodeCount,
                                 IActivationFunctionLibrary activationFnLib,
                                 NodeList nodeList, ConnectionList connectionList,
                                 bool isAcyclic)
        {
            _inputNodeCount = inputNodeCount;
            _outputNodeCount = outputNodeCount;
            _activationFnLib = activationFnLib;
            _nodeList = nodeList;
            _connectionList = connectionList;
            _isAcyclic = isAcyclic;

            Debug.Assert(isAcyclic == !CyclicNetworkTest.IsNetworkCyclic(this));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of input nodes. This does not include the bias node which is always present.
        /// </summary>
        public int InputNodeCount
        {
            get { return _inputNodeCount; }
        }

        /// <summary>
        /// Gets the number of output nodes.
        /// </summary>
        public int OutputNodeCount
        {
            get { return _outputNodeCount; }
        }

        /// <summary>
        /// Gets a bool flag that indicates if the network is acyclic.
        /// </summary>
        public bool IsAcyclic 
        { 
            get { return _isAcyclic;  }
        }

        /// <summary>
        /// Gets the network's activation function library. The activation function at each node is represented
        /// by an integer ID, which refers to a function in this library.
        /// </summary>
        public IActivationFunctionLibrary ActivationFnLibrary
        {
            get { return _activationFnLib; }
        }

        /// <summary>
        /// Gets the list of network nodes.
        /// </summary>
        public INodeList NodeList
        {
            get { return _nodeList; }
        }

        /// <summary>
        /// Gets the list of network connections.
        /// </summary>
        public IConnectionList ConnectionList
        {
            get { return _connectionList; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets NetworkConnectivityData for the network.
        /// </summary>
        public NetworkConnectivityData GetConnectivityData()
        {
            if(null != _networkConnectivityData) {
                return _networkConnectivityData;
            }

            int nodeCount = _nodeList.Count;
            NodeConnectivityData[] nodeConnectivityDataArr = new NodeConnectivityData[nodeCount];
            Dictionary<uint,NodeConnectivityData> nodeConnectivityDataById = new Dictionary<uint,NodeConnectivityData>(nodeCount);

            // Instantiate NodeConnectivityData for each node.
            for(int i=0; i<nodeCount; i++)
            {
                uint nodeId = _nodeList[i].Id;
                NodeConnectivityData ncd = new NodeConnectivityData(nodeId);
                nodeConnectivityDataArr[i] = ncd;
                nodeConnectivityDataById.Add(nodeId, ncd);
            }

            // Loop connections and register them with the source and target nodes.
            int conCount = _connectionList.Count;
            for(int i=0; i<conCount; i++)
            {
                INetworkConnection conn = _connectionList[i];
                NodeConnectivityData srcNodeData = nodeConnectivityDataById[conn.SourceNodeId];
                NodeConnectivityData tgtNodeData = nodeConnectivityDataById[conn.TargetNodeId];
                srcNodeData._tgtNodes.Add(conn.TargetNodeId);
                tgtNodeData._srcNodes.Add(conn.SourceNodeId);
            }

            _networkConnectivityData = new NetworkConnectivityData(nodeConnectivityDataArr, nodeConnectivityDataById);
            return _networkConnectivityData;
        }

        #endregion

        #region Public Methods [Debugging]

        /// <summary>
        /// Performs an integrity check on the network definition.
        /// Returns true if OK.
        /// </summary>
        public bool PerformIntegrityCheck()
        {
            // We will always have at least a bias and an output.
            int count = _nodeList.Count;
            if(count < 2) {
                Debug.WriteLine(string.Format("Node list has less than the minimum number of neuron genes [{0}]", count));
                return false;
            }

            // Check bias neuron.
            if(NodeType.Bias != _nodeList[0].NodeType) {
                Debug.WriteLine("Missing bias gene");
                return false;
            }

            if(0u != _nodeList[0].Id) {
                Debug.WriteLine(string.Format("Bias neuron ID != 0. [{0}]",  _nodeList[0].Id));
                return false;
            }

            // Check input nodes.
            uint prevId = 0u;
            int idx = 1;
            for(int i=0; i<_inputNodeCount; i++, idx++)
            {
                if(NodeType.Input != _nodeList[idx].NodeType) {
                    Debug.WriteLine(string.Format("Invalid node type. Expected Input, got [{0}]", _nodeList[idx].NodeType));
                    return false;
                }

                if(_nodeList[idx].Id <= prevId) {
                    Debug.WriteLine("Input node is out of order and/or a duplicate.");
                    return false;
                }

                prevId = _nodeList[idx].Id;
            }

            // Check output neurons.
            for(int i=0; i<_outputNodeCount; i++, idx++)
            {
                if(NodeType.Output != _nodeList[idx].NodeType) {
                    Debug.WriteLine(string.Format("Invalid node type. Expected Output, got [{0}]", _nodeList[idx].NodeType));
                    return false;
                }

                if(_nodeList[idx].Id <= prevId) {
                    Debug.WriteLine("Output node is out of order and/or a duplicate.");
                    return false;
                }

                prevId = _nodeList[idx].Id;
            }

            // Check hidden neurons.
            // All remaining neurons should be hidden neurons.
            for(; idx<count; idx++)
            {
                if(NodeType.Hidden != _nodeList[idx].NodeType) {
                    Debug.WriteLine(string.Format("Invalid node type. Expected Hidden, got [{0}]", _nodeList[idx].NodeType));
                    return false;
                }

                if(_nodeList[idx].Id <= prevId) {
                    Debug.WriteLine("Hidden node is out of order and/or a duplicate.");
                    return false;
                }

                prevId = _nodeList[idx].Id;
            }

            // Check connections.
            count = _connectionList.Count;
            if(0 == count) 
            {   // At least one connection is required, connectionless genomes are pointless.
                Debug.WriteLine("Zero connections.");
                return false;
            }

            // Check for multiple connections between nodes.
            // Build a dictionary of connection endpoints.
            Dictionary<ConnectionEndpointsStruct, object> endpointDict = new Dictionary<ConnectionEndpointsStruct,object>(count);
            foreach(NetworkConnection conn in _connectionList)
            {
                ConnectionEndpointsStruct key = new ConnectionEndpointsStruct(conn.SourceNodeId, conn.TargetNodeId);
                if(endpointDict.ContainsKey(key))
                {
                    Debug.WriteLine("Connection error. A connection between the specified endpoints already exists.");
                    return false;
                }
                endpointDict.Add(key, null);
            }

            // Integrity check OK.
            return true;
        }

        #endregion
    }
}
