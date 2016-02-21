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
    /// Abstracted representation of a network definition.
    /// This interface and the related types INodeList, IConnectionList, INetworkNode,
    /// INetworkConnection, etc, allow networks to be described abstractly. 
    /// 
    /// One significant use of this class is in the decoding of genome classes into concrete 
    /// network instances; The decode methods can be written to operate on INetworkDefinition
    /// rather than specific genome types.
    /// </summary>
    public interface INetworkDefinition
    {
        /// <summary>
        /// Gets the number of input nodes. This does not include the bias node which is always present.
        /// </summary>
        int InputNodeCount { get; }
        /// <summary>
        /// Gets the number of output nodes.
        /// </summary>
        int OutputNodeCount { get; }
        /// <summary>
        /// Gets a bool flag that indicates if the network is acyclic.
        /// </summary>
        bool IsAcyclic { get; }
        /// <summary>
        /// Gets the network's activation function library. The activation function at each node is 
        /// represented by an integer ID, which refers to a function in this activation function library.
        /// </summary>
        IActivationFunctionLibrary ActivationFnLibrary { get; }
        /// <summary>
        /// Gets the list of network nodes.
        /// </summary>
        INodeList NodeList { get; }
        /// <summary>
        /// Gets the list of network connections.
        /// </summary>
        IConnectionList ConnectionList { get; }
        /// <summary>
        /// Gets NetworkConnectivityData for the network.
        /// </summary>
        NetworkConnectivityData GetConnectivityData();
    }
}
