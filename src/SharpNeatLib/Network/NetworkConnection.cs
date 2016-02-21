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
