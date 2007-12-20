using System;

namespace SharpNeatLib.NeuralNetwork
{
	/// <summary>
	/// Summary description for FastSigmoid.
	/// </summary>
	public class NullFn : IActivationFunction
	{
		public double Calculate(double inputSignal)
		{
			return 0.0;
		}

		public float Calculate(float inputSignal)
		{
			return 0.0F;
		}

		/// <summary>
		/// Unique ID. Stored in network XML to identify which function network the network is supposed to use.
		/// </summary>
		public string FunctionId
		{
			get
			{
				return this.GetType().Name;
			}
		}

		/// <summary>
		/// The function as a string in a platform agnostic form. For documentation purposes only, this isn;t actually compiled!
		/// </summary>
		public string FunctionString
		{
			get
			{
				return "";
			}
		}


		/// <summary>
		/// A human readable / verbose description of the activation function.
		/// </summary>
		public string FunctionDescription
		{
			get
			{
				return "";
			}
		}
	}
}
