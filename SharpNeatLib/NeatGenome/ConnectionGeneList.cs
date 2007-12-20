using System;
using System.Collections;
using System.Diagnostics;


namespace SharpNeatLib.NeatGenome
{
	public class ConnectionGeneList : CollectionBase
	{
		static ConnectionGeneComparer connectionGeneComparer = new ConnectionGeneComparer();
		//public bool OrderInvalidated=false;

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConnectionGeneList()
		{}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="copyFrom"></param>
		public ConnectionGeneList(ConnectionGeneList copyFrom)
		{
			int count = copyFrom.Count;
			InnerList.Capacity = count;

			for(int i=0; i<count; i++)
				InnerList.Add(new ConnectionGene(copyFrom[i]));

			Debug.Assert(copyFrom.IsSorted(), "ConnectionGeneList is not sorted by innovation ID");
		}

		#endregion

		#region Indexer

		public ConnectionGene this[int index]
		{
			get
			{
				return ((ConnectionGene)InnerList[index]);
			}
//			set
//			{
//				InnerList[index] = value;
//			}
		}

		#endregion

		#region Public Methods

		public int Add(ConnectionGene connectionGene)
		{
			// Ensure we aren't inserting a ConnectionGene out of order.
			Debug.Assert(InnerList.Count==0 || ((ConnectionGene)InnerList[Count-1]).InnovationId < connectionGene.InnovationId, "Attempt to break ConnectionGeneList sort order.");
			return (InnerList.Add(connectionGene));
		}

		/// <summary>
		/// Inserts a ConnectionGene into its correct (sorted) location within the gene list.
		/// Normally connection genes can safely be assumed to have a new Innovation ID higher
		/// than all existing ID's, and so we can just call Add().
		/// This routine handles genes with older ID's that need placing correctly.
		/// </summary>
		/// <param name="connectionGene"></param>
		/// <returns></returns>
		public void InsertIntoPosition(ConnectionGene connectionGene)
		{
			// Determine the insert idx with a linear search, starting from the end 
			// since mostly we expect to be adding genes that belong only 1 or 2 genes
			// from the end at most.
			int idx=InnerList.Count-1;
			for(; idx>-1; idx--)
			{
				if(((ConnectionGene)InnerList[idx]).InnovationId < connectionGene.InnovationId)
				{	// Insert idx found.
					break;
				}
			}

			InnerList.Insert(idx+1, connectionGene);
		}

		public void Remove(ConnectionGene connectionGene)
		{
			Remove(connectionGene.InnovationId);

			// This invokes a linear search. Invoke our binary search instead.
			//InnerList.Remove(connectionGene);
		}

		public void Remove(uint innovationId)
		{
			int idx = BinarySearch(innovationId);
			if(idx<0)
				throw new ApplicationException("Attempt to remove connection with an unknown innovationId");
			else
				InnerList.RemoveAt(idx);
		}

		public void SortByInnovationId()
		{
			InnerList.Sort(connectionGeneComparer);
			//OrderInvalidated=false;
		}

		public int BinarySearch(uint innovationId) 
		{            
			int lo = 0;
			int hi = List.Count-1;

			while (lo <= hi) 
			{
				int i = (lo + hi) >> 1;
				int c = (int)((ConnectionGene)InnerList[i]).InnovationId - (int)innovationId;
				if (c == 0) return i;

				if (c < 0) 
					lo = i + 1;
				else 
					hi = i - 1;
			}
			
			return ~lo;
		}

		/// <summary>
		/// For debug purposes only. Don't call this in normal circumstances as it is an
		/// expensive O(n) operation.
		/// </summary>
		/// <returns></returns>
		public bool IsSorted()
		{
			uint prevId=0;
			foreach(ConnectionGene gene in InnerList)
			{
				if(gene.InnovationId<prevId)
					return false;
				prevId = gene.InnovationId;
			}
			return true;
		}

		#endregion
	}
}
