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
namespace SharpNeat.Decoders.HyperNeat
{
    /// <summary>
    /// Represents a connection between two nodes in a HyperNEAT substrate.
    /// The node positions are represented as arrays of numbers so as not to limit the number of
    /// dimensions that positions (and therefore substrates) can be defined within.
    /// </summary>
    public class SubstrateConnection
    {
        /// <summary>The source node.</summary>
        public SubstrateNode _srcNode;
        /// <summary>The target node.</summary>
        public SubstrateNode _tgtNode;

        /// <summary>
        /// Constructs with the specified source and target substrate nodes.
        /// </summary>
        public SubstrateConnection(SubstrateNode srcNode, SubstrateNode tgtNode)
        {
            _srcNode = srcNode;
            _tgtNode = tgtNode;
        }
    }
}
