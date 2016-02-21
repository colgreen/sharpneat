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
    /// Conveys summary information from a network depth analysis .
    /// </summary>
    public struct NetworkDepthInfo
    {
        /// <summary>
        /// Indicates the total depth of the network.
        /// This is the highest value within _nodeDepths + 1 (because the first layer is layer 0)
        /// </summary>
        public readonly int _networkDepth;
        /// <summary>
        /// An array containing the depth of each node in the network 
        /// (indexed by position within the analysed INodeList).
        /// </summary>
        public readonly int[] _nodeDepthArr;

        /// <summary>
        /// Construct with the provided info.
        /// </summary>
        public NetworkDepthInfo(int networkDepth, int[] nodeDepthArr)
        {
            _networkDepth = networkDepth;
            _nodeDepthArr = nodeDepthArr;
        }
    }
}
