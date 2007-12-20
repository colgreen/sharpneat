using System;
using SharpNeatLib.NeuralNetwork;

namespace SharpNeatLib.Experiments
{
	/// <summary>
	/// This structure is used by INetworkPairEvaluator. Using such a structure 
	/// may be slightly more efficient than a 2 element array, and also less
	/// not prone to index out of bound errors.
	/// </summary>
	public struct FitnessPair
	{
		public double fitness1;
		public double fitness2;
	}

	/// <summary>
	/// An interface that defines a method for evaluating a pair of networks.
	/// This interface is a useful abstraction for certain types of 
	/// co-evolution experiment where networks are evaluated by comparing
	/// against a set of other networks, one at a time - in pairs.
	/// </summary>
	public interface INetworkPairEvaluator
	{
		FitnessPair EvaluateNetworkPair(INetwork net1, INetwork net2);
	}
}
