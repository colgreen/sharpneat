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
    /// Concrete implementation of INodeList.
    /// </summary>
    public class NodeList : List<INetworkNode>, INodeList
    {
        #region Constructors

        /// <summary>
        /// Constructs an empty list.
        /// </summary>
        public NodeList()
        {
        }

        /// <summary>
        /// Constructs a list with the specified initial capacity.
        /// </summary>
        public NodeList(int capacity) : base(capacity)
        {
        }

        #endregion

        #region INodeList Members

        /// <summary>
        /// Gets the index of the INetworkNode with the specified ID. 
        /// Uses a binary search for fast searching, however this assumes the nodes are sorted by
        /// ID in ascending order.
        /// </summary>
        public int BinarySearch(uint id) 
        {            
            int lo = 0;
            int hi = Count-1;

            while (lo <= hi) 
            {
                int i = (lo + hi) >> 1;

                if(this[i].Id < id) {
                    lo = i + 1;
                }
                else if(this[i].Id > id) {
                    hi = i - 1;
                }
                else {
                    return i;
                }
            }
            
            return ~lo;
        }

        /// <summary>
        /// Indicates if the nodes are sorted by ID in ascending order, as required by BinarySearch().
        /// For debug purposes only. Don't call this method in normal circumstances as it is an
        /// expensive O(n) operation.
        /// </summary>
        public bool IsSorted()
        {
            int count = this.Count;
            if(0 == count) {
                return true;
            }

            uint prev = this[0].Id;
            for(int i=1; i<count; i++)
            {
                if(this[i].Id <= prev) {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
