using System;

namespace SharpNeatLib.NeuralNetwork
{
	public struct FloatFastConnection
	{
		public int sourceNeuronIdx;
		public int targetNeuronIdx;
		public float weight;
		public float signal;
	}
}
