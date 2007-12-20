using System;
using System.Collections;

namespace SharpNeatLib.Evolution
{

	public class GenomeList : CollectionBase
	{
		static GenomeComparer genomeComparer = new GenomeComparer();
		static PruningModeGenomeComparer pruningModeGenomeComparer = new PruningModeGenomeComparer();

		public IGenome this[int index]
		{
			get
			{
				return ((IGenome)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(IGenome genome)
		{
			return (List.Add(genome));
		}

		public void AddRange(ICollection c)
		{
			InnerList.AddRange(c);
		}

		public void Sort()
		{
			InnerList.Sort(genomeComparer);
		}

		/// <summary>
		/// This perfroms a secondary sort on genome size (ascending order), so that small genomes
		/// are more likely to be selected thus aiding a pruning phase.
		/// </summary>
		public void Sort_PruningMode()
		{
			InnerList.Sort(pruningModeGenomeComparer);
		}

		public void Reverse()
		{
			InnerList.Reverse();
		}

		public void RemoveRange(int index, int count)
		{
			InnerList.RemoveRange(index, count);
		}

		public void Remove(IGenome genome)
		{
			// TODO: remove use of this method - it is inefficient!
			InnerList.Remove(genome);
		}

	}
}
