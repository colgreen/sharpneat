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
using System.Collections.Generic;

namespace SharpNeat.Network
{
    /// <summary>
    /// Network connectivity data for a network. Connectivity data in a form that is
    /// convenient for network traversal algorithms.
    /// </summary>
    public class NetworkConnectivityData
    {
        readonly NodeConnectivityData[] _nodeConnectivityDataArr;
        readonly Dictionary<uint,NodeConnectivityData> _nodeConnectivityDataById;

        #region Constructor

        /// <summary>
        /// Construct with the provided connectivity data.
        /// </summary>
        public NetworkConnectivityData(NodeConnectivityData[] nodeConnectivityDataArr,
                                       Dictionary<uint,NodeConnectivityData> nodeConnectivityDataById)
        {
            _nodeConnectivityDataArr = nodeConnectivityDataArr;
            _nodeConnectivityDataById = nodeConnectivityDataById;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the NodeConnectivityData for the node with the specified index in its host network definition.
        /// </summary>
        public NodeConnectivityData GetNodeDataByIndex(int idx)
        {
            return _nodeConnectivityDataArr[idx];
        }

        /// <summary>
        /// Get the NodeConnectivityData for the node with the specified ID.
        /// </summary>
        public NodeConnectivityData GetNodeDataById(uint id)
        {
            return _nodeConnectivityDataById[id];
        }

        #endregion
    }
}
