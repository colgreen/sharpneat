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
    /// Concrete implementation of INetworkConnection.
    /// </summary>
    public class NetworkConnection : INetworkConnection
    {
        readonly uint _sourceNodeId;
        readonly uint _targetNodeId;
        readonly double _weight;

        #region Constructor

        /// <summary>
        /// Constructs with the provided source and target node IDs and weight.
        /// </summary>
        public NetworkConnection(uint sourceNodeId, uint targetNodeId, double weight)
        {
            _sourceNodeId = sourceNodeId;
            _targetNodeId = targetNodeId;
            _weight = weight;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of the connection's source node.
        /// </summary>
        public uint SourceNodeId
        {
            get { return _sourceNodeId; }
        }

        /// <summary>
        /// Gets the ID of the connection's target node.
        /// </summary>
        public uint TargetNodeId
        {
            get { return _targetNodeId; }
        }

        /// <summary>
        /// Gets the connection's weight.
        /// </summary>
        public double Weight
        {
            get { return _weight; }
        }

        #endregion
    }
}
