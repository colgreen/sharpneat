using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Network2;

namespace SharpNeat.Neat.Genome
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
    public class ConnectionGeneList : List<ConnectionGene>, IList<IWeightedDirectedConnection<double>>
    {
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

        #endregion

        #region Properties

        /// <summary>
        /// Get the innovation ID of the last connection gene.
        /// </summary>
        /// <remarks>he genes are sorted by innovation ID, therefore the last ID is also has the highest value.</remarks>
        public uint LastInnovationId
        {
            get { return this[this.Count-1].Id; }
        }



        #endregion

        #region Public Methods

        // TODO: Clean up.

        ///// <summary>
        ///// Inserts a ConnectionGene into its correct (sorted) location within the gene list.
        ///// Normally connection genes can safely be assumed to have a new Innovation ID higher
        ///// than all existing IDs, and so we can just call Add().
        ///// This routine handles genes with older IDs that need placing correctly.
        ///// </summary>
        //public void InsertIntoPosition(ConnectionGene connectionGene)
        //{
        //    // Determine the insert idx with a linear search, starting from the end 
        //    // since mostly we expect to be adding genes that belong only 1 or 2 genes
        //    // from the end at most.
        //    int idx=Count-1;
        //    for(; idx > -1; idx--)
        //    {
        //        if(this[idx].InnovationId < connectionGene.InnovationId)
        //        {   // Insert idx found.
        //            break;
        //        }
        //    }
        //    Insert(idx+1, connectionGene);
        //}

        ///// <summary>
        ///// Remove the connection gene with the specified innovation ID.
        ///// </summary>
        //public void Remove(uint innovationId)
        //{
        //    int idx = BinarySearch(innovationId);
        //    if(idx<0) {
        //        throw new ApplicationException("Attempt to remove connection with an unknown innovationId");
        //    } 
        //    RemoveAt(idx);
        //}

        ///// <summary>
        ///// Sort connection genes into ascending order by their innovation IDs.
        ///// </summary>
        //public void SortByInnovationId()
        //{
        //    Sort(delegate(ConnectionGene x, ConnectionGene y)
        //        {
        //            // Test the most likely cases first.
        //            if (x.InnovationId < y.InnovationId) {
        //                return -1;
        //            } 
        //            if (x.InnovationId > y.InnovationId) {
        //                return 1;
        //            } 
        //            return 0;
        //        });
        //}

        /// <summary>
        /// Returns true of the given innovation ID exists within the connection gene list.
        /// </summary>
        /// <param name="innovationId"></param>
        /// <returns></returns>
        public bool ContainsInnovationId(uint innovationId) {
            return BinarySearch(innovationId) >= 0;
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
                if(this[i].Id < innovationId) {
                    lo = i + 1;
                } else if(this[i].Id > innovationId) {
                    hi = i - 1;
                } else {
                    return i;
                }
            }

            return ~lo;
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

        #region IList<IWeightedDirectedConnection>

        public bool IsReadOnly => true;

        IWeightedDirectedConnection<double> IList<IWeightedDirectedConnection<double>>.this[int index] 
        { 
            get => this[index];
            set => throw new NotImplementedException(); 
        }

        public int IndexOf(IWeightedDirectedConnection<double> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IWeightedDirectedConnection<double> item)
        {
            throw new NotImplementedException();
        }

        public void Add(IWeightedDirectedConnection<double> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IWeightedDirectedConnection<double> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IWeightedDirectedConnection<double>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IWeightedDirectedConnection<double> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<IWeightedDirectedConnection<double>> IEnumerable<IWeightedDirectedConnection<double>>.GetEnumerator()
        {
            for(int i=0; i<this.Count; i++) {
                yield return this[i];
            }
        }

        #endregion
    }
}
