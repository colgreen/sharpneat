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

using System;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Network;

namespace SharpNeat.Genomes.Neat
{
    // ENHANCEMENT: Consider switching to a SortedList[K,V] - which guarantees item sort order at all times. 

    /// <summary>
    /// Represents a sorted list of ConnectionGene objects. The sorting of the items is done on request
    /// rather than being strictly enforced at all times (e.g. as part of adding and removing genes). This
    /// approach is currently more convenient for use in some of the routines that work with NEAT genomes.
    /// 
    /// Because we are not using a strictly sorted list such as the generic class SortedList[K,V] a customised 
    /// BinarySearch() method is provided for fast lookup of items if the list is known to be sorted. If the list is
    /// not sorted then the BinarySearch method's behaviour is undefined. This is potentially a source of bugs 
    /// and thus this class should probably migrate to SortedList[K,V] or be modified to ensure items are sorted 
    /// prior to a binary search.
    /// 
    /// Sort order is with respect to connection gene innovation ID.
    /// </summary>
    public class ConnectionGeneList : List<ConnectionGene>, IConnectionList
    {
        static readonly ConnectionGeneComparer __connectionGeneComparer = new ConnectionGeneComparer();

        #region Constructors

        /// <summary>
        /// Construct an empty list.
        /// </summary>
        public ConnectionGeneList()
        {
        }

        /// <summary>
        /// Construct an empty list with the specified capacity.
        /// </summary>
        public ConnectionGeneList(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// Copy constructor. The newly allocated list has a capacity 2 larger than copyFrom
        /// allowing addition mutations to occur without reallocation of memory.
        /// Note that a single add node mutation adds two connections and a single
        /// add connection mutation adds one.
        /// </summary>
        public ConnectionGeneList(ICollection<ConnectionGene> copyFrom) : base(copyFrom.Count + 2)
        {
            // ENHANCEMENT: List.Foreach() is potentially faster then a foreach loop. 
            // http://diditwith.net/2006/10/05/PerformanceOfForeachVsListForEach.aspx
            foreach(ConnectionGene srcGene in copyFrom) {
                Add(srcGene.CreateCopy());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts a ConnectionGene into its correct (sorted) location within the gene list.
        /// Normally connection genes can safely be assumed to have a new Innovation ID higher
        /// than all existing IDs, and so we can just call Add().
        /// This routine handles genes with older IDs that need placing correctly.
        /// </summary>
        public void InsertIntoPosition(ConnectionGene connectionGene)
        {
            // Determine the insert idx with a linear search, starting from the end 
            // since mostly we expect to be adding genes that belong only 1 or 2 genes
            // from the end at most.
            int idx=Count-1;
            for(; idx > -1; idx--)
            {
                if(this[idx].InnovationId < connectionGene.InnovationId)
                {   // Insert idx found.
                    break;
                }
            }
            Insert(idx+1, connectionGene);
        }

        /// <summary>
        /// Remove the connection gene with the specified innovation ID.
        /// </summary>
        public void Remove(uint innovationId)
        {
            int idx = BinarySearch(innovationId);
            if(idx<0) {
                throw new ApplicationException("Attempt to remove connection with an unknown innovationId");
            } 
            RemoveAt(idx);
        }

        /// <summary>
        /// Sort connection genes into ascending order by their innovation IDs.
        /// </summary>
        public void SortByInnovationId()
        {
            Sort(__connectionGeneComparer);
        }

        /// <summary>
        /// Obtain the index of the gene with the specified innovation ID by performing a binary search.
        /// Binary search is fast and can be performed so long as we know the genes are sorted by innovation ID.
        /// If the genes are not sorted then the behaviour of this method is undefined.
        /// </summary>
        public int BinarySearch(uint innovationId) 
        {            
            int lo = 0;
            int hi = Count-1;

            while (lo <= hi) 
            {
                int i = (lo + hi) >> 1;

                // Note. we don't calculate this[i].InnovationId-innovationId because we are dealing with uint.
                // ENHANCEMENT: List<T>[i] invokes a bounds check on each call. Can we avoid this?
                if(this[i].InnovationId < innovationId) {
                    lo = i + 1;
                } else if(this[i].InnovationId > innovationId) {
                    hi = i - 1;
                } else {
                    return i;
                }
            }

            return ~lo;
        }

        /// <summary>
        /// Resets the IsMutated flag on all ConnectionGenes in the list.
        /// </summary>
        public void ResetIsMutatedFlags()
        {
            int count = this.Count;
            for(int i=0; i<count; i++) {
                this[i].IsMutated = false;
            }
        }

        /// <summary>
        /// For debug purposes only. Don't call this method in normal circumstances as it is an
        /// expensive O(n) operation.
        /// </summary>
        public bool IsSorted()
        {
            int count = this.Count;
            if(0 == count) {
                return true;
            }

            uint prev = this[0].InnovationId;
            for(int i=1; i<count; i++)
            {
                if(this[i].InnovationId <= prev) {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region IConnectionList Members

        INetworkConnection IConnectionList.this[int index]
        {
            get { return this[index]; }
        }

        int IConnectionList.Count
        {
            get { return this.Count; }
        }

        IEnumerator<INetworkConnection> IEnumerable<INetworkConnection>.GetEnumerator()
        {
            foreach(ConnectionGene gene in this) {
                yield return gene;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<INetworkConnection>)this).GetEnumerator();
        }

        #endregion
    }
}
