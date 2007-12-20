using System;
using System.Collections;

namespace SharpNeatLib.NetworkVisualization
{

	public class ModelNeuronList : CollectionBase
	{
		public ModelNeuron this[int index]
		{
			get
			{
				return ((ModelNeuron)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(ModelNeuron neuron)
		{
			return (List.Add(neuron));
		}

		public void Sort()
		{
			InnerList.Sort();
		}
	}
}
