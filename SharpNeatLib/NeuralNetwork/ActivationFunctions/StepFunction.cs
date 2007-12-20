using System;

namespace SharpNeatLib.NeuralNetwork
{
	public class StepFunction : IActivationFunction
	{
		public double Calculate(double inputSignal)
		{
			if(inputSignal<0.0F)
				return 0.0;
			else
				return 1.0;
		}

		public float Calculate(float inputSignal)
		{
			if(inputSignal<0F)
				return 0F;
			else
				return 1F;
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
				return "x<0 ? 0.0 : 1.0";
			}
		}


		/// <summary>
		/// A human readable / verbose description of the activation function.
		/// </summary>
		public string FunctionDescription
		{
			get
			{
				return "Step function [xrange -5.0,5.0][yrange, 0.0,1.0]";
			}
		}
	}
}
