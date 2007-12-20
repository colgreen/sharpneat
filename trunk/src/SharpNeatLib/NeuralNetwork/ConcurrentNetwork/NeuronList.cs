using System;
using System.Collections;

namespace SharpNeatLib.NeuralNetwork
{

	public class NeuronList : CollectionBase
	{
		public Neuron this[int index]
		{
			get
			{
				return ((Neuron)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(Neuron neuron)
		{
			return (List.Add(neuron));
		}
	}
}
