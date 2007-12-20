using System;

namespace SharpNeatLib.NeuralNetwork
{
	public class InverseAbsoluteSigmoid : IActivationFunction
	{
		public double Calculate(double inputSignal)
		{
			//return 1.0+(inputSignal/(0.1+Math.Abs(inputSignal)));
			return 0.5 + ((inputSignal / (1.0+Math.Abs(inputSignal)))*0.5);
		}

		public float Calculate(float inputSignal)
		{
			
			//return 1.0F+(inputSignal/(0.1F+Math.Abs(inputSignal)));
			return 0.5F + ((inputSignal / (1.0F+Math.Abs(inputSignal)))*0.5F);
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
