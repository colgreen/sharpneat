using System;
using System.Collections;


namespace SharpNeatLib.NeatGenome
{
	public class NeuronGeneList : CollectionBase
	{
		static NeuronGeneComparer neuronGeneComparer = new NeuronGeneComparer();
		public bool OrderInvalidated=false;

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public NeuronGeneList()
		{}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="copyFrom"></param>
		public NeuronGeneList(NeuronGeneList copyFrom)
		{
			int count = copyFrom.Count;
			InnerList.Capacity = count;
			
			for(int i=0; i<count; i++)
				InnerList.Add(new NeuronGene(copyFrom[i]));

//			foreach(NeuronGene neuronGene in copyFrom)
//				InnerList.Add(new NeuronGene(neuronGene));
		}

		#endregion

		#region Indexer

		public NeuronGene this[int index]
		{
			get
			{
				return ((NeuronGene)InnerList[index]);
			}
			set
			{
				InnerList[index] = value;
			}
		}

		#endregion

		#region Public Methods

		public int Add(NeuronGene neuronGene)
		{
			return InnerList.Add(neuronGene);
		}

		public void Remove(NeuronGene neuronGene)
		{
			Remove(neuronGene.InnovationId);

			// This invokes a linear search. Invoke our binary search instead.
			//InnerList.Remove(neuronGene);
		}

		public void Remove(uint neuronId)
		{
			int idx = BinarySearch(neuronId);
			if(idx<0)
				throw new ApplicationException("Attempt to remove neuron with an unknown neuronId");
			else
				InnerList.RemoveAt(idx);

//			// Inefficient scan through the neuron list.
//			// TODO: Implement a binary search method for NeuronList (Will generics resolve this problem anyway?).
//			int bound = List.Count;
//			for(int i=0; i<bound; i++)
//			{
//				if(((NeuronGene)List[i]).InnovationId == neuronId)
//				{
//					InnerList.RemoveAt(i);
//					return;
//				}
//			}
//			throw new ApplicationException("Attempt to remove neuron with an unknown neuronId");
		}

		public NeuronGene GetNeuronById(uint neuronId)
		{
			int idx = BinarySearch(neuronId);
			if(idx<0)
				return null;
			else
				return (NeuronGene)InnerList[idx];

//			// Inefficient scan through the neuron list.
//			// TODO: Implement a binary search method for NeuronList (Will generics resolve this problem anyway?).
//			int bound = List.Count;
//			for(int i=0; i<bound; i++)
//			{
//				if(((NeuronGene)List[i]).InnovationId == neuronId)
//					return (NeuronGene)List[i];
//			}
//
//			// Not found;
//			return null;
		}

		public void SortByInnovationId()
		{
			InnerList.Sort(neuronGeneComparer);
			OrderInvalidated=false;
		}

		public int BinarySearch(uint innovationId) 
		{            
			int lo = 0;
			int hi = List.Count-1;

			while (lo <= hi) 
			{
				int i = (lo + hi) >> 1;

				if(((NeuronGene)InnerList[i]).InnovationId<innovationId)
					lo = i + 1;
				else if(((NeuronGene)InnerList[i]).InnovationId>innovationId)
					hi = i - 1;
				else
					return i;


				// TODO: This is wrong. It will fail for large innovation numbers because they are of type uint.
				// Fortunately it's very unlikely anyone has reached such large numbers!
//				int c = (int)((NeuronGene)InnerList[i]).InnovationId - (int)innovationId;
//				if (c == 0) return i;
//
//				if (c < 0) 
//					lo = i + 1;
//				else 
//					hi = i - 1;
			}
			
			return ~lo;
		}

		// For debug purposes only.
//		public bool IsSorted()
//		{
//			uint prevId=0;
//			foreach(NeuronGene gene in InnerList)
//			{
//				if(gene.InnovationId<prevId)
//					return false;
//				prevId = gene.InnovationId;
//			}
//			return true;
//		}

		#endregion
	}
}
