using System;
using System.Collections;

namespace SharpNeatLib.NeuralNetwork
{

	public class NetworkList : CollectionBase
	{
		public INetwork this[int index]
		{
			get
			{
				return ((INetwork)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(INetwork network)
		{
			return (List.Add(network));
		}

	}
}
