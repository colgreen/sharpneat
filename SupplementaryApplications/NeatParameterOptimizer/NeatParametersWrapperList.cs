using System;
using System.Collections;

using SharpNeatLib;

namespace NeatParameterOptimizer
{
	public class NeatParametersWrapperList : CollectionBase
	{
		#region Class Variables

		NeatParametersWrapperComparer comparer = new NeatParametersWrapperComparer();
		double selectionTotalFitness;
		double meanFitness;

		#endregion

		#region Properties

		public double SelectionTotalFitness
		{
			get
			{
				return selectionTotalFitness;
			}	
			set
			{
				selectionTotalFitness = value;
			}
		}

		public double MeanFitness
		{
			get
			{
				return meanFitness;
			}
		}

		#endregion

		#region Indexer

		public NeatParametersWrapper this[int index]
		{
			get
			{
				return ((NeatParametersWrapper)InnerList[index]);
			}
			set
			{
				InnerList[index] = value;
			}
		}

		#endregion

		#region Public Methods

		public int Add(NeatParametersWrapper neatParametersWrapper)
		{
			return (InnerList.Add(neatParametersWrapper));
		}

		public void Remove(NeatParametersWrapper neatParametersWrapper)
		{
			InnerList.Remove(neatParametersWrapper);
		}

		public void RemoveRange(int index, int count)
		{
			InnerList.RemoveRange(index, count);
		}

		public void Sort()
		{
			// Allocate a random number to each npw - this will randomize the order of npw's with
			// equal fitness.
			foreach(NeatParametersWrapper npw in InnerList)
				npw.OrderRandomizer = (int)(Utilities.NextDouble() * 10000.0);

			// Sort the list.
			InnerList.Sort(comparer);
		}

		public void UpdateStats()
		{
			double totalFitness=0.0;
			foreach(NeatParametersWrapper npw in InnerList)
				totalFitness += npw.Fitness;
			
			meanFitness = totalFitness / InnerList.Count;
		}

		#endregion
	}
}
