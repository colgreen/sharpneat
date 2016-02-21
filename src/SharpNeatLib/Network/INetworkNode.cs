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
    /// Represents a single network node.
    /// Part of the INetworkDefinition type hierarchy.
    /// </summary>
    public interface INetworkNode
    {
        /// <summary>
        /// Gets the node's unique ID.
        /// </summary>
        uint Id { get; }
        /// <summary>
        /// Gets the node's type.
        /// </summary>
        NodeType NodeType { get; }
        /// <summary>
        /// Gets the node's activation function ID. This is an ID into an IActivationFunctionLibrary
        /// associated with the network as a whole.
        /// </summary>
        int ActivationFnId { get; }
        /// <summary>
        /// Optional auxilliary node state. Null if no aux state is present. 
        /// Note. Radial Basis Function center and epsilon values are stored here.
        /// </summary>
        double[] AuxState { get; }
    }
}
