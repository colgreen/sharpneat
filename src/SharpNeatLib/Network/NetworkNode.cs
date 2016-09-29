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

namespace SharpNeat.Network
{
    /// <summary>
    /// Concrete implementation of an INetworkNode.
    /// </summary>
    public class NetworkNode : INetworkNode
    {
        readonly uint _id;
        readonly NodeType _nodeType;
        readonly int _activationFnId;
        readonly double[] _auxState;
        
        #region Constructor

        /// <summary>
        /// Constructs with the provided node ID, type and activation function ID.
        /// </summary>
        public NetworkNode(uint id, NodeType nodeType, int activationFnId)
        {
            _id = id;
            _nodeType = nodeType;
            _activationFnId = activationFnId;
            _auxState = null;
        }

        /// <summary>
        /// Constructs with the provided node ID, type, activation function ID and auxiliary state data.
        /// </summary>
        public NetworkNode(uint id, NodeType nodeType, int activationFnId, double[] auxState)
        {
            _id = id;
            _nodeType = nodeType;
            _activationFnId = activationFnId;
            _auxState = auxState;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the node's unique ID.
        /// </summary>
        public uint Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the node's type.
        /// </summary>
        public NodeType NodeType
        {
            get { return _nodeType; }
        }

        /// <summary>
        /// Gets the node's activation function ID. This is an ID into an IActivationFunctionLibrary
        /// associated with the network as a whole.
        /// </summary>
        public int ActivationFnId
        {
            get { return _activationFnId; }
        }

        /// <summary>
        /// Optional auxiliary node state. Null if no aux state is present. 
        /// Note. Radial Basis Function center and epsilon values are stored here.
        /// </summary>
        public double[] AuxState
        {
            get { return _auxState; }
        }

        #endregion
    }
}
