using System;

namespace SharpNeatLib.NeuralNetwork
{
	public struct IntegerFastConnection
	{
		public int sourceNeuronIdx;
		public int targetNeuronIdx;
		public int weight;
		public int signal;
	}
}
