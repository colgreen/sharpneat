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

namespace SharpNeat.Phenomes.NeuralNets
{
    /// <summary>
    /// Stores a node and connection index that represent a layer within the network 
    /// (the nodes and connections at a given depth in a network).
    /// </summary>
    public struct LayerInfo
    {
        /// <summary>
        /// The index of the last node in the layer + 1.
        /// </summary>
        public int _endNodeIdx;
        /// <summary>
        /// The index of the last connection in the layer + 1.
        /// </summary>        
        public int _endConnectionIdx;
    }
}
