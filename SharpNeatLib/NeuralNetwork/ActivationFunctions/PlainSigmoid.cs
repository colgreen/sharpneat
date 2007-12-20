using System;

namespace SharpNeatLib.NeuralNetwork
{
	public class PlainSigmoid : IActivationFunction
	{
		public double Calculate(double inputSignal)
		{
			// good for x input range -5.0->5.0 (y 0.0->1.0)
			return 1.0D/(1.0D+(Math.Exp(-inputSignal)));  
		}

		public float Calculate(float inputSignal)
		{
			// good for x input range -5.0->5.0 (y 0.0->1.0)
			return 1.0F/(1.0F+((float)Math.Exp(-inputSignal)));  
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
				return "1.0/(1.0+(exp(-inputSignal)))";
			}
		}


		/// <summary>
		/// A human readable / verbose description of the activation function.
		/// </summary>
		public string FunctionDescription
		{
			get
			{
				return "Plain sigmoid [xrange -5.0,5.0][yrange, 0.0,1.0]";
			}
		}
	}
}
