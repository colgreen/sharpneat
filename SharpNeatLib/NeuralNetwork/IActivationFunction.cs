using System;

namespace SharpNeatLib.NeuralNetwork
{
	public interface IActivationFunction
	{
		double Calculate(double inputSignal);

		/// <summary>
		/// A float equivalent should be implemented as this provides approx. a 60% speed boost
		/// in the right circumstances. Partly through not having to cast to/from double and partly
		/// because floats are [sometimes] faster to calculate. They are also small and require less
		/// memory bus bandwidth and CPU cache.
		/// </summary>
		/// <param name="inputSignal"></param>
		/// <returns></returns>
		float Calculate(float inputSignal);

		/// <summary>
		/// Unique ID. Stored in network XML to identify which function network the network is supposed to use.
		/// </summary>
		string FunctionId
		{
			get;
		}

		/// <summary>
		/// The function as a string in a platform agnostic form. For documentation purposes only, this isn;t actually compiled!
		/// </summary>
		string FunctionString
		{
			get;
		}

		/// <summary>
		/// A human readable / verbose description of the activation function.
		/// </summary>
		string FunctionDescription
		{
			get;
		}
	}
}
