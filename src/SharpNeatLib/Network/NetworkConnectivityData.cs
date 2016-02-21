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

namespace SharpNeat.Network
{
    /// <summary>
    /// Network connectivity data for a network. Connectivity data in a form that is
    /// convenient for network traversal algorithms.
    /// </summary>
    public class NetworkConnectivityData
    {
        NodeConnectivityData[] _nodeConnectivityDataArr;
        Dictionary<uint,NodeConnectivityData> _nodeConnectivityDataById;

        #region Constructor

        /// <summary>
        /// Construct with the provided conenctivity data.
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
