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

namespace SharpNeat.Decoders.HyperNeat
{
    /// <summary>
    /// Represenst a set of nodes on a substrate. Nodesets are used to represent sets such as the input and output nodes.
    /// Hidden nodes can be represented as layers with each layer being represented by a set. This allows connection mapping
    /// to be defined between between sets.
    /// </summary>
    public class SubstrateNodeSet
    {
        List<SubstrateNode> _nodePosList;

        #region Constructors

        /// <summary>
        /// Construct an empty nodeset. Node can be added after construction.
        /// </summary>
        public SubstrateNodeSet()
        {
            _nodePosList = new List<SubstrateNode>();
        }

        /// <summary>
        /// Construct an empty nodeset with an initial capacity. Node can be added after construction.
        /// </summary>
        public SubstrateNodeSet(int capacity)
        {
            _nodePosList = new List<SubstrateNode>(capacity);
        }

        /// <summary>
        /// Construct a nodeset with the provided list of nodes.
        /// </summary>
        public SubstrateNodeSet(List<SubstrateNode> nodePosList)
        {
            _nodePosList = nodePosList;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the nodes in the nodeset.
        /// </summary>
        public IList<SubstrateNode> NodeList
        {
            get { return _nodePosList; }
        }

        #endregion
    }
}
