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
        /// Optional auxilliary node state. Null if no aux state is present. 
        /// Note. Radial Basis Function center and epsilon values are stored here.
        /// </summary>
        public double[] AuxState
        {
            get { return _auxState; }
        }

        #endregion
    }
}
